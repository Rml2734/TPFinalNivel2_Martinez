using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace presentacion
{
    // Formulario principal que muestra el catálogo de artículos
    public partial class Form1 : Form
    {
        // Lista local para mantener los datos en memoria y evitar consultas innecesarias
        private List<Articulo> listaArticulo;

        public Form1()
        {
            InitializeComponent();
        }

        // Evento de inicio: Aquí configuramos la carga inicial de datos
        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();

            // Cargamos las opciones del desplegable para el filtro avanzado
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripción");
            cboCampo.Items.Add("Precio");
        }

        // Método centralizado para refrescar la grilla desde la base de datos
        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                // 1. Invocamos a la capa de negocio para obtener los datos
                listaArticulo = negocio.listar();

                // 2. Enlazamos la lista obtenida con el control visual (DataGridView)
                dgvArticulos.DataSource = listaArticulo;

                // 3. Estética: Ocultamos columnas técnicas o rutas de archivos
                if (dgvArticulos.Columns["ImagenUrl"] != null)
                    dgvArticulos.Columns["ImagenUrl"].Visible = false;
                //if (dgvArticulos.Columns["Id"] != null)
                //  dgvArticulos.Columns["Id"].Visible = false;

                // 4. Previsualización: Cargamos la imagen del primer artículo de la lista
                if (listaArticulo.Count > 0)
                {
                    cargarImagen(listaArticulo[0].ImagenUrl);
                }
            }
            catch (Exception ex)
            {
                // Mostramos el error de forma amigable para el usuario
                MessageBox.Show("Error al cargar la lista: " + ex.ToString());
            }

        }

        // Este evento se dispara cuando el usuario cambia de fila en la grilla
        private void dgvArticulos_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                // Obtenemos el objeto que representa la fila seleccionada (Casting)
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.ImagenUrl);
            }
        }

        // Método para cargar imágenes de forma segura (con manejo de errores)
        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulo.Load(imagen);
            }
            catch (Exception)
            {
                // Si la imagen falla (URL rota o nula), cargamos un placeholder local o remoto
                pbxArticulo.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        // --- NOS LLEVA AL FORMULARIO DE ALTA ---

        // Abre el formulario de alta y refresca la grilla al cerrar
        private void btnAgregar_Click_1(object sender, EventArgs e)
        {
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.ShowDialog(); // ShowDialog detiene la ejecución del Form1 hasta que se cierre 'alta'
            cargar();
        }

        // Captura el elemento seleccionado y lo envía al formulario de edición
        private void btnModificar_Click(object sender, EventArgs e)
        {
            // 1. Validamos que haya una fila seleccionada
            if (dgvArticulos.CurrentRow != null)
            {
                // 2. Capturamos el objeto completo de la fila seleccionada
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

                // 3. Abrimos la ventana de Alta pero le PASAMOS el seleccionado
                frmAltaArticulo modificar = new frmAltaArticulo(seleccionado);
                modificar.ShowDialog();

                // 4. Refrescamos la grilla al volver
                cargar();
            }
        }

        // --- FUNCIONALIDAD: VER DETALLE ---

        // Permite ver los detalles de un artículo sin poder editarlos (Modo Lectura)
        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                // El segundo parámetro 'true' activa el modo solo lectura en el formulario
                frmAltaArticulo detalle = new frmAltaArticulo(seleccionado, true);
                detalle.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione un artículo para ver su detalle.");
            }
        }

        // Lógica de eliminación física con confirmación del usuario
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;
            try
            {
                // 1. Verificamos que haya una fila seleccionada en la grilla
                if (dgvArticulos.CurrentRow != null)
                {
                    // 2.  Preguntamos antes de borrar (Crucial para la experiencia de usuario)
                    DialogResult respuesta = MessageBox.Show("¿De verdad desea eliminar este artículo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (respuesta == DialogResult.Yes)
                    {
                        // 3. Obtenemos el objeto de la fila seleccionada
                        seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

                        // 4. Llamamos al método eliminar de la capa de negocio usando el Id
                        negocio.eliminar(seleccionado.Id);

                        // 5. Refrescamos la grilla llamando a nuestro método cargar()
                        cargar();

                        MessageBox.Show("Eliminado correctamente.");
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un artículo de la lista para eliminar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar eliminar: " + ex.ToString());
            }
        }

        // --- FILTRO AVANZADO ---

        // --- EVENTO: CAMBIO DE CAMPO ---
        // Se dispara al elegir Nombre, Descripcion o Precio
        // Lógica de Filtro Avanzado: Cambia las opciones de criterio según el campo elegido
        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // CORRECCIÓN: Si no hay nada seleccionado (índice -1), salimos del método
            // Esto evita que el programa rompa al usar el botón Limpiar
            if (cboCampo.SelectedIndex < 0) return;

            string opcion = cboCampo.SelectedItem.ToString();
            cboCriterio.Items.Clear(); // Limpiamos para que no se acumulen

            if (opcion == "Precio")
            {
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        // --- EVENTO: BOTÓN BUSCAR ---

        // Ejecuta la búsqueda avanzada llamando a la capa de negocio
        private void btnFiltroAvanzado_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                // Antes de buscar, validamos
                if (validarFiltro())
                    return;

                // Capturar los datos de la interfaz
                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;

                // Llamamos al método que ya pusimos en ArticuloNegocio.cs
                dgvArticulos.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.ToString());
            }
        }

        // --- VALIDACIONES ---

        // Verifica que los datos ingresados para el filtro sean correctos
        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el campo para filtrar.");
                return true;
            }
            if (cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el criterio para filtrar.");
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar un número para filtrar por precio.");
                    return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Solo números para filtrar por campo numérico.");
                    return true;
                }
            }
            return false;
        }

        // Función auxiliar para validar que el texto sea numérico (evita caídas por tipos de datos)
        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        // Resetea la vista a la lista original sin filtros
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cargar();
            txtFiltroAvanzado.Text = "";
            cboCampo.SelectedIndex = -1;
            cboCriterio.SelectedIndex = -1;
        }
    }
}
