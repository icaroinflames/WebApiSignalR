using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiSignalR.Models
{
    public class NotificationRequest
    {
        public string Username { get; set; }
        public string Message { get; set; }
    }
}