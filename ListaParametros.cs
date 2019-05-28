using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ConexionesBD
{
    /// <summary>
    /// Esta clase permite crear una lista de parametros que puede ser usada en la clase CConexion
    /// </summary>
    public class ListaParametros
    {
        private List<SqlParameter> _ListaParametros;
        /// <summary>
        /// Crea una lista de parametros de sql para utilizar en la clase CConexion
        /// </summary>
        public ListaParametros()
        {
            this._ListaParametros = new List<SqlParameter>();
        }
        /// <summary>
        /// Agrega un Parametro a la lista especificando su nombre y su tipo.
        /// </summary>
        /// <param name="Nombre"> Nombre del parametro. Ejem. @Parametro1 </param>
        /// <param name="Tipo"> Valor de la enumeracion de SqlDbType </param>
        public void Agregar(String Nombre, SqlDbType Tipo)
        {
            this.Agregar(Nombre, Tipo, 0, "", ParameterDirection.Input);
        }              
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nombre"> Nombre del parametro. Ejem. @Parametro1 </param>
        /// <param name="Tipo"> Tipo de dato para el parametro </param>
        /// <param name="Valor"> Valor a asignar al parametro </param>
        public void Agregar(String Nombre, SqlDbType Tipo, Object Valor)
        {
            this.Agregar(Nombre, Tipo, 0, Valor, ParameterDirection.Input);
        }       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nombre"> Nombre del parametro. Ejem. @Parametro1 </param>
        /// <param name="Tipo"> Tipo de dato para el parametro </param>
        /// <param name="Direccion"> Tipo de parametro, Output, Input, OutputInput </param>
        public void Agregar(String Nombre, SqlDbType Tipo, ParameterDirection Direccion)
        {
            this.Agregar(Nombre, Tipo, 0, "", Direccion);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nombre"> Nombre del parametro. Ejem. @Parametro1 </param>
        /// <param name="Tipo"> Tipo de dato para el parametro </param>
        /// <param name="Longitud"> Longitud del tipo de dato del parametro </param>
        /// <param name="Valor"> Valor a asignar al parametro </param>
        public void Agregar(String Nombre, SqlDbType Tipo, Int32 Longitud, Object Valor)
        {
            this.Agregar(Nombre, Tipo, Longitud, Valor, ParameterDirection.Input);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nombre"> Nombre del parametro. Ejem. @Parametro1 </param>
        /// <param name="Tipo"> Tipo de dato para el parametro </param>
        /// <param name="Longitud"> Longitud del tipo de dato del parametro </param>
        /// <param name="Direccion"> Tipo de parametro, Output, Input, OutputInput </param>
        public void Agregar(String Nombre, SqlDbType Tipo, Int32 Longitud, ParameterDirection Direccion)
        {
            if (Tipo == SqlDbType.Char || Tipo == SqlDbType.NChar || Tipo == SqlDbType.NText || Tipo == SqlDbType.NVarChar ||
               Tipo == SqlDbType.Text || Tipo == SqlDbType.VarChar)
            {
                this.Agregar(Nombre, Tipo, Longitud, "", Direccion);
            }
            else 
            {
                this.Agregar(Nombre, Tipo, 0, Longitud, Direccion);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nombre"> Nombre del parametro. Ejem. @Parametro1 </param>
        /// <param name="Tipo"> Tipo de dato para el parametro </param>
        /// <param name="Valor"> Valor a asignar al parametro </param>
        /// <param name="Direccion"> Tipo de parametro, Output, Input, OutputInput </param>
        public void Agregar(String Nombre, SqlDbType Tipo, Object Valor, ParameterDirection Direccion)
        {
            this.Agregar(Nombre, Tipo, 0, Valor, Direccion);
        }        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Nombre"> Nombre del parametro. Ejem. @Parametro1 </param>
        /// <param name="Tipo"> Tipo de dato para el parametro </param>
        /// <param name="Longitud"> Longitud del tipo de dato del parametro </param>
        /// <param name="Valor"> Valor a asignar al parametro </param>
        /// <param name="Direccion"> Tipo de parametro, Output, Input, OutputInput </param>
        public void Agregar(String Nombre, SqlDbType Tipo, Int32 Longitud, Object Valor, ParameterDirection Direccion)
        {
            SqlParameter parametro = new SqlParameter();
            parametro.ParameterName = Nombre;
            parametro.SqlDbType = Tipo;
            parametro.Size = Longitud; 
            parametro.Value = Valor;            
            parametro.Direction = Direccion;
            this._ListaParametros.Add(parametro);            
        }
        /// <summary>
        /// Obtiene la lista completa de parametros que se han agregado.
        /// </summary>
        /// <returns> obtiene un List del tipo SqlParameter </returns>
        public List<SqlParameter> ObtenerListaParametros()
        {            
            return this._ListaParametros;
        }
        /// <summary>
        /// Obtiene el valor asignado al parametro especificado. Se implementa cuando el parametro es del tipo Output.
        /// </summary>
        /// <param name="Nombre"> Nombre del parametro </param>
        /// <returns> Regresa un objeto generico que puede ser casteado a un tipo concreto de dato.</returns>
        public GenericObject ObtenerValorParametro(String Nombre)
        {            
            IEnumerable<String> Resultado = from parametro in _ListaParametros 
                               where parametro.ParameterName == Nombre 
                               select parametro.Value != null ? parametro.Value.ToString() : "";
            GenericObject obj = new GenericObject(Resultado.ToList()[0]);
            return obj;
        }
        /// <summary>
        /// Obtiene el valor asignado al parametro especificado. 
        /// </summary>
        /// <param name="parametro">Nombre del parametro</param>
        /// <returns>Regresa un objeto generico que puede ser casteado a un tipo concreto de dato.</returns>
        public GenericObject this[String parametro]
        {
            get
            {
                if (!parametro.Contains("@"))
                {
                    parametro = String.Format("@{0}", parametro);
                }

                IEnumerable<String> Resultado = from param in _ListaParametros
                                                where param.ParameterName == parametro
                                                select param.Value != null ? param.Value.ToString() : "";
                GenericObject obj = new GenericObject(Resultado.ToList()[0]);
                return obj;
            }

        }
    
        /// <summary>
        /// Vacia la lista de parametros.
        /// </summary>
        public void Limpiar()
        {
            this._ListaParametros.Clear();
        }
    }
}
