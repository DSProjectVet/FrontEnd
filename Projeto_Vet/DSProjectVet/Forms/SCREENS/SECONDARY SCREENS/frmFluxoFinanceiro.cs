
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
    public class frmFluxoFinanceiro : Form
    {
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

        private readonly CultureInfo cultura =
            new CultureInfo("pt-BR");

        private DateTimePicker dtInicio;
        private DateTimePicker dtFim;

        private TextBox txtPesquisa;
        private ComboBox cmbTipo;
        private Button btnAtualizar;

        private TabControl abas;

        private DataGridView gridEntradas;
        private DataGridView gridSaidas;
        private DataGridView gridMovimentacoes;

        private Label lblEntradas;
        private Label lblSaidas;
        private Label lblSaldo;
        private Label lblMovimentacoes;
        private Label lblQtdEntradas;
        private Label lblQtdSaidas;
        private Label lblMaisSaiu;
        private Label lblMenosSaiu;
        private Label lblStatus;

        private DataTable movimentos = new DataTable();

        private bool carregando;

        private class DadosFinanceiros
        {
            public DataTable Movimentos;
            public DataTable Produtos;
            public DataTable Quantidades;
        }

        public frmFluxoFinanceiro()
        {
            Text = "Med Vet - Fluxo Financeiro";
            BackColor = fundo;
            Font = new Font("Segoe UI", 10F);

            MinimumSize = new Size(940, 640);

            ConstruirInterface();

            Shown += async (s, e) =>
            {
                await CarregarDadosAsync();
            };
        }

        // ======================================
        // CONSTRUIR INTERFACE
        // ======================================

        private void ConstruirInterface()
        {
            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.Padding =
                new Padding(22, 18, 22, 15);

            layout.ColumnCount = 1;
            layout.RowCount = 4;

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 83));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 88));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 43));

            Controls.Add(layout);

            // ==================================
            // CABEÇALHO
            // ==================================

            Panel cabecalho = new Panel
            {
                Dock = DockStyle.Fill
            };

            layout.Controls.Add(cabecalho, 0, 0);

            Label titulo = CriarLabel(
                "Fluxo financeiro", 23, true);

            titulo.SetBounds(0, 0, 640, 45);
            cabecalho.Controls.Add(titulo);

            Label subtitulo = CriarLabel(
                "Entradas, saídas e movimentações da clínica.",
                10, false);

            subtitulo.ForeColor = cinza;
            subtitulo.SetBounds(2, 47, 700, 27);

            cabecalho.Controls.Add(subtitulo);

            // ==================================
            // FILTROS
            // ==================================

            FlowLayoutPanel filtros =
                new FlowLayoutPanel();

            filtros.Dock = DockStyle.Fill;
            filtros.WrapContents = false;
            filtros.AutoScroll = true;
            filtros.BackColor = Color.White;

            filtros.Padding = new Padding(12, 5, 8, 4);

            layout.Controls.Add(filtros, 0, 1);

            dtInicio = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(-29)
            };

            dtFim = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            txtPesquisa = new TextBox();

            cmbTipo = new ComboBox
            {
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };

            cmbTipo.Items.AddRange(new object[]
            {
                "Todas",
                "Entradas (E)",
                "Saídas (S)"
            });

            cmbTipo.SelectedIndex = 0;

            filtros.Controls.Add(
                CriarCampoFiltro("Data inicial", dtInicio, 140));

            filtros.Controls.Add(
                CriarCampoFiltro("Data final", dtFim, 140));

            filtros.Controls.Add(
                CriarCampoFiltro(
                    "Pesquisar movimentação", txtPesquisa, 260));

            filtros.Controls.Add(
                CriarCampoFiltro(
                    "Tipo", cmbTipo, 145));

            btnAtualizar = CriarBotao(
                "Atualizar",
                verde,
                Color.White,
                125);

            btnAtualizar.Margin =
                new Padding(8, 22, 0, 0);

            filtros.Controls.Add(btnAtualizar);

            btnAtualizar.Click += async (s, e) =>
            {
                await CarregarDadosAsync();
            };

            txtPesquisa.TextChanged += (s, e) =>
                AplicarFiltros();

            cmbTipo.SelectedIndexChanged += (s, e) =>
                AplicarFiltros();

            // ==================================
            // ABAS
            // ==================================

            abas = new TabControl();

            abas.Dock = DockStyle.Fill;
            abas.Font = new Font("Segoe UI", 10F);

            abas.Margin = new Padding(0, 12, 0, 0);

            layout.Controls.Add(abas, 0, 2);

            TabPage paginaResumo =
                CriarPagina("Resumo");

            TabPage paginaEntradas =
                CriarPagina("Entradas");

            TabPage paginaSaidas =
                CriarPagina("Saídas");

            TabPage paginaMovimentacoes =
                CriarPagina("Movimentações");

            abas.TabPages.Add(paginaResumo);
            abas.TabPages.Add(paginaEntradas);
            abas.TabPages.Add(paginaSaidas);
            abas.TabPages.Add(paginaMovimentacoes);

            ConstruirResumo(paginaResumo);

            gridEntradas =
                CriarTabela(paginaEntradas);

            gridSaidas =
                CriarTabela(paginaSaidas);

            gridMovimentacoes =
                CriarTabela(paginaMovimentacoes);

            // ==================================
            // RODAPÉ
            // ==================================

            lblStatus = CriarLabel(
                "Aguardando dados financeiros...",
                9, false);

            lblStatus.Dock = DockStyle.Fill;
            lblStatus.ForeColor = cinza;

            layout.Controls.Add(lblStatus, 0, 3);
        }

        // ======================================
        // ABA RESUMO
        // ======================================

        private void ConstruirResumo(TabPage pagina)
        {
            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(12);
            layout.ColumnCount = 2;
            layout.RowCount = 3;

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 33));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 33));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 34));

            pagina.Controls.Add(layout);

            layout.Controls.Add(
                CriarCartao(
                    "Entradas",
                    out lblEntradas,
                    out lblQtdEntradas),
                0, 0);

            layout.Controls.Add(
                CriarCartao(
                    "Saídas",
                    out lblSaidas,
                    out lblQtdSaidas),
                1, 0);

            Label descricaoSaldo;

            layout.Controls.Add(
                CriarCartao(
                    "Saldo estimado",
                    out lblSaldo,
                    out descricaoSaldo),
                0, 1);

            descricaoSaldo.Text =
                "Entradas menos saídas";

            Label descricaoMovimentacoes;

            layout.Controls.Add(
                CriarCartao(
                    "Total de movimentações",
                    out lblMovimentacoes,
                    out descricaoMovimentacoes),
                1, 1);

            descricaoMovimentacoes.Text =
                "Lançamentos do período";

            Label detalheMais;

            layout.Controls.Add(
                CriarCartao(
                    "Produto que mais saiu",
                    out lblMaisSaiu,
                    out detalheMais),
                0, 2);

            detalheMais.Text =
                "Quantidade consumida";

            Label detalheMenos;

            layout.Controls.Add(
                CriarCartao(
                    "Produto que menos saiu",
                    out lblMenosSaiu,
                    out detalheMenos),
                1, 2);

            detalheMenos.Text =
                "Entre os produtos consumidos";
        }

        // ======================================
        // COMPONENTES VISUAIS
        // ======================================

        private Label CriarLabel(
            string titulo,
            float tamanho,
            bool negrito)
        {
            return new Label
            {
                Text = titulo,
                ForeColor = texto,
                Font = new Font(
                    "Segoe UI",
                    tamanho,
                    negrito
                        ? FontStyle.Bold
                        : FontStyle.Regular),
                TextAlign =
                    ContentAlignment.MiddleLeft,
                AutoSize = false
            };
        }

        private TabPage CriarPagina(string titulo)
        {
            return new TabPage(titulo)
            {
                BackColor = fundo
            };
        }

        private Control CriarCampoFiltro(
            string titulo,
            Control campo,
            int largura)
        {
            Panel painel = new Panel
            {
                Width = largura,
                Height = 69,
                Margin = new Padding(0, 0, 12, 0)
            };

            Label label = CriarLabel(
                titulo, 9, true);

            label.SetBounds(
                0, 2, largura - 4, 22);

            painel.Controls.Add(label);

            campo.SetBounds(
                0, 30, largura - 4, 30);

            campo.Font =
                new Font("Segoe UI", 10F);

            painel.Controls.Add(campo);

            return painel;
        }

        private Button CriarBotao(
            string titulo,
            Color corFundo,
            Color corTexto,
            int largura)
        {
            Button botao = new Button
            {
                Text = titulo,
                Size = new Size(largura, 40),
                BackColor = corFundo,
                ForeColor = corTexto,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                Font = new Font(
                    "Segoe UI", 9F, FontStyle.Bold)
            };

            botao.FlatAppearance.BorderSize = 0;

            return botao;
        }

        private Panel CriarCartao(
            string titulo,
            out Label valor,
            out Label detalhe)
        {
            Panel painel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 12, 12),
                Padding = new Padding(17)
            };

            TableLayoutPanel conteudo =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3
                };

            conteudo.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 30));

            conteudo.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100));

            conteudo.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 26));

            painel.Controls.Add(conteudo);

            Label cabecalho = CriarLabel(
                titulo, 10, true);

            cabecalho.Dock = DockStyle.Fill;

            conteudo.Controls.Add(cabecalho, 0, 0);

            valor = CriarLabel("—", 19, true);

            valor.Dock = DockStyle.Fill;
            valor.ForeColor = verdeEscuro;
            valor.AutoEllipsis = true;

            conteudo.Controls.Add(valor, 0, 1);

            detalhe = CriarLabel("", 9, false);

            detalhe.Dock = DockStyle.Fill;
            detalhe.ForeColor = cinza;
            detalhe.AutoEllipsis = true;

            conteudo.Controls.Add(detalhe, 0, 2);

            return painel;
        }


        private DataGridView CriarTabela(TabPage pagina)
        {
            Panel painel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            pagina.Controls.Add(painel);

            DataGridView grade = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                MultiSelect = false,

                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.DisplayedCells,

                RowTemplate = { Height = 40 },
                ColumnHeadersHeight = 43,
                EnableHeadersVisualStyles = false
            };

            // ==========================================
            // CABEÇALHO
            // ==========================================

            grade.ColumnHeadersDefaultCellStyle.BackColor =
                verdeEscuro;

            grade.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            grade.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            // ==========================================
            // LINHAS PADRÃO
            // ==========================================

            grade.DefaultCellStyle.BackColor =
                Color.White;

            grade.DefaultCellStyle.ForeColor =
                texto;

            grade.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            grade.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(225, 248, 240);

            grade.DefaultCellStyle.SelectionForeColor =
                verdeEscuro;

            grade.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 252, 250);

            grade.AlternatingRowsDefaultCellStyle.ForeColor =
                texto;

            grade.GridColor =
                Color.FromArgb(235, 240, 237);

            // ==========================================
            // CORES DAS SAÍDAS FINANCEIRAS
            // ==========================================

            Color vermelhoClaro =
                Color.FromArgb(255, 235, 235);

            Color vermelhoClaroAlternado =
                Color.FromArgb(255, 244, 244);

            Color vermelhoSelecionado =
                Color.FromArgb(255, 210, 210);

            Color vermelhoTexto =
                Color.FromArgb(160, 45, 45);

            // ==========================================
            // COLORIR LINHAS APÓS CARREGAR DADOS
            // ==========================================

            grade.DataBindingComplete += (s, e) =>
            {
                bool abaSaidas =
                    pagina.Text.IndexOf(
                        "Saídas",
                        StringComparison.OrdinalIgnoreCase) >= 0
                    ||
                    pagina.Text.IndexOf(
                        "Saidas",
                        StringComparison.OrdinalIgnoreCase) >= 0;

                foreach (DataGridViewRow linha in grade.Rows)
                {
                    if (linha.IsNewRow)
                        continue;

                    bool ehSaida = abaSaidas;

                    // Identifica saídas na aba Movimentações.
                    if (grade.Columns.Contains("Tipo"))
                    {
                        string tipo = Convert.ToString(
                            linha.Cells["Tipo"].Value)
                            .Trim();

                        if (tipo.Equals(
                                "S",
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            tipo.Equals(
                                "Saída",
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            tipo.Equals(
                                "Saida",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            ehSaida = true;
                        }
                    }

                    if (ehSaida)
                    {
                        linha.DefaultCellStyle.BackColor =
                            linha.Index % 2 == 0
                                ? vermelhoClaro
                                : vermelhoClaroAlternado;

                        linha.DefaultCellStyle.ForeColor =
                            vermelhoTexto;

                        linha.DefaultCellStyle.SelectionBackColor =
                            vermelhoSelecionado;

                        linha.DefaultCellStyle.SelectionForeColor =
                            vermelhoTexto;
                    }
                    else
                    {
                        linha.DefaultCellStyle.BackColor =
                            linha.Index % 2 == 0
                                ? Color.White
                                : Color.FromArgb(248, 252, 250);

                        linha.DefaultCellStyle.ForeColor =
                            texto;

                        linha.DefaultCellStyle.SelectionBackColor =
                            Color.FromArgb(225, 248, 240);

                        linha.DefaultCellStyle.SelectionForeColor =
                            verdeEscuro;
                    }
                }
            };

            painel.Controls.Add(grade);

            return grade;
        }


        // ======================================
        // CONNECTION STRING
        // ======================================

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
                "Connection string não encontrada.");
        }

        // ======================================
        // CONSULTAS SQL
        // ======================================

        private DataTable Consultar(
            SqlConnection conexao,
            string sql,
            DateTime inicio,
            DateTime fimExclusivo)
        {
            DataTable resultado = new DataTable();

            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            using (SqlDataAdapter adaptador =
                new SqlDataAdapter(comando))
            {
                comando.Parameters.Add(
                    "@Inicio",
                    SqlDbType.DateTime2).Value = inicio;

                comando.Parameters.Add(
                    "@Fim",
                    SqlDbType.DateTime2).Value =
                        fimExclusivo;

                comando.CommandTimeout = 30;

                adaptador.Fill(resultado);
            }

            return resultado;
        }

        private DadosFinanceiros BuscarDados(
            DateTime inicio,
            DateTime fimExclusivo)
        {
            DadosFinanceiros resultado =
                new DadosFinanceiros();

            string sqlMovimentos = @"
                SELECT
                    ID_Movimentacao,
                    Tipo,
                    DataHora,
                    Valor,
                    Descricao,
                    fk_Consultas_ID_Consulta
                        AS Consulta,
                    Origem,
                    IdOrigem
                FROM dbo.Movimentacao_Financeira
                WHERE DataHora >= @Inicio
                  AND DataHora < @Fim
                ORDER BY
                    DataHora DESC,
                    ID_Movimentacao DESC;
            ";

            // Produtos efetivamente lançados como
            // consumo financeiro no período.
            string sqlProdutos = @"
                SELECT
                    p.Nome,
                    SUM(cp.Quantidade) AS Unidades
                FROM dbo.Consulta_Produto AS cp
                INNER JOIN dbo.Produtos AS p
                    ON p.ID_Produto =
                       cp.fk_Produtos_ID_Produto
                INNER JOIN dbo.Movimentacao_Financeira AS mf
                    ON mf.fk_Consultas_ID_Consulta =
                       cp.fk_Consultas_ID_Consulta
                   AND mf.Origem = 'PRODUTO'
                   AND mf.IdOrigem =
                       cp.fk_Produtos_ID_Produto
                WHERE mf.DataHora >= @Inicio
                  AND mf.DataHora < @Fim
                GROUP BY p.ID_Produto, p.Nome
                ORDER BY
                    SUM(cp.Quantidade) DESC,
                    p.Nome;
            ";

            string sqlQuantidades = @"
                SELECT
                    (
                        SELECT ISNULL(SUM(cs.Quantidade), 0)
                        FROM dbo.Consulta_Servico AS cs
                        INNER JOIN dbo.Movimentacao_Financeira AS mf
                            ON mf.fk_Consultas_ID_Consulta =
                               cs.fk_Consultas_ID_Consulta
                           AND mf.Origem = 'SERVICO'
                           AND mf.IdOrigem =
                               cs.fk_Servicos_ID_Servico
                        WHERE mf.DataHora >= @Inicio
                          AND mf.DataHora < @Fim
                    ) AS UnidadesServicos,

                    (
                        SELECT ISNULL(SUM(cp.Quantidade), 0)
                        FROM dbo.Consulta_Produto AS cp
                        INNER JOIN dbo.Movimentacao_Financeira AS mf
                            ON mf.fk_Consultas_ID_Consulta =
                               cp.fk_Consultas_ID_Consulta
                           AND mf.Origem = 'PRODUTO'
                           AND mf.IdOrigem =
                               cp.fk_Produtos_ID_Produto
                        WHERE mf.DataHora >= @Inicio
                          AND mf.DataHora < @Fim
                    )
                    +
                    (
                        SELECT ISNULL(SUM(pr.Quantidade), 0)
                        FROM dbo.Prescricao AS pr
                        INNER JOIN dbo.Movimentacao_Financeira AS mf
                            ON mf.fk_Consultas_ID_Consulta =
                               pr.fk_Consultas_ID_Consulta
                           AND mf.Origem = 'PRESCRICAO'
                           AND mf.IdOrigem = pr.ID_Prescricao
                        WHERE mf.DataHora >= @Inicio
                          AND mf.DataHora < @Fim
                    ) AS UnidadesSaida;
            ";

            using (SqlConnection conexao =
                new SqlConnection(
                    ObterConnectionString()))
            {
                conexao.Open();

                resultado.Movimentos =
                    Consultar(
                        conexao, sqlMovimentos,
                        inicio, fimExclusivo);

                resultado.Produtos =
                    Consultar(
                        conexao, sqlProdutos,
                        inicio, fimExclusivo);

                resultado.Quantidades =
                    Consultar(
                        conexao, sqlQuantidades,
                        inicio, fimExclusivo);
            }

            return resultado;
        }

        // ======================================
        // CARREGAR DASHBOARD
        // ======================================

        private async Task CarregarDadosAsync()
        {
            if (carregando)
                return;

            DateTime inicio = dtInicio.Value.Date;
            DateTime fim = dtFim.Value.Date;

            if (fim < inicio)
            {
                MessageBox.Show(
                    this,
                    "A data final deve ser igual ou " +
                    "posterior à data inicial.",
                    "Período inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            carregando = true;
            btnAtualizar.Enabled = false;

            lblStatus.Text =
                "Consultando movimentações financeiras...";

            try
            {
                DadosFinanceiros dados =
                    await Task.Run(() =>
                        BuscarDados(
                            inicio,
                            fim.AddDays(1)));

                movimentos = dados.Movimentos;

                AtualizarResumo(dados);

                AplicarFiltros();

                lblStatus.Text =
                    movimentos.Rows.Count +
                    " movimentação(ões) encontrada(s).";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível carregar " +
                    "o módulo financeiro.\n\n" +
                    ex.Message,
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text =
                    "Erro ao consultar SQL Server.";
            }
            finally
            {
                carregando = false;
                btnAtualizar.Enabled = true;
            }
        }

        // ======================================
        // RESUMO
        // ======================================

        private void AtualizarResumo(
            DadosFinanceiros dados)
        {
            decimal totalE = 0;
            decimal totalS = 0;

            int quantidadeE = 0;
            int quantidadeS = 0;

            foreach (DataRow linha in dados.Movimentos.Rows)
            {
                string tipo = Convert.ToString(
                    linha["Tipo"]).Trim();

                decimal valor =
                    linha["Valor"] == DBNull.Value
                        ? 0
                        : Convert.ToDecimal(linha["Valor"]);

                if (tipo == "E")
                {
                    totalE += valor;
                    quantidadeE++;
                }
                else if (tipo == "S")
                {
                    totalS += valor;
                    quantidadeS++;
                }
            }

            lblEntradas.Text =
                totalE.ToString("C2", cultura);

            lblSaidas.Text =
                totalS.ToString("C2", cultura);

            lblSaldo.Text =
                (totalE - totalS).ToString(
                    "C2", cultura);

            lblMovimentacoes.Text =
                dados.Movimentos.Rows.Count.ToString();

            int unidadesServicos = 0;
            int unidadesSaida = 0;

            if (dados.Quantidades.Rows.Count > 0)
            {
                DataRow quantidades =
                    dados.Quantidades.Rows[0];

                unidadesServicos = Convert.ToInt32(
                    quantidades["UnidadesServicos"]);

                unidadesSaida = Convert.ToInt32(
                    quantidades["UnidadesSaida"]);
            }

            lblQtdEntradas.Text =
                unidadesServicos +
                " serviço(s) • " +
                quantidadeE + " lançamento(s)";

            lblQtdSaidas.Text =
                unidadesSaida +
                " unidade(s) • " +
                quantidadeS + " lançamento(s)";

            if (dados.Produtos.Rows.Count == 0)
            {
                lblMaisSaiu.Text = "Sem dados";
                lblMenosSaiu.Text = "Sem dados";
            }
            else
            {
                DataRow maior =
                    dados.Produtos.Rows[0];

                DataRow menor =
                    dados.Produtos.Rows[
                        dados.Produtos.Rows.Count - 1];

                lblMaisSaiu.Text =
                    Convert.ToString(maior["Nome"]) +
                    " (" + maior["Unidades"] + ")";

                lblMenosSaiu.Text =
                    Convert.ToString(menor["Nome"]) +
                    " (" + menor["Unidades"] + ")";
            }
        }

        // ======================================
        // FILTROS DAS TABELAS
        // ======================================

        private void AplicarFiltros()
        {
            if (gridMovimentacoes == null ||
                txtPesquisa == null ||
                cmbTipo == null)
                return;

            string busca = txtPesquisa.Text.Trim();

            string tipoSelecionado =
                cmbTipo.SelectedIndex == 1 ? "E" :
                cmbTipo.SelectedIndex == 2 ? "S" : "";

            DataTable entradas = movimentos.Clone();
            DataTable saidas = movimentos.Clone();
            DataTable geral = movimentos.Clone();

            foreach (DataRow linha in movimentos.Rows)
            {
                string tipo =
                    Convert.ToString(linha["Tipo"]).Trim();

                string conteudo =
                    Convert.ToString(linha["Descricao"]) +
                    " " + Convert.ToString(
                        linha["Consulta"]) +
                    " " + Convert.ToString(
                        linha["ID_Movimentacao"]) +
                    " " + Convert.ToString(
                        linha["Origem"]);

                bool corresponde =
                    busca.Length == 0 ||
                    conteudo.IndexOf(
                        busca,
                        StringComparison
                            .CurrentCultureIgnoreCase) >= 0;

                if (!corresponde)
                    continue;

                if (tipo == "E")
                    entradas.ImportRow(linha);

                if (tipo == "S")
                    saidas.ImportRow(linha);

                if (tipoSelecionado == "" ||
                    tipo == tipoSelecionado)
                {
                    geral.ImportRow(linha);
                }
            }

            gridEntradas.DataSource = entradas;
            gridSaidas.DataSource = saidas;
            gridMovimentacoes.DataSource = geral;

            ConfigurarColunas(gridEntradas);
            ConfigurarColunas(gridSaidas);
            ConfigurarColunas(gridMovimentacoes);

            LimparSelecao(gridEntradas);
            LimparSelecao(gridSaidas);
            LimparSelecao(gridMovimentacoes);
        }

        private void LimparSelecao(
            DataGridView grade)
        {
            grade.ClearSelection();
            grade.CurrentCell = null;
        }

        private void ConfigurarColunas(
            DataGridView grade)
        {
            if (grade.Columns.Contains("ID_Movimentacao"))
            {
                grade.Columns["ID_Movimentacao"]
                    .HeaderText = "Código";
            }

            if (grade.Columns.Contains("DataHora"))
            {
                grade.Columns["DataHora"]
                    .HeaderText = "Data / Hora";

                grade.Columns["DataHora"]
                    .DefaultCellStyle.Format =
                        "dd/MM/yyyy HH:mm";
            }

            if (grade.Columns.Contains("Valor"))
            {
                grade.Columns["Valor"]
                    .DefaultCellStyle.Format = "C2";

                grade.Columns["Valor"]
                    .DefaultCellStyle.FormatProvider =
                        cultura;
            }

            if (grade.Columns.Contains("Descricao"))
            {
                grade.Columns["Descricao"]
                    .HeaderText = "Descrição";

                grade.Columns["Descricao"]
                    .AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
            }

            if (grade.Columns.Contains("IdOrigem"))
            {
                grade.Columns["IdOrigem"]
                    .HeaderText = "Referência";
            }

            if (grade.Columns.Contains("Tipo"))
            {
                grade.Columns["Tipo"]
                    .HeaderText = "Tipo";
            }
        }
    }
}
