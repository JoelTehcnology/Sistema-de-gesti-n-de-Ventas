using CapaEntidad;
using CapaNegocio;
using ClosedXML.Excel;
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
    public partial class frmReporteVentas : Form
    {
        public frmReporteVentas()
        {
            InitializeComponent();
        }

        private void txtfechainicio_ValueChanged(object sender, EventArgs e)
        {

        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {

            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;
        }
        private void btnbuscar_Click(object sender, EventArgs e)
        {
                // Formatear las fechas en el formato ISO 'yyyy-MM-dd'
                string fechainicio = txtfechainicio.Value.ToString("yyyy-MM-dd");
                string fechafin = txtfechafin.Value.ToString("yyyy-MM-dd");

                // Obtener los datos de ventas utilizando el método ajustado con el formato de fecha
                List<ReporteVenta> lista = new CN_Reporte().Venta(fechainicio, fechafin);

                dgvdata.Rows.Clear();

                foreach (ReporteVenta rv in lista)
                {
                      dgvdata.Rows.Add(new object[]
                      {
                            rv.FechaCreacion,
                            rv.TipoDocumento,
                            rv.NumeroDocumento,
                            rv.MontoTotal,
                            rv.UsuarioRegistro,
                            rv.DocumentoCliente,
                            rv.NombreCliente,
                            rv.CodigoProducto,
                            rv.NombreProducto,
                            rv.Categoria,
                            rv.PrecioVenta,
                            rv.Cantidad,
                            rv.SubTotal
                      });
                }
            
        }



        private void btnbuscarpor_Click(object sender, EventArgs e)
        {
            string columnabusqueda = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {

                    if (row.Cells[columnabusqueda].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))

                        row.Visible = true;

                    else

                        row.Visible = false;

                }
            }
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;

            }
        }

        private void btnexcel_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para guardar!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn columna in dgvdata.Columns)
                {

                    dt.Columns.Add(columna.HeaderText, typeof(string));
                }
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Visible)
                        dt.Rows.Add(new Object[]
                        {
                            row.Cells[0].Value.ToString(),
                            row.Cells[1].Value.ToString(),
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString(),
                            row.Cells[4].Value.ToString(),
                            row.Cells[5].Value.ToString(),
                            row.Cells[6].Value.ToString(),
                            row.Cells[7].Value.ToString(),
                            row.Cells[8].Value.ToString(),
                            row.Cells[9].Value.ToString(),
                            row.Cells[10].Value.ToString(),
                            row.Cells[11].Value.ToString(),
                            row.Cells[12].Value.ToString()
                           

                        });

                }



                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.FileName = String.Format("RepórteVenta_ {0}.xlsx ", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                saveFile.Filter = "Excel Files | *.xlsx";

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        XLWorkbook wb = new XLWorkbook();
                        var hoja = wb.Worksheets.Add(dt, "Informe");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(saveFile.FileName);

                        MessageBox.Show("REPORTE GURARDADO", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("ERROR AL GENERAR REPORTE", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

     
    }
}
