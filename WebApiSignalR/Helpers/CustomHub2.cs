using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebApiSignalR.Helpers
{
    /**
     * Este CustomHub2 ofrece una alternativa a CustomHub en la que las conexiones del usuario se administran en una lista en vez de usar los grupos
     */
    public class CustomHub2:Hub
    {
        private static readonly IDictionary<String, ISet<String>> users = new ConcurrentDictionary<String, ISet<String>>();

        public override Task OnConnected()
        {           
            AddUser(this.Context.Request.User.Identity.Name, this.Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            RemoveUser(this.Context.Request.User.Identity.Name, this.Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public static void Send(string name, string data)
        {           
            try
            {
                var conexion = GlobalHost.ConnectionManager.GetHubContext<CustomHub2>();
                if(conexion != null)
                {
                    foreach(String connectionId in CustomHub2.GetUserConnections(name))
                    {
                        conexion.Clients.Client(connectionId).mostrarMensaje(name, data);//llama al método "mostrarMensaje" de los clientes conectados
                    }
                    
                }
            }
            catch(Exception e)
            {
                var a = e.Message;
            }
        }

        public static IEnumerable<String> GetUserConnections(String username)
        {
            ISet<String> connections;
            users.TryGetValue(username, out connections);
            return connections ?? Enumerable.Empty<String>();
        }

        private static void AddUser(String username, String connectionId)
        {
            ISet<String> connections;
            if(!users.TryGetValue(username, out connections))
            {
                connections = users[username] = new HashSet<String>();
            }

            connections.Add(connectionId);
        }

        private static void RemoveUser(String username, String connectionId)
        {
            users[username].Remove(connectionId);
        }

    }
}