using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public partial class frm_Login : Form
    {
        public frm_Login()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            // Credenciais para teste
            string usuarioCorreto = "admin";
            string senhaCorreta = "1234";

            // Dados digitados
            string usuarioDigitado = txtUser.Text;
            string senhaDigitada = txtPass.Text;

            // Verificação do login
            if (usuarioDigitado == usuarioCorreto && senhaDigitada == senhaCorreta)
            {
                MessageBox.Show(
                    "Login realizado com sucesso!",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Abre o formulário MDI_Menu
                MDI_Menu menu = new MDI_Menu();
                menu.Show();

                // Esconde a tela de login
                this.Hide();
            }
            else
            {
                MessageBox.Show(
                    "Usuário ou senha incorretos!",
                    "Erro de Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                txtPass.Clear();
                txtPass.Focus();
            }
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
