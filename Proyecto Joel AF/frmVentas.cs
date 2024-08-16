using CapaEntidad;
using CapaNegocio;
using Proyecto_Joel_AF.Modales;
using Proyecto_Joel_AF.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Joel_AF
{
    public partial class frmVentas : Form
    {
        private Usuario _Usuario;
        public frmVentas(Usuario oUsuario = null)
        {
            _Usuario = oUsuario; 
            InitializeComponent();
        }

        private void frmVentas_Load(object sender, EventArgs e)
        {
            {

                cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
                cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
                cbotipodocumento.DisplayMember = "Texto";
                cbotipodocumento.ValueMember = "Valor";
                cbotipodocumento.SelectedIndex = 0;

                //MOSTRAR A FECHA
                txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

                txttotalpago.Text = "";
                txtidproducto.Text = "0";
                txtcantidad.Text = "0"; 
                txtcambio.Text = "";

            }

        }

        private void btnbuscarproveedor_Click(object sender, EventArgs e)
        {
            using (var rt = new mdCliente())
            {
                var result = rt.ShowDialog();
                if (result == DialogResult.OK)
                {
                    
                    txtnombrecompleto.Text = rt._Cliente.NombreCompleto;
                    txtnumerodocumento.Text = rt._Cliente.Documento;
                    txtcodproducto.Select();
                }
                else
                {
                    txtnumerodocumento.Select();
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
                    txtprecio.Text = rt._Producto.PrecioVenta.ToString("0.00");
                    txtstock.Text = rt._Producto.Stock.ToString();
                    txtcantidad.Select();
                }
                else
                {
                   txtcodproducto.Select();
                }
            }
        }

        private void txtcodproducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

                Producto oProducto = new CN_Producto().Listar().Where(p => p.Codigo == txtcodproducto.Text && p.Estado == true).FirstOrDefault();

                if (oProducto != null)
                {
                    txtcodproducto.BackColor = System.Drawing.Color.Honeydew;
                    txtidproducto.Text = oProducto.IdProducto.ToString();
                    txtproducto.Text = oProducto.Nombre;
                    txtprecio.Text = oProducto.PrecioVenta.ToString("0,00");   
                    txtstock.Text = oProducto.Stock.ToString();
                    txtcantidad.Select();
                }
                else
                {
                    txtcodproducto.BackColor = System.Drawing.Color.MistyRose;
                    txtidproducto.Text = "0";
                    txtproducto.Text = "";
                    txtprecio.Text = "";
                    txtstock.Text = "";
                    txtcantidad.Value = 1;
                }
            }
        }

        private void btnagregarprducto_Click(object sender, EventArgs e)
        {
               decimal precio = 0;
           
            bool producto_existe = false;

            if (int.Parse(txtidproducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtprecio.Text, out precio))
            {
                MessageBox.Show("Error! Precio Compra-Formato de moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Convert.ToUInt32(txtstock.Text) < Convert.ToInt32(txtcantidad.Value.ToString()))
            {
                MessageBox.Show("Error!la cantidad no puede ser mayor al sctok", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (DataGridViewRow fila in dgvdata.Rows)
            {

                if (fila.Cells["IdProducto"].Value != null && fila.Cells["IdProducto"].Value.ToString() == txtidproducto.Text)
                {
                    producto_existe = true;
                    break;
                }
            }

            if (!producto_existe)
            {
               
                bool respuesta = new CN_Venta().RestarStock(Convert.ToInt32(txtidproducto.Text),
                    Convert.ToInt32(txtcantidad.Value.ToString())
                    );
                if (respuesta)
                {

                    dgvdata.Rows.Add(new object[]
                    {
                           txtidproducto.Text,
                          txtproducto.Text,
                          precio.ToString("0.00"),
                          txtcantidad.Value.ToString(),
                          (txtcantidad.Value * precio).ToString("0.00")


                    });

                    CalcualTotal();
                    LimpiarProducto();
                    txtcodproducto.Select();

                }

   
    
            }
        }

        private void LimpiarProducto()
        {
            txtidproducto.Text = "0";
            txtcodproducto.Text = "";
            txtcodproducto.BackColor = System.Drawing.Color.White;
            txtprecio.Text = "";
            txtstock.Text = "";
            txtcantidad.Value = 1;
            txtproducto.Text = "";
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

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 5)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var W = Properties.Resources.compartimiento.Width;
                var H = Properties.Resources.compartimiento.Height;
                var X = e.CellBounds.Left + (e.CellBounds.Width - W) / 2;
                var Y = e.CellBounds.Top + (e.CellBounds.Width - H) / 15;

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

                    bool respuesta = new CN_Venta().SumarStock(
                       Convert.ToInt32(dgvdata.Rows[indice].Cells["IdProducto"].Value.ToString()),
                       Convert.ToInt32(dgvdata.Rows[indice].Cells["Cantidad"].Value.ToString()));

                    if (respuesta)
                    {
                        dgvdata.Rows.RemoveAt(indice);
                        CalcualTotal();
                    }

                 

                }
            }
        }

        private void txtprecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtprecio.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void txtpagacon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtpagacon.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void calcularcambio()
        {
            if (txttotalpago.Text.Trim() == "")
            {
                MessageBox.Show("No existen productos en la venta", "Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Error);  
                return;
            }

            decimal pagacon;
            decimal total = Convert.ToDecimal(txttotalpago.Text);   


            if(txtpagacon.Text.Trim() == "") 
            {
                txtpagacon.Text = "0";  

            }

            if (decimal.TryParse(txtpagacon.Text.Trim(), out pagacon))
            {
                if(pagacon < total)
                {
                    txtcambio.Text = "0.00";   
                }
                else
                {
                    decimal cambio = pagacon - total;
                    txtcambio.Text = cambio.ToString("0.00");
                }
            }
        }

        private void txtpagacon_KeyDown(object sender, KeyEventArgs e)
        {
          if(e.KeyData == Keys.Enter)
          { 
            calcularcambio();
            
          }
        }

        private void btnregistrar_Click(object sender, EventArgs e)
        {
            if (txtnumerodocumento.Text == "")
            {
                MessageBox.Show("Debes ingresar el documento del cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtnombrecompleto.Text == "")
            {
                MessageBox.Show("Debes ingresar el nombre del cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debes ingresar productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


             DataTable detalle_venta = new DataTable();

            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_venta.Columns.Add("Cantidad", typeof(int));
            detalle_venta.Columns.Add("MontoTotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                detalle_venta.Rows.Add(new object[] {
                
              
                  row.Cells["IdProducto"].Value.ToString(),
                  row.Cells["Precio"].Value.ToString(),
                  row.Cells["Cantidad"].Value.ToString(),
                  row.Cells["MontoTotal"].Value.ToString()
              });
            
            }

            int idcorrelativo = new CN_Venta().ObteberCorrelativo();
            string numeroDocumento = string.Format("{0:00000}", idcorrelativo);
            calcularcambio();

            Venta oVenta = new Venta()
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numeroDocumento,
                DocumentoCliente = txtnumerodocumento.Text,
                NombreCliente = txtnombrecompleto.Text,
                MontoPago = Convert.ToDecimal(txtpagacon.Text),
                MontoCambio = Convert.ToDecimal(txtcambio.Text),
                MontoTotal = Convert.ToDecimal(txttotalpago.Text),



            };
            string mensaje = string.Empty;
            bool respuesta = new CN_Venta().Registrar(oVenta,detalle_venta, out mensaje);

            if (respuesta)
            {
                var result = MessageBox.Show("Numero de venta generado:\n" + numeroDocumento + "\n\nDesea copiar al portapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numeroDocumento);

                txtnumerodocumento.Text = "";
                txtnombrecompleto.Text = "";
                dgvdata.Rows.Clear();
                CalcualTotal();
                txtpagacon.Text = "";
                txtcambio.Text = "";
                txtproducto.Text = "";
            }
            else
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }

 
    }   

}
