using dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio
{
    // Clase encargada de gestionar las consultas relacionadas con las Marcas
    public class MarcaNegocio
    {
        // Obtiene todas las marcas disponibles en la base de datos
        public List<Marca> listar()
        {           
            List<Marca> lista = new List<Marca>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                // Solo traemos Id y Descripcion para optimizar la carga de ComboBoxes
                datos.setearConsulta("Select Id, Descripcion from MARCAS");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    // Usamos inicialización de objetos para simplificar el código
                    lista.Add(new Marca { 
                        Id = (int)datos.Lector["Id"], 
                        Descripcion = (string)datos.Lector["Descripcion"] });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
    }

    // Clase encargada de gestionar las consultas relacionadas con las Categorías
    public class CategoriaNegocio
    {
        // Obtiene todas las categorías disponibles para clasificar artículos
        public List<Categoria> listar()
        {
            List<Categoria> lista = new List<Categoria>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Select Id, Descripcion from CATEGORIAS");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(new Categoria { 
                        Id = (int)datos.Lector["Id"], 
                        Descripcion = (string)datos.Lector["Descripcion"] });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
    }
}
