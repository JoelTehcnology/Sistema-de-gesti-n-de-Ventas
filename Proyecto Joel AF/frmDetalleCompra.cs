using CapaEntidad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.IO;
using DocumentFormat.OpenXml.Drawing.Diagrams;




namespace Proyecto_Joel_AF
{
    public partial class frmDetalleCompra : Form
    {
        public frmDetalleCompra()
        {
            InitializeComponent();
        }


        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            txtfecha.Text = "";
            txttipodocumento.Text = "";
            txtusuario.Text = "";
            txtdocumentoproveedor.Text = "";
            txtempresa.Text = "";
            txtmontototal.Text = "0.00";
            dgvdata.Rows.Clear();

        }



        private void btnexcel_Click(object sender, EventArgs e)
        {
            if (txttipodocumento.Text == "")
            {
                MessageBox.Show("No se encuentran resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string Texto_Html = Properties.Resources.PlantillaDeCompra.ToString();
            Negocio odatos = new CN_Negocio().ObtenerDatos();

            Texto_Html = Texto_Html.Replace("@nombrenegocio", odatos.Nombre.ToUpper());
            Texto_Html = Texto_Html.Replace("@docnegocio", odatos.RUC);
            Texto_Html = Texto_Html.Replace("@direcnegocio", odatos.Direccion);
            Texto_Html = Texto_Html.Replace("@tipodocumento", txttipodocumento.Text.ToUpper());
            Texto_Html = Texto_Html.Replace("@numerodocumento", txtnumerodocumento.Text);
            Texto_Html = Texto_Html.Replace("@docproveedor", txtdocumentoproveedor.Text);
            Texto_Html = Texto_Html.Replace("@nombreproveedor", txtempresa.Text);
            Texto_Html = Texto_Html.Replace("@fecharegistro", txtfecha.Text);
            Texto_Html = Texto_Html.Replace("@usuarioregistro", txtusuario.Text);

            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["producto"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["PrecioCompra"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["SubTotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }
            Texto_Html = Texto_Html.Replace("@filas", filas);
            Texto_Html = Texto_Html.Replace("@montototal", txtmontototal.Text);

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = String.Format("ReporteCompra_ {0}.pdf.", txtnumerodocumento.Text);
            saveFile.Filter = "Pdf Files | ";

            if (saveFile.ShowDialog() == DialogResult.OK)
                using (FileStream stream = new FileStream(saveFile.FileName, FileMode.Create))
                {
                    Document pdfdoc = new Document(PageSize.A4, 25, 25, 25, 25);

                    PdfWriter writer = PdfWriter.GetInstance(pdfdoc, stream);
                    pdfdoc.Open();

                    bool obtenido = true;
                    byte[] byteImage = new CN_Negocio().ObtenerLogo(out obtenido);

                    if (obtenido)
                    {
                        iTextSharp.text.Image IMG = iTextSharp.text.Image.GetInstance(byteImage);
                        IMG.ScaleToFit(60, 60);
                        IMG.Alignment = iTextSharp.text.Image.UNDERLYING;
                        IMG.SetAbsolutePosition(pdfdoc.Left, pdfdoc.GetTop(51));
                        pdfdoc.Add(IMG);
                    }
                    using (StringReader sr = new StringReader(Texto_Html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfdoc, sr);
                    }
                    pdfdoc.Close();
                    stream.Close();
                    MessageBox.Show("Documento Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

        }
        private void btnbuscar_Click(object sender, EventArgs e)
        {

            if (txtbusqueda.Text == "")
            {
                MessageBox.Show("No se encuentran resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            // Obtener la compra según el texto ingresado en txtbusqueda
            Compra oCompra = new CN_Compra().ObtenerCompra(txtbusqueda.Text);

            if (oCompra.IdCompra != 0)
            {
                // Si se encontró una compra, llenar los campos correspondientes
                txtnumerodocumento.Text = oCompra.NumeroDocumento;
                txtfecha.Text = oCompra.FechaCreacion;
                txttipodocumento.Text = oCompra.TipoDocumento;
                txtusuario.Text = oCompra.oUsuario.NombreCompleto;
                txtdocumentoproveedor.Text = oCompra.oProveedor.Documento;
                txtempresa.Text = oCompra.oProveedor.RazonSocial;

                 // Limpiar el DataGridView y llenarlo con los detalles de la compra
              dgvdata.Rows.Clear();
              foreach (Detalle_Compra dc in oCompra.oDetalleCompra)
              {
                dgvdata.Rows.Add(new object[] { dc.oProducto.Nombre, dc.PrecioCompra, dc.Cantidad, dc.MontoTotal });
              }

                // Mostrar el monto total en txtmontototal
                txtmontototal.Text = oCompra.MontoTotal.ToString("0.00");
            }
            else
            {

                MessageBox.Show("No se encontró ninguna compra con el número ingresado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

           txtbusqueda.Text="";

        }
    }
}
