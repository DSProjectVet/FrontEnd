
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{
    public class frmReceitaConsulta : Form
    {
        private readonly int idConsulta;

        // ==========================================
        // CORES MED VET
        // ==========================================

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

        private readonly Color vermelho =
            Color.FromArgb(220, 53, 69);

        private readonly CultureInfo cultura =
            new CultureInfo("pt-BR");

        // ==========================================
        // CONTROLES
        // ==========================================

        private TabControl abas;

        private ComboBox cmbProduto;
        private ComboBox cmbServico;
        private Button btnAdicionarProduto;

        private NumericUpDown numQtdProduto;
        private NumericUpDown numQtdServico;

        private TextBox txtPrecoServico;

        private DataGridView gridProdutos;
        private DataGridView gridServicos;

        private Label lblDisponibilidade;
        private Label lblSubtotalServico;
        private Label lblRodape;

        private bool ocupado = false;
        private bool carregamentoInicial = false;
        private bool carregandoDados = false;

        // ==========================================
        // CONSTRUTOR
        // ==========================================

        public frmReceitaConsulta(
            int id,
            bool abrirAbaServicos = false)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "ID da consulta inválido.");
            }

            idConsulta = id;

            ConstruirInterface();

            // Evita acesso a controles descartados durante
            // uma gravação ou um carregamento assíncrono.
            FormClosing += (s, e) =>
            {
                if (ocupado || carregandoDados)
                {
                    e.Cancel = true;
                    lblRodape.Text =
                        "Conclua a operação antes de fechar a tela.";
                }
            };

            if (abrirAbaServicos)
            {
                abas.SelectedIndex = 1;
            }

            Shown += async (s, e) =>
            {
                if (carregamentoInicial)
                    return;

                carregamentoInicial = true;

                await CarregarDadosAsync();
            };
        }

        // ==========================================
        // INTERFACE PRINCIPAL
        // ==========================================

        private void ConstruirInterface()
        {
            Text = "Med Vet - Receita e Consumo";

            ClientSize = new Size(1040, 690);

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

            layout.Padding =
                new Padding(24, 18, 24, 18);

            layout.ColumnCount = 1;
            layout.RowCount = 3;

            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 84F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 66F));

            Controls.Add(layout);

            // ======================================
            // CABEÇALHO
            // ======================================

            Panel cabecalho = new Panel();

            cabecalho.Dock = DockStyle.Fill;

            layout.Controls.Add(
                cabecalho, 0, 0);

            Label titulo = new Label();

            titulo.Text =
                "Receita e consumo da consulta";

            titulo.Font = new Font(
                "Segoe UI", 21F, FontStyle.Bold);

            titulo.ForeColor = texto;

            titulo.SetBounds(
                0, 0, 920, 45);

            cabecalho.Controls.Add(titulo);

            Label subtitulo = new Label();

            subtitulo.Text =
                "Consulta #" +
                idConsulta.ToString("D4") +
                "  •  Produtos utilizados e serviços realizados";

            subtitulo.Font =
                new Font("Segoe UI", 10F);

            subtitulo.ForeColor = cinza;

            subtitulo.SetBounds(
                2, 46, 940, 28);

            cabecalho.Controls.Add(subtitulo);

            // ======================================
            // ABAS
            // ======================================

            abas = new TabControl();

            abas.Dock = DockStyle.Fill;

            abas.Font =
                new Font("Segoe UI", 10F);

            abas.Margin = new Padding(0);

            layout.Controls.Add(abas, 0, 1);

            TabPage paginaProdutos =
                new TabPage("Produtos / Estoque");

            TabPage paginaServicos =
                new TabPage("Serviços");

            paginaProdutos.BackColor = fundo;
            paginaServicos.BackColor = fundo;

            abas.TabPages.Add(paginaProdutos);
            abas.TabPages.Add(paginaServicos);

            ConstruirAbaProdutos(paginaProdutos);
            ConstruirAbaServicos(paginaServicos);


            // ==========================================
            // TERCEIRA ABA: PRESCRIÇÕES
            // ==========================================

            TabPage paginaPrescricoes =
                new TabPage("Prescrições");

            paginaPrescricoes.BackColor = fundo;

            abas.TabPages.Add(paginaPrescricoes);

            TableLayoutPanel layoutPrescricoes =
                new TableLayoutPanel();

            layoutPrescricoes.Dock = DockStyle.Fill;

            layoutPrescricoes.Padding =
                new Padding(30);

            layoutPrescricoes.ColumnCount = 1;
            layoutPrescricoes.RowCount = 3;

            layoutPrescricoes.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 75F));

            layoutPrescricoes.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 85F));

            layoutPrescricoes.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            paginaPrescricoes.Controls.Add(
                layoutPrescricoes);

            Label tituloPrescricoes = new Label();

            tituloPrescricoes.Text =
                "Prescrições de medicamentos";

            tituloPrescricoes.Font =
                new Font("Segoe UI", 19F, FontStyle.Bold);

            tituloPrescricoes.ForeColor = texto;
            tituloPrescricoes.Dock = DockStyle.Fill;

            layoutPrescricoes.Controls.Add(
                tituloPrescricoes, 0, 0);

            Label descricaoPrescricoes = new Label();

            descricaoPrescricoes.Text =
                "Cadastre medicamentos prescritos, " +
                "dosagem, frequência, duração e observações.\n" +
                "O consumo de estoque será registrado " +
                "junto com a dispensação.";

            descricaoPrescricoes.Font =
                new Font("Segoe UI", 10F);

            descricaoPrescricoes.ForeColor = cinza;

            descricaoPrescricoes.Dock = DockStyle.Fill;

            layoutPrescricoes.Controls.Add(
                descricaoPrescricoes, 0, 1);

            Button btnAbrirPrescricoes = CriarBotao(
                "Gerenciar prescrições",
                verde,
                Color.White,
                230);

            btnAbrirPrescricoes.Dock = DockStyle.Top;
            btnAbrirPrescricoes.Margin = new Padding(0);

            layoutPrescricoes.Controls.Add(
                btnAbrirPrescricoes, 0, 2);

            btnAbrirPrescricoes.Click += (s, e) =>
            {
                try
                {
                    using (frmPrescricaoConsulta tela =
                        new frmPrescricaoConsulta(idConsulta))
                    {
                        tela.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this,
                        "Não foi possível abrir prescrições.\n\n" +
                        ex.Message,
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };


            // ======================================
            // RODAPÉ
            // ======================================

            TableLayoutPanel rodape =
                new TableLayoutPanel();

            rodape.Dock = DockStyle.Fill;
            rodape.Margin = new Padding(0);

            rodape.ColumnCount = 2;
            rodape.RowCount = 1;

            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute, 160F));

            rodape.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            layout.Controls.Add(rodape, 0, 2);

            lblRodape = new Label();

            lblRodape.Text =
                "Aguardando carregamento...";

            lblRodape.ForeColor = cinza;

            lblRodape.Dock = DockStyle.Fill;

            lblRodape.TextAlign =
                ContentAlignment.MiddleLeft;

            lblRodape.AutoEllipsis = true;

            rodape.Controls.Add(lblRodape, 0, 0);

            Button btnFechar = CriarBotao(
                "Fechar",
                Color.FromArgb(231, 240, 236),
                texto,
                145);

            btnFechar.Dock = DockStyle.Fill;

            btnFechar.Margin =
                new Padding(5, 12, 0, 6);

            btnFechar.DialogResult =
                DialogResult.Cancel;

            rodape.Controls.Add(btnFechar, 1, 0);

            CancelButton = btnFechar;
        }

        // ==========================================
        // ABA DE PRODUTOS
        // ==========================================

        private void ConstruirAbaProdutos(
            TabPage pagina)
        {
            TableLayoutPanel layout =
                CriarLayoutAba(pagina);

            FlowLayoutPanel campos =
                CriarBarraCampos();

            layout.Controls.Add(campos, 0, 0);

            // PRODUTO
            cmbProduto = new ComboBox();

            cmbProduto.DropDownStyle =
                ComboBoxStyle.DropDownList;

            AdicionarCampo(
                campos,
                "Selecione o produto",
                cmbProduto,
                390);

            // QUANTIDADE
            numQtdProduto =
                CriarNumero(1, 1000000);

            AdicionarCampo(
                campos,
                "Quantidade",
                numQtdProduto,
                120);

            // ADICIONAR
            btnAdicionarProduto = CriarBotao(
                "+ Adicionar produto",
                verde,
                Color.White,
                185);

            // Será habilitado apenas com estoque suficiente.
            btnAdicionarProduto.Enabled = false;
            campos.Controls.Add(btnAdicionarProduto);

            btnAdicionarProduto.Click += async (s, e) =>
            {
                DataRowView produto =
                    cmbProduto.SelectedItem
                    as DataRowView;

                if (produto == null)
                {
                    Aviso("Selecione um produto.");
                    return;
                }

                int idProduto =
                    Convert.ToInt32(
                        produto["ID_Produto"]);

                int quantidade =
                    Decimal.ToInt32(
                        numQtdProduto.Value);

                // Defesa de interface. O método RegistrarProduto
                // também valida o estoque atualizado no SQL Server.
                int disponivel = Convert.ToInt32(
                    produto["Disponivel"]);

                if (quantidade > disponivel)
                {
                    MessageBox.Show(
                        this,
                        "Estoque insuficiente.\n\n" +
                        "Disponível: " + disponivel +
                        " unidade(s).\n" +
                        "Solicitado: " + quantidade +
                        " unidade(s).",
                        "Med Vet - Estoque",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    MostrarDisponibilidade();
                    return;
                }

                await ExecutarOperacaoAsync(
                    () => RegistrarProduto(
                        idProduto, quantidade),
                    "Produto registrado e estoque atualizado.");
            };

            // DISPONIBILIDADE
            lblDisponibilidade = new Label();

            lblDisponibilidade.Dock =
                DockStyle.Fill;

            lblDisponibilidade.ForeColor = cinza;

            lblDisponibilidade.TextAlign =
                ContentAlignment.MiddleLeft;

            lblDisponibilidade.Padding =
                new Padding(8, 0, 0, 0);

            layout.Controls.Add(
                lblDisponibilidade, 0, 1);

            cmbProduto.SelectedIndexChanged +=
                (s, e) =>
                {
                    MostrarDisponibilidade();
                };

            // Atualiza a validação quando o funcionário muda
            // a quantidade, e não apenas o produto.
            numQtdProduto.ValueChanged += (s, e) =>
            {
                MostrarDisponibilidade();
            };

            // TABELA DE PRODUTOS
            gridProdutos = CriarGrade();

            layout.Controls.Add(
                gridProdutos, 0, 2);

            // AÇÕES INFERIORES
            Panel painelAcoes = new Panel();

            painelAcoes.Dock = DockStyle.Fill;

            layout.Controls.Add(
                painelAcoes, 0, 3);

            Button btnRemover = CriarBotao(
                "Cancelar lançamento",
                vermelho,
                Color.White,
                195);

            btnRemover.Dock = DockStyle.Right;

            btnRemover.Margin =
                new Padding(0, 6, 0, 4);

            painelAcoes.Controls.Add(btnRemover);

            btnRemover.Click += async (s, e) =>
            {
                int? idProduto = ObterIdGrade(
                    gridProdutos, "ID_Produto");

                if (!idProduto.HasValue)
                {
                    Aviso(
                        "Selecione um produto na tabela.");

                    return;
                }

                string mensagem =
                    "Cancelar o lançamento do produto?\n\n" +
                    "A quantidade será DEVOLVIDA " +
                    "ao estoque.\n\n" +
                    "Confirme somente se o lançamento " +
                    "foi incorreto ou se o material " +
                    "foi efetivamente devolvido.\n\n" +
                    "Não estorne produtos já utilizados.";

                if (!ConfirmarRemocao(mensagem))
                    return;

                await ExecutarOperacaoAsync(
                    () => RemoverProduto(
                        idProduto.Value),
                    "Lançamento cancelado e estoque estornado.");
            };
        }

        // ==========================================
        // ABA DE SERVIÇOS
        // ==========================================

        private void ConstruirAbaServicos(
            TabPage pagina)
        {
            TableLayoutPanel layout =
                CriarLayoutAba(pagina);

            FlowLayoutPanel campos =
                CriarBarraCampos();

            layout.Controls.Add(campos, 0, 0);

            // ==================================
            // COMBOBOX DE SERVIÇOS
            // ==================================

            cmbServico = new ComboBox();

            cmbServico.DropDownStyle =
                ComboBoxStyle.DropDownList;

            AdicionarCampo(
                campos,
                "Selecione o serviço",
                cmbServico,
                390);

            // ==================================
            // QUANTIDADE
            // ==================================

            numQtdServico =
                CriarNumero(1, 1000000);

            AdicionarCampo(
                campos,
                "Quantidade",
                numQtdServico,
                120);

            // ==================================
            // PREÇO AUTOMÁTICO
            // ==================================

            txtPrecoServico = new TextBox();

            txtPrecoServico.ReadOnly = true;

            txtPrecoServico.BackColor =
                Color.FromArgb(235, 243, 239);

            txtPrecoServico.ForeColor =
                verdeEscuro;

            txtPrecoServico.Text = "R$ 0,00";

            AdicionarCampo(
                campos,
                "Preço unitário",
                txtPrecoServico,
                145);

            // ==================================
            // BOTÃO ADICIONAR
            // ==================================

            Button btnAdicionar = CriarBotao(
                "+ Adicionar serviço",
                verde,
                Color.White,
                185);

            campos.Controls.Add(btnAdicionar);

            btnAdicionar.Click += async (s, e) =>
            {
                DataRowView servico =
                    cmbServico.SelectedItem
                    as DataRowView;

                if (servico == null)
                {
                    Aviso("Selecione um serviço.");
                    return;
                }

                int idServico =
                    Convert.ToInt32(
                        servico["ID_Servico"]);

                int quantidade =
                    Decimal.ToInt32(
                        numQtdServico.Value);

                await ExecutarOperacaoAsync(
                    () => RegistrarServico(
                        idServico, quantidade),
                    "Serviço registrado na consulta.");
            };

            // ==================================
            // PRÉVIA DO TOTAL
            // ==================================

            lblSubtotalServico = new Label();

            lblSubtotalServico.Dock =
                DockStyle.Fill;

            lblSubtotalServico.ForeColor =
                verdeEscuro;

            lblSubtotalServico.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);

            lblSubtotalServico.TextAlign =
                ContentAlignment.MiddleLeft;

            lblSubtotalServico.Padding =
                new Padding(8, 0, 0, 0);

            layout.Controls.Add(
                lblSubtotalServico, 0, 1);

            cmbServico.SelectedIndexChanged +=
                (s, e) =>
                {
                    MostrarPrecoServico();
                };

            numQtdServico.ValueChanged +=
                (s, e) =>
                {
                    MostrarPrecoServico();
                };

            // ==================================
            // TABELA DE SERVIÇOS
            // ==================================

            gridServicos = CriarGrade();

            layout.Controls.Add(
                gridServicos, 0, 2);

            // ==================================
            // BOTÃO REMOVER
            // ==================================

            Panel painelAcoes = new Panel();

            painelAcoes.Dock = DockStyle.Fill;

            layout.Controls.Add(
                painelAcoes, 0, 3);

            Button btnRemover = CriarBotao(
                "Remover serviço",
                vermelho,
                Color.White,
                180);

            btnRemover.Dock = DockStyle.Right;

            btnRemover.Margin =
                new Padding(0, 6, 0, 4);

            painelAcoes.Controls.Add(btnRemover);

            btnRemover.Click += async (s, e) =>
            {
                int? idServico = ObterIdGrade(
                    gridServicos, "ID_Servico");

                if (!idServico.HasValue)
                {
                    Aviso(
                        "Selecione um serviço na tabela.");

                    return;
                }

                if (!ConfirmarRemocao(
                    "Remover o serviço selecionado " +
                    "desta consulta?"))
                {
                    return;
                }

                await ExecutarOperacaoAsync(
                    () => RemoverServico(
                        idServico.Value),
                    "Serviço removido da consulta.");
            };
        }

        // ==========================================
        // COMPONENTES VISUAIS
        // ==========================================

        private TableLayoutPanel CriarLayoutAba(
            TabPage pagina)
        {
            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;

            layout.Padding =
                new Padding(14);

            layout.Margin = new Padding(0);

            layout.ColumnCount = 1;
            layout.RowCount = 4;

            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 95F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 42F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 58F));

            pagina.Controls.Add(layout);

            return layout;
        }

        private FlowLayoutPanel CriarBarraCampos()
        {
            FlowLayoutPanel painel =
                new FlowLayoutPanel();

            painel.Dock = DockStyle.Fill;

            painel.FlowDirection =
                FlowDirection.LeftToRight;

            painel.WrapContents = true;

            painel.AutoScroll = true;

            painel.Margin = new Padding(0);

            return painel;
        }

        private void AdicionarCampo(
            FlowLayoutPanel painel,
            string titulo,
            Control campo,
            int largura)
        {
            Panel grupo = new Panel();

            grupo.Width = largura;
            grupo.Height = 77;

            grupo.Margin =
                new Padding(0, 0, 12, 0);

            Label label = new Label();

            label.Text = titulo;

            label.ForeColor = texto;

            label.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            label.SetBounds(
                0, 3, largura, 23);

            grupo.Controls.Add(label);

            campo.SetBounds(
                0, 32, largura - 3, 32);

            campo.Font =
                new Font("Segoe UI", 10F);

            grupo.Controls.Add(campo);

            painel.Controls.Add(grupo);
        }

        private NumericUpDown CriarNumero(
            int minimo, int maximo)
        {
            NumericUpDown numero =
                new NumericUpDown();

            numero.Minimum = minimo;
            numero.Maximum = maximo;
            numero.Value = minimo;

            numero.ThousandsSeparator = true;

            return numero;
        }

        private Button CriarBotao(
            string titulo,
            Color corFundo,
            Color corTexto,
            int largura)
        {
            Button botao = new Button();

            botao.Text = titulo;

            botao.Size =
                new Size(largura, 42);

            botao.Margin =
                new Padding(5, 29, 5, 4);

            botao.BackColor = corFundo;
            botao.ForeColor = corTexto;

            botao.FlatStyle = FlatStyle.Flat;

            botao.FlatAppearance.BorderSize = 0;

            botao.UseVisualStyleBackColor = false;

            botao.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            botao.Cursor = Cursors.Hand;

            return botao;
        }

        private DataGridView CriarGrade()
        {
            DataGridView grade =
                new DataGridView();

            grade.Dock = DockStyle.Fill;

            grade.BackgroundColor = Color.White;

            grade.BorderStyle =
                BorderStyle.None;

            grade.ReadOnly = true;

            grade.AutoGenerateColumns = true;

            grade.AllowUserToAddRows = false;
            grade.AllowUserToDeleteRows = false;
            grade.AllowUserToResizeRows = false;

            grade.RowHeadersVisible = false;

            grade.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            grade.MultiSelect = false;

            grade.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            grade.EnableHeadersVisualStyles = false;

            grade.ColumnHeadersHeight = 43;
            grade.RowTemplate.Height = 40;

            grade.ColumnHeadersDefaultCellStyle.BackColor =
                verdeEscuro;

            grade.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            grade.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI", 9F, FontStyle.Bold);

            grade.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            grade.DefaultCellStyle.ForeColor = texto;
            grade.DefaultCellStyle.BackColor = Color.White;

            grade.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(225, 248, 240);

            grade.DefaultCellStyle.SelectionForeColor =
                verdeEscuro;

            grade.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 252, 250);
            grade.AlternatingRowsDefaultCellStyle.ForeColor = texto;

            return grade;
        }

        // ==========================================
        // CONNECTION STRING
        // ==========================================

        private string ObterConnectionString()
        {
            foreach (SettingsProperty propriedade in
                Properties.Settings.Default.Properties)
            {
                SpecialSettingAttribute atributo =
                    propriedade.Attributes[
                        typeof(SpecialSettingAttribute)]
                    as SpecialSettingAttribute;

                if (atributo == null)
                    continue;

                if (atributo.SpecialSetting !=
                    SpecialSetting.ConnectionString)
                    continue;

                string valor = Convert.ToString(
                    Properties.Settings.Default[
                        propriedade.Name]);

                if (!string.IsNullOrWhiteSpace(valor))
                    return valor;
            }

            throw new Exception(
                "Nenhuma connection string foi " +
                "encontrada em Properties.Settings.");
        }

        // ==========================================
        // ESTRUTURA DE CARREGAMENTO
        // ==========================================

        private class DadosTela
        {
            public DataTable CatalogoProdutos;
            public DataTable CatalogoServicos;

            public DataTable ProdutosConsulta;
            public DataTable ServicosConsulta;
        }

        private DataTable ConsultarTabela(
            SqlConnection conexao,
            string sql,
            bool filtrarConsulta)
        {
            DataTable tabela = new DataTable();

            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(comando))
            {
                comando.CommandTimeout = 30;

                if (filtrarConsulta)
                {
                    comando.Parameters.Add(
                        "@Consulta",
                        SqlDbType.Int).Value =
                            idConsulta;
                }

                adapter.Fill(tabela);
            }

            return tabela;
        }

        private DadosTela ConsultarDadosNoBanco()
        {
            DadosTela dados = new DadosTela();

            // ==================================
            // CATÁLOGO DE PRODUTOS
            // ==================================

            string sqlCatalogoProdutos = @"
                SELECT
                    p.ID_Produto,
                    p.Nome,
                    CAST(ISNULL(p.Preco, 0)
                        AS DECIMAL(18,2)) AS Preco,

                    ISNULL(ep.Quantidade, 0)
                        AS Disponivel,

                    ISNULL(p.Nome, 'Produto sem nome')
                    + ' (Estoque: '
                    + CAST(
                        ISNULL(ep.Quantidade, 0)
                        AS VARCHAR(20))
                    + ')' AS Exibicao

                FROM dbo.Produtos AS p

                LEFT JOIN dbo.Estoque_Produto AS ep
                    ON ep.fk_Produtos_ID_Produto =
                       p.ID_Produto

                ORDER BY p.Nome;
            ";

            // ==================================
            // CATÁLOGO DE SERVIÇOS
            // ==================================

            string sqlCatalogoServicos = @"
                SELECT
                    s.ID_Servico,
                    s.Tipo,
                    s.Descricao,

                    CAST(ISNULL(s.Preco, 0)
                        AS DECIMAL(18,2)) AS Preco,

                    ISNULL(s.Descricao, 'Serviço')
                    + '  |  '
                    + CASE s.Tipo
                        WHEN 'C' THEN 'Consulta'
                        WHEN 'V' THEN 'Vacina'
                        WHEN 'E' THEN 'Exame'
                        WHEN 'P' THEN 'Procedimento'
                        ELSE 'Outro'
                      END AS Exibicao

                FROM dbo.Servicos AS s

                ORDER BY s.Descricao;
            ";

            // ==================================
            // PRODUTOS DA CONSULTA
            // ==================================

            string sqlProdutosConsulta = @"
                SELECT
                    cp.fk_Produtos_ID_Produto
                        AS ID_Produto,

                    p.Nome AS Produto,

                    cp.Quantidade,
                    cp.Preco,

                    CAST(
                        cp.Quantidade * cp.Preco
                        AS DECIMAL(18,2))
                        AS Subtotal

                FROM dbo.Consulta_Produto AS cp

                INNER JOIN dbo.Produtos AS p
                    ON p.ID_Produto =
                       cp.fk_Produtos_ID_Produto

                WHERE cp.fk_Consultas_ID_Consulta =
                      @Consulta

                ORDER BY p.Nome;
            ";

            // ==================================
            // SERVIÇOS DA CONSULTA
            // ==================================

            string sqlServicosConsulta = @"
                SELECT
                    cs.fk_Servicos_ID_Servico
                        AS ID_Servico,

                    s.Descricao AS Servico,

                    CASE s.Tipo
                        WHEN 'C' THEN 'Consulta'
                        WHEN 'V' THEN 'Vacina'
                        WHEN 'E' THEN 'Exame'
                        WHEN 'P' THEN 'Procedimento'
                        ELSE 'Outro'
                    END AS Categoria,

                    cs.Quantidade,
                    cs.Preco,

                    CAST(
                        cs.Quantidade * cs.Preco
                        AS DECIMAL(18,2))
                        AS Subtotal

                FROM dbo.Consulta_Servico AS cs

                INNER JOIN dbo.Servicos AS s
                    ON s.ID_Servico =
                       cs.fk_Servicos_ID_Servico

                WHERE cs.fk_Consultas_ID_Consulta =
                      @Consulta

                ORDER BY s.Descricao;
            ";

            using (SqlConnection conexao =
                new SqlConnection(
                    ObterConnectionString()))
            {
                conexao.Open();

                dados.CatalogoProdutos =
                    ConsultarTabela(
                        conexao,
                        sqlCatalogoProdutos,
                        false);

                dados.CatalogoServicos =
                    ConsultarTabela(
                        conexao,
                        sqlCatalogoServicos,
                        false);

                dados.ProdutosConsulta =
                    ConsultarTabela(
                        conexao,
                        sqlProdutosConsulta,
                        true);

                dados.ServicosConsulta =
                    ConsultarTabela(
                        conexao,
                        sqlServicosConsulta,
                        true);
            }

            return dados;
        }

        // ==========================================
        // ATUALIZAR INTERFACE COM DADOS DO BANCO
        // ==========================================

        private async Task<bool> CarregarDadosAsync()
        {
            carregandoDados = true;
            abas.Enabled = false;

            lblRodape.Text =
                "Atualizando informações da consulta...";

            try
            {
                DadosTela dados =
                    await Task.Run(
                        () => ConsultarDadosNoBanco());

                // ==================================
                // PRODUTOS
                // ==================================

                cmbProduto.DataSource = null;

                cmbProduto.DisplayMember =
                    "Exibicao";

                cmbProduto.ValueMember =
                    "ID_Produto";

                cmbProduto.DataSource =
                    dados.CatalogoProdutos;

                cmbProduto.SelectedIndex = -1;

                // ==================================
                // SERVIÇOS
                // ==================================

                cmbServico.DataSource = null;

                cmbServico.DisplayMember =
                    "Exibicao";

                cmbServico.ValueMember =
                    "ID_Servico";

                cmbServico.DataSource =
                    dados.CatalogoServicos;

                cmbServico.SelectedIndex = -1;

                // ==================================
                // TABELAS
                // ==================================

                gridProdutos.DataSource =
                    dados.ProdutosConsulta;

                gridServicos.DataSource =
                    dados.ServicosConsulta;

                ConfigurarColunas(gridProdutos);
                ConfigurarColunas(gridServicos);

                gridProdutos.ClearSelection();
                gridServicos.ClearSelection();

                gridProdutos.CurrentCell = null;
                gridServicos.CurrentCell = null;

                MostrarDisponibilidade();
                MostrarPrecoServico();

                // ==================================
                // TOTAIS
                // ==================================

                decimal totalProdutos =
                    SomarSubtotais(
                        dados.ProdutosConsulta);

                decimal totalServicos =
                    SomarSubtotais(
                        dados.ServicosConsulta);

                decimal totalGeral =
                    totalProdutos + totalServicos;

                lblRodape.Text =
                    "Produtos: " +
                    totalProdutos.ToString("C2", cultura)
                    + "  |  Serviços: " +
                    totalServicos.ToString("C2", cultura)
                    + "  |  Total: " +
                    totalGeral.ToString("C2", cultura);

                return true;
            }
            catch (Exception ex)
            {
                lblRodape.Text =
                    "Não foi possível carregar os dados.";

                MessageBox.Show(
                    this,
                    "Erro ao consultar a receita:\n\n" +
                    ex.Message,
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                carregandoDados = false;
                if (!ocupado)
                    abas.Enabled = true;
            }
        }

        private decimal SomarSubtotais(
            DataTable tabela)
        {
            decimal total = 0;

            foreach (DataRow linha in tabela.Rows)
            {
                if (linha["Subtotal"] != DBNull.Value)
                {
                    total += Convert.ToDecimal(
                        linha["Subtotal"]);
                }
            }

            return total;
        }

        private void ConfigurarColunas(
            DataGridView grade)
        {
            if (grade.Columns.Contains("ID_Produto"))
            {
                grade.Columns["ID_Produto"]
                    .HeaderText = "Código";
            }

            if (grade.Columns.Contains("ID_Servico"))
            {
                grade.Columns["ID_Servico"]
                    .HeaderText = "Código";
            }

            if (grade.Columns.Contains("Servico"))
            {
                grade.Columns["Servico"]
                    .HeaderText = "Serviço";
            }

            if (grade.Columns.Contains("Preco"))
            {
                grade.Columns["Preco"]
                    .HeaderText = "Preço unitário";

                grade.Columns["Preco"]
                    .DefaultCellStyle.Format = "C2";

                grade.Columns["Preco"]
                    .DefaultCellStyle.FormatProvider =
                        cultura;
            }

            if (grade.Columns.Contains("Subtotal"))
            {
                grade.Columns["Subtotal"]
                    .DefaultCellStyle.Format = "C2";

                grade.Columns["Subtotal"]
                    .DefaultCellStyle.FormatProvider =
                        cultura;
            }
        }

        // ==========================================
        // PREÇO E ESTOQUE NA INTERFACE
        // ==========================================

        private void MostrarDisponibilidade()
        {
            // Os eventos do ComboBox podem ocorrer durante a
            // construção do formulário e o carregamento dos dados.
            if (cmbProduto == null ||
                numQtdProduto == null ||
                lblDisponibilidade == null ||
                btnAdicionarProduto == null)
                return;

            DataRowView produto =
                cmbProduto.SelectedItem as DataRowView;

            if (produto == null)
            {
                lblDisponibilidade.Text =
                    "Selecione um produto para consultar o estoque.";
                lblDisponibilidade.ForeColor = cinza;
                btnAdicionarProduto.Enabled = false;
                return;
            }

            int disponivel = Convert.ToInt32(
                produto["Disponivel"]);
            int solicitado = Decimal.ToInt32(
                numQtdProduto.Value);
            decimal preco = Convert.ToDecimal(
                produto["Preco"]);

            if (disponivel <= 0)
            {
                lblDisponibilidade.Text =
                    "SEM ESTOQUE — selecione outro produto.";
                lblDisponibilidade.ForeColor = vermelho;
                btnAdicionarProduto.Enabled = false;
                return;
            }

            if (solicitado > disponivel)
            {
                lblDisponibilidade.Text =
                    "ESTOQUE INSUFICIENTE — disponível: " +
                    disponivel + ", solicitado: " + solicitado +
                    ". Reduza a quantidade.";
                lblDisponibilidade.ForeColor = vermelho;
                btnAdicionarProduto.Enabled = false;
                return;
            }

            lblDisponibilidade.Text =
                "Disponível: " + disponivel +
                " unidade(s)  •  Preço unitário: " +
                preco.ToString("C2", cultura) +
                "  •  Total solicitado: " +
                (preco * solicitado).ToString("C2", cultura);

            lblDisponibilidade.ForeColor = verdeEscuro;
            btnAdicionarProduto.Enabled = !ocupado;
        }

        private void MostrarPrecoServico()
        {
            if (cmbServico == null ||
                txtPrecoServico == null ||
                lblSubtotalServico == null)
                return;

            DataRowView servico =
                cmbServico.SelectedItem
                as DataRowView;

            if (servico == null)
            {
                txtPrecoServico.Text = "R$ 0,00";

                lblSubtotalServico.Text =
                    "Selecione um serviço cadastrado.";

                return;
            }

            decimal preco =
                Convert.ToDecimal(
                    servico["Preco"]);

            decimal total =
                preco * numQtdServico.Value;

            txtPrecoServico.Text =
                preco.ToString("C2", cultura);

            lblSubtotalServico.Text =
                "Total do lançamento: " +
                total.ToString("C2", cultura);
        }

        // ==========================================
        // SELEÇÃO E MENSAGENS
        // ==========================================

        private int? ObterIdGrade(
            DataGridView grade,
            string coluna)
        {
            if (grade.CurrentRow == null ||
                grade.SelectedRows.Count == 0)
                return null;

            DataRowView linha =
                grade.CurrentRow.DataBoundItem
                as DataRowView;

            if (linha == null ||
                !linha.Row.Table.Columns.Contains(coluna))
                return null;

            if (linha[coluna] == DBNull.Value)
                return null;

            return Convert.ToInt32(
                linha[coluna]);
        }

        private void Aviso(string mensagem)
        {
            MessageBox.Show(
                this,
                mensagem,
                "Med Vet",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private bool ConfirmarRemocao(
            string mensagem)
        {
            return MessageBox.Show(
                this,
                mensagem,
                "Confirmar operação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2)
                == DialogResult.Yes;
        }

        private async Task ExecutarOperacaoAsync(
            Action operacao,
            string mensagemSucesso)
        {
            if (ocupado)
                return;

            // Salva a seleção para facilitar a correção de
            // quantidade após uma falha de estoque.
            int? produtoSelecionado = null;
            decimal quantidadeDigitada = numQtdProduto.Value;
            DataRowView produto = cmbProduto.SelectedItem as DataRowView;
            if (produto != null)
                produtoSelecionado = Convert.ToInt32(
                    produto["ID_Produto"]);

            ocupado = true;
            abas.Enabled = false;
            btnAdicionarProduto.Enabled = false;

            try
            {
                lblRodape.Text =
                    "Gravando alterações no SQL Server...";

                // SQL exclusivamente na thread de trabalho.
                // Nenhum MessageBox ou controle WinForms é
                // acessado dentro do delegate da operação.
                await Task.Run(operacao);

                // A continuação do await executa na thread UI.
                bool atualizado = await CarregarDadosAsync();
                if (atualizado)
                {
                    Aviso(mensagemSucesso);
                }
                else
                {
                    MessageBox.Show(
                        this,
                        "A operação foi gravada no banco, mas " +
                        "não foi possível atualizar a listagem.\n" +
                        "Use Atualizar ou reabra esta tela.",
                        "Registro salvo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (EstoqueInsuficienteException ex)
            {
                // Aviso na thread da interface; não deixa o
                // erro chegar ao manipulador não tratado.
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Estoque insuficiente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                // Atualiza o saldo, pois outro funcionário
                // pode ter consumido o estoque simultaneamente.
                await CarregarDadosAsync();

                if (produtoSelecionado.HasValue)
                {
                    cmbProduto.SelectedValue =
                        produtoSelecionado.Value;
                    numQtdProduto.Value = quantidadeDigitada;
                }
            }
            catch (SqlException ex) when (ex.Number == 1205)
            {
                MessageBox.Show(
                    this,
                    "O banco identificou um conflito entre " +
                    "operações simultâneas.\n" +
                    "Nenhuma alteração parcial foi mantida. " +
                    "Atualize os dados e tente novamente.",
                    "Operação não concluída",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível concluir a operação.\n\n" +
                    ex.Message,
                    "Erro ao salvar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                ocupado = false;
                abas.Enabled = true;
                MostrarDisponibilidade();
            }
        }

        // ==========================================
        // COMANDOS SQL AUXILIARES
        // ==========================================

        private SqlCommand CriarComando(
            string sql,
            SqlConnection conexao,
            SqlTransaction transacao)
        {
            SqlCommand comando =
                new SqlCommand(
                    sql, conexao, transacao);

            comando.CommandTimeout = 30;

            comando.Parameters.Add(
                "@Consulta",
                SqlDbType.Int).Value =
                    idConsulta;

            return comando;
        }

        private void AdicionarParametroProduto(
            SqlCommand comando,
            int idProduto)
        {
            comando.Parameters.Add(
                "@Produto",
                SqlDbType.Int).Value =
                    idProduto;
        }

        private void AdicionarParametroServico(
            SqlCommand comando,
            int idServico)
        {
            comando.Parameters.Add(
                "@Servico",
                SqlDbType.Int).Value =
                    idServico;
        }

        private void AdicionarQuantidade(
            SqlCommand comando,
            int quantidade)
        {
            comando.Parameters.Add(
                "@Quantidade",
                SqlDbType.Int).Value =
                    quantidade;
        }

        private void AdicionarPreco(
            SqlCommand comando,
            decimal preco)
        {
            SqlParameter parametro =
                comando.Parameters.Add(
                    "@Preco",
                    SqlDbType.Decimal);

            parametro.Precision = 18;
            parametro.Scale = 2;

            parametro.Value = preco;
        }

        private int ExecutarLinhas(
            SqlCommand comando)
        {
            // O SQL fornecido a este método deve
            // terminar com SELECT @@ROWCOUNT.
            return Convert.ToInt32(
                comando.ExecuteScalar());
        }

        // ==========================================
        // TRANSAÇÃO SQL
        // ==========================================

        private void EmTransacao(
            Action<SqlConnection, SqlTransaction> acao)
        {
            using (SqlConnection conexao =
                new SqlConnection(
                    ObterConnectionString()))
            {
                conexao.Open();

                using (SqlTransaction transacao =
                    conexao.BeginTransaction(
                        IsolationLevel.Serializable))
                {
                    try
                    {
                        acao(conexao, transacao);

                        transacao.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transacao.Rollback();
                        }
                        catch
                        {
                            // Mantém o erro original.
                        }

                        throw;
                    }
                }
            }
        }

        // ==========================================
        // IMPEDIR DUPLICIDADE COM TRIGGER ANTIGO
        // ==========================================

        private void VerificarTriggerEstoque(
            SqlConnection conexao,
            SqlTransaction transacao)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM sys.triggers
                WHERE parent_id =
                    OBJECT_ID('dbo.Consulta_Produto')
                  AND name =
                    'trg_Consulta_Produto_Estoque'
                  AND is_disabled = 0;
            ";

            using (SqlCommand comando =
                CriarComando(
                    sql, conexao, transacao))
            {
                int total =
                    Convert.ToInt32(
                        comando.ExecuteScalar());

                if (total > 0)
                {
                    throw new Exception(
                        "O trigger antigo de estoque " +
                        "continua ativo.\n\n" +
                        "Ele pode descontar ou devolver " +
                        "o estoque automaticamente, " +
                        "duplicando a movimentação do C#.\n\n" +
                        "Revise-o antes de continuar.");
                }
            }
        }

        // ==========================================
        // REGISTRAR PRODUTO COM BAIXA
        // ==========================================


        // Exceção de negócio: sai do trabalho em segundo plano
        // e é exibida como aviso na thread principal do WinForms.
        private class EstoqueInsuficienteException : Exception
        {
            public EstoqueInsuficienteException(
                int disponivel,
                int solicitado)
                : base(
                    "Estoque insuficiente!\n\n" +
                    "Disponível: " + disponivel + " unidade(s).\n" +
                    "Solicitado: " + solicitado + " unidade(s).\n\n" +
                    "Reduza a quantidade ou selecione outro produto.")
            {
            }
        }


        private void RegistrarProduto(
            int idProduto,
            int quantidade)
        {

            if (idProduto <= 0 ||
                quantidade <= 0)
            {
                throw new Exception(
                    "Produto ou quantidade inválida.");
            }


            EmTransacao((conexao, transacao) =>
            {
                VerificarTriggerEstoque(
                    conexao, transacao);

                // ==================================
                // BUSCAR E BLOQUEAR ESTOQUE
                // ==================================

                int disponivel;
                decimal preco;

                string sqlEstoque = @"
                    SELECT
                        ep.Quantidade,
                        p.Preco
                    FROM dbo.Estoque_Produto AS ep
                        WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN dbo.Produtos AS p
                        ON p.ID_Produto =
                           ep.fk_Produtos_ID_Produto
                    WHERE p.ID_Produto = @Produto;
                ";

                using (SqlCommand comando =
                    CriarComando(
                        sqlEstoque,
                        conexao,
                        transacao))
                {
                    AdicionarParametroProduto(
                        comando, idProduto);

                    using (SqlDataReader leitor =
                        comando.ExecuteReader())
                    {
                        if (!leitor.Read())
                        {
                            throw new Exception(
                                "Produto não encontrado " +
                                "ou sem registro de estoque.");
                        }

                        disponivel =
                            Convert.ToInt32(
                                leitor["Quantidade"]);

                        if (leitor["Preco"] == DBNull.Value)
                        {
                            throw new Exception(
                                "O produto não possui preço.");
                        }

                        preco =
                            Convert.ToDecimal(
                                leitor["Preco"]);

                        if (leitor.Read())
                        {
                            throw new Exception(
                                "Existem registros duplicados " +
                                "de estoque para este produto.");
                        }
                    }
                }


                if (disponivel < quantidade)
                {
                    throw new EstoqueInsuficienteException(
                        disponivel,
                        quantidade
                    );
                }




                // ==================================
                // CONFERIR LANÇAMENTO EXISTENTE
                // ==================================

                if (preco < 0m)
                    throw new Exception(
                        "O produto possui preço negativo no cadastro.");

                string sqlContagem = @"
                    SELECT COUNT(*)
                    FROM dbo.Consulta_Produto
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE fk_Consultas_ID_Consulta =
                          @Consulta
                      AND fk_Produtos_ID_Produto =
                          @Produto;
                ";

                int existentes;

                using (SqlCommand comando =
                    CriarComando(
                        sqlContagem,
                        conexao,
                        transacao))
                {
                    AdicionarParametroProduto(
                        comando, idProduto);

                    existentes =
                        Convert.ToInt32(
                            comando.ExecuteScalar());
                }

                if (existentes > 1)
                {
                    throw new Exception(
                        "Há lançamentos duplicados " +
                        "deste produto nesta consulta.");
                }

                // ==================================
                // BAIXA ATÔMICA DO ESTOQUE
                // ==================================

                string sqlBaixa = @"
                    UPDATE dbo.Estoque_Produto
                    SET Quantidade =
                        Quantidade - @Quantidade
                    WHERE fk_Produtos_ID_Produto =
                          @Produto
                      AND Quantidade >= @Quantidade;

                    SELECT @@ROWCOUNT;
                ";

                using (SqlCommand comando =
                    CriarComando(
                        sqlBaixa,
                        conexao,
                        transacao))
                {
                    AdicionarParametroProduto(
                        comando, idProduto);

                    AdicionarQuantidade(
                        comando, quantidade);

                    if (ExecutarLinhas(comando) != 1)
                    {
                        throw new Exception(
                            "Não foi possível descontar " +
                            "a quantidade do estoque.");
                    }
                }

                // ==================================
                // REGISTRAR CONSUMO
                // ==================================

                string sqlLancamento;

                if (existentes == 0)
                {
                    sqlLancamento = @"
                        INSERT INTO dbo.Consulta_Produto
                        (
                            fk_Consultas_ID_Consulta,
                            fk_Produtos_ID_Produto,
                            Quantidade,
                            Preco
                        )
                        VALUES
                        (
                            @Consulta,
                            @Produto,
                            @Quantidade,
                            @Preco
                        );

                        SELECT @@ROWCOUNT;
                    ";
                }
                else
                {
                    // Preserva o preço original
                    // de um lançamento existente.
                    sqlLancamento = @"
                        UPDATE dbo.Consulta_Produto
                        SET Quantidade =
                            Quantidade + @Quantidade
                        WHERE fk_Consultas_ID_Consulta =
                              @Consulta
                          AND fk_Produtos_ID_Produto =
                              @Produto;

                        SELECT @@ROWCOUNT;
                    ";
                }

                using (SqlCommand comando =
                    CriarComando(
                        sqlLancamento,
                        conexao,
                        transacao))
                {
                    AdicionarParametroProduto(
                        comando, idProduto);

                    AdicionarQuantidade(
                        comando, quantidade);

                    if (existentes == 0)
                    {
                        AdicionarPreco(
                            comando, preco);
                    }

                    if (ExecutarLinhas(comando) != 1)
                    {
                        throw new Exception(
                            "Falha ao registrar o consumo.");
                    }
                }

                // Atualiza o financeiro na mesma transação.
                FinanceiroSync.Sincronizar(
                    conexao,
                    transacao,
                    idConsulta);
            });
        }

        // ==========================================
        // REMOVER PRODUTO E ESTORNAR QUANTIDADE
        // ==========================================

        private void RemoverProduto(int idProduto)
        {
            if (idProduto <= 0)
                throw new Exception("Produto inválido.");

            EmTransacao((conexao, transacao) =>
            {
                VerificarTriggerEstoque(
                    conexao, transacao);

                // Bloqueia primeiro o registro
                // de estoque para manter uma
                // ordem consistente de bloqueio.
                string sqlBloqueioEstoque = @"
                    SELECT Quantidade
                    FROM dbo.Estoque_Produto
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE fk_Produtos_ID_Produto =
                          @Produto;
                ";

                using (SqlCommand comando =
                    CriarComando(
                        sqlBloqueioEstoque,
                        conexao,
                        transacao))
                {
                    AdicionarParametroProduto(
                        comando, idProduto);

                    using (SqlDataReader leitor =
                        comando.ExecuteReader())
                    {
                        if (!leitor.Read())
                        {
                            throw new Exception(
                                "Registro de estoque " +
                                "não encontrado.");
                        }

                        if (leitor.Read())
                        {
                            throw new Exception(
                                "Há registros de estoque " +
                                "duplicados para o produto.");
                        }
                    }
                }

                // ==================================
                // OBTER QUANTIDADE A ESTORNAR
                // ==================================

                string sqlLancamento = @"
                    SELECT Quantidade
                    FROM dbo.Consulta_Produto
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE fk_Consultas_ID_Consulta =
                          @Consulta
                      AND fk_Produtos_ID_Produto =
                          @Produto;
                ";

                int quantidade;

                using (SqlCommand comando =
                    CriarComando(
                        sqlLancamento,
                        conexao,
                        transacao))
                {
                    AdicionarParametroProduto(
                        comando, idProduto);

                    using (SqlDataReader leitor =
                        comando.ExecuteReader())
                    {
                        if (!leitor.Read())
                        {
                            throw new Exception(
                                "Lançamento não encontrado.");
                        }

                        quantidade =
                            Convert.ToInt32(
                                leitor["Quantidade"]);

                        if (leitor.Read())
                        {
                            throw new Exception(
                                "Há lançamentos duplicados " +
                                "para este produto.");
                        }
                    }
                }

                if (quantidade <= 0)
                    throw new Exception(
                        "Quantidade de lançamento inválida.");

                // ==================================
                // EXCLUIR LANÇAMENTO
                // ==================================

                string sqlExcluir = @"
                    DELETE FROM dbo.Consulta_Produto
                    WHERE fk_Consultas_ID_Consulta =
                          @Consulta
                      AND fk_Produtos_ID_Produto =
                          @Produto;

                    SELECT @@ROWCOUNT;
                ";

                using (SqlCommand comando =
                    CriarComando(
                        sqlExcluir,
                        conexao,
                        transacao))
                {
                    AdicionarParametroProduto(
                        comando, idProduto);

                    if (ExecutarLinhas(comando) != 1)
                    {
                        throw new Exception(
                            "Falha ao remover lançamento.");
                    }
                }

                // ==================================
                // DEVOLVER AO ESTOQUE
                // ==================================

                string sqlEstorno = @"
                    UPDATE dbo.Estoque_Produto
                    SET Quantidade =
                        Quantidade + @Quantidade
                    WHERE fk_Produtos_ID_Produto =
                          @Produto;

                    SELECT @@ROWCOUNT;
                ";

                using (SqlCommand comando =
                    CriarComando(
                        sqlEstorno,
                        conexao,
                        transacao))
                {
                    AdicionarParametroProduto(
                        comando, idProduto);

                    AdicionarQuantidade(
                        comando, quantidade);

                    if (ExecutarLinhas(comando) != 1)
                    {
                        throw new Exception(
                            "Falha ao devolver quantidade.");
                    }
                }

                // Atualiza o financeiro na mesma transação.
                FinanceiroSync.Sincronizar(
                    conexao,
                    transacao,
                    idConsulta);
            });
        }

        // ==========================================
        // REGISTRAR SERVIÇO DA CONSULTA
        // ==========================================

        private void RegistrarServico(
            int idServico,
            int quantidade)
        {
            if (idServico <= 0 ||
                quantidade <= 0)
            {
                throw new Exception(
                    "Serviço ou quantidade inválida.");
            }

            EmTransacao((conexao, transacao) =>
            {
                // ==================================
                // BUSCAR PREÇO REAL NO BANCO
                // ==================================

                string sqlPreco = @"
                    SELECT Preco
                    FROM dbo.Servicos
                        WITH (HOLDLOCK)
                    WHERE ID_Servico = @Servico;
                ";

                decimal preco;

                using (SqlCommand comando =
                    CriarComando(
                        sqlPreco,
                        conexao,
                        transacao))
                {
                    AdicionarParametroServico(
                        comando, idServico);

                    object resultado =
                        comando.ExecuteScalar();

                    if (resultado == null ||
                        resultado == DBNull.Value)
                    {
                        throw new Exception(
                            "Serviço não encontrado " +
                            "ou sem preço cadastrado.");
                    }

                    preco =
                        Convert.ToDecimal(resultado);
                }

                if (preco < 0)
                {
                    throw new Exception(
                        "Preço inválido no cadastro " +
                        "do serviço.");
                }

                // ==================================
                // VERIFICAR LANÇAMENTO EXISTENTE
                // ==================================

                string sqlContagem = @"
                    SELECT COUNT(*)
                    FROM dbo.Consulta_Servico
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE fk_Consultas_ID_Consulta =
                          @Consulta
                      AND fk_Servicos_ID_Servico =
                          @Servico;
                ";

                int existentes;

                using (SqlCommand comando =
                    CriarComando(
                        sqlContagem,
                        conexao,
                        transacao))
                {
                    AdicionarParametroServico(
                        comando, idServico);

                    existentes =
                        Convert.ToInt32(
                            comando.ExecuteScalar());
                }

                if (existentes > 1)
                {
                    throw new Exception(
                        "Existem lançamentos duplicados " +
                        "deste serviço na consulta.");
                }

                // ==================================
                // INSERT OU UPDATE
                // ==================================

                string sql;

                if (existentes == 0)
                {
                    sql = @"
                        INSERT INTO dbo.Consulta_Servico
                        (
                            fk_Servicos_ID_Servico,
                            fk_Consultas_ID_Consulta,
                            Quantidade,
                            Preco
                        )
                        VALUES
                        (
                            @Servico,
                            @Consulta,
                            @Quantidade,
                            @Preco
                        );

                        SELECT @@ROWCOUNT;
                    ";
                }
                else
                {
                    // Mantém o preço unitário
                    // histórico do lançamento.
                    sql = @"
                        UPDATE dbo.Consulta_Servico
                        SET Quantidade =
                            Quantidade + @Quantidade
                        WHERE fk_Consultas_ID_Consulta =
                              @Consulta
                          AND fk_Servicos_ID_Servico =
                              @Servico;

                        SELECT @@ROWCOUNT;
                    ";
                }

                using (SqlCommand comando =
                    CriarComando(
                        sql,
                        conexao,
                        transacao))
                {
                    AdicionarParametroServico(
                        comando, idServico);

                    AdicionarQuantidade(
                        comando, quantidade);

                    if (existentes == 0)
                    {
                        AdicionarPreco(
                            comando, preco);
                    }

                    if (ExecutarLinhas(comando) != 1)
                    {
                        throw new Exception(
                            "Não foi possível registrar " +
                            "o serviço na consulta.");
                    }
                }

                // Atualiza o financeiro na mesma transação.
                FinanceiroSync.Sincronizar(
                    conexao,
                    transacao,
                    idConsulta);
            });
        }

        // ==========================================
        // REMOVER SERVIÇO DA CONSULTA
        // ==========================================

        private void RemoverServico(int idServico)
        {
            if (idServico <= 0)
                throw new Exception("Serviço inválido.");

            EmTransacao((conexao, transacao) =>
            {
                string sqlContagem = @"
                    SELECT COUNT(*)
                    FROM dbo.Consulta_Servico
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE fk_Consultas_ID_Consulta =
                          @Consulta
                      AND fk_Servicos_ID_Servico =
                          @Servico;
                ";

                int existentes;

                using (SqlCommand comando =
                    CriarComando(
                        sqlContagem,
                        conexao,
                        transacao))
                {
                    AdicionarParametroServico(
                        comando, idServico);

                    existentes =
                        Convert.ToInt32(
                            comando.ExecuteScalar());
                }

                if (existentes != 1)
                {
                    throw new Exception(
                        "O serviço não foi encontrado " +
                        "ou possui registros duplicados.");
                }

                string sqlExcluir = @"
                    DELETE FROM dbo.Consulta_Servico
                    WHERE fk_Consultas_ID_Consulta =
                          @Consulta
                      AND fk_Servicos_ID_Servico =
                          @Servico;

                    SELECT @@ROWCOUNT;
                ";

                using (SqlCommand comando =
                    CriarComando(
                        sqlExcluir,
                        conexao,
                        transacao))
                {
                    AdicionarParametroServico(
                        comando, idServico);

                    if (ExecutarLinhas(comando) != 1)
                    {
                        throw new Exception(
                            "Não foi possível remover " +
                            "o serviço da consulta.");
                    }
                }

                // Atualiza o financeiro na mesma transação.
                FinanceiroSync.Sincronizar(
                    conexao,
                    transacao,
                    idConsulta);
            });
        }
    }
}
