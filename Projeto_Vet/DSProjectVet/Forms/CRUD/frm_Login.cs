
using DSProjectVet.Dados.DataSet_ClinicaVetTableAdapters;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public partial class frm_Login : Form
    {
        private int erros = 0;
        private bool autenticando = false;

        public frm_Login()
        {
            InitializeComponent();
        }

        // ==========================================
        // CARREGAMENTO DO LOGIN
        // ==========================================

        private void frm_Login_Load(
            object sender, EventArgs e)
        {
            LimparSessao();

            txtUser.Clear();
            txtPass.Clear();

            btn_ok.Enabled = true;
        }

        // ==========================================
        // BOTÃO ENTRAR
        // ==========================================

        private async void btn_ok_Click(
            object sender, EventArgs e)
        {
            if (autenticando)
                return;

            string usuarioDigitado =
                txtUser.Text.Trim();

            string senhaDigitada =
                txtPass.Text;

            errorProvider.SetError(txtUser, "");
            errorProvider.SetError(txtPass, "");

            // VALIDAR USUÁRIO
            if (string.IsNullOrWhiteSpace(
                usuarioDigitado))
            {
                errorProvider.SetError(
                    txtUser,
                    "Informe seu usuário.");

                txtUser.Focus();
                return;
            }

            // VALIDAR SENHA
            if (string.IsNullOrWhiteSpace(
                senhaDigitada))
            {
                errorProvider.SetError(
                    txtPass,
                    "Informe sua senha.");

                txtPass.Focus();
                return;
            }

            autenticando = true;
            btn_ok.Enabled = false;

            try
            {
                UsuarioAutenticado usuario =
                    await AutenticarAsync(
                        usuarioDigitado,
                        senhaDigitada);

                if (usuario == null)
                {
                    erros++;

                    MessageBox.Show(
                        this,
                        "Usuário ou senha incorretos.",
                        "Falha na autenticação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    if (erros >= 3)
                    {
                        MessageBox.Show(
                            this,
                            "Foram realizadas três " +
                            "tentativas incorretas.\n\n" +
                            "Confira suas credenciais " +
                            "antes de tentar novamente.",
                            "Med Vet",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        erros = 0;
                    }

                    return;
                }

                // LOGIN VALIDADO
                erros = 0;

                Properties.Settings.Default
                    .NomeUsuarioLogado =
                    usuario.Nome;

                Properties.Settings.Default
                    .NivelUsuarioLogado =
                    usuario.Nivel;

                Properties.Settings.Default.Save();

                // CRIAR MENU PRINCIPAL
                MDI_Menu menu = new MDI_Menu();

                // IMPORTANTE:
                // O evento pertence ao MENU,
                // não ao formulário de LOGIN.
                menu.FormClosed += Menu_FormClosed;

                // Mostrar o menu e ocultar login
                menu.Show();
                this.Hide();

                // Limpa credenciais digitadas
                txtUser.Clear();
                txtPass.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível realizar o login.\n\n" +
                    ex.Message,
                    "Erro de autenticação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.Show();
            }
            finally
            {
                autenticando = false;
                btn_ok.Enabled = true;

                if (this.Visible)
                {
                    txtPass.Clear();
                    txtPass.Focus();
                }
            }
        }

        // ==========================================
        // MODELO DO USUÁRIO AUTENTICADO
        // ==========================================

        private class UsuarioAutenticado
        {
            public string Nome { get; set; }
            public int Nivel { get; set; }
        }

        // ==========================================
        // AUTENTICAÇÃO SQL + BCRYPT
        // ==========================================

        private async Task<UsuarioAutenticado>
            AutenticarAsync(
                string usuario,
                string senha)
        {
            // Consulta executada fora da UI.
            DataTable tabela = await Task.Run(() =>
            {
                using (FuncionariosTableAdapter adapter =
                    new FuncionariosTableAdapter())
                {
                    return adapter.LoginUser(usuario);
                }
            });

            using (tabela)
            {
                if (tabela == null ||
                    tabela.Rows.Count == 0)
                {
                    return null;
                }

                DataRow linha = tabela.Rows[0];

                string hashArmazenado =
                    Convert.ToString(
                        linha["SenhaHash"]);

                if (string.IsNullOrWhiteSpace(
                    hashArmazenado))
                {
                    return null;
                }

                // BCrypt pode ser custoso.
                // Executar fora da thread visual.
                bool senhaCorreta =
                    await Task.Run(() =>
                    {
                        return BCrypt.Net.BCrypt.Verify(
                            senha,
                            hashArmazenado);
                    });

                if (!senhaCorreta)
                    return null;

                string nome =
                    Convert.ToString(
                        linha["Nome"]);

                string nivelTexto =
                    Convert.ToString(
                        linha["NivelAcesso"]).Trim();

                int nivel;

                if (!int.TryParse(
                    nivelTexto, out nivel))
                {
                    throw new Exception(
                        "Nível de acesso inválido " +
                        "para este funcionário.");
                }

                if (nivel < 1 || nivel > 3)
                {
                    throw new Exception(
                        "O funcionário não possui " +
                        "um nível de acesso válido.");
                }

                return new UsuarioAutenticado
                {
                    Nome = nome,
                    Nivel = nivel
                };
            }
        }

        // ==========================================
        // QUANDO O MDI É FECHADO
        // ==========================================

        private void Menu_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            // Encerra a sessão anterior.
            LimparSessao();

            if (this.IsDisposed || this.Disposing)
                return;

            // Retorna ao mesmo formulário de login.
            this.Show();

            this.WindowState =
                FormWindowState.Normal;

            this.Activate();

            txtUser.Clear();
            txtPass.Clear();

            errorProvider.SetError(txtUser, "");
            errorProvider.SetError(txtPass, "");

            txtUser.Focus();
        }

        // ==========================================
        // LIMPAR DADOS DA SESSÃO
        // ==========================================

        private void LimparSessao()
        {
            Properties.Settings.Default
                .NomeUsuarioLogado = "";

            Properties.Settings.Default
                .NivelUsuarioLogado = 0;

            Properties.Settings.Default.Save();
        }

        // ==========================================
        // EVENTOS DO DESIGNER
        // ==========================================

        private void txtUser_TextChanged(
            object sender, EventArgs e)
        {
            errorProvider.SetError(txtUser, "");
        }
    }
}
