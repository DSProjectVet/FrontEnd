
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public partial class Perfil : Form
    {
        // ==========================================
        // PALETA MED VET
        // ==========================================

        private readonly Color verdePrincipal =
            Color.FromArgb(20, 192, 138);

        private readonly Color verdeEscuro =
            Color.FromArgb(14, 145, 104);

        private readonly Color verdeClaro =
            Color.FromArgb(225, 248, 240);

        private readonly Color fundoTela =
            Color.FromArgb(248, 250, 249);

        private readonly Color textoPrincipal =
            Color.FromArgb(35, 49, 45);

        private readonly Color textoSecundario =
            Color.FromArgb(110, 120, 116);

        private readonly Color borda =
            Color.FromArgb(228, 235, 232);

        // ==========================================
        // CONTROLES DO NOVO LAYOUT
        // ==========================================

        private Panel painelRaiz;
        private Panel painelConteudo;

        private Panel cartaoPerfil;
        private Panel cartaoAcesso;
        private Panel cartaoDispositivo;
        private Panel cartaoSeguranca;
        private Panel painelAvatar;

        private Label lblTituloTela;
        private Label lblSubtituloTela;
        private Label lblAvatar;
        private Label lblStatus;
        private Label lblRodape;

        private bool layoutCriado = false;

        public Perfil()
        {
            InitializeComponent();

            // Garante apenas uma inscrição no evento.
            this.Load -= Perfil_Load;
            this.Load += Perfil_Load;
        }

        // ==========================================
        // CARREGAMENTO
        // ==========================================

        private void Perfil_Load(object sender, EventArgs e)
        {
            CarregarDadosUsuario();
            CarregarUltimoAcesso();
            CarregarDispositivo();

            EstilizarTela();
            ConfigurarPermissoes();

            // Remove eventual inscrição do Designer e registra
            // uma única vez o evento de logout.
            btnTrocaSenha.Click -= btnTrocaSenha_Click;
            btnTrocaSenha.Click += btnTrocaSenha_Click;

            btnGerenciarUsuario.Click -= button1_Click;
            btnGerenciarUsuario.Click += button1_Click;
        }

        // ==========================================
        // DADOS DO USUÁRIO
        // ==========================================

        private void CarregarDadosUsuario()
        {
            string nome =
                Properties.Settings.Default.NomeUsuarioLogado;

            lbl_Username.Text =
                string.IsNullOrWhiteSpace(nome)
                ? "Usuário"
                : nome;

            int nivel =
                Properties.Settings.Default.NivelUsuarioLogado;

            switch (nivel)
            {
                case 1:
                    lbl_cargo.Text = "Administrador(a)";
                    break;

                case 2:
                    lbl_cargo.Text = "Atendente / Recepcionista";
                    break;

                case 3:
                    lbl_cargo.Text = "Veterinário(a)";
                    break;

                default:
                    lbl_cargo.Text = "Usuário";
                    break;
            }
        }

        // ==========================================
        // ÚLTIMO ACESSO
        // ==========================================

        private void CarregarUltimoAcesso()
        {
            DateTime ultimo =
                Properties.Settings.Default.UltimaExecucao;

            if (ultimo.Year <= 2001)
            {
                lbl_lastAccess.Text = "Primeiro acesso";
            }
            else
            {
                lbl_lastAccess.Text =
                    ultimo.ToString("dd/MM/yyyy 'às' HH:mm");
            }

            Properties.Settings.Default.UltimaExecucao =
                DateTime.Now;

            Properties.Settings.Default.Save();
        }

        // ==========================================
        // DISPOSITIVO
        // ==========================================

        private void CarregarDispositivo()
        {
            lbl_desktop.Text = Environment.MachineName;
        }

        // ==========================================
        // PERMISSÕES VISUAIS
        // ==========================================

        private void ConfigurarPermissoes()
        {
            bool administrador =
                Properties.Settings.Default.NivelUsuarioLogado == 1;

            btnGerenciarUsuario.Visible = administrador;

            if (layoutCriado)
                AtualizarLayout();
        }

        // ==========================================
        // ESTILIZAR TELA COMPLETA
        // ==========================================

        private void EstilizarTela()
        {
            if (layoutCriado)
                return;

            layoutCriado = true;

            this.BackColor = fundoTela;
            this.Font = new Font("Segoe UI", 10F);
            this.AutoScroll = false;
            this.MinimumSize = new Size(440, 470);

            // Oculta os elementos antigos do Designer.
            // Os controles necessários serão reutilizados.
            foreach (Control controle in this.Controls)
            {
                controle.Visible = false;
            }

            // ======================================
            // PAINEL RAIZ
            // ======================================

            painelRaiz = new Panel();

            painelRaiz.Dock = DockStyle.Fill;
            painelRaiz.BackColor = fundoTela;
            painelRaiz.AutoScroll = true;

            this.Controls.Add(painelRaiz);
            painelRaiz.BringToFront();

            painelConteudo = new Panel();

            painelConteudo.BackColor = fundoTela;

            painelRaiz.Controls.Add(painelConteudo);

            // ======================================
            // CABEÇALHO
            // ======================================

            lblTituloTela = new Label();

            lblTituloTela.Text = "Meu perfil";
            lblTituloTela.Font =
                new Font("Segoe UI", 24F, FontStyle.Bold);

            lblTituloTela.ForeColor = textoPrincipal;
            lblTituloTela.AutoSize = false;

            painelConteudo.Controls.Add(lblTituloTela);

            lblSubtituloTela = new Label();

            lblSubtituloTela.Text =
                "Visualize suas informações e gerencie sua conta.";

            lblSubtituloTela.Font =
                new Font("Segoe UI", 10F);

            lblSubtituloTela.ForeColor = textoSecundario;
            lblSubtituloTela.AutoSize = false;

            painelConteudo.Controls.Add(lblSubtituloTela);

            // ======================================
            // CARTÃO PRINCIPAL DO PERFIL
            // ======================================

            cartaoPerfil = CriarCartao();

            painelConteudo.Controls.Add(cartaoPerfil);

            // AVATAR
            painelAvatar = new Panel();

            painelAvatar.BackColor = verdeClaro;

            cartaoPerfil.Controls.Add(painelAvatar);

            Arredondar(painelAvatar, 60);

            lblAvatar = new Label();

            lblAvatar.Dock = DockStyle.Fill;
            lblAvatar.TextAlign =
                ContentAlignment.MiddleCenter;

            lblAvatar.Font =
                new Font("Segoe UI", 27F, FontStyle.Bold);

            lblAvatar.ForeColor = verdeEscuro;
            lblAvatar.BackColor = Color.Transparent;

            string nome =
                lbl_Username.Text.Trim();

            lblAvatar.Text =
                nome.Length > 0
                ? nome.Substring(0, 1).ToUpper()
                : "U";

            painelAvatar.Controls.Add(lblAvatar);

            // STATUS
            lblStatus = new Label();

            lblStatus.Text = "●  CONTA ATIVA";
            lblStatus.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            lblStatus.ForeColor = verdeEscuro;
            lblStatus.AutoSize = false;

            cartaoPerfil.Controls.Add(lblStatus);

            // NOME EXISTENTE DO DESIGNER
            lbl_Username.Parent = cartaoPerfil;
            lbl_Username.Visible = true;
            lbl_Username.AutoSize = false;
            lbl_Username.Font =
                new Font("Segoe UI", 22F, FontStyle.Bold);

            lbl_Username.ForeColor = textoPrincipal;
            lbl_Username.TextAlign =
                ContentAlignment.MiddleLeft;

            lbl_Username.Anchor =
                AnchorStyles.Top | AnchorStyles.Left;

            // CARGO EXISTENTE DO DESIGNER
            lbl_cargo.Parent = cartaoPerfil;
            lbl_cargo.Visible = true;
            lbl_cargo.AutoSize = false;

            lbl_cargo.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            lbl_cargo.ForeColor = verdeEscuro;
            lbl_cargo.BackColor = verdeClaro;
            lbl_cargo.TextAlign =
                ContentAlignment.MiddleCenter;

            lbl_cargo.Anchor =
                AnchorStyles.Top | AnchorStyles.Left;

            Arredondar(lbl_cargo, 12);

            // ======================================
            // CARTÃO ÚLTIMO ACESSO
            // ======================================

            cartaoAcesso = CriarCartao();

            painelConteudo.Controls.Add(cartaoAcesso);

            Label tituloAcesso = CriarLabel(
                "Último acesso",
                12F,
                FontStyle.Bold,
                textoPrincipal);

            tituloAcesso.SetBounds(24, 23, 320, 30);

            cartaoAcesso.Controls.Add(tituloAcesso);

            Label descricaoAcesso = CriarLabel(
                "Último registro de acesso",
                9F,
                FontStyle.Regular,
                textoSecundario);

            descricaoAcesso.SetBounds(24, 58, 320, 24);

            cartaoAcesso.Controls.Add(descricaoAcesso);

            lbl_lastAccess.Parent = cartaoAcesso;
            lbl_lastAccess.Visible = true;
            lbl_lastAccess.AutoSize = false;

            lbl_lastAccess.Font =
                new Font("Segoe UI", 15F, FontStyle.Bold);

            lbl_lastAccess.ForeColor = verdeEscuro;
            lbl_lastAccess.TextAlign =
                ContentAlignment.MiddleLeft;

            // ======================================
            // CARTÃO DISPOSITIVO
            // ======================================

            cartaoDispositivo = CriarCartao();

            painelConteudo.Controls.Add(cartaoDispositivo);

            Label tituloDispositivo = CriarLabel(
                "Dispositivo",
                12F,
                FontStyle.Bold,
                textoPrincipal);

            tituloDispositivo.SetBounds(24, 23, 320, 30);

            cartaoDispositivo.Controls.Add(
                tituloDispositivo);

            Label descricaoDispositivo = CriarLabel(
                "Computador utilizado nesta sessão",
                9F,
                FontStyle.Regular,
                textoSecundario);

            descricaoDispositivo.SetBounds(24, 58, 340, 24);

            cartaoDispositivo.Controls.Add(
                descricaoDispositivo);

            lbl_desktop.Parent = cartaoDispositivo;
            lbl_desktop.Visible = true;
            lbl_desktop.AutoSize = false;

            lbl_desktop.Font =
                new Font("Segoe UI", 15F, FontStyle.Bold);

            lbl_desktop.ForeColor = verdeEscuro;
            lbl_desktop.TextAlign =
                ContentAlignment.MiddleLeft;

            // ======================================
            // CARTÃO DE SESSÃO E SEGURANÇA
            // ======================================

            cartaoSeguranca = CriarCartao();

            painelConteudo.Controls.Add(cartaoSeguranca);

            Label tituloSeguranca = CriarLabel(
                "Sessão e permissões",
                13F,
                FontStyle.Bold,
                textoPrincipal);

            tituloSeguranca.SetBounds(24, 20, 440, 32);

            cartaoSeguranca.Controls.Add(tituloSeguranca);

            Label descricaoSeguranca = CriarLabel(
                "Encerre a sessão ou gerencie usuários autorizados.",
                9F,
                FontStyle.Regular,
                textoSecundario);

            descricaoSeguranca.SetBounds(24, 54, 510, 24);

            cartaoSeguranca.Controls.Add(
                descricaoSeguranca);

            // BOTÃO SAIR DA CONTA
            // Reaproveitamos o controle existente do Designer
            // para manter compatibilidade com Perfil.Designer.cs.
            btnTrocaSenha.Parent = cartaoSeguranca;
            btnTrocaSenha.Visible = true;
            btnTrocaSenha.Enabled = true;
            btnTrocaSenha.Text = "Sair da conta";

            EstilizarBotao(
                btnTrocaSenha,
                Color.FromArgb(212, 66, 78),
                Color.FromArgb(181, 46, 59),
                Color.White);

            // BOTÃO GERENCIAR USUÁRIOS
            btnGerenciarUsuario.Parent = cartaoSeguranca;

            btnGerenciarUsuario.Text = "Gerenciar usuários";

            EstilizarBotao(
                btnGerenciarUsuario,
                verdeClaro,
                Color.FromArgb(205, 239, 225),
                verdeEscuro);

            btnGerenciarUsuario.Visible =
                Properties.Settings.Default
                    .NivelUsuarioLogado == 1;

            // ======================================
            // RODAPÉ
            // ======================================

            lblRodape = CriarLabel(
                "MED VET  •  Sistema de Gestão Veterinária",
                9F,
                FontStyle.Regular,
                textoSecundario);

            lblRodape.TextAlign =
                ContentAlignment.MiddleCenter;

            painelConteudo.Controls.Add(lblRodape);

            // RESPONSIVIDADE
            painelRaiz.Resize += (s, e) =>
            {
                AtualizarLayout();
            };

            AtualizarLayout();
        }

        // ==========================================
        // CRIAR CARTÃO
        // ==========================================

        private Panel CriarCartao()
        {
            Panel painel = new Panel();

            painel.BackColor = Color.White;
            painel.BorderStyle = BorderStyle.None;

            Arredondar(painel, 16);

            painel.Paint += (s, e) =>
            {
                if (painel.Width < 3 || painel.Height < 3)
                    return;

                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                Rectangle area = new Rectangle(
                    0, 0,
                    painel.Width - 1,
                    painel.Height - 1);

                using (GraphicsPath caminho =
                    CriarCaminhoArredondado(area, 16))
                using (Pen caneta = new Pen(borda, 1F))
                {
                    e.Graphics.DrawPath(caneta, caminho);
                }
            };

            return painel;
        }

        // ==========================================
        // CRIAR LABEL
        // ==========================================

        private Label CriarLabel(
            string texto,
            float tamanho,
            FontStyle estilo,
            Color cor)
        {
            Label label = new Label();

            label.Text = texto;
            label.Font =
                new Font("Segoe UI", tamanho, estilo);

            label.ForeColor = cor;
            label.BackColor = Color.Transparent;

            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleLeft;

            return label;
        }

        // ==========================================
        // ESTILIZAR BOTÕES
        // ==========================================

        private void EstilizarBotao(
            Button botao,
            Color corNormal,
            Color corHover,
            Color corTexto)
        {
            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = 0;

            botao.UseVisualStyleBackColor = false;

            botao.BackColor = corNormal;
            botao.ForeColor = corTexto;

            botao.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            botao.Cursor = Cursors.Hand;
            botao.AutoSize = false;

            botao.Anchor =
                AnchorStyles.Top | AnchorStyles.Left;

            botao.Height = 44;

            Arredondar(botao, 11);

            botao.MouseEnter += (s, e) =>
            {
                botao.BackColor = corHover;
            };

            botao.MouseLeave += (s, e) =>
            {
                botao.BackColor = corNormal;
            };
        }

        // ==========================================
        // BORDAS ARREDONDADAS
        // ==========================================

        private GraphicsPath CriarCaminhoArredondado(
            Rectangle area,
            int raio)
        {
            GraphicsPath caminho = new GraphicsPath();

            int diametro = Math.Min(
                raio * 2,
                Math.Min(area.Width, area.Height));

            if (diametro <= 0)
            {
                caminho.AddRectangle(area);
                return caminho;
            }

            caminho.AddArc(
                area.Left,
                area.Top,
                diametro,
                diametro,
                180,
                90);

            caminho.AddArc(
                area.Right - diametro,
                area.Top,
                diametro,
                diametro,
                270,
                90);

            caminho.AddArc(
                area.Right - diametro,
                area.Bottom - diametro,
                diametro,
                diametro,
                0,
                90);

            caminho.AddArc(
                area.Left,
                area.Bottom - diametro,
                diametro,
                diametro,
                90,
                90);

            caminho.CloseFigure();

            return caminho;
        }

        private void Arredondar(Control controle, int raio)
        {
            Action aplicar = () =>
            {
                if (controle.Width < 2 ||
                    controle.Height < 2)
                    return;

                Rectangle area = new Rectangle(
                    0,
                    0,
                    controle.Width - 1,
                    controle.Height - 1);

                using (GraphicsPath caminho =
                    CriarCaminhoArredondado(area, raio))
                {
                    Region regiaoAntiga = controle.Region;

                    controle.Region = new Region(caminho);

                    if (regiaoAntiga != null)
                        regiaoAntiga.Dispose();
                }
            };

            controle.Resize += (s, e) => aplicar();

            aplicar();
        }

        // ==========================================
        // LAYOUT RESPONSIVO
        // ==========================================

        private void AtualizarLayout()
        {
            if (!layoutCriado ||
                painelRaiz == null ||
                painelConteudo == null ||
                cartaoPerfil == null)
                return;

            int larguraDisponivel =
                painelRaiz.ClientSize.Width;

            int margem =
                larguraDisponivel < 650 ? 16 : 30;

            int largura = Math.Min(
                1040,
                Math.Max(
                    300,
                    larguraDisponivel - margem * 2));

            int posicaoX = Math.Max(
                margem,
                (larguraDisponivel - largura) / 2);

            painelConteudo.SetBounds(
                posicaoX, 22, largura, 750);

            // CABEÇALHO
            lblTituloTela.SetBounds(
                4, 0, largura - 8, 50);

            lblSubtituloTela.SetBounds(
                5, 54, largura - 10, 29);

            // CARTÃO PRINCIPAL
            cartaoPerfil.SetBounds(
                0, 103, largura, 166);

            bool compacto = largura < 550;

            int avatarTamanho = compacto ? 62 : 86;
            int avatarX = compacto ? 19 : 30;

            painelAvatar.SetBounds(
                avatarX,
                (166 - avatarTamanho) / 2,
                avatarTamanho,
                avatarTamanho);

            int textoX =
                avatarX + avatarTamanho +
                (compacto ? 13 : 27);

            int textoLargura =
                Math.Max(100, largura - textoX - 20);

            lblStatus.SetBounds(
                textoX, 23, textoLargura, 25);

            lbl_Username.Font = new Font(
                "Segoe UI",
                compacto ? 16F : 22F,
                FontStyle.Bold);

            lbl_Username.SetBounds(
                textoX, 49, textoLargura, 48);

            int larguraCargo = Math.Min(
                255, textoLargura);

            lbl_cargo.SetBounds(
                textoX, 106, larguraCargo, 30);

            // CARTÕES DE ACESSO E DISPOSITIVO
            int proximoY = 287;

            if (largura >= 720)
            {
                int espaco = 16;
                int larguraCartao =
                    (largura - espaco) / 2;

                cartaoAcesso.SetBounds(
                    0, proximoY, larguraCartao, 156);

                cartaoDispositivo.SetBounds(
                    larguraCartao + espaco,
                    proximoY,
                    largura - larguraCartao - espaco,
                    156);

                proximoY += 174;
            }
            else
            {
                cartaoAcesso.SetBounds(
                    0, proximoY, largura, 148);

                proximoY += 164;

                cartaoDispositivo.SetBounds(
                    0, proximoY, largura, 148);

                proximoY += 166;
            }

            lbl_lastAccess.SetBounds(
                24, 91,
                Math.Max(100, cartaoAcesso.Width - 48),
                40);

            lbl_desktop.SetBounds(
                24, 91,
                Math.Max(100, cartaoDispositivo.Width - 48),
                40);

            // CARTÃO DE SESSÃO E SEGURANÇA
            bool admin =
                Properties.Settings.Default
                    .NivelUsuarioLogado == 1;

            bool botoesLadoALado =
                largura >= 650 && admin;

            int alturaSeguranca =
                admin && !botoesLadoALado
                ? 208
                : 153;

            cartaoSeguranca.SetBounds(
                0,
                proximoY,
                largura,
                alturaSeguranca);

            int larguraInterna = largura - 48;

            if (botoesLadoALado)
            {
                int larguraBotao =
                    (larguraInterna - 14) / 2;

                btnTrocaSenha.SetBounds(
                    24, 91, larguraBotao, 44);

                btnGerenciarUsuario.SetBounds(
                    24 + larguraBotao + 14,
                    91,
                    larguraBotao,
                    44);
            }
            else
            {
                btnTrocaSenha.SetBounds(
                    24, 91, larguraInterna, 44);

                if (admin)
                {
                    btnGerenciarUsuario.SetBounds(
                        24, 145, larguraInterna, 44);
                }
            }

            proximoY += alturaSeguranca + 20;

            // RODAPÉ
            lblRodape.SetBounds(
                0, proximoY,
                largura, 30);

            painelConteudo.Height = proximoY + 55;

            painelRaiz.AutoScrollMinSize =
                new Size(
                    0,
                    painelConteudo.Top +
                    painelConteudo.Height + 10);
        }

        // ==========================================
        // BOTÃO GERENCIAR USUÁRIOS
        // ==========================================


        private void button1_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.NivelUsuarioLogado != 1)
            {
                MessageBox.Show(
                    "Acesso permitido somente a administradores.");
                return;
            }

            using (frmGerenciarUsuarios tela =
                new frmGerenciarUsuarios())
            {
                tela.ShowDialog(this);
            }
        }


        // ==========================================
        // LOGOUT - ENCERRAR SESSÃO
        // ==========================================

        // Mantido com este nome para preservar
        // o vínculo do evento em Perfil.Designer.cs.
        private void btnTrocaSenha_Click(
            object sender, EventArgs e)
        {
            ExecutarLogout();
        }


        private void ExecutarLogout()
        {
            DialogResult resposta = MessageBox.Show(
                this,
                "Deseja realmente sair da sua conta?\n\n" +
                "Você precisará informar seu usuário " +
                "e senha para entrar novamente.",
                "Confirmar logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
            );

            if (resposta != DialogResult.Yes)
                return;

            // Procura o MDI principal da aplicação.
            Form menu = this.MdiParent;

            if (menu == null)
            {
                foreach (Form formulario in
                    Application.OpenForms)
                {
                    if (formulario is MDI_Menu)
                    {
                        menu = formulario;
                        break;
                    }
                }
            }

            if (menu == null)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível localizar " +
                    "a janela principal do sistema.",
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Ao fechar o MDI, Menu_FormClosed
            // do frm_Login será executado e abrirá
            // a tela de login novamente.
            menu.Close();
        }


        // ==========================================
        // EVENTOS DO DESIGNER
        // ==========================================

        private void label1_Click(
            object sender, EventArgs e)
        {
        }

        private void label10_Click(
            object sender, EventArgs e)
        {
        }

        private void label11_Click(
            object sender, EventArgs e)
        {
        }

        private void lbl_lastAccess_Click(
            object sender, EventArgs e)
        {
        }

        private void lbl_desktop_Click(
            object sender, EventArgs e)
        {
        }
    }
}
    