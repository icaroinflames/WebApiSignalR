﻿using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebApiSignalR.Models;

namespace WebApiSignalR.Controllers
{
    internal class TokenValidationHandler : DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token;

            HttpResponseMessage respuesta = new HttpResponseMessage();
            Models.RespuestaAPI<string> respuestaApi = new Models.RespuestaAPI<string>();

            // determine whether a jwt exists or not
            if (!TryRetrieveToken(request, out token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                return base.SendAsync(request, cancellationToken);
            }

            try
            {
                
                // Extract and assign Current Principal and user
                Thread.CurrentPrincipal = getClaimData(token);
                HttpContext.Current.User = getClaimData(token);

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                respuestaApi = new RespuestaAPI<string>() { respuesta = RespuestaAPI<string>.nombreRespuesta(eRespuestas.Unauthorized), resultado = "" };
            }
            catch (Exception)
            {
                statusCode = HttpStatusCode.InternalServerError;
                respuestaApi = new RespuestaAPI<string>() { respuesta = RespuestaAPI<string>.nombreRespuesta(eRespuestas.InternalServerError), resultado = "" };
            }

            //Devolvemos algún error en la respuesta
            var jsonString = JsonConvert.SerializeObject(respuestaApi);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            respuesta.Content = content;
            respuesta.StatusCode = statusCode;

            return Task<HttpResponseMessage>.Factory.StartNew(() => respuesta);
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }

        public ClaimsPrincipal getClaimData(string token)
        {
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));

            SecurityToken securityToken;
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidAudience = audienceToken,
                ValidIssuer = issuerToken,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                LifetimeValidator = this.LifetimeValidator,
                IssuerSigningKey = securityKey
            };

            // Extract and assign Current Principal and user
            return tokenHandler.ValidateToken(token, validationParameters, out securityToken);
        }
    }
}