using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ConexionesBD
{
    /// <summary>
    /// Crea un objeto conexion que permite realizar consultas a un servidor SQL Server.
    /// </summary>
    public class CConexion : IDisposable
    {
        Boolean _disposed = false;
        SqlConnection _conexion = null;
        SqlCommand _cmd = null;
        SqlDataReader _reader = null;
        SqlTransaction _transaction = null;
        
        /// <summary>
        /// Establece el tiempo maximo que durara una consulta en ejecutarse.
        /// </summary>
        public int TimeOut 
        {
            get
            {
                return _cmd != null ? _cmd.CommandTimeout : 0;
            }
            set
            {
                _cmd.CommandTimeout = value;
            } 
        }    
        /// <summary>
        /// Esta clase permite abrir una conexion con sql server y ejecutar consultas, procedimientos, transacciones, etc.
        /// </summary>
        /// <param name="Servidor">Direccion IP o Nombre del servidor</param>
        /// <param name="BaseDatos">Base de datos</param>
        /// <param name="Usuario">Usuario de la base de datos</param>
        /// <param name="Password">Contraseña de la base de datos</param>
        public CConexion(String Servidor, String BaseDatos, String Usuario, String Password)
        {
            String cadenaConexion = 
                String.Format("Data source={0}; Initial catalog={1}; User id={2}; Password={3}",
                               Servidor,
                               BaseDatos,
                               Usuario,
                               Password);
            this._conexion = new SqlConnection(cadenaConexion);
            this._cmd = new SqlCommand("", this._conexion);
        }
        /// <summary>
        /// Esta clase permite abrir una conexion con sql server y ejecutar consultas, procedimientos, transacciones, etc.
        /// </summary>
        /// <param name="CadenaConexion">Cadena de conexión completa para sql server</param>
        public CConexion(String CadenaConexion)
        {
            this._conexion = new SqlConnection(CadenaConexion);
            this._cmd = new SqlCommand("", this._conexion);
        }
        /// <summary>
        /// Destructor de la clase
        /// </summary>
        ~CConexion()
        {
            this.Dispose();
        }
        /// <summary>
        /// Libera los recursos que fueron asignados a esta conexion.
        /// </summary>
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                Cerrar();
                _conexion.Dispose();
                _cmd.Dispose();
                if (_reader != null)
                {
                    _reader.Dispose();
                }
                if (_transaction != null)
                {
                    _transaction.Dispose();
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);    
        }
        /// <summary>
        /// Abre la conexión.
        /// </summary>
        public void Abrir()
        {
            try
            {
                if (this._conexion.State != System.Data.ConnectionState.Open)
                {
                    this._conexion.Open();
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Cierra la conexión.
        /// </summary>
        public void Cerrar()
        {
            try
            {
                if (this._conexion.State != System.Data.ConnectionState.Closed)
                {
                    CerrarLector();
                    this._conexion.Close();
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Inicia una transacción, si la conexión esta cerrada esta funcion la abre.
        /// </summary>
        public void IniciarTransaccion()
        {
            this.Abrir();
            try
            {
                this._transaction = this._conexion.BeginTransaction();
                this._cmd.Transaction = this._transaction;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }                
        }
        private void CerrarLector()
        {
            if (this._reader != null && !this._reader.IsClosed)
            {
                _reader.Close();
            }
        }
        /// <summary>
        /// Guarda los resultados de la transacción. 
        /// </summary>
        public void CommitTransaccion()
        {
            try
            {
                CerrarLector();               
                this._transaction.Commit();
                this.Cerrar();
                this._transaction = null;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Restablece los cambios de la transacción.
        /// </summary>
        public void RollBackTransaccion()
        {
            try
            {
                CerrarLector();         
                this._transaction.Rollback();
                this._conexion.Close();
                this._transaction = null;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Ejecuta la consulta especificada. Se requiere abrir la conexión.
        /// </summary>
        /// <param name="consulta"> Consulta a ejecutar.</param>
        /// <returns> Regresa el numero de filas afectadas. </returns>
        public int EjecutaConsulta(String consulta)
        {
            try
            {
                CerrarLector(); 
                this._cmd.CommandType = CommandType.Text;
                this._cmd.CommandText = consulta;                        
                return this._cmd.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Ejecuta el procedimiento almacenado especificado.
        /// </summary>
        /// <param name="procedimiento"> Nombre del procedimiento almacenado </param>
        /// <param name="parametros"> Lista de parametros del procedimiento almacenado, deben de ser pasados por referencia. </param>
        /// <returns> Regresa el numero de filas afectadas. </returns>
        public int EjecutaConsulta(String procedimiento, ListaParametros parametros)
        {
            try
            {
                CerrarLector();
                int FilasAfectadas = 0;

                this._cmd.CommandType = System.Data.CommandType.StoredProcedure;
                this._cmd.CommandText = procedimiento;
                this._cmd.Parameters.Clear();
                List<SqlParameter> param = parametros.ObtenerListaParametros();
                foreach (SqlParameter p in param)
                {
                    this._cmd.Parameters.Add(p);
                }
                      
                FilasAfectadas = this._cmd.ExecuteNonQuery();
                this._cmd.CommandType = CommandType.Text;
                return FilasAfectadas;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }        
        /// <summary>
        /// Ejecuta la consulta especificada, despues de esto debe de usarse el metodo LeerFila() para obtener los registros.
        /// </summary>
        /// <param name="consulta"> Consulta a ejecutar </param>
        public void EjecutaConsultaResultados(String consulta)
        {
            try
            {
                CerrarLector();
                this._cmd.CommandType = CommandType.Text;
                this._cmd.CommandText = consulta;
                this._cmd.Parameters.Clear();
                this._reader = this._cmd.ExecuteReader();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Ejecuta el procedimiento almacenado especificado, despues de esto debe usarse el metodo LeerFila() para obtener los registros.
        /// </summary>
        /// <param name="procedimiento"> Nombre del procedimiento almacenado </param>
        /// <param name="parametros"> Lista de parametros del procedimiento almacenado, deben de ser pasados por referencia. </param>
        public void EjecutaConsultaResultados(String procedimiento, ListaParametros parametros)
        {
            try
            {
                CerrarLector(); 
                this._cmd.CommandType = System.Data.CommandType.StoredProcedure;
                this._cmd.CommandText = procedimiento;
                this._cmd.Parameters.Clear();
                List<SqlParameter> param = parametros.ObtenerListaParametros();
                foreach (SqlParameter p in param)
                {
                    this._cmd.Parameters.Add(p);
                }                
                this._reader = this._cmd.ExecuteReader();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Ejecuta la consulta especificada y la regresa en el DataTable especificado
        /// </summary>
        /// <param name="consulta"> Consulta a ejecutar </param>
        /// <param name="table"> DataTable donde se guardaran los resultados, debe de ser pasada por referencia </param>
        public void EjecutaConsultaResultados(String consulta, ref DataTable table)
        {
            try
            {
                CerrarLector();
                DataTable Resultado = new DataTable();
                this._cmd.CommandType = CommandType.Text;
                this._cmd.CommandText = consulta;
                this._cmd.Parameters.Clear();
                this._reader = this._cmd.ExecuteReader();
                while (this._reader.Read())
                {
                    if (Resultado.Columns.Count == 0)//crear el esquema de la tabla
                    {
                        for (int i = 0; i < _reader.FieldCount; i++)
                        {
                            Resultado.Columns.Add(_reader.GetName(i), _reader.GetFieldType(i));
                        }
                    }
                    DataRow Renglon = Resultado.NewRow();

                    for (int i = 0; i < _reader.FieldCount; i++)
                    {
                        Renglon[i] = _reader[i];
                    }

                    Resultado.Rows.Add(Renglon);
                }
                table = Resultado;
                _reader.Close();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Ejecuta el procedimiento almacenado y regresa los resultados en el DataTable especificado.
        /// </summary>
        /// <param name="procedimiento"> Nombre del procedimiento almacenado </param>
        /// <param name="parametros"> Lista de parametros del procedimiento almacenado, debe de ser pasada por referencia </param>
        /// <param name="table">DataTable donde se guardaran los resultados, debe de ser pasada por referencia</param>
        public void EjecutaConsultaResultados(String procedimiento, ListaParametros parametros, ref DataTable table)
        {
            try
            {
                CerrarLector();
                DataTable Resultado = new DataTable();
                this._cmd.CommandType = CommandType.StoredProcedure;
                this._cmd.CommandText = procedimiento;
                this._cmd.Parameters.Clear();
                List<SqlParameter> param = parametros.ObtenerListaParametros();
                foreach (SqlParameter p in param)
                {
                    this._cmd.Parameters.Add(p);
                }
                this._reader = this._cmd.ExecuteReader();
                
                while (this._reader.Read())
                {
                    if (Resultado.Columns.Count == 0)//crear el esquema de la tabla
                    {
                        for (int i = 0; i < _reader.FieldCount; i++)
                        {
                            Resultado.Columns.Add(_reader.GetName(i), _reader.GetFieldType(i));
                        }
                    }
                    DataRow Renglon = Resultado.NewRow();

                    for (int i = 0; i < _reader.FieldCount; i++)
                    {
                        Renglon[i] = _reader[i];
                    }

                    Resultado.Rows.Add(Renglon);
                }
                table = Resultado;
                _reader.Close();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// regresa los resultados en un dataset
        /// </summary>
        /// <param name="procedimiento">EjecutaConsultaResultados</param>
        /// <param name="parametros">Los parametros de la consulta</param>
        /// <param name="dataSet">resultado</param>
        public void EjecutaConsultaResultados(String procedimiento, ListaParametros parametros, ref DataSet dataSet)
        {
            try
            {
                CerrarLector();
                SqlDataAdapter adaptador = new SqlDataAdapter();
                this._cmd.CommandType = CommandType.StoredProcedure;
                this._cmd.CommandText = procedimiento;
                this._cmd.Parameters.Clear();
                List<SqlParameter> param = parametros.ObtenerListaParametros();
                foreach (SqlParameter p in param)
                {
                    this._cmd.Parameters.Add(p);
                }
                adaptador.SelectCommand = this._cmd;
                adaptador.Fill(dataSet);
                adaptador.Dispose();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// regresa los resultados en un dataset
        /// </summary>
        /// <param name="procedimiento">EjecutaConsultaResultados</param>
        /// <param name="parametros">Los parametros de la consulta</param>
        /// <param name="dataSet">resultado</param>
        public void EjecutaConsultaResultados(String procedimiento, ref DataSet dataSet)
        {
            try
            {
                CerrarLector();
                SqlDataAdapter adaptador = new SqlDataAdapter();
                this._cmd.CommandType = CommandType.StoredProcedure;
                this._cmd.CommandText = procedimiento;
                this._cmd.Parameters.Clear();
                
                adaptador.SelectCommand = this._cmd;
                adaptador.Fill(dataSet);
                adaptador.Dispose();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Posiciona el Lector en la siguiente fila, usarse de preferencia en un while en conjunto con el metodo Leer([NombreColumna]) para leer las columnas que desee.
        /// </summary>
        /// <returns> True si hay filas para leer, de lo contrario False</returns>
        public Boolean LeerFila()
        {
            Boolean Resultado = _reader.Read();
            if (!Resultado)
            {
                _reader.Close();
            }            
            return Resultado;
        }

        /// <summary>
        /// Posiciona el Lector en la siguiente fila y se puede especificar si se deja abierto el lector o no, usarse de preferencia en un while en conjunto con el metodo Leer([NombreColumna]) para leer las columnas que desee.
        /// </summary>
        /// <returns> True si hay filas para leer, de lo contrario False</returns>
        public Boolean LeerFila(bool ExisteOtroRecordSet)
        {
            Boolean Resultado = _reader.Read();            
            if (!Resultado)
            {
                if (!ExisteOtroRecordSet)
                {
                    _reader.Close();
                }
            }           
 
            return Resultado;
        }

        /// <summary>
        /// Posiciona el Lector en la siguiente conjunto de resultados, 
        /// </summary>
        /// <returns> True si hay filas para leer, de lo contrario False</returns>
        public Boolean SiguienteRecordSet()
        {
            Boolean Resultado = _reader.NextResult();
            if (!Resultado)
            {
                _reader.Close();
            }

            return Resultado;
        }
        /// <summary>
        /// Regresa el valor de la columna especificada
        /// </summary>
        /// <param name="campo"> Nombre de la columna a leer. </param>
        /// <returns> Regresa un objeto que puede ser casteado al tipo deseado. Ejemp. Leer("Columna1").ToInteger() regresara un valor entero </returns>
        public GenericObject Leer(String campo)
        {            
            GenericObject resultado = new GenericObject(this._reader[campo]);
            return resultado;
        }
        /// <summary>
        /// Regresa el valor de la columna especificada
        /// </summary>
        /// <param name="indice"> Numero de la columna a leer. </param>
        /// <returns> Regresa un objeto que puede ser casteado al tipo deseado. Ejemp. Leer("Columna1").ToInteger() regresara un valor entero </returns>
        public GenericObject Leer(Int32 indice)
        {
            GenericObject resultado = new GenericObject(this._reader[indice]);
            return resultado;
        }
        /// <summary>
        /// Regresa el valor de la columna especificada
        /// </summary>
        /// <param name="campo"></param>
        /// <returns></returns>
        public GenericObject this[String campo]
        {
            get
            {
                GenericObject resultado = new GenericObject(this._reader[campo]);
                return resultado;
            }        
        }
        /// <summary>
        /// Regresa el valor de la columna especifica
        /// </summary>
        /// <param name="indice"></param>
        /// <returns></returns>
        public GenericObject this[int indice]
        {
            get
            {
                GenericObject resultado = new GenericObject(this._reader[indice]);
                return resultado;
            }
        }
    
        /// <summary>
        /// Ejecuta la consulta y obtiene un solo valor. Usarse cuando solo se regresa un valor.
        /// </summary>
        /// <param name="consulta"> Consulta a ejecutar. </param>
        /// <returns> Regresa un objeto que puede ser casteado al tipo deseado. Ejemp. EjecutaEscalar("SELECT CURRENT_TIMESTAMP").ToDateTime() </returns>
        public GenericObject EjecutaEscalar(String consulta)
        {
            try
            {
                CerrarLector();  
                this._cmd.CommandType = CommandType.Text;
                this._cmd.Parameters.Clear();
                this._cmd.CommandText = consulta;
                GenericObject resultado = new GenericObject(this._cmd.ExecuteScalar());
                return resultado;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Ejecuta la consulta y obtiene un solo valor. Usarse cuando solo se regresa un valor.
        /// </summary>
        /// <param name="consulta"> Consulta a ejecutar. </param>
        /// <param name="parametros"></param>
        /// <returns> Regresa un objeto que puede ser casteado al tipo deseado. Ejemp. EjecutaEscalar("SELECT CURRENT_TIMESTAMP").ToDateTime() </returns>
        public GenericObject EjecutaEscalar(String consulta, ListaParametros parametros)
        {
            try
            {
                CerrarLector();
                this._cmd.CommandType = CommandType.StoredProcedure;
                this._cmd.Parameters.Clear();
                List<SqlParameter> param = parametros.ObtenerListaParametros();
                foreach (SqlParameter p in param)
                {
                    this._cmd.Parameters.Add(p);
                }
                this._cmd.CommandText = consulta;

                GenericObject resultado = new GenericObject(this._cmd.ExecuteScalar());
                return resultado;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new ConexionException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Obtiene el conjunto de resultados en formato de lista del objeto especificado
        /// </summary>
        /// <typeparam name="T">Clase de los objetos de la lista</typeparam>
        /// <returns></returns>
        public List<T> GetLista<T>()
        {
            return this.GetLista<T>(false);
        }
        /// <summary>
        /// Obtiene el conjunto de resultados en formato de lista del objeto especificado
        /// </summary>
        /// <typeparam name="T">Clase de los objetos de la lista</typeparam>
        /// <returns></returns>
        public List<T> GetLista<T>(bool existeOtroRecordSet)
        {
            List<T> list = new List<T>();
            Type temp = typeof(T);            

            while (this._reader.Read())
            {
                T obj = Activator.CreateInstance<T>();

                for (int i =0; i < this._reader.FieldCount; i++)
                {
                    foreach (FieldInfo pro in temp.GetFields())
                    {
                        if (pro.Name == this._reader.GetName(i))
                        {
                            pro.SetValue(obj, this._reader[i]);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if (pro.Name == this._reader.GetName(i))
                        {
                            pro.SetValue(obj, this._reader[i], null);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                list.Add(obj);
            }

            if (!existeOtroRecordSet)
            {
                _reader.Close();
            }

            return list;                        
        }
    }
}
