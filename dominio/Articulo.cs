using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio
{
    public class Articulo
    {
        public int Id { get; set; }

        // Código alfanumérico del artículo
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }

        // URL de la imagen para visualización en la interfaz
        public string ImagenUrl { get; set; }

        // Composición: Relación con la clase Marca
        public Marca Marca { get; set; }

        // Composición: Relación con la clase Categoria
        public Categoria Categoria { get; set; }
    }
}
