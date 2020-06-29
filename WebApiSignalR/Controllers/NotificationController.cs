using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiSignalR.Models;

namespace WebApiSignalR.Controllers
{
    [Authorize]
    [RoutePrefix("api/notificaciones")]
    public class NotificationController : ApiController
    {
        [HttpPost]
        [Route("notificar")]
        public IHttpActionResult notifyClient(NotificationRequest notificationRequest)
        {
            WebApiConfig.Global.SignalRMessage(notificationRequest.Username, notificationRequest.Message, notificationRequest.Origin);
            return Ok();
        }
    }
}
