using DSProjectVet.Dados.DataSet_ClinicaVetTableAdapters;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{
    public partial class frmAnimais : Form
    {
        private int? clienteId = null;

        // CORES DO SISTEMA
        private readonly Color verdePrincipal = Color.FromArgb(20, 192, 138);
        private readonly Color verdeEscuro = Color.FromArgb(14, 145, 104);
        private readonly Color verdeClaro = Color.FromArgb(225, 248, 240);
        private readonly Color fundoTela = Color.FromArgb(248, 250, 249);
        private readonly Color textoPrincipal = Color.FromArgb(45, 55, 52);
        private readonly Color textoSecundario = Color.FromArgb(110, 120, 116);


        private TextBox CriarCampoEdicao(
            FlowLayoutPanel painel,
            string titulo,
            object valor)
        {
            Label label = new Label();

            label.Text = titulo;
            label.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            label.ForeColor = textoPrincipal;
            label.Width = 390;
            label.Height = 22;
            label.Margin = new Padding(0, 5, 0, 0);

            painel.Controls.Add(label);

            TextBox campo = new TextBox();

            campo.Width = 390;
            campo.Height = 32;
            campo.Font = new Font("Segoe UI", 10F);
            campo.ForeColor = textoPrincipal;
            campo.BackColor = Color.White;

            campo.Text = valor == null || valor == DBNull.Value
                ? ""
                : Convert.ToString(valor);

            campo.Margin = new Padding(0, 0, 0, 8);

            painel.Controls.Add(campo);

            return campo;
        }


        // Abre pelo MDI -> mostra todos
        public frmAnimais()
        {
            InitializeComponent();
        }

        // Abre pelo frmClientes -> mostra somente daquele cliente
        public frmAnimais(int idCliente)
        {
            InitializeComponent();

            clienteId = idCliente;
        }

        private void frmAnimais_Load(object sender, EventArgs e)
        {
            EstilizarTela();
            CarregarAnimais();
            ConfigurarCabecalhoCliente();

            // Configuração do botão editar
            btnEditar.FlatStyle = FlatStyle.Flat;
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.BackColor = verdePrincipal;
            btnEditar.ForeColor = Color.White;
            btnEditar.Cursor = Cursors.Hand;

            btnEditar.Click += btnEditar_Click;
        }

        // =========================================================
        // CARREGAMENTO DOS ANIMAIS
        // =========================================================

        private void CarregarAnimais()
        {
            try
            {
                AnimaisTableAdapter animaisAdapter =
                    new AnimaisTableAdapter();

                if (clienteId == null)
                {
                    // Abriu pelo menu -> todos os animais
                    dataGridView1.DataSource =
                        animaisAdapter.GetData();
                }
                else
                {
                    // Abriu pelo cliente -> somente animais dele
                    dataGridView1.DataSource =
                        animaisAdapter.GetDataByCliente(clienteId.Value);
                }

                ConfigurarColunas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível carregar os animais.\n\n" +
                    ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // NOME E CPF DO CLIENTE
        // =========================================================

        private void ConfigurarCabecalhoCliente()
        {
            // Se abriu pelo MDI
            if (clienteId == null)
            {
                lblNome.Text = "Todos os animais";
                lblCpf.Text = "Visualização geral dos animais cadastrados";

                lblNome.ForeColor = textoPrincipal;
                lblCpf.ForeColor = textoSecundario;

                return;
            }

            try
            {
                ClientesTableAdapter clientesAdapter =
                    new ClientesTableAdapter();

                DataTable clientes = clientesAdapter.GetData();

                foreach (DataRow cliente in clientes.Rows)
                {
                    int id = Convert.ToInt32(cliente["ID"]);

                    if (id == clienteId.Value)
                    {
                        string nome =
                            cliente["Nome"]?.ToString();

                        string cpf =
                            cliente["CPF"]?.ToString();

                        lblNome.Text = nome;
                        lblCpf.Text = cpf;

                        return;
                    }
                }

                lblNome.Text = "Cliente não encontrado";
                lblCpf.Text = "";
            }
            catch (Exception ex)
            {
                lblNome.Text = "Cliente";
                lblCpf.Text = "";

                MessageBox.Show(
                    "Não foi possível carregar os dados do cliente.\n\n" +
                    ex.Message,
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // =========================================================
        // ESTILIZAÇÃO GERAL DA TELA
        // =========================================================

        private void EstilizarTela()
        {
            // FORMULÁRIO
            this.BackColor = fundoTela;

            // LABEL DO NOME
            lblNome.Font = new Font(
                "Segoe UI",
                18F,
                FontStyle.Bold
            );

            lblNome.ForeColor = textoPrincipal;

            // LABEL CPF
            lblCpf.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Regular
            );

            lblCpf.ForeColor = textoSecundario;

            // GRID
            EstilizarDataGridView();
        }

        // =========================================================
        // ESTILIZAÇÃO DO DATAGRIDVIEW
        // =========================================================

        private void EstilizarDataGridView()
        {
            // Permite estilizar cabeçalho
            dataGridView1.EnableHeadersVisualStyles = false;

            // FUNDO
            dataGridView1.BackgroundColor = Color.White;

            dataGridView1.BorderStyle =
                BorderStyle.None;

            // BORDAS
            dataGridView1.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            dataGridView1.GridColor =
                Color.FromArgb(232, 238, 235);

            // REMOVE COLUNA LATERAL
            dataGridView1.RowHeadersVisible = false;

            // SELEÇÃO
            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.MultiSelect = false;

            // NÃO DEIXA EDITAR DIRETAMENTE
            dataGridView1.ReadOnly = true;

            // ALTURA
            dataGridView1.RowTemplate.Height = 42;

            dataGridView1.ColumnHeadersHeight = 45;

            // COLUNAS PREENCHENDO A TELA
            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // EVITA LINHA VAZIA PARA CADASTRO
            dataGridView1.AllowUserToAddRows = false;

            // EVITA REDIMENSIONAMENTO DAS LINHAS
            dataGridView1.AllowUserToResizeRows = false;

            // =====================================================
            // CABEÇALHO
            // =====================================================

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor =
                verdePrincipal;

            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(8, 0, 0, 0);

            // =====================================================
            // CÉLULAS
            // =====================================================

            dataGridView1.DefaultCellStyle.BackColor =
                Color.White;

            dataGridView1.DefaultCellStyle.ForeColor =
                textoPrincipal;

            dataGridView1.DefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Regular
                );

            dataGridView1.DefaultCellStyle.SelectionBackColor =
                verdeClaro;

            dataGridView1.DefaultCellStyle.SelectionForeColor =
                verdeEscuro;

            dataGridView1.DefaultCellStyle.Padding =
                new Padding(8, 0, 8, 0);

            dataGridView1.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            // =====================================================
            // EFEITO ZEBRA
            // =====================================================

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 252, 250);

            // =====================================================
            // RESPONSIVIDADE
            // =====================================================

            dataGridView1.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;
        }

        // =========================================================
        // NOMES E TAMANHOS DAS COLUNAS
        // =========================================================

        private void ConfigurarColunas()
        {
            if (dataGridView1.Columns.Count == 0)
                return;

            // ID
            if (dataGridView1.Columns.Contains("ID"))
            {
                dataGridView1.Columns["ID"].HeaderText = "ID";
                dataGridView1.Columns["ID"].FillWeight = 35;
            }

            // NOME
            if (dataGridView1.Columns.Contains("Nome"))
            {
                dataGridView1.Columns["Nome"].HeaderText = "Nome";
                dataGridView1.Columns["Nome"].FillWeight = 90;
            }

            // RAÇA
            if (dataGridView1.Columns.Contains("Raca"))
            {
                dataGridView1.Columns["Raca"].HeaderText = "Raça";
                dataGridView1.Columns["Raca"].FillWeight = 75;
            }

            // SEXO
            if (dataGridView1.Columns.Contains("Sexo"))
            {
                dataGridView1.Columns["Sexo"].HeaderText = "Sexo";
                dataGridView1.Columns["Sexo"].FillWeight = 55;
            }

            // ENFERMIDADE
            if (dataGridView1.Columns.Contains("Enfermidade"))
            {
                dataGridView1.Columns["Enfermidade"].HeaderText =
                    "Enfermidade";

                dataGridView1.Columns["Enfermidade"].FillWeight = 100;
            }

            // DESCRIÇÃO
            if (dataGridView1.Columns.Contains("Descricao"))
            {
                dataGridView1.Columns["Descricao"].HeaderText =
                    "Descrição";

                dataGridView1.Columns["Descricao"].FillWeight = 140;
            }

            // ESPÉCIE
            if (dataGridView1.Columns.Contains("Especie"))
            {
                dataGridView1.Columns["Especie"].HeaderText =
                    "Espécie";

                dataGridView1.Columns["Especie"].FillWeight = 70;
            }

            // ESCONDE O ID DO CLIENTE
            if (dataGridView1.Columns.Contains("fk_Clientes_ID"))
            {
                dataGridView1.Columns["fk_Clientes_ID"].Visible = false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblNome_Click(object sender, EventArgs e)
        {

        }

        private void lblCpf_Click(object sender, EventArgs e)
        {

        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
                dataGridView1.CurrentRow.IsNewRow)
            {
                MessageBox.Show(
                    "Selecione um animal para editar.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DataRowView linhaSelecionada =
                dataGridView1.CurrentRow.DataBoundItem as DataRowView;

            if (linhaSelecionada == null)
            {
                MessageBox.Show("Não foi possível localizar o animal.");
                return;
            }

            DataRow animal = linhaSelecionada.Row;

            int idAnimal = Convert.ToInt32(animal["ID"]);

            // JANELA DE EDIÇÃO
            using (Form telaEdicao = new Form())
            {
                telaEdicao.Text = "Editar Animal #" + idAnimal;
                telaEdicao.Size = new Size(480, 610);
                telaEdicao.StartPosition = FormStartPosition.CenterParent;
                telaEdicao.BackColor = fundoTela;
                telaEdicao.FormBorderStyle = FormBorderStyle.FixedDialog;
                telaEdicao.MaximizeBox = false;
                telaEdicao.MinimizeBox = false;
                telaEdicao.AutoScroll = true;

                FlowLayoutPanel painel = new FlowLayoutPanel();
                painel.Dock = DockStyle.Fill;
                painel.FlowDirection = FlowDirection.TopDown;
                painel.WrapContents = false;
                painel.AutoScroll = true;
                painel.Padding = new Padding(22);

                telaEdicao.Controls.Add(painel);

                Label titulo = new Label();
                titulo.Text = "Editar dados do animal";
                titulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                titulo.ForeColor = textoPrincipal;
                titulo.Width = 400;
                titulo.Height = 45;

                painel.Controls.Add(titulo);

                // CAMPOS PREENCHIDOS AUTOMATICAMENTE
                TextBox txtNome = CriarCampoEdicao(
                    painel, "Nome", animal["Nome"]);

                TextBox txtRaca = CriarCampoEdicao(
                    painel, "Raça", animal["Raca"]);

                TextBox txtSexo = CriarCampoEdicao(
                    painel, "Sexo", animal["Sexo"]);

                TextBox txtEspecie = CriarCampoEdicao(
                    painel, "Espécie", animal["Especie"]);

                TextBox txtEnfermidade = CriarCampoEdicao(
                    painel, "Enfermidade", animal["Enfermidade"]);

                TextBox txtDescricao = CriarCampoEdicao(
                    painel, "Descrição", animal["Descricao"]);

                // BOTÃO SALVAR
                Button btnSalvar = new Button();
                btnSalvar.Text = "Salvar Alterações";
                btnSalvar.Width = 390;
                btnSalvar.Height = 45;
                btnSalvar.BackColor = verdePrincipal;
                btnSalvar.ForeColor = Color.White;
                btnSalvar.FlatStyle = FlatStyle.Flat;
                btnSalvar.FlatAppearance.BorderSize = 0;
                btnSalvar.Font = new Font(
                    "Segoe UI", 10F, FontStyle.Bold);
                btnSalvar.Cursor = Cursors.Hand;
                btnSalvar.Margin = new Padding(0, 15, 0, 10);

                painel.Controls.Add(btnSalvar);

                btnSalvar.Click += (s, ev) =>
                {
                    if (string.IsNullOrWhiteSpace(txtNome.Text))
                    {
                        MessageBox.Show(
                            "Informe o nome do animal.",
                            "Campo obrigatório",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        // Guarda os dados anteriores para recuperação
                        object[] valoresOriginais =
                            (object[])animal.ItemArray.Clone();

                        // Atualiza os campos da linha
                        animal["Nome"] = txtNome.Text.Trim();
                        animal["Raca"] = txtRaca.Text.Trim();
                        animal["Sexo"] = txtSexo.Text.Trim();
                        animal["Especie"] = txtEspecie.Text.Trim();
                        animal["Enfermidade"] = txtEnfermidade.Text.Trim();
                        animal["Descricao"] = txtDescricao.Text.Trim();

                        try
                        {
                            // Salva no banco de dados
                            AnimaisTableAdapter adapter =
                                new AnimaisTableAdapter();

                            adapter.Update(
                                (DSProjectVet.Dados.DataSet_ClinicaVet.AnimaisDataTable)
                                animal.Table
                            );
                        }
                        catch
                        {
                            // Desfaz alterações locais se falhar
                            animal.ItemArray = valoresOriginais;
                            animal.AcceptChanges();
                            throw;
                        }

                        MessageBox.Show(
                            "Animal atualizado com sucesso!",
                            "Sucesso",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        telaEdicao.DialogResult = DialogResult.OK;
                        telaEdicao.Close();

                        // Recarrega a tabela
                        CarregarAnimais();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Erro ao atualizar o animal:\n\n" + ex.Message,
                            "Erro",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                };

                telaEdicao.ShowDialog(this);
            }
        }
    }
}