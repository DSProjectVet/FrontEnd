using DSProjectVet.Forms.SCREENS;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public partial class frmClientes : Form
    {
        private int clienteSelecionadoId = 0;

        // =========================
        // CORES DO SISTEMA
        // =========================

        private readonly Color verdePrincipal =
            Color.FromArgb(20, 192, 138);

        private readonly Color verdeEscuro =
            Color.FromArgb(14, 145, 104);

        private readonly Color verdeClaro =
            Color.FromArgb(225, 248, 240);

        private readonly Color fundoTela =
            Color.FromArgb(248, 250, 249);

        private readonly Color textoPrincipal =
            Color.FromArgb(45, 55, 52);

        private readonly Color textoSecundario =
            Color.FromArgb(110, 120, 116);

        public frmClientes()
        {
            InitializeComponent();
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {
            CarregarClientes();

            EstilizarTela();

            ConfigurarColunas();

            LimparDetalhesCliente();

            // Mantém sua estilização personalizada dos botões
            btnCadastrar.btnPrimary();
            btnVerAnimais.btnPrimary();
        }

        // =========================================================
        // CARREGAR CLIENTES
        // =========================================================

        public void CarregarClientes()
        {
            try
            {
                this.clientesTableAdapter.Fill(
                    this.dataSet_ClinicaVet.Clientes
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível carregar os clientes.\n\n" +
                    ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // ESTILIZAÇÃO GERAL
        // =========================================================

        private void EstilizarTela()
        {
            this.BackColor = fundoTela;

            EstilizarLabels();

            EstilizarDataGridView();
        }

        // =========================================================
        // LABELS
        // =========================================================

        private void EstilizarLabels()
        {
            // Nome do cliente
            lblNome.Font = new Font(
                "Segoe UI",
                17F,
                FontStyle.Bold
            );

            lblNome.ForeColor = textoPrincipal;

            // CPF
            lblCpf.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Regular
            );

            lblCpf.ForeColor = textoSecundario;

            // Endereço
            lblEndereço.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Regular
            );

            lblEndereço.ForeColor = textoSecundario;

            // Telefone
            lblTelefone.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Regular
            );

            lblTelefone.ForeColor = textoSecundario;
        }

        // =========================================================
        // DATAGRIDVIEW
        // =========================================================

        private void EstilizarDataGridView()
        {
            dataGridViewClientes.EnableHeadersVisualStyles = false;

            // FUNDO
            dataGridViewClientes.BackgroundColor = Color.White;

            dataGridViewClientes.BorderStyle =
                BorderStyle.None;

            // GRID
            dataGridViewClientes.GridColor =
                Color.FromArgb(232, 238, 235);

            dataGridViewClientes.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            // REMOVE CABEÇALHO LATERAL
            dataGridViewClientes.RowHeadersVisible = false;

            // SELEÇÃO
            dataGridViewClientes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dataGridViewClientes.MultiSelect = false;

            // EVITA ALTERAÇÃO DIRETA
            dataGridViewClientes.ReadOnly = true;

            // REMOVE LINHA EXTRA
            dataGridViewClientes.AllowUserToAddRows = false;

            dataGridViewClientes.AllowUserToDeleteRows = false;

            dataGridViewClientes.AllowUserToResizeRows = false;

            // TAMANHO
            dataGridViewClientes.RowTemplate.Height = 42;

            dataGridViewClientes.ColumnHeadersHeight = 45;

            // RESPONSIVIDADE
            dataGridViewClientes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dataGridViewClientes.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.None;

            dataGridViewClientes.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            // =========================
            // CABEÇALHO
            // =========================

            dataGridViewClientes.ColumnHeadersDefaultCellStyle.BackColor =
                verdePrincipal;

            dataGridViewClientes.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dataGridViewClientes.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            dataGridViewClientes.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dataGridViewClientes.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(8, 0, 0, 0);

            // =========================
            // CÉLULAS
            // =========================

            dataGridViewClientes.DefaultCellStyle.BackColor =
                Color.White;

            dataGridViewClientes.DefaultCellStyle.ForeColor =
                textoPrincipal;

            dataGridViewClientes.DefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Regular
                );

            dataGridViewClientes.DefaultCellStyle.SelectionBackColor =
                verdeClaro;

            dataGridViewClientes.DefaultCellStyle.SelectionForeColor =
                verdeEscuro;

            dataGridViewClientes.DefaultCellStyle.Padding =
                new Padding(8, 0, 8, 0);

            dataGridViewClientes.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            // =========================
            // EFEITO ZEBRA
            // =========================

            dataGridViewClientes.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 252, 250);
        }

        // =========================================================
        // COLUNAS
        // =========================================================

        private void ConfigurarColunas()
        {
            if (dataGridViewClientes.Columns.Count == 0)
                return;

            // Esconde ID
            if (dataGridViewClientes.Columns.Contains("ID_Cliente"))
            {
                dataGridViewClientes.Columns["ID_Cliente"].Visible = false;
            }

            // Nome
            if (dataGridViewClientes.Columns.Contains("Nome"))
            {
                dataGridViewClientes.Columns["Nome"].HeaderText =
                    "Nome";

                dataGridViewClientes.Columns["Nome"].FillWeight = 100;
            }

            // CPF
            if (dataGridViewClientes.Columns.Contains("CPF"))
            {
                dataGridViewClientes.Columns["CPF"].HeaderText =
                    "CPF";

                dataGridViewClientes.Columns["CPF"].FillWeight = 75;
            }

            // Telefone
            if (dataGridViewClientes.Columns.Contains("Telefone"))
            {
                dataGridViewClientes.Columns["Telefone"].HeaderText =
                    "Telefone";

                dataGridViewClientes.Columns["Telefone"].FillWeight = 70;
            }

            // Endereço
            if (dataGridViewClientes.Columns.Contains("Endereco"))
            {
                dataGridViewClientes.Columns["Endereco"].HeaderText =
                    "Endereço";

                dataGridViewClientes.Columns["Endereco"].FillWeight = 120;
            }
        }

        // =========================================================
        // SELEÇÃO DO CLIENTE
        // =========================================================

        private void dataGridView1_CellEnter(
            object sender,
            DataGridViewCellEventArgs e
        )
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow linhaAtual =
                dataGridViewClientes.Rows[e.RowIndex];

            if (linhaAtual.DataBoundItem is DataRowView dadosCliente)
            {
                clienteSelecionadoId =
                    Convert.ToInt32(dadosCliente["ID_Cliente"]);

                lblNome.Text =
                    dadosCliente["Nome"]?.ToString();

                lblCpf.Text =
                    FormatarCPF(
                        dadosCliente["CPF"]?.ToString()
                    );

                lblEndereço.Text =
                    dadosCliente["Endereco"]?.ToString();

                lblTelefone.Text =
                    dadosCliente["Telefone"]?.ToString();
            }
        }

        // =========================================================
        // BOTÃO VER ANIMAIS
        // =========================================================

        private void button1_Click(object sender, EventArgs e)
        {
            if (clienteSelecionadoId == 0)
            {
                MessageBox.Show(
                    "Selecione um cliente primeiro.",
                    "Cliente não selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            frmAnimais telaAnimais =
                new frmAnimais(clienteSelecionadoId);

            telaAnimais.ShowDialog();
        }

        // =========================================================
        // LIMPAR DETALHES
        // =========================================================

        private void LimparDetalhesCliente()
        {
            clienteSelecionadoId = 0;

            lblNome.Text = "Selecione um cliente";

            lblCpf.Text =
                "CPF será exibido aqui";

            lblEndereço.Text =
                "Endereço será exibido aqui";

            lblTelefone.Text =
                "Telefone será exibido aqui";
        }

        // =========================================================
        // FORMATAÇÃO CPF
        // =========================================================

        private string FormatarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return "CPF não informado";

            string numeros = "";

            foreach (char caractere in cpf)
            {
                if (char.IsDigit(caractere))
                {
                    numeros += caractere;
                }
            }

            if (numeros.Length == 11)
            {
                return Convert.ToUInt64(numeros)
                    .ToString(@"000\.000\.000\-00");
            }

            return cpf;
        }

        // =========================================================
        // EVENTOS EXISTENTES
        // =========================================================

        private void dataGridView1_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e
        )
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            frmNovoCliente TelaCadastro =
             new frmNovoCliente();

            TelaCadastro.ShowDialog();
        }
    }
}