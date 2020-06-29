using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using WebApiSignalR.Models;

namespace WebApiSignalR.Controllers
{

    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("echoping")]
        public IHttpActionResult EchoPing()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }

        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(LoginRequest login)
        {
            if (login == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);


            if (isCredentialValid(login.Username, login.Password))
            {
                var token = TokenGenerator.GenerateTokenJwt(login.Username);
                return Ok(new RespuestaAPI<string>() { respuesta = RespuestaAPI<string>.nombreRespuesta(eRespuestas.OK), resultado = token });
            }
            else
            {
                return Ok(new RespuestaAPI<string>() { respuesta = RespuestaAPI<string>.nombreRespuesta(eRespuestas.Unauthorized), resultado = "" });
            }
        }

        //TODO use a secure validation, this is only for demo
        private bool isCredentialValid(string usuario, string pass)
        {
            return !string.IsNullOrEmpty(usuario) && pass == "123456";
        }
    }
}
