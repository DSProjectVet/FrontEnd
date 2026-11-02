
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{
    public class frmConsultas : Form
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

        private readonly Color vermelho =
            Color.FromArgb(220, 53, 69);

        private DataTable todasConsultas = new DataTable();

        private DataGridView gridConsultas;
        private DataGridView gridServicos;

        private TextBox txtPesquisar;
        private TextBox txtDetalhes;

        private Label lblTotal;
        private Label lblSelecionada;
        private Label lblServicos;
        private Label lblRodape;

        private Button btnAtualizar;

        private bool carregando;
        private int? idSelecionado;
        private int versaoSelecao = 0;

        public frmConsultas()
        {
            ConfigurarJanela();
            ConstruirInterface();

            Shown += async (s, e) =>
            {
                await CarregarConsultasAsync();
            };
        }

        // ==========================================
        // JANELA
        // ==========================================

        private void ConfigurarJanela()
        {
            Text = "Med Vet - Consultas";
            BackColor = fundo;
            Font = new Font("Segoe UI", 10F);
            MinimumSize = new Size(850, 580);
        }

        // ==========================================
        // INTERFACE PRINCIPAL
        // ==========================================

        private void ConstruirInterface()
        {
            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(20);
            layout.ColumnCount = 1;
            layout.RowCount = 5;

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 85));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 90));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 65));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 80));

            Controls.Add(layout);

            // ======================================
            // CABEÇALHO
            // ======================================

            Panel cabecalho = new Panel();
            cabecalho.Dock = DockStyle.Fill;

            layout.Controls.Add(cabecalho, 0, 0);

            Label titulo = CriarLabel(
                "Consultas veterinárias", 22, true);

            titulo.SetBounds(0, 0, 490, 45);
            cabecalho.Controls.Add(titulo);

            Label subtitulo = CriarLabel(
                "Atendimentos e serviços da clínica.",
                10, false);

            subtitulo.ForeColor = cinza;
            subtitulo.SetBounds(2, 45, 490, 28);

            cabecalho.Controls.Add(subtitulo);

            FlowLayoutPanel botoesTopo =
                new FlowLayoutPanel();

            botoesTopo.Dock = DockStyle.Right;
            botoesTopo.Width = 340;
            botoesTopo.WrapContents = false;

            botoesTopo.FlowDirection =
                FlowDirection.RightToLeft;

            cabecalho.Controls.Add(botoesTopo);

            Button btnNovo = CriarBotao(
                "+ Nova consulta", verde, 160);

            btnAtualizar = CriarBotao(
                "Atualizar",
                Color.FromArgb(229, 238, 234),
                135,
                texto);

            botoesTopo.Controls.Add(btnNovo);
            botoesTopo.Controls.Add(btnAtualizar);

            btnNovo.Click += async (s, e) =>
            {
                await AbrirEditorAsync(null);
            };

            btnAtualizar.Click += async (s, e) =>
            {
                await CarregarConsultasAsync();
            };

            // ======================================
            // INDICADORES
            // ======================================

            TableLayoutPanel indicadores =
                new TableLayoutPanel();

            indicadores.Dock = DockStyle.Fill;
            indicadores.ColumnCount = 3;

            for (int i = 0; i < 3; i++)
            {
                indicadores.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent, 33.3333F));
            }

            indicadores.Controls.Add(
                CriarIndicador(
                    "Total de consultas", out lblTotal),
                0, 0);

            indicadores.Controls.Add(
                CriarIndicador(
                    "Consulta selecionada",
                    out lblSelecionada),
                1, 0);

            indicadores.Controls.Add(
                CriarIndicador(
                    "Serviços associados",
                    out lblServicos),
                2, 0);

            layout.Controls.Add(indicadores, 0, 1);

            // ======================================
            // PESQUISA
            // ======================================

            Panel painelPesquisa = new Panel();
            painelPesquisa.Dock = DockStyle.Fill;

            layout.Controls.Add(painelPesquisa, 0, 2);

            txtPesquisar = new TextBox();

            txtPesquisar.Font =
                new Font("Segoe UI", 11F);

            txtPesquisar.SetBounds(
                0, 12, 380, 35);

            painelPesquisa.Controls.Add(
                txtPesquisar);

            Label dica = CriarLabel(
                "Pesquisar por animal, responsável ou diagnóstico",
                9, false);

            dica.ForeColor = cinza;
            dica.SetBounds(396, 17, 390, 26);

            painelPesquisa.Controls.Add(dica);

            txtPesquisar.TextChanged += (s, e) =>
            {
                AplicarFiltro();
            };

            // ======================================
            // ÁREA PRINCIPAL
            // ======================================

            SplitContainer split = new SplitContainer();

            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Vertical;
            split.SplitterWidth = 7;
            split.Margin = new Padding(0);

            layout.Controls.Add(split, 0, 3);

            split.SizeChanged += (s, e) =>
            {
                if (split.Width > 650)
                {
                    int destino =
                        (int)(split.Width * 0.56);

                    if (split.SplitterDistance != destino)
                        split.SplitterDistance = destino;
                }
            };

            // TABELA DE CONSULTAS
            gridConsultas = CriarGrade();

            gridConsultas.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode
                    .DisplayedCells;

            split.Panel1.Controls.Add(gridConsultas);

            gridConsultas.SelectionChanged +=
                async (s, e) =>
                {
                    await SelecionarConsultaAsync();
                };

            gridConsultas.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    VisualizarConsulta();
            };

            // DETALHES + SERVIÇOS
            TableLayoutPanel lateral =
                new TableLayoutPanel();

            lateral.Dock = DockStyle.Fill;
            lateral.ColumnCount = 1;
            lateral.RowCount = 3;

            lateral.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 40));

            lateral.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50));

            lateral.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50));

            split.Panel2.Controls.Add(lateral);

            Label tituloDetalhes = CriarLabel(
                "Detalhes do atendimento", 13, true);

            tituloDetalhes.Dock = DockStyle.Fill;
            lateral.Controls.Add(
                tituloDetalhes, 0, 0);

            txtDetalhes = new TextBox();

            txtDetalhes.Dock = DockStyle.Fill;
            txtDetalhes.Multiline = true;
            txtDetalhes.ReadOnly = true;

            txtDetalhes.ScrollBars =
                ScrollBars.Vertical;

            txtDetalhes.BackColor = Color.White;
            txtDetalhes.BorderStyle =
                BorderStyle.FixedSingle;

            txtDetalhes.Font =
                new Font("Segoe UI", 10F);

            txtDetalhes.Text =
                "Selecione uma consulta para visualizar.";

            lateral.Controls.Add(txtDetalhes, 0, 1);

            gridServicos = CriarGrade();

            gridServicos.Margin =
                new Padding(0, 10, 0, 0);

            lateral.Controls.Add(gridServicos, 0, 2);

            // ======================================
            // RODAPÉ
            // ======================================

            TableLayoutPanel rodape =
                new TableLayoutPanel();

            rodape.Dock = DockStyle.Fill;
            rodape.ColumnCount = 2;

            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100));

            rodape.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.AutoSize));

            layout.Controls.Add(rodape, 0, 4);

            lblRodape = CriarLabel(
                "Aguardando dados...", 9, false);

            lblRodape.ForeColor = cinza;
            lblRodape.Dock = DockStyle.Fill;

            rodape.Controls.Add(lblRodape, 0, 0);

            FlowLayoutPanel acoes =
                new FlowLayoutPanel();

            acoes.Dock = DockStyle.Fill;
            acoes.AutoSize = true;
            acoes.WrapContents = false;

            rodape.Controls.Add(acoes, 1, 0);

            Button btnVisualizar = CriarBotao(
                "Visualizar",
                Color.FromArgb(229, 238, 234),
                95, texto);

            Button btnEditar = CriarBotao(
                "Editar", verde, 90);

            Button btnExcluir = CriarBotao(
                "Excluir", vermelho, 90);

            Button btnServico = CriarBotao(
                "+ Serviço",
                Color.FromArgb(229, 238, 234),
                105, texto);

            Button btnReceita = CriarBotao(
                "Receita / consumo",
                verdeEscuro, 160);

            acoes.Controls.Add(btnVisualizar);
            acoes.Controls.Add(btnEditar);
            acoes.Controls.Add(btnExcluir);
            acoes.Controls.Add(btnServico);
            acoes.Controls.Add(btnReceita);

            btnVisualizar.Click += (s, e) =>
                VisualizarConsulta();

            btnEditar.Click += async (s, e) =>
            {
                ConsultaDados atual =
                    ObterConsultaSelecionada();

                if (atual != null)
                    await AbrirEditorAsync(atual);
            };

            btnExcluir.Click += async (s, e) =>
            {
                await ExcluirConsultaAsync();
            };


            btnServico.Click += async (s, e) =>
            {
                ConsultaDados consulta =
                    ObterConsultaSelecionada();

                if (consulta == null)
                {
                    MessageBox.Show(
                        this,
                        "Selecione uma consulta primeiro.",
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                using (frmReceitaConsulta tela =
                    new frmReceitaConsulta(consulta.Id, true))
                {
                    tela.ShowDialog(this);
                }

                idSelecionado = null;

                await SelecionarConsultaAsync();
            };



            btnReceita.Click += async (s, e) =>
            {
                ConsultaDados consulta =
                    ObterConsultaSelecionada();

                if (consulta == null)
                {
                    MessageBox.Show(
                        this,
                        "Selecione uma consulta primeiro.",
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                using (frmReceitaConsulta receita =
                    new frmReceitaConsulta(consulta.Id))
                {
                    receita.ShowDialog(this);
                }

                // Atualiza as informações de serviços
                // depois que a janela for fechada.
                idSelecionado = null;

                await SelecionarConsultaAsync();
            };



        }

        // ==========================================
        // COMPONENTES VISUAIS
        // ==========================================

        private Label CriarLabel(
            string valor, float tamanho, bool negrito)
        {
            return new Label
            {
                Text = valor,
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
            string nome,
            Color cor,
            int largura,
            Color? corTexto = null)
        {
            Button botao = new Button();

            botao.Text = nome;
            botao.Size = new Size(largura, 42);

            botao.Margin =
                new Padding(4, 10, 4, 4);

            botao.BackColor = cor;
            botao.ForeColor =
                corTexto ?? Color.White;

            botao.UseVisualStyleBackColor = false;
            botao.FlatStyle = FlatStyle.Flat;

            botao.FlatAppearance.BorderSize = 0;

            botao.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            botao.Cursor = Cursors.Hand;

            return botao;
        }

        private Panel CriarIndicador(
            string titulo, out Label valor)
        {
            Panel painel = new Panel();

            painel.Dock = DockStyle.Fill;
            painel.BackColor = Color.White;

            painel.Margin =
                new Padding(0, 0, 12, 10);

            Label descricao = CriarLabel(
                titulo, 9, false);

            descricao.ForeColor = cinza;
            descricao.SetBounds(15, 8, 260, 25);

            painel.Controls.Add(descricao);

            valor = CriarLabel("—", 20, true);

            valor.ForeColor = verdeEscuro;
            valor.SetBounds(15, 34, 245, 43);

            painel.Controls.Add(valor);

            return painel;
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

            grade.RowTemplate.Height = 39;

            grade.ColumnHeadersHeight = 42;

            grade.EnableHeadersVisualStyles = false;

            grade.ColumnHeadersDefaultCellStyle.BackColor =
                verdeEscuro;

            grade.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.Black;

            grade.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            grade.DefaultCellStyle.ForeColor = Color.Black;

            grade.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            grade.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(225, 248, 240);

            grade.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            grade.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 252, 250);

            return grade;
        }

        // ==========================================
        // CONEXÃO SQL
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

                if (atributo == null ||
                    atributo.SpecialSetting !=
                    SpecialSetting.ConnectionString)
                    continue;

                string valor = Convert.ToString(
                    Properties.Settings.Default[
                        propriedade.Name]);

                if (!string.IsNullOrWhiteSpace(valor))
                    return valor;
            }

            throw new Exception(
                "A connection string não foi localizada " +
                "em Properties.Settings.");
        }

        private DataTable ExecutarTabela(
            string sql, int? id = null)
        {
            DataTable tabela = new DataTable();

            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(comando))
            {
                if (id.HasValue)
                {
                    comando.Parameters.Add(
                        "@ID", SqlDbType.Int)
                        .Value = id.Value;
                }

                comando.CommandTimeout = 30;

                adapter.Fill(tabela);
            }

            return tabela;
        }

        // ==========================================
        // LISTAR CONSULTAS
        // ==========================================

        private async Task CarregarConsultasAsync()
        {
            if (carregando)
                return;

            carregando = true;
            btnAtualizar.Enabled = false;

            lblRodape.Text =
                "Consultando registros no SQL Server...";

            try
            {
                string sql = @"
                    SELECT
                        c.ID_Consulta,
                        c.DataHora,
                        a.Nome AS Animal,
                        f.Nome AS Responsavel,
                        c.Diagnostico,
                        c.Observacoes,
                        c.fk_Animais_ID_Animal,
                        c.fk_Funcionarios_ID_Funcionario
                    FROM dbo.Consultas AS c

                    LEFT JOIN dbo.Animais AS a
                        ON a.ID_Animal =
                           c.fk_Animais_ID_Animal

                    LEFT JOIN dbo.Funcionarios AS f
                        ON f.ID_Funcionario =
                           c.fk_Funcionarios_ID_Funcionario

                    ORDER BY
                        c.DataHora DESC,
                        c.ID_Consulta DESC;
                ";

                todasConsultas = await Task.Run(
                    () => ExecutarTabela(sql));

                AplicarFiltro();

                lblTotal.Text =
                    todasConsultas.Rows.Count.ToString();

                lblRodape.Text =
                    "Consultas carregadas com sucesso.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Erro ao carregar consultas:\n\n" +
                    ex.Message,
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblRodape.Text =
                    "Não foi possível consultar o banco.";
            }
            finally
            {
                btnAtualizar.Enabled = true;
                carregando = false;
            }
        }

        // ==========================================
        // FILTRO
        // ==========================================

        private void AplicarFiltro()
        {
            if (gridConsultas == null ||
                txtPesquisar == null)
                return;

            string busca =
                txtPesquisar.Text.Trim();

            DataTable filtrada =
                todasConsultas.Clone();

            foreach (DataRow linha in
                todasConsultas.Rows)
            {
                bool encontrou = busca.Length == 0;

                if (!encontrou)
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
                            encontrou = true;
                            break;
                        }
                    }
                }

                if (encontrou)
                    filtrada.ImportRow(linha);
            }

            versaoSelecao++;
            carregando = true;

            try
            {
                gridConsultas.DataSource = filtrada;

                if (gridConsultas.Columns.Contains(
                    "fk_Animais_ID_Animal"))
                {
                    gridConsultas.Columns[
                        "fk_Animais_ID_Animal"].Visible = false;
                }

                if (gridConsultas.Columns.Contains(
                    "fk_Funcionarios_ID_Funcionario"))
                {
                    gridConsultas.Columns[
                        "fk_Funcionarios_ID_Funcionario"]
                        .Visible = false;
                }

                if (gridConsultas.Columns.Contains(
                    "ID_Consulta"))
                {
                    gridConsultas.Columns[
                        "ID_Consulta"].HeaderText = "ID";
                }

                if (gridConsultas.Columns.Contains(
                    "DataHora"))
                {
                    gridConsultas.Columns[
                        "DataHora"].HeaderText =
                        "Data / Hora";

                    gridConsultas.Columns[
                        "DataHora"].DefaultCellStyle.Format =
                        "dd/MM/yyyy HH:mm";
                }

                if (gridConsultas.Columns.Contains(
                    "Responsavel"))
                {
                    gridConsultas.Columns[
                        "Responsavel"].HeaderText =
                        "Responsável";
                }

                gridConsultas.ClearSelection();
                gridConsultas.CurrentCell = null;

                idSelecionado = null;
                lblSelecionada.Text = "—";
                lblServicos.Text = "—";

                txtDetalhes.Text =
                    "Selecione uma consulta.";

                gridServicos.DataSource = null;
            }
            finally
            {
                carregando = false;
            }
        }

        // ==========================================
        // CONSULTA SELECIONADA
        // ==========================================

        private ConsultaDados ObterConsultaSelecionada()
        {
            if (gridConsultas.CurrentRow == null)
                return null;

            DataRowView linha =
                gridConsultas.CurrentRow.DataBoundItem
                as DataRowView;

            if (linha == null)
                return null;

            return new ConsultaDados
            {
                Id = Convert.ToInt32(
                    linha["ID_Consulta"]),

                DataHora = Convert.ToDateTime(
                    linha["DataHora"]),

                Diagnostico = Convert.ToString(
                    linha["Diagnostico"]),

                Observacoes = Convert.ToString(
                    linha["Observacoes"]),

                IdAnimal = Convert.ToInt32(
                    linha["fk_Animais_ID_Animal"]),

                IdFuncionario = Convert.ToInt32(
                    linha["fk_Funcionarios_ID_Funcionario"]),

                NomeAnimal = Convert.ToString(
                    linha["Animal"]),

                NomeFuncionario = Convert.ToString(
                    linha["Responsavel"])
            };
        }

        private async Task SelecionarConsultaAsync()
        {
            if (carregando)
                return;

            ConsultaDados consulta =
                ObterConsultaSelecionada();

            if (consulta == null)
                return;

            int id = consulta.Id;
            int versao = ++versaoSelecao;

            idSelecionado = id;

            lblSelecionada.Text =
                "#" + id.ToString("D4");

            txtDetalhes.Text =
                TextoDetalhes(consulta);

            lblServicos.Text = "…";

            try
            {
                string sql = @"
                    SELECT
                        fk_Servicos_ID_Servico
                            AS [ID Serviço],
                        Quantidade,
                        Preco AS [Preço]
                    FROM dbo.Consulta_Servico
                    WHERE fk_Consultas_ID_Consulta = @ID;
                ";

                DataTable servicos = await Task.Run(
                    () => ExecutarTabela(sql, id));

                if (versao != versaoSelecao ||
                    idSelecionado != id)
                    return;

                gridServicos.DataSource = servicos;

                lblServicos.Text =
                    servicos.Rows.Count.ToString();

                if (gridServicos.Columns.Contains("Preço"))
                {
                    gridServicos.Columns["Preço"]
                        .DefaultCellStyle.Format = "C2";
                }
            }
            catch (Exception ex)
            {
                if (versao != versaoSelecao)
                    return;

                lblServicos.Text = "—";

                lblRodape.Text =
                    "Erro ao carregar serviços: " +
                    ex.Message;
            }
        }

        private string TextoDetalhes(ConsultaDados c)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(
                "CONSULTA #" + c.Id.ToString("D4"));

            sb.AppendLine();

            sb.AppendLine(
                "Animal: " + c.NomeAnimal);

            sb.AppendLine(
                "Responsável: " + c.NomeFuncionario);

            sb.AppendLine(
                "Data: " +
                c.DataHora.ToString("dd/MM/yyyy HH:mm"));

            sb.AppendLine();

            sb.AppendLine("DIAGNÓSTICO");
            sb.AppendLine(
                string.IsNullOrWhiteSpace(c.Diagnostico)
                    ? "Não informado."
                    : c.Diagnostico);

            sb.AppendLine();
            sb.AppendLine("OBSERVAÇÕES");

            sb.AppendLine(
                string.IsNullOrWhiteSpace(c.Observacoes)
                    ? "Não informadas."
                    : c.Observacoes);

            return sb.ToString();
        }

        private void VisualizarConsulta()
        {
            ConsultaDados c = ObterConsultaSelecionada();

            if (c == null)
            {
                MessageBox.Show(
                    this,
                    "Selecione uma consulta.",
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            Form janela = new Form();

            janela.Text =
                "Consulta #" + c.Id.ToString("D4");

            janela.ClientSize = new Size(620, 450);
            janela.StartPosition =
                FormStartPosition.CenterParent;

            janela.BackColor = fundo;

            TextBox visualizacao = new TextBox();

            visualizacao.Dock = DockStyle.Fill;

            visualizacao.Margin =
                new Padding(20);

            visualizacao.Multiline = true;
            visualizacao.ReadOnly = true;

            visualizacao.ScrollBars =
                ScrollBars.Vertical;

            visualizacao.Font =
                new Font("Segoe UI", 11F);

            visualizacao.Text = TextoDetalhes(c);

            Panel painel = new Panel();

            painel.Dock = DockStyle.Fill;
            painel.Padding = new Padding(20);

            painel.Controls.Add(visualizacao);
            janela.Controls.Add(painel);

            using (janela)
            {
                janela.ShowDialog(this);
            }
        }

        // ==========================================
        // CADASTRAR / EDITAR
        // ==========================================

        private async Task AbrirEditorAsync(
            ConsultaDados existente)
        {
            try
            {
                // Carrega as opções reais do banco.
                string sqlAnimais = @"
                    SELECT
                        ID_Animal,
                        CAST(ID_Animal AS VARCHAR(15)) +
                        ' - ' + ISNULL(Nome, '')
                            AS Exibicao
                    FROM dbo.Animais
                    ORDER BY Exibicao;
                ";


                string sqlFuncionarios = @"
                    SELECT
                        ID_Funcionario,
                        Nome,
                        CRMV,
                        CAST(ID_Funcionario AS VARCHAR(15)) +
                        ' - ' + ISNULL(Nome, '') +
                        CASE
                            WHEN CRMV IS NOT NULL
                                 AND LTRIM(RTRIM(CRMV)) <> ''
                            THEN ' | CRMV: ' + CRMV
                            ELSE ''
                        END AS Exibicao
                    FROM dbo.Funcionarios
                    WHERE NivelAcesso = '3'
                    ORDER BY Nome;
                ";
                

                DataTable animais = await Task.Run(
                    () => ExecutarTabela(sqlAnimais));

                DataTable funcionarios = await Task.Run(
                    () => ExecutarTabela(sqlFuncionarios));

                using (EditorConsulta editor =
                    new EditorConsulta(
                        animais, funcionarios, existente))
                {
                    if (editor.ShowDialog(this) !=
                        DialogResult.OK)
                        return;

                    ConsultaDados dados = editor.Resultado;

                    int idGerado = await Task.Run(
                        () => SalvarConsultaNoBanco(
                            dados,
                            existente == null));

                    MessageBox.Show(
                        this,
                        (existente == null
                            ? "Consulta cadastrada!"
                            : "Consulta atualizada!") +
                        "\n\nCódigo: #" + idGerado,
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await CarregarConsultasAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível salvar a consulta.\n\n" +
                    ex.Message,
                    "Erro SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private int SalvarConsultaNoBanco(
            ConsultaDados c, bool nova)
        {
            string sql;

            if (nova)
            {
                sql = @"
                    INSERT INTO dbo.Consultas
                    (
                        Diagnostico,
                        DataHora,
                        Observacoes,
                        fk_Animais_ID_Animal,
                        fk_Funcionarios_ID_Funcionario
                    )
                    OUTPUT INSERTED.ID_Consulta
                    VALUES
                    (
                        @Diagnostico,
                        @DataHora,
                        @Observacoes,
                        @Animal,
                        @Funcionario
                    );
                ";
            }
            else
            {
                sql = @"
                    UPDATE dbo.Consultas
                    SET
                        Diagnostico = @Diagnostico,
                        DataHora = @DataHora,
                        Observacoes = @Observacoes,
                        fk_Animais_ID_Animal = @Animal,
                        fk_Funcionarios_ID_Funcionario =
                            @Funcionario
                    WHERE ID_Consulta = @ID;
                ";
            }

            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            {
                comando.Parameters.Add(
                    "@Diagnostico",
                    SqlDbType.NVarChar, -1)
                    .Value =
                    (object)c.Diagnostico ?? "";

                comando.Parameters.Add(
                    "@Observacoes",
                    SqlDbType.NVarChar, -1)
                    .Value =
                    (object)c.Observacoes ?? "";

                comando.Parameters.Add(
                    "@DataHora",
                    SqlDbType.DateTime2)
                    .Value = c.DataHora;

                comando.Parameters.Add(
                    "@Animal",
                    SqlDbType.Int)
                    .Value = c.IdAnimal;

                comando.Parameters.Add(
                    "@Funcionario",
                    SqlDbType.Int)
                    .Value = c.IdFuncionario;

                if (!nova)
                {
                    comando.Parameters.Add(
                        "@ID", SqlDbType.Int)
                        .Value = c.Id;
                }

                conexao.Open();

                if (nova)
                {
                    object valor = comando.ExecuteScalar();

                    if (valor == null ||
                        valor == DBNull.Value)
                        throw new Exception(
                            "O banco não retornou " +
                            "o ID da consulta.");

                    return Convert.ToInt32(valor);
                }

                int linhas = comando.ExecuteNonQuery();

                if (linhas != 1)
                    throw new Exception(
                        "A consulta não foi encontrada.");

                return c.Id;
            }
        }

        // ==========================================
        // EXCLUIR CONSULTA
        // ==========================================

        // Vínculos são uma condição esperada, não um erro fatal.
        private enum ResultadoExclusao
        {
            Excluida,
            PossuiVinculos,
            NaoEncontrada
        }

        private async Task ExcluirConsultaAsync()
        {
            ConsultaDados consulta = ObterConsultaSelecionada();

            if (consulta == null)
            {
                MessageBox.Show(
                    this,
                    "Selecione uma consulta para excluir.",
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmar = MessageBox.Show(
                this,
                "Deseja excluir a consulta #" + consulta.Id + "?\n\n" +
                "Animal: " + consulta.NomeAnimal + "\n" +
                "Data: " + consulta.DataHora.ToString("dd/MM/yyyy HH:mm") +
                "\n\nA exclusão não poderá ser desfeita.",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirmar != DialogResult.Yes)
                return;

            try
            {
                lblRodape.Text = "Verificando vínculos da consulta...";

                ResultadoExclusao resultado = await Task.Run(
                    () => ExcluirConsultaNoBanco(consulta.Id));

                if (resultado == ResultadoExclusao.PossuiVinculos)
                {
                    MessageBox.Show(
                        this,
                        "A consulta #" + consulta.Id +
                        " possui registros vinculados, como serviços, " +
                        "produtos, prescrições ou movimentações financeiras.\n\n" +
                        "Por segurança, ela NÃO foi excluída. " +
                        "Seus registros permanecem intactos.",
                        "Exclusão não permitida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    lblRodape.Text =
                        "Exclusão bloqueada: consulta possui vínculos.";
                    return;
                }

                if (resultado == ResultadoExclusao.NaoEncontrada)
                {
                    MessageBox.Show(
                        this,
                        "Esta consulta não foi encontrada no banco. " +
                        "Ela pode ter sido removida por outro funcionário.",
                        "Consulta não encontrada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await CarregarConsultasAsync();
                    return;
                }

                MessageBox.Show(
                    this,
                    "Consulta excluída com sucesso.",
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await CarregarConsultasAsync();
            }
            catch (SqlException ex)
            {
                // Uma FK de outra tabela não prevista na verificação
                // também pode impedir a exclusão. Tratar sem encerrar a tela.
                if (ex.Number == 547)
                {
                    MessageBox.Show(
                        this,
                        "A consulta possui registros vinculados " +
                        "e não pode ser excluída.\n\n" +
                        "Nenhum registro foi apagado.",
                        "Exclusão não permitida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    lblRodape.Text =
                        "Exclusão bloqueada por vínculo no banco.";
                    return;
                }

                MessageBox.Show(
                    this,
                    "Não foi possível excluir a consulta.\n\n" +
                    ex.Message,
                    "Erro no SQL Server",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblRodape.Text = "Falha ao excluir consulta.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível excluir a consulta.\n\n" +
                    ex.Message,
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblRodape.Text = "Falha ao excluir consulta.";
            }
        }

        private ResultadoExclusao ExcluirConsultaNoBanco(int id)
        {
            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            {
                conexao.Open();

                using (SqlTransaction transacao =
                    conexao.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        // Bloqueia a consulta durante a checagem e exclusão,
                        // evitando alterações concorrentes entre as etapas.
                        using (SqlCommand localizar = new SqlCommand(
                            @"SELECT ID_Consulta
                              FROM dbo.Consultas WITH (UPDLOCK, HOLDLOCK)
                              WHERE ID_Consulta = @ID;",
                            conexao, transacao))
                        {
                            localizar.Parameters.Add(
                                "@ID", SqlDbType.Int).Value = id;

                            if (localizar.ExecuteScalar() == null)
                            {
                                transacao.Rollback();
                                return ResultadoExclusao.NaoEncontrada;
                            }
                        }

                        // Identifica TODOS os registros vinculados por meio da
                        // coluna fk_Consultas_ID_Consulta. Isso inclui serviços,
                        // produtos, prescrições, movimentações financeiras e
                        // futuras tabelas que utilizem esse mesmo vínculo.
                        // Os nomes são obtidos do catálogo do SQL Server e
                        // protegidos por QUOTENAME (não vêm de texto do usuário).
                        const string sqlTabelasVinculadas = @"
                            SELECT
                                QUOTENAME(s.name) + '.' + QUOTENAME(t.name)
                                    AS NomeTabela
                            FROM sys.tables AS t
                            INNER JOIN sys.schemas AS s
                                ON s.schema_id = t.schema_id
                            INNER JOIN sys.columns AS c
                                ON c.object_id = t.object_id
                            WHERE c.name = N'fk_Consultas_ID_Consulta'
                              AND t.object_id <> OBJECT_ID(N'dbo.Consultas');";

                        DataTable tabelasVinculadas = new DataTable();

                        using (SqlCommand descobrir = new SqlCommand(
                            sqlTabelasVinculadas, conexao, transacao))
                        using (SqlDataAdapter adaptador =
                            new SqlDataAdapter(descobrir))
                        {
                            adaptador.Fill(tabelasVinculadas);
                        }

                        foreach (DataRow tabela in tabelasVinculadas.Rows)
                        {
                            string nomeTabela = Convert.ToString(
                                tabela["NomeTabela"]);

                            string sqlVerificar =
                                "SELECT TOP (1) 1 FROM " + nomeTabela +
                                " WITH (HOLDLOCK) " +
                                "WHERE [fk_Consultas_ID_Consulta] = @ID;";

                            using (SqlCommand verificar = new SqlCommand(
                                sqlVerificar, conexao, transacao))
                            {
                                verificar.Parameters.Add(
                                    "@ID", SqlDbType.Int).Value = id;

                                if (verificar.ExecuteScalar() != null)
                                {
                                    transacao.Rollback();
                                    return ResultadoExclusao.PossuiVinculos;
                                }
                            }
                        }

                        using (SqlCommand excluir = new SqlCommand(
                            @"DELETE FROM dbo.Consultas
                              WHERE ID_Consulta = @ID;",
                            conexao, transacao))
                        {
                            excluir.Parameters.Add(
                                "@ID", SqlDbType.Int).Value = id;

                            if (excluir.ExecuteNonQuery() != 1)
                            {
                                transacao.Rollback();
                                return ResultadoExclusao.NaoEncontrada;
                            }
                        }

                        transacao.Commit();
                        return ResultadoExclusao.Excluida;
                    }
                    catch (SqlException ex)
                    {
                        try { transacao.Rollback(); }
                        catch { }

                        // Outros relacionamentos do banco, por exemplo
                        // movimentações financeiras, também ficam protegidos.
                        if (ex.Number == 547)
                            return ResultadoExclusao.PossuiVinculos;

                        throw;
                    }
                    catch
                    {
                        try { transacao.Rollback(); }
                        catch { }
                        throw;
                    }
                }
            }
        }

        // ==========================================
        // SERVIÇOS DA CONSULTA
        // ==========================================

        private async Task AdicionarServicoAsync()
        {
            ConsultaDados consulta =
                ObterConsultaSelecionada();

            if (consulta == null)
            {
                MessageBox.Show(
                    this,
                    "Selecione uma consulta.",
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            using (Form janela = new Form())
            {
                janela.Text = "Adicionar serviço";

                janela.ClientSize =
                    new Size(440, 365);

                janela.StartPosition =
                    FormStartPosition.CenterParent;

                janela.FormBorderStyle =
                    FormBorderStyle.FixedDialog;

                janela.MaximizeBox = false;
                janela.MinimizeBox = false;
                janela.BackColor = fundo;

                Label titulo = CriarLabel(
                    "Serviço da consulta #" +
                    consulta.Id, 15, true);

                titulo.SetBounds(22, 15, 390, 40);
                janela.Controls.Add(titulo);

                NumericUpDown numServico =
                    CampoNumerico(
                        janela, "ID do serviço", 75);

                numServico.Minimum = 1;

                NumericUpDown numQuantidade =
                    CampoNumerico(
                        janela, "Quantidade", 140);

                numQuantidade.Minimum = 1;

                NumericUpDown numPreco =
                    CampoNumerico(
                        janela, "Preço (R$)", 205);

                numPreco.DecimalPlaces = 2;

                Button btnSalvar = CriarBotao(
                    "Adicionar serviço", verde, 180);

                btnSalvar.SetBounds(
                    234, 290, 180, 43);

                janela.Controls.Add(btnSalvar);

                btnSalvar.Click += (s, e) =>
                {
                    janela.DialogResult =
                        DialogResult.OK;
                    janela.Close();
                };

                if (janela.ShowDialog(this) !=
                    DialogResult.OK)
                    return;

                int idServico =
                    Decimal.ToInt32(numServico.Value);

                int quantidade =
                    Decimal.ToInt32(numQuantidade.Value);

                decimal preco = numPreco.Value;

                try
                {
                    await Task.Run(() =>
                    {
                        InserirServicoNoBanco(
                            consulta.Id, idServico,
                            quantidade, preco);
                    });

                    MessageBox.Show(
                        this,
                        "Serviço registrado com sucesso.",
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Atualiza os serviços da
                    // consulta selecionada.
                    idSelecionado = null;

                    await SelecionarConsultaAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this,
                        "Erro ao incluir serviço:\n\n" +
                        ex.Message,
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private NumericUpDown CampoNumerico(
            Form janela, string titulo, int y)
        {
            Label label = CriarLabel(
                titulo, 9, true);

            label.SetBounds(24, y, 390, 23);
            janela.Controls.Add(label);

            NumericUpDown numero =
                new NumericUpDown();

            numero.Minimum = 0;
            numero.Maximum = 100000000;

            numero.Font =
                new Font("Segoe UI", 11F);

            numero.SetBounds(
                24, y + 27, 390, 31);

            janela.Controls.Add(numero);

            return numero;
        }

        private void InserirServicoNoBanco(
            int idConsulta,
            int idServico,
            int quantidade,
            decimal preco)
        {
            string sql = @"
                INSERT INTO dbo.Consulta_Servico
                (
                    fk_Servicos_ID_Servico,
                    fk_Consultas_ID_Consulta,
                    Quantidade,
                    Preco
                )
                VALUES
                (
                    @Servico, @Consulta,
                    @Quantidade, @Preco
                );
            ";

            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            {
                comando.Parameters.Add(
                    "@Servico", SqlDbType.Int)
                    .Value = idServico;

                comando.Parameters.Add(
                    "@Consulta", SqlDbType.Int)
                    .Value = idConsulta;

                comando.Parameters.Add(
                    "@Quantidade", SqlDbType.Int)
                    .Value = quantidade;

                SqlParameter parametroPreco =
                    comando.Parameters.Add(
                        "@Preco", SqlDbType.Decimal);

                parametroPreco.Precision = 18;
                parametroPreco.Scale = 2;
                parametroPreco.Value = preco;

                conexao.Open();
                comando.ExecuteNonQuery();
            }
        }

        // ==========================================
        // MODELO DOS DADOS
        // ==========================================

        private class ConsultaDados
        {
            public int Id { get; set; }

            public string Diagnostico { get; set; }
            public string Observacoes { get; set; }

            public DateTime DataHora { get; set; }

            public int IdAnimal { get; set; }
            public int IdFuncionario { get; set; }

            public string NomeAnimal { get; set; }
            public string NomeFuncionario { get; set; }
        }

        // ==========================================
        // JANELA DE CADASTRO E EDIÇÃO
        // ==========================================

        private class EditorConsulta : Form
        {
            private ComboBox cmbAnimal;
            private ComboBox cmbFuncionario;

            private DateTimePicker dtData;

            private TextBox txtDiagnostico;
            private TextBox txtObservacoes;

            private readonly ConsultaDados original;

            public ConsultaDados Resultado
            {
                get;
                private set;
            }

            public EditorConsulta(
                DataTable animais,
                DataTable funcionarios,
                ConsultaDados existente)
            {
                original = existente;

                Text = existente == null
                    ? "Med Vet - Nova Consulta"
                    : "Med Vet - Editar Consulta";

                ClientSize = new Size(640, 675);

                StartPosition =
                    FormStartPosition.CenterParent;

                FormBorderStyle =
                    FormBorderStyle.FixedDialog;

                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;

                BackColor =
                    Color.FromArgb(248, 250, 249);

                Font = new Font("Segoe UI", 10F);

                Construir(animasTabela: animais,
                    funcionariosTabela: funcionarios);

                if (existente != null)
                    Preencher(existente);
            }

            private void Construir(
                DataTable animasTabela,
                DataTable funcionariosTabela)
            {
                TableLayoutPanel layout =
                    new TableLayoutPanel();

                layout.Dock = DockStyle.Fill;
                layout.Padding = new Padding(25);

                layout.ColumnCount = 1;
                layout.RowCount = 7;

                layout.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent, 100));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 70));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 72));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 72));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 72));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 115));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Percent, 100));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 68));

                Controls.Add(layout);

                Label titulo = new Label();

                titulo.Text = original == null
                    ? "Nova consulta"
                    : "Editar consulta #" + original.Id;

                titulo.Font = new Font(
                    "Segoe UI", 21F, FontStyle.Bold);

                titulo.ForeColor =
                    Color.FromArgb(35, 49, 45);

                titulo.Dock = DockStyle.Fill;

                layout.Controls.Add(
                    titulo, 0, 0);

                // ANIMAL
                cmbAnimal = new ComboBox();

                cmbAnimal.DropDownStyle =
                    ComboBoxStyle.DropDownList;

                cmbAnimal.DisplayMember = "Exibicao";
                cmbAnimal.ValueMember = "ID_Animal";
                cmbAnimal.DataSource = animasTabela;

                cmbAnimal.SelectedIndex = -1;

                AdicionarCampo(
                    layout, "Animal atendido *",
                    cmbAnimal, 1);

                // FUNCIONÁRIO
                cmbFuncionario = new ComboBox();

                cmbFuncionario.DropDownStyle =
                    ComboBoxStyle.DropDownList;


                // ==========================================
                // VETERINÁRIO RESPONSÁVEL
                // ==========================================

                cmbFuncionario = new ComboBox();

                cmbFuncionario.DropDownStyle =
                    ComboBoxStyle.DropDownList;

                cmbFuncionario.Font =
                    new Font("Segoe UI", 10F);

                // Exibe nome e CRMV do veterinário
                cmbFuncionario.DisplayMember = "Exibicao";

                // Armazena o ID real do funcionário
                cmbFuncionario.ValueMember = "ID_Funcionario";

                // A tabela já foi filtrada no SQL
                cmbFuncionario.DataSource = funcionariosTabela;

                cmbFuncionario.SelectedIndex = -1;

                AdicionarCampo(
                    layout,
                    "Veterinário responsável *",
                    cmbFuncionario,
                    2);


                // DATA E HORA
                dtData = new DateTimePicker();

                dtData.Format =
                    DateTimePickerFormat.Custom;

                dtData.CustomFormat =
                    "dd/MM/yyyy HH:mm";

                dtData.ShowUpDown = true;
                dtData.Value = DateTime.Now;

                AdicionarCampo(
                    layout, "Data e horário *",
                    dtData, 3);

                // DIAGNÓSTICO
                txtDiagnostico = new TextBox();

                txtDiagnostico.Multiline = true;
                txtDiagnostico.ScrollBars =
                    ScrollBars.Vertical;

                AdicionarCampo(
                    layout, "Diagnóstico",
                    txtDiagnostico, 4);

                // OBSERVAÇÕES
                txtObservacoes = new TextBox();

                txtObservacoes.Multiline = true;
                txtObservacoes.ScrollBars =
                    ScrollBars.Vertical;

                AdicionarCampo(
                    layout, "Observações clínicas",
                    txtObservacoes, 5);

                // BOTÕES
                FlowLayoutPanel rodape =
                    new FlowLayoutPanel();

                rodape.FlowDirection =
                    FlowDirection.RightToLeft;

                rodape.WrapContents = false;
                rodape.Dock = DockStyle.Fill;

                layout.Controls.Add(rodape, 0, 6);

                Button btnSalvar = new Button();

                btnSalvar.Text = original == null
                    ? "Cadastrar consulta"
                    : "Salvar alterações";

                btnSalvar.Size =
                    new Size(180, 44);

                btnSalvar.BackColor =
                    Color.FromArgb(20, 192, 138);

                btnSalvar.ForeColor = Color.White;

                btnSalvar.FlatStyle =
                    FlatStyle.Flat;

                btnSalvar.FlatAppearance.BorderSize = 0;

                btnSalvar.Font = new Font(
                    "Segoe UI", 10F, FontStyle.Bold);

                Button btnCancelar = new Button();

                btnCancelar.Text = "Cancelar";
                btnCancelar.Size =
                    new Size(140, 44);

                btnCancelar.BackColor =
                    Color.FromArgb(231, 240, 236);

                btnCancelar.FlatStyle =
                    FlatStyle.Flat;

                btnCancelar.FlatAppearance.BorderSize = 0;

                rodape.Controls.Add(btnSalvar);
                rodape.Controls.Add(btnCancelar);

                btnSalvar.Click += (s, e) =>
                    Confirmar();

                btnCancelar.Click += (s, e) =>
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                };

                AcceptButton = btnSalvar;
                CancelButton = btnCancelar;
            }

            private void AdicionarCampo(
                TableLayoutPanel layout,
                string titulo,
                Control controle,
                int linha)
            {
                TableLayoutPanel grupo =
                    new TableLayoutPanel();

                grupo.Dock = DockStyle.Fill;
                grupo.ColumnCount = 1;
                grupo.RowCount = 2;

                grupo.Margin =
                    new Padding(0, 0, 0, 8);

                grupo.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 27));

                grupo.RowStyles.Add(
                    new RowStyle(
                        SizeType.Percent, 100));

                Label label = new Label();

                label.Text = titulo;
                label.Dock = DockStyle.Fill;

                label.ForeColor =
                    Color.FromArgb(35, 49, 45);

                label.Font = new Font(
                    "Segoe UI", 9F, FontStyle.Bold);

                grupo.Controls.Add(label, 0, 0);

                controle.Dock = DockStyle.Fill;

                controle.Font =
                    new Font("Segoe UI", 10F);

                grupo.Controls.Add(
                    controle, 0, 1);

                layout.Controls.Add(
                    grupo, 0, linha);
            }

            private void Preencher(ConsultaDados c)
            {
                cmbAnimal.SelectedValue =
                    c.IdAnimal;

                cmbFuncionario.SelectedValue =
                    c.IdFuncionario;

                dtData.Value = c.DataHora;

                txtDiagnostico.Text =
                    c.Diagnostico ?? "";

                txtObservacoes.Text =
                    c.Observacoes ?? "";
            }

            private void Confirmar()
            {
                if (cmbAnimal.SelectedValue == null)
                {
                    MessageBox.Show(
                        "Selecione um animal.");
                    return;
                }

                if (cmbFuncionario.SelectedValue == null)
                {
                    MessageBox.Show(
                        "Selecione um funcionário.");
                    return;
                }

                Resultado = new ConsultaDados
                {
                    Id = original == null
                        ? 0 : original.Id,

                    IdAnimal = Convert.ToInt32(
                        cmbAnimal.SelectedValue),

                    IdFuncionario = Convert.ToInt32(
                        cmbFuncionario.SelectedValue),

                    DataHora = dtData.Value,

                    Diagnostico =
                        txtDiagnostico.Text.Trim(),

                    Observacoes =
                        txtObservacoes.Text.Trim()
                };

                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
