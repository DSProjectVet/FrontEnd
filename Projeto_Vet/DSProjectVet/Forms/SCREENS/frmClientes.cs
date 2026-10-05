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

        private void CarregarClientes()
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
            dataGridView1.EnableHeadersVisualStyles = false;

            // FUNDO
            dataGridView1.BackgroundColor = Color.White;

            dataGridView1.BorderStyle =
                BorderStyle.None;

            // GRID
            dataGridView1.GridColor =
                Color.FromArgb(232, 238, 235);

            dataGridView1.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            // REMOVE CABEÇALHO LATERAL
            dataGridView1.RowHeadersVisible = false;

            // SELEÇÃO
            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.MultiSelect = false;

            // EVITA ALTERAÇÃO DIRETA
            dataGridView1.ReadOnly = true;

            // REMOVE LINHA EXTRA
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.AllowUserToDeleteRows = false;

            dataGridView1.AllowUserToResizeRows = false;

            // TAMANHO
            dataGridView1.RowTemplate.Height = 42;

            dataGridView1.ColumnHeadersHeight = 45;

            // RESPONSIVIDADE
            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.None;

            dataGridView1.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            // =========================
            // CABEÇALHO
            // =========================

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor =
                verdePrincipal;

            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(8, 0, 0, 0);

            // =========================
            // CÉLULAS
            // =========================

            dataGridView1.DefaultCellStyle.BackColor =
                Color.White;

            dataGridView1.DefaultCellStyle.ForeColor =
                textoPrincipal;

            dataGridView1.DefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Regular
                );

            dataGridView1.DefaultCellStyle.SelectionBackColor =
                verdeClaro;

            dataGridView1.DefaultCellStyle.SelectionForeColor =
                verdeEscuro;

            dataGridView1.DefaultCellStyle.Padding =
                new Padding(8, 0, 8, 0);

            dataGridView1.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            // =========================
            // EFEITO ZEBRA
            // =========================

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 252, 250);
        }

        // =========================================================
        // COLUNAS
        // =========================================================

        private void ConfigurarColunas()
        {
            if (dataGridView1.Columns.Count == 0)
                return;

            // Esconde ID
            if (dataGridView1.Columns.Contains("ID"))
            {
                dataGridView1.Columns["ID"].Visible = false;
            }

            // Nome
            if (dataGridView1.Columns.Contains("Nome"))
            {
                dataGridView1.Columns["Nome"].HeaderText =
                    "Nome";

                dataGridView1.Columns["Nome"].FillWeight = 100;
            }

            // CPF
            if (dataGridView1.Columns.Contains("CPF"))
            {
                dataGridView1.Columns["CPF"].HeaderText =
                    "CPF";

                dataGridView1.Columns["CPF"].FillWeight = 75;
            }

            // Telefone
            if (dataGridView1.Columns.Contains("Telefone"))
            {
                dataGridView1.Columns["Telefone"].HeaderText =
                    "Telefone";

                dataGridView1.Columns["Telefone"].FillWeight = 70;
            }

            // Endereço
            if (dataGridView1.Columns.Contains("Endereco"))
            {
                dataGridView1.Columns["Endereco"].HeaderText =
                    "Endereço";

                dataGridView1.Columns["Endereco"].FillWeight = 120;
            }

            // Nível
            if (dataGridView1.Columns.Contains("Nivel"))
            {
                dataGridView1.Columns["Nivel"].Visible = false;
            }

            // Senha
            if (dataGridView1.Columns.Contains("Senha"))
            {
                dataGridView1.Columns["Senha"].Visible = false;
            }

            // Usuário
            if (dataGridView1.Columns.Contains("Usuario"))
            {
                dataGridView1.Columns["Usuario"].Visible = false;
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
                dataGridView1.Rows[e.RowIndex];

            if (linhaAtual.DataBoundItem is DataRowView dadosCliente)
            {
                clienteSelecionadoId =
                    Convert.ToInt32(dadosCliente["ID"]);

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