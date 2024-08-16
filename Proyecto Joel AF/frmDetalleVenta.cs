using CapaEntidad;
using CapaNegocio;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Joel_AF
{
    public partial class frmDetalleVenta : Form
    {
        public frmDetalleVenta()
        {
            InitializeComponent();
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            // Obtener la venta
            Venta oVenta = new CN_Venta().ObtenerVenta(txtbusqueda.Text);

            if (oVenta.IdVenta != 0)
            {
                // Actualizar los controles de texto
                txtnumerodocumento.Text = oVenta.NumeroDocumento;
                txtfecha.Text = oVenta.FechaCreacion;
                txttipodocumento.Text = oVenta.TipoDocumento;
                txtusuario.Text = oVenta.oUsuario.NombreCompleto;
                txtdocliente.Text = oVenta.DocumentoCliente;
                txtnombrecliente.Text = oVenta.NombreCliente;

                // Limpiar las filas existentes en el DataGridView
                dgvdata.Rows.Clear();

                // Agregar detalles de venta al DataGridView
                foreach (Detalle_Venta dv in oVenta.oDetalleVenta)
                {
                    dgvdata.Rows.Add(dv.oProducto.Nombre, dv.PrecioVenta, dv.Cantidad, dv.SubTotal);
                }

                // Actualizar otros controles
                txtmontototal.Text = oVenta.MontoTotal.ToString("0.00");
                txtmontopago.Text = oVenta.MontoPago.ToString("0.00");
                txtcambio.Text = oVenta.MontoCambio.ToString("0.00");
            }
        }

        private void btnpdf_Click(object sender, EventArgs e)
        {
            if (txttipodocumento.Text == "")
            {
                MessageBox.Show("No se encuentran resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string Texto_Html = Properties.Resources.PlantillaVenta.ToString();
            Negocio odatos = new CN_Negocio().ObtenerDatos();

            Texto_Html = Texto_Html.Replace("@nombrenegocio", odatos.Nombre.ToUpper());
            Texto_Html = Texto_Html.Replace("@docnegocio", odatos.RUC);
            Texto_Html = Texto_Html.Replace("@direcnegocio", odatos.Direccion);
            Texto_Html = Texto_Html.Replace("@tipodocumento", txttipodocumento.Text.ToUpper());
            Texto_Html = Texto_Html.Replace("@numerodocumento", txtnumerodocumento.Text);
            Texto_Html = Texto_Html.Replace("@docproveedor", txtdocliente.Text);
            Texto_Html = Texto_Html.Replace("@nombreproveedor", txtnombrecliente.Text);
            Texto_Html = Texto_Html.Replace("@fecharegistro", txtfecha.Text);
            Texto_Html = Texto_Html.Replace("@usuarioregistro", txtusuario.Text);

            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["producto"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["PrecioVenta"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["SubTotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }
            Texto_Html = Texto_Html.Replace("@filas", filas);
            Texto_Html = Texto_Html.Replace("@montototal", txtmontototal.Text);
            Texto_Html = Texto_Html.Replace("@pagocon", txtmontopago.Text);
            Texto_Html = Texto_Html.Replace("@cambio", txtcambio.Text);

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = String.Format("ReporteVenta_ {0}.pdf.", txtnumerodocumento.Text);
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

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            txtfecha.Text = "";
            txttipodocumento.Text = "";
            txtusuario.Text = "";
            txtdocliente.Text = "";
            txtnombrecliente.Text = "";
            txtbusqueda.Text = "";
            txtnumerodocumento.Text = "";
            txtmontototal.Text = "0.00";
            txtmontopago.Text = "0.00";
            txtcambio.Text = "0.00";
            dgvdata.Rows.Clear();
        }

    }
}
