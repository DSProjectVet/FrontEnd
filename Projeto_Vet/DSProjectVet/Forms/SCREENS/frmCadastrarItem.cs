
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{
    public class frmCadastrarItem : Form
    {
        public ItemEstoqueDetalhe ItemCadastrado
        {
            get;
            private set;
        }

        private readonly Color verde =
            Color.FromArgb(20, 192, 138);

        private readonly Color verdeEscuro =
            Color.FromArgb(14, 145, 104);

        private readonly Color fundo =
            Color.FromArgb(248, 250, 249);

        private readonly Color texto =
            Color.FromArgb(35, 49, 45);

        private readonly Color cinza =
            Color.FromArgb(110, 120, 116);

        private TextBox txtNome;
        private TextBox txtDescricao;
        private ComboBox cmbTipo;

        private NumericUpDown numPreco;
        private NumericUpDown numQuantidade;
        private NumericUpDown numMinimo;

        private Label lblStatus;
        private FlowLayoutPanel painelCampos;

        public frmCadastrarItem()
        {
            ConstruirInterface();
            AtualizarStatus();
        }

        // ==========================================
        // CONSTRUIR INTERFACE
        // ==========================================

        private void ConstruirInterface()
        {
            Text = "Med Vet - Novo Item";
            ClientSize = new Size(640, 700);

            StartPosition =
                FormStartPosition.CenterParent;

            FormBorderStyle =
                FormBorderStyle.FixedDialog;

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            BackColor = fundo;
            Font = new Font("Segoe UI", 10F);

            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 3;
            layout.Margin = new Padding(0);

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 82F));

            Controls.Add(layout);

            // ======================================
            // CABEÇALHO
            // ======================================

            Panel cabecalho = new Panel();

            cabecalho.Dock = DockStyle.Fill;
            cabecalho.BackColor = Color.White;
            cabecalho.Margin = new Padding(0);

            layout.Controls.Add(cabecalho, 0, 0);

            Label titulo = new Label();

            titulo.Text = "Cadastrar novo item";
            titulo.Font = new Font(
                "Segoe UI", 21F, FontStyle.Bold);

            titulo.ForeColor = texto;
            titulo.SetBounds(28, 18, 570, 45);

            cabecalho.Controls.Add(titulo);

            Label subtitulo = new Label();

            subtitulo.Text =
                "Adicione um produto ou medicamento ao estoque.";

            subtitulo.Font =
                new Font("Segoe UI", 10F);

            subtitulo.ForeColor = cinza;
            subtitulo.SetBounds(30, 64, 570, 25);

            cabecalho.Controls.Add(subtitulo);

            // ======================================
            // CAMPOS DO CADASTRO
            // ======================================

            painelCampos = new FlowLayoutPanel();

            painelCampos.Dock = DockStyle.Fill;
            painelCampos.FlowDirection =
                FlowDirection.TopDown;

            painelCampos.WrapContents = false;
            painelCampos.AutoScroll = true;
            painelCampos.Margin = new Padding(0);

            painelCampos.Padding =
                new Padding(28, 18, 28, 14);

            painelCampos.BackColor = fundo;

            layout.Controls.Add(painelCampos, 0, 1);

            // TIPO
            cmbTipo = new ComboBox();

            cmbTipo.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbTipo.Items.AddRange(new object[]
            {
                "Produto",
                "Medicamento"
            });

            cmbTipo.SelectedIndex = -1;

            AdicionarCampo(
                "Tipo do item *",
                cmbTipo, 70);

            // NOME
            txtNome = CriarTexto();

            AdicionarCampo(
                "Nome *",
                txtNome, 70);

            // DESCRIÇÃO
            txtDescricao = CriarTexto();
            txtDescricao.Multiline = true;
            txtDescricao.ScrollBars =
                ScrollBars.Vertical;

            AdicionarCampo(
                "Descrição",
                txtDescricao, 120);

            // PREÇO
            numPreco = CriarNumero();

            numPreco.DecimalPlaces = 2;
            numPreco.Increment = 0.50M;

            AdicionarCampo(
                "Preço unitário (R$) *",
                numPreco, 70);

            // QUANTIDADE
            numQuantidade = CriarNumero();

            AdicionarCampo(
                "Quantidade inicial",
                numQuantidade, 70);

            // ESTOQUE MÍNIMO
            numMinimo = CriarNumero();

            AdicionarCampo(
                "Estoque mínimo",
                numMinimo, 70);

            // STATUS
            lblStatus = new Label();

            lblStatus.Width = 540;
            lblStatus.Height = 38;

            lblStatus.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);

            lblStatus.Margin =
                new Padding(0, 8, 0, 12);

            painelCampos.Controls.Add(lblStatus);

            numQuantidade.ValueChanged +=
                (s, e) => AtualizarStatus();

            numMinimo.ValueChanged +=
                (s, e) => AtualizarStatus();

            // ======================================
            // RODAPÉ
            // ======================================

            TableLayoutPanel rodape =
                new TableLayoutPanel();

            rodape.Dock = DockStyle.Fill;
            rodape.BackColor = Color.White;
            rodape.Margin = new Padding(0);
            rodape.Padding =
                new Padding(24, 16, 24, 14);

            rodape.ColumnCount = 3;
            rodape.RowCount = 1;

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 150F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 185F));

            rodape.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            layout.Controls.Add(rodape, 0, 2);

            Label info = new Label();

            info.Text = "Novo cadastro";
            info.ForeColor = cinza;
            info.Font = new Font("Segoe UI", 9F);
            info.Dock = DockStyle.Fill;
            info.TextAlign =
                ContentAlignment.MiddleLeft;

            rodape.Controls.Add(info, 0, 0);

            Button btnCancelar = new Button();

            btnCancelar.Text = "Cancelar";
            btnCancelar.Dock = DockStyle.Fill;
            btnCancelar.Margin =
                new Padding(0, 0, 12, 0);

            btnCancelar.BackColor =
                Color.FromArgb(237, 242, 240);

            btnCancelar.ForeColor = texto;

            EstilizarBotao(btnCancelar);

            btnCancelar.DialogResult =
                DialogResult.Cancel;

            rodape.Controls.Add(btnCancelar, 1, 0);

            Button btnCadastrar = new Button();

            btnCadastrar.Text = "Cadastrar item";
            btnCadastrar.Dock = DockStyle.Fill;
            btnCadastrar.Margin = new Padding(0);

            btnCadastrar.BackColor = verde;
            btnCadastrar.ForeColor = Color.White;

            EstilizarBotao(btnCadastrar);

            btnCadastrar.Click +=
                BtnCadastrar_Click;

            rodape.Controls.Add(btnCadastrar, 2, 0);

            AcceptButton = btnCadastrar;
            CancelButton = btnCancelar;
        }

        // ==========================================
        // CRIAR CONTROLES
        // ==========================================

        private TextBox CriarTexto()
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                BorderStyle =
                    BorderStyle.FixedSingle,

                BackColor = Color.White,
                ForeColor = texto
            };
        }

        private NumericUpDown CriarNumero()
        {
            return new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100000000M,
                Font = new Font("Segoe UI", 11F),
                ThousandsSeparator = true,
                BackColor = Color.White,
                ForeColor = texto
            };
        }

        private void AdicionarCampo(
            string titulo,
            Control campo,
            int altura)
        {
            Panel grupo = new Panel();

            grupo.Width = 540;
            grupo.Height = altura;

            grupo.Margin =
                new Padding(0, 0, 0, 9);

            Label label = new Label();

            label.Text = titulo;
            label.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            label.ForeColor = texto;
            label.SetBounds(0, 0, 540, 23);

            grupo.Controls.Add(label);

            int alturaCampo =
                campo is TextBox &&
                ((TextBox)campo).Multiline
                ? 82
                : 34;

            campo.SetBounds(
                0, 29, 540, alturaCampo);

            grupo.Controls.Add(campo);

            painelCampos.Controls.Add(grupo);
        }

        private void EstilizarBotao(Button botao)
        {
            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = 0;

            botao.UseVisualStyleBackColor = false;

            botao.Cursor = Cursors.Hand;

            botao.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);
        }

        // ==========================================
        // STATUS AUTOMÁTICO
        // ==========================================

        private void AtualizarStatus()
        {
            if (lblStatus == null)
                return;

            if (numQuantidade.Value == 0)
            {
                lblStatus.Text =
                    "●  Situação inicial: Sem estoque";

                lblStatus.ForeColor =
                    Color.FromArgb(220, 53, 69);
            }
            else if (numQuantidade.Value <=
                     numMinimo.Value)
            {
                lblStatus.Text =
                    "●  Situação inicial: Estoque baixo";

                lblStatus.ForeColor =
                    Color.FromArgb(194, 130, 18);
            }
            else
            {
                lblStatus.Text =
                    "●  Situação inicial: Estoque normal";

                lblStatus.ForeColor = verdeEscuro;
            }
        }

        // ==========================================
        // VALIDAR E ENVIAR DADOS
        // ==========================================

        private void BtnCadastrar_Click(
            object sender, EventArgs e)
        {
            if (cmbTipo.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Selecione Produto ou Medicamento.",
                    "Campo obrigatório",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbTipo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show(
                    "Informe o nome do item.",
                    "Campo obrigatório",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtNome.Focus();
                return;
            }

            if (numPreco.Value <= 0)
            {
                MessageBox.Show(
                    "Informe um preço maior que zero.",
                    "Preço inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                numPreco.Focus();
                return;
            }

            ItemCadastrado = new ItemEstoqueDetalhe
            {
                Id = 0,

                Tipo =
                    cmbTipo.SelectedItem.ToString(),

                Nome = txtNome.Text.Trim(),

                Descricao =
                    txtDescricao.Text.Trim(),

                Preco = numPreco.Value,

                Quantidade =
                    Decimal.ToInt32(
                        numQuantidade.Value),

                EstoqueMinimo =
                    Decimal.ToInt32(
                        numMinimo.Value)
            };

            // O SQL será executado no frmEstoque.
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
