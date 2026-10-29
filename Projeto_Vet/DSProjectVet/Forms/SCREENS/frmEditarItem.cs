
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{
    public class frmEditarItem : Form
    {
        private readonly ItemEstoqueDetalhe itemOriginal;

        public ItemEstoqueDetalhe ItemEditado
        {
            get;
            private set;
        }

        // CORES MED VET
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

        // CAMPOS
        private TextBox txtNome;
        private TextBox txtDescricao;
        private TextBox txtTipo;

        private NumericUpDown numPreco;
        private NumericUpDown numQuantidade;
        private NumericUpDown numMinimo;

        private Label lblStatus;

        private FlowLayoutPanel painelCampos;

        public frmEditarItem(ItemEstoqueDetalhe item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            itemOriginal = item;

            ConstruirInterface();
            PreencherCampos();
            AtualizarStatus();
        }

        // ==========================================
        // CONSTRUIR INTERFACE
        // ==========================================

        private void ConstruirInterface()
        {
            this.Text = "Med Vet - Editar Item";
            this.ClientSize = new Size(620, 740);

            this.StartPosition =
                FormStartPosition.CenterParent;

            this.FormBorderStyle =
                FormBorderStyle.FixedDialog;

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            this.BackColor = fundo;
            this.Font = new Font("Segoe UI", 10F);

            // LAYOUT PRINCIPAL
            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 3;

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 108F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 85F));

            this.Controls.Add(layout);

            // ======================================
            // CABEÇALHO
            // ======================================

            Panel cabecalho = new Panel();

            cabecalho.Dock = DockStyle.Fill;
            cabecalho.BackColor = Color.White;
            cabecalho.Margin = new Padding(0);

            layout.Controls.Add(cabecalho, 0, 0);

            Label titulo = new Label();

            titulo.Text = "Editar item do estoque";
            titulo.Font = new Font(
                "Segoe UI", 20F, FontStyle.Bold);

            titulo.ForeColor = texto;
            titulo.SetBounds(26, 20, 560, 42);

            cabecalho.Controls.Add(titulo);

            Label subtitulo = new Label();

            subtitulo.Text =
                "Atualize as informações do produto ou medicamento.";

            subtitulo.Font =
                new Font("Segoe UI", 10F);

            subtitulo.ForeColor = cinza;
            subtitulo.SetBounds(28, 66, 560, 25);

            cabecalho.Controls.Add(subtitulo);

            // ======================================
            // CORPO COM ROLAGEM
            // ======================================

            painelCampos = new FlowLayoutPanel();

            painelCampos.Dock = DockStyle.Fill;
            painelCampos.FlowDirection =
                FlowDirection.TopDown;

            painelCampos.WrapContents = false;
            painelCampos.AutoScroll = true;

            painelCampos.Padding =
                new Padding(26, 20, 26, 15);

            painelCampos.Margin = new Padding(0);
            painelCampos.BackColor = fundo;

            layout.Controls.Add(painelCampos, 0, 1);

            // NOME
            txtNome = CriarTextBox();

            AdicionarCampo(
                "Nome do item *",
                txtNome, 70);

            // TIPO
            txtTipo = CriarTextBox();
            txtTipo.ReadOnly = true;
            txtTipo.BackColor =
                Color.FromArgb(236, 242, 239);

            AdicionarCampo(
                "Tipo do item",
                txtTipo, 70);

            // DESCRIÇÃO
            txtDescricao = CriarTextBox();
            txtDescricao.Multiline = true;

            txtDescricao.ScrollBars =
                ScrollBars.Vertical;

            AdicionarCampo(
                "Descrição",
                txtDescricao, 114);

            // PREÇO
            numPreco = CriarNumero();

            numPreco.DecimalPlaces = 2;
            numPreco.Increment = 0.50M;
            numPreco.Maximum = 100000000M;

            AdicionarCampo(
                "Preço unitário (R$)",
                numPreco, 70);

            // QUANTIDADE
            numQuantidade = CriarNumero();

            numQuantidade.Maximum = 100000000M;

            AdicionarCampo(
                "Quantidade em estoque",
                numQuantidade, 70);

            // ESTOQUE MÍNIMO
            numMinimo = CriarNumero();

            numMinimo.Maximum = 100000000M;

            AdicionarCampo(
                "Estoque mínimo",
                numMinimo, 70);

            // STATUS AUTOMÁTICO
            lblStatus = new Label();

            lblStatus.Width = 520;
            lblStatus.Height = 42;

            lblStatus.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);

            lblStatus.Margin =
                new Padding(0, 8, 0, 5);

            lblStatus.TextAlign =
                ContentAlignment.MiddleLeft;

            painelCampos.Controls.Add(lblStatus);

            numQuantidade.ValueChanged +=
                (s, e) => AtualizarStatus();

            numMinimo.ValueChanged +=
                (s, e) => AtualizarStatus();

            // ======================================
            // RODAPÉ
            // ======================================


            // ==========================================
            // RODAPÉ - BOTÕES ORGANIZADOS
            // ==========================================

            TableLayoutPanel rodape =
                new TableLayoutPanel();

            rodape.Dock = DockStyle.Fill;
            rodape.BackColor = Color.White;
            rodape.Margin = new Padding(0);
            rodape.Padding = new Padding(22, 16, 22, 16);

            rodape.ColumnCount = 3;
            rodape.RowCount = 1;

            // Espaço flexível + dois botões
            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 150F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 188F));

            rodape.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            layout.Controls.Add(rodape, 0, 2);

            // ==========================================
            // INFORMAÇÃO À ESQUERDA
            // ==========================================

            Label lblInfo = new Label();

            lblInfo.Text = "Alterações locais";
            lblInfo.Font = new Font(
                "Segoe UI", 9F, FontStyle.Regular);

            lblInfo.ForeColor = cinza;
            lblInfo.Dock = DockStyle.Fill;
            lblInfo.TextAlign =
                ContentAlignment.MiddleLeft;

            lblInfo.AutoEllipsis = true;
            lblInfo.Margin = new Padding(0, 0, 8, 0);

            rodape.Controls.Add(lblInfo, 0, 0);

            // ==========================================
            // BOTÃO CANCELAR
            // ==========================================

            Button btnCancelar = new Button();

            btnCancelar.Text = "Cancelar";
            btnCancelar.Height = 44;
            btnCancelar.Dock = DockStyle.Top;

            btnCancelar.Margin =
                new Padding(0, 0, 12, 0);

            btnCancelar.BackColor =
                Color.FromArgb(237, 242, 240);

            btnCancelar.ForeColor = texto;

            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.FlatAppearance.BorderSize = 0;

            btnCancelar.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);

            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.UseVisualStyleBackColor = false;

            btnCancelar.DialogResult =
                DialogResult.Cancel;

            rodape.Controls.Add(btnCancelar, 1, 0);

            btnCancelar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // ==========================================
            // BOTÃO APLICAR ALTERAÇÕES
            // ==========================================

            Button btnSalvar = new Button();

            btnSalvar.Text = "Aplicar alterações";
            btnSalvar.Height = 44;
            btnSalvar.Dock = DockStyle.Top;

            btnSalvar.Margin = new Padding(0);

            btnSalvar.BackColor = verde;
            btnSalvar.ForeColor = Color.White;

            btnSalvar.FlatStyle = FlatStyle.Flat;
            btnSalvar.FlatAppearance.BorderSize = 0;

            btnSalvar.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);

            btnSalvar.Cursor = Cursors.Hand;
            btnSalvar.UseVisualStyleBackColor = false;

            rodape.Controls.Add(btnSalvar, 2, 0);

            // Mantém o evento original de salvamento
            btnSalvar.Click += BtnSalvar_Click;

            // ==========================================
            // EFEITOS HOVER
            // ==========================================

            btnCancelar.MouseEnter += (s, e) =>
            {
                btnCancelar.BackColor =
                    Color.FromArgb(220, 230, 225);
            };

            btnCancelar.MouseLeave += (s, e) =>
            {
                btnCancelar.BackColor =
                    Color.FromArgb(237, 242, 240);
            };

            btnSalvar.MouseEnter += (s, e) =>
            {
                btnSalvar.BackColor = verdeEscuro;
            };

            btnSalvar.MouseLeave += (s, e) =>
            {
                btnSalvar.BackColor = verde;
            };

            // ==========================================
            // ATALHOS DO FORMULÁRIO
            // ==========================================

            this.AcceptButton = btnSalvar;
            this.CancelButton = btnCancelar;
        }

        // ==========================================
        // CRIAR TEXTBOX
        // ==========================================

        private TextBox CriarTextBox()
        {
            TextBox campo = new TextBox();

            campo.Font = new Font("Segoe UI", 11F);
            campo.BackColor = Color.White;
            campo.ForeColor = texto;

            campo.BorderStyle =
                BorderStyle.FixedSingle;

            return campo;
        }

        // ==========================================
        // CRIAR CAMPO NUMÉRICO
        // ==========================================

        private NumericUpDown CriarNumero()
        {
            NumericUpDown campo =
                new NumericUpDown();

            campo.Font = new Font("Segoe UI", 11F);

            campo.Minimum = 0;
            campo.Maximum = 100000000M;

            campo.ThousandsSeparator = true;

            campo.BackColor = Color.White;
            campo.ForeColor = texto;

            campo.TextAlign =
                HorizontalAlignment.Left;

            return campo;
        }

        // ==========================================
        // ADICIONAR CAMPO AO FORMULÁRIO
        // ==========================================

        private void AdicionarCampo(
            string titulo,
            Control campo,
            int altura)
        {
            Panel grupo = new Panel();

            grupo.Width = 520;
            grupo.Height = altura;

            grupo.Margin =
                new Padding(0, 0, 0, 9);

            Label label = new Label();

            label.Text = titulo;
            label.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            label.ForeColor = texto;
            label.SetBounds(0, 0, 520, 23);

            grupo.Controls.Add(label);

            int alturaCampo =
                campo is TextBox &&
                ((TextBox)campo).Multiline
                ? 77
                : 34;

            campo.SetBounds(
                0, 29, 520, alturaCampo);

            grupo.Controls.Add(campo);

            painelCampos.Controls.Add(grupo);
        }

        // ==========================================
        // PREENCHER DADOS EXISTENTES
        // ==========================================

        private void PreencherCampos()
        {
            txtNome.Text =
                itemOriginal.Nome ?? "";

            txtDescricao.Text =
                itemOriginal.Descricao ?? "";

            txtTipo.Text =
                itemOriginal.Tipo ?? "";

            numPreco.Value = Math.Min(
                numPreco.Maximum,
                Math.Max(numPreco.Minimum,
                    itemOriginal.Preco));

            numQuantidade.Value = Math.Min(
                numQuantidade.Maximum,
                Math.Max(numQuantidade.Minimum,
                    itemOriginal.Quantidade));

            numMinimo.Value = Math.Min(
                numMinimo.Maximum,
                Math.Max(numMinimo.Minimum,
                    itemOriginal.EstoqueMinimo));
        }

        // ==========================================
        // ATUALIZAR SITUAÇÃO DO ESTOQUE
        // ==========================================

        private void AtualizarStatus()
        {
            if (lblStatus == null ||
                numQuantidade == null ||
                numMinimo == null)
                return;

            decimal quantidade =
                numQuantidade.Value;

            decimal minimo =
                numMinimo.Value;

            if (quantidade == 0)
            {
                lblStatus.Text =
                    "●  Situação: Sem estoque";

                lblStatus.ForeColor =
                    Color.FromArgb(220, 53, 69);
            }
            else if (quantidade <= minimo)
            {
                lblStatus.Text =
                    "●  Situação: Estoque baixo";

                lblStatus.ForeColor =
                    Color.FromArgb(194, 130, 18);
            }
            else
            {
                lblStatus.Text =
                    "●  Situação: Estoque normal";

                lblStatus.ForeColor = verdeEscuro;
            }
        }

        // ==========================================
        // APLICAR EDIÇÃO
        // ==========================================

        private void BtnSalvar_Click(
            object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                txtNome.Text))
            {
                MessageBox.Show(
                    "Informe o nome do item.",
                    "Campo obrigatório",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtNome.Focus();
                return;
            }

            // Cria um objeto atualizado.
            // Não executa UPDATE no banco.
            ItemEditado = new ItemEstoqueDetalhe
            {
                Id = itemOriginal.Id,

                Nome = txtNome.Text.Trim(),

                Descricao =
                    txtDescricao.Text.Trim(),

                Tipo = itemOriginal.Tipo,

                Preco = numPreco.Value,

                Quantidade =
                    Decimal.ToInt32(numQuantidade.Value),

                EstoqueMinimo =
                    Decimal.ToInt32(numMinimo.Value)
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
