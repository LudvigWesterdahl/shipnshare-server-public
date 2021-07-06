using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.Entities;
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
    public sealed class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> logger;
        private readonly ServerInfo serverInfo;
        private readonly ICreatePostUseCase createPostUseCase;
        private readonly IGetPostsUseCase getPostsUseCase;
        private readonly IReportPostUseCase reportPostUseCase;
        private readonly IGetReportedPostsUseCase getReportedPostsUseCase;
        private readonly IRequestToChatUseCase requestToChatUseCase;
        private readonly IGetChatsUseCase getChatsUseCase;
        private readonly IClosePostUseCase closePostUseCase;

        public PostsController(
            ILogger<PostsController> logger,
            ServerInfo serverInfo,
            ICreatePostUseCase createPostUseCase,
            IGetPostsUseCase getPostsUseCase,
            IReportPostUseCase reportPostUseCase,
            IGetReportedPostsUseCase getReportedPostsUseCase,
            IRequestToChatUseCase requestToChatUseCase,
            IGetChatsUseCase getChatsUseCase,
            IClosePostUseCase closePostUseCase)
        {
            this.logger = logger;
            this.serverInfo = serverInfo;
            this.createPostUseCase = createPostUseCase;
            this.getPostsUseCase = getPostsUseCase;
            this.reportPostUseCase = reportPostUseCase;
            this.getReportedPostsUseCase = getReportedPostsUseCase;
            this.requestToChatUseCase = requestToChatUseCase;
            this.getChatsUseCase = getChatsUseCase;
            this.closePostUseCase = closePostUseCase;
        }

        /// <summary>
        /// Returns the user id of this authenticated user.
        /// </summary>
        private long UserId
        {
            get
            {
                return AuthenticationHelper.GetUserId(User);
            }
        }

        /// <summary>
        /// Returns the email of this authenticated user.
        /// </summary>
        private string UserEmail
        {
            get
            {
                return AuthenticationHelper.GetUserEmail(User);
            }
        }

        /// <summary>
        /// Returns the user name of this authenticated user.
        /// </summary>
        private string UserName
        {
            get
            {
                return AuthenticationHelper.GetUserName(User);
            }
        }

        /// <summary>
        /// Returns all posts with distance calculated from the user location.
        /// </summary>
        /// <param name="userLocation">the user location</param>
        /// <returns>the posts with distance attached</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistanceToPostResponse>>> Get(
            [FromQuery] Coordinates userLocation, [FromQuery] double? maxDistance)
        {
            logger.LogTrace("userLocation = {}", userLocation);

            var distanceToPosts = maxDistance.HasValue
                ? await getPostsUseCase.GetAll(userLocation.Latitude.Value, userLocation.Longitude.Value, maxDistance.Value)
                : await getPostsUseCase.GetAll(userLocation.Latitude.Value, userLocation.Longitude.Value);

            var response = distanceToPosts.Select(dto =>
                new DistanceToPostResponse(dto.Post, dto.MetersToPickupLocation))
                .ToList();

            return response;
        }

        /// <summary>
        /// Returns all posts created by the user.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>the posts created by the user</returns>
        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PostResponse>>> Get(
            long userId,
            [FromQuery] bool includeClosed = false)
        {
            logger.LogInformation("User {UserId}/{UserName} requested for {UserId}", UserId, UserName, userId);

            var posts = await getPostsUseCase.GetByUserId(userId, !includeClosed);

            var responses = posts.Select(p => new PostResponse(p)).ToList();

            return responses;
        }

        [AllowAnonymous]
        [HttpGet("{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostResponse>> GetByPostId(string postId)
        {
            logger.LogTrace("postId = {}", postId);

            var post = await getPostsUseCase.GetById(postId);

            if (post == null)
            {
                return NotFound();
            }

            return new PostResponse(post);
        }

        private string SaveImageToUploads(string image, string name, string imageType)
        {
            var filePath = Path.Combine(serverInfo.WwwRootPath, "uploads", $"{name}.{imageType}");
            var file = Convert.FromBase64String(image);
            System.IO.File.WriteAllBytes(filePath, file);

            return Path.GetRelativePath(serverInfo.WwwRootPath, filePath);
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> Create(CreatePost model)
        {
            var owner = new UserEntity(UserId, UserEmail, UserName);

            if (model.ImageType != null && model.Image != null)
            {
                var imageName = Guid.NewGuid().ToString("N");
                var imagePath = SaveImageToUploads(model.Image, imageName, model.ImageType);
                var createdPost = await createPostUseCase.Create(model.ToGeneralPostInfo(owner, imagePath));

                return CreatedAtAction(
                    nameof(GetByPostId),
                    new { postId = createdPost.Id },
                    new PostResponse(createdPost));
            }
            else
            {
                var createdPost = await createPostUseCase.Create(model.ToGeneralPostInfo(owner));

                return CreatedAtAction(
                    nameof(GetByPostId),
                    new { postId = createdPost.Id },
                    new PostResponse(createdPost));
            }
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpPost("close/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Close(string postId)
        {
            var success = await closePostUseCase.Close(postId, UserId);

            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


        //[Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [AllowAnonymous]
        [HttpGet("timezones")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<string>> GetValidTimeZones()
        {
            
            var bytes = System.IO.File.ReadAllBytes(Path.Combine(serverInfo.WwwRootPath, "uploads", "indecode-icon.png"));

            return Ok(TimeZoneInfo.GetSystemTimeZones().Select(tz => tz.Id));

            // TODO: Need to allow sender to send images
            // try then with this one: https://codebeautify.org/image-to-base64-converter
            // with some simple image that is very small.
            // Check if we can automatically detect if its a png, jpg, jpeg.
            /*
            return Ok(new
            {
                Image = Convert.ToBase64String(bytes)
            });
            */
            /*
            return Ok(new
            {
                Success = "OK",
                Path = serverInfo.WwwRootPath
            });
            */
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("{postId}/report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReportedPostResponse>>> GetReportedPosts(string postId)
        {
            var reportedPostEntites = await getReportedPostsUseCase.Get(postId);

            var reportedPostResponses = reportedPostEntites.Select(rp => new ReportedPostResponse {
                PostId = rp.Post.Id,
                UserEmail = rp.User.Email,
                CreatedAt = DateTimeUtils.ToString(rp.CreatedAt),
                Message = rp.Message
            }).ToList();

            return reportedPostResponses;
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpPost("{postId}/report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ReportPost(string postId, [FromBody] ReportPostInfo reportPostInfo)
        {
            var res = await reportPostUseCase.Report(postId, UserId, reportPostInfo.Message);

            if (res)
            {
                return Ok();
            }
            else
            {
                return BadRequest(BadRequestResponse.PostAlreadyReportedByUser());
            }
        }
    }
}
