using System;
using System.Drawing;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public partial class Perfil : Form
    {
        // =========================
        // CORES DO SISTEMA
        // =========================

        private readonly Color verdePrincipal =
            Color.FromArgb(20, 192, 138);

        private readonly Color verdeEscuro =
            Color.FromArgb(14, 145, 104);

        private readonly Color verdeClaro =
            Color.FromArgb(225, 248, 240);

        private readonly Color fundoTela =
            Color.FromArgb(248, 250, 249);

        private readonly Color textoPrincipal =
            Color.FromArgb(45, 55, 52);

        private readonly Color textoSecundario =
            Color.FromArgb(110, 120, 116);

        public Perfil()
        {
            InitializeComponent();
        }

        private void Perfil_Load(object sender, EventArgs e)
        {
            EstilizarTela();
            CarregarDadosUsuario();
            CarregarUltimoAcesso();
            CarregarDispositivo();

            btnTrocaSenha.btnPrimary();
        }

        // =========================================================
        // CARREGAR DADOS DO USUÁRIO
        // =========================================================

        private void CarregarDadosUsuario()
        {
            lbl_Username.Text =
                Properties.Settings.Default.NomeUsuarioLogado;

            switch (Properties.Settings.Default.NivelUsuarioLogado)
            {
                case 1:
                    lbl_cargo.Text = "Administrador(a)";
                    break;

                case 2:
                    lbl_cargo.Text = "Veterinário(a)";
                    break;

                case 3:
                    lbl_cargo.Text = "Caixa";
                    break;

                default:
                    lbl_cargo.Text = "Usuário";
                    break;
            }
        }

        // =========================================================
        // ÚLTIMO ACESSO
        // =========================================================

        private void CarregarUltimoAcesso()
        {
            DateTime ultimaExecucao =
                Properties.Settings.Default.UltimaExecucao;

            // Se nunca houve acesso anterior
            if (ultimaExecucao.Year <= 2001)
            {
                lbl_lastAccess.Text = "Primeiro acesso";
            }
            else
            {
                lbl_lastAccess.Text =
                    ultimaExecucao.ToString("dd/MM/yyyy 'às' HH:mm");
            }

            // Atualiza para o acesso atual
            Properties.Settings.Default.UltimaExecucao =
                DateTime.Now;

            Properties.Settings.Default.Save();
        }

        // =========================================================
        // DISPOSITIVO
        // =========================================================

        private void CarregarDispositivo()
        {
            lbl_desktop.Text = Environment.MachineName;
        }

        // =========================================================
        // ESTILIZAÇÃO DA TELA
        // =========================================================

        private void EstilizarTela()
        {
            this.BackColor = fundoTela;

            // =========================
            // NOME DO USUÁRIO
            // =========================

            lbl_Username.Font =
                new Font(
                    "Segoe UI",
                    22F,
                    FontStyle.Bold
                );

            lbl_Username.ForeColor =
                textoPrincipal;

            // =========================
            // CARGO
            // =========================

            lbl_cargo.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            lbl_cargo.ForeColor =
                verdePrincipal;

            // =========================
            // ÚLTIMO ACESSO
            // =========================

            lbl_lastAccess.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Regular
                );

            lbl_lastAccess.ForeColor =
                textoSecundario;

            // =========================
            // DISPOSITIVO
            // =========================

            lbl_desktop.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Regular
                );

            lbl_desktop.ForeColor =
                textoSecundario;
        }

        // =========================================================
        // EVENTOS EXISTENTES
        // =========================================================

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void lbl_lastAccess_Click(object sender, EventArgs e)
        {

        }

        private void lbl_desktop_Click(object sender, EventArgs e)
        {

        }

        private void btnTrocaSenha_Click(object sender, EventArgs e)
        {

        }
    }
}