
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{
    public class frmRelatorios : Form
    {
        // ==========================================
        // IDENTIDADE VISUAL MED VET
        // ==========================================

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

        private readonly CultureInfo cultura =
            new CultureInfo("pt-BR");

        // ==========================================
        // TIPOS DE RELATÓRIOS
        // ==========================================

        private enum TipoRelatorio
        {
            Consultas,
            Servicos,
            Prescricoes,
            ConsumoProdutos,
            EstoqueProdutos,
            EstoqueMedicamentos,
            ResumoFinanceiro,
            HistoricoFinanceiro
        }

        private class OpcaoRelatorio
        {
            public TipoRelatorio Tipo;
            public string Nome;
            public string Descricao;

            public OpcaoRelatorio(
                TipoRelatorio tipo,
                string nome,
                string descricao)
            {
                Tipo = tipo;
                Nome = nome;
                Descricao = descricao;
            }

            public override string ToString()
            {
                return Nome;
            }
        }

        // ==========================================
        // CONTROLES
        // ==========================================

        private ComboBox cmbRelatorios;

        private DateTimePicker dtInicio;
        private DateTimePicker dtFim;

        private TextBox txtPesquisa;

        private DataGridView gridRelatorio;

        private Button btnGerar;
        private Button btnExportar;

        private Label lblDescricao;
        private Label lblStatus;

        private readonly Label[] lblTitulos =
            new Label[4];

        private readonly Label[] lblValores =
            new Label[4];

        private DataTable dadosAtuais =
            new DataTable();

        private TipoRelatorio? relatorioCarregado;

        private bool carregando = false;

        // ==========================================
        // CONSTRUTOR
        // ==========================================

        public frmRelatorios()
        {
            Text = "Med Vet - Relatórios";

            BackColor = fundo;

            Font = new Font("Segoe UI", 10F);

            MinimumSize = new Size(920, 625);

            ConstruirInterface();
            CarregarOpcoes();

            Shown += async (s, e) =>
            {
                if (!UsuarioPodeAbrirTela())
                {
                    MessageBox.Show(
                        this,
                        "Seu nível de acesso não permite " +
                        "abrir os relatórios.",
                        "Acesso restrito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    Close();
                    return;
                }

                await GerarRelatorioAsync();
            };
        }

        // ==========================================
        // CONTROLE DE ACESSO
        // ==========================================

        private int NivelAtual()
        {
            return DSProjectVet.Properties.Settings
                .Default.NivelUsuarioLogado;
        }

        private bool UsuarioPodeAbrirTela()
        {
            int nivel = NivelAtual();

            return nivel == 1 ||
                   nivel == 2 ||
                   nivel == 3;
        }

        private bool PodeVerRelatorio(
            TipoRelatorio tipo)
        {
            int nivel = NivelAtual();

            // ADMINISTRADOR: TODOS
            if (nivel == 1)
                return true;

            // ATENDENTE: RELATÓRIOS OPERACIONAIS
            if (nivel == 2)
            {
                return tipo == TipoRelatorio.Consultas ||
                       tipo == TipoRelatorio.Servicos;
            }

            // VETERINÁRIO: RELATÓRIOS CLÍNICOS
            if (nivel == 3)
            {
                return tipo == TipoRelatorio.Consultas ||
                       tipo == TipoRelatorio.Servicos ||
                       tipo == TipoRelatorio.Prescricoes;
            }

            return false;
        }

        // ==========================================
        // CONSTRUIR INTERFACE
        // ==========================================

        private void ConstruirInterface()
        {
            TableLayoutPanel principal =
                new TableLayoutPanel();

            principal.Dock = DockStyle.Fill;

            principal.Padding =
                new Padding(22, 16, 22, 14);

            principal.ColumnCount = 1;
            principal.RowCount = 5;

            principal.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            principal.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 85F));

            principal.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 96F));

            principal.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 112F));

            principal.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            principal.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 65F));

            Controls.Add(principal);

            // ======================================
            // CABEÇALHO
            // ======================================

            Panel cabecalho = new Panel();

            cabecalho.Dock = DockStyle.Fill;

            principal.Controls.Add(
                cabecalho, 0, 0);

            Label titulo = CriarLabel(
                "Central de relatórios", 22, true);

            titulo.SetBounds(
                0, 0, 650, 45);

            cabecalho.Controls.Add(titulo);

            Label subtitulo = CriarLabel(
                "Análise operacional, clínica e financeira.",
                10, false);

            subtitulo.ForeColor = cinza;

            subtitulo.SetBounds(
                2, 47, 690, 26);

            cabecalho.Controls.Add(subtitulo);

            // ======================================
            // FILTROS
            // ======================================

            FlowLayoutPanel filtros =
                new FlowLayoutPanel();

            filtros.Dock = DockStyle.Fill;

            filtros.WrapContents = false;

            filtros.AutoScroll = true;

            filtros.FlowDirection =
                FlowDirection.LeftToRight;

            filtros.BackColor = Color.White;

            filtros.Padding =
                new Padding(12, 4, 8, 4);

            principal.Controls.Add(
                filtros, 0, 1);

            cmbRelatorios = new ComboBox();

            cmbRelatorios.DropDownStyle =
                ComboBoxStyle.DropDownList;

            filtros.Controls.Add(
                CriarGrupoFiltro(
                    "Tipo de relatório",
                    cmbRelatorios,
                    245));

            dtInicio = new DateTimePicker();

            dtInicio.Format =
                DateTimePickerFormat.Short;

            dtInicio.Value =
                DateTime.Today.AddDays(-29);

            filtros.Controls.Add(
                CriarGrupoFiltro(
                    "Data inicial",
                    dtInicio,
                    132));

            dtFim = new DateTimePicker();

            dtFim.Format =
                DateTimePickerFormat.Short;

            dtFim.Value = DateTime.Today;

            filtros.Controls.Add(
                CriarGrupoFiltro(
                    "Data final",
                    dtFim,
                    132));

            txtPesquisa = new TextBox();

            filtros.Controls.Add(
                CriarGrupoFiltro(
                    "Pesquisar resultados",
                    txtPesquisa,
                    175));

            btnGerar = CriarBotao(
                "Gerar relatório",
                verde,
                Color.White,
                145);

            btnGerar.Margin =
                new Padding(4, 24, 0, 0);

            filtros.Controls.Add(btnGerar);

            btnGerar.Click += async (s, e) =>
            {
                await GerarRelatorioAsync();
            };

            cmbRelatorios.SelectedIndexChanged +=
                (s, e) =>
                {
                    OpcaoRelatorio opcao =
                        cmbRelatorios.SelectedItem
                        as OpcaoRelatorio;

                    if (opcao != null)
                    {
                        lblDescricao.Text =
                            opcao.Descricao;
                    }

                    // Evita mostrar dados de um relatório
                    // anterior após trocar de opção.
                    relatorioCarregado = null;
                    dadosAtuais = new DataTable();

                    gridRelatorio.DataSource = null;
                    btnExportar.Enabled = false;

                    LimparIndicadores();
                };

            txtPesquisa.TextChanged += (s, e) =>
            {
                AplicarPesquisa();
            };

            // ======================================
            // INDICADORES
            // ======================================

            TableLayoutPanel indicadores =
                new TableLayoutPanel();

            indicadores.Dock = DockStyle.Fill;
            indicadores.ColumnCount = 4;

            for (int i = 0; i < 4; i++)
            {
                indicadores.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent, 25F));
            }

            for (int i = 0; i < 4; i++)
            {
                indicadores.Controls.Add(
                    CriarIndicador(i), i, 0);
            }

            principal.Controls.Add(
                indicadores, 0, 2);

            // ======================================
            // RESULTADOS
            // ======================================

            TableLayoutPanel painelResultados =
                new TableLayoutPanel();

            painelResultados.Dock = DockStyle.Fill;

            painelResultados.ColumnCount = 1;
            painelResultados.RowCount = 2;

            painelResultados.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 67F));

            painelResultados.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            painelResultados.BackColor = Color.White;

            principal.Controls.Add(
                painelResultados, 0, 3);

            Panel topoResultados = new Panel();

            topoResultados.Dock = DockStyle.Fill;
            topoResultados.Padding =
                new Padding(12, 6, 12, 0);

            painelResultados.Controls.Add(
                topoResultados, 0, 0);

            Label tituloResultados = CriarLabel(
                "Resultados do relatório", 14, true);

            tituloResultados.SetBounds(
                10, 3, 600, 32);

            topoResultados.Controls.Add(
                tituloResultados);

            lblDescricao = CriarLabel(
                "Selecione um relatório.",
                9, false);

            lblDescricao.ForeColor = cinza;

            lblDescricao.SetBounds(
                12, 34, 760, 26);

            topoResultados.Controls.Add(
                lblDescricao);

            Panel painelGrade = new Panel();

            painelGrade.Dock = DockStyle.Fill;

            painelGrade.Padding =
                new Padding(12, 0, 12, 12);

            painelResultados.Controls.Add(
                painelGrade, 0, 1);

            gridRelatorio = CriarGrade();

            painelGrade.Controls.Add(gridRelatorio);

            // ======================================
            // RODAPÉ
            // ======================================

            TableLayoutPanel rodape =
                new TableLayoutPanel();

            rodape.Dock = DockStyle.Fill;
            rodape.ColumnCount = 2;

            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute, 180F));

            principal.Controls.Add(
                rodape, 0, 4);

            lblStatus = CriarLabel(
                "Selecione um relatório para começar.",
                9, false);

            lblStatus.Dock = DockStyle.Fill;
            lblStatus.ForeColor = cinza;
            lblStatus.AutoEllipsis = true;

            rodape.Controls.Add(
                lblStatus, 0, 0);

            btnExportar = CriarBotao(
                "Exportar CSV",
                verdeEscuro,
                Color.White,
                165);

            btnExportar.Dock = DockStyle.Fill;
            btnExportar.Enabled = false;

            btnExportar.Margin =
                new Padding(5, 10, 0, 5);

            rodape.Controls.Add(
                btnExportar, 1, 0);

            btnExportar.Click += (s, e) =>
            {
                ExportarCsv();
            };

            LimparIndicadores();
        }

        // ==========================================
        // OPÇÕES DISPONÍVEIS POR NÍVEL
        // ==========================================

        private void CarregarOpcoes()
        {
            List<OpcaoRelatorio> opcoes =
                new List<OpcaoRelatorio>
                {
                    new OpcaoRelatorio(
                        TipoRelatorio.Consultas,
                        "Consultas por período",
                        "Quantidade de consultas por dia."),

                    new OpcaoRelatorio(
                        TipoRelatorio.Servicos,
                        "Serviços realizados",
                        "Ranking de serviços e valores lançados."),

                    new OpcaoRelatorio(
                        TipoRelatorio.Prescricoes,
                        "Medicamentos prescritos",
                        "Quantidade prescrita por medicamento."),

                    new OpcaoRelatorio(
                        TipoRelatorio.ConsumoProdutos,
                        "Consumo de produtos",
                        "Produtos utilizados nas consultas."),

                    new OpcaoRelatorio(
                        TipoRelatorio.EstoqueProdutos,
                        "Estoque crítico de produtos",
                        "Produtos abaixo ou no estoque mínimo."),

                    new OpcaoRelatorio(
                        TipoRelatorio.EstoqueMedicamentos,
                        "Estoque crítico de medicamentos",
                        "Medicamentos abaixo ou no estoque mínimo."),

                    new OpcaoRelatorio(
                        TipoRelatorio.ResumoFinanceiro,
                        "Resumo financeiro",
                        "Totais de entradas e saídas registradas."),

                    new OpcaoRelatorio(
                        TipoRelatorio.HistoricoFinanceiro,
                        "Histórico financeiro",
                        "Movimentações financeiras do período.")
                };

            cmbRelatorios.Items.Clear();

            foreach (OpcaoRelatorio opcao in opcoes)
            {
                if (PodeVerRelatorio(opcao.Tipo))
                {
                    cmbRelatorios.Items.Add(opcao);
                }
            }

            if (cmbRelatorios.Items.Count > 0)
                cmbRelatorios.SelectedIndex = 0;
        }

        // ==========================================
        // COMPONENTES VISUAIS
        // ==========================================

        private Label CriarLabel(
            string conteudo,
            float tamanho,
            bool negrito)
        {
            return new Label
            {
                Text = conteudo,

                AutoSize = false,

                ForeColor = texto,

                Font = new Font(
                    "Segoe UI",
                    tamanho,
                    negrito
                        ? FontStyle.Bold
                        : FontStyle.Regular),

                TextAlign =
                    ContentAlignment.MiddleLeft
            };
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

            botao.BackColor = corFundo;
            botao.ForeColor = corTexto;

            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = 0;

            botao.UseVisualStyleBackColor = false;

            botao.Cursor = Cursors.Hand;

            botao.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            return botao;
        }

        private Control CriarGrupoFiltro(
            string titulo,
            Control campo,
            int largura)
        {
            Panel grupo = new Panel();

            grupo.Size =
                new Size(largura, 76);

            grupo.Margin =
                new Padding(0, 0, 10, 0);

            Label label = CriarLabel(
                titulo, 9, true);

            label.SetBounds(
                0, 3, largura - 4, 24);

            grupo.Controls.Add(label);

            campo.SetBounds(
                0, 33, largura - 5, 30);

            campo.Font =
                new Font("Segoe UI", 9F);

            grupo.Controls.Add(campo);

            return grupo;
        }

        private Panel CriarIndicador(int indice)
        {
            Panel cartao = new Panel();

            cartao.Dock = DockStyle.Fill;
            cartao.BackColor = Color.White;

            cartao.Margin =
                new Padding(0, 0, 12, 12);

            cartao.Padding =
                new Padding(15, 10, 8, 8);

            TableLayoutPanel conteudo =
                new TableLayoutPanel();

            conteudo.Dock = DockStyle.Fill;
            conteudo.ColumnCount = 1;
            conteudo.RowCount = 2;

            conteudo.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 29F));

            conteudo.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            cartao.Controls.Add(conteudo);

            lblTitulos[indice] =
                CriarLabel("Indicador", 9, false);

            lblTitulos[indice].Dock = DockStyle.Fill;
            lblTitulos[indice].ForeColor = cinza;

            conteudo.Controls.Add(
                lblTitulos[indice], 0, 0);

            lblValores[indice] =
                CriarLabel("—", 17, true);

            lblValores[indice].Dock = DockStyle.Fill;
            lblValores[indice].ForeColor = verdeEscuro;
            lblValores[indice].AutoEllipsis = true;

            conteudo.Controls.Add(
                lblValores[indice], 0, 1);

            return cartao;
        }

        private DataGridView CriarGrade()
        {
            DataGridView grade = new DataGridView();

            grade.Dock = DockStyle.Fill;

            grade.BackgroundColor = Color.White;
            grade.BorderStyle = BorderStyle.None;

            grade.ReadOnly = true;
            grade.AllowUserToAddRows = false;
            grade.AllowUserToDeleteRows = false;
            grade.AllowUserToResizeRows = false;

            grade.RowHeadersVisible = false;
            grade.MultiSelect = false;

            grade.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            grade.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode
                    .DisplayedCells;

            grade.RowTemplate.Height = 39;
            grade.ColumnHeadersHeight = 43;

            grade.EnableHeadersVisualStyles = false;

            grade.ColumnHeadersDefaultCellStyle.BackColor =
                verdeEscuro;

            grade.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            grade.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI", 9F, FontStyle.Bold);

            grade.DefaultCellStyle.BackColor =
                Color.White;

            grade.DefaultCellStyle.ForeColor = texto;

            grade.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            grade.DefaultCellStyle.SelectionBackColor =
                verdeClaro;

            grade.DefaultCellStyle.SelectionForeColor =
                verdeEscuro;

            grade.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 252, 250);

            grade.AlternatingRowsDefaultCellStyle.ForeColor =
                texto;

            grade.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            grade.GridColor =
                Color.FromArgb(233, 239, 235);

            return grade;
        }

        // ==========================================
        // CONEXÃO SQL
        // ==========================================

        private string ObterConnectionString()
        {
            foreach (SettingsProperty propriedade in
                DSProjectVet.Properties.Settings
                    .Default.Properties)
            {
                SpecialSettingAttribute atributo =
                    propriedade.Attributes[
                        typeof(SpecialSettingAttribute)]
                    as SpecialSettingAttribute;

                if (atributo == null ||
                    atributo.SpecialSetting !=
                    SpecialSetting.ConnectionString)
                    continue;

                string valor = Convert.ToString(
                    DSProjectVet.Properties.Settings
                        .Default[propriedade.Name]);

                if (!string.IsNullOrWhiteSpace(valor))
                    return valor;
            }

            throw new Exception(
                "Connection string não encontrada " +
                "nas configurações do Med Vet.");
        }

        // ==========================================
        // LOCALIZAR TABELA FINANCEIRA
        // ==========================================

        private string LocalizarTabelaFinanceira(
            SqlConnection conexao)
        {
            // Como o nome apareceu com grafias
            // diferentes nas etapas anteriores,
            // identificamos o nome real no SQL.

            string sql = @"
                SELECT TOP (1)
                    QUOTENAME(s.name) + '.' +
                    QUOTENAME(t.name)
                FROM sys.tables AS t
                INNER JOIN sys.schemas AS s
                    ON s.schema_id = t.schema_id
                WHERE s.name = 'dbo'
                  AND t.name LIKE N'Movimenta%Financeira'
                ORDER BY
                    CASE WHEN t.name =
                        N'Movimentacao_Financeira'
                    THEN 0 ELSE 1 END,
                    t.name;
            ";

            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            {
                object resultado =
                    comando.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    throw new Exception(
                        "Não foi encontrada a tabela " +
                        "de movimentação financeira. " +
                        "Confira seu nome no SQL Server.");
                }

                return Convert.ToString(resultado);
            }
        }

        // ==========================================
        // CONSULTAR RELATÓRIO
        // ==========================================

        private DataTable BuscarRelatorio(
            TipoRelatorio tipo,
            DateTime inicio,
            DateTime fimExclusivo)
        {
            DataTable resultado = new DataTable();

            using (SqlConnection conexao =
                new SqlConnection(
                    ObterConnectionString()))
            {
                conexao.Open();

                string tabelaFinanceira = null;

                if (tipo == TipoRelatorio.ResumoFinanceiro ||
                    tipo == TipoRelatorio.HistoricoFinanceiro)
                {
                    tabelaFinanceira =
                        LocalizarTabelaFinanceira(conexao);
                }

                string sql =
                    ObterSql(tipo, tabelaFinanceira);

                using (SqlCommand comando =
                    new SqlCommand(sql, conexao))
                using (SqlDataAdapter adaptador =
                    new SqlDataAdapter(comando))
                {
                    comando.CommandTimeout = 30;

                    if (UsaPeriodo(tipo))
                    {
                        comando.Parameters.Add(
                            "@Inicio",
                            SqlDbType.DateTime2).Value =
                                inicio;

                        comando.Parameters.Add(
                            "@Fim",
                            SqlDbType.DateTime2).Value =
                                fimExclusivo;
                    }

                    adaptador.Fill(resultado);
                }
            }

            return resultado;
        }

        private bool UsaPeriodo(TipoRelatorio tipo)
        {
            return tipo != TipoRelatorio.EstoqueProdutos &&
                   tipo != TipoRelatorio.EstoqueMedicamentos;
        }

        // ==========================================
        // CONSULTAS SQL POR RELATÓRIO
        // ==========================================

        private string ObterSql(
            TipoRelatorio tipo,
            string tabelaFinanceira)
        {
            switch (tipo)
            {
                // ==================================
                // 1. CONSULTAS POR DIA
                // ==================================

                case TipoRelatorio.Consultas:
                    return @"
                        SELECT
                            CAST(c.DataHora AS DATE) AS Dia,
                            COUNT(*) AS Consultas
                        FROM dbo.Consultas AS c
                        WHERE c.DataHora >= @Inicio
                          AND c.DataHora < @Fim
                        GROUP BY CAST(c.DataHora AS DATE)
                        ORDER BY Dia DESC;
                    ";

                // ==================================
                // 2. SERVIÇOS REALIZADOS
                // ==================================

                case TipoRelatorio.Servicos:
                    return @"
                        SELECT
                            s.Descricao AS Servico,

                            CASE s.Tipo
                                WHEN 'C' THEN 'Consulta'
                                WHEN 'V' THEN 'Vacina'
                                WHEN 'E' THEN 'Exame'
                                WHEN 'P' THEN 'Procedimento'
                                ELSE 'Outro'
                            END AS Categoria,

                            SUM(CAST(cs.Quantidade AS BIGINT))
                                AS Quantidade,

                            CAST(
                                SUM(
                                    CAST(cs.Quantidade
                                        AS DECIMAL(18,2))
                                    * CAST(cs.Preco
                                        AS DECIMAL(18,2))
                                ) AS DECIMAL(18,2)
                            ) AS ValorLancado

                        FROM dbo.Consulta_Servico AS cs

                        INNER JOIN dbo.Servicos AS s
                            ON s.ID_Servico =
                               cs.fk_Servicos_ID_Servico

                        INNER JOIN dbo.Consultas AS c
                            ON c.ID_Consulta =
                               cs.fk_Consultas_ID_Consulta

                        WHERE c.DataHora >= @Inicio
                          AND c.DataHora < @Fim

                        GROUP BY
                            s.ID_Servico,
                            s.Descricao,
                            s.Tipo

                        ORDER BY Quantidade DESC;
                    ";

                // ==================================
                // 3. MEDICAMENTOS PRESCRITOS
                // ==================================

                case TipoRelatorio.Prescricoes:
                    return @"
                        SELECT
                            m.Nome AS Medicamento,

                            COUNT(*) AS Prescricoes,

                            SUM(CAST(p.Quantidade AS BIGINT))
                                AS Unidades

                        FROM dbo.Prescricao AS p

                        INNER JOIN dbo.Medicamentos AS m
                            ON m.ID_Medicamento =
                               p.fk_Medicamentos_ID_Medicamento

                        INNER JOIN dbo.Consultas AS c
                            ON c.ID_Consulta =
                               p.fk_Consultas_ID_Consulta

                        WHERE c.DataHora >= @Inicio
                          AND c.DataHora < @Fim

                        GROUP BY
                            m.ID_Medicamento,
                            m.Nome

                        ORDER BY Unidades DESC;
                    ";

                // ==================================
                // 4. CONSUMO DE PRODUTOS
                // ==================================

                case TipoRelatorio.ConsumoProdutos:
                    return @"
                        SELECT
                            p.Nome AS Produto,

                            SUM(CAST(cp.Quantidade AS BIGINT))
                                AS Unidades,

                            CAST(
                                SUM(
                                    CAST(cp.Quantidade
                                        AS DECIMAL(18,2))
                                    * CAST(cp.Preco
                                        AS DECIMAL(18,2))
                                ) AS DECIMAL(18,2)
                            ) AS ValorEstimado

                        FROM dbo.Consulta_Produto AS cp

                        INNER JOIN dbo.Produtos AS p
                            ON p.ID_Produto =
                               cp.fk_Produtos_ID_Produto

                        INNER JOIN dbo.Consultas AS c
                            ON c.ID_Consulta =
                               cp.fk_Consultas_ID_Consulta

                        WHERE c.DataHora >= @Inicio
                          AND c.DataHora < @Fim

                        GROUP BY
                            p.ID_Produto,
                            p.Nome

                        ORDER BY
                            Unidades DESC,
                            p.Nome;
                    ";

                // ==================================
                // 5. ESTOQUE CRÍTICO - PRODUTOS
                // ==================================

                case TipoRelatorio.EstoqueProdutos:
                    return @"
                        SELECT
                            p.ID_Produto AS Codigo,
                            p.Nome AS Produto,

                            ISNULL(ep.Quantidade, 0)
                                AS Quantidade,

                            ISNULL(ep.EstoqueMinimo, 0)
                                AS EstoqueMinimo,

                            CASE
                                WHEN ISNULL(ep.Quantidade, 0) <= 0
                                    THEN 'Esgotado'
                                ELSE 'Abaixo ou no mínimo'
                            END AS Situacao

                        FROM dbo.Produtos AS p

                        LEFT JOIN dbo.Estoque_Produto AS ep
                            ON ep.fk_Produtos_ID_Produto =
                               p.ID_Produto

                        WHERE ep.fk_Produtos_ID_Produto IS NULL
                           OR ISNULL(ep.Quantidade, 0) <=
                              ISNULL(ep.EstoqueMinimo, 0)

                        ORDER BY
                            Quantidade ASC,
                            p.Nome;
                    ";

                // ==================================
                // 6. ESTOQUE CRÍTICO - MEDICAMENTOS
                // ==================================

                case TipoRelatorio.EstoqueMedicamentos:
                    return @"
                        SELECT
                            m.ID_Medicamento AS Codigo,
                            m.Nome AS Medicamento,

                            ISNULL(em.Quantidade, 0)
                                AS Quantidade,

                            ISNULL(em.EstoqueMinimo, 0)
                                AS EstoqueMinimo,

                            CASE
                                WHEN ISNULL(em.Quantidade, 0) <= 0
                                    THEN 'Esgotado'
                                ELSE 'Abaixo ou no mínimo'
                            END AS Situacao

                        FROM dbo.Medicamentos AS m

                        LEFT JOIN dbo.Estoque_Medicamento AS em
                            ON em.fk_Medicamentos_ID_Medicamento =
                               m.ID_Medicamento

                        WHERE em.fk_Medicamentos_ID_Medicamento IS NULL
                           OR ISNULL(em.Quantidade, 0) <=
                              ISNULL(em.EstoqueMinimo, 0)

                        ORDER BY
                            Quantidade ASC,
                            m.Nome;
                    ";

                // ==================================
                // 7. RESUMO FINANCEIRO
                // ==================================

                case TipoRelatorio.ResumoFinanceiro:
                    return @"
                        SELECT
                            CASE LTRIM(RTRIM(m.Tipo))
                                WHEN 'E' THEN 'Entrada'
                                WHEN 'S' THEN 'Saída'
                            END AS Movimento,

                            COUNT(*) AS Lancamentos,

                            CAST(
                                ISNULL(SUM(m.Valor), 0)
                                AS DECIMAL(18,2)
                            ) AS ValorTotal

                        FROM " + tabelaFinanceira + @" AS m

                        WHERE m.DataHora >= @Inicio
                          AND m.DataHora < @Fim
                          AND m.Tipo IN ('E', 'S')

                        GROUP BY m.Tipo

                        ORDER BY Movimento;
                    ";

                // ==================================
                // 8. HISTÓRICO FINANCEIRO
                // ==================================

                case TipoRelatorio.HistoricoFinanceiro:
                    return @"
                        SELECT
                            m.ID_Movimentacao AS Codigo,
                            m.DataHora,

                            CASE LTRIM(RTRIM(m.Tipo))
                                WHEN 'E' THEN 'Entrada'
                                WHEN 'S' THEN 'Saída'
                                ELSE 'Outro'
                            END AS Tipo,

                            m.Descricao,

                            CAST(m.Valor AS DECIMAL(18,2))
                                AS Valor,

                            m.fk_Consultas_ID_Consulta
                                AS Consulta

                        FROM " + tabelaFinanceira + @" AS m

                        WHERE m.DataHora >= @Inicio
                          AND m.DataHora < @Fim

                        ORDER BY
                            m.DataHora DESC,
                            m.ID_Movimentacao DESC;
                    ";

                default:
                    throw new Exception(
                        "Tipo de relatório inválido.");
            }
        }

        // ==========================================
        // GERAR RELATÓRIO
        // ==========================================

        private async Task GerarRelatorioAsync()
        {
            if (carregando)
                return;

            OpcaoRelatorio opcao =
                cmbRelatorios.SelectedItem
                as OpcaoRelatorio;

            if (opcao == null)
                return;

            // Não confia somente na visibilidade
            // das opções do ComboBox.
            if (!PodeVerRelatorio(opcao.Tipo))
            {
                MessageBox.Show(
                    this,
                    "Seu usuário não possui permissão " +
                    "para gerar este relatório.",
                    "Acesso negado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DateTime inicio = dtInicio.Value.Date;
            DateTime fim = dtFim.Value.Date;

            if (fim < inicio && UsaPeriodo(opcao.Tipo))
            {
                MessageBox.Show(
                    this,
                    "A data final não pode ser anterior " +
                    "à data inicial.",
                    "Período inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            carregando = true;
            btnGerar.Enabled = false;
            btnExportar.Enabled = false;

            lblStatus.Text =
                "Gerando relatório: " + opcao.Nome + "...";

            try
            {
                DataTable resultado =
                    await Task.Run(() =>
                        BuscarRelatorio(
                            opcao.Tipo,
                            inicio,
                            fim.AddDays(1)));

                dadosAtuais = resultado;
                relatorioCarregado = opcao.Tipo;

                AplicarPesquisa();

                AtualizarIndicadores(
                    opcao.Tipo,
                    resultado,
                    inicio,
                    fim);

                btnExportar.Enabled =
                    resultado.Rows.Count > 0;

                lblStatus.Text =
                    resultado.Rows.Count +
                    " registro(s) no relatório: " +
                    opcao.Nome + ".";

                if (!UsaPeriodo(opcao.Tipo))
                {
                    lblStatus.Text +=
                        " Estoque atual: as datas " +
                        "não são aplicadas.";
                }
            }
            catch (Exception ex)
            {
                relatorioCarregado = null;
                dadosAtuais = new DataTable();

                gridRelatorio.DataSource = null;

                LimparIndicadores();

                lblStatus.Text =
                    "Erro ao gerar relatório.";

                MessageBox.Show(
                    this,
                    "Não foi possível gerar o relatório.\n\n" +
                    ex.Message,
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                carregando = false;
                btnGerar.Enabled = true;
            }
        }

        // ==========================================
        // PESQUISA NA TABELA ATUAL
        // ==========================================

        private void AplicarPesquisa()
        {
            if (gridRelatorio == null ||
                txtPesquisa == null ||
                relatorioCarregado == null)
                return;

            DataTable filtrada =
                dadosAtuais.Clone();

            string busca =
                txtPesquisa.Text.Trim();

            foreach (DataRow linha in dadosAtuais.Rows)
            {
                bool encontrada =
                    busca.Length == 0;

                if (!encontrada)
                {
                    foreach (object campo in linha.ItemArray)
                    {
                        string valor =
                            Convert.ToString(campo);

                        if (valor.IndexOf(
                            busca,
                            StringComparison
                                .CurrentCultureIgnoreCase) >= 0)
                        {
                            encontrada = true;
                            break;
                        }
                    }
                }

                if (encontrada)
                    filtrada.ImportRow(linha);
            }

            gridRelatorio.DataSource = filtrada;

            ConfigurarColunas();

            gridRelatorio.ClearSelection();
            gridRelatorio.CurrentCell = null;
        }

        private void ConfigurarColunas()
        {
            foreach (DataGridViewColumn coluna
                in gridRelatorio.Columns)
            {
                string nome = coluna.Name;

                if (nome == "Valor" ||
                    nome == "ValorTotal" ||
                    nome == "ValorLancado" ||
                    nome == "ValorEstimado")
                {
                    coluna.DefaultCellStyle.Format =
                        "C2";

                    coluna.DefaultCellStyle
                        .FormatProvider = cultura;
                }

                if (nome == "DataHora")
                {
                    coluna.HeaderText =
                        "Data / Hora";

                    coluna.DefaultCellStyle.Format =
                        "dd/MM/yyyy HH:mm";
                }

                if (nome == "EstoqueMinimo")
                    coluna.HeaderText = "Estoque mínimo";

                if (nome == "Situacao")
                    coluna.HeaderText = "Situação";

                if (nome == "Prescricoes")
                    coluna.HeaderText = "Prescrições";

                if (nome == "Lancamentos")
                    coluna.HeaderText = "Lançamentos";
            }

            if (gridRelatorio.Columns.Contains(
                "Descricao"))
            {
                gridRelatorio.Columns["Descricao"]
                    .AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        // ==========================================
        // INDICADORES DO RESUMO
        // ==========================================

        private void DefinirIndicador(
            int indice,
            string titulo,
            string valor)
        {
            lblTitulos[indice].Text = titulo;
            lblValores[indice].Text = valor;
        }

        private void LimparIndicadores()
        {
            for (int i = 0; i < 4; i++)
            {
                DefinirIndicador(i, "Indicador", "—");
            }
        }

        private decimal Somar(
            DataTable tabela,
            string coluna)
        {
            decimal total = 0;

            if (!tabela.Columns.Contains(coluna))
                return 0;

            foreach (DataRow linha in tabela.Rows)
            {
                if (linha[coluna] != DBNull.Value)
                {
                    total += Convert.ToDecimal(
                        linha[coluna]);
                }
            }

            return total;
        }

        private string Moeda(decimal valor)
        {
            return valor.ToString("C2", cultura);
        }

        private void AtualizarIndicadores(
            TipoRelatorio tipo,
            DataTable tabela,
            DateTime inicio,
            DateTime fim)
        {
            int linhas = tabela.Rows.Count;

            int dias =
                (fim - inicio).Days + 1;

            switch (tipo)
            {
                case TipoRelatorio.Consultas:
                    decimal consultas =
                        Somar(tabela, "Consultas");

                    DefinirIndicador(
                        0, "Consultas", consultas.ToString("N0"));

                    DefinirIndicador(
                        1, "Dias com consultas",
                        linhas.ToString());

                    DefinirIndicador(
                        2, "Média por dia",
                        (consultas / dias).ToString("N1"));

                    DefinirIndicador(
                        3, "Período analisado",
                        dias + " dia(s)");
                    break;

                case TipoRelatorio.Servicos:
                    DefinirIndicador(
                        0, "Serviços diferentes",
                        linhas.ToString());

                    DefinirIndicador(
                        1, "Quantidade realizada",
                        Somar(tabela, "Quantidade")
                            .ToString("N0"));

                    DefinirIndicador(
                        2, "Valor lançado",
                        Moeda(Somar(
                            tabela, "ValorLancado")));

                    DefinirIndicador(
                        3, "Mais realizado",
                        linhas == 0
                            ? "—"
                            : Convert.ToString(
                                tabela.Rows[0]["Servico"]));
                    break;

                case TipoRelatorio.Prescricoes:
                    DefinirIndicador(
                        0, "Medicamentos diferentes",
                        linhas.ToString());

                    DefinirIndicador(
                        1, "Prescrições",
                        Somar(tabela, "Prescricoes")
                            .ToString("N0"));

                    DefinirIndicador(
                        2, "Unidades prescritas",
                        Somar(tabela, "Unidades")
                            .ToString("N0"));

                    DefinirIndicador(
                        3, "Mais prescrito",
                        linhas == 0
                            ? "—"
                            : Convert.ToString(
                                tabela.Rows[0]["Medicamento"]));
                    break;

                case TipoRelatorio.ConsumoProdutos:
                    DefinirIndicador(
                        0, "Produtos diferentes",
                        linhas.ToString());

                    DefinirIndicador(
                        1, "Unidades utilizadas",
                        Somar(tabela, "Unidades")
                            .ToString("N0"));

                    DefinirIndicador(
                        2, "Valor dos materiais",
                        Moeda(Somar(
                            tabela, "ValorEstimado")));

                    DefinirIndicador(
                        3, "Mais consumido",
                        linhas == 0
                            ? "—"
                            : Convert.ToString(
                                tabela.Rows[0]["Produto"]));
                    break;

                case TipoRelatorio.EstoqueProdutos:
                case TipoRelatorio.EstoqueMedicamentos:
                    int esgotados = 0;

                    foreach (DataRow linha in tabela.Rows)
                    {
                        if (Convert.ToDecimal(
                            linha["Quantidade"]) <= 0)
                        {
                            esgotados++;
                        }
                    }

                    DefinirIndicador(
                        0, "Itens em alerta",
                        linhas.ToString());

                    DefinirIndicador(
                        1, "Itens esgotados",
                        esgotados.ToString());

                    DefinirIndicador(
                        2, "Unidades restantes",
                        Somar(tabela, "Quantidade")
                            .ToString("N0"));

                    DefinirIndicador(
                        3, "Situação",
                        linhas == 0
                            ? "Sem alertas"
                            : "Requer atenção");
                    break;

                case TipoRelatorio.ResumoFinanceiro:
                    decimal entradas = 0;
                    decimal saidas = 0;
                    decimal lancamentos = 0;

                    foreach (DataRow linha in tabela.Rows)
                    {
                        string movimento =
                            Convert.ToString(
                                linha["Movimento"]);

                        decimal valor =
                            Convert.ToDecimal(
                                linha["ValorTotal"]);

                        lancamentos += Convert.ToDecimal(
                            linha["Lancamentos"]);

                        if (movimento == "Entrada")
                            entradas += valor;
                        else if (movimento == "Saída")
                            saidas += valor;
                    }

                    DefinirIndicador(
                        0, "Entradas", Moeda(entradas));

                    DefinirIndicador(
                        1, "Saídas", Moeda(saidas));

                    DefinirIndicador(
                        2, "Saldo estimado",
                        Moeda(entradas - saidas));

                    DefinirIndicador(
                        3, "Movimentações",
                        lancamentos.ToString("N0"));
                    break;

                case TipoRelatorio.HistoricoFinanceiro:
                    decimal totalE = 0;
                    decimal totalS = 0;

                    foreach (DataRow linha in tabela.Rows)
                    {
                        string movimento =
                            Convert.ToString(linha["Tipo"]);

                        decimal valor =
                            Convert.ToDecimal(linha["Valor"]);

                        if (movimento == "Entrada")
                            totalE += valor;

                        if (movimento == "Saída")
                            totalS += valor;
                    }

                    DefinirIndicador(
                        0, "Movimentações",
                        linhas.ToString());

                    DefinirIndicador(
                        1, "Entradas",
                        Moeda(totalE));

                    DefinirIndicador(
                        2, "Saídas",
                        Moeda(totalS));

                    DefinirIndicador(
                        3, "Saldo estimado",
                        Moeda(totalE - totalS));
                    break;
            }
        }

        // ==========================================
        // EXPORTAR RESULTADOS PARA CSV
        // ==========================================

        private void ExportarCsv()
        {
            if (relatorioCarregado == null ||
                !PodeVerRelatorio(
                    relatorioCarregado.Value))
            {
                MessageBox.Show(
                    this,
                    "Você não possui acesso " +
                    "a este relatório.",
                    "Acesso negado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DataTable tabela =
                gridRelatorio.DataSource as DataTable;

            if (tabela == null ||
                tabela.Rows.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Não há dados para exportar.",
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            using (SaveFileDialog salvar =
                new SaveFileDialog())
            {
                salvar.Title =
                    "Exportar relatório";

                salvar.Filter =
                    "Arquivo CSV (*.csv)|*.csv";

                salvar.DefaultExt = "csv";
                salvar.AddExtension = true;

                salvar.FileName =
                    "MedVet_Relatorio_" +
                    DateTime.Now.ToString(
                        "yyyyMMdd_HHmmss") + ".csv";

                if (salvar.ShowDialog(this) !=
                    DialogResult.OK)
                {
                    return;
                }

                try
                {
                    StringBuilder csv =
                        new StringBuilder();

                    for (int i = 0;
                        i < tabela.Columns.Count;
                        i++)
                    {
                        if (i > 0)
                            csv.Append(";");

                        csv.Append(
                            CampoCsv(
                                tabela.Columns[i]
                                    .ColumnName,
                                true));
                    }

                    csv.AppendLine();

                    foreach (DataRow linha
                        in tabela.Rows)
                    {
                        for (int i = 0;
                            i < tabela.Columns.Count;
                            i++)
                        {
                            if (i > 0)
                                csv.Append(";");

                            object valor = linha[i];

                            string conteudo = "";

                            if (valor != DBNull.Value)
                            {
                                if (valor is DateTime)
                                {
                                    conteudo =
                                        ((DateTime)valor)
                                        .ToString(
                                            "dd/MM/yyyy HH:mm");
                                }
                                else
                                {
                                    conteudo =
                                        Convert.ToString(
                                            valor, cultura);
                                }
                            }

                            csv.Append(
                                CampoCsv(
                                    conteudo,
                                    valor is string));
                        }

                        csv.AppendLine();
                    }

                    File.WriteAllText(
                        salvar.FileName,
                        csv.ToString(),
                        new UTF8Encoding(true));

                    MessageBox.Show(
                        this,
                        "Relatório exportado com sucesso!",
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this,
                        "Erro ao exportar CSV:\n\n" +
                        ex.Message,
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private string CampoCsv(
            string valor,
            bool protegerFormula)
        {
            string conteudo = valor ?? "";

            // Impede que um texto vindo do banco
            // seja interpretado como fórmula
            // quando aberto no Excel.
            if (protegerFormula)
            {
                string inicio =
                    conteudo.TrimStart();

                if (inicio.StartsWith("=") ||
                    inicio.StartsWith("+") ||
                    inicio.StartsWith("-") ||
                    inicio.StartsWith("@"))
                {
                    conteudo = "'" + conteudo;
                }
            }

            return "\"" +
                conteudo.Replace("\"", "\"\"") +
                "\"";
        }
    }
}
