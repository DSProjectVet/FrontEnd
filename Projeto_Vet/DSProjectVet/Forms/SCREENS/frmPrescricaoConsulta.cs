
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DSProjectVet.Forms.SCREENS
{
    public class frmPrescricaoConsulta : Form
    {
        private bool limitesCarregados = false;
        private Button btnGerarPdf;

        private readonly int idConsulta;

        private class EstoqueIndisponivelException : Exception
        {
            public EstoqueIndisponivelException(string mensagem)
                : base(mensagem)
            {
            }
        }

        // ======================================
        // CORES MED VET
        // ======================================

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

        // ======================================
        // CONTROLES
        // ======================================

        private DataGridView gridPrescricoes;

        private ComboBox cmbMedicamento;
        private NumericUpDown numQuantidade;

        private TextBox txtDosagem;
        private TextBox txtFrequencia;
        private TextBox txtDuracao;
        private TextBox txtObservacoes;

        private Label lblEstoque;
        private Label lblModo;
        private Label lblRodape;

        private Button btnNovo;
        private Button btnSalvar;
        private Button btnExcluir;
        private Button btnAtualizar;

        private int? idEditando;
        private int medicamentoAnterior;
        private int quantidadeAnterior;

        private bool carregando;
        private bool ocupado;

        // ======================================
        // MODELOS INTERNOS
        // ======================================

        private class DadosPrescricao
        {
            public int? Id;
            public int MedicamentoId;
            public int Quantidade;

            public string Dosagem;
            public string Frequencia;
            public string Duracao;
            public string Observacoes;
        }

        private class DadosTela
        {
            public DataTable Medicamentos;
            public DataTable Prescricoes;
        }

        // ======================================
        // CONSTRUTOR
        // ======================================

        public frmPrescricaoConsulta(int consultaId)
        {
            if (consultaId <= 0)
                throw new ArgumentException(
                    "Consulta inválida.");

            idConsulta = consultaId;

            ConstruirInterface();

            Shown += async (s, e) =>
            {
                try
                {
                    await ConfigurarLimitesCamposAsync();
                }
                catch (Exception ex)
                {
                    btnSalvar.Enabled = false;

                    MessageBox.Show(
                        this,
                        "Não foi possível carregar os limites " +
                        "dos campos:\n\n" + ex.Message,
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }



                await CarregarDadosAsync();
            };
        }

        // ======================================
        // CONSTRUIR INTERFACE
        // ======================================


        private void ConstruirInterface()
        {
            // ==========================================
            // CONFIGURAÇÃO DA JANELA
            // ==========================================

            Text = "Med Vet - Prescrições";
            ClientSize = new Size(1140, 720);
            MinimumSize = new Size(980, 620);

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = false;

            BackColor = fundo;
            Font = new Font("Segoe UI", 10F);
            AutoScaleMode = AutoScaleMode.Font;

            // ==========================================
            // ESTRUTURA PRINCIPAL
            // ==========================================

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = fundo,
                Padding = new Padding(22, 18, 22, 15),
                ColumnCount = 1,
                RowCount = 3
            };

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 92F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 72F));

            Controls.Add(layout);

            // ==========================================
            // CABEÇALHO
            // ==========================================

            Panel cabecalho = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = fundo,
                Margin = new Padding(0, 0, 0, 12)
            };

            layout.Controls.Add(cabecalho, 0, 0);

            Panel detalheVerde = new Panel
            {
                BackColor = verde,
                Location = new Point(0, 11),
                Size = new Size(5, 56)
            };

            cabecalho.Controls.Add(detalheVerde);

            Label titulo = CriarLabel(
                "Prescrições veterinárias", 22, true);

            titulo.SetBounds(20, 3, 680, 45);
            cabecalho.Controls.Add(titulo);

            Label subtitulo = CriarLabel(
                "Gerencie medicamentos e orientações do tratamento.",
                10, false);

            subtitulo.ForeColor = cinza;
            subtitulo.SetBounds(22, 49, 700, 27);

            cabecalho.Controls.Add(subtitulo);

            Label lblNumeroConsulta = CriarLabel(
                "CONSULTA #" + idConsulta.ToString("D4"),
                10, true);

            lblNumeroConsulta.Size = new Size(170, 39);
            lblNumeroConsulta.BackColor =
                Color.FromArgb(225, 248, 240);

            lblNumeroConsulta.ForeColor = verdeEscuro;
            lblNumeroConsulta.TextAlign =
                ContentAlignment.MiddleCenter;

            cabecalho.Controls.Add(lblNumeroConsulta);

            cabecalho.Resize += (s, e) =>
            {
                lblNumeroConsulta.Location = new Point(
                    Math.Max(0, cabecalho.ClientSize.Width - 175),
                    15);
            };

            // ==========================================
            // DIVISÃO DA TELA
            // ==========================================

            SplitContainer split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 10,
                BackColor = fundo,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0)
            };

            layout.Controls.Add(split, 0, 1);

            // ==========================================
            // PAINEL ESQUERDO - LISTAGEM
            // ==========================================

            Panel cardLista = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                Margin = new Padding(0, 0, 6, 0)
            };

            split.Panel1.Controls.Add(cardLista);

            TableLayoutPanel layoutLista = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = new Padding(0)
            };

            layoutLista.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            layoutLista.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 37F));

            layoutLista.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 38F));

            layoutLista.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            cardLista.Controls.Add(layoutLista);

            Label tituloLista = CriarLabel(
                "Prescrições cadastradas", 14, true);

            tituloLista.Dock = DockStyle.Fill;
            layoutLista.Controls.Add(tituloLista, 0, 0);

            Label descricaoLista = CriarLabel(
                "Selecione uma prescrição para visualizar ou editar.",
                9, false);

            descricaoLista.ForeColor = cinza;
            descricaoLista.Dock = DockStyle.Fill;

            layoutLista.Controls.Add(descricaoLista, 0, 1);

            gridPrescricoes = new DataGridView();

            ConfigurarGrade(gridPrescricoes);

            gridPrescricoes.Dock = DockStyle.Fill;
            gridPrescricoes.Margin = new Padding(0);

            gridPrescricoes.BackgroundColor = Color.White;

            gridPrescricoes.SelectionChanged += (s, e) =>
            {
                if (!carregando && !ocupado)
                    SelecionarPrescricao();
            };

            layoutLista.Controls.Add(gridPrescricoes, 0, 2);

            // ==========================================
            // PAINEL DIREITO - FORMULÁRIO
            // ==========================================

            Panel cardFormulario = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(16, 14, 16, 12),
                Margin = new Padding(6, 0, 0, 0)
            };

            split.Panel2.Controls.Add(cardFormulario);

            TableLayoutPanel layoutFormulario =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3,
                    Margin = new Padding(0)
                };

            layoutFormulario.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            layoutFormulario.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 73F));

            layoutFormulario.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            layoutFormulario.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 68F));

            cardFormulario.Controls.Add(layoutFormulario);

            // ==========================================
            // TÍTULO DO FORMULÁRIO
            // ==========================================

            TableLayoutPanel cabecalhoFormulario =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2,
                    Margin = new Padding(0)
                };

            cabecalhoFormulario.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 40F));

            cabecalhoFormulario.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            lblModo = CriarLabel(
                "Nova prescrição", 16, true);

            lblModo.Dock = DockStyle.Fill;

            cabecalhoFormulario.Controls.Add(
                lblModo, 0, 0);

            Label ajuda = CriarLabel(
                "Preencha os dados do medicamento e tratamento.",
                9, false);

            ajuda.ForeColor = cinza;
            ajuda.Dock = DockStyle.Fill;

            cabecalhoFormulario.Controls.Add(
                ajuda, 0, 1);

            layoutFormulario.Controls.Add(
                cabecalhoFormulario, 0, 0);

            // ==========================================
            // ÁREA ROLÁVEL DOS CAMPOS
            // ==========================================

            Panel areaRolagem = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            layoutFormulario.Controls.Add(areaRolagem, 0, 1);

            TableLayoutPanel campos = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                // Altura explícita para a rolagem funcionar e
                // impedir que os campos internos fiquem ocultos.
                AutoSize = false,
                Height = 475,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0, 5, 5, 12)
            };

            campos.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            campos.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 80F));

            campos.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 44F));

            campos.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 86F));

            campos.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 86F));

            campos.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 135F));

            areaRolagem.Controls.Add(campos);

            // ==========================================
            // MEDICAMENTO
            // ==========================================

            cmbMedicamento = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White
            };

            campos.Controls.Add(
                CriarGrupoCampo(
                    "Medicamento *", cmbMedicamento),
                0, 0);

            cmbMedicamento.SelectedIndexChanged += (s, e) =>
            {
                MostrarEstoque();
            };

            // ==========================================
            // ESTOQUE DISPONÍVEL
            // ==========================================

            lblEstoque = CriarLabel(
                "Selecione um medicamento para consultar o estoque.",
                9, false);

            lblEstoque.Dock = DockStyle.Fill;
            lblEstoque.BackColor =
                Color.FromArgb(235, 248, 241);

            lblEstoque.ForeColor = verdeEscuro;
            lblEstoque.Padding = new Padding(10, 0, 8, 0);
            lblEstoque.Margin = new Padding(0, 0, 8, 8);

            campos.Controls.Add(lblEstoque, 0, 1);

            // ==========================================
            // QUANTIDADE + DOSAGEM
            // ==========================================

            TableLayoutPanel linhaDosagem =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    Margin = new Padding(0)
                };

            linhaDosagem.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 35F));

            linhaDosagem.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 65F));


            numQuantidade = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 1000000,
                Value = 1,
                ThousandsSeparator = true
            };

            // O evento deve ser adicionado
            // DEPOIS que o controle for criado.
            numQuantidade.ValueChanged += (s, e) =>
            {
                MostrarEstoque();
            };

            txtDosagem = new TextBox();

            linhaDosagem.Controls.Add(
                CriarGrupoCampo(
                    "Quantidade *", numQuantidade),
                0, 0);

            linhaDosagem.Controls.Add(
                CriarGrupoCampo(
                    "Dosagem *", txtDosagem),
                1, 0);

            // Adicionar UMA ÚNICA VEZ: duplicar este bloco
            // substituía o controle de dosagem e ocultava a linha.
            campos.Controls.Add(linhaDosagem, 0, 2);

            // ==========================================
            // FREQUÊNCIA + DURAÇÃO
            // ==========================================

            TableLayoutPanel linhaTratamento =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    Margin = new Padding(0)
                };

            linhaTratamento.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 55F));

            linhaTratamento.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 45F));

            txtFrequencia = new TextBox();
            txtDuracao = new TextBox();

            linhaTratamento.Controls.Add(
                CriarGrupoCampo(
                    "Frequência *", txtFrequencia),
                0, 0);

            linhaTratamento.Controls.Add(
                CriarGrupoCampo(
                    "Duração *", txtDuracao),
                1, 0);

            campos.Controls.Add(linhaTratamento, 0, 3);

            // ==========================================
            // OBSERVAÇÕES
            // ==========================================

            txtObservacoes = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            campos.Controls.Add(
                CriarGrupoCampo(
                    "Observações adicionais",
                    txtObservacoes),
                0, 4);

            // ==========================================
            // AÇÕES DO FORMULÁRIO
            // ==========================================

            FlowLayoutPanel acoes = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0)
            };

            layoutFormulario.Controls.Add(acoes, 0, 2);

            btnSalvar = CriarBotao(
                "Salvar prescrição",
                verde,
                Color.White,
                158);

            btnExcluir = CriarBotao(
                "Excluir",
                vermelho,
                Color.White,
                92);

            btnNovo = CriarBotao(
                "Novo",
                Color.FromArgb(231, 240, 236),
                texto,
                86);

            btnSalvar.Margin = new Padding(5, 7, 0, 5);
            btnExcluir.Margin = new Padding(5, 7, 0, 5);
            btnNovo.Margin = new Padding(5, 7, 0, 5);

            acoes.Controls.Add(btnSalvar);
            acoes.Controls.Add(btnExcluir);
            acoes.Controls.Add(btnNovo);

            btnSalvar.Click += async (s, e) =>
            {
                await SalvarAsync();
            };

            btnExcluir.Click += async (s, e) =>
            {
                await ExcluirAsync();
            };

            btnNovo.Click += (s, e) =>
            {
                IniciarNovaPrescricao();
            };

            // ==========================================
            // RODAPÉ - SEM DUPLICAÇÃO
            // ==========================================

            TableLayoutPanel rodape = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(0, 8, 0, 0)
            };

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 155F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 135F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 118F));

            rodape.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            layout.Controls.Add(rodape, 0, 2);

            lblRodape = CriarLabel(
                "Aguardando carregamento...", 9, false);

            lblRodape.ForeColor = cinza;
            lblRodape.Dock = DockStyle.Fill;
            lblRodape.AutoEllipsis = true;

            rodape.Controls.Add(lblRodape, 0, 0);

            // GERAR PDF
            btnGerarPdf = CriarBotao(
                "Gerar PDF",
                verde,
                Color.White,
                145);

            btnGerarPdf.Dock = DockStyle.Fill;
            btnGerarPdf.Margin = new Padding(4, 7, 8, 6);

            rodape.Controls.Add(btnGerarPdf, 1, 0);

            btnGerarPdf.Click += async (s, e) =>
            {
                await GerarPdfAsync();
            };

            // ATUALIZAR
            btnAtualizar = CriarBotao(
                "Atualizar",
                Color.FromArgb(231, 240, 236),
                texto,
                125);

            btnAtualizar.Dock = DockStyle.Fill;
            btnAtualizar.Margin = new Padding(0, 7, 8, 6);

            rodape.Controls.Add(btnAtualizar, 2, 0);

            btnAtualizar.Click += async (s, e) =>
            {
                await CarregarDadosAsync();
            };

            // FECHAR
            Button btnFechar = CriarBotao(
                "Fechar",
                verdeEscuro,
                Color.White,
                108);

            btnFechar.Dock = DockStyle.Fill;
            btnFechar.Margin = new Padding(0, 7, 0, 6);

            btnFechar.DialogResult = DialogResult.Cancel;

            rodape.Controls.Add(btnFechar, 3, 0);

            CancelButton = btnFechar;

            // ==========================================
            // POSICIONAMENTO INICIAL DAS COLUNAS
            // ==========================================

            Shown += (s, e) =>
            {
                if (split.Width > 800)
                {
                    int destino = (int)(split.Width * 0.50);

                    split.SplitterDistance = destino;

                    split.Panel1MinSize = 300;
                    split.Panel2MinSize = 380;
                }
            };

            IniciarNovaPrescricao();
        }


        private TableLayoutPanel CriarGrupoCampo(
            string titulo,
            Control campo)
        {
            TableLayoutPanel grupo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 10, 8)
            };

            grupo.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            grupo.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 25F));

            grupo.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            Label label = CriarLabel(titulo, 9, true);

            label.Dock = DockStyle.Fill;
            label.ForeColor = texto;

            grupo.Controls.Add(label, 0, 0);

            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(0, 3, 0, 2);
            campo.Font = new Font("Segoe UI", 10F);

            if (campo is TextBox)
            {
                TextBox caixa = (TextBox)campo;

                caixa.BorderStyle = BorderStyle.FixedSingle;
                caixa.BackColor = Color.White;
                caixa.ForeColor = texto;
            }

            if (campo is ComboBox)
            {
                ComboBox combo = (ComboBox)campo;

                combo.FlatStyle = FlatStyle.Flat;
                combo.BackColor = Color.White;
            }

            grupo.Controls.Add(campo, 0, 1);

            return grupo;
        }


        // ======================================
        // CONTROLES VISUAIS
        // ======================================

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
            string nome,
            Color corFundo,
            Color corTexto,
            int largura)
        {
            Button botao = new Button();

            botao.Text = nome;

            botao.Size = new Size(largura, 43);

            botao.Margin =
                new Padding(4, 8, 4, 4);

            botao.BackColor = corFundo;
            botao.ForeColor = corTexto;

            botao.UseVisualStyleBackColor = false;

            botao.FlatStyle = FlatStyle.Flat;

            botao.FlatAppearance.BorderSize = 0;

            botao.Cursor = Cursors.Hand;

            botao.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            return botao;
        }

        private void AdicionarCampo(
            FlowLayoutPanel painel,
            string titulo,
            Control campo,
            int altura)
        {
            Panel grupo = new Panel();

            grupo.Width = 380;
            grupo.Height = altura;

            grupo.Margin =
                new Padding(0, 0, 0, 8);

            Label label = CriarLabel(
                titulo, 9, true);

            label.SetBounds(
                0, 0, 370, 23);

            grupo.Controls.Add(label);

            campo.SetBounds(
                0,
                28,
                360,
                altura - 35);

            campo.Font =
                new Font("Segoe UI", 10F);

            grupo.Controls.Add(campo);

            painel.Controls.Add(grupo);
        }

        private void ConfigurarGrade(
            DataGridView grade)
        {
            grade.BackgroundColor = Color.White;

            grade.BorderStyle =
                BorderStyle.None;

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

            grade.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            // Texto normal visível, inclusive sem seleção.
            grade.DefaultCellStyle.BackColor = Color.White;
            grade.DefaultCellStyle.ForeColor = texto;
            grade.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 252, 250);
            grade.AlternatingRowsDefaultCellStyle.ForeColor = texto;

            grade.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(225, 248, 240);

            grade.DefaultCellStyle.SelectionForeColor =
                verdeEscuro;
        }

        // ======================================
        // CONEXÃO EXISTENTE
        // ======================================

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
                {
                    continue;
                }

                string valor = Convert.ToString(
                    Properties.Settings.Default[
                        propriedade.Name]);

                if (!string.IsNullOrWhiteSpace(valor))
                    return valor;
            }

            throw new Exception(
                "Connection string não encontrada " +
                "em Properties.Settings.");
        }

        // ======================================
        // CARREGAR MEDICAMENTOS E PRESCRIÇÕES
        // ======================================

        private DataTable ExecutarTabela(
            SqlConnection conexao,
            string sql,
            bool usarConsulta)
        {
            DataTable tabela = new DataTable();

            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            using (SqlDataAdapter adaptador =
                new SqlDataAdapter(comando))
            {
                if (usarConsulta)
                {
                    comando.Parameters.Add(
                        "@Consulta",
                        SqlDbType.Int).Value =
                            idConsulta;
                }

                adaptador.Fill(tabela);
            }

            return tabela;
        }

        private DadosTela ConsultarDados()
        {
            DadosTela dados = new DadosTela();

            string sqlMedicamentos = @"
                SELECT
                    m.ID_Medicamento,
                    m.Nome,

                    ISNULL(e.Quantidade, 0)
                        AS Disponivel,

                    CAST(m.ID_Medicamento
                        AS VARCHAR(15))
                    + ' - ' + ISNULL(m.Nome, '')
                        AS Exibicao

                FROM dbo.Medicamentos AS m

                LEFT JOIN dbo.Estoque_Medicamento AS e
                    ON e.fk_Medicamentos_ID_Medicamento =
                       m.ID_Medicamento

                ORDER BY m.Nome;
            ";

            string sqlPrescricoes = @"
                SELECT
                    p.ID_Prescricao,
                    p.fk_Medicamentos_ID_Medicamento
                        AS ID_Medicamento,

                    m.Nome AS Medicamento,

                    p.Quantidade,
                    p.Dosagem,
                    p.Frequencia,
                    p.Duracao,
                    p.Observacoes

                FROM dbo.Prescricao AS p

                INNER JOIN dbo.Medicamentos AS m
                    ON m.ID_Medicamento =
                       p.fk_Medicamentos_ID_Medicamento

                WHERE p.fk_Consultas_ID_Consulta =
                      @Consulta

                ORDER BY p.ID_Prescricao DESC;
            ";

            using (SqlConnection conexao =
                new SqlConnection(
                    ObterConnectionString()))
            {
                conexao.Open();

                dados.Medicamentos =
                    ExecutarTabela(
                        conexao,
                        sqlMedicamentos,
                        false);

                dados.Prescricoes =
                    ExecutarTabela(
                        conexao,
                        sqlPrescricoes,
                        true);
            }

            return dados;
        }

        private async Task CarregarDadosAsync(
            int? selecionarId = null)
        {
            if (ocupado)
            {
                // Permitimos a atualização interna
                // após uma operação de salvamento.
            }

            btnAtualizar.Enabled = false;

            lblRodape.Text =
                "Consultando prescrições no SQL Server...";

            try
            {
                DadosTela dados =
                    await Task.Run(
                        () => ConsultarDados());

                carregando = true;

                try
                {
                    cmbMedicamento.DataSource = null;

                    cmbMedicamento.DisplayMember =
                        "Exibicao";

                    cmbMedicamento.ValueMember =
                        "ID_Medicamento";

                    cmbMedicamento.DataSource =
                        dados.Medicamentos;

                    cmbMedicamento.SelectedIndex = -1;

                    gridPrescricoes.DataSource =
                        dados.Prescricoes;

                    if (gridPrescricoes.Columns.Contains(
                        "ID_Medicamento"))
                    {
                        gridPrescricoes.Columns[
                            "ID_Medicamento"].Visible = false;
                    }

                    if (gridPrescricoes.Columns.Contains(
                        "Observacoes"))
                    {
                        gridPrescricoes.Columns[
                            "Observacoes"].Visible = false;
                    }

                    gridPrescricoes.ClearSelection();
                    gridPrescricoes.CurrentCell = null;

                    idEditando = null;

                    LimparCampos();

                    if (selecionarId.HasValue)
                    {
                        foreach (DataGridViewRow linha
                            in gridPrescricoes.Rows)
                        {
                            DataRowView registro =
                                linha.DataBoundItem
                                as DataRowView;

                            if (registro != null &&
                                Convert.ToInt32(
                                    registro["ID_Prescricao"])
                                    == selecionarId.Value)
                            {
                                gridPrescricoes.CurrentCell =
                                    linha.Cells[
                                        "ID_Prescricao"];

                                linha.Selected = true;

                                break;
                            }
                        }
                    }
                }
                finally
                {
                    carregando = false;
                }

                if (gridPrescricoes.CurrentRow != null)
                    SelecionarPrescricao();

                lblRodape.Text =
                    dados.Prescricoes.Rows.Count +
                    " prescrição(ões) vinculadas à consulta.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Erro ao carregar prescrições:\n\n" +
                    ex.Message,
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblRodape.Text =
                    "Falha na consulta ao banco.";
            }
            finally
            {
                btnAtualizar.Enabled = true;
            }
        }

        // ======================================
        // SELECIONAR / NOVA PRESCRIÇÃO
        // ======================================

        private void SelecionarPrescricao()
        {
            if (gridPrescricoes.CurrentRow == null)
                return;

            DataRowView linha =
                gridPrescricoes.CurrentRow.DataBoundItem
                as DataRowView;

            if (linha == null)
                return;

            idEditando =
                Convert.ToInt32(
                    linha["ID_Prescricao"]);

            medicamentoAnterior =
                Convert.ToInt32(
                    linha["ID_Medicamento"]);

            quantidadeAnterior =
                Convert.ToInt32(
                    linha["Quantidade"]);

            cmbMedicamento.SelectedValue =
                medicamentoAnterior;

            decimal quantidade =
                Math.Max(
                    numQuantidade.Minimum,
                    Math.Min(
                        numQuantidade.Maximum,
                        quantidadeAnterior));

            numQuantidade.Value = quantidade;

            txtDosagem.Text =
                Convert.ToString(linha["Dosagem"]);

            txtFrequencia.Text =
                Convert.ToString(linha["Frequencia"]);

            txtDuracao.Text =
                Convert.ToString(linha["Duracao"]);

            txtObservacoes.Text =
                Convert.ToString(linha["Observacoes"]);

            lblModo.Text =
                "Editar prescrição #" + idEditando;

            btnExcluir.Enabled = true;

            MostrarEstoque();
        }

        private void IniciarNovaPrescricao()
        {
            if (gridPrescricoes != null)
            {
                carregando = true;

                gridPrescricoes.ClearSelection();
                gridPrescricoes.CurrentCell = null;

                carregando = false;
            }

            idEditando = null;

            LimparCampos();
        }

        private void LimparCampos()
        {
            medicamentoAnterior = 0;
            quantidadeAnterior = 0;

            if (cmbMedicamento != null)
                cmbMedicamento.SelectedIndex = -1;

            numQuantidade.Value = 1;

            txtDosagem.Clear();
            txtFrequencia.Clear();
            txtDuracao.Clear();
            txtObservacoes.Clear();

            lblModo.Text = "Nova prescrição";

            btnExcluir.Enabled = false;

            MostrarEstoque();
        }


        private void MostrarEstoque()
        {

            if (cmbMedicamento == null ||
                numQuantidade == null ||
                lblEstoque == null ||
                btnSalvar == null)
            {
                return;
            }

            DataRowView medicamento =
                cmbMedicamento.SelectedItem as DataRowView;

            // Nenhum medicamento selecionado
            if (medicamento == null)
            {
                lblEstoque.Text =
                    "Selecione um medicamento.";

                lblEstoque.ForeColor = Color.Gray;
                lblEstoque.BackColor =
                    Color.FromArgb(245, 245, 245);

                btnSalvar.Enabled = false;
                return;
            }

            int idMedicamento =
                Convert.ToInt32(
                    medicamento["ID_Medicamento"]);

            int disponivel =
                Convert.ToInt32(
                    medicamento["Disponivel"]);

            int quantidadeSolicitada =
                Decimal.ToInt32(numQuantidade.Value);

            // Na edição, só precisamos de estoque
            // para a quantidade ADICIONAL.
            int quantidadeParaRetirar = quantidadeSolicitada;

            if (idEditando.HasValue &&
                medicamentoAnterior == idMedicamento)
            {
                quantidadeParaRetirar = Math.Max(
                    0,
                    quantidadeSolicitada - quantidadeAnterior
                );
            }

            // ======================================
            // MEDICAMENTO ESGOTADO
            // ======================================

            if (disponivel <= 0 && quantidadeParaRetirar > 0)
            {
                lblEstoque.Text =
                    "SEM ESTOQUE! Escolha outro medicamento.";

                lblEstoque.ForeColor =
                    Color.FromArgb(190, 35, 45);

                lblEstoque.BackColor =
                    Color.FromArgb(255, 235, 238);

                btnSalvar.Enabled = false;
                return;
            }

            // ======================================
            // QUANTIDADE INSUFICIENTE
            // ======================================

            if (quantidadeParaRetirar > disponivel)
            {
                lblEstoque.Text =
                    "ESTOQUE INSUFICIENTE! Disponível: " +
                    disponivel + " unidade(s). " +
                    "Necessário retirar: " +
                    quantidadeParaRetirar + ".";

                lblEstoque.ForeColor =
                    Color.FromArgb(190, 35, 45);

                lblEstoque.BackColor =
                    Color.FromArgb(255, 235, 238);

                btnSalvar.Enabled = false;
                return;
            }

            // ======================================
            // ESTOQUE SUFICIENTE
            // ======================================

            lblEstoque.Text =
                "Estoque disponível: " +
                disponivel + " unidade(s).";

            lblEstoque.ForeColor =
                Color.FromArgb(14, 145, 104);

            lblEstoque.BackColor =
                Color.FromArgb(225, 248, 240);

            // Não habilitar salvar se a consulta aos limites
            // das colunas SQL falhou.
            btnSalvar.Enabled = !ocupado && limitesCarregados;
        }


        // ======================================
        // VALIDAÇÃO
        // ======================================

        // Reconsulta somente o medicamento selecionado depois
        // de uma concorrência de estoque, sem perder o formulário.
        private async Task AtualizarEstoqueSelecionadoAsync()
        {
            DataRowView item = cmbMedicamento.SelectedItem as DataRowView;
            if (item == null)
                return;

            int medicamentoId = Convert.ToInt32(item["ID_Medicamento"]);

            int disponivel = await Task.Run(() =>
            {
                using (SqlConnection conexao =
                    new SqlConnection(ObterConnectionString()))
                using (SqlCommand comando = new SqlCommand(@"
                    SELECT ISNULL(MIN(Quantidade), 0)
                    FROM dbo.Estoque_Medicamento
                    WHERE fk_Medicamentos_ID_Medicamento = @Id;",
                    conexao))
                {
                    comando.Parameters.Add("@Id", SqlDbType.Int).Value = medicamentoId;
                    conexao.Open();
                    return Convert.ToInt32(comando.ExecuteScalar());
                }
            });

            // A edição dos dados vinculados é feita na UI thread,
            // após o await.
            item["Disponivel"] = disponivel;
            MostrarEstoque();
        }

        private DadosPrescricao CapturarDados()
        {
            DataRowView medicamento =
                cmbMedicamento.SelectedItem
                as DataRowView;

            if (medicamento == null)
            {
                MessageBox.Show(
                    "Selecione um medicamento.");

                return null;
            }

            if (string.IsNullOrWhiteSpace(
                txtDosagem.Text) ||
                string.IsNullOrWhiteSpace(
                    txtFrequencia.Text) ||
                string.IsNullOrWhiteSpace(
                    txtDuracao.Text))
            {
                MessageBox.Show(
                    "Preencha dosagem, frequência " +
                    "e duração do tratamento.");

                return null;
            }

            return new DadosPrescricao
            {
                Id = idEditando,

                MedicamentoId =
                    Convert.ToInt32(
                        medicamento["ID_Medicamento"]),

                Quantidade =
                    Decimal.ToInt32(
                        numQuantidade.Value),

                Dosagem =
                    txtDosagem.Text.Trim(),

                Frequencia =
                    txtFrequencia.Text.Trim(),

                Duracao =
                    txtDuracao.Text.Trim(),

                Observacoes =
                    txtObservacoes.Text.Trim()
            };
        }

        // ======================================
        // SALVAR PRESCRIÇÃO
        // ======================================

        private async Task SalvarAsync()
        {
            if (ocupado)
                return;

            if (!ValidarLimitesCampos())
                return;

            DadosPrescricao dados =
                CapturarDados();

            if (dados == null)
                return;

            ocupado = true;

            btnSalvar.Enabled = false;
            btnExcluir.Enabled = false;
            btnNovo.Enabled = false;

            try
            {
                int idSalvo = await Task.Run(
                    () => SalvarNoBanco(dados));

                await CarregarDadosAsync(idSalvo);

                MessageBox.Show(
                    this,
                    "Prescrição salva com sucesso!\n\n" +
                    "A quantidade foi ajustada " +
                    "no estoque de medicamentos.",
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            catch (EstoqueIndisponivelException ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Estoque indisponível",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                // Atualiza somente a disponibilidade.
                // Preserva dosagem, frequência e duração digitadas.
                try
                {
                    await AtualizarEstoqueSelecionadoAsync();
                }
                catch (Exception erroAtualizacao)
                {
                    MessageBox.Show(
                        this,
                        "Não foi possível atualizar o estoque: " +
                        erroAtualizacao.Message,
                        "Med Vet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível salvar:\n\n" +
                    ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                ocupado = false;

                btnNovo.Enabled = true;
                btnExcluir.Enabled = idEditando.HasValue;

                // Reavalia o estoque antes de liberar o botão.
                MostrarEstoque();
            }
        }

        // ======================================
        // EXCLUIR PRESCRIÇÃO
        // ======================================

        private async Task ExcluirAsync()
        {
            if (ocupado || !idEditando.HasValue)
                return;

            DialogResult confirmar =
                MessageBox.Show(
                    this,
                    "Deseja excluir a prescrição #" +
                    idEditando.Value + "?\n\n" +
                    "A quantidade registrada será " +
                    "devolvida ao estoque.\n\n" +
                    "Só confirme se a dispensação " +
                    "for realmente cancelada.",
                    "Confirmar exclusão",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

            if (confirmar != DialogResult.Yes)
                return;

            int id = idEditando.Value;

            ocupado = true;

            btnSalvar.Enabled = false;
            btnExcluir.Enabled = false;
            btnNovo.Enabled = false;

            try
            {
                await Task.Run(
                    () => ExcluirNoBanco(id));

                await CarregarDadosAsync();

                MessageBox.Show(
                    this,
                    "Prescrição excluída e " +
                    "estoque estornado.",
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Erro ao excluir:\n\n" +
                    ex.Message,
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                ocupado = false;
                btnNovo.Enabled = true;

                btnExcluir.Enabled =
                    idEditando.HasValue;

                MostrarEstoque();
            }
        }

        // ======================================
        // PARÂMETROS SQL
        // ======================================

        private void ParametrosPrescricao(
            SqlCommand cmd,
            DadosPrescricao p)
        {
            cmd.Parameters.Add(
                "@Consulta",
                SqlDbType.Int).Value = idConsulta;

            cmd.Parameters.Add(
                "@Medicamento",
                SqlDbType.Int).Value =
                    p.MedicamentoId;

            cmd.Parameters.Add(
                "@Quantidade",
                SqlDbType.Int).Value =
                    p.Quantidade;

            cmd.Parameters.Add(
                "@Dosagem",
                SqlDbType.NVarChar, -1).Value =
                    p.Dosagem;

            cmd.Parameters.Add(
                "@Frequencia",
                SqlDbType.NVarChar, -1).Value =
                    p.Frequencia;

            cmd.Parameters.Add(
                "@Duracao",
                SqlDbType.NVarChar, -1).Value =
                    p.Duracao;

            cmd.Parameters.Add(
                "@Observacoes",
                SqlDbType.NVarChar, -1).Value =
                    p.Observacoes;
        }

        // ======================================
        // AJUSTAR ESTOQUE
        // ======================================

        private void AjustarEstoque(
            SqlConnection conexao,
            SqlTransaction transacao,
            int idMedicamento,
            int delta)
        {
            if (delta == 0)
                return;

            // Delta positivo: desconta.
            // Delta negativo: devolve.
            string sql = delta > 0
                ? @"
                    UPDATE dbo.Estoque_Medicamento
                    SET Quantidade =
                        Quantidade - @Quantidade
                    WHERE
                        fk_Medicamentos_ID_Medicamento =
                            @Medicamento
                        AND Quantidade >= @Quantidade;
                  "
                : @"
                    UPDATE dbo.Estoque_Medicamento
                    SET Quantidade =
                        Quantidade + @Quantidade
                    WHERE
                        fk_Medicamentos_ID_Medicamento =
                            @Medicamento;
                  ";

            using (SqlCommand cmd =
                new SqlCommand(
                    sql, conexao, transacao))
            {
                cmd.Parameters.Add(
                    "@Medicamento",
                    SqlDbType.Int).Value =
                        idMedicamento;

                cmd.Parameters.Add(
                    "@Quantidade",
                    SqlDbType.Int).Value =
                        Math.Abs(delta);

                int linhas =
                    cmd.ExecuteNonQuery();


                if (linhas != 1)
                {
                    if (delta > 0)
                    {
                        string sqlVerificar = @"

                        SELECT
                            COUNT(*) AS Registros,
                            ISNULL(MAX(Quantidade), 0) AS Disponivel
                        FROM dbo.Estoque_Medicamento
                        WHERE fk_Medicamentos_ID_Medicamento =
                              @Medicamento;";

                        using (SqlCommand verificar =
                            new SqlCommand(
                                sqlVerificar, conexao, transacao))
                        {
                            verificar.Parameters.Add(
                                "@Medicamento",
                                SqlDbType.Int).Value = idMedicamento;

                            using (SqlDataReader leitor =
                                verificar.ExecuteReader())
                            {
                                leitor.Read();

                                int registros =
                                    Convert.ToInt32(leitor["Registros"]);

                                int disponivel =
                                    Convert.ToInt32(leitor["Disponivel"]);

                                if (registros == 0)
                                {
                                    throw new EstoqueIndisponivelException(
                                        "Este medicamento não possui " +
                                        "registro de estoque.\n\n" +
                                        "Selecione outro medicamento.");
                                }

                                if (registros > 1)
                                {
                                    throw new Exception(
                                        "Existem registros duplicados " +
                                        "de estoque para o medicamento #" +
                                        idMedicamento + ".");
                                }

                                if (disponivel == 0)
                                {
                                    throw new EstoqueIndisponivelException(
                                        "Este medicamento está ESGOTADO!\n\n" +
                                        "Não existem unidades disponíveis.\n\n" +
                                        "Selecione outro medicamento.");
                                }

                                if (disponivel < delta)
                                {
                                    throw new EstoqueIndisponivelException(
                                        "Estoque insuficiente!\n\n" +
                                        "Disponível: " + disponivel +
                                        " unidade(s).\n" +
                                        "Solicitado: " + delta +
                                        " unidade(s).\n\n" +
                                        "Selecione outro medicamento " +
                                        "ou reduza a quantidade.");
                                }
                            }
                        }
                    }

                    throw new Exception(
                        "Não foi possível atualizar o estoque " +
                        "do medicamento #" + idMedicamento + ".");
                }

            }
        }

        // ======================================
        // INSERIR / ATUALIZAR NO SQL
        // ======================================

        private int SalvarNoBanco(
            DadosPrescricao dados)
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
                        int idRetorno;

                        if (!dados.Id.HasValue)
                        {
                            // NOVA PRESCRIÇÃO:
                            // descontar quantidade integral.
                            AjustarEstoque(
                                conexao,
                                transacao,
                                dados.MedicamentoId,
                                dados.Quantidade);

                            string sql = @"
                                INSERT INTO dbo.Prescricao
                                (
                                    Quantidade,
                                    Dosagem,
                                    Frequencia,
                                    Duracao,
                                    Observacoes,
                                    fk_Medicamentos_ID_Medicamento,
                                    fk_Consultas_ID_Consulta
                                )
                                VALUES
                                (
                                    @Quantidade,
                                    @Dosagem,
                                    @Frequencia,
                                    @Duracao,
                                    @Observacoes,
                                    @Medicamento,
                                    @Consulta
                                );

                                SELECT CAST(
                                    SCOPE_IDENTITY() AS INT);
                            ";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    sql,
                                    conexao,
                                    transacao))
                            {
                                ParametrosPrescricao(
                                    cmd, dados);

                                object resultado =
                                    cmd.ExecuteScalar();

                                if (resultado == null ||
                                    resultado == DBNull.Value)
                                {
                                    throw new Exception(
                                        "O banco não retornou " +
                                        "o ID da prescrição.");
                                }

                                idRetorno =
                                    Convert.ToInt32(
                                        resultado);
                            }
                        }
                        else
                        {
                            // ==================================
                            // LER PRESCRIÇÃO ORIGINAL NO BANCO
                            // ==================================

                            int medicamentoAntigo;
                            int quantidadeAntiga;

                            string sqlAntiga = @"
                                SELECT
                                    fk_Medicamentos_ID_Medicamento,
                                    Quantidade
                                FROM dbo.Prescricao
                                    WITH (UPDLOCK, HOLDLOCK)
                                WHERE ID_Prescricao = @ID
                                  AND fk_Consultas_ID_Consulta =
                                      @Consulta;
                            ";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    sqlAntiga,
                                    conexao,
                                    transacao))
                            {
                                cmd.Parameters.Add(
                                    "@ID",
                                    SqlDbType.Int).Value =
                                        dados.Id.Value;

                                cmd.Parameters.Add(
                                    "@Consulta",
                                    SqlDbType.Int).Value =
                                        idConsulta;

                                using (SqlDataReader leitor =
                                    cmd.ExecuteReader())
                                {
                                    if (!leitor.Read())
                                    {
                                        throw new Exception(
                                            "Prescrição não encontrada.");
                                    }

                                    medicamentoAntigo =
                                        Convert.ToInt32(
                                            leitor[0]);

                                    quantidadeAntiga =
                                        Convert.ToInt32(
                                            leitor[1]);
                                }
                            }

                            // ==================================
                            // RECALCULAR APENAS A DIFERENÇA
                            // ==================================

                            if (medicamentoAntigo ==
                                dados.MedicamentoId)
                            {
                                int diferenca =
                                    dados.Quantidade -
                                    quantidadeAntiga;

                                AjustarEstoque(
                                    conexao,
                                    transacao,
                                    dados.MedicamentoId,
                                    diferenca);
                            }
                            else
                            {
                                // Desconta medicamento novo.
                                AjustarEstoque(
                                    conexao,
                                    transacao,
                                    dados.MedicamentoId,
                                    dados.Quantidade);

                                // Devolve medicamento anterior.
                                AjustarEstoque(
                                    conexao,
                                    transacao,
                                    medicamentoAntigo,
                                    -quantidadeAntiga);
                            }

                            // ==================================
                            // ATUALIZAR PRESCRIÇÃO
                            // ==================================

                            string sqlEditar = @"
                                UPDATE dbo.Prescricao
                                SET
                                    Quantidade = @Quantidade,
                                    Dosagem = @Dosagem,
                                    Frequencia = @Frequencia,
                                    Duracao = @Duracao,
                                    Observacoes = @Observacoes,
                                    fk_Medicamentos_ID_Medicamento =
                                        @Medicamento
                                WHERE ID_Prescricao = @ID
                                  AND fk_Consultas_ID_Consulta =
                                      @Consulta;
                            ";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    sqlEditar,
                                    conexao,
                                    transacao))
                            {
                                ParametrosPrescricao(
                                    cmd, dados);

                                cmd.Parameters.Add(
                                    "@ID",
                                    SqlDbType.Int).Value =
                                        dados.Id.Value;

                                if (cmd.ExecuteNonQuery() != 1)
                                {
                                    throw new Exception(
                                        "Falha ao atualizar " +
                                        "a prescrição.");
                                }
                            }

                            idRetorno = dados.Id.Value;
                        }

                        FinanceiroSync.Sincronizar(
                            conexao,
                            transacao,
                            idConsulta);

                        transacao.Commit();

                        return idRetorno;
                    }
                    catch
                    {
                        try
                        {
                            transacao.Rollback();
                        }
                        catch { }

                        throw;
                    }
                }
            }
        }

        // ======================================
        // EXCLUIR NO SQL E ESTORNAR ESTOQUE
        // ======================================

        private void ExcluirNoBanco(int id)
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
                        int idMedicamento;
                        int quantidade;

                        string sqlBuscar = @"
                            SELECT
                                fk_Medicamentos_ID_Medicamento,
                                Quantidade
                            FROM dbo.Prescricao
                                WITH (UPDLOCK, HOLDLOCK)
                            WHERE ID_Prescricao = @ID
                              AND fk_Consultas_ID_Consulta =
                                  @Consulta;
                        ";

                        using (SqlCommand cmd =
                            new SqlCommand(
                                sqlBuscar,
                                conexao,
                                transacao))
                        {
                            cmd.Parameters.Add(
                                "@ID",
                                SqlDbType.Int).Value = id;

                            cmd.Parameters.Add(
                                "@Consulta",
                                SqlDbType.Int).Value =
                                    idConsulta;

                            using (SqlDataReader leitor =
                                cmd.ExecuteReader())
                            {
                                if (!leitor.Read())
                                {
                                    throw new Exception(
                                        "Prescrição não encontrada.");
                                }

                                idMedicamento =
                                    Convert.ToInt32(leitor[0]);

                                quantidade =
                                    Convert.ToInt32(leitor[1]);
                            }
                        }

                        string sqlExcluir = @"
                            DELETE FROM dbo.Prescricao
                            WHERE ID_Prescricao = @ID
                              AND fk_Consultas_ID_Consulta =
                                  @Consulta;
                        ";

                        using (SqlCommand cmd =
                            new SqlCommand(
                                sqlExcluir,
                                conexao,
                                transacao))
                        {
                            cmd.Parameters.Add(
                                "@ID",
                                SqlDbType.Int).Value = id;

                            cmd.Parameters.Add(
                                "@Consulta",
                                SqlDbType.Int).Value =
                                    idConsulta;

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                throw new Exception(
                                    "Não foi possível excluir.");
                            }
                        }

                        AjustarEstoque(
                            conexao,
                            transacao,
                            idMedicamento,
                            -quantidade);

                        FinanceiroSync.Sincronizar(
                            conexao,
                            transacao,
                            idConsulta);

                        transacao.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transacao.Rollback();
                        }
                        catch { }

                        throw;
                    }
                }
            }
        }


        private async Task ConfigurarLimitesCamposAsync()
        {
            // Usa os tamanhos reais da tabela.
            Dictionary<string, int> limites = await Task.Run(() =>
            {
                Dictionary<string, int> resultado =
                    new Dictionary<string, int>(
                        StringComparer.OrdinalIgnoreCase);

                string sql = @"
            SELECT
                COLUMN_NAME,
                CHARACTER_MAXIMUM_LENGTH
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo'
              AND TABLE_NAME = 'Prescricao'
              AND COLUMN_NAME IN
              (
                  'Dosagem',
                  'Frequencia',
                  'Duracao',
                  'Observacoes'
              );
        ";

                using (SqlConnection conexao =
                    new SqlConnection(ObterConnectionString()))
                using (SqlCommand comando =
                    new SqlCommand(sql, conexao))
                {
                    conexao.Open();

                    using (SqlDataReader leitor =
                        comando.ExecuteReader())
                    {
                        while (leitor.Read())
                        {
                            string coluna =
                                Convert.ToString(leitor["COLUMN_NAME"]);

                            if (leitor["CHARACTER_MAXIMUM_LENGTH"]
                                == DBNull.Value)
                            {
                                throw new Exception(
                                    "A coluna " + coluna +
                                    " não é do tipo texto.");
                            }

                            resultado[coluna] =
                                Convert.ToInt32(
                                    leitor["CHARACTER_MAXIMUM_LENGTH"]);
                        }
                    }
                }

                return resultado;
            });

            AplicarLimite(txtDosagem, "Dosagem", limites);
            AplicarLimite(txtFrequencia, "Frequencia", limites);
            AplicarLimite(txtDuracao, "Duracao", limites);
            AplicarLimite(txtObservacoes, "Observacoes", limites);

            limitesCarregados = true;
            MostrarEstoque();
        }

        private void AplicarLimite(
            TextBox campo,
            string coluna,
            Dictionary<string, int> limites)
        {
            if (!limites.ContainsKey(coluna))
            {
                throw new Exception(
                    "A coluna " + coluna +
                    " não foi encontrada em dbo.Prescricao.");
            }

            int tamanho = limites[coluna];

            // -1 significa NVARCHAR(MAX) ou VARCHAR(MAX).
            // Usamos 4000 como limite prático da interface.
            // Isso não modifica o banco.
            if (tamanho == -1)
                tamanho = 4000;

            if (tamanho <= 0)
            {
                throw new Exception(
                    "Tamanho inválido para a coluna " +
                    coluna + ".");
            }

            campo.MaxLength = tamanho;

            ToolTip dica = new ToolTip();

            dica.SetToolTip(
                campo,
                coluna + ": máximo de " +
                tamanho + " caracteres.");
        }

        private bool ValidarLimitesCampos()
        {
            if (!limitesCarregados)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível confirmar os limites " +
                    "dos campos no banco de dados.",
                    "Validação indisponível",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            TextBox[] campos =
            {
        txtDosagem,
        txtFrequencia,
        txtDuracao,
        txtObservacoes
    };

            foreach (TextBox campo in campos)
            {
                if (campo.Text.Length > campo.MaxLength)
                {
                    MessageBox.Show(
                        this,
                        "O texto ultrapassa o limite de " +
                        campo.MaxLength + " caracteres.\n\n" +
                        "Revise o campo antes de salvar.",
                        "Texto muito longo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    campo.Focus();
                    return false;
                }
            }

            return true;
        }


        private async Task GerarPdfAsync()
        {
            if (ocupado)
                return;

            using (SaveFileDialog salvar =
                new SaveFileDialog())
            {
                salvar.Title =
                    "Salvar receita veterinária";

                salvar.Filter =
                    "Documento PDF (*.pdf)|*.pdf";

                salvar.DefaultExt = "pdf";
                salvar.AddExtension = true;
                salvar.OverwritePrompt = true;

                salvar.FileName =
                    "Receita_Consulta_" +
                    idConsulta.ToString("D4") + ".pdf";

                if (salvar.ShowDialog(this) !=
                    DialogResult.OK)
                {
                    return;
                }

                string caminho = salvar.FileName;

                // Captura a conexão na thread da interface.
                string conexao = ObterConnectionString();

                ocupado = true;
                btnGerarPdf.Enabled = false;

                try
                {
                    lblRodape.Text =
                        "Gerando receita em PDF...";

                    await Task.Run(() =>
                    {
                        ReceitaPdfService.Gerar(
                            idConsulta,
                            conexao,
                            caminho);
                    });

                    lblRodape.Text =
                        "Receita gerada com sucesso.";

                    DialogResult abrir =
                        MessageBox.Show(
                            this,
                            "Receita salva com sucesso!\n\n" +
                            caminho +
                            "\n\nDeseja abrir o documento?",
                            "PDF gerado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                    if (abrir == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(
                            new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = caminho,
                                UseShellExecute = true
                            });
                    }
                }
                catch (Exception ex)
                {
                    lblRodape.Text =
                        "Falha ao gerar PDF.";

                    MessageBox.Show(
                        this,
                        "Não foi possível gerar a receita.\n\n" +
                        ex.Message,
                        "Erro ao gerar PDF",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    ocupado = false;
                    btnGerarPdf.Enabled = true;
                }
            }
        }


    }
}
