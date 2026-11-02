
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public class frmGerenciarUsuarios : Form
    {
        // ==========================================
        // CORES MED VET
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

        private readonly Color vermelho =
            Color.FromArgb(220, 53, 69);

        // ==========================================
        // CONTROLES
        // ==========================================

        private DataGridView gridUsuarios;
        private TextBox txtPesquisa;

        private Label lblTotal;
        private Label lblAdmins;
        private Label lblFuncionarios;
        private Label lblStatus;

        private Button btnNovo;
        private Button btnEditar;
        private Button btnSenha;
        private Button btnExcluir;
        private Button btnAtualizar;

        private DataTable usuarios = new DataTable();

        private bool carregando = false;
        private bool ocupado = false;

        // Evita encerrar o formulário enquanto o SQL está gravando.
        private void BloquearFechamentoDuranteGravacao(object sender,
            FormClosingEventArgs e)
        {
            if (!ocupado || e.CloseReason != CloseReason.UserClosing)
                return;

            e.Cancel = true;
            MessageBox.Show(this,
                "Aguarde a conclusão da operação antes de fechar a tela.",
                "Operação em andamento", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ==========================================
        // MODELO DE FUNCIONÁRIO
        // ==========================================

        private class FuncionarioDados
        {
            public int Id { get; set; }

            public string Nome { get; set; }
            public string Login { get; set; }
            public string Senha { get; set; }

            public string Nivel { get; set; }
            public string Telefone { get; set; }
            public string CRMV { get; set; }
            public string Tipo { get; set; }
        }

        // ==========================================
        // CONSTRUTOR
        // ==========================================

        public frmGerenciarUsuarios()
        {
            ConfigurarJanela();
            ConstruirInterface();
            FormClosing += BloquearFechamentoDuranteGravacao;

            Shown += async (s, e) =>
            {
                if (!VerificarPermissao())
                {
                    Close();
                    return;
                }

                await CarregarUsuariosAsync();
            };
        }

        // ==========================================
        // JANELA
        // ==========================================

        private void ConfigurarJanela()
        {
            Text = "Med Vet - Gerenciar Usuários";

            ClientSize = new Size(1120, 730);
            MinimumSize = new Size(980, 630);

            StartPosition =
                FormStartPosition.CenterParent;

            BackColor = fundo;

            Font = new Font("Segoe UI", 10F);
        }

        // ==========================================
        // PERMISSÕES
        // ==========================================

        private bool VerificarPermissao()
        {
            int nivel =
                DSProjectVet.Properties.Settings
                    .Default.NivelUsuarioLogado;

            if (nivel == 1)
                return true;

            MessageBox.Show(
                this,
                "Somente administradores podem " +
                "gerenciar usuários.",
                "Acesso negado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            return false;
        }

        // ==========================================
        // CONSTRUIR INTERFACE
        // ==========================================

        private void ConstruirInterface()
        {
            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;

            layout.Padding =
                new Padding(24, 20, 24, 18);

            layout.ColumnCount = 1;
            layout.RowCount = 5;

            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 91F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 67F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 78F));

            Controls.Add(layout);

            // ======================================
            // CABEÇALHO
            // ======================================

            Panel cabecalho = new Panel();

            cabecalho.Dock = DockStyle.Fill;

            layout.Controls.Add(cabecalho, 0, 0);

            Label titulo = CriarLabel(
                "Gerenciar usuários", 23F, true);

            titulo.SetBounds(
                0, 2, 620, 49);

            cabecalho.Controls.Add(titulo);

            Label subtitulo = CriarLabel(
                "Gerencie funcionários, acessos e senhas.",
                10F, false);

            subtitulo.ForeColor = cinza;

            subtitulo.SetBounds(
                2, 51, 640, 28);

            cabecalho.Controls.Add(subtitulo);

            btnNovo = CriarBotao(
                "+ Novo usuário",
                verde,
                Color.White,
                170);

            btnNovo.Dock = DockStyle.Right;

            btnNovo.Margin = new Padding(0);

            cabecalho.Controls.Add(btnNovo);

            btnNovo.Click += async (s, e) =>
            {
                await NovoUsuarioAsync();
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
                        SizeType.Percent,
                        33.3333F));
            }

            indicadores.Controls.Add(
                CriarIndicador(
                    "Total de usuários",
                    out lblTotal), 0, 0);

            indicadores.Controls.Add(
                CriarIndicador(
                    "Administradores",
                    out lblAdmins), 1, 0);

            indicadores.Controls.Add(
                CriarIndicador(
                    "Demais funcionários",
                    out lblFuncionarios), 2, 0);

            layout.Controls.Add(
                indicadores, 0, 1);

            // ======================================
            // PESQUISA
            // ======================================

            Panel painelPesquisa = new Panel();

            painelPesquisa.Dock = DockStyle.Fill;

            layout.Controls.Add(
                painelPesquisa, 0, 2);

            txtPesquisa = new TextBox();

            txtPesquisa.Font =
                new Font("Segoe UI", 11F);

            txtPesquisa.SetBounds(
                0, 13, 390, 35);

            painelPesquisa.Controls.Add(
                txtPesquisa);

            Label dicaPesquisa = CriarLabel(
                "Pesquisar nome, login, telefone ou CRMV",
                9F, false);

            dicaPesquisa.ForeColor = cinza;

            dicaPesquisa.SetBounds(
                405, 16, 390, 28);

            painelPesquisa.Controls.Add(
                dicaPesquisa);

            txtPesquisa.TextChanged += (s, e) =>
            {
                AplicarPesquisa();
            };

            // ======================================
            // TABELA
            // ======================================

            Panel painelTabela = new Panel();

            painelTabela.Dock = DockStyle.Fill;

            painelTabela.BackColor = Color.White;

            painelTabela.Padding =
                new Padding(10);

            layout.Controls.Add(
                painelTabela, 0, 3);

            gridUsuarios = CriarGrade();

            painelTabela.Controls.Add(gridUsuarios);

            AdicionarColuna(
                "ID_Funcionario", "Código", 55);

            AdicionarColuna(
                "Nome", "Nome", 180);

            AdicionarColuna(
                "Login", "Login", 135);

            AdicionarColuna(
                "NivelAcesso", "Nível", 60);

            AdicionarColuna(
                "Telefone", "Telefone", 125);

            AdicionarColuna(
                "CRMV", "CRMV", 90);

            AdicionarColuna(
                "Funcionarios_TIPO", "Tipo", 60);

            gridUsuarios.SelectionChanged += (s, e) => AtualizarBotoes();

            gridUsuarios.CellDoubleClick +=
                async (s, e) =>
                {
                    if (e.RowIndex >= 0)
                        await EditarUsuarioAsync();
                };

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
                    SizeType.AutoSize));

            layout.Controls.Add(
                rodape, 0, 4);

            lblStatus = CriarLabel(
                "Aguardando carregamento...",
                9F, false);

            lblStatus.Dock = DockStyle.Fill;

            lblStatus.ForeColor = cinza;

            lblStatus.AutoEllipsis = true;

            rodape.Controls.Add(
                lblStatus, 0, 0);

            FlowLayoutPanel barraAcoes =
                new FlowLayoutPanel();

            barraAcoes.Dock = DockStyle.Fill;

            barraAcoes.AutoSize = true;

            barraAcoes.WrapContents = false;

            barraAcoes.FlowDirection =
                FlowDirection.LeftToRight;

            rodape.Controls.Add(
                barraAcoes, 1, 0);

            btnEditar = CriarBotao(
                "Editar",
                verde,
                Color.White,
                100);

            btnSenha = CriarBotao(
                "Redefinir senha",
                verdeEscuro,
                Color.White,
                160);

            btnExcluir = CriarBotao(
                "Excluir",
                vermelho,
                Color.White,
                105);

            btnAtualizar = CriarBotao(
                "Atualizar",
                Color.FromArgb(230, 240, 235),
                texto,
                110);

            barraAcoes.Controls.Add(btnEditar);
            barraAcoes.Controls.Add(btnSenha);
            barraAcoes.Controls.Add(btnExcluir);
            barraAcoes.Controls.Add(btnAtualizar);

            btnEditar.Click += async (s, e) =>
            {
                await EditarUsuarioAsync();
            };

            btnSenha.Click += async (s, e) =>
            {
                await RedefinirSenhaAsync();
            };

            btnExcluir.Click += async (s, e) =>
            {
                await ExcluirUsuarioAsync();
            };

            btnAtualizar.Click += async (s, e) =>
            {
                await CarregarUsuariosAsync();
            };

            AtualizarBotoes();
        }

        private void AtualizarBotoes()
        {
            if (btnNovo == null || btnEditar == null ||
                btnSenha == null || btnExcluir == null ||
                btnAtualizar == null) return;

            bool autorizado =
                Properties.Settings.Default.NivelUsuarioLogado == 1;
            bool livre = !ocupado && !carregando && autorizado;
            FuncionarioDados selecionado = ObterSelecionado();
            bool modificavel = selecionado != null &&
                               selecionado.Nivel != "1";

            btnNovo.Enabled = livre;
            btnEditar.Enabled = livre && modificavel;
            btnSenha.Enabled = livre && modificavel;
            btnExcluir.Enabled = livre && modificavel;
            btnAtualizar.Enabled = !ocupado && !carregando && autorizado;
        }

        // ==========================================
        // CONTROLES VISUAIS
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
            Color fundoBotao,
            Color corTexto,
            int largura)
        {
            Button botao = new Button();

            botao.Text = titulo;

            botao.Size =
                new Size(largura, 43);

            botao.BackColor = fundoBotao;
            botao.ForeColor = corTexto;

            botao.FlatStyle = FlatStyle.Flat;

            botao.FlatAppearance.BorderSize = 0;

            botao.UseVisualStyleBackColor = false;

            botao.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            botao.Cursor = Cursors.Hand;

            botao.Margin =
                new Padding(4, 12, 4, 4);

            return botao;
        }

        private Panel CriarIndicador(
            string titulo,
            out Label valor)
        {
            Panel cartao = new Panel();

            cartao.BackColor = Color.White;
            cartao.Dock = DockStyle.Fill;

            cartao.Margin =
                new Padding(0, 0, 12, 12);

            Label descricao = CriarLabel(
                titulo, 9F, false);

            descricao.ForeColor = cinza;

            descricao.SetBounds(
                16, 10, 250, 26);

            cartao.Controls.Add(descricao);

            valor = CriarLabel("—", 22F, true);

            valor.ForeColor = verdeEscuro;

            valor.SetBounds(
                16, 36, 220, 44);

            cartao.Controls.Add(valor);

            return cartao;
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

            grade.AutoGenerateColumns = false;

            grade.AllowUserToAddRows = false;
            grade.AllowUserToDeleteRows = false;
            grade.AllowUserToResizeRows = false;

            grade.RowHeadersVisible = false;

            grade.MultiSelect = false;

            grade.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            grade.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            grade.RowTemplate.Height = 40;

            grade.ColumnHeadersHeight = 44;

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

            grade.DefaultCellStyle.ForeColor =
                texto;

            grade.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            grade.AlternatingRowsDefaultCellStyle.SelectionBackColor = verdeClaro;
            grade.AlternatingRowsDefaultCellStyle.SelectionForeColor = verdeEscuro;

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

        private void AdicionarColuna(
            string campo,
            string titulo,
            float peso)
        {
            DataGridViewTextBoxColumn coluna =
                new DataGridViewTextBoxColumn();

            coluna.Name = campo;
            coluna.DataPropertyName = campo;
            coluna.HeaderText = titulo;
            coluna.FillWeight = peso;

            gridUsuarios.Columns.Add(coluna);
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
                "A connection string não foi encontrada " +
                "em Properties.Settings.");
        }

        // ==========================================
        // LISTAR FUNCIONÁRIOS
        // ==========================================

        private DataTable ConsultarFuncionarios()
        {
            DataTable resultado = new DataTable();

            string sql = @"
                SELECT
                    ID_Funcionario,
                    Nome,
                    Login,
                    NivelAcesso,
                    Telefone,
                    CRMV,
                    Funcionarios_TIPO
                FROM dbo.Funcionarios
                ORDER BY Nome, ID_Funcionario;
            ";

            using (SqlConnection conexao =
                new SqlConnection(
                    ObterConnectionString()))
            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(comando))
            {
                comando.CommandTimeout = 30;

                adapter.Fill(resultado);
            }

            return resultado;
        }

        private async Task CarregarUsuariosAsync()
        {
            if (carregando || ocupado)
                return;

            if (!VerificarPermissao())
                return;

            carregando = true;

            btnAtualizar.Enabled = false;

            lblStatus.Text =
                "Carregando funcionários...";

            try
            {
                usuarios = await Task.Run(
                    () => ConsultarFuncionarios());

                AplicarPesquisa();

                int quantidadeAdmins = 0;

                foreach (DataRow linha in usuarios.Rows)
                {
                    string nivel = Convert.ToString(
                        linha["NivelAcesso"]).Trim();

                    if (nivel == "1")
                        quantidadeAdmins++;
                }

                lblTotal.Text =
                    usuarios.Rows.Count.ToString();

                lblAdmins.Text =
                    quantidadeAdmins.ToString();

                lblFuncionarios.Text =
                    (usuarios.Rows.Count -
                     quantidadeAdmins).ToString();

                lblStatus.Text =
                    usuarios.Rows.Count +
                    " funcionário(s) cadastrado(s).";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível listar usuários.\n\n" +
                    ex.Message,
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text =
                    "Erro ao carregar funcionários.";
            }
            finally
            {
                carregando = false;
                AtualizarBotoes();
            }
        }

        // ==========================================
        // PESQUISAR
        // ==========================================

        private void AplicarPesquisa()
        {
            if (gridUsuarios == null ||
                txtPesquisa == null)
                return;

            DataTable filtrada =
                usuarios.Clone();

            string pesquisa =
                txtPesquisa.Text.Trim();

            foreach (DataRow linha in usuarios.Rows)
            {
                string conteudo =
                    Convert.ToString(linha["Nome"]) + " " +
                    Convert.ToString(linha["Login"]) + " " +
                    Convert.ToString(linha["Telefone"]) + " " +
                    Convert.ToString(linha["CRMV"]) + " " +
                    Convert.ToString(linha["NivelAcesso"]);

                if (pesquisa.Length == 0 ||
                    conteudo.IndexOf(
                        pesquisa,
                        StringComparison
                            .CurrentCultureIgnoreCase) >= 0)
                {
                    filtrada.ImportRow(linha);
                }
            }

            gridUsuarios.DataSource = filtrada;

            gridUsuarios.ClearSelection();
            gridUsuarios.CurrentCell = null;
            AtualizarBotoes();
        }

        // ==========================================
        // FUNCIONÁRIO SELECIONADO
        // ==========================================

        private FuncionarioDados ObterSelecionado()
        {
            if (gridUsuarios.CurrentRow == null)
                return null;

            DataRowView linha =
                gridUsuarios.CurrentRow.DataBoundItem
                as DataRowView;

            if (linha == null)
                return null;

            return new FuncionarioDados
            {
                Id = Convert.ToInt32(
                    linha["ID_Funcionario"]),

                Nome = Convert.ToString(
                    linha["Nome"]),

                Login = Convert.ToString(
                    linha["Login"]),

                Nivel = Convert.ToString(
                    linha["NivelAcesso"]).Trim(),

                Telefone = Convert.ToString(
                    linha["Telefone"]),

                CRMV = Convert.ToString(
                    linha["CRMV"]),

                Tipo = Convert.ToString(
                    linha["Funcionarios_TIPO"]).Trim()
            };
        }

        private FuncionarioDados SelecionadoOuAviso()
        {
            FuncionarioDados funcionario =
                ObterSelecionado();

            if (funcionario == null)
            {
                MessageBox.Show(
                    this,
                    "Selecione um funcionário na tabela.",
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            return funcionario;
        }

        // ==========================================
        // ERROS AMIGÁVEIS E VALIDAÇÕES DO BANCO
        // ==========================================

        private void ExibirErroOperacao(string operacao, Exception ex,
            string crmv = null, IWin32Window janela = null)
        {
            string mensagem;
            SqlException erroSql = ex as SqlException;

            if (erroSql != null)
            {
                switch (erroSql.Number)
                {
                    case 2601:
                    case 2627:
                        mensagem =
                            "Já existe um cadastro com um valor exclusivo " +
                            "igual ao informado (por exemplo, login ou CRMV).";

                        if (string.IsNullOrWhiteSpace(crmv))
                        {
                            mensagem +=
                                "\n\nSe o conflito menciona NULL, " +
                                "provavelmente existe uma restrição UNIQUE " +
                                "convencional no CRMV. O banco precisa " +
                                "permitir vários CRMVs nulos e exigir " +
                                "unicidade somente dos preenchidos.";
                        }
                        break;

                    case 547:
                        mensagem =
                            "O registro possui vínculos com outros dados " +
                            "da clínica e não pode ser alterado ou excluído.";
                        break;

                    case 515:
                        mensagem =
                            "O banco exige um campo que foi enviado vazio. " +
                            "Confira os campos obrigatórios e, se o funcionário " +
                            "não for veterinário, a configuração de CRMV nulo.";
                        break;

                    case 8152:
                    case 2628:
                        mensagem =
                            "Um dos valores ultrapassou o tamanho permitido " +
                            "no banco de dados. Confira nome, login, CRMV e telefone.";
                        break;

                    case -2:
                        mensagem =
                            "O banco de dados demorou demais para responder. " +
                            "Confira a conexão e atualize a listagem antes de tentar novamente.";
                        break;

                    case 53:
                    case 4060:
                        mensagem =
                            "Não foi possível conectar ao banco de dados. " +
                            "Verifique o servidor e a conexão.";
                        break;

                    default:
                        mensagem =
                            "O SQL Server não conseguiu concluir a operação.\n\n" +
                            "Detalhes: " + erroSql.Message;
                        break;
                }
            }
            else
            {
                mensagem = ex.Message;
            }

            MessageBox.Show(janela ?? this,
                operacao + " não foi concluída.\n\n" + mensagem +
                "\n\nO formulário continuará aberto para você corrigir os " +
                "dados ou tentar novamente.",
                "Med Vet - Operação não concluída",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (lblStatus != null)
                lblStatus.Text = operacao + " não concluída.";
        }

        private void VerificarDuplicidade(SqlConnection conexao,
            FuncionarioDados dados)
        {
            const string sql = @"
                SELECT TOP (1) Login, CRMV
                FROM dbo.Funcionarios
                WHERE ID_Funcionario <> @ID
                  AND (Login = @Login
                       OR (@CRMV IS NOT NULL AND CRMV = @CRMV));";

            using (SqlCommand cmd = new SqlCommand(sql, conexao))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = dados.Id;
                cmd.Parameters.Add("@Login", SqlDbType.NVarChar, 50).Value = dados.Login;
                cmd.Parameters.Add("@CRMV", SqlDbType.VarChar, 10).Value =
                    string.IsNullOrWhiteSpace(dados.CRMV)
                    ? (object)DBNull.Value : dados.CRMV;

                using (SqlDataReader leitor = cmd.ExecuteReader())
                {
                    if (!leitor.Read()) return;

                    string loginEncontrado = Convert.ToString(leitor["Login"]).Trim();
                    if (string.Equals(loginEncontrado, dados.Login,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Este login já está em uso. Informe outro login.");
                    }

                    throw new InvalidOperationException(
                        "Este CRMV já pertence a outro funcionário.");
                }
            }
        }

        // ==========================================
        // NOVO FUNCIONÁRIO
        // ==========================================


        private async Task NovoUsuarioAsync()
        {
            if (ocupado || !VerificarPermissao()) return;

            try
            {
                using (EditorFuncionario editor = new EditorFuncionario(null))
                {
                    // O editor permanece aberto se houver erro ao salvar.
                    editor.GravarAsync = async dados =>
                    {
                        if (ocupado) return false;
                        ocupado = true;
                        AtualizarBotoes();

                        try
                        {
                            lblStatus.Text = "Cadastrando funcionário...";
                            int id = await Task.Run(() =>
                                CadastrarFuncionarioNoBanco(dados));

                            lblStatus.Text = "Funcionário #" + id + " cadastrado.";
                            MessageBox.Show(editor,
                                "Funcionário cadastrado com sucesso!\n\n" +
                                "Código: #" + id + "\nNome: " + dados.Nome,
                                "Med Vet", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            dados.Senha = null;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            ExibirErroOperacao("Cadastro", ex, dados.CRMV, editor);
                            return false;
                        }
                        finally
                        {
                            ocupado = false;
                            AtualizarBotoes();
                        }
                    };

                    if (editor.ShowDialog(this) == DialogResult.OK)
                        await CarregarUsuariosAsync();
                }
            }
            catch (Exception ex)
            {
                ExibirErroOperacao("Abertura do cadastro", ex);
            }
        }

        private int CadastrarFuncionarioNoBanco(FuncionarioDados dados)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(
                dados.Senha, workFactor: 11);

            // Usa a procedure SQL diretamente, sem DataSet tipado.
            // Isso evita exceções de constraints do TableAdapter.
            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            {
                conexao.Open();
                VerificarDuplicidade(conexao, dados);

                using (SqlCommand comando =
                    new SqlCommand("dbo.sp_CadastrarFuncionario", conexao))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.CommandTimeout = 30;

                    comando.Parameters.Add("@Nome", SqlDbType.NVarChar, 50)
                        .Value = dados.Nome;
                    comando.Parameters.Add("@Login", SqlDbType.NVarChar, 50)
                        .Value = dados.Login;
                    comando.Parameters.Add("@SenhaHash", SqlDbType.NVarChar, 100)
                        .Value = hash;
                    comando.Parameters.Add("@NivelAcesso", SqlDbType.Char, 1)
                        .Value = dados.Nivel;
                    comando.Parameters.Add("@Telefone", SqlDbType.VarChar, 11)
                        .Value = dados.Telefone;
                    comando.Parameters.Add("@CRMV", SqlDbType.VarChar, 10)
                        .Value = string.IsNullOrWhiteSpace(dados.CRMV)
                            ? (object)DBNull.Value : dados.CRMV;
                    comando.Parameters.Add("@Funcionarios_TIPO", SqlDbType.Char, 1)
                        .Value = dados.Tipo;

                    comando.ExecuteNonQuery();
                }

                // Confirma o resultado da procedure na mesma conexão.
                const string sql = @"
                    SELECT TOP (1) ID_Funcionario
                    FROM dbo.Funcionarios
                    WHERE Login = @Login AND SenhaHash = @Hash
                    ORDER BY ID_Funcionario DESC;";

                using (SqlCommand verificar = new SqlCommand(sql, conexao))
                {
                    verificar.Parameters.Add("@Login", SqlDbType.NVarChar, 50)
                        .Value = dados.Login;
                    verificar.Parameters.Add("@Hash", SqlDbType.NVarChar, 100)
                        .Value = hash;

                    object id = verificar.ExecuteScalar();
                    if (id == null || id == DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            "A procedure foi executada, mas não foi possível " +
                            "confirmar o cadastro. Atualize a listagem antes " +
                            "de tentar novamente e verifique " +
                            "dbo.sp_CadastrarFuncionario.");
                    }
                    return Convert.ToInt32(id);
                }
            }
        }

        // ==========================================
        // EDITAR FUNCIONÁRIO
        // ==========================================

        private async Task EditarUsuarioAsync()
        {
            if (ocupado || !VerificarPermissao()) return;

            FuncionarioDados atual = SelecionadoOuAviso();
            if (atual == null) return;

            if (atual.Nivel == "1")
            {
                MessageBox.Show(this,
                    "Contas administradoras estão protegidas contra edição nesta tela.",
                    "Conta protegida", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (EditorFuncionario editor = new EditorFuncionario(atual))
                {
                    editor.GravarAsync = async dados =>
                    {
                        if (ocupado) return false;
                        ocupado = true;
                        AtualizarBotoes();

                        try
                        {
                            lblStatus.Text = "Atualizando funcionário...";
                            await Task.Run(() => AtualizarFuncionario(dados));
                            lblStatus.Text = "Funcionário atualizado.";
                            MessageBox.Show(editor,
                                "Dados atualizados com sucesso!",
                                "Med Vet", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            ExibirErroOperacao("Edição", ex, dados.CRMV, editor);
                            return false;
                        }
                        finally
                        {
                            ocupado = false;
                            AtualizarBotoes();
                        }
                    };

                    if (editor.ShowDialog(this) == DialogResult.OK)
                        await CarregarUsuariosAsync();
                }
            }
            catch (Exception ex)
            {
                ExibirErroOperacao("Abertura da edição", ex);
            }
        }

        private void AtualizarFuncionario(
            FuncionarioDados dados)
        {
            string sql = @"
                UPDATE dbo.Funcionarios
                SET
                    Nome = @Nome,
                    Login = @Login,
                    NivelAcesso = @Nivel,
                    Telefone = @Telefone,
                    CRMV = @CRMV,
                    Funcionarios_TIPO = @Tipo
                WHERE ID_Funcionario = @ID
                  AND NivelAcesso <> '1';
            ";

            using (SqlConnection conexao =
                new SqlConnection(
                    ObterConnectionString()))
            using (SqlCommand comando =
                new SqlCommand(sql, conexao))
            {
                AdicionarParametrosFuncionario(
                    comando, dados);

                comando.Parameters.Add(
                    "@ID", SqlDbType.Int).Value =
                        dados.Id;

                conexao.Open();
                VerificarDuplicidade(conexao, dados);

                int alterados =
                    comando.ExecuteNonQuery();

                if (alterados != 1)
                {
                    throw new Exception(
                        "Funcionário não encontrado " +
                        "ou protegido contra alterações.");
                }
            }
        }

        private void AdicionarParametrosFuncionario(
            SqlCommand comando,
            FuncionarioDados dados)
        {
            comando.Parameters.Add(
                "@Nome", SqlDbType.NVarChar, 50)
                .Value = dados.Nome;

            comando.Parameters.Add(
                "@Login", SqlDbType.NVarChar, 50)
                .Value = dados.Login;

            comando.Parameters.Add(
                "@Nivel", SqlDbType.Char, 1)
                .Value = dados.Nivel;

            comando.Parameters.Add(
                "@Telefone", SqlDbType.VarChar, 11)
                .Value = dados.Telefone;

            comando.Parameters.Add(
                "@CRMV", SqlDbType.VarChar, 10)
                .Value = string.IsNullOrWhiteSpace(
                    dados.CRMV)
                    ? (object)DBNull.Value
                    : dados.CRMV;

            comando.Parameters.Add(
                "@Tipo", SqlDbType.Char, 1)
                .Value = dados.Tipo;
        }

        // ==========================================
        // REDEFINIR SENHA
        // ==========================================

        private async Task RedefinirSenhaAsync()
        {
            if (ocupado || !VerificarPermissao()) return;
            FuncionarioDados funcionario = SelecionadoOuAviso();
            if (funcionario == null) return;

            if (funcionario.Nivel == "1")
            {
                MessageBox.Show(this,
                    "A redefinição de senhas administrativas está protegida.",
                    "Conta protegida", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (EditorSenha editor = new EditorSenha(funcionario.Nome))
                {
                    editor.GravarAsync = async senha =>
                    {
                        if (ocupado) return false;
                        ocupado = true;
                        AtualizarBotoes();
                        try
                        {
                            lblStatus.Text = "Atualizando senha...";
                            await Task.Run(() =>
                                RedefinirSenhaNoBanco(funcionario.Id, senha));
                            lblStatus.Text = "Senha redefinida.";
                            MessageBox.Show(editor,
                                "Senha redefinida com sucesso!",
                                "Med Vet", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            ExibirErroOperacao("Redefinição de senha", ex,
                                funcionario.CRMV, editor);
                            return false;
                        }
                        finally
                        {
                            ocupado = false;
                            AtualizarBotoes();
                        }
                    };
                    editor.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ExibirErroOperacao("Abertura da redefinição de senha", ex);
            }
        }

        private void RedefinirSenhaNoBanco(int idFuncionario, string senha)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(
                senha, workFactor: 11);
            const string sql = @"
                UPDATE dbo.Funcionarios
                SET SenhaHash = @Hash
                WHERE ID_Funcionario = @ID AND NivelAcesso <> '1';";

            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            using (SqlCommand comando = new SqlCommand(sql, conexao))
            {
                comando.Parameters.Add("@Hash", SqlDbType.NVarChar, 100)
                    .Value = hash;
                comando.Parameters.Add("@ID", SqlDbType.Int)
                    .Value = idFuncionario;
                conexao.Open();

                if (comando.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException(
                        "Funcionário não encontrado ou conta protegida.");
                }
            }
        }

        // ==========================================
        // EXCLUIR FUNCIONÁRIO
        // ==========================================

        private async Task ExcluirUsuarioAsync()
        {
            if (ocupado || !VerificarPermissao()) return;
            FuncionarioDados funcionario = SelecionadoOuAviso();
            if (funcionario == null) return;

            if (funcionario.Nivel == "1")
            {
                MessageBox.Show(this,
                    "Contas administradoras não podem ser excluídas nesta tela.",
                    "Conta protegida", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(this,
                "Deseja excluir permanentemente o funcionário?\n\n" +
                funcionario.Nome + " (#" + funcionario.Id + ")\n\n" +
                "Esta operação não poderá ser desfeita.",
                "Confirmar exclusão", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            ocupado = true;
            AtualizarBotoes();

            try
            {
                lblStatus.Text = "Verificando vínculos do funcionário...";
                string bloqueio = await Task.Run(() =>
                    ExcluirFuncionarioNoBanco(funcionario.Id));

                if (bloqueio != null)
                {
                    MessageBox.Show(this, bloqueio,
                        "Exclusão não permitida", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    lblStatus.Text = "Exclusão bloqueada. Nenhum dado foi apagado.";
                }
                else
                {
                    MessageBox.Show(this,
                        "Funcionário excluído com sucesso!",
                        "Med Vet", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    lblStatus.Text = "Funcionário excluído.";
                }
            }
            catch (Exception ex)
            {
                ExibirErroOperacao("Exclusão", ex, funcionario.CRMV);
            }
            finally
            {
                ocupado = false;
                AtualizarBotoes();
            }

            await CarregarUsuariosAsync();
        }

        // Retorna null quando exclui, ou o aviso se a exclusão
        // deve ser bloqueada. Não lança exceção para vínculos esperados.
        private string ExcluirFuncionarioNoBanco(int idFuncionario)
        {
            using (SqlConnection conexao =
                new SqlConnection(ObterConnectionString()))
            {
                conexao.Open();

                using (SqlTransaction transacao = conexao.BeginTransaction(
                    IsolationLevel.Serializable))
                {
                    try
                    {
                        const string sqlVinculos = @"
                            SELECT COUNT(*)
                            FROM dbo.Consultas WITH (UPDLOCK, HOLDLOCK)
                            WHERE fk_Funcionarios_ID_Funcionario = @ID;";

                        using (SqlCommand verificar =
                            new SqlCommand(sqlVinculos, conexao, transacao))
                        {
                            verificar.Parameters.Add("@ID", SqlDbType.Int)
                                .Value = idFuncionario;

                            if (Convert.ToInt32(verificar.ExecuteScalar()) > 0)
                            {
                                transacao.Rollback();
                                return "Este funcionário possui consultas vinculadas.\n\n" +
                                    "Por segurança, a exclusão foi bloqueada " +
                                    "e nenhum registro foi alterado.";
                            }
                        }

                        const string sqlExcluir = @"
                            DELETE FROM dbo.Funcionarios
                            WHERE ID_Funcionario = @ID
                              AND NivelAcesso <> '1';
                            SELECT @@ROWCOUNT;";

                        using (SqlCommand excluir =
                            new SqlCommand(sqlExcluir, conexao, transacao))
                        {
                            excluir.Parameters.Add("@ID", SqlDbType.Int)
                                .Value = idFuncionario;

                            int alterados = Convert.ToInt32(
                                excluir.ExecuteScalar());
                            if (alterados != 1)
                            {
                                transacao.Rollback();
                                return "Funcionário não encontrado ou " +
                                    "protegido contra exclusão.";
                            }
                        }

                        transacao.Commit();
                        return null;
                    }
                    catch
                    {
                        try { transacao.Rollback(); } catch { }
                        throw;
                    }
                }
            }
        }

        // ==========================================
        // JANELA CADASTRAR / EDITAR
        // ==========================================

        private class EditorFuncionario : Form
        {
            private readonly FuncionarioDados original;

            private TextBox txtNome;
            private TextBox txtLogin;
            private TextBox txtSenha;
            private TextBox txtConfirmarSenha;
            private TextBox txtTelefone;
            private TextBox txtCRMV;
            private TextBox txtTipo;

            private ComboBox cmbNivel;
            private Button btnSalvarFormulario;
            private Button btnCancelarFormulario;
            private bool gravando;

            // O diálogo só é fechado se o banco confirmar a gravação.
            public Func<FuncionarioDados, Task<bool>> GravarAsync { get; set; }

            public FuncionarioDados Resultado
            {
                get;
                private set;
            }

            public EditorFuncionario(
                FuncionarioDados funcionario)
            {
                original = funcionario;

                Text = funcionario == null
                    ? "Med Vet - Novo Usuário"
                    : "Med Vet - Editar Usuário";

                ClientSize = new Size(715, 585);

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

                ConstruirInterface();
                FormClosing += (s, e) =>
                {
                    if (gravando && e.CloseReason == CloseReason.UserClosing)
                    {
                        e.Cancel = true;
                        MessageBox.Show(this,
                            "Aguarde a conclusão do salvamento.",
                            "Med Vet", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                };

                if (funcionario != null)
                    Preencher(funcionario);
            }

            private void ConstruirInterface()
            {
                TableLayoutPanel layout =
                    new TableLayoutPanel();

                layout.Dock = DockStyle.Fill;

                layout.Padding =
                    new Padding(26, 22, 26, 18);

                layout.ColumnCount = 2;
                layout.RowCount = 7;

                layout.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent, 50F));

                layout.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent, 50F));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 72F));

                for (int i = 0; i < 4; i++)
                {
                    layout.RowStyles.Add(
                        new RowStyle(
                            SizeType.Absolute, 82F));
                }

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Percent, 100F));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 67F));

                Controls.Add(layout);

                // TÍTULO
                Label titulo = new Label();

                titulo.Text = original == null
                    ? "Cadastrar funcionário"
                    : "Editar funcionário";

                titulo.Font = new Font(
                    "Segoe UI", 21F, FontStyle.Bold);

                titulo.ForeColor =
                    Color.FromArgb(35, 49, 45);

                titulo.Dock = DockStyle.Fill;

                layout.Controls.Add(titulo, 0, 0);

                layout.SetColumnSpan(titulo, 2);

                // ==================================
                // CAMPOS
                // ==================================

                txtNome = new TextBox();
                txtNome.MaxLength = 50;

                txtLogin = new TextBox();
                txtLogin.MaxLength = 50;

                txtSenha = new TextBox();
                txtSenha.UseSystemPasswordChar = true;
                txtSenha.MaxLength = 72;

                txtConfirmarSenha = new TextBox();
                txtConfirmarSenha.UseSystemPasswordChar = true;
                txtConfirmarSenha.MaxLength = 72;

                cmbNivel = new ComboBox();

                cmbNivel.DropDownStyle =
                    ComboBoxStyle.DropDownList;

                cmbNivel.Items.AddRange(new object[]
                {
                    "1 - Administrador",
                    "2 - Atendente / Operacional",
                    "3 - Veterinário"
                });

                cmbNivel.SelectedIndex = 1;

                txtTipo = new TextBox();
                txtTipo.MaxLength = 1;

                txtTelefone = new TextBox();
                txtTelefone.MaxLength = 15; // máscara livre; serão salvos apenas 10 ou 11 dígitos

                txtCRMV = new TextBox();
                txtCRMV.MaxLength = 10;

                // LINHA 1
                layout.Controls.Add(
                    CriarCampo("Nome *", txtNome),
                    0, 1);

                layout.Controls.Add(
                    CriarCampo("Login *", txtLogin),
                    1, 1);

                // LINHA 2
                layout.Controls.Add(
                    CriarCampo("Senha inicial *", txtSenha),
                    0, 2);

                layout.Controls.Add(
                    CriarCampo(
                        "Confirmar senha *",
                        txtConfirmarSenha),
                    1, 2);

                // LINHA 3
                layout.Controls.Add(
                    CriarCampo(
                        "Nível de acesso *",
                        cmbNivel),
                    0, 3);

                layout.Controls.Add(
                    CriarCampo(
                        "Funcionarios_TIPO *",
                        txtTipo),
                    1, 3);

                // LINHA 4
                layout.Controls.Add(
                    CriarCampo(
                        "Telefone *",
                        txtTelefone),
                    0, 4);

                layout.Controls.Add(
                    CriarCampo(
                        "CRMV (veterinários)",
                        txtCRMV),
                    1, 4);

                // ==================================
                // INFORMAÇÃO
                // ==================================

                Label ajuda = new Label();

                ajuda.Text =
                    "O campo Funcionarios_TIPO aceita um " +
                    "código de 1 caractere, conforme " +
                    "sua configuração no banco.\n" +
                    "As senhas são armazenadas com BCrypt.";

                ajuda.ForeColor = Color.Gray;

                ajuda.Dock = DockStyle.Fill;

                ajuda.Font =
                    new Font("Segoe UI", 9F);

                layout.Controls.Add(ajuda, 0, 5);

                layout.SetColumnSpan(ajuda, 2);

                // ==================================
                // RODAPÉ
                // ==================================

                FlowLayoutPanel rodape =
                    new FlowLayoutPanel();

                rodape.Dock = DockStyle.Fill;

                rodape.FlowDirection =
                    FlowDirection.RightToLeft;

                rodape.WrapContents = false;

                layout.Controls.Add(
                    rodape, 0, 6);

                layout.SetColumnSpan(rodape, 2);

                Button btnSalvar = new Button();
                btnSalvarFormulario = btnSalvar;

                btnSalvar.Text = original == null
                    ? "Cadastrar usuário"
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
                btnCancelarFormulario = btnCancelar;

                btnCancelar.Text = "Cancelar";

                btnCancelar.Size =
                    new Size(140, 44);

                btnCancelar.BackColor =
                    Color.FromArgb(230, 240, 235);

                btnCancelar.FlatStyle =
                    FlatStyle.Flat;

                btnCancelar.FlatAppearance.BorderSize = 0;

                rodape.Controls.Add(btnSalvar);
                rodape.Controls.Add(btnCancelar);

                btnSalvar.Click += (s, e) =>
                {
                    Confirmar();
                };

                btnCancelar.Click += (s, e) =>
                {
                    if (gravando) return;
                    DialogResult = DialogResult.Cancel;
                    Close();
                };

                AcceptButton = btnSalvar;
                CancelButton = btnCancelar;

                // Senhas são alteradas em outra janela.
                if (original != null)
                {
                    txtSenha.Enabled = false;
                    txtConfirmarSenha.Enabled = false;
                }
            }

            private Control CriarCampo(
                string titulo,
                Control entrada)
            {
                TableLayoutPanel grupo =
                    new TableLayoutPanel();

                grupo.Dock = DockStyle.Fill;

                grupo.ColumnCount = 1;
                grupo.RowCount = 2;

                grupo.Margin =
                    new Padding(0, 0, 12, 8);

                grupo.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 27F));

                grupo.RowStyles.Add(
                    new RowStyle(
                        SizeType.Percent, 100F));

                Label label = new Label();

                label.Text = titulo;

                label.Dock = DockStyle.Fill;

                label.Font = new Font(
                    "Segoe UI", 9F, FontStyle.Bold);

                label.ForeColor =
                    Color.FromArgb(35, 49, 45);

                entrada.Dock = DockStyle.Fill;

                entrada.Font =
                    new Font("Segoe UI", 10F);

                grupo.Controls.Add(
                    label, 0, 0);

                grupo.Controls.Add(
                    entrada, 0, 1);

                return grupo;
            }

            private void Preencher(
                FuncionarioDados funcionario)
            {
                txtNome.Text = funcionario.Nome;

                txtLogin.Text = funcionario.Login;

                txtTelefone.Text =
                    funcionario.Telefone;

                txtCRMV.Text =
                    funcionario.CRMV;

                txtTipo.Text =
                    funcionario.Tipo;

                switch (funcionario.Nivel)
                {
                    case "1":
                        cmbNivel.SelectedIndex = 0;
                        break;

                    case "2":
                        cmbNivel.SelectedIndex = 1;
                        break;

                    case "3":
                        cmbNivel.SelectedIndex = 2;
                        break;
                }
            }

            private async void Confirmar()
            {
                if (gravando) return;
                string nome =
                    txtNome.Text.Trim();

                string login =
                    txtLogin.Text.Trim();

                string telefone =
                    Regex.Replace(
                        txtTelefone.Text,
                        @"\D",
                        "");

                string crmv =
                    txtCRMV.Text.Trim();

                string tipo =
                    txtTipo.Text.Trim();

                string nivel =
                    (cmbNivel.SelectedIndex + 1)
                    .ToString();

                if (nome.Length == 0 ||
                    login.Length == 0)
                {
                    MessageBox.Show(
                        this,
                        "Informe o nome e o login.",
                        "Campos obrigatórios",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (telefone.Length != 10 &&
                    telefone.Length != 11)
                {
                    MessageBox.Show(
                        this,
                        "O telefone deve conter " +
                        "10 ou 11 dígitos.",
                        "Telefone inválido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (tipo.Length != 1)
                {
                    MessageBox.Show(
                        this,
                        "Informe um código de 1 caractere " +
                        "para Funcionarios_TIPO.",
                        "Tipo obrigatório",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (nivel == "3" &&
                    crmv.Length == 0)
                {
                    MessageBox.Show(
                        this,
                        "Informe o CRMV do veterinário.",
                        "CRMV obrigatório",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (original == null)
                {
                    if (txtSenha.Text.Length < 8)
                    {
                        MessageBox.Show(
                            this,
                            "A senha deve ter pelo menos " +
                            "8 caracteres.",
                            "Senha inválida",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    if (txtSenha.Text !=
                        txtConfirmarSenha.Text)
                    {
                        MessageBox.Show(
                            this,
                            "As senhas não coincidem.",
                            "Confirmação de senha",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }
                }

                Resultado = new FuncionarioDados
                {
                    Id = original == null
                        ? 0 : original.Id,

                    Nome = nome,

                    Login = login,

                    Senha = original == null
                        ? txtSenha.Text
                        : null,

                    Nivel = nivel,

                    Telefone = telefone,

                    CRMV = crmv,

                    Tipo = tipo
                };

                if (GravarAsync != null)
                {
                    gravando = true;
                    btnSalvarFormulario.Enabled = false;
                    btnCancelarFormulario.Enabled = false;
                    UseWaitCursor = true;
                    try
                    {
                        bool sucesso = await GravarAsync(Resultado);
                        if (!sucesso) return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this,
                            "Não foi possível salvar os dados.\n\n" + ex.Message,
                            "Med Vet", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                    finally
                    {
                        gravando = false;
                        btnSalvarFormulario.Enabled = true;
                        btnCancelarFormulario.Enabled = true;
                        UseWaitCursor = false;
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        // ==========================================
        // JANELA REDEFINIR SENHA
        // ==========================================

        private class EditorSenha : Form
        {
            private TextBox txtSenha;
            private TextBox txtConfirmacao;
            private Button btnConfirmarSenha;
            private bool gravando;

            public Func<string, Task<bool>> GravarAsync { get; set; }

            public string NovaSenha
            {
                get;
                private set;
            }

            public EditorSenha(string nomeFuncionario)
            {
                Text = "Med Vet - Redefinir Senha";

                ClientSize = new Size(480, 300);

                StartPosition =
                    FormStartPosition.CenterParent;

                FormBorderStyle =
                    FormBorderStyle.FixedDialog;

                MaximizeBox = false;
                MinimizeBox = false;

                BackColor =
                    Color.FromArgb(248, 250, 249);

                Font = new Font("Segoe UI", 10F);

                Label titulo = new Label();

                titulo.Text = "Redefinir senha";

                titulo.Font = new Font(
                    "Segoe UI", 19F, FontStyle.Bold);

                titulo.SetBounds(
                    22, 15, 420, 43);

                Controls.Add(titulo);

                Label nome = new Label();

                nome.Text =
                    "Funcionário: " + nomeFuncionario;

                nome.ForeColor = Color.Gray;

                nome.SetBounds(
                    24, 60, 430, 27);

                Controls.Add(nome);

                Label lblSenha = new Label();

                lblSenha.Text = "Nova senha";

                lblSenha.SetBounds(
                    24, 100, 420, 22);

                Controls.Add(lblSenha);

                txtSenha = new TextBox();

                txtSenha.UseSystemPasswordChar = true;
                txtSenha.MaxLength = 72;

                txtSenha.SetBounds(
                    24, 125, 430, 32);

                Controls.Add(txtSenha);

                Label lblConfirmacao = new Label();

                lblConfirmacao.Text =
                    "Confirmar nova senha";

                lblConfirmacao.SetBounds(
                    24, 167, 420, 22);

                Controls.Add(lblConfirmacao);

                txtConfirmacao = new TextBox();

                txtConfirmacao.UseSystemPasswordChar = true;
                txtConfirmacao.MaxLength = 72;

                txtConfirmacao.SetBounds(
                    24, 192, 430, 32);

                Controls.Add(txtConfirmacao);

                Button btnConfirmar = new Button();
                btnConfirmarSenha = btnConfirmar;

                btnConfirmar.Text = "Salvar nova senha";

                btnConfirmar.SetBounds(
                    269, 241, 185, 42);

                btnConfirmar.BackColor =
                    Color.FromArgb(20, 192, 138);

                btnConfirmar.ForeColor = Color.White;

                btnConfirmar.FlatStyle =
                    FlatStyle.Flat;

                btnConfirmar.FlatAppearance.BorderSize = 0;

                btnConfirmar.Click += (s, e) =>
                {
                    Confirmar();
                };

                Controls.Add(btnConfirmar);

                AcceptButton = btnConfirmar;
                FormClosing += (s, e) =>
                {
                    if (gravando && e.CloseReason == CloseReason.UserClosing)
                    {
                        e.Cancel = true;
                        MessageBox.Show(this,
                            "Aguarde a conclusão da alteração de senha.",
                            "Med Vet", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                };
            }

            private async void Confirmar()
            {
                if (gravando) return;
                if (txtSenha.Text.Length < 8)
                {
                    MessageBox.Show(
                        this,
                        "A senha deve ter pelo menos " +
                        "8 caracteres.",
                        "Senha inválida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (txtSenha.Text !=
                    txtConfirmacao.Text)
                {
                    MessageBox.Show(
                        this,
                        "As senhas não coincidem.",
                        "Confirmação de senha",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                NovaSenha = txtSenha.Text;

                if (GravarAsync != null)
                {
                    gravando = true;
                    btnConfirmarSenha.Enabled = false;
                    UseWaitCursor = true;
                    try
                    {
                        bool sucesso = await GravarAsync(NovaSenha);
                        if (!sucesso) return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this,
                            "Não foi possível redefinir a senha.\n\n" + ex.Message,
                            "Med Vet", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                    finally
                    {
                        gravando = false;
                        btnConfirmarSenha.Enabled = true;
                        UseWaitCursor = false;
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            }
        }

    }
}
