using DSProjectVet.Dados.DataSet_ClinicaVetTableAdapters;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{
    public partial class frmAnimais : Form
    {
        private int? clienteId;
        private bool interfaceMontada = false;
        private bool buscaPlaceholder = true;
        private BindingSource fonteAnimais = new BindingSource();
        private TextBox txtBusca;
        private Label lblTotal;

        // Identidade visual do Med Vet
        private readonly Color verdePrincipal = Color.FromArgb(16, 176, 128);
        private readonly Color verdeEscuro = Color.FromArgb(10, 128, 99);
        private readonly Color verdeClaro = Color.FromArgb(225, 247, 240);
        private readonly Color fundoTela = Color.FromArgb(245, 248, 247);
        private readonly Color textoPrincipal = Color.FromArgb(31, 47, 43);
        private readonly Color textoSecundario = Color.FromArgb(107, 122, 118);
        private readonly Color borda = Color.FromArgb(225, 233, 229);

        public frmAnimais()
        {
            InitializeComponent();
        }

        public frmAnimais(int idCliente)
        {
            InitializeComponent();
            clienteId = idCliente;
        }

        private void frmAnimais_Load(object sender, EventArgs e)
        {
            if (interfaceMontada) return;
            interfaceMontada = true;

            MontarInterface();
            ConfigurarCabecalhoCliente();
            CarregarAnimais();

            btnNovoAnimal.Click -= btnNovoAnimal_Click;
            btnNovoAnimal.Click += btnNovoAnimal_Click;
            btnEditar.Click -= btnEditar_Click;
            btnEditar.Click += btnEditar_Click;
        }

        // =========================================================
        // INTERFACE PRINCIPAL: CONTROLES DO DESIGNER SÃO REUTILIZADOS
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
            colunas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 286));
            colunas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            colunas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            fundo.Controls.Add(colunas);

            // LATERAL
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
                Text = "ANIMAIS",
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = verdeEscuro,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Label subtituloLateral = new Label
            {
                Text = "Gestão de pacientes veterinários",
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

            Label lblSecao = new Label
            {
                Text = "Animais do cliente",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = textoSecundario,
                Width = 230,
                Height = 26
            };
            corpoLateral.Controls.Add(lblSecao);

            lblNome.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblNome.ForeColor = textoPrincipal;
            lblNome.Width = 232;
            lblNome.Height = 62;
            lblNome.AutoSize = false;
            lblNome.Margin = new Padding(0, 0, 0, 8);
            corpoLateral.Controls.Add(lblNome);

            Label lblCpfTitulo = new Label
            {
                Text = "CPF / informações",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = textoSecundario,
                Width = 230,
                Height = 25
            };
            corpoLateral.Controls.Add(lblCpfTitulo);

            lblCpf.Font = new Font("Segoe UI", 9F);
            lblCpf.ForeColor = textoSecundario;
            lblCpf.Width = 232;
            lblCpf.Height = 64;
            lblCpf.AutoSize = false;
            corpoLateral.Controls.Add(lblCpf);

            Panel espaco = new Panel { Width = 230, Height = 18 };
            corpoLateral.Controls.Add(espaco);

            EstilizarBotao(btnNovoAnimal, true);
            btnNovoAnimal.Text = "Novo animal";
            corpoLateral.Controls.Add(btnNovoAnimal);

            EstilizarBotao(btnEditar, false);
            btnEditar.Text = "Editar selecionado";
            corpoLateral.Controls.Add(btnEditar);

            Label dica = new Label
            {
                Text = "Selecione uma linha na tabela para editar ou excluir o animal.",
                Width = 228,
                Height = 78,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = textoSecundario,
                Margin = new Padding(1, 20, 0, 0)
            };
            corpoLateral.Controls.Add(dica);

            // ÁREA DA LISTA
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
            conteudo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            conteudo.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
            conteudo.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            conteudo.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            cartao.Controls.Add(conteudo);

            TableLayoutPanel topo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty
            };
            topo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57));
            topo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43));
            topo.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            conteudo.Controls.Add(topo, 0, 0);

            Panel cabecalho = new Panel { Dock = DockStyle.Fill };
            topo.Controls.Add(cabecalho, 0, 0);
            Label titulo = new Label
            {
                Text = "Lista de animais",
                Location = new Point(0, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = textoPrincipal
            };
            Label subtitulo = new Label
            {
                Text = "Visualize e gerencie os animais cadastrados.",
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
                Text = "Pesquisar animal..."
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
                txtBusca.Text = "Pesquisar animal...";
            };
            txtBusca.TextChanged += (s, e) => AplicarBusca();

            EstilizarDataGridView();
            dataGridView1.Margin = Padding.Empty;
            dataGridView1.Dock = DockStyle.Fill;
            conteudo.Controls.Add(dataGridView1, 0, 1);
            dataGridView1.CellDoubleClick -= dataGridView1_CellDoubleClick;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;

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
                Text = "Total de animais: 0",
                Dock = DockStyle.Left,
                Width = 210,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = verdeEscuro,
                TextAlign = ContentAlignment.MiddleLeft
            };
            rodape.Controls.Add(lblTotal);
            fonteAnimais.ListChanged += (s, e) => AtualizarContador();
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
            botao.FlatAppearance.MouseOverBackColor = primario ? verdeEscuro : Color.FromArgb(205, 239, 226);
        }

        private void EstilizarDataGridView()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.GridColor = borda;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowTemplate.Height = 46;
            dataGridView1.ColumnHeadersHeight = 48;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = verdePrincipal;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = textoPrincipal;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dataGridView1.DefaultCellStyle.Padding = new Padding(9, 0, 7, 0);
            dataGridView1.DefaultCellStyle.SelectionBackColor = verdeClaro;
            dataGridView1.DefaultCellStyle.SelectionForeColor = verdeEscuro;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 252, 250);
            dataGridView1.CellFormatting -= dataGridView1_CellFormatting;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.Value == null) return;
            if (dataGridView1.Columns[e.ColumnIndex].Name != "Sexo") return;
            string sexo = Convert.ToString(e.Value).Trim().ToUpperInvariant();
            if (sexo == "M") { e.Value = "Macho"; e.FormattingApplied = true; }
            if (sexo == "F") { e.Value = "Fêmea"; e.FormattingApplied = true; }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnEditar_Click(sender, EventArgs.Empty);
        }

        // =========================================================
        // DADOS E BUSCA
        // =========================================================
        private void CarregarAnimais()
        {
            try
            {
                AnimaisTableAdapter adapter = new AnimaisTableAdapter();
                DataTable dados = clienteId.HasValue
                    ? (DataTable)adapter.GetDataByCliente(clienteId.Value)
                    : (DataTable)adapter.GetData();

                fonteAnimais.DataSource = dados;
                dataGridView1.DataSource = fonteAnimais;
                ConfigurarColunas();
                AplicarBusca();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar os animais.\n\n" + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string EscaparFiltro(string valor)
        {
            System.Text.StringBuilder seguro = new System.Text.StringBuilder();
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
            if (fonteAnimais.DataSource == null) return;
            string termo = buscaPlaceholder || txtBusca == null ? "" : txtBusca.Text.Trim();
            if (termo.Length == 0)
            {
                fonteAnimais.RemoveFilter();
                AtualizarContador();
                return;
            }

            DataTable dados = fonteAnimais.DataSource as DataTable;
            if (dados == null) return;
            string valor = EscaparFiltro(termo);
            string filtro = "";
            foreach (string nome in new[] { "Nome", "Raca", "Especie", "Descricao" })
            {
                if (!dados.Columns.Contains(nome)) continue;
                if (filtro.Length > 0) filtro += " OR ";
                filtro += "IsNull([" + nome + "], '') LIKE '%" + valor + "%'";
            }
            try
            {
                fonteAnimais.Filter = filtro;
            }
            catch (EvaluateException)
            {
                fonteAnimais.RemoveFilter();
            }
            catch (SyntaxErrorException)
            {
                fonteAnimais.RemoveFilter();
            }
            AtualizarContador();
        }

        private void AtualizarContador()
        {
            if (lblTotal != null)
                lblTotal.Text = "Total de animais: " + fonteAnimais.Count;
        }

        private void ConfigurarColunas()
        {
            DataGridViewColumnCollection c = dataGridView1.Columns;
            if (c.Count == 0) return;

            if (c.Contains("ID_Animal")) c["ID_Animal"].Visible = false;
            if (c.Contains("fk_Clientes_ID_Cliente")) c["fk_Clientes_ID_Cliente"].Visible = false;
            DefinirColuna("Nome", "Nome", 95);
            DefinirColuna("Raca", "Raça", 80);
            DefinirColuna("Sexo", "Sexo", 67);
            DefinirColuna("Especie", "Espécie", 83);
            DefinirColuna("Data_Nascimento", "Nascimento", 98);
            DefinirColuna("Descricao", "Descrição", 150);

            if (c.Contains("Data_Nascimento"))
                c["Data_Nascimento"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void DefinirColuna(string nome, string titulo, float peso)
        {
            if (!dataGridView1.Columns.Contains(nome)) return;
            DataGridViewColumn coluna = dataGridView1.Columns[nome];
            coluna.HeaderText = titulo;
            coluna.FillWeight = peso;
        }

        private void ConfigurarCabecalhoCliente()
        {
            if (!clienteId.HasValue)
            {
                lblNome.Text = "Todos os animais";
                lblCpf.Text = "Visualização geral dos animais cadastrados";
                return;
            }
            try
            {
                ClientesTableAdapter adapter = new ClientesTableAdapter();
                foreach (DataRow cliente in adapter.GetData().Rows)
                {
                    if (Convert.ToInt32(cliente["ID_Cliente"]) != clienteId.Value) continue;
                    lblNome.Text = Convert.ToString(cliente["Nome"]);
                    lblCpf.Text = "CPF: " + Convert.ToString(cliente["CPF"]);
                    return;
                }
                lblNome.Text = "Cliente não encontrado";
                lblCpf.Text = "";
            }
            catch (Exception ex)
            {
                lblNome.Text = "Cliente";
                lblCpf.Text = "";
                MessageBox.Show("Erro ao carregar os dados do proprietário.\n\n" + ex.Message,
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =========================================================
        // MODAIS E CAMPOS: PADRÃO VISUAL REUTILIZÁVEL
        // =========================================================
        private Form CriarJanela(string titulo, string descricao, out FlowLayoutPanel campos, out Panel rodape)
        {
            Form janela = new Form
            {
                Text = titulo,
                Size = new Size(520, 730),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = fundoTela,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            TableLayoutPanel estrutura = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = Padding.Empty
            };
            estrutura.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            estrutura.RowStyles.Add(new RowStyle(SizeType.Absolute, 98));
            estrutura.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            estrutura.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
            janela.Controls.Add(estrutura);

            Panel cabecalho = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            Label lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = textoPrincipal,
                Location = new Point(24, 17),
                AutoSize = true
            };
            Label lblDescricao = new Label
            {
                Text = descricao,
                Font = new Font("Segoe UI", 9F),
                ForeColor = textoSecundario,
                Location = new Point(26, 58),
                AutoSize = true
            };
            cabecalho.Controls.Add(lblTitulo);
            cabecalho.Controls.Add(lblDescricao);
            estrutura.Controls.Add(cabecalho, 0, 0);

            campos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(24, 17, 14, 14),
                BackColor = fundoTela,
                Margin = Padding.Empty
            };
            estrutura.Controls.Add(campos, 0, 1);

            rodape = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(24, 17, 24, 16)
            };
            estrutura.Controls.Add(rodape, 0, 2);
            return janela;
        }

        private Label CriarRotulo(string texto)
        {
            return new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = textoPrincipal,
                Width = 440,
                Height = 23,
                Margin = new Padding(0, 4, 0, 0)
            };
        }

        private TextBox CriarCampoEdicao(FlowLayoutPanel painel, string titulo, object valor)
        {
            painel.Controls.Add(CriarRotulo(titulo));
            TextBox campo = new TextBox
            {
                Width = 440,
                Height = 32,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F),
                ForeColor = textoPrincipal,
                BackColor = Color.White,
                Text = valor == null || valor == DBNull.Value ? "" : Convert.ToString(valor),
                Margin = new Padding(0, 0, 0, 12)
            };
            painel.Controls.Add(campo);
            return campo;
        }

        private ComboBox CriarCampoSexo(FlowLayoutPanel painel, object valor)
        {
            painel.Controls.Add(CriarRotulo("Sexo *"));
            ComboBox combo = new ComboBox
            {
                Width = 440,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(0, 0, 0, 12)
            };
            combo.Items.Add("Macho");
            combo.Items.Add("Fêmea");
            string sexo = valor == null || valor == DBNull.Value
                ? "" : Convert.ToString(valor).Trim().ToUpperInvariant();
            if (sexo == "M" || sexo == "MACHO") combo.SelectedIndex = 0;
            if (sexo == "F" || sexo == "FÊMEA" || sexo == "FEMEA") combo.SelectedIndex = 1;
            painel.Controls.Add(combo);
            return combo;
        }

        private DateTimePicker CriarCampoNascimento(FlowLayoutPanel painel, object valor)
        {
            painel.Controls.Add(CriarRotulo("Data de nascimento"));
            DateTimePicker campo = new DateTimePicker
            {
                Width = 440,
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                MaxDate = DateTime.Today,
                ShowCheckBox = true,
                Checked = false,
                Margin = new Padding(0, 0, 0, 12)
            };
            if (valor != null && valor != DBNull.Value)
            {
                DateTime data = Convert.ToDateTime(valor);
                if (data >= campo.MinDate && data <= campo.MaxDate)
                {
                    campo.Value = data;
                    campo.Checked = true;
                }
            }
            painel.Controls.Add(campo);
            return campo;
        }

        private void SalvarDataNascimento(DataRow animal, DateTimePicker campo)
        {
            if (!animal.Table.Columns.Contains("Data_Nascimento"))
                throw new Exception("Coluna Data_Nascimento não encontrada no DataSet.");
            if (campo.Checked)
                animal["Data_Nascimento"] = campo.Value.Date;
            else if (animal.Table.Columns["Data_Nascimento"].AllowDBNull)
                animal["Data_Nascimento"] = DBNull.Value;
            else
                throw new Exception("O banco exige a data de nascimento.");
        }

        private Button CriarBotaoModal(string texto, bool principal)
        {
            Button botao = new Button
            {
                Text = texto,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = principal ? verdePrincipal : Color.FromArgb(254, 235, 237),
                ForeColor = principal ? Color.White : Color.FromArgb(176, 44, 65),
                Cursor = Cursors.Hand
            };
            botao.FlatAppearance.BorderSize = 0;
            return botao;
        }

        private void CriarRodape(Panel painel, Button salvar, Button excluir)
        {
            TableLayoutPanel colunas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = excluir == null ? 1 : 2,
                RowCount = 1
            };
            if (excluir == null)
                colunas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            else
            {
                colunas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
                colunas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            }
            colunas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            painel.Controls.Add(colunas);
            salvar.Margin = excluir == null ? Padding.Empty : new Padding(0, 0, 8, 0);
            colunas.Controls.Add(salvar, 0, 0);
            if (excluir != null)
            {
                excluir.Margin = Padding.Empty;
                colunas.Controls.Add(excluir, 1, 0);
            }
        }

        private bool ValidarCampos(TextBox nome, TextBox especie, ComboBox sexo)
        {
            if (string.IsNullOrWhiteSpace(nome.Text))
            {
                MessageBox.Show("Informe o nome do animal.", "Campo obrigatório",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nome.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(especie.Text))
            {
                MessageBox.Show("Informe a espécie do animal.", "Campo obrigatório",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                especie.Focus();
                return false;
            }
            if (sexo.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione o sexo do animal.", "Campo obrigatório",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sexo.Focus();
                return false;
            }
            return true;
        }

        private void PreencherAnimal(DataRow animal, TextBox nome, TextBox especie,
            TextBox raca, ComboBox sexo, TextBox descricao, DateTimePicker nascimento)
        {
            animal["Nome"] = nome.Text.Trim();
            animal["Especie"] = especie.Text.Trim();
            animal["Raca"] = raca.Text.Trim();
            animal["Sexo"] = sexo.SelectedIndex == 0 ? "M" : "F";
            animal["Descricao"] = descricao.Text.Trim();
            SalvarDataNascimento(animal, nascimento);
        }

        // =========================================================
        // EDIÇÃO / EXCLUSÃO
        // =========================================================
        private void btnEditar_Click(object sender, EventArgs e)
        {
            DataRowView selecionado = dataGridView1.CurrentRow == null
                ? null : dataGridView1.CurrentRow.DataBoundItem as DataRowView;
            if (selecionado == null)
            {
                MessageBox.Show("Selecione um animal na tabela para editar.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataRow animal = selecionado.Row;
            int idAnimal = Convert.ToInt32(animal["ID_Animal"]);

            FlowLayoutPanel campos;
            Panel rodape;
            using (Form janela = CriarJanela("Editar animal", "Atualize os dados do paciente #" + idAnimal,
                out campos, out rodape))
            {
                TextBox nome = CriarCampoEdicao(campos, "Nome do animal *", animal["Nome"]);
                TextBox especie = CriarCampoEdicao(campos, "Espécie *", animal["Especie"]);
                TextBox raca = CriarCampoEdicao(campos, "Raça", animal["Raca"]);
                ComboBox sexo = CriarCampoSexo(campos, animal["Sexo"]);
                DateTimePicker nascimento = CriarCampoNascimento(campos, animal["Data_Nascimento"]);
                TextBox descricao = CriarCampoEdicao(campos, "Descrição", animal["Descricao"]);

                Button salvar = CriarBotaoModal("Salvar alterações", true);
                Button excluir = CriarBotaoModal("Excluir animal", false);
                CriarRodape(rodape, salvar, excluir);

                salvar.Click += (s, ev) =>
                {
                    if (!ValidarCampos(nome, especie, sexo)) return;
                    try
                    {
                        PreencherAnimal(animal, nome, especie, raca, sexo, descricao, nascimento);
                        AnimaisTableAdapter adapter = new AnimaisTableAdapter();
                        adapter.Update((DSProjectVet.Dados.DataSet_ClinicaVet.AnimaisDataTable)animal.Table);
                        MessageBox.Show("Animal atualizado com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        janela.DialogResult = DialogResult.OK;
                        janela.Close();
                        CarregarAnimais();
                    }
                    catch (Exception ex)
                    {
                        if (animal.RowState != DataRowState.Detached &&
                            animal.RowState != DataRowState.Deleted)
                            animal.RejectChanges();
                        MessageBox.Show("Erro ao atualizar o animal:\n\n" + ex.Message,
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                excluir.Click += (s, ev) =>
                {
                    string nomeAnimal = Convert.ToString(animal["Nome"]);
                    DialogResult confirmacao = MessageBox.Show(
                        "Deseja excluir o animal \"" + nomeAnimal + "\"?\n\nEsta ação não poderá ser desfeita.",
                        "Confirmar exclusão", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (confirmacao != DialogResult.Yes) return;
                    try
                    {
                        AnimaisTableAdapter adapter = new AnimaisTableAdapter();
                        DSProjectVet.Dados.DataSet_ClinicaVet.AnimaisDataTable tabela =
                            (DSProjectVet.Dados.DataSet_ClinicaVet.AnimaisDataTable)animal.Table;
                        animal.Delete();
                        int excluidos = adapter.Update(tabela);
                        if (excluidos <= 0) throw new Exception("Nenhum registro foi excluído.");
                        MessageBox.Show("Animal excluído com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        janela.DialogResult = DialogResult.OK;
                        janela.Close();
                        CarregarAnimais();
                    }
                    catch (Exception ex)
                    {
                        if (animal.RowState != DataRowState.Detached)
                            animal.RejectChanges();
                        MessageBox.Show("Não foi possível excluir o animal.\n\n" + ex.Message,
                            "Erro ao excluir", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                janela.ShowDialog(this);
            }
        }

        // =========================================================
        // CADASTRO
        // =========================================================
        private void btnNovoAnimal_Click(object sender, EventArgs e)
        {
            FlowLayoutPanel campos;
            Panel rodape;
            using (Form janela = CriarJanela("Cadastrar animal", "Preencha os dados do novo paciente",
                out campos, out rodape))
            {
                ComboBox comboCliente = null;
                if (clienteId.HasValue)
                {
                    Label tutor = CriarRotulo("Proprietário: " + lblNome.Text);
                    tutor.ForeColor = verdeEscuro;
                    tutor.Height = 42;
                    campos.Controls.Add(tutor);
                }
                else
                {
                    campos.Controls.Add(CriarRotulo("Proprietário *"));
                    comboCliente = new ComboBox
                    {
                        Width = 440,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Font = new Font("Segoe UI", 10F),
                        Margin = new Padding(0, 0, 0, 12)
                    };
                    campos.Controls.Add(comboCliente);
                    try
                    {
                        ClientesTableAdapter clienteAdapter = new ClientesTableAdapter();
                        DataTable clientes = clienteAdapter.GetData();
                        if (clientes.Rows.Count == 0)
                        {
                            MessageBox.Show("Cadastre um proprietário antes de cadastrar animais.",
                                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        DataTable lista = new DataTable();
                        lista.Columns.Add("ID", typeof(int));
                        lista.Columns.Add("Identificacao", typeof(string));
                        foreach (DataRow cliente in clientes.Rows)
                        {
                            lista.Rows.Add(Convert.ToInt32(cliente["ID_Cliente"]),
                                Convert.ToString(cliente["Nome"]) + " - " + Convert.ToString(cliente["CPF"]));
                        }
                        comboCliente.DisplayMember = "Identificacao";
                        comboCliente.ValueMember = "ID";
                        comboCliente.DataSource = lista;
                        comboCliente.SelectedIndex = -1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao carregar os proprietários:\n\n" + ex.Message,
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                TextBox nome = CriarCampoEdicao(campos, "Nome do animal *", "");
                TextBox especie = CriarCampoEdicao(campos, "Espécie *", "");
                TextBox raca = CriarCampoEdicao(campos, "Raça", "");
                ComboBox sexo = CriarCampoSexo(campos, DBNull.Value);
                DateTimePicker nascimento = CriarCampoNascimento(campos, DBNull.Value);
                TextBox descricao = CriarCampoEdicao(campos, "Descrição", "");
                Button salvar = CriarBotaoModal("Cadastrar animal", true);
                CriarRodape(rodape, salvar, null);

                salvar.Click += (s, ev) =>
                {
                    if (!ValidarCampos(nome, especie, sexo)) return;
                    int idProprietario;
                    if (clienteId.HasValue)
                        idProprietario = clienteId.Value;
                    else
                    {
                        if (comboCliente.SelectedIndex < 0)
                        {
                            MessageBox.Show("Selecione o proprietário do animal.", "Campo obrigatório",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            comboCliente.Focus();
                            return;
                        }
                        idProprietario = Convert.ToInt32(comboCliente.SelectedValue);
                    }

                    try
                    {
                        AnimaisTableAdapter adapter = new AnimaisTableAdapter();
                        DSProjectVet.Dados.DataSet_ClinicaVet.AnimaisDataTable tabela = adapter.GetData();
                        DataRow animal = tabela.NewRow();
                        PreencherAnimal(animal, nome, especie, raca, sexo, descricao, nascimento);
                        animal["fk_Clientes_ID_Cliente"] = idProprietario;
                        tabela.Rows.Add(animal);
                        int salvos = adapter.Update(tabela);
                        if (salvos <= 0) throw new Exception("Nenhum registro foi cadastrado.");
                        MessageBox.Show("Animal cadastrado com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        janela.DialogResult = DialogResult.OK;
                        janela.Close();
                        CarregarAnimais();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao cadastrar animal:\n\n" + ex.Message,
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                janela.ShowDialog(this);
            }
        }

        // Eventos mantidos para compatibilidade com frmAnimais.Designer.cs
        private void label1_Click(object sender, EventArgs e) { }
        private void lblNome_Click(object sender, EventArgs e) { }
        private void lblCpf_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
    }
}
