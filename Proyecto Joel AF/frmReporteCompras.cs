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
    public partial class frmReporteCompras : Form
    {
        public frmReporteCompras()
        {
            InitializeComponent();
        }

        private void frmReporteCompras_Load(object sender, EventArgs e)
        {
            List<Proveedor> lista = new CN_Proveedor().Listar();

            cboproveedor.Items.Add(new OpcionCombo() { Valor = 0, Texto = "TODOS" });
            foreach (Proveedor item in lista)
            {
                cboproveedor.Items.Add(new OpcionCombo() { Valor = item.IdProveedor, Texto = item.RazonSocial});    
            }
            cboproveedor.DisplayMember = "Texto";
            cboproveedor.ValueMember = "Valor"; 
            cboproveedor.SelectedIndex = 0;

            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

        }

        private void btnbuscarcompra_Click(object sender, EventArgs e)
        {
                    int idproveedor = Convert.ToInt32(((OpcionCombo)cboproveedor.SelectedItem).Valor.ToString());

                    // Formatear las fechas en el formato ISO 'yyyy-MM-dd'
                    string fechainicio = txtfechainicio.Value.ToString("yyyy-MM-dd");
                    string fechafin = txtfechafin.Value.ToString("yyyy-MM-dd");
                
                    List<ReporteCompra> lista = new CN_Reporte().Compra(
                        fechainicio,
                        fechafin,
                        idproveedor
                    );

                    dgvdata.Rows.Clear();

                    foreach (ReporteCompra rc in lista)
                    {
                        dgvdata.Rows.Add(new object[] {

                            rc.FechaCreacion,
                            rc.TipoDocumento,
                            rc.NumeroDocumento,
                            rc.MontoTotal,
                            rc.UsuarioRegistro,
                            rc.DocumentoProveedor,
                            rc.RazonSocial,
                            rc.CodigoProducto,
                            rc.NombreProducto,
                            rc.Categoria,
                            rc.PrecioCompra,
                            rc.PrecioVenta,
                            rc.Cantidad,
                            rc.SubTotal
                        });
                    }
        }


        private void btnexcel_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count <1) 
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
                            row.Cells[12].Value.ToString(),
                            row.Cells[13].Value.ToString()

                        });

                }



                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.FileName = String.Format("RepórteCompra_ {0}.xlsx ", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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

        private void btnbuscar_Click(object sender, EventArgs e)
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

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;

            }
        }
    }
}
