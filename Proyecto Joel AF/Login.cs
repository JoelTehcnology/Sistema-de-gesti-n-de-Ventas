using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio;
using CapaEntidad;
using System.Diagnostics.Eventing.Reader;

namespace Proyecto_Joel_AF
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        //LIMPIAR TEXTOS "USUARIO" Y "CONTRASEÑA"
      private void txtuser_Enter(object sender, EventArgs e)
    {
        if (txtuser.Text == "USUARIO")
        {
            txtuser.Text = "";
            txtuser.ForeColor = Color.LightGray;
        }

    }
    private void txtuser_Leave(object sender, EventArgs e)
    {
        if (txtuser.Text == "")
        {

            txtuser.Text = "USUARIO";
            txtuser.ForeColor = Color.DimGray;
        }

    }

    private void txtpass_Enter(object sender, EventArgs e)
    {
        if (txtpass.Text == "CONTRASEÑA")
        {

                txtpass.Text = "";
            txtpass.ForeColor = Color.LightGray;
            txtpass.UseSystemPasswordChar = true;

        }

    }

    private void txtpass_Leave(object sender, EventArgs e)
    {
        if (txtpass.Text == "")
        {

            txtpass.Text = "CONTRASEÑA";
            txtpass.ForeColor = Color.DimGray;
                txtpass.UseSystemPasswordChar = false;
            }
    }

        //VALIDACION DE USUARIO CON BASE DE DATOS
        private void btningresar_Click(object sender, EventArgs e)
        {

            if (txtuser.Text != "USUARIO")
            {
                if (txtpass.Text != "CONTRASEÑA")
                {

                    List<Usuario> TEST = new CN_Usuario().Listar();

                    Usuario ousuario = new CN_Usuario().Listar().Where(u => u.Documento == txtuser.Text && u.Clave == txtpass.Text).FirstOrDefault();

                    //LIMPIAR TEXTOS Y ENTRAR AL SEGUNDO FORMULARIO SI EL ININIO DE SESSION FUE EXITOSO
                    if (ousuario != null)
                    {

                        Form Inicio = new Inicio(ousuario);
                        Inicio.Show();
                        this.Hide();
                        Inicio.FormClosing += frm_closing;
                    }


                    else msgError("   Usuario o contraseña incorrecto!");
                    {

                        txtuser.Focus();
                        txtpass.Text = "CONTRASEÑA";
                        txtpass.Clear();
                        txtuser.Clear();
                        txtpass.UseSystemPasswordChar = true;

                    }
                    if (ousuario == null)
                    {
                        txtpass.Text = "CONTRASEÑA";
                        txtpass.ForeColor = Color.DimGray;
                        txtpass.UseSystemPasswordChar = false;

                    }


                }
                else msgError("   Porfavor escriba su contraseña!");
            }
            else msgError("   Porfavor escriba su usuario!");
        }   


       //REFENCIAL DEL FRM
        private void frm_closing(object sender, EventArgs e)
        {
            
    
            
        }

        //MINIMIZAR Y CERRAR FORMULARIO
        private void btnminimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        private void btncerrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(" Desea salir del programa?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
           
        }

        ///MOVER EL FORMULARIO///

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnsalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(" Desea salir del programa?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void msgError(string msg)
        {
            lblErrorMessage.Text = "   " + msg;
            lblErrorMessage.Visible = true;

        }


    }
}
