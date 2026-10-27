using DSProjectVet.Dados;
using DSProjectVet.Dados.DataSet_ClinicaVetTableAdapters;
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

namespace DSProjectVet.Forms.SCREENS
{
    public partial class frmNovoCliente : Form
    {
        public frmNovoCliente()
        {
            InitializeComponent();
        }

        private void frmNovoCliente_Load(object sender, EventArgs e)
        {
            btnCadastrar.btnPrimary();
        }

        //frmClientes frmClientes = new frmClientes();

        private async void btnCadastrar_Click(object sender, EventArgs e)
        {

            // Dados digitados
            string nomeDigitado = txtNome.Text;
            string cpfDigitado = txtCpf.Text;
            string telefoneDigitado = txtTelefone.Text;
            string enderecoDigitado = txtEndereco.Text;

            if (nomeDigitado.Trim() == "")
            {
                errorProvider.SetError(txtNome, "Favor preencher corretamente!");
                return;
            }
            else
            {
                errorProvider.SetError(txtNome, "");
            }

            if (cpfDigitado.Trim() == "")
            {
                errorProvider.SetError(txtCpf, "Favor preencher corretamente!");
                return;
            }
            else
            {
                errorProvider.SetError(txtCpf, "");
            }
            if (telefoneDigitado.Trim() == "")
            {
                errorProvider.SetError(txtTelefone, "Favor preencher corretamente!");
                return;
            }
            else
            {
                errorProvider.SetError(txtTelefone, "");
            }
            if (enderecoDigitado.Trim() == "")
            {
                errorProvider.SetError(txtEndereco, "Favor preencher corretamente!");
                return;
            }
            else
            {
                errorProvider.SetError(txtEndereco, "");
            }

            try
            {
                //loadingForm.Show();
                btnCadastrar.Enabled = false;
                bool result = await Register(nomeDigitado, cpfDigitado, telefoneDigitado, enderecoDigitado);

                if (result)
                {
                    MessageBox.Show("Cliente cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                txtNome.Clear();
                txtCpf.Clear();
                txtTelefone.Clear();
                txtEndereco.Clear();

                txtNome.Focus();
                btnCadastrar.Enabled = true;
            }

        }

        private async Task<bool> Register(string nome, string cpf, string telefone, string endereco)
        {
            await Task.Delay(550); // simula um tempo de espera para a conexão com o banco de 
            CadastroClienteResultadoTableAdapter taClientes = new CadastroClienteResultadoTableAdapter();

            DataTable dt = await Task.Run(() => taClientes.SP_CadastroCliente(nome, telefone, cpf, endereco));
            if (dt.Rows.Count > 0)
            {                    
                if (dt.Columns.Contains("ERROR") && 
                    dt.Rows[0]["ERROR"].ToString().Trim() != "")
                {
                    MessageBox.Show($"Erro ao cadastrar cliente: {dt.Rows[0]["ERROR"]}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //DataSet_ClinicaVet dataSet_ClinicaVet = new DataSet_ClinicaVet();

                //ClientesTableAdapter teste = new ClientesTableAdapter();
                //teste.Fill(dataSet_ClinicaVet.Clientes);
                //frmClientes.CarregarClientes();
                return true;
            }

            MessageBox.Show("Não foi possível cadastrar cliente!",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
}
