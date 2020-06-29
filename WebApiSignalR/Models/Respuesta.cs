using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiSignalR.Models
{
    public enum eRespuestas
    {
        OK = 0,
        KO = 1,
        Unauthorized = 2,
        InternalServerError = 3,
        BDExiste = 4,
        ErrorCreacionBD = 5
    }

    public class RespuestaAPI<T>
    {

        public T resultado;

        public string respuesta;

        public static string nombreRespuesta(eRespuestas resp)
        {
            return Enum.GetName(resp.GetType(), resp);
        }

    }
}