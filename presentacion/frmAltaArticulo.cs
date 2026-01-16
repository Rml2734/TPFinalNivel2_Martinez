using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace presentacion
{
    // Este formulario es polifuncional: sirve para Crear, Editar y Ver Detalle.
    public partial class frmAltaArticulo : Form
    {
        // Atributo para manejar el objeto en memoria durante la sesión del formulario
        // Si es null, el formulario se comporta como "Nuevo Artículo".
        private Articulo articulo = null;

        // Flag para determinar si el formulario se abre solo para visualización
        private bool soloLectura = false;

        // Constructor para un artículo NUEVO
        public frmAltaArticulo()
        {
            InitializeComponent();
           
        }

        // Constructor para MODIFICAR o VER DETALLE (recibe el objeto seleccionado)
        public frmAltaArticulo(Articulo seleccionado, bool verDetalle = false)
        {
            InitializeComponent();
            this.articulo = seleccionado;
            this.soloLectura = verDetalle;

            // Personalizamos el título de la ventana según la intención
            if (soloLectura)
                Text = "Detalle del Artículo";
            else
                Text = "Modificar Artículo";
        }

        private void frmAltaArticulo_Load(object sender, EventArgs e)
        {
            MarcaNegocio marcaNegocio = new MarcaNegocio();
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
            try
            {
                // Carga de desplegables (ComboBox) desde la base de datos
                cboMarca.DataSource = marcaNegocio.listar();
                // Importante: Qué valor mostramos y qué valor guardamos (ID)
                cboMarca.ValueMember = "Id"; // El valor oculto (PK)
                cboMarca.DisplayMember = "Descripcion"; // Lo que ve el usuario

                // Llenamos el combo de categorías
                cboCategoria.DataSource = categoriaNegocio.listar();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";

                // Si 'articulo' no es nulo, precargamos los controles con sus datos (Modificar/Detalle)
                // Entonces llenamos los campos con la información que ya existe.
                if (articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtImagenUrl.Text = articulo.ImagenUrl;
                    txtPrecio.Text = articulo.Precio.ToString();

                    // Seteamos los combos en la posición correcta según el objeto recibido
                    cboMarca.SelectedValue = articulo.Marca.Id;
                    cboCategoria.SelectedValue = articulo.Categoria.Id;
                }

                // --- MODO SOLO LECTURA ---

                // Configuración para el modo "Ver Detalle"
                if (soloLectura)
                {
                    // Deshabilitamos la edición en todos los campos
                    txtCodigo.Enabled = false;
                    txtNombre.Enabled = false;
                    txtDescripcion.Enabled = false;
                    txtImagenUrl.Enabled = false;
                    txtPrecio.Enabled = false;
                    cboMarca.Enabled = false;
                    cboCategoria.Enabled = false;

                    // Ocultamos el botón de aceptar porque no tiene sentido guardar
                    btnAceptar.Visible = false;
                    // El botón cancelar lo podemos dejar como "Cerrar" o "Volver"
                    btnCancelar.Text = "Volver";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                // 1. Validaciones de Negocio básicas (Campos obligatorios)
                if (string.IsNullOrEmpty(txtCodigo.Text) || string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtPrecio.Text))
                {
                    MessageBox.Show("Código, Nombre y Precio son obligatorios.");
                    return;
                }

                // 2. Validación de formato
                if (!soloNumeros(txtPrecio.Text))
                {
                    MessageBox.Show("Por favor, ingrese solo números en el campo Precio.");
                    return;
                }

                // 3. Mapeo de datos: De la pantalla al objeto
                if (articulo == null) articulo = new Articulo();

                articulo.Codigo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.ImagenUrl = txtImagenUrl.Text;
                articulo.Precio = decimal.Parse(txtPrecio.Text);

                // Obtenemos los objetos completos de Marca y Categoría seleccionados
                articulo.Marca = (Marca)cboMarca.SelectedItem;
                articulo.Categoria = (Categoria)cboCategoria.SelectedItem;

                // 4. Persistencia: Decidimos si insertar o actualizar según el ID
                // Si el ID es distinto de cero, ya existe en la DB, entonces modificamos.
                if (articulo.Id != 0)
                {
                    negocio.modificar(articulo);
                    MessageBox.Show("Modificado exitosamente");
                }
                else
                {
                    negocio.agregar(articulo);
                    MessageBox.Show("Agregado exitosamente");
                }
                // Cerramos el formulario y regresamos al principal (Form1 capturará el cierre y refrescará)
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Asegúrese de que el formato de los datos es correcto.");
            }
        }

        // Método de validación de caracteres para evitar errores de conversión (casting/parsing)
        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                // Permitimos el punto o coma decimal si fuera necesario, 
                // pero por ahora validamos números puros para evitar errores de parseo
                if (!(char.IsNumber(caracter)) && caracter != '.' && caracter != ',')
                    return false;
            }
            return true;
        }
    }
}



