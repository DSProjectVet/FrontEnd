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
            this.Cursor = Cursors.Default;
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
                this.Cursor = Cursors.WaitCursor;
                bool resultado = await Register(nomeDigitado, loginDigitado, senhaDigitada, 
                                    "1", "13556789065", crmvDigitado, "1");

                if (resultado)
                {
                    MessageBox.Show("Funcionário cadastrado com sucesso!", 
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao tentar se conectar com o banco de dados!\n {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                txtNome.Focus();
                txtLogin.Clear();
                txtSenha.Clear();
                btnCadastrar.Enabled = true;
                this.Cursor = Cursors.Default;
                //loadingForm.Close();
            }
        }

        private async Task<bool> Register(string nome, string login, string senha, string nivelAcesso, 
            string telefone, string crmv, string funcionario_tipo)
        {
            string senhaHash = GerarHash(senha);

            await Task.Delay(600); // simula um tempo de espera para a conexão com o banco de 
            CadastroFuncionarioResultadoTableAdapter taFuncionarios = new CadastroFuncionarioResultadoTableAdapter();

            DataTable dt = await Task.Run(() => taFuncionarios.SP_CadastrarFuncionario(nome, login, senhaHash, 
                nivelAcesso, telefone, crmv, funcionario_tipo));
            
            if (dt.Rows.Count > 0)
            {
                if (dt.Columns.Contains("ERROR") && 
                    dt.Rows[0]["ERROR"].ToString().Trim() != "")
                {
                    MessageBox.Show($"Erro ao tentar cadastrar novo Funcionário(a)!\n {dt.Rows[0]["ERROR"]}",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }

            MessageBox.Show("Não foi possível cadastrar Funcionário (a)!",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
