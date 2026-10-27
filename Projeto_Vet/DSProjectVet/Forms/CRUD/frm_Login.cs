using DSProjectVet.Dados.DataSet_ClinicaVetTableAdapters;
using Projeto_Agenda.Formularios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    using BCrypt.Net;

    public partial class frm_Login : Form
    {
        public frm_Login()
        {
            InitializeComponent();
        }

        int erros;
        string hash;

        private bool VerificarSenha(string senhaDigitada)
        {
            return BCrypt.Verify(senhaDigitada, hash);
        }

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

                    this.FormClosed += (s, args) => this.Close(); // Fecha o formulário de login quando o menu for fechado
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
            
        }

        private async Task<bool> SingIn(string user, string password)
        {
            try
            {
                await Task.Delay(1000); // simula um tempo de espera para a conexão com o banco de 
                FuncionariosTableAdapter taFuncionarios = new FuncionariosTableAdapter();

                DataTable dt = await Task.Run(() => taFuncionarios.LoginUser(user));
                if (dt.Rows.Count > 0) // verifica se retornou alguma linha. Caso exista, user e pass estão corretos
                {
                    hash = dt.Rows[0]["SenhaHash"].ToString();

                    if (VerificarSenha(password))
                    {
                        DSProjectVet.Properties.Settings.Default.NomeUsuarioLogado = dt.Rows[0]["Nome"].ToString(); // seta a var "NomeUsuarioLogado" com o nome do user caso esteja tudo correto
                        DSProjectVet.Properties.Settings.Default.NivelUsuarioLogado = int.Parse(dt.Rows[0]["NivelAcesso"].ToString());
                        MessageBox.Show(dt.Rows[0]["Nome"].ToString());
                        return true;
                        //dt.Rows[0]["NIVEL"] -> na linha zero, coluna NIVEL, pega o valor do nível do user
                    }
                }

                erros++;
                return false;
            } catch (Exception ex)
            {
                MessageBox.Show($"Erro ao tentar se conectar com o banco de dados!\n {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {

        }
    }
}