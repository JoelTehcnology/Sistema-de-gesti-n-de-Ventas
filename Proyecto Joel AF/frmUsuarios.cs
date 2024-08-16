using Proyecto_Joel_AF.Utilidades;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using System.Drawing;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace Proyecto_Joel_AF
{
    public partial class frmUsuarios : Form
    {
        public frmUsuarios()
        {
            InitializeComponent();
        }

        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 0;


            List<Rol> listaRol = new CN_Rol().Listar();

            foreach (Rol item in listaRol)
            {
                cborol.Items.Add(new OpcionCombo() { Valor = item.IdRol, Texto = item.Descripcion });
                cborol.DisplayMember = "Texto";
                cborol.ValueMember = "Valor";
                cborol.SelectedIndex = 0;

            }

            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnseleccionar")
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }

            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;


            btnguardar.Enabled = false;


            //MOSTRAR TODOS LOS UDUARIOS XD

            List<Usuario> listaUsuario = new CN_Usuario().Listar();

            foreach (Usuario item in listaUsuario)
            {
                dgvdata.Rows.Add(new object[] {"",item.IdUsuario,item.Documento,item.NombreCompleto,item.Correo,item.Clave,
                  item.oRol.IdRol,
                  item.oRol.Descripcion,
                  item.Estado == true ?1 :0,
                  item.Estado == true ? "Activo" :"No Activo"


                });

            }
        }



        private void btnguardar_Click(object sender, EventArgs e)
        {
            String mensaje = String.Empty;

            Usuario objusuario = new Usuario()
            {
                IdUsuario = Convert.ToInt32(txtid.Text),
                Documento = txtdocumento.Text,
                NombreCompleto = txtnombrecompleto.Text,
                Correo = txtcorreo.Text,
                Clave = txtclave.Text,
                oRol = new Rol() { IdRol = Convert.ToInt32(((OpcionCombo)cborol.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1 ? true : false
            };

            if (objusuario.IdUsuario == 0)
            {

                int idusuariogenerado = new CN_Usuario().Registrar(objusuario, out mensaje);
                if (idusuariogenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] {"",idusuariogenerado,txtdocumento.Text,txtnombrecompleto.Text,txtcorreo.Text,txtclave.Text,
                        ((OpcionCombo)cborol.SelectedItem).Valor.ToString(),
                         ((OpcionCombo)cborol.SelectedItem).Texto.ToString(),
                           ((OpcionCombo)cboestado.SelectedItem).Valor.ToString(),
                         ((OpcionCombo)cboestado.SelectedItem).Texto.ToString()

                   });

                    Limpiar();


                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }

            else
            {
                bool Resultado = new CN_Usuario().Editar(objusuario, out mensaje);

                if (Resultado)
                {
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Documento"].Value = txtdocumento.Text;
                    row.Cells["NombreCompleto"].Value = txtnombrecompleto.Text;
                    row.Cells["Correo"].Value = txtcorreo.Text;
                    row.Cells["Clave"].Value = txtclave.Text;
                    row.Cells["IdRol"].Value = ((OpcionCombo)cborol.SelectedItem).Valor.ToString();
                    row.Cells["Rol"].Value = ((OpcionCombo)cborol.SelectedItem).Texto.ToString();
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cboestado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cboestado.SelectedItem).Texto.ToString();
                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }


            }



        }
        //VALIDAR CAMPOS..TEXBOXS-
        private void ValidarCampo()
        {
            var vr = !string.IsNullOrEmpty(txtdocumento.Text) &&
              !string.IsNullOrEmpty(txtnombrecompleto.Text) &&
                !string.IsNullOrEmpty(txtcorreo.Text) &&
               !string.IsNullOrEmpty(txtclave.Text) &&
                !string.IsNullOrEmpty(txtconfirmarclave.Text);
            btnguardar.Enabled = vr;

        }


        private void Limpiar()
        {
            txtbusqueda.Text = "";
            txtindice.Text = "0";
            txtid.Text = "0";
            txtdocumento.Text = "";
            txtnombrecompleto.Text = "";
            txtcorreo.Text = "";
            txtclave.Text = "";
            txtconfirmarclave.Text = "";
          
        }

        private void txtdocumento_TextChanged(object sender, EventArgs e)
        {
            ValidarCampo();
        }

        private void txtnombrecompleto_TextChanged(object sender, EventArgs e)
        {
            ValidarCampo();
        }

        private void txtcorreo_TextChanged(object sender, EventArgs e)
        {
            ValidarCampo();
        }

        private void txtclave_TextChanged(object sender, EventArgs e)
        {
            ValidarCampo();
        }

        private void txtconfirmarclave_TextChanged(object sender, EventArgs e)
        {
            ValidarCampo();
        }
        //Almacenar icono en DataGrid..Ancho y largo.. 
        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var W = Properties.Resources.cheque.Width;
                var H = Properties.Resources.cheque.Height;
                var X = e.CellBounds.Left + (e.CellBounds.Width - W) / 2;
                var Y = e.CellBounds.Top + (e.CellBounds.Width - H) / 5;

                e.Graphics.DrawImage(Properties.Resources.cheque, new Rectangle(X, Y, W, H));
                e.Handled = true;

            }

        }
        //MOSTRAR LOS VALORES DEL DATA GRID EN EL TEXBOX 
        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnseleccionar")
            {
                int indice = e.RowIndex;
                if (indice >= 0)
                {
                    txtindice.Text = indice.ToString();
                    txtid.Text = dgvdata.Rows[indice].Cells["Id"].Value.ToString();
                    txtdocumento.Text = dgvdata.Rows[indice].Cells["Documento"].Value.ToString();
                    txtnombrecompleto.Text = dgvdata.Rows[indice].Cells["NombreCompleto"].Value.ToString();
                    txtcorreo.Text = dgvdata.Rows[indice].Cells["Correo"].Value.ToString();
                    txtclave.Text = dgvdata.Rows[indice].Cells["Clave"].Value.ToString();
                    txtconfirmarclave.Text = dgvdata.Rows[indice].Cells["Clave"].Value.ToString();

                    foreach (OpcionCombo oc in cborol.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["IdRol"].Value))
                        {
                            int indice_combo = cborol.Items.IndexOf(oc);
                            cborol.SelectedIndex = indice_combo;
                            break;
                        }
                    }


                    foreach (OpcionCombo oc in cboestado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indice_combo = cboestado.Items.IndexOf(oc);
                            cboestado.SelectedIndex = indice_combo;
                            break;
                        }
                    }





                }
            }
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("DESEA ELEMINAR EL USUARIO?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Mensaje= string.Empty;
                    Usuario objusuario = new Usuario()
                    {
                        IdUsuario = Convert.ToInt32(txtid.Text),

                    };
                    bool Respuesta = new CN_Usuario().Eliminar(objusuario, out Mensaje);

                    if (Respuesta) 
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                        
                    }
                    else 
                    {
                      MessageBox.Show(Mensaje,"Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Warning);  
                    }
                }   
            }
            Limpiar();

        }


        //BUSCAR DATOS DEL DATAGRID EN UN TEXBOX 
        private void btnbuscar_Click(object sender, EventArgs e)
        {
            string columnabusqueda = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            if(dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row  in dgvdata.Rows)
                {

                    if (row.Cells[columnabusqueda].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper())) 
                    
                        row.Visible = true;
                    
                    else
                    
                    row.Visible=false;  
                    
                }
            }
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text= "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;

            }

        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

      
    }
}

