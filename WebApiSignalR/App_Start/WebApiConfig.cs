using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApiSignalR.Controllers;
using WebApiSignalR.Helpers;

namespace WebApiSignalR
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web
            //habilitamos CORS
            //var cors = new EnableCorsAttribute("http://localhost:8080", "*", "*");
            config.EnableCors();

            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new TokenValidationHandler());
          
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //SignalR
            Global.SignalRMessage = CustomHub.Send;
            Global.SignalRMessageAll = CustomHub.SendAll;
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
        }

        public class Global
        {
            public delegate void DelSignalRMessage(string name, string data, string origen = "server");
            public delegate void DelSignalRMessageAll(string data, string origen = "server");
            public static DelSignalRMessage SignalRMessage;
            public static DelSignalRMessageAll SignalRMessageAll;
        }
    }
}
