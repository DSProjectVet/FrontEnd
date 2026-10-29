using DSProjectVet.Forms.SCREENS;
using DSProjectVet.Dados.DataSet_ClinicaVetTableAdapters;
using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public partial class frmClientes : Form
    {
        private int clienteSelecionadoId = 0;
        private bool interfaceMontada = false;
        private bool buscaPlaceholder = true;
        private bool atualizandoLista = false;

        private readonly BindingSource fonteClientes = new BindingSource();
        private TextBox txtBusca;
        private Label lblTotal;
        private Label lblStatusSelecao;
        private Button btnEditarCliente;
        private Button btnExcluirCliente;

        // Mesma identidade visual da tela de animais
        private readonly Color verdePrincipal = Color.FromArgb(16, 176, 128);
        private readonly Color verdeEscuro = Color.FromArgb(10, 128, 99);
        private readonly Color verdeClaro = Color.FromArgb(225, 247, 240);
        private readonly Color fundoTela = Color.FromArgb(245, 248, 247);
        private readonly Color textoPrincipal = Color.FromArgb(31, 47, 43);
        private readonly Color textoSecundario = Color.FromArgb(107, 122, 118);
        private readonly Color borda = Color.FromArgb(225, 233, 229);

        public frmClientes()
        {
            InitializeComponent();
        }

        // =========================================================
        // INICIALIZAÇÃO
        // =========================================================
        private void frmClientes_Load(object sender, EventArgs e)
        {
            if (interfaceMontada) return;
            interfaceMontada = true;

            MontarInterface();

            // Mantém os eventos do Designer, sem duplicar cliques.
            btnCadastrar.Click -= btnCadastrar_Click;
            btnCadastrar.Click += btnCadastrar_Click;
            btnVerAnimais.Click -= button1_Click;
            btnVerAnimais.Click += button1_Click;
            dataGridViewClientes.CellEnter -= dataGridView1_CellEnter;
            dataGridViewClientes.CellEnter += dataGridView1_CellEnter;
            dataGridViewClientes.CellDoubleClick -= dataGridViewClientes_CellDoubleClick;
            dataGridViewClientes.CellDoubleClick += dataGridViewClientes_CellDoubleClick;

            CarregarClientes();
        }

        // =========================================================
        // INTERFACE - CONTROLES ORIGINAIS SÃO REUTILIZADOS
        // =========================================================
        private void MontarInterface()
        {
            BackColor = fundoTela;
            AutoScroll = false;
            MinimumSize = new Size(700, 440);

            Panel fundo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = fundoTela,
                Padding = new Padding(18)
            };
            Controls.Add(fundo);
            fundo.BringToFront();

            TableLayoutPanel colunas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = fundoTela,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty
            };
            colunas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 286F));
            colunas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            colunas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            fundo.Controls.Add(colunas);

            // PAINEL LATERAL
            Panel lateral = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 14, 0)
            };
            colunas.Controls.Add(lateral, 0, 0);

            Panel faixaLateral = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                BackColor = verdeClaro,
                Padding = new Padding(20, 16, 12, 8)
            };
            lateral.Controls.Add(faixaLateral);

            Label tituloLateral = new Label
            {
                Text = "CLIENTES",
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = verdeEscuro,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Label subtituloLateral = new Label
            {
                Text = "Gestão de tutores e responsáveis",
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9F),
                ForeColor = textoSecundario
            };
            faixaLateral.Controls.Add(subtituloLateral);
            faixaLateral.Controls.Add(tituloLateral);

            FlowLayoutPanel corpoLateral = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(19, 20, 13, 10)
            };
            lateral.Controls.Add(corpoLateral);
            corpoLateral.BringToFront();
            faixaLateral.BringToFront();

            Label lblSecao = CriarLabelLateral("Cliente selecionado", true);
            corpoLateral.Controls.Add(lblSecao);

            PrepararLabelExistente(lblNome, 15F, textoPrincipal, 62, true);
            lblNome.Margin = new Padding(0, 0, 0, 8);
            corpoLateral.Controls.Add(lblNome);

            corpoLateral.Controls.Add(CriarLabelLateral("CPF", true));
            PrepararLabelExistente(lblCpf, 9.5F, textoSecundario, 30, false);
            corpoLateral.Controls.Add(lblCpf);

            corpoLateral.Controls.Add(CriarLabelLateral("Telefone", true));
            PrepararLabelExistente(lblTelefone, 9.5F, textoSecundario, 30, false);
            corpoLateral.Controls.Add(lblTelefone);

            corpoLateral.Controls.Add(CriarLabelLateral("Endereço", true));
            PrepararLabelExistente(lblEndereço, 9.5F, textoSecundario, 54, false);
            corpoLateral.Controls.Add(lblEndereço);

            corpoLateral.Controls.Add(new Panel
            {
                Width = 230,
                Height = 15,
                Margin = Padding.Empty
            });

            // Cadastro e animais: mesmos controles do Designer.
            EstilizarBotao(btnCadastrar, true);
            btnCadastrar.Text = "+  Novo cliente";
            corpoLateral.Controls.Add(btnCadastrar);

            // Novos botões criados por código, sem modificar o Designer.
            btnEditarCliente = new Button { Text = "✎  Editar cliente" };
            EstilizarBotao(btnEditarCliente, false);
            corpoLateral.Controls.Add(btnEditarCliente);
            btnEditarCliente.Click += btnEditarCliente_Click;

            EstilizarBotao(btnVerAnimais, false);
            btnVerAnimais.Text = "Ver animais do cliente";
            corpoLateral.Controls.Add(btnVerAnimais);

            btnExcluirCliente = new Button { Text = "Excluir cliente" };
            EstilizarBotaoExclusao(btnExcluirCliente);
            corpoLateral.Controls.Add(btnExcluirCliente);
            btnExcluirCliente.Click += btnExcluirCliente_Click;

            lblStatusSelecao = new Label
            {
                Text = "Selecione uma linha na tabela para visualizar os dados e os animais do cliente.",
                Width = 228,
                Height = 75,
                AutoSize = false,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = textoSecundario,
                Margin = new Padding(1, 18, 0, 0)
            };
            corpoLateral.Controls.Add(lblStatusSelecao);

            // ÁREA CENTRAL: TÍTULO, PESQUISA, TABELA E CONTADOR
            Panel cartao = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(18),
                Margin = Padding.Empty
            };
            colunas.Controls.Add(cartao, 1, 0);

            TableLayoutPanel conteudo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty
            };
            conteudo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            conteudo.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
            conteudo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            conteudo.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            cartao.Controls.Add(conteudo);

            TableLayoutPanel topo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty
            };
            topo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));
            topo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));
            topo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            conteudo.Controls.Add(topo, 0, 0);

            Panel cabecalho = new Panel { Dock = DockStyle.Fill };
            topo.Controls.Add(cabecalho, 0, 0);

            Label titulo = new Label
            {
                Text = "Lista de clientes",
                Location = new Point(0, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = textoPrincipal
            };
            Label subtitulo = new Label
            {
                Text = "Visualize e gerencie os tutores cadastrados.",
                Location = new Point(0, 52),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = textoSecundario
            };
            cabecalho.Controls.Add(titulo);
            cabecalho.Controls.Add(subtitulo);

            Panel buscaBox = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(6, 24, 0, 28)
            };
            topo.Controls.Add(buscaBox, 1, 0);

            txtBusca = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = textoSecundario,
                Text = "Pesquisar cliente..."
            };
            buscaBox.Controls.Add(txtBusca);
            txtBusca.Enter += (s, e) =>
            {
                if (!buscaPlaceholder) return;
                buscaPlaceholder = false;
                txtBusca.Text = "";
                txtBusca.ForeColor = textoPrincipal;
            };
            txtBusca.Leave += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtBusca.Text)) return;
                buscaPlaceholder = true;
                txtBusca.ForeColor = textoSecundario;
                txtBusca.Text = "Pesquisar cliente...";
            };
            txtBusca.TextChanged += (s, e) => AplicarBusca();

            // Usa o DataGridView existente, sem criar outro no Designer.
            EstilizarDataGridView();
            dataGridViewClientes.DataSource = null;
            dataGridViewClientes.Columns.Clear();
            dataGridViewClientes.AutoGenerateColumns = true;
            dataGridViewClientes.Margin = Padding.Empty;
            dataGridViewClientes.Dock = DockStyle.Fill;
            conteudo.Controls.Add(dataGridViewClientes, 0, 1);

            Panel rodape = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = fundoTela,
                Padding = new Padding(12, 12, 10, 0),
                Margin = Padding.Empty
            };
            conteudo.Controls.Add(rodape, 0, 2);

            lblTotal = new Label
            {
                Text = "Total de clientes: 0",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = verdeEscuro,
                TextAlign = ContentAlignment.MiddleLeft
            };
            rodape.Controls.Add(lblTotal);

            fonteClientes.ListChanged += (s, e) => AtualizarContador();
        }

        private Label CriarLabelLateral(string texto, bool negrito)
        {
            return new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 9F,
                    negrito ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = textoSecundario,
                Width = 230,
                Height = 24,
                AutoSize = false,
                Margin = new Padding(0, 3, 0, 0)
            };
        }

        private void PrepararLabelExistente(
            Label label, float tamanho, Color cor, int altura, bool negrito)
        {
            label.AutoSize = false;
            label.Font = new Font("Segoe UI", tamanho,
                negrito ? FontStyle.Bold : FontStyle.Regular);
            label.ForeColor = cor;
            label.Width = 230;
            label.Height = altura;
            label.Margin = new Padding(0, 0, 0, 8);
        }

        private void EstilizarBotao(Button botao, bool primario)
        {
            botao.Size = new Size(228, 48);
            botao.Margin = new Padding(0, 0, 0, 12);
            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = primario ? 0 : 1;
            botao.FlatAppearance.BorderColor = Color.FromArgb(171, 222, 203);
            botao.BackColor = primario ? verdePrincipal : verdeClaro;
            botao.ForeColor = primario ? Color.White : verdeEscuro;
            botao.Cursor = Cursors.Hand;
            botao.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            botao.UseVisualStyleBackColor = false;
            botao.FlatAppearance.MouseOverBackColor =
                primario ? verdeEscuro : Color.FromArgb(205, 239, 226);
        }

        private void EstilizarBotaoExclusao(Button botao)
        {
            botao.Size = new Size(228, 45);
            botao.Margin = new Padding(0, 2, 0, 12);
            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = 1;
            botao.FlatAppearance.BorderColor = Color.FromArgb(244, 188, 188);
            botao.BackColor = Color.FromArgb(255, 241, 241);
            botao.ForeColor = Color.FromArgb(177, 48, 54);
            botao.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 224, 224);
            botao.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            botao.Cursor = Cursors.Hand;
            botao.UseVisualStyleBackColor = false;
        }

        // =========================================================
        // TABELA
        // =========================================================
        private void EstilizarDataGridView()
        {
            dataGridViewClientes.EnableHeadersVisualStyles = false;
            dataGridViewClientes.BackgroundColor = Color.White;
            dataGridViewClientes.BorderStyle = BorderStyle.None;
            dataGridViewClientes.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewClientes.GridColor = borda;
            dataGridViewClientes.RowHeadersVisible = false;
            dataGridViewClientes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridViewClientes.MultiSelect = false;
            dataGridViewClientes.ReadOnly = true;
            dataGridViewClientes.AllowUserToAddRows = false;
            dataGridViewClientes.AllowUserToDeleteRows = false;
            dataGridViewClientes.AllowUserToResizeRows = false;
            dataGridViewClientes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewClientes.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.None;
            dataGridViewClientes.RowTemplate.Height = 46;
            dataGridViewClientes.ColumnHeadersHeight = 48;
            dataGridViewClientes.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewClientes.ColumnHeadersDefaultCellStyle.BackColor =
                verdePrincipal;
            dataGridViewClientes.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dataGridViewClientes.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewClientes.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(8, 0, 0, 0);
            dataGridViewClientes.DefaultCellStyle.BackColor = Color.White;
            dataGridViewClientes.DefaultCellStyle.ForeColor = textoPrincipal;
            dataGridViewClientes.DefaultCellStyle.Font =
                new Font("Segoe UI", 9.5F);
            dataGridViewClientes.DefaultCellStyle.Padding =
                new Padding(9, 0, 7, 0);
            dataGridViewClientes.DefaultCellStyle.SelectionBackColor = verdeClaro;
            dataGridViewClientes.DefaultCellStyle.SelectionForeColor = verdeEscuro;
            dataGridViewClientes.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(249, 252, 250);
        }

        private void ConfigurarColunas()
        {
            DataGridViewColumnCollection c = dataGridViewClientes.Columns;
            if (c.Count == 0) return;

            if (c.Contains("ID_Cliente")) c["ID_Cliente"].Visible = false;
            DefinirColuna("Nome", "Nome do cliente", 130);
            DefinirColuna("CPF", "CPF", 92);
            DefinirColuna("Telefone", "Telefone", 100);
            DefinirColuna("Endereco", "Endereço", 155);
        }

        private void DefinirColuna(string nome, string titulo, float peso)
        {
            if (!dataGridViewClientes.Columns.Contains(nome)) return;
            DataGridViewColumn coluna = dataGridViewClientes.Columns[nome];
            coluna.HeaderText = titulo;
            coluna.FillWeight = peso;
        }

        // =========================================================
        // DADOS E PESQUISA
        // =========================================================
        public void CarregarClientes()
        {
            try
            {
                atualizandoLista = true;
                clientesTableAdapter.Fill(dataSet_ClinicaVet.Clientes);
                fonteClientes.DataSource = dataSet_ClinicaVet.Clientes;
                dataGridViewClientes.DataSource = fonteClientes;
                ConfigurarColunas();
                AplicarBusca();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível carregar os clientes.\n\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                atualizandoLista = false;
            }

            // A linha não é selecionada automaticamente: evita ver animais
            // de um cliente que o usuário ainda não escolheu.
            LimparDetalhesCliente();
            dataGridViewClientes.ClearSelection();
            dataGridViewClientes.CurrentCell = null;
            AtualizarContador();
        }

        private static string EscaparFiltro(string valor)
        {
            StringBuilder seguro = new StringBuilder();
            foreach (char caractere in valor)
            {
                if (caractere == '\'') seguro.Append("''");
                else if (caractere == '[') seguro.Append("[[]");
                else if (caractere == ']') seguro.Append("[]]");
                else if (caractere == '%') seguro.Append("[%]");
                else if (caractere == '*') seguro.Append("[*]");
                else seguro.Append(caractere);
            }
            return seguro.ToString();
        }

        private void AplicarBusca()
        {
            if (fonteClientes.DataSource == null) return;

            bool carregamentoEmAndamento = atualizandoLista;
            atualizandoLista = true;

            try
            {
                string termo = buscaPlaceholder || txtBusca == null
                    ? "" : txtBusca.Text.Trim();

                if (termo.Length == 0)
                {
                    fonteClientes.RemoveFilter();
                    return;
                }

                DataTable dados = fonteClientes.DataSource as DataTable;
                if (dados == null) return;

                string valor = EscaparFiltro(termo);
                string filtro = "";

                foreach (string nome in new[] { "Nome", "CPF", "Telefone", "Endereco" })
                {
                    if (!dados.Columns.Contains(nome)) continue;
                    if (filtro.Length > 0) filtro += " OR ";
                    filtro += "IsNull([" + nome + "], '') LIKE '%" + valor + "%'";
                }

                try
                {
                    if (filtro.Length == 0) fonteClientes.RemoveFilter();
                    else fonteClientes.Filter = filtro;
                }
                catch (EvaluateException)
                {
                    fonteClientes.RemoveFilter();
                }
                catch (SyntaxErrorException)
                {
                    fonteClientes.RemoveFilter();
                }
            }
            finally
            {
                atualizandoLista = carregamentoEmAndamento;

                // Após uma busca nova, exige que o usuário escolha
                // explicitamente o cliente correto na tabela.
                if (!carregamentoEmAndamento)
                {
                    dataGridViewClientes.ClearSelection();
                    dataGridViewClientes.CurrentCell = null;
                    LimparDetalhesCliente();
                }

                AtualizarContador();
            }
        }

        private void AtualizarContador()
        {
            if (lblTotal == null) return;
            int total = dataSet_ClinicaVet.Clientes.Rows.Count;
            int exibidos = fonteClientes.Count;
            lblTotal.Text = exibidos == total
                ? "Total de clientes: " + total
                : "Clientes encontrados: " + exibidos + " de " + total;
        }

        // =========================================================
        // SELEÇÃO E DETALHES
        // =========================================================
        private void dataGridView1_CellEnter(
            object sender, DataGridViewCellEventArgs e)
        {
            if (atualizandoLista || e.RowIndex < 0 ||
                e.RowIndex >= dataGridViewClientes.Rows.Count) return;

            DataGridViewRow linha = dataGridViewClientes.Rows[e.RowIndex];
            DataRowView dados = linha.DataBoundItem as DataRowView;
            if (dados == null) return;

            clienteSelecionadoId = Convert.ToInt32(dados["ID_Cliente"]);
            lblNome.Text = TextoOuPadrao(dados["Nome"], "Não informado");
            lblCpf.Text = FormatarCPF(Convert.ToString(dados["CPF"]));
            lblTelefone.Text = TextoOuPadrao(dados["Telefone"], "Não informado");
            lblEndereço.Text = TextoOuPadrao(dados["Endereco"], "Não informado");

            if (lblStatusSelecao != null)
                lblStatusSelecao.Text =
                    "Cliente selecionado. Clique em 'Ver animais' para acessar os registros vinculados.";
        }

        private static string TextoOuPadrao(object valor, string padrao)
        {
            string texto = valor == null || valor == DBNull.Value
                ? "" : Convert.ToString(valor);
            return string.IsNullOrWhiteSpace(texto) ? padrao : texto;
        }

        private void LimparDetalhesCliente()
        {
            clienteSelecionadoId = 0;
            lblNome.Text = "Selecione um cliente";
            lblCpf.Text = "CPF será exibido aqui";
            lblTelefone.Text = "Telefone será exibido aqui";
            lblEndereço.Text = "Endereço será exibido aqui";

            if (lblStatusSelecao != null)
                lblStatusSelecao.Text =
                    "Selecione uma linha na tabela para visualizar os dados e os animais do cliente.";
        }

        private string FormatarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return "CPF não informado";

            StringBuilder numeros = new StringBuilder();
            foreach (char c in cpf)
                if (char.IsDigit(c)) numeros.Append(c);

            if (numeros.Length == 11)
                return Convert.ToUInt64(numeros.ToString())
                    .ToString(@"000\.000\.000\-00");
            return cpf;
        }

        private void dataGridViewClientes_CellDoubleClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                button1_Click(sender, EventArgs.Empty);
        }

        // =========================================================
        // AÇÕES ORIGINAIS
        // =========================================================
        private void button1_Click(object sender, EventArgs e)
        {
            if (clienteSelecionadoId == 0)
            {
                MessageBox.Show(
                    "Selecione um cliente primeiro.",
                    "Cliente não selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (frmAnimais telaAnimais = new frmAnimais(clienteSelecionadoId))
            {
                telaAnimais.ShowDialog();
            }
            // Dados do cliente continuam disponíveis após fechar os animais.
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            using (frmNovoCliente telaCadastro = new frmNovoCliente())
            {
                telaCadastro.ShowDialog();
            }
            CarregarClientes();
        }

        // =========================================================
        // EDITAR / EXCLUIR CLIENTE
        // =========================================================

        private DataRow ObterRegistroSelecionado()
        {
            if (clienteSelecionadoId <= 0 ||
                dataGridViewClientes.CurrentRow == null ||
                !dataGridViewClientes.CurrentRow.Selected)
            {
                MessageBox.Show(
                    "Selecione um cliente na tabela primeiro.",
                    "Nenhum cliente selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return null;
            }

            DataRowView exibido =
                dataGridViewClientes.CurrentRow.DataBoundItem as DataRowView;

            if (exibido == null ||
                Convert.ToInt32(exibido["ID_Cliente"]) != clienteSelecionadoId)
            {
                MessageBox.Show(
                    "Não foi possível identificar o cliente selecionado. " +
                    "Selecione uma linha novamente.",
                    "Seleção inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return null;
            }

            return exibido.Row;
        }

        private TextBox AdicionarCampoCliente(
            FlowLayoutPanel painel, string titulo, object valor,
            bool multilinha = false)
        {
            Label legenda = new Label
            {
                Text = titulo,
                Width = 420,
                Height = 24,
                AutoSize = false,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = textoPrincipal,
                Margin = new Padding(0, 8, 0, 2)
            };
            painel.Controls.Add(legenda);

            TextBox campo = new TextBox
            {
                Text = valor == null || valor == DBNull.Value
                    ? "" : Convert.ToString(valor),
                Width = 420,
                Height = multilinha ? 68 : 37,
                AutoSize = false,
                Multiline = multilinha,
                ScrollBars = multilinha ? ScrollBars.Vertical : ScrollBars.None,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F),
                ForeColor = textoPrincipal,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 5)
            };
            painel.Controls.Add(campo);
            return campo;
        }

        private void btnEditarCliente_Click(object sender, EventArgs e)
        {
            DataRow registro = ObterRegistroSelecionado();
            if (registro == null) return;

            int id = Convert.ToInt32(registro["ID_Cliente"]);

            using (Form tela = new Form())
            {
                tela.Text = "Editar cliente #" + id;
                tela.ClientSize = new Size(505, 580);
                tela.MinimumSize = new Size(490, 460);
                tela.StartPosition = FormStartPosition.CenterParent;
                tela.FormBorderStyle = FormBorderStyle.FixedDialog;
                tela.MaximizeBox = false;
                tela.MinimizeBox = false;
                tela.BackColor = fundoTela;

                FlowLayoutPanel painel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    AutoScroll = true,
                    Padding = new Padding(24, 20, 12, 20),
                    BackColor = Color.White
                };
                tela.Controls.Add(painel);

                painel.Controls.Add(new Label
                {
                    Text = "Editar dados do cliente",
                    Width = 430,
                    Height = 42,
                    Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                    ForeColor = textoPrincipal,
                    Margin = new Padding(0, 0, 0, 0)
                });
                painel.Controls.Add(new Label
                {
                    Text = "Atualize as informações do tutor selecionado.",
                    Width = 430,
                    Height = 29,
                    Font = new Font("Segoe UI", 9.5F),
                    ForeColor = textoSecundario,
                    Margin = new Padding(0, 0, 0, 5)
                });

                TextBox txtNome = AdicionarCampoCliente(
                    painel, "Nome completo *", registro["Nome"]);
                TextBox txtCpf = AdicionarCampoCliente(
                    painel, "CPF", registro["CPF"]);
                TextBox txtTelefone = AdicionarCampoCliente(
                    painel, "Telefone", registro["Telefone"]);
                TextBox txtEndereco = AdicionarCampoCliente(
                    painel, "Endereço", registro["Endereco"], true);

                FlowLayoutPanel acoes = new FlowLayoutPanel
                {
                    Width = 435,
                    Height = 62,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    Margin = new Padding(0, 18, 0, 0)
                };
                painel.Controls.Add(acoes);

                Button btnSalvar = new Button { Text = "Salvar alterações" };
                EstilizarBotao(btnSalvar, true);
                btnSalvar.Size = new Size(215, 45);
                btnSalvar.Margin = new Padding(0, 0, 10, 0);
                acoes.Controls.Add(btnSalvar);

                Button btnCancelar = new Button { Text = "Cancelar" };
                EstilizarBotao(btnCancelar, false);
                btnCancelar.Size = new Size(190, 45);
                btnCancelar.Margin = Padding.Empty;
                acoes.Controls.Add(btnCancelar);
                btnCancelar.Click += (s, ev) => tela.Close();

                btnSalvar.Click += (s, ev) =>
                {

                    if (string.IsNullOrWhiteSpace(txtNome.Text))
                    {
                        MessageBox.Show(
                            "Informe o nome do cliente.",
                            "Campo obrigatório",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        txtNome.Focus();
                        return;
                    }

                    try
                    {
                        registro["Nome"] = txtNome.Text.Trim();
                        registro["CPF"] = txtCpf.Text.Trim();
                        registro["Telefone"] = txtTelefone.Text.Trim();
                        registro["Endereco"] = txtEndereco.Text.Trim();

                        if (registro.RowState == DataRowState.Modified)
                        {
                            int alterados = clientesTableAdapter.Update(
                                dataSet_ClinicaVet.Clientes);
                            if (alterados == 0)
                                throw new Exception(
                                    "Nenhuma alteração foi gravada no banco.");
                        }

                        MessageBox.Show(
                            "Dados do cliente salvos com sucesso!",
                            "Cadastro atualizado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        tela.DialogResult = DialogResult.OK;
                        tela.Close();
                        CarregarClientes();
                        SelecionarClientePorId(id);
                    }
                    catch (Exception ex)
                    {
                        if (registro.RowState != DataRowState.Detached &&
                            registro.RowState != DataRowState.Deleted)
                            registro.RejectChanges();

                        MessageBox.Show(
                            "Não foi possível editar o cliente.\n\n" + ex.Message,
                            "Erro na edição",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                };

                tela.ShowDialog(this);
            }
        }

        private void SelecionarClientePorId(int id)
        {
            // Mantém a pesquisa atual e só seleciona se o cliente estiver visível.
            foreach (DataGridViewRow linha in dataGridViewClientes.Rows)
            {
                DataRowView dados = linha.DataBoundItem as DataRowView;
                if (dados == null ||
                    Convert.ToInt32(dados["ID_Cliente"]) != id)
                    continue;

                foreach (DataGridViewCell celula in linha.Cells)
                {
                    if (!celula.Visible) continue;
                    dataGridViewClientes.CurrentCell = celula;
                    linha.Selected = true;
                    return;
                }
            }
        }

        private void btnExcluirCliente_Click(object sender, EventArgs e)
        {
            DataRow registro = ObterRegistroSelecionado();
            if (registro == null) return;

            int id = Convert.ToInt32(registro["ID_Cliente"]);
            string nome = Convert.ToString(registro["Nome"]);

            // Impede exclusão de clientes com animais relacionados.
            // Se a consulta falhar, interrompe a operação por segurança.
            try
            {
                AnimaisTableAdapter animaisAdapter =
                    new AnimaisTableAdapter();
                DataTable animais = animaisAdapter.GetDataByCliente(id);
                if (animais.Rows.Count > 0)
                {
                    MessageBox.Show(
                        "O cliente '" + nome + "' possui " +
                        animais.Rows.Count + " animal(is) cadastrado(s).\n\n" +
                        "Para preservar os registros, a exclusão foi bloqueada. " +
                        "Verifique os animais vinculados antes de excluir o cliente.",
                        "Cliente com animais vinculados",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível verificar os animais do cliente. " +
                    "A exclusão foi cancelada por segurança.\n\n" + ex.Message,
                    "Verificação indisponível",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            DialogResult confirmacao = MessageBox.Show(
                "Excluir definitivamente o cliente '" + nome + "'?\n\n" +
                "Essa ação não poderá ser desfeita.",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirmacao != DialogResult.Yes) return;

            try
            {
                registro.Delete();
                int excluidos = clientesTableAdapter.Update(
                    dataSet_ClinicaVet.Clientes);

                if (excluidos <= 0)
                    throw new Exception("Nenhum registro foi excluído.");

                MessageBox.Show(
                    "Cliente excluído com sucesso!",
                    "Exclusão realizada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                CarregarClientes();
            }
            catch (Exception ex)
            {
                if (registro.RowState != DataRowState.Detached)
                    registro.RejectChanges();

                MessageBox.Show(
                    "Não foi possível excluir o cliente.\n\n" +
                    "Ele pode possuir outros registros relacionados no banco " +
                    "ou o comando DELETE do TableAdapter pode não estar configurado.\n\n" +
                    ex.Message,
                    "Erro na exclusão",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // EVENTOS GERADOS PELO DESIGNER (MANTIDOS)
        // =========================================================
        private void dataGridView1_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        { }

        private void label1_Click(object sender, EventArgs e) { }

        private void label7_Click(object sender, EventArgs e) { }

        private void dataGridViewClientes_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        { }
    }
}
