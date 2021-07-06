using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.SharedKernel;
using ShipWithMeCore.UseCases;
using ShipWithMeWeb.Authentication;
using ShipWithMeWeb.RequestInputs;
using ShipWithMeWeb.Responses;

namespace ShipWithMeWeb.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public sealed class ChatsController : ControllerBase
    {
        private readonly ILogger<ChatsController> logger;
        private readonly IGetChatsUseCase getChatsUseCase;
        private readonly IGetChatRequestsUseCase getChatRequestsUseCase;
        private readonly IAnswerChatRequestUseCase answerChatRequestUseCase;
        private readonly IRequestToChatUseCase requestToChatUseCase;
        private readonly ISendMessageToChatUseCase sendMessageToChatUseCase;
        private readonly ILeaveChatUseCase leaveChatUseCase;
        private readonly IGetPostsUseCase getPostsUseCase;

        public ChatsController(
            ILogger<ChatsController> logger,
            IGetChatsUseCase getChatsUseCase,
            IGetChatRequestsUseCase getChatRequestsUseCase,
            IAnswerChatRequestUseCase answerChatRequestUseCase,
            IRequestToChatUseCase requestToChatUseCase,
            ISendMessageToChatUseCase sendMessageToChatUseCase,
            ILeaveChatUseCase leaveChatUseCase,
            IGetPostsUseCase getPostsUseCase)
        {
            this.logger = logger;
            this.getChatsUseCase = getChatsUseCase;
            this.getChatRequestsUseCase = getChatRequestsUseCase;
            this.answerChatRequestUseCase = answerChatRequestUseCase;
            this.requestToChatUseCase = requestToChatUseCase;
            this.sendMessageToChatUseCase = sendMessageToChatUseCase;
            this.leaveChatUseCase = leaveChatUseCase;
            this.getPostsUseCase = getPostsUseCase;
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

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChats([FromQuery(Name = "messagescount")] int messagesCount = -1)
        {
            var chats = await getChatsUseCase.GetAll(UserId);

            var chatResponses = chats
                .Select(c => ChatResponse.NewChatResponse(c))
                .Select(cr =>
                {
                    cr.Messages = cr.Messages
                        .Skip(messagesCount < 0 ? 0 : cr.Messages.Count - messagesCount)
                        .ToList();

                    return cr;
                })
                .ToList();

            return Ok(chatResponses);
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("{chatId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChat(
            string chatId,
            [FromQuery(Name = "messagescount")] int messagesCount = -1)
        {
            var chat = await getChatsUseCase.GetById(chatId, UserId);

            if (chat == null)
            {
                return BadRequest();
            }

            var chatResponse = ChatResponse.NewChatResponse(chat);

            chatResponse.Messages = chatResponse.Messages
                        .Skip(messagesCount < 0 ? 0 : chatResponse.Messages.Count - messagesCount)
                        .ToList();

            return Ok(chatResponse);
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("{chatId}/post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChatPost(string chatId)
        {
            var chat = await getChatsUseCase.GetById(chatId, UserId);

            if (chat == null)
            {
                return BadRequest();
            }

            var post = await getPostsUseCase.GetById(chat.Post.Id);

            Validate.That(post, nameof(post)).IsNot(null);

            return Ok(new PostResponse(post));
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOpenChatRequests()
        {
            var chatRequests = await getChatRequestsUseCase.GetAll(UserId, true);

            var chatRequestResponses = chatRequests
                .Select(cr => ChatRequestResponse.NewChatRequestResponse(cr))
                .ToList();

            return Ok(chatRequestResponses);
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("requests/sent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSentChatRequests()
        {
            logger.LogInformation("requests/sent with user: {UserId}", UserId);
            var chatRequests = (await getChatRequestsUseCase.GetSent(UserId, false)).ToList();

            var chatRequestResponses = chatRequests
                .Select(cr => ChatRequestResponse.NewChatRequestResponse(cr))
                .ToList();

            return Ok(chatRequestResponses);
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("requests/sent/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSendChatRequestForPost(string postId)
        {
            logger.LogInformation("requests/sent/postId with user: {UserId}", UserId);
            var chatRequests = (await getChatRequestsUseCase.GetSent(UserId, false)).ToList();

            var chatRequestResponse = chatRequests
                .Where(cr => cr.Chat.Post.Id.Equals(postId))
                .Select(cr => ChatRequestResponse.NewChatRequestResponse(cr))
                .FirstOrDefault();

            if (chatRequestResponse == null)
            {
                return NoContent();
            }

            return Ok(chatRequestResponse);
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpPost("requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AnswerChatRequest(AnswerChatRequestInfo answerChatRequestInfo)
        {
            var success = await answerChatRequestUseCase.Answer(
                answerChatRequestInfo.ChatRequestId, UserId, answerChatRequestInfo.Accept);

            if (success)
            {
                var chatRequests = await getChatRequestsUseCase.GetReceived(UserId, false);

                var chatRequest = chatRequests
                        .Where(cr => cr.Id.Equals(answerChatRequestInfo.ChatRequestId))
                        .First();

                return Ok(AnswerChatRequestResponse.NewAnswerChatRequestResponse(chatRequest));
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpPost("post/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestChatWithPostOwner(string postId)
        {
            var chatRequest = await requestToChatUseCase.Request(postId, UserId);
            if (chatRequest != null)
            {
                return Ok(ChatRequestResponse.NewChatRequestResponse(chatRequest));
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("post/{postid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChatWithPostOwner(
            string postId,
            [FromQuery(Name = "messagescount")] int messagesCount = 100)
        {
            var chats = await getChatsUseCase.GetAll(UserId);
            var chat = chats.Where(c => c.Post.Id.Equals(postId)).FirstOrDefault();

            if (chat != null)
            {
                var chatResponse = ChatResponse.NewChatResponse(chat);
                chatResponse.Messages = chatResponse.Messages
                    .Skip(chatResponse.Messages.Count - messagesCount)
                    .ToList();

                return Ok(chatResponse);
            }
            else
            {
                return BadRequest(BadRequestResponse.ChatWithPostOwnerDoesNotExist());
            }
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpPost("message")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendMessage(ChatMessageInfo chatMessageInfo)
        {
            var updatedChat = await sendMessageToChatUseCase.Send(
                chatMessageInfo.ChatId, UserId, chatMessageInfo.Message);

            if (updatedChat != null)
            {
                return CreatedAtAction(
                    nameof(GetChatWithPostOwner),
                    new { postId = updatedChat.Post.Id },
                    new { });
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpPost("leave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LeaveChat(LeaveChatInfo leaveChatInfo)
        {
            var success = await leaveChatUseCase.Leave(leaveChatInfo.ChatId, UserId);

            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
