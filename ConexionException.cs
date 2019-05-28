using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ConexionesBD
{
    /// <summary>
    /// 
    /// </summary>
    public class ConexionException : Exception
    {        
        /// <summary>
        /// Obtiene la consulta se trato de ejecutar.
        /// </summary>
        public String MessageSQL { get; set; }
        /// <summary>
        /// Obtiene el nivel de gravedad del error que devuelve el proveedor de datos de .NET Framework para SQL Server.
        /// </summary>
        public Byte Class { get; set; }
        /// <summary>
        /// Obtiene HRESULT del error.
        /// </summary>
        public Int32 ErrorCode { get; set; }
        /// <summary>
        /// Obtiene una colección de uno o varios objetos System.Data.SqlClient.SqlError que proporcionan información detallada sobre las excepciones que genera el proveedor de datos .NET Framework para SQL Server.
        /// </summary>
        public SqlErrorCollection Errors{ get; set;}
        /// <summary>
        /// Obtiene el numero de linea del procedimiento almacenado o el lote de comandos de Transact-SQL que ha generado el error.
        /// </summary>
        public Int32 LineNumber { get; set; }
        /// <summary>
        /// Obtiene el nombre del procedimiento almacenado o llamada a procedimiento remoto (RPC) que ha generado el error.
        /// </summary>
        public String Procedure { get; set; }
        /// <summary>
        /// Obtiene el nombre del equipo que ejecuta la instancia de SQL que ha generado el error.
        /// </summary>
        public String Server { get; set; }
        /// <summary>
        /// Obtiene el código de error númerico de SQL Server que representa un error o un mensaje.
        /// </summary>
        public Byte State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ConexionException(String message):base(message)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ConexionException(String message, System.Data.SqlClient.SqlException innerException) : base(message, innerException)
        {            
            this.MessageSQL = message;            
            this.Class = innerException.Class;
            this.ErrorCode = innerException.ErrorCode;
            this.Errors = innerException.Errors;
            this.LineNumber = innerException.LineNumber;
            this.Procedure = innerException.Procedure;
            this.Server = innerException.Server;
            this.State = innerException.State;                                                 
        }
    }
}
