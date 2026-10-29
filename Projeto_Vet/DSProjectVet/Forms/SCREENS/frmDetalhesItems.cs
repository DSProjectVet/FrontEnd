
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{
    // ==========================================
    // MODELO DE DADOS DO ESTOQUE
    // ==========================================

    public class ItemEstoqueDetalhe
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Tipo { get; set; }

        public int Quantidade { get; set; }

        public int EstoqueMinimo { get; set; }

        public decimal Preco { get; set; }
    }

    // ==========================================
    // FORMULÁRIO DE DETALHES DO ITEM
    // ==========================================

    public class frmDetalhesItem : Form
    {
        private readonly ItemEstoqueDetalhe item;

        // RESULTADO DA EDIÇÃO
        public ItemEstoqueDetalhe ItemEditado
        {
            get;
            private set;
        }

        // RESULTADO DA EXCLUSÃO
        public bool ExclusaoSolicitada
        {
            get;
            private set;
        }

        // ======================================
        // CORES MED VET
        // ======================================

        private readonly Color verde =
            Color.FromArgb(20, 192, 138);

        private readonly Color verdeEscuro =
            Color.FromArgb(14, 145, 104);

        private readonly Color verdeClaro =
            Color.FromArgb(225, 248, 240);

        private readonly Color fundo =
            Color.FromArgb(248, 250, 249);

        private readonly Color texto =
            Color.FromArgb(35, 49, 45);

        private readonly Color cinza =
            Color.FromArgb(110, 120, 116);

        private readonly Color borda =
            Color.FromArgb(230, 236, 233);

        private readonly Color vermelho =
            Color.FromArgb(220, 53, 69);

        private readonly CultureInfo cultura =
            new CultureInfo("pt-BR");

        // ======================================
        // CONSTRUTOR
        // ======================================

        public frmDetalhesItem(ItemEstoqueDetalhe dados)
        {
            if (dados == null)
            {
                throw new ArgumentNullException("dados");
            }

            item = dados;

            ItemEditado = null;
            ExclusaoSolicitada = false;

            ConfigurarJanela();
            ConstruirInterface();
        }

        // ==========================================
        // CONFIGURAÇÃO DA JANELA
        // ==========================================

        private void ConfigurarJanela()
        {
            this.Text = "Med Vet - Detalhes do Item";

            this.ClientSize = new Size(790, 625);

            this.StartPosition =
                FormStartPosition.CenterParent;

            this.FormBorderStyle =
                FormBorderStyle.FixedDialog;

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            this.BackColor = fundo;

            this.Font =
                new Font("Segoe UI", 10F);

            this.DoubleBuffered = true;
        }

        // ==========================================
        // CONSTRUIR INTERFACE
        // ==========================================

        private void ConstruirInterface()
        {
            // ======================================
            // LAYOUT PRINCIPAL
            // ======================================

            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;

            layout.Padding =
                new Padding(24);

            layout.Margin =
                new Padding(0);

            layout.BackColor = fundo;

            layout.ColumnCount = 1;
            layout.RowCount = 4;

            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 140F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 220F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 74F));

            this.Controls.Add(layout);

            // ======================================
            // CABEÇALHO
            // ======================================

            Panel cabecalho = CriarCartao();

            cabecalho.Dock = DockStyle.Fill;

            cabecalho.Margin =
                new Padding(0, 0, 0, 14);

            layout.Controls.Add(
                cabecalho, 0, 0);

            // ÍCONE DO ITEM
            Panel painelIcone = new Panel();

            painelIcone.BackColor = verdeClaro;

            painelIcone.SetBounds(
                22, 29, 68, 68);

            cabecalho.Controls.Add(
                painelIcone);

            Arredondar(painelIcone, 18);

            Label lblIcone = CriarLabel(
                string.Equals(
                    item.Tipo,
                    "Medicamento",
                    StringComparison.OrdinalIgnoreCase)
                    ? "M" : "P",
                28F,
                FontStyle.Bold,
                verdeEscuro);

            lblIcone.Dock = DockStyle.Fill;

            lblIcone.TextAlign =
                ContentAlignment.MiddleCenter;

            painelIcone.Controls.Add(lblIcone);

            // NOME
            Label lblNome = CriarLabel(
                string.IsNullOrWhiteSpace(item.Nome)
                    ? "Item sem nome"
                    : item.Nome,
                21F,
                FontStyle.Bold,
                texto);

            lblNome.SetBounds(
                110, 17, 590, 44);

            lblNome.AutoEllipsis = true;

            cabecalho.Controls.Add(lblNome);

            // TIPO E IDENTIFICAÇÃO
            Label lblIdentificacao = CriarLabel(
                (item.Tipo ?? "ITEM").ToUpperInvariant() +
                "  •  CÓDIGO #" +
                item.Id.ToString("D4"),
                9F,
                FontStyle.Regular,
                cinza);

            lblIdentificacao.SetBounds(
                112, 62, 540, 25);

            cabecalho.Controls.Add(
                lblIdentificacao);

            // STATUS
            Label lblStatus = CriarLabel(
                "●  " + ObterStatus().ToUpperInvariant(),
                9F,
                FontStyle.Bold,
                ObterCorStatus());

            lblStatus.SetBounds(
                112, 91, 250, 26);

            cabecalho.Controls.Add(lblStatus);

            // ======================================
            // LINHA COM DOIS CARTÕES
            // ======================================

            TableLayoutPanel linhaCartoes =
                new TableLayoutPanel();

            linhaCartoes.Dock = DockStyle.Fill;

            linhaCartoes.ColumnCount = 2;
            linhaCartoes.RowCount = 1;

            linhaCartoes.Margin =
                new Padding(0);

            linhaCartoes.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 50F));

            linhaCartoes.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 50F));

            linhaCartoes.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            layout.Controls.Add(
                linhaCartoes, 0, 1);

            // ======================================
            // CARTÃO: INFORMAÇÕES GERAIS
            // ======================================

            Panel cartaoDados = CriarCartao();

            cartaoDados.Dock = DockStyle.Fill;

            cartaoDados.Margin =
                new Padding(0, 0, 8, 12);

            linhaCartoes.Controls.Add(
                cartaoDados, 0, 0);

            Label tituloDados = CriarLabel(
                "Informações gerais",
                13F,
                FontStyle.Bold,
                texto);

            tituloDados.SetBounds(
                20, 15, 320, 32);

            cartaoDados.Controls.Add(
                tituloDados);

            AdicionarCampo(
                cartaoDados,
                "Código do item",
                "#" + item.Id.ToString("D4"),
                60);

            AdicionarCampo(
                cartaoDados,
                "Categoria",
                item.Tipo ?? "Não informado",
                109);

            AdicionarCampo(
                cartaoDados,
                "Preço unitário",
                item.Preco.ToString("C2", cultura),
                158);

            // ======================================
            // CARTÃO: CONTROLE DE ESTOQUE
            // ======================================

            Panel cartaoEstoque = CriarCartao();

            cartaoEstoque.Dock = DockStyle.Fill;

            cartaoEstoque.Margin =
                new Padding(8, 0, 0, 12);

            linhaCartoes.Controls.Add(
                cartaoEstoque, 1, 0);

            Label tituloEstoque = CriarLabel(
                "Controle de estoque",
                13F,
                FontStyle.Bold,
                texto);

            tituloEstoque.SetBounds(
                20, 15, 320, 32);

            cartaoEstoque.Controls.Add(
                tituloEstoque);

            // QUANTIDADE
            Label lblQuantidade = CriarLabel(
                item.Quantidade.ToString("N0", cultura),
                30F,
                FontStyle.Bold,
                ObterCorStatus());

            lblQuantidade.SetBounds(
                20, 52, 250, 60);

            cartaoEstoque.Controls.Add(
                lblQuantidade);

            Label lblUnidades = CriarLabel(
                "unidades disponíveis",
                9F,
                FontStyle.Regular,
                cinza);

            lblUnidades.SetBounds(
                23, 111, 270, 25);

            cartaoEstoque.Controls.Add(
                lblUnidades);

            AdicionarCampo(
                cartaoEstoque,
                "Estoque mínimo",
                item.EstoqueMinimo.ToString("N0", cultura) +
                " unidades",
                148);

            // ======================================
            // CARTÃO: DESCRIÇÃO
            // ======================================

            Panel cartaoDescricao = CriarCartao();

            cartaoDescricao.Dock = DockStyle.Fill;

            cartaoDescricao.Margin =
                new Padding(0, 4, 0, 0);

            layout.Controls.Add(
                cartaoDescricao, 0, 2);

            TableLayoutPanel layoutDescricao =
                new TableLayoutPanel();

            layoutDescricao.Dock = DockStyle.Fill;

            layoutDescricao.Padding =
                new Padding(20, 12, 20, 12);

            layoutDescricao.ColumnCount = 1;
            layoutDescricao.RowCount = 2;

            layoutDescricao.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            layoutDescricao.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 35F));

            layoutDescricao.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            cartaoDescricao.Controls.Add(
                layoutDescricao);

            Label tituloDescricao = CriarLabel(
                "Descrição do item",
                13F,
                FontStyle.Bold,
                texto);

            tituloDescricao.Dock = DockStyle.Fill;

            layoutDescricao.Controls.Add(
                tituloDescricao, 0, 0);

            TextBox txtDescricao = new TextBox();

            txtDescricao.Multiline = true;
            txtDescricao.ReadOnly = true;

            txtDescricao.Dock = DockStyle.Fill;

            txtDescricao.BorderStyle =
                BorderStyle.None;

            txtDescricao.ScrollBars =
                ScrollBars.Vertical;

            txtDescricao.Font =
                new Font("Segoe UI", 10F);

            txtDescricao.BackColor =
                Color.White;

            txtDescricao.ForeColor = cinza;

            txtDescricao.TabStop = false;

            txtDescricao.Text =
                string.IsNullOrWhiteSpace(item.Descricao)
                    ? "Nenhuma descrição cadastrada."
                    : item.Descricao;

            layoutDescricao.Controls.Add(
                txtDescricao, 0, 1);

            // ======================================
            // RODAPÉ - TRÊS BOTÕES
            // ======================================

            TableLayoutPanel rodape =
                new TableLayoutPanel();

            rodape.Dock = DockStyle.Fill;

            rodape.BackColor = fundo;

            rodape.Margin =
                new Padding(0);

            rodape.Padding =
                new Padding(0, 14, 0, 0);

            rodape.ColumnCount = 4;
            rodape.RowCount = 1;

            // ESPAÇO FLEXÍVEL À ESQUERDA
            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            // FECHAR
            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute, 132F));

            // EXCLUIR
            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute, 150F));

            // EDITAR
            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute, 165F));

            rodape.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            layout.Controls.Add(
                rodape, 0, 3);

            // ======================================
            // TEXTO DO RODAPÉ
            // ======================================

            Label lblInfo = CriarLabel(
                "Informações do estoque.",
                9F,
                FontStyle.Regular,
                cinza);

            lblInfo.Dock = DockStyle.Fill;

            lblInfo.Margin =
                new Padding(2, 0, 8, 0);

            lblInfo.AutoEllipsis = true;

            rodape.Controls.Add(
                lblInfo, 0, 0);

            // ======================================
            // BOTÃO FECHAR
            // ======================================

            Button btnFechar = CriarBotao(
                "Fechar",
                Color.FromArgb(237, 242, 240),
                texto);

            btnFechar.Dock = DockStyle.Top;
            btnFechar.Height = 44;

            btnFechar.Margin =
                new Padding(0, 3, 10, 0);

            rodape.Controls.Add(
                btnFechar, 1, 0);

            btnFechar.Click += (s, e) =>
            {
                this.DialogResult =
                    DialogResult.Cancel;

                this.Close();
            };

            // ======================================
            // BOTÃO EXCLUIR ITEM
            // ======================================

            Button btnExcluir = CriarBotao(
                "Excluir item",
                vermelho,
                Color.White);

            btnExcluir.Dock = DockStyle.Top;
            btnExcluir.Height = 44;

            btnExcluir.Margin =
                new Padding(0, 3, 10, 0);

            rodape.Controls.Add(
                btnExcluir, 2, 0);

            // ======================================
            // BOTÃO EDITAR ITEM
            // ======================================

            Button btnEditar = CriarBotao(
                "Editar item",
                verde,
                Color.White);

            btnEditar.Dock = DockStyle.Top;
            btnEditar.Height = 44;

            btnEditar.Margin =
                new Padding(0, 3, 0, 0);

            rodape.Controls.Add(
                btnEditar, 3, 0);

            // ======================================
            // ARREDONDAR OS BOTÕES
            // ======================================

            Arredondar(btnFechar, 10);
            Arredondar(btnExcluir, 10);
            Arredondar(btnEditar, 10);

            // ======================================
            // EFEITOS AO PASSAR O MOUSE
            // ======================================

            btnFechar.MouseEnter += (s, e) =>
            {
                btnFechar.BackColor =
                    Color.FromArgb(220, 230, 225);
            };

            btnFechar.MouseLeave += (s, e) =>
            {
                btnFechar.BackColor =
                    Color.FromArgb(237, 242, 240);
            };

            btnExcluir.MouseEnter += (s, e) =>
            {
                btnExcluir.BackColor =
                    Color.FromArgb(190, 36, 53);
            };

            btnExcluir.MouseLeave += (s, e) =>
            {
                btnExcluir.BackColor = vermelho;
            };

            btnEditar.MouseEnter += (s, e) =>
            {
                btnEditar.BackColor = verdeEscuro;
            };

            btnEditar.MouseLeave += (s, e) =>
            {
                btnEditar.BackColor = verde;
            };

            // ======================================
            // EVENTO: EDITAR ITEM
            // ======================================

            btnEditar.Click += (s, e) =>
            {
                using (frmEditarItem telaEditar =
                    new frmEditarItem(item))
                {
                    DialogResult resultado =
                        telaEditar.ShowDialog(this);

                    if (resultado != DialogResult.OK)
                        return;

                    if (telaEditar.ItemEditado == null)
                        return;

                    // Devolve os dados editados
                    // para o frmEstoque.
                    ItemEditado =
                        telaEditar.ItemEditado;

                    ExclusaoSolicitada = false;

                    this.DialogResult =
                        DialogResult.OK;

                    this.Close();
                }
            };

            // ======================================
            // EVENTO: EXCLUIR ITEM
            // ======================================

            btnExcluir.Click += (s, e) =>
            {
                string mensagem =
                    "Tem certeza que deseja excluir " +
                    "permanentemente este item?\n\n" +

                    "Nome: " + item.Nome + "\n" +

                    "Tipo: " + item.Tipo + "\n" +

                    "Código: #" +
                    item.Id.ToString("D4") + "\n\n" +

                    "Esta ação não poderá ser desfeita.";

                DialogResult confirmar =
                    MessageBox.Show(
                        this,
                        mensagem,
                        "Confirmar exclusão",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2);

                if (confirmar != DialogResult.Yes)
                    return;

                // Somente solicita a exclusão.
                // O DELETE será executado
                // no frmEstoque.
                ExclusaoSolicitada = true;

                ItemEditado = null;

                this.DialogResult =
                    DialogResult.Yes;

                this.Close();
            };

            // ======================================
            // ATALHO ESC
            // ======================================

            this.CancelButton = btnFechar;
        }

        // ==========================================
        // CRIAR LABEL
        // ==========================================

        private Label CriarLabel(
            string conteudo,
            float tamanho,
            FontStyle estilo,
            Color cor)
        {
            Label label = new Label();

            label.Text = conteudo;

            label.Font = new Font(
                "Segoe UI",
                tamanho,
                estilo);

            label.ForeColor = cor;

            label.BackColor =
                Color.Transparent;

            label.AutoSize = false;

            label.TextAlign =
                ContentAlignment.MiddleLeft;

            return label;
        }

        // ==========================================
        // CRIAR BOTÃO ESTILIZADO
        // ==========================================

        private Button CriarBotao(
            string titulo,
            Color corFundo,
            Color corTexto)
        {
            Button botao = new Button();

            botao.Text = titulo;

            botao.BackColor = corFundo;
            botao.ForeColor = corTexto;

            botao.FlatStyle =
                FlatStyle.Flat;

            botao.FlatAppearance.BorderSize = 0;

            botao.UseVisualStyleBackColor = false;

            botao.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Bold);

            botao.Cursor = Cursors.Hand;

            botao.TextAlign =
                ContentAlignment.MiddleCenter;

            return botao;
        }

        // ==========================================
        // ADICIONAR CAMPO DE INFORMAÇÕES
        // ==========================================

        private void AdicionarCampo(
            Panel painel,
            string titulo,
            string valor,
            int y)
        {
            Label lblTitulo = CriarLabel(
                titulo,
                9F,
                FontStyle.Regular,
                cinza);

            lblTitulo.SetBounds(
                22, y, 310, 21);

            painel.Controls.Add(lblTitulo);

            Label lblValor = CriarLabel(
                string.IsNullOrWhiteSpace(valor)
                    ? "Não informado"
                    : valor,
                10F,
                FontStyle.Bold,
                texto);

            lblValor.SetBounds(
                22, y + 19, 310, 29);

            lblValor.AutoEllipsis = true;

            painel.Controls.Add(lblValor);
        }

        // ==========================================
        // IDENTIFICAR SITUAÇÃO DO ESTOQUE
        // ==========================================

        private string ObterStatus()
        {
            if (item.Quantidade <= 0)
                return "Sem estoque";

            if (item.Quantidade <= item.EstoqueMinimo)
                return "Estoque baixo";

            return "Estoque normal";
        }

        // ==========================================
        // COR DA SITUAÇÃO
        // ==========================================

        private Color ObterCorStatus()
        {
            if (item.Quantidade <= 0)
            {
                return vermelho;
            }

            if (item.Quantidade <= item.EstoqueMinimo)
            {
                return Color.FromArgb(
                    194, 130, 18);
            }

            return verdeEscuro;
        }

        // ==========================================
        // CRIAR CARTÃO
        // ==========================================

        private Panel CriarCartao()
        {
            Panel painel = new Panel();

            painel.BackColor = Color.White;

            painel.BorderStyle =
                BorderStyle.None;

            Arredondar(painel, 14);

            painel.Paint += (s, e) =>
            {
                if (painel.Width < 3 ||
                    painel.Height < 3)
                    return;

                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                Rectangle area = new Rectangle(
                    0,
                    0,
                    painel.Width - 1,
                    painel.Height - 1);

                using (GraphicsPath caminho =
                    CriarCaminho(area, 14))
                using (Pen caneta = new Pen(borda))
                {
                    e.Graphics.DrawPath(
                        caneta, caminho);
                }
            };

            return painel;
        }

        // ==========================================
        // CAMINHO COM CANTOS ARREDONDADOS
        // ==========================================

        private GraphicsPath CriarCaminho(
            Rectangle area,
            int raio)
        {
            GraphicsPath caminho =
                new GraphicsPath();

            if (area.Width <= 0 ||
                area.Height <= 0)
            {
                return caminho;
            }

            int d = Math.Min(
                raio * 2,
                Math.Min(
                    area.Width,
                    area.Height));

            if (d <= 0)
            {
                caminho.AddRectangle(area);
                return caminho;
            }

            caminho.AddArc(
                area.Left,
                area.Top,
                d, d, 180, 90);

            caminho.AddArc(
                area.Right - d,
                area.Top,
                d, d, 270, 90);

            caminho.AddArc(
                area.Right - d,
                area.Bottom - d,
                d, d, 0, 90);

            caminho.AddArc(
                area.Left,
                area.Bottom - d,
                d, d, 90, 90);

            caminho.CloseFigure();

            return caminho;
        }

        // ==========================================
        // APLICAR BORDAS ARREDONDADAS
        // ==========================================

        private void Arredondar(
            Control controle,
            int raio)
        {
            Action aplicar = () =>
            {
                if (controle.Width < 2 ||
                    controle.Height < 2)
                {
                    return;
                }

                Rectangle area = new Rectangle(
                    0,
                    0,
                    controle.Width - 1,
                    controle.Height - 1);

                using (GraphicsPath caminho =
                    CriarCaminho(area, raio))
                {
                    Region antiga = controle.Region;

                    controle.Region =
                        new Region(caminho);

                    if (antiga != null)
                    {
                        antiga.Dispose();
                    }
                }
            };

            controle.SizeChanged += (s, e) =>
            {
                aplicar();
            };

            aplicar();
        }
    }
}
