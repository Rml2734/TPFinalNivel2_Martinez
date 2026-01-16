using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio
{
    public class Categoria
    {
        public int Id { get; set; }

        // Nombre o detalle de la categoría
        public string Descripcion { get; set; }

        // Sobreescritura del método para mostrar la descripción en ComboBoxes
        public override string ToString()
        {
            return Descripcion;
        }
    }
}
