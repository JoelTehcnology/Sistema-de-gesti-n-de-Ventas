using CapaEntidad;
using CapaNegocio;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using Proyecto_Joel_AF.Modales;
using Proyecto_Joel_AF.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Joel_AF
{
    public partial class frmCompras : Form
    {
        private Usuario _Usuario;
        public frmCompras(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;    
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {
           
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cbotipodocumento.DisplayMember = "Texto";
            cbotipodocumento.ValueMember = "Valor";
            cbotipodocumento.SelectedIndex = 0;

            //MOSTRAR A FECHA
            txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            txtidproducto.Text = "0";
            txtidproducto.Text = "0";

        }

        private void btnbuscarproveedor_Click(object sender, EventArgs e)
        {
            using (var rt = new mdProveedor())
            {
                var result = rt.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtidproveedor.Text = rt._Proveedor.IdProveedor.ToString();
                    txtdocproveedor.Text = rt._Proveedor.Documento;
                    txtnombreproveedor.Text = rt._Proveedor.RazonSocial;
                }
                else
                {
                    txtidproveedor.Select();
                }
            }
        }

        private void btnbuscarproducto_Click(object sender, EventArgs e)
        {
            using (var rt = new mdproducto())
            {
                var result = rt.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtidproducto.Text = rt._Producto.IdProducto.ToString();
                    txtcodproducto.Text = rt._Producto.Codigo;
                    txtproducto.Text = rt._Producto.Nombre;
                    txtpreciocompra.Select();
                }
                else
                {
                    txtidproducto.Select();
                }
            }
        }

        private void txtcodproducto_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter) {

                Producto oProducto = new CN_Producto().Listar().Where(p => p.Codigo == txtcodproducto.Text && p.Estado == true).FirstOrDefault();

                if (oProducto != null) {
                    txtcodproducto.BackColor = System.Drawing.Color.Honeydew;
                    txtidproducto.Text = oProducto.IdProducto.ToString();
                    txtproducto.Text = oProducto.Nombre;
                    txtpreciocompra.Select();
                }
                else
                {
                    txtcodproducto.BackColor = System.Drawing.Color.MistyRose;
                    txtidproducto.Text = "0";
                    txtcodproducto.Text = "";
                }
            }


        }

        private void btnagregarprducto_Click(object sender, EventArgs e)
        {
            decimal preciocompra = 0;
            decimal precioventa = 0;
            bool producto_existe = false;

            if (int.Parse(txtidproducto.Text) == 0) { 
                MessageBox.Show("Debe seleccionar un producto!","Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtpreciocompra.Text, out preciocompra))
            {
                MessageBox.Show("Error! Precio Compra-Formato de moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtprecioventa.Text, out precioventa))
            {
                MessageBox.Show("Error! Precio Venta-Formato de moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (DataGridViewRow fila in dgvdata.Rows)
            {

                if  (fila.Cells["IdProducto"].Value != null && fila.Cells["IdProducto"].Value.ToString() == txtidproducto.Text)
                {
                    producto_existe = true;  
                    break;
                }
            }

            if (!producto_existe) 
            {
                dgvdata.Rows.Add(new object[]
                {
                  txtidproducto.Text,
                  txtproducto.Text,
                  preciocompra.ToString("0.00"),
                  precioventa.ToString("0,00"),
                  txtcantidad.Value.ToString(),
                  (txtcantidad.Value * preciocompra).ToString("0.00")


                });

                CalcualTotal();
                LimpiarProducto();
                txtcodproducto.Select();    
            }
     
        }
        private void LimpiarProducto()
        {
            txtidproducto.Text = "0";
            txtcodproducto.Text = "";
            txtcodproducto.BackColor = System.Drawing.Color.White;
            txtproducto.Text = "";
            txtpreciocompra.Text = "";
            txtprecioventa.Text = "";
            txtcantidad.Value = 1;
        }

        private void CalcualTotal()
        {
            decimal total = 0;
            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    
                    if (row.Cells["MontoTotal"].Value != null)
                    {
                        total += Convert.ToDecimal(row.Cells["MontoTotal"].Value.ToString());
                    }
                }
            }
            txttotalpago.Text = total.ToString("0.00");
        }


        private void dgvdata_CellPainting_1(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 6)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var W = Properties.Resources.compartimiento.Width;
                var H = Properties.Resources.compartimiento.Height;
                var X = e.CellBounds.Left + (e.CellBounds.Width - W) / 2;
                var Y = e.CellBounds.Top + (e.CellBounds.Width - H) / 15 ;

                e.Graphics.DrawImage(Properties.Resources.compartimiento, new Rectangle(X, Y, W, H));
                e.Handled = true;

            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btneliminar")
            {
                int indice = e.RowIndex;
                if (indice >= 0)
                {
                  dgvdata.Rows.RemoveAt(indice);
                    CalcualTotal();

                }
            }
        }

        private void txtpreciocompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtpreciocompra.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".") {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }

                }
            }   
        }

        private void txtprecioventa_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtprecioventa.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }

                }
            }
        }
        private void btnregistrar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtidproveedor.Text) == 0)
            {
                MessageBox.Show("Error: Debes seleccionar un proveedor", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Error: Debes ingresar productos en la compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            DataTable detalle_compra = new DataTable();

            detalle_compra.Columns.Add("IdProducto", typeof(int));
            detalle_compra.Columns.Add("PrecioCompra", typeof(decimal));
            detalle_compra.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_compra.Columns.Add("Cantidad", typeof(int));
            detalle_compra.Columns.Add("MontoTotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                detalle_compra.Rows.Add(
                    new object[] {
                          Convert.ToInt32(row.Cells["IdProducto"].Value.ToString()),
                         Convert.ToDecimal(row.Cells["PrecioCompra"].Value.ToString()),
                         Convert.ToDecimal(row.Cells["PrecioVenta"].Value.ToString()),
                         Convert.ToInt32(row.Cells["Cantidad"].Value.ToString()),
                         Convert.ToDecimal(row.Cells["MontoTotal"].Value.ToString())
                    });
            }



            int idcorrelativo = new CN_Compra().ObteberCorrelativo();
            string numerodocumento = string.Format("{0:00000}", idcorrelativo);

            Compra oCompra = new Compra()
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
              oProveedor = new Proveedor() { IdProveedor = Convert.ToInt32(txtidproveedor.Text) },
              TipoDocumento =((OpcionCombo) cbotipodocumento.SelectedItem).Texto,
              NumeroDocumento = numerodocumento,
               MontoTotal = Convert.ToDecimal(txttotalpago.Text)

            };
               string mensaje = string.Empty;
               bool respuesta = new CN_Compra().Registrar(oCompra, detalle_compra, out mensaje);

            if (respuesta)
            {

                var resulto = MessageBox.Show("Numero de compra generada:\n" + numerodocumento + "\n\n ¿Desea copiar al portapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (resulto == DialogResult.Yes)
                    Clipboard.SetText(numerodocumento);

                txtidproveedor.Text = "0";
                txtdocproveedor.Text = "0";
                txtnombreproveedor.Text = "";
                dgvdata.Rows.Clear();
                CalcualTotal();
            }
            else
            { 
              MessageBox.Show(mensaje, "Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Error);   
            }
        }

    }
}
 