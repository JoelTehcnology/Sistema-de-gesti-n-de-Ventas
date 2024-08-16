using CapaEntidad;
using CapaNegocio;
using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
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

namespace Proyecto_Joel_AF.Modales
{
    public partial class mdCliente : Form
    {
        public Cliente _Cliente {get; set; }
        public mdCliente()
        {
            InitializeComponent();
            this.dgvdata.CellMouseEnter += new DataGridViewCellEventHandler(this.dgvdata_CellMouseEnter);
            this.dgvdata.CellMouseLeave += new DataGridViewCellEventHandler(this.dgvdata_CellMouseLeave);
        }

        private void mdCliente_Load(object sender, EventArgs e)
        {

            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }

            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;


            //MOSTRAR TODOS LOS UDUARIOS XD

            List<Cliente> lista = new CN_Cliente().Listar();

            foreach (Cliente item in lista)
            {
                if (item.Estado)
                dgvdata.Rows.Add(new object[] { item.Documento, item.NombreCompleto});

                

            }

        }

        private void dgvdata_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                int iRow = e.RowIndex;
                int iColumn = e.ColumnIndex;

                // Verifica que el clic no se hizo en el encabezado o fuera del rango de celdas
                if (iRow >= 0 && iColumn >= 0)
                {
                    _Cliente = new Cliente()
                    {
                        Documento = dgvdata.Rows[iRow].Cells["Documento"].Value?.ToString(),
                        NombreCompleto = dgvdata.Rows[iRow].Cells["NombreCompleto"].Value?.ToString()
                    };
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se ha producido un error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;

            }
        }

        private void dgvdata_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dgvdata.Columns[e.ColumnIndex].Name;

                if (columnName == "Documento" || columnName == "NombreCompleto")
                {
                    dgvdata.Cursor = Cursors.Hand;
                }
                else
                {
                    dgvdata.Cursor = Cursors.Default;
                }
            }
            else
            {
                dgvdata.Cursor = Cursors.Default;
            }
        }

        private void dgvdata_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvdata.Cursor = Cursors.Default;
        }


    }
}
