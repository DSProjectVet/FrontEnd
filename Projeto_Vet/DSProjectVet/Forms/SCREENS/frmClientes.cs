using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public partial class frmClientes : Form
    {
        public frmClientes()
        {
            InitializeComponent();
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {
            // TODO: esta linha de código carrega dados na tabela 'dataSet_ClinicaVet.Clientes'. Você pode movê-la ou removê-la conforme necessário.
            this.clientesTableAdapter.Fill(this.dataSet_ClinicaVet.Clientes);

            // Desabilita os estilos padrões do Windows para permitir customização total
            dataGridView1.EnableHeadersVisualStyles = false;

            // 1. Configuração do Fundo Geral e Linhas de Grade
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.GridColor = Color.FromArgb(220, 240, 225); // Grade verde bem suave
            dataGridView1.BorderStyle = BorderStyle.None;

            // 2. Estilo Padrão das Células (Fundo branco com texto escuro)
            DataGridViewCellStyle estiloCelulas = new DataGridViewCellStyle();
            estiloCelulas.BackColor = Color.White;
            estiloCelulas.ForeColor = Color.FromArgb(30, 50, 35); // Verde escuro quase preto para contraste
            estiloCelulas.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            estiloCelulas.SelectionBackColor = Color.FromArgb(200, 235, 210); // Verde clarinho ao selecionar
            estiloCelulas.SelectionForeColor = Color.FromArgb(10, 40, 15); // Texto escuro na seleção
            dataGridView1.DefaultCellStyle = estiloCelulas;

            // 3. Efeito Zebra (Linhas alternadas em verde bem clarinho)
            DataGridViewCellStyle estiloAlternado = new DataGridViewCellStyle();
            estiloAlternado.BackColor = Color.FromArgb(245, 252, 247); // Verde quase branco
            dataGridView1.AlternatingRowsDefaultCellStyle = estiloAlternado;

            // 4. Estilo do Cabeçalho (Verde pastel com texto branco)
            DataGridViewCellStyle estiloCabecalho = new DataGridViewCellStyle();
            estiloCabecalho.BackColor = Color.FromArgb(46, 125, 50); // Verde floresta médio (ótimo contraste)
            estiloCabecalho.ForeColor = Color.White;
            estiloCabecalho.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            estiloCabecalho.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.ColumnHeadersDefaultCellStyle = estiloCabecalho;
            dataGridView1.ColumnHeadersHeight = 35;

            // 5. Ajustes Visuais Extras de Layout
            dataGridView1.RowHeadersVisible = false; // Remove a primeira coluna em branco
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // Linhas apenas horizontais (moderno)
            dataGridView1.RowTemplate.Height = 30; // Linhas mais altas para facilitar o clique
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Seleciona a linha inteira ao clicar

            // 6. Responsividade (Varia de largura e altura com a tela)
            // Faz o grid esticar para os 4 lados acompanhando o tamanho da janela
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Mantém as colunas preenchendo toda a largura gerada pelo redimensionamento
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Ajusta a altura das linhas automaticamente se o conteúdo exigir (opcional)
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;



            btnCadastrar.btnPrimary();
            btnVerAnimais.btnPrimary();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica se a linha clicada é válida (evita erros com o cabeçalho)
            if (e.RowIndex >= 0)
            {
                // Obtém a linha atual que recebeu o foco
                DataGridViewRow linhaAtual = dataGridView1.Rows[e.RowIndex];

                // Atualiza os Labels instantaneamente com o valor de cada célula
                lblNome.Text = linhaAtual.Cells["Nome"].Value?.ToString();
                lblCpf.Text = linhaAtual.Cells["Cpf"].Value?.ToString();
                lblEndereço.Text = linhaAtual.Cells["Endereco"].Value?.ToString();
                lblTelefone.Text = linhaAtual.Cells["Telefone"].Value?.ToString();
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
