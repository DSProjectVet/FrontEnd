using DSProjectVet.Dados.DataSet_ClinicaVetTableAdapters;
using Projeto_Agenda.Formularios;
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

        int erros;

        private async void btn_ok_Click(object sender, EventArgs e)
        {
            //loadingForm loadingForm = new loadingForm();

            // Credenciais para teste
            //string usuarioCorreto = "admin";
            //string senhaCorreta = "1234";

            // Dados digitados
            string usuarioDigitado = txtUser.Text;
            string senhaDigitada = txtPass.Text;

            if (txtUser.Text.Trim() == "")
            {
                errorProvider.SetError(txtUser, "Favor preencher corretamente!");
                return;
            } else
            {
                errorProvider.SetError(txtUser, "");
            }

            if(txtPass.Text.Trim() == "")
            {
                errorProvider.SetError(txtPass, "Favor preencher corretamente!");
                return;
            }
            else
            {
                errorProvider.SetError(txtPass, "");
            }

            try
            {
                //loadingForm.Show();
                btn_ok.Enabled = false;
                bool autenticado = await SingIn(usuarioDigitado, senhaDigitada);

                if(autenticado)
                {
                    MDI_Menu menu = new MDI_Menu();
                    menu.Show();
                    Hide();
                }
                else
                {
                    btn_ok.Enabled = true;
                    MessageBox.Show("Usuário ou senha incorreto!", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (erros == 3)
                    {
                        MessageBox.Show("Nº de tentativas esgotado", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.OpenForms.OfType<frm_SplashScreen>().First().Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao tentar se conectar com o banco de dados!\n {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                txtUser.Clear();
                txtPass.Clear();
                txtUser.Focus();
                btn_ok.Enabled = true;
                //loadingForm.Close();
            }

            //// Verificação do login
            //if (usuarioDigitado == usuarioCorreto && senhaDigitada == senhaCorreta)
            //{
            //    MessageBox.Show(
            //        "Login realizado com sucesso!",
            //        "Login",
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Information
            //    );

            //    // Abre o formulário MDI_Menu
            //    MDI_Menu menu = new MDI_Menu();
            //    menu.Show();

            //    // Esconde a tela de login
            //    this.Hide();
            //}
            //else
            //{
            //    MessageBox.Show(
            //        "Usuário ou senha incorretos!",
            //        "Erro de Login",
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Error
            //    );

            //    txtPass.Clear();
            //    txtPass.Focus();
            //}
        }

        private async Task<bool> SingIn(string user, string password)
        {
            await Task.Delay(1000); // simula um tempo de espera para a conexão com o banco de 
            VeterinariosTableAdapter taUsuario = new VeterinariosTableAdapter();

            DataTable dt = await Task.Run(() => taUsuario.Procurar_Usuario(user, password));
            if (dt.Rows.Count > 0) // verifica se retornou alguma linha. Caso exista, user e pass estão corretos
            {
                DSProjectVet.Properties.Settings.Default.NomeUsuarioLogado = dt.Rows[0]["Nome"].ToString(); // seta a var "NomeUsuarioLogado" com o nome do user caso esteja tudo correto
                DSProjectVet.Properties.Settings.Default.NivelUsuarioLogado = int.Parse(dt.Rows[0]["Nivel"].ToString());
                MessageBox.Show(dt.Rows[0]["Nome"].ToString());
                return true;
                //dt.Rows[0]["NIVEL"] -> na linha zero, coluna NIVEL, pega o valor do nível do user
            }

            erros++;
            return false;
        }
    }
}