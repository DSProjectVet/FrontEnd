
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
    public partial class frmEstoque : Form
    {
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

        private readonly Color borda =
            Color.FromArgb(230, 236, 233);

        // ==========================================
        // DADOS E CONTROLES
        // ==========================================

        private DataTable todosItens = new DataTable();

        private DataGridView gradeEstoqueDashboard;

        private TextBox txtPesquisaEstoque;

        private ComboBox cmbTipoEstoque;
        private ComboBox cmbStatusEstoque;

        private Label lblTotal;
        private Label lblProdutos;
        private Label lblMedicamentos;
        private Label lblBaixo;
        private Label lblRodape;

        private Button btnAtualizarEstoque;

        private bool carregamentoInicial = false;

        public frmEstoque()
        {

            this.Text = "Med Vet - Estoque";
            this.BackColor = fundo;
            this.MinimumSize = new Size(850, 570);

            ConstruirDashboard();
        }

        // ==========================================
        // CONSTRUIR A INTERFACE
        // ==========================================

        private void ConstruirDashboard()
        {
            // Remove controles visuais antigos do Form.
            // O Designer permanece preservado.
            this.Controls.Clear();

            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.BackColor = fundo;
            layout.Padding = new Padding(22, 18, 22, 18);

            layout.ColumnCount = 1;
            layout.RowCount = 5;

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 90F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 115F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 76F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 45F));

            this.Controls.Add(layout);

            // ======================================
            // CABEÇALHO
            // ======================================

            Panel cabecalho = new Panel();

            cabecalho.Dock = DockStyle.Fill;
            cabecalho.Margin = new Padding(0);

            Label titulo = new Label();

            titulo.Text = "Gestão de Estoque";
            titulo.Font = new Font(
                "Segoe UI", 23F, FontStyle.Bold);

            titulo.ForeColor = texto;
            titulo.SetBounds(0, 0, 550, 48);

            cabecalho.Controls.Add(titulo);

            Label subtitulo = new Label();

            subtitulo.Text =
                "Controle de produtos e medicamentos da clínica.";

            subtitulo.Font = new Font("Segoe UI", 10F);
            subtitulo.ForeColor = cinza;

            subtitulo.SetBounds(2, 51, 600, 26);

            cabecalho.Controls.Add(subtitulo);

            btnAtualizarEstoque = new Button();

            btnAtualizarEstoque.Text = "Atualizar estoque";
            btnAtualizarEstoque.Size = new Size(160, 42);

            btnAtualizarEstoque.BackColor = verde;
            btnAtualizarEstoque.ForeColor = Color.White;

            btnAtualizarEstoque.FlatStyle =
                FlatStyle.Flat;

            btnAtualizarEstoque.FlatAppearance.BorderSize = 0;

            btnAtualizarEstoque.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            btnAtualizarEstoque.Cursor = Cursors.Hand;

            btnAtualizarEstoque.Anchor =
                AnchorStyles.Top | AnchorStyles.Right;

            cabecalho.Controls.Add(btnAtualizarEstoque);


            // ==========================================
            // BOTÃO NOVO ITEM
            // ==========================================

            Button btnNovoItem = new Button();

            btnNovoItem.Text = "+ Novo item";
            btnNovoItem.Size = new Size(150, 42);

            btnNovoItem.BackColor = verde;
            btnNovoItem.ForeColor = Color.White;

            btnNovoItem.FlatStyle = FlatStyle.Flat;
            btnNovoItem.FlatAppearance.BorderSize = 0;

            btnNovoItem.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            btnNovoItem.Cursor = Cursors.Hand;

            cabecalho.Controls.Add(btnNovoItem);

            // SUBSTITUIR O EVENTO DE RESIZE ANTERIOR
            // PELO POSICIONAMENTO DOS DOIS BOTÕES.
            cabecalho.Resize += (s, e) =>
            {
                btnAtualizarEstoque.Location = new Point(
                    cabecalho.ClientSize.Width -
                    btnAtualizarEstoque.Width,
                    13);

                btnNovoItem.Location = new Point(
                    btnAtualizarEstoque.Left -
                    btnNovoItem.Width - 12,
                    13);

                titulo.Width = Math.Max(
                    200, btnNovoItem.Left - 15);
            };

            // ==========================================
            // EVENTO DO NOVO CADASTRO
            // ==========================================

            btnNovoItem.Click += async (s, e) =>
            {
                using (frmCadastrarItem tela =
                    new frmCadastrarItem())
                {
                    if (tela.ShowDialog(this) != DialogResult.OK ||
                        tela.ItemCadastrado == null)
                    {
                        return;
                    }

                    ItemEstoqueDetalhe novoItem =
                        tela.ItemCadastrado;

                    btnNovoItem.Enabled = false;

                    try
                    {
                        await Task.Run(() =>
                        {
                            CadastrarItemNoBanco(novoItem);
                        });

                        MessageBox.Show(
                            this,
                            "O item \"" + novoItem.Nome +
                            "\" foi cadastrado com sucesso!",
                            "Cadastro realizado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        await CarregarEstoqueAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            this,
                            "Não foi possível cadastrar o item.\n\n" +
                            ex.Message,
                            "Erro ao cadastrar",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnNovoItem.Enabled = true;
                    }
                }
            };


            cabecalho.Resize += (s, e) =>
            {
                btnAtualizarEstoque.Location = new Point(
                    cabecalho.ClientSize.Width -
                    btnAtualizarEstoque.Width, 13);
            };

            btnAtualizarEstoque.Click += async (s, e) =>
            {
                await CarregarEstoqueAsync();
            };

            layout.Controls.Add(cabecalho, 0, 0);

            // ======================================
            // INDICADORES
            // ======================================

            TableLayoutPanel indicadores =
                new TableLayoutPanel();

            indicadores.Dock = DockStyle.Fill;
            indicadores.ColumnCount = 4;
            indicadores.RowCount = 1;
            indicadores.Margin = new Padding(0, 0, 0, 12);

            for (int i = 0; i < 4; i++)
            {
                indicadores.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent, 25F));
            }

            indicadores.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            indicadores.Controls.Add(
                CriarIndicador(
                    "Total de itens",
                    Color.FromArgb(228, 249, 241),
                    verdeEscuro,
                    out lblTotal),
                0, 0);

            indicadores.Controls.Add(
                CriarIndicador(
                    "Produtos",
                    Color.FromArgb(232, 241, 255),
                    Color.FromArgb(48, 105, 185),
                    out lblProdutos),
                1, 0);

            indicadores.Controls.Add(
                CriarIndicador(
                    "Medicamentos",
                    Color.FromArgb(241, 235, 255),
                    Color.FromArgb(112, 75, 180),
                    out lblMedicamentos),
                2, 0);

            indicadores.Controls.Add(
                CriarIndicador(
                    "Estoque baixo",
                    Color.FromArgb(255, 246, 225),
                    Color.FromArgb(175, 112, 10),
                    out lblBaixo),
                3, 0);

            layout.Controls.Add(indicadores, 0, 1);

            // ======================================
            // PESQUISA E FILTROS
            // ======================================

            FlowLayoutPanel painelFiltros =
                new FlowLayoutPanel();

            painelFiltros.Dock = DockStyle.Fill;

            painelFiltros.FlowDirection =
                FlowDirection.LeftToRight;

            painelFiltros.WrapContents = true;

            painelFiltros.Padding = new Padding(0, 10, 0, 5);
            painelFiltros.Margin = new Padding(0);

            // PESQUISA
            txtPesquisaEstoque = new TextBox();

            txtPesquisaEstoque.Width = 330;
            txtPesquisaEstoque.Font =
                new Font("Segoe UI", 11F);

            txtPesquisaEstoque.Margin =
                new Padding(0, 4, 12, 8);

            painelFiltros.Controls.Add(txtPesquisaEstoque);

            // FILTRO DE TIPO
            cmbTipoEstoque = new ComboBox();

            cmbTipoEstoque.Width = 170;
            cmbTipoEstoque.Font =
                new Font("Segoe UI", 10F);

            cmbTipoEstoque.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbTipoEstoque.Items.AddRange(
                new object[]
                {
                    "Todos os tipos",
                    "Produto",
                    "Medicamento"
                });

            cmbTipoEstoque.SelectedIndex = 0;

            cmbTipoEstoque.Margin =
                new Padding(0, 4, 12, 8);

            painelFiltros.Controls.Add(cmbTipoEstoque);

            // FILTRO DE STATUS
            cmbStatusEstoque = new ComboBox();

            cmbStatusEstoque.Width = 180;
            cmbStatusEstoque.Font =
                new Font("Segoe UI", 10F);

            cmbStatusEstoque.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmbStatusEstoque.Items.AddRange(
                new object[]
                {
                    "Todos os estoques",
                    "Estoque normal",
                    "Estoque baixo",
                    "Sem estoque"
                });

            cmbStatusEstoque.SelectedIndex = 0;

            cmbStatusEstoque.Margin =
                new Padding(0, 4, 10, 8);

            painelFiltros.Controls.Add(cmbStatusEstoque);

            // EVENTOS
            txtPesquisaEstoque.TextChanged +=
                (s, e) => AplicarFiltros();

            cmbTipoEstoque.SelectedIndexChanged +=
                (s, e) => AplicarFiltros();

            cmbStatusEstoque.SelectedIndexChanged +=
                (s, e) => AplicarFiltros();

            layout.Controls.Add(painelFiltros, 0, 2);

            // ======================================
            // TABELA DO ESTOQUE
            // ======================================

            gradeEstoqueDashboard = new DataGridView();

            gradeEstoqueDashboard.Dock = DockStyle.Fill;
            gradeEstoqueDashboard.Margin =
                new Padding(0);

            gradeEstoqueDashboard.BackgroundColor =
                Color.White;

            gradeEstoqueDashboard.BorderStyle =
                BorderStyle.None;

            gradeEstoqueDashboard.EnableHeadersVisualStyles =
                false;

            gradeEstoqueDashboard.AutoGenerateColumns =
                false;

            gradeEstoqueDashboard.ReadOnly = true;

            gradeEstoqueDashboard.AllowUserToAddRows =
                false;

            gradeEstoqueDashboard.AllowUserToDeleteRows =
                false;

            gradeEstoqueDashboard.AllowUserToResizeRows =
                false;

            gradeEstoqueDashboard.RowHeadersVisible = false;

            gradeEstoqueDashboard.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            gradeEstoqueDashboard.MultiSelect = false;

            gradeEstoqueDashboard.RowTemplate.Height = 44;

            gradeEstoqueDashboard.ColumnHeadersHeight = 46;

            gradeEstoqueDashboard.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            gradeEstoqueDashboard.GridColor = borda;

            gradeEstoqueDashboard.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            gradeEstoqueDashboard.ColumnHeadersDefaultCellStyle.BackColor =
                verdeEscuro;

            gradeEstoqueDashboard.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            gradeEstoqueDashboard.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI", 10F, FontStyle.Bold);

            gradeEstoqueDashboard.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(7, 0, 0, 0);

            gradeEstoqueDashboard.DefaultCellStyle.Font =
                new Font("Segoe UI", 10F);

            gradeEstoqueDashboard.DefaultCellStyle.ForeColor =
                texto;

            gradeEstoqueDashboard.DefaultCellStyle.BackColor =
                Color.White;

            gradeEstoqueDashboard.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(225, 248, 240);

            gradeEstoqueDashboard.DefaultCellStyle.SelectionForeColor =
                verdeEscuro;

            gradeEstoqueDashboard.DefaultCellStyle.Padding =
                new Padding(7, 0, 6, 0);

            gradeEstoqueDashboard.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 252, 250);

            AdicionarColuna("ID", "ID", 35);

            AdicionarColuna("Nome", "Nome", 130);

            AdicionarColuna(
                "Descricao", "Descrição", 150);

            AdicionarColuna(
                "Tipo", "Tipo", 85);

            AdicionarColuna(
                "Quantidade", "Qtd.", 55);

            AdicionarColuna(
                "EstoqueMinimo", "Mínimo", 55);

            AdicionarColuna(
                "Preco", "Preço", 75, "C2");

            AdicionarColuna(
                "Status", "Situação", 95);

            gradeEstoqueDashboard.CellFormatting +=
                GradeEstoque_CellFormatting;

            gradeEstoqueDashboard.CellDoubleClick +=
                GradeEstoque_CellDoubleClick;

            layout.Controls.Add(
                gradeEstoqueDashboard, 0, 3);

            // ======================================
            // RODAPÉ
            // ======================================

            Panel rodape = new Panel();

            rodape.Dock = DockStyle.Fill;
            rodape.BackColor = fundo;
            rodape.Margin = new Padding(0);

            lblRodape = new Label();

            lblRodape.Text = "Nenhum item carregado.";

            lblRodape.Font =
                new Font("Segoe UI", 9F);

            lblRodape.ForeColor = cinza;

            lblRodape.Dock = DockStyle.Fill;

            lblRodape.TextAlign =
                ContentAlignment.MiddleLeft;

            rodape.Controls.Add(lblRodape);

            layout.Controls.Add(rodape, 0, 4);
        }

        // ==========================================
        // CRIAR CARD DE INDICADOR
        // ==========================================

        private Panel CriarIndicador(
            string titulo,
            Color fundoCard,
            Color corValor,
            out Label valor)
        {
            Panel card = new Panel();

            card.Dock = DockStyle.Fill;
            card.BackColor = fundoCard;
            card.Margin = new Padding(0, 0, 12, 0);

            Label lblTitulo = new Label();

            lblTitulo.Text = titulo;

            lblTitulo.Font =
                new Font("Segoe UI", 10F);

            lblTitulo.ForeColor = texto;

            lblTitulo.SetBounds(
                16, 16, 250, 25);

            lblTitulo.Anchor =
                AnchorStyles.Left | AnchorStyles.Top;

            card.Controls.Add(lblTitulo);

            valor = new Label();

            valor.Text = "0";

            valor.Font = new Font(
                "Segoe UI", 23F, FontStyle.Bold);

            valor.ForeColor = corValor;

            valor.SetBounds(
                15, 44, 200, 48);

            card.Controls.Add(valor);

            return card;
        }

        // ==========================================
        // ADICIONAR COLUNAS NA TABELA
        // ==========================================

        private void AdicionarColuna(
            string campo,
            string titulo,
            float peso,
            string formato = null)
        {
            DataGridViewTextBoxColumn coluna =
                new DataGridViewTextBoxColumn();

            coluna.Name = campo;
            coluna.DataPropertyName = campo;

            coluna.HeaderText = titulo;
            coluna.FillWeight = peso;

            if (!string.IsNullOrEmpty(formato))
            {
                coluna.DefaultCellStyle.Format = formato;

                coluna.DefaultCellStyle.FormatProvider =
                    new CultureInfo("pt-BR");
            }

            gradeEstoqueDashboard.Columns.Add(coluna);
        }

        // ==========================================
        // RECUPERAR CONEXÃO JÁ CONFIGURADA
        // ==========================================

        private string ObterConnectionString()
        {
            // Procura a connection string criada
            // pelo Visual Studio em Properties.Settings.

            foreach (SettingsProperty propriedade in
                Properties.Settings.Default.Properties)
            {
                SpecialSettingAttribute atributo =
                    propriedade.Attributes[
                        typeof(SpecialSettingAttribute)]
                    as SpecialSettingAttribute;

                if (atributo != null &&
                    atributo.SpecialSetting ==
                    SpecialSetting.ConnectionString)
                {
                    string conexao = Convert.ToString(
                        Properties.Settings.Default[
                            propriedade.Name]);

                    if (!string.IsNullOrWhiteSpace(conexao))
                        return conexao;
                }
            }

            throw new Exception(
                "Nenhuma connection string foi localizada " +
                "em Properties.Settings. Verifique a " +
                "configuração de conexão do DataSet.");
        }

        // ==========================================
        // CONSULTAR ESTOQUE NO SQL SERVER
        // ==========================================

        private DataTable ConsultarEstoque()
        {
            DataTable tabela = new DataTable();

            string sql = @"
                SELECT
                    m.ID_Medicamento AS ID,
                    m.Nome,
                    CAST(m.Descricao AS NVARCHAR(MAX))
                        AS Descricao,
                    CAST('Medicamento' AS NVARCHAR(20))
                        AS Tipo,
                    ISNULL(em.Quantidade, 0)
                        AS Quantidade,
                    ISNULL(em.EstoqueMinimo, 0)
                        AS EstoqueMinimo,
                    CAST(ISNULL(m.Preco, 0)
                        AS DECIMAL(18, 2)) AS Preco

                FROM dbo.Medicamentos AS m

                LEFT JOIN dbo.Estoque_Medicamento AS em
                    ON em.fk_Medicamentos_ID_Medicamento =
                        m.ID_Medicamento

                UNION ALL

                SELECT
                    p.ID_Produto AS ID,
                    p.Nome,
                    CAST(p.Descricao AS NVARCHAR(MAX))
                        AS Descricao,
                    CAST('Produto' AS NVARCHAR(20))
                        AS Tipo,
                    ISNULL(ep.Quantidade, 0)
                        AS Quantidade,
                    ISNULL(ep.EstoqueMinimo, 0)
                        AS EstoqueMinimo,
                    CAST(ISNULL(p.Preco, 0)
                        AS DECIMAL(18, 2)) AS Preco

                FROM dbo.Produtos AS p

                LEFT JOIN dbo.Estoque_Produto AS ep
                    ON ep.fk_Produtos_ID_Produto =
                        p.ID_Produto

                ORDER BY Nome;
            ";

            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            {
                using (SqlCommand comando =
                    new SqlCommand(sql, conexao))
                {
                    comando.CommandTimeout = 30;

                    using (SqlDataAdapter adapter =
                        new SqlDataAdapter(comando))
                    {
                        adapter.Fill(tabela);
                    }
                }
            }

            // Acrescenta status visual sem modificar
            // a estrutura do banco de dados.
            tabela.Columns.Add(
                "Status", typeof(string));

            foreach (DataRow linha in tabela.Rows)
            {
                int quantidade =
                    Convert.ToInt32(linha["Quantidade"]);

                int minimo =
                    Convert.ToInt32(linha["EstoqueMinimo"]);

                linha["Status"] =
                    ObterStatus(quantidade, minimo);
            }

            return tabela;
        }

        // ==========================================
        // CARREGAR DADOS
        // ==========================================

        private async Task CarregarEstoqueAsync()
        {
            btnAtualizarEstoque.Enabled = false;

            btnAtualizarEstoque.Text = "Carregando...";

            lblRodape.Text =
                "Consultando o estoque no banco de dados...";

            try
            {
                DataTable resultado = await Task.Run(
                    () => ConsultarEstoque());

                todosItens = resultado;

                AtualizarIndicadores();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                lblRodape.Text =
                    "Não foi possível atualizar o estoque.";

                MessageBox.Show(
                    this,
                    "Erro ao consultar o estoque:\n\n" +
                    ex.Message +
                    "\n\nVerifique a conexão e os nomes " +
                    "das tabelas e colunas.",
                    "Med Vet - Estoque",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnAtualizarEstoque.Enabled = true;

                btnAtualizarEstoque.Text =
                    "Atualizar estoque";
            }
        }

        // ==========================================
        // IDENTIFICAR STATUS
        // ==========================================

        private string ObterStatus(
            int quantidade,
            int minimo)
        {
            if (quantidade <= 0)
                return "Sem estoque";

            if (quantidade <= minimo)
                return "Estoque baixo";

            return "Normal";
        }

        // ==========================================
        // ATUALIZAR INDICADORES
        // ==========================================

        private void AtualizarIndicadores()
        {
            int produtos = 0;
            int medicamentos = 0;
            int estoqueBaixo = 0;

            foreach (DataRow linha in todosItens.Rows)
            {
                string tipo =
                    Convert.ToString(linha["Tipo"]);

                string status =
                    Convert.ToString(linha["Status"]);

                if (tipo == "Produto")
                    produtos++;

                if (tipo == "Medicamento")
                    medicamentos++;

                if (status == "Estoque baixo")
                    estoqueBaixo++;
            }

            lblTotal.Text =
                todosItens.Rows.Count.ToString();

            lblProdutos.Text =
                produtos.ToString();

            lblMedicamentos.Text =
                medicamentos.ToString();

            lblBaixo.Text =
                estoqueBaixo.ToString();
        }

        // ==========================================
        // FILTRAR DADOS
        // ==========================================

        private void AplicarFiltros()
        {
            if (todosItens == null ||
                gradeEstoqueDashboard == null ||
                txtPesquisaEstoque == null)
                return;

            string pesquisa =
                txtPesquisaEstoque.Text.Trim();

            string tipo =
                cmbTipoEstoque.SelectedItem == null
                ? "Todos os tipos"
                : cmbTipoEstoque.SelectedItem.ToString();

            string status =
                cmbStatusEstoque.SelectedItem == null
                ? "Todos os estoques"
                : cmbStatusEstoque.SelectedItem.ToString();

            DataTable resultado =
                todosItens.Clone();

            foreach (DataRow linha in todosItens.Rows)
            {
                string nome =
                    Convert.ToString(linha["Nome"]);

                string descricao =
                    Convert.ToString(linha["Descricao"]);

                string id =
                    Convert.ToString(linha["ID"]);

                string tipoItem =
                    Convert.ToString(linha["Tipo"]);

                string statusItem =
                    Convert.ToString(linha["Status"]);

                // PESQUISA
                bool correspondePesquisa =
                    pesquisa.Length == 0 ||
                    nome.IndexOf(
                        pesquisa,
                        StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                    descricao.IndexOf(
                        pesquisa,
                        StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                    id.IndexOf(
                        pesquisa,
                        StringComparison.CurrentCultureIgnoreCase) >= 0;

                // TIPO
                bool correspondeTipo =
                    tipo == "Todos os tipos" ||
                    tipoItem == tipo;

                // STATUS
                bool correspondeStatus =
                    status == "Todos os estoques" ||
                    statusItem == status;

                if (correspondePesquisa &&
                    correspondeTipo &&
                    correspondeStatus)
                {
                    resultado.ImportRow(linha);
                }
            }

            gradeEstoqueDashboard.DataSource = resultado;

            lblRodape.Text =
                "Exibindo " +
                resultado.Rows.Count +
                " de " +
                todosItens.Rows.Count +
                " itens cadastrados.";
        }

        // ==========================================
        // ESTILIZAR STATUS NA TABELA
        // ==========================================

        private void GradeEstoque_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0)
                return;

            if (gradeEstoqueDashboard.Columns[
                e.ColumnIndex].Name != "Status")
                return;

            string status =
                Convert.ToString(e.Value);

            if (status == "Normal")
            {
                e.CellStyle.ForeColor = verdeEscuro;
            }
            else if (status == "Estoque baixo")
            {
                e.CellStyle.ForeColor =
                    Color.FromArgb(191, 126, 11);
            }
            else if (status == "Sem estoque")
            {
                e.CellStyle.ForeColor =
                    Color.FromArgb(210, 55, 66);
            }
        }

        // ==========================================
        // VISUALIZAR ITEM COM DUPLO CLIQUE
        // ==========================================



        
private async void GradeEstoque_CellDoubleClick(
    object sender,
    DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0)
        return;

    DataRowView dados =
        gradeEstoqueDashboard.Rows[e.RowIndex]
            .DataBoundItem as DataRowView;

    if (dados == null)
        return;

    try
    {
        // ======================================
        // IDENTIFICAR ITEM
        // ======================================

        ItemEstoqueDetalhe item =
            new ItemEstoqueDetalhe
            {
                Id = Convert.ToInt32(dados["ID"]),

                Nome = Convert.ToString(
                    dados["Nome"]),

                Descricao = Convert.ToString(
                    dados["Descricao"]),

                Tipo = Convert.ToString(
                    dados["Tipo"]),

                Preco = Convert.ToDecimal(
                    dados["Preco"]),

                Quantidade = Convert.ToInt32(
                    dados["Quantidade"]),

                EstoqueMinimo = Convert.ToInt32(
                    dados["EstoqueMinimo"])
            };

        // ======================================
        // ABRIR DETALHES
        // ======================================

        using (frmDetalhesItem tela =
            new frmDetalhesItem(item))
        {
            DialogResult resultado =
                tela.ShowDialog(this);

            // ==================================
            // EXCLUIR ITEM
            // ==================================

            if (resultado == DialogResult.Yes &&
                tela.ExclusaoSolicitada)
            {
                gradeEstoqueDashboard.Enabled = false;

                try
                {
                    await Task.Run(() =>
                    {
                        ExcluirItemNoBanco(item);
                    });

                    MessageBox.Show(
                        this,
                        "O item \"" + item.Nome +
                        "\" foi excluído com sucesso!",
                        "Exclusão realizada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await CarregarEstoqueAsync();
                }
                catch (SqlException ex)
                {
                    string mensagem =
                        ex.Number == 547
                        ? "Este item possui registros " +
                          "vinculados no banco de dados " +
                          "e não pode ser excluído " +
                          "sem revisar esses vínculos."
                        : ex.Message;

                    MessageBox.Show(
                        this,
                        mensagem,
                        "Erro ao excluir",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this,
                        "Erro ao excluir item:\n\n" +
                        ex.Message,
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    gradeEstoqueDashboard.Enabled = true;
                }

                return;
            }

            // ==================================
            // EDITAR ITEM
            // ==================================

            if (resultado == DialogResult.OK &&
                tela.ItemEditado != null)
            {
                ItemEstoqueDetalhe atualizado =
                    tela.ItemEditado;

                gradeEstoqueDashboard.Enabled = false;

                try
                {
                    await Task.Run(() =>
                    {
                        SalvarEdicaoNoBanco(atualizado);
                    });

                    MessageBox.Show(
                        this,
                        "Alterações salvas com sucesso!",
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await CarregarEstoqueAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this,
                        "Erro ao salvar alterações:\n\n" +
                        ex.Message,
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    gradeEstoqueDashboard.Enabled = true;
                }
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            this,
            "Erro ao abrir o item:\n\n" +
            ex.Message,
            "Erro",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}




        // ==========================================
        // CARREGAR AO EXIBIR A TELA
        // ==========================================

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (carregamentoInicial)
                return;

            carregamentoInicial = true;

            await CarregarEstoqueAsync();
        }

        // Compatibilidade com evento do Designer.
        private void frmEstoque_Load(
            object sender,
            EventArgs e)
        {
        }



        private void SalvarEdicaoNoBanco(
            ItemEstoqueDetalhe item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.Id <= 0)
                throw new Exception("ID do item inválido.");

            if (string.IsNullOrWhiteSpace(item.Nome))
                throw new Exception("Informe o nome do item.");

            if (item.Quantidade < 0 ||
                item.EstoqueMinimo < 0 ||
                item.Preco < 0)
            {
                throw new Exception(
                    "Preço, quantidade e estoque mínimo " +
                    "não podem ser negativos.");
            }

            // ==========================================
            // IDENTIFICAR O TIPO DE ITEM
            // ==========================================

            string sqlCadastro;
            string sqlEstoque;

            if (string.Equals(
                item.Tipo,
                "Medicamento",
                StringComparison.OrdinalIgnoreCase))
            {
                // ATUALIZAR DADOS DO MEDICAMENTO
                sqlCadastro = @"
            UPDATE dbo.Medicamentos
            SET
                Nome = @Nome,
                Descricao = @Descricao,
                Preco = @Preco
            WHERE ID_Medicamento = @ID;
        ";

                // ATUALIZAR ESTOQUE DO MEDICAMENTO
                // Se não existir registro de estoque,
                // cria automaticamente.
                sqlEstoque = @"
            UPDATE dbo.Estoque_Medicamento
            SET
                Quantidade = @Quantidade,
                EstoqueMinimo = @EstoqueMinimo
            WHERE fk_Medicamentos_ID_Medicamento = @ID;

            IF @@ROWCOUNT = 0
            BEGIN
                INSERT INTO dbo.Estoque_Medicamento
                (
                    fk_Medicamentos_ID_Medicamento,
                    Quantidade,
                    EstoqueMinimo
                )
                VALUES
                (
                    @ID,
                    @Quantidade,
                    @EstoqueMinimo
                );
            END;
        ";
            }
            else if (string.Equals(
                item.Tipo,
                "Produto",
                StringComparison.OrdinalIgnoreCase))
            {
                // ATUALIZAR DADOS DO PRODUTO
                sqlCadastro = @"
            UPDATE dbo.Produtos
            SET
                Nome = @Nome,
                Descricao = @Descricao,
                Preco = @Preco
            WHERE ID_Produto = @ID;
        ";

                // ATUALIZAR ESTOQUE DO PRODUTO
                sqlEstoque = @"
            UPDATE dbo.Estoque_Produto
            SET
                Quantidade = @Quantidade,
                EstoqueMinimo = @EstoqueMinimo
            WHERE fk_Produtos_ID_Produto = @ID;

            IF @@ROWCOUNT = 0
            BEGIN
                INSERT INTO dbo.Estoque_Produto
                (
                    fk_Produtos_ID_Produto,
                    Quantidade,
                    EstoqueMinimo
                )
                VALUES
                (
                    @ID,
                    @Quantidade,
                    @EstoqueMinimo
                );
            END;
        ";
            }
            else
            {
                throw new Exception(
                    "Tipo de item não reconhecido: " +
                    item.Tipo);
            }

            // ==========================================
            // ABRIR CONEXÃO SQL
            // ==========================================

            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            {
                conexao.Open();

                // Transação para garantir que as duas
                // tabelas sejam atualizadas juntas.
                using (SqlTransaction transacao =
                    conexao.BeginTransaction(
                        IsolationLevel.Serializable))
                {
                    try
                    {
                        // ==================================
                        // ATUALIZAR DADOS DO ITEM
                        // ==================================

                        using (SqlCommand comando =
                            new SqlCommand(
                                sqlCadastro,
                                conexao,
                                transacao))
                        {
                            comando.CommandTimeout = 30;

                            comando.Parameters.Add(
                                "@ID",
                                SqlDbType.Int).Value = item.Id;

                            comando.Parameters.Add(
                                "@Nome",
                                SqlDbType.NVarChar,
                                4000).Value = item.Nome.Trim();

                            comando.Parameters.Add(
                                "@Descricao",
                                SqlDbType.NVarChar,
                                -1).Value =
                                    item.Descricao ?? "";

                            SqlParameter parametroPreco =
                                comando.Parameters.Add(
                                    "@Preco",
                                    SqlDbType.Decimal);

                            parametroPreco.Precision = 18;
                            parametroPreco.Scale = 2;
                            parametroPreco.Value = item.Preco;

                            int linhasAtualizadas =
                                comando.ExecuteNonQuery();

                            if (linhasAtualizadas != 1)
                            {
                                throw new Exception(
                                    "O item não foi encontrado " +
                                    "para atualização no banco.");
                            }
                        }

                        // ==================================
                        // ATUALIZAR ESTOQUE
                        // ==================================

                        using (SqlCommand comandoEstoque =
                            new SqlCommand(
                                sqlEstoque,
                                conexao,
                                transacao))
                        {
                            comandoEstoque.CommandTimeout = 30;

                            comandoEstoque.Parameters.Add(
                                "@ID",
                                SqlDbType.Int).Value = item.Id;

                            comandoEstoque.Parameters.Add(
                                "@Quantidade",
                                SqlDbType.Int).Value =
                                    item.Quantidade;

                            comandoEstoque.Parameters.Add(
                                "@EstoqueMinimo",
                                SqlDbType.Int).Value =
                                    item.EstoqueMinimo;

                            comandoEstoque.ExecuteNonQuery();
                        }

                        // ==================================
                        // CONFIRMAR ALTERAÇÕES
                        // ==================================

                        transacao.Commit();
                    }
                    catch
                    {
                        // ==================================
                        // DESFAZER ALTERAÇÕES EM CASO DE ERRO
                        // ==================================

                        try
                        {
                            transacao.Rollback();
                        }
                        catch
                        {
                            // A conexão pode ter sido encerrada.
                            // Preserva o erro original.
                        }

                        throw;
                    }
                }
            }
        }


        private void CadastrarItemNoBanco(
            ItemEstoqueDetalhe item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrWhiteSpace(item.Nome))
                throw new Exception("Nome do item obrigatório.");

            if (item.Preco <= 0 ||
                item.Quantidade < 0 ||
                item.EstoqueMinimo < 0)
            {
                throw new Exception(
                    "Os valores informados são inválidos.");
            }

            string tabelaCadastro;
            string tabelaEstoque;
            string colunaID;
            string colunaFK;

            // ==========================================
            // IDENTIFICAR TIPO
            // ==========================================

            if (string.Equals(
                item.Tipo,
                "Medicamento",
                StringComparison.OrdinalIgnoreCase))
            {
                tabelaCadastro = "Medicamentos";
                tabelaEstoque = "Estoque_Medicamento";

                colunaID = "ID_Medicamento";

                colunaFK =
                    "fk_Medicamentos_ID_Medicamento";
            }
            else if (string.Equals(
                item.Tipo,
                "Produto",
                StringComparison.OrdinalIgnoreCase))
            {
                tabelaCadastro = "Produtos";
                tabelaEstoque = "Estoque_Produto";

                colunaID = "ID_Produto";

                colunaFK =
                    "fk_Produtos_ID_Produto";
            }
            else
            {
                throw new Exception(
                    "Tipo de item não reconhecido.");
            }

            // Identificadores SQL são escolhidos
            // exclusivamente entre valores fixos do código.

            string sqlCadastro = $@"
        INSERT INTO dbo.{tabelaCadastro}
        (
            Nome,
            Descricao,
            Preco
        )
        VALUES
        (
            @Nome,
            @Descricao,
            @Preco
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

            string sqlEstoque = $@"
        INSERT INTO dbo.{tabelaEstoque}
        (
            {colunaFK},
            Quantidade,
            EstoqueMinimo
        )
        VALUES
        (
            @ID,
            @Quantidade,
            @EstoqueMinimo
        );
    ";

            // ==========================================
            // TRANSAÇÃO SQL
            // ==========================================

            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            {
                conexao.Open();

                using (SqlTransaction transacao =
                    conexao.BeginTransaction())
                {
                    try
                    {
                        int idGerado;

                        // CADASTRAR ITEM PRINCIPAL
                        using (SqlCommand comando =
                            new SqlCommand(
                                sqlCadastro,
                                conexao,
                                transacao))
                        {
                            comando.Parameters.Add(
                                "@Nome",
                                SqlDbType.NVarChar,
                                255).Value = item.Nome;

                            comando.Parameters.Add(
                                "@Descricao",
                                SqlDbType.NVarChar,
                                -1).Value =
                                    item.Descricao ?? "";

                            SqlParameter preco =
                                comando.Parameters.Add(
                                    "@Preco",
                                    SqlDbType.Decimal);

                            preco.Precision = 18;
                            preco.Scale = 2;
                            preco.Value = item.Preco;

                            object resultado =
                                comando.ExecuteScalar();

                            if (resultado == null ||
                                resultado == DBNull.Value)
                            {
                                throw new Exception(
                                    "O banco não retornou " +
                                    "o ID do item cadastrado.");
                            }

                            idGerado =
                                Convert.ToInt32(resultado);

                            if (idGerado <= 0)
                            {
                                throw new Exception(
                                    "ID gerado inválido.");
                            }
                        }

                        // CADASTRAR ESTOQUE
                        using (SqlCommand comando =
                            new SqlCommand(
                                sqlEstoque,
                                conexao,
                                transacao))
                        {
                            comando.Parameters.Add(
                                "@ID",
                                SqlDbType.Int).Value =
                                    idGerado;

                            comando.Parameters.Add(
                                "@Quantidade",
                                SqlDbType.Int).Value =
                                    item.Quantidade;

                            comando.Parameters.Add(
                                "@EstoqueMinimo",
                                SqlDbType.Int).Value =
                                    item.EstoqueMinimo;

                            int linhas =
                                comando.ExecuteNonQuery();

                            if (linhas != 1)
                            {
                                throw new Exception(
                                    "Não foi possível criar " +
                                    "o registro de estoque.");
                            }
                        }

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


        private void ExcluirItemNoBanco(
            ItemEstoqueDetalhe item)
        {
            if (item == null || item.Id <= 0)
                throw new Exception("Item inválido.");

            string tabelaCadastro;
            string tabelaEstoque;
            string colunaID;
            string colunaFK;

            // ==========================================
            // IDENTIFICAR TIPO
            // ==========================================

            if (string.Equals(
                item.Tipo,
                "Medicamento",
                StringComparison.OrdinalIgnoreCase))
            {
                tabelaCadastro = "Medicamentos";
                tabelaEstoque = "Estoque_Medicamento";

                colunaID = "ID_Medicamento";

                colunaFK =
                    "fk_Medicamentos_ID_Medicamento";
            }
            else if (string.Equals(
                item.Tipo,
                "Produto",
                StringComparison.OrdinalIgnoreCase))
            {
                tabelaCadastro = "Produtos";
                tabelaEstoque = "Estoque_Produto";

                colunaID = "ID_Produto";

                colunaFK =
                    "fk_Produtos_ID_Produto";
            }
            else
            {
                throw new Exception(
                    "Tipo de item não reconhecido.");
            }

            string sqlEstoque = $@"
        DELETE FROM dbo.{tabelaEstoque}
        WHERE {colunaFK} = @ID;
    ";

            string sqlCadastro = $@"
        DELETE FROM dbo.{tabelaCadastro}
        WHERE {colunaID} = @ID;
    ";

            // ==========================================
            // EXCLUSÃO TRANSACIONAL
            // ==========================================

            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            {
                conexao.Open();

                using (SqlTransaction transacao =
                    conexao.BeginTransaction())
                {
                    try
                    {
                        // REMOVER REGISTRO DE ESTOQUE
                        using (SqlCommand comando =
                            new SqlCommand(
                                sqlEstoque,
                                conexao,
                                transacao))
                        {
                            comando.Parameters.Add(
                                "@ID",
                                SqlDbType.Int).Value =
                                    item.Id;

                            comando.ExecuteNonQuery();
                        }

                        // REMOVER PRODUTO OU MEDICAMENTO
                        using (SqlCommand comando =
                            new SqlCommand(
                                sqlCadastro,
                                conexao,
                                transacao))
                        {
                            comando.Parameters.Add(
                                "@ID",
                                SqlDbType.Int).Value =
                                    item.Id;

                            int excluidos =
                                comando.ExecuteNonQuery();

                            if (excluidos != 1)
                            {
                                throw new Exception(
                                    "O item não foi encontrado " +
                                    "ou já foi excluído.");
                            }
                        }

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
                            // Preserva o erro original.
                        }

                        throw;
                    }
                }
            }
        }






    }
}
