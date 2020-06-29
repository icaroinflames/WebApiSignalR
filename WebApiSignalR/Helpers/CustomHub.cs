using Microsoft.AspNet.SignalR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebApiSignalR.Controllers;

namespace WebApiSignalR.Helpers
{
    /**
     * This custom hut uses SignalR Groups to identify all connections from a user
     */
    public class CustomHub:Hub
    {
        public override Task OnConnected()
        {
            
            var claims = GetClaim();
            if(claims != null) this.Groups.Add(this.Context.ConnectionId, claims.Identity.Name);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            var claims = GetClaim();
            if (claims != null) this.Groups.Remove(this.Context.ConnectionId, claims.Identity.Name);

            return base.OnDisconnected(stopCalled);
        }

        public static void Send(string name, string data, string origen = "server")
        {           
            try
            {
                var conexion = GlobalHost.ConnectionManager.GetHubContext<CustomHub>();
                if(conexion != null)
                {
                    conexion.Clients.Group(name).mostrarMensaje(origen, data);//llama al método "mostrarMensaje" de los clientes conectados
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public ClaimsPrincipal GetClaim()
        {
            try
            {
                var token = this.Context.QueryString["token"];
                var validationHandler = new TokenValidationHandler();
                var claims = validationHandler.getClaimData(token);

                return validationHandler.getClaimData(token);
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}