using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.Entities;
using ShipWithMeCore.ExternalServices;
using ShipWithMeCore.SharedKernel;
using ShipWithMeCore.UseCases;
using ShipWithMeWeb.Authentication;
using ShipWithMeWeb.Helpers;
using ShipWithMeWeb.RequestInputs;
using ShipWithMeWeb.Responses;

namespace ShipWithMeWeb.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public sealed class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly ServerInfo serverInfo;
        private readonly AuthenticationHelper authenticationHelper;
        private readonly IEmailService emailService;
        private readonly IReviewUserUseCase reviewUserUseCase;
        private readonly IGetUserReviewsUseCase getUserReviewsUseCase;
        private readonly IGetUserUseCase getUserUseCase;

        public UsersController(
            ILogger<UsersController> logger,
            ServerInfo serverInfo,
            AuthenticationHelper authenticationHelper,
            IEmailService emailService,
            IReviewUserUseCase reviewUserUseCase,
            IGetUserReviewsUseCase getUserReviewsUseCase,
            IGetUserUseCase getUserUseCase)
        {
            this.logger = logger;
            this.serverInfo = serverInfo;
            this.authenticationHelper = authenticationHelper;
            this.emailService = emailService;
            this.reviewUserUseCase = reviewUserUseCase;
            this.getUserReviewsUseCase = getUserReviewsUseCase;
            this.getUserUseCase = getUserUseCase;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationInfo model)
        {
            var token = await authenticationHelper.Authenticate(model.Email, model.Password);

            if (token == null)
            {
                logger.LogInformation($"Failed to authenticate {model.Email}");
                return BadRequest(new { Message = $"Failed to authenticate {model.Email}" });
            }

            return Ok(new
            {
                Token = token.Item1,
                TokenExpiration = DateTimeUtils.ToString(token.Item2),
                RefreshToken = token.Item3,
                RefreshTokenExpiration = DateTimeUtils.ToString(token.Item4),
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh_authenticate")]
        public async Task<IActionResult> RefreshAuthenticate([FromBody] RefreshAuthenticationInfo model)
        {
            var token = await authenticationHelper.RefreshAuthentication(model.RefreshToken);

            if (token == null)
            {
                logger.LogInformation($"Failed to refresh token {model.RefreshToken}");
                return BadRequest(new { Message = $"Failed to refresh token {model.RefreshToken}" });
            }

            return Ok(new
            {
                Token = token.Item1,
                TokenExpiration = DateTimeUtils.ToString(token.Item2),
                RefreshToken = token.Item3,
                RefreshTokenExpiration = DateTimeUtils.ToString(token.Item4),
            });
        }

        [Authorize(Policy = AuthenticationHelper.AdminRights)]
        [HttpPost("block")]
        public async Task<ActionResult<IEnumerable<UserBlockResponse>>> BlockUser(
            [FromBody] UserBlockInfo userBlockInfo)
        {
            var currentUserBlocks = await authenticationHelper.BlockUser(userBlockInfo);

            var userBlockResponses = currentUserBlocks.Select(bu => new UserBlockResponse
            {
                Id = bu.Id,
                Version = bu.Version,
                UserEmail = bu.User.Email,
                Reason = bu.Reason,
                From = DateTimeUtils.ToString(bu.From),
                To = DateTimeUtils.ToString(bu.To),
                Active = bu.Active
            });

            return userBlockResponses.ToList();
        }

        [Authorize(Policy = AuthenticationHelper.AdminRights)]
        [HttpPost("unblock")]
        public async Task<ActionResult<IEnumerable<UserBlockResponse>>> UnblockUser(
            [FromBody] UserUnblockInfo userUnblockInfo)
        {
            var currentUserBlocks = await authenticationHelper.UnblockUser(userUnblockInfo);

            var userBlockResponses = currentUserBlocks.Select(bu => new UserBlockResponse
            {
                Id = bu.Id,
                Version = bu.Version,
                UserEmail = bu.User.Email,
                Reason = bu.Reason,
                From = DateTimeUtils.ToString(bu.From),
                To = DateTimeUtils.ToString(bu.To),
                Active = bu.Active
            });

            return userBlockResponses.ToList();
        }

        [Authorize(Policy = AuthenticationHelper.AdminRights)]
        [HttpPost("isblocked")]
        public async Task<ActionResult<UserBlocksResponse>> IsBlocked(
            [FromBody] UserEmail userEmail)
        {
            var currentUserBlocks = await authenticationHelper.GetUserBlocks(userEmail.Email);

            var userBlockResponses = currentUserBlocks.Select(bu => new UserBlockResponse
            {
                Id = bu.Id,
                Version = bu.Version,
                UserEmail = bu.User.Email,
                Reason = bu.Reason,
                From = DateTimeUtils.ToString(bu.From),
                To = DateTimeUtils.ToString(bu.To),
                Active = bu.Active
            });

            var blocked = await authenticationHelper.IsBlocked(userEmail.Email);

            return new UserBlocksResponse
            {
                Blocked = blocked,
                CurrentDateTime = DateTimeUtils.ToString(DateTime.UtcNow),
                UserBlocks = userBlockResponses,
            };
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserInfo([FromQuery] string userName)
        {
            var userInfoResponse = new UserInfoResponse
            {
                UserId = AuthenticationHelper.GetUserId(User),
                UserName = AuthenticationHelper.GetUserName(User)
            };

            if (userName != null)
            {
                try
                {
                    var requestedUser = await getUserUseCase.GetByUserName(userName);
                    if (requestedUser != null)
                    {
                        userInfoResponse.HasRequestedUserInfo = true;
                        userInfoResponse.RequestedUserId = requestedUser.Id;
                        userInfoResponse.RequestedUserName = requestedUser.UserName;
                    }
                }
                catch
                {
                    return BadRequest();
                }

            }

            return Ok(userInfoResponse);
        }

        private async Task<IList<UserReviewEntity>> GetUserReviewsHelper(string userName, long userId)
        {
            if (userId != 0)
            {
                return (await getUserReviewsUseCase.GetReceived(userId)).ToList();
            }
            else if (userName != null)
            {
                var user = await getUserUseCase.GetByUserName(userName);

                if (user == null)
                {
                    return new List<UserReviewEntity>();
                }

                
                return (await getUserReviewsUseCase.GetReceived(user.Id)).ToList();
            }
            else
            {
                return (await getUserReviewsUseCase.GetReceived(AuthenticationHelper.GetUserId(User))).ToList();
            }
        }

        [AllowAnonymous]
        [HttpGet("reviews")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserReviews(
            [FromQuery] string userName,
            [FromQuery] long userId,
            [FromQuery] int reviewsCount = -1)
        {
            var userReviews = (await GetUserReviewsHelper(userName, userId)).ToList();

            double averageRating = 0;
            if (userReviews.Count != 0)
            {
                averageRating = userReviews.Average(ur => ur.Rating);
            }

            var userReviewResponses = userReviews
                .OrderBy(ur => ur.CreatedAt)
                .Skip(reviewsCount < 0 ? 0 : userReviews.Count - reviewsCount)
                .Select(ur => UserReviewResponse.NewUserReviewResponse(ur))
                .ToList();

            var userReviewsResponse = new UserReviewsResponse
            {
                AverageRating = averageRating,
                NumberOfReviews = userReviews.Count,
                NumberOfGoodReviews = userReviews.Where(ur => ur.Rating == 3).Count(),
                NumberOfGoodButReviews = userReviews.Where(ur => ur.Rating == 2).Count(),
                NumberOfBadButReviews = userReviews.Where(ur => ur.Rating == 1).Count(),
                NumberOfBadReviews = userReviews.Where(ur => ur.Rating == 0).Count(),
                UserReviews = userReviewResponses
            };

            return Ok(userReviewsResponse);
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("canreview/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CanReviewUser(string postId)
        {
            var canReview = await reviewUserUseCase.CanReview(postId, AuthenticationHelper.GetUserId(User));

            var canReviewResponse = new CanReviewResponse
            {
                CanReview = canReview
            };

            return Ok(canReviewResponse);
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpPost("reviews")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ReviewUser(ReviewUserInfo reviewUserInfo)
        {
            var success = await reviewUserUseCase.Review(
                reviewUserInfo.PostId,
                AuthenticationHelper.GetUserId(User),
                reviewUserInfo.Rating,
                reviewUserInfo.Message);

            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> SendConfirmEmailLink([FromBody] CreateUser createUser)
        {
            var userByEmail = await getUserUseCase.GetByEmail(createUser.Email);

            if (userByEmail != null)
            {
                return BadRequest(BadRequestResponse.EmailAddressAlreadyUsed());
            }

            var userByUserName = await getUserUseCase.GetByUserName(createUser.UserName);

            if (userByUserName != null)
            {
                return BadRequest(BadRequestResponse.UserNameAlreadyUsed());
            }

            var userIdToken = authenticationHelper.RegisterAccount(createUser);

            if (userIdToken != null)
            {
                var confirmEmailLink = Url.Page("/ConfirmEmail",
                    pageHandler: null,
                    values: new {
                        userid = userIdToken.Item1,
                        token = userIdToken.Item2
                    },
                    protocol: Request.Scheme,
                    host: serverInfo.Hostname);

                emailService.Send("ShipnShare confirm email",
                    $"<p>Welcome {createUser.UserName}!</p>"
                    + "<p>Please confirm your email by clicking the following link:</p>"
                    + confirmEmailLink,
                    createUser.Email)
                .GetAwaiter().GetResult();

                return Ok();
            }
            else
            {
                return BadRequest(BadRequestResponse.BadPassword());
            }
        }

        [AllowAnonymous]
        [HttpPost("resetpassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendResetPasswordLink([FromBody] UserEmail userEmail)
        {
            logger.LogTrace("SendResetPasswordLink");

            var user = await getUserUseCase.GetByEmail(userEmail.Email);

            if (user == null)
            {
                return BadRequest();
            }

            var token = authenticationHelper.GenerateResetPasswordToken(user.Id);

            var resetPasswordLink = Url.Page("/ResetPassword",
                pageHandler: null,
                values: new
                {
                    userid = user.Id,
                    token = token
                },
                protocol: Request.Scheme,
                host: serverInfo.Hostname);

            Console.WriteLine($"Link is: {resetPasswordLink}");

            await emailService.Send("ShipnShare Password Reset",
                $"<p>Reset your password by clicking the following link:</p><p>{resetPasswordLink}</p>",
                user.Email);

            return Ok();
        }
    }
}
