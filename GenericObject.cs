using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConexionesBD
{    
    public class GenericObject 
    {
        public GenericObject(Object obj)
        {
            Valor = obj;
        }

        Object Valor { get; set; }                       
        /// <summary>
        /// Convierte el valor al tipo de dato Entero. 
        /// </summary>
        /// <returns> Un Entero que puede tener valores de -2.147.483.648 a 2.147.483.647 </returns>
        public  Int32 ToInteger()
        {
            Int32 resultado = 0;
            if (!Int32.TryParse(Valor.ToString(), out resultado))
            {
                throw LanzaException(resultado.GetType());
            }
            return resultado;
        }
        /// <summary>
        /// Convierte el valor al tipo de dato Entero sin signo.
        /// </summary> 
        /// <returns> Un Entero que puede tener valores de 0 a 4.294.967.295 </returns>
        public  UInt32 ToUInteger()
        {
            UInt32 resultado = 0;
            if (!UInt32.TryParse(Valor.ToString(), out resultado))
            {
                throw LanzaException(resultado.GetType());
            }
            return resultado;
        }
        /// <summary>
        /// Convierte el valor al tipo de dato Decimal que puede ser usado para valores monetarios.
        /// </summary>
        /// <returns>         
        /// El tipo de valor Decimal representa números decimales comprendidos entre más 79,228,162,514,264,337,593,543,950,335 y 
        /// menos 79,228,162,514,264,337,593,543,950,335. El tipo de valorDecimal es adecuado para realizar cálculos financieros que requieren 
        /// un gran número de dígitos integrales y fraccionarios significativos sin errores de redondeo.        
        /// </returns>
        public  Decimal ToDecimal()
        {
            Decimal resultado = 0;
            if (!Decimal.TryParse(Valor.ToString(), out resultado))
            {
                throw LanzaException(resultado.GetType());
            }
            return resultado;
        }
        /// <summary>
        /// Convierte el valor al tipo de dato Flotante.
        /// </summary>
        /// <returns></returns>
        public  Single ToFloat()
        {
            Single resultado = 0;
            if (!Single.TryParse(Valor.ToString(), out resultado))
            {
                throw LanzaException(resultado.GetType());
            }
            return resultado; 
        }
        /// <summary>
        /// Convierte el valor al tipo de dato Double. 
        /// </summary>
        /// <returns></returns>
        public  Double ToDouble()
        {
            Double resultado = 0;
            if (!Double.TryParse(Valor.ToString(), out resultado))
            {
                throw LanzaException(resultado.GetType());
            }
            return resultado;
        }
        /// <summary>
        /// Convierte el valor al tipo de dato entero grande.
        /// </summary>
        /// <returns></returns>
        public  Int64 ToLong()
        {
            Int64 resultado = 0;
            if (!Int64.TryParse(Valor.ToString(), out resultado))
            {
                throw LanzaException(resultado.GetType());
            }
            return resultado;
        }
        /// <summary>
        /// Convierte el valor al tipo de dato entero grande sin signo.
        /// </summary>
        /// <returns></returns>
        public  UInt64 ToULong()
        {
            UInt64 resultado = 0;
            if (!UInt64.TryParse(Valor.ToString(), out resultado))
            {
                throw LanzaException(resultado.GetType());
            }
            return resultado;
        }
        /// <summary>
        /// Convierte el valor al tipo de dato Booleano.
        /// </summary>
        /// <returns></returns>
        public  Boolean ToBoolean()
        {            
            return Convert.ToBoolean(Valor.ToString());
        }
        /// <summary>
        /// Convierte el valor al tipo de dato Byte.
        /// </summary>
        /// <returns></returns>
        public  Byte ToByte()
        {
            Byte resultado = 0;
            if (!Byte.TryParse(Valor.ToString(), out resultado))
            {
                throw LanzaException(resultado.GetType());
            }
            return resultado;
        }
        /// <summary>
        /// Convierte el valor a un arreglo de bytes
        /// </summary>
        /// <returns></returns>
        public Byte[] ToBytes()
        {
            Byte[] resultado = (byte[]) Valor;            

            return resultado;
        }
        /// <summary>
        /// Convierte el valor en un Xml
        /// </summary>
        /// <returns>Regresa el valor en un tipo XElement</returns>
        public XElement ToXml()
        {
            XElement elemento = new XElement(Valor.ToString());

            return elemento;            
        }

        /// <summary>
        /// Convierte el valor al tipo de dato DateTime.
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            DateTime resultado = new DateTime();
            if (!DateTime.TryParse(Valor.ToString(), out resultado))
            {
                LanzaException(resultado.GetType());
            }
            return resultado;
        }
        /// <summary>
        /// Regresa un String en el formato especificado.
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public String ToDateTime(String Format)
        {
            DateTime resultado = new DateTime();
            if (!DateTime.TryParse(Valor.ToString(), out resultado))
            {
                LanzaException(resultado.GetType());
            }
            return resultado.ToString(Format);            
        }
        /// <summary>
        /// Convierte el valor al tipo de dato String.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {            
            return Valor != null ? Valor.ToString() : "";
        }        
        
        private Exception LanzaException(Type tipo)
        {            
           return new Exception(String.Format("No se puede convertir el valor \"{0}\" al tipo \"{1}\"", Valor, tipo.Name));            
        }
    }
}
