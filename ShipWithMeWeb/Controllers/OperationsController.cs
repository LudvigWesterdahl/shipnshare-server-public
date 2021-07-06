using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class OperationsController : ControllerBase
    {
        private readonly ILogger<OperationsController> logger;
        private readonly ICreateInternalNotificationUseCase createInternalNotificationUseCase;
        private readonly IGetInternalNotificationsUseCase getInternalNotificationsUseCase;

        public OperationsController(
            ILogger<OperationsController> logger,
            ICreateInternalNotificationUseCase createInternalNotificationUseCase,
            IGetInternalNotificationsUseCase getInternalNotificationsUseCase)
        {
            this.logger = logger;
            this.createInternalNotificationUseCase = createInternalNotificationUseCase;
            this.getInternalNotificationsUseCase = getInternalNotificationsUseCase;
        }

        [Authorize(Policy = AuthenticationHelper.AdminRights)]
        [HttpPost("internalnotification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendInternalNotification([FromBody] IEnumerable<InternalNotificationInfo> notifications)
        {

            var languageCodeMessages = notifications.ToDictionary(ini => ini.LanguageCode, ini => ini.Message);
            var success = await createInternalNotificationUseCase.Create(languageCodeMessages);

            if (success)
            {
                return Ok(notifications);
            }
            else
            {
                logger.LogWarning("Failed to create notifications.");
                return BadRequest();
            }
        }

        [Authorize(Policy = AuthenticationHelper.CustomerRights)]
        [HttpGet("internalnotification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InternalNotificationResponse>>> GetInternalNotifications(
            [QueryDate, FromQuery(Name = "activedatetime")] string dateTimeQueryString)
        {
            var dateTime = DateTimeUtils.Parse(dateTimeQueryString, format: DateTimeUtils.QueryFormat);

            var internalNotifications = await getInternalNotificationsUseCase.GetForDateTime(dateTime);

            var response = internalNotifications.Select(i => new InternalNotificationResponse
            {
                CreatedAt = DateTimeUtils.ToString(i.CreatedAt),
                LanguageCodeMessages = i.LanguageCodeMessages
            });

            return response.ToList();
        }
    }
}
