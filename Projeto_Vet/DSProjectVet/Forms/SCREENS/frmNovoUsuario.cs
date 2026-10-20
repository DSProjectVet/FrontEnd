using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{

    using BCrypt.Net;
    using DSProjectVet.Dados.DataSet_ClinicaVetTableAdapters;

    public partial class frmNovoUsuario : Form
    {
        public frmNovoUsuario()
        {
            InitializeComponent();
        }

        private void frmNovoUsuario_Load(object sender, EventArgs e)
        {
            btnCadastrar.btnPrimary();
        }

        private string GerarHash(string senha)
        {
            return BCrypt.HashPassword(senha);
        }

        private async void btnCadastrar_Click(object sender, EventArgs e)
        {
            string nomeDigitado = txtNome.Text;
            string loginDigitado = txtLogin.Text;
            string senhaDigitada = txtSenha.Text;
            string crmvDigitado = txtCrmv.Text;

            if (txtNome.Text.Trim() == "")
            {
                errorProvider.SetError(txtNome, "Favor preencher corretamente!");
                return;
            }
            else
            {
                errorProvider.SetError(txtNome, "");
            }

            if (txtLogin.Text.Trim() == "")
            {
                errorProvider.SetError(txtLogin, "Favor preencher corretamente!");
                return;
            }
            else
            {
                errorProvider.SetError(txtLogin, "");
            }

            if (txtSenha.Text.Trim() == "")
            {
                errorProvider.SetError(txtSenha, "Favor preencher corretamente!");
                return;
            }
            else
            {
                errorProvider.SetError(txtSenha, "");
            }

            try
            {
                //loadingForm.Show();
                btnCadastrar.Enabled = false;
                string resultado = await Register(nomeDigitado, loginDigitado, senhaDigitada, 
                                    "", "", crmvDigitado, "");

                if (resultado.Trim() != "")
                {
                    MessageBox.Show($"{resultado}", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao tentar cadastrar novo Funcionário(a)!\n {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                txtNome.Focus();
                txtLogin.Clear();
                txtSenha.Clear();
                btnCadastrar.Enabled = true;
                //loadingForm.Close();
            }
        }

        private async Task<string> Register(string nome, string login, string senha, string nivelAcesso,
                                        string telefone, string crmv, string funcionario_tipo)
        {
            string senhaHash = GerarHash(senha);

            await Task.Delay(1000); // simula um tempo de espera para a conexão com o banco de 
            FuncionariosTableAdapter taFuncionarios = new FuncionariosTableAdapter();

            Object teste = await Task.Run(() => taFuncionarios.CadastrarFuncionario(nome, login, senhaHash, 
                nivelAcesso, telefone, crmv, funcionario_tipo));
            return teste.ToString();
        }
    }
}
