using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio
{
    // Clase centralizada para gestionar la comunicación con SQL Server
    public class AccesoDatos
    {
        // Objetos de ADO.NET necesarios para la conexión y ejecución
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        // Propiedad para permitir que las clases de Negocio lean los datos obtenidos
        public SqlDataReader Lector
        {
            get { return lector; }
        }

        // Constructor: Configura la conexión inicial
        public AccesoDatos()
        {
            // String de conexión estándar. El punto (.) indica servidor local.
            conexion = new SqlConnection("server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=true");
            comando = new SqlCommand();
        }

        // Configura el tipo de comando y la sentencia SQL (Query)
        public void setearConsulta(string consulta)
        {
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = consulta;
        }

        // Ejecuta consultas de lectura (SELECT)
        public void ejecutarLectura()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex; // Relanzamos la excepción para capturarla en la capa de Presentación
            }
        }

        // Ejecuta acciones de escritura (INSERT, UPDATE, DELETE)
        public void ejecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Método para cargar parámetros y evitar SQL Injection (Seguridad)
        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor);
        }

        // Cierra el lector y la conexión para liberar recursos en el servidor
        public void cerrarConexion()
        {
            if (lector != null)
                lector.Close();
            conexion.Close();
        }
    }
}
