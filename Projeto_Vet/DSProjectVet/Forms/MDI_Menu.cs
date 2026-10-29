
using DSProjectVet.Forms.SCREENS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public partial class MDI_Menu : Form
    {
        // ======================================
        // MÓDULOS DO SISTEMA
        // ======================================

        private enum Modulo
        {
            Inicio,
            Clientes,
            Animais,
            Consultas,
            Estoque,
            Financeiro,
            Perfil
        }

        // ======================================
        // IDENTIDADE VISUAL
        // ======================================

        private readonly Color verde =
            Color.FromArgb(20, 192, 138);

        private readonly Color verdeEscuro =
            Color.FromArgb(16, 48, 42);

        private readonly Color verdeSelecionado =
            Color.FromArgb(35, 89, 75);

        private readonly Color fundo =
            Color.FromArgb(246, 250, 248);

        private readonly Color texto =
            Color.FromArgb(35, 49, 45);

        private readonly Color cinza =
            Color.FromArgb(111, 125, 119);

        private const int LARGURA_MENU = 238;
        private const int ALTURA_TOPO = 83;

        // ======================================
        // ESTADO
        // ======================================

        private readonly string cargoSessao;

        private readonly Dictionary<Modulo, Button>
            botoesMenu =
                new Dictionary<Modulo, Button>();

        private readonly Dictionary<Modulo, Form>
            formulariosAbertos =
                new Dictionary<Modulo, Form>();

        private MdiClient areaMdi;
        private Panel painelLateral;
        private Panel painelTopo;

        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblUsuario;
        private Label lblCargo;
        private Label lblUsuarioLateral;

        private Button btnPerfilTopo;

        // ======================================
        // CONSTRUTORES
        // ======================================

        public MDI_Menu() : this(null)
        {
        }

        // Permite receber o cargo real do login.
        public MDI_Menu(string cargoFuncionario)
        {
            InitializeComponent();

            int nivel =
                DSProjectVet.Properties.Settings
                    .Default.NivelUsuarioLogado;

            cargoSessao =
                string.IsNullOrWhiteSpace(cargoFuncionario)
                ? CargoPadrao(nivel)
                : cargoFuncionario.Trim();

            ConfigurarJanela();
            ConstruirMenu();

            this.Resize += (s, e) =>
            {
                AjustarLayout();
            };

            this.MdiChildActivate += (s, e) =>
            {
                AtualizarSelecaoPeloFormulario();
            };

            this.Shown += (s, e) =>
            {
                AplicarPermissoes();
                AjustarLayout();
                AbrirModulo(Modulo.Inicio);
            };
        }

        // ======================================
        // CONFIGURAR JANELA
        // ======================================

        private void ConfigurarJanela()
        {
            this.Text = "Med Vet | Gestão Veterinária";

            this.MinimumSize = new Size(1060, 690);

            this.StartPosition =
                FormStartPosition.CenterScreen;

            this.WindowState =
                FormWindowState.Maximized;

            this.BackColor = fundo;

            this.Font =
                new Font("Segoe UI", 10F);

            // Mantém os componentes do Designer,
            // mas substitui a navegação visual.
            menuDashBoard.Visible = false;

            if (!this.IsMdiContainer)
                this.IsMdiContainer = true;

            areaMdi = this.Controls
                .OfType<MdiClient>()
                .FirstOrDefault();

            if (areaMdi == null)
            {
                throw new InvalidOperationException(
                    "A área MDI não foi encontrada.");
            }

            areaMdi.Dock = DockStyle.None;
            areaMdi.Anchor = AnchorStyles.None;
            areaMdi.BackColor = fundo;
        }

        // ======================================
        // CONSTRUIR MENU
        // ======================================

        private void ConstruirMenu()
        {
            painelLateral = new Panel();

            painelLateral.BackColor = verdeEscuro;

            this.Controls.Add(painelLateral);

            TableLayoutPanel estruturaLateral =
                new TableLayoutPanel();

            estruturaLateral.Dock = DockStyle.Fill;
            estruturaLateral.ColumnCount = 1;
            estruturaLateral.RowCount = 4;

            estruturaLateral.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            estruturaLateral.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 108F));

            estruturaLateral.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 45F));

            estruturaLateral.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            estruturaLateral.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 106F));

            painelLateral.Controls.Add(
                estruturaLateral);

            // ==================================
            // LOGOTIPO
            // ==================================

            Panel marca = new Panel();

            marca.Dock = DockStyle.Fill;

            marca.Margin = new Padding(0);

            estruturaLateral.Controls.Add(
                marca, 0, 0);

            Panel iconeLogo = new Panel();

            iconeLogo.BackColor = verde;

            iconeLogo.SetBounds(18, 28, 48, 48);

            marca.Controls.Add(iconeLogo);

            Label sinal = new Label();

            sinal.Text = "+";
            sinal.ForeColor = Color.White;

            sinal.Font = new Font(
                "Segoe UI", 27F, FontStyle.Bold);

            sinal.TextAlign =
                ContentAlignment.MiddleCenter;

            sinal.Dock = DockStyle.Fill;

            iconeLogo.Controls.Add(sinal);

            Label nomeMarca = new Label();

            nomeMarca.Text = "MED VET";

            nomeMarca.ForeColor = Color.White;

            nomeMarca.Font = new Font(
                "Segoe UI", 17F, FontStyle.Bold);

            nomeMarca.SetBounds(
                78, 26, 145, 35);

            marca.Controls.Add(nomeMarca);

            Label descricaoMarca = new Label();

            descricaoMarca.Text =
                "GESTÃO VETERINÁRIA";

            descricaoMarca.ForeColor =
                Color.FromArgb(160, 199, 187);

            descricaoMarca.Font =
                new Font("Segoe UI", 7.5F);

            descricaoMarca.SetBounds(
                80, 61, 150, 21);

            marca.Controls.Add(descricaoMarca);

            // ==================================
            // TÍTULO DO MENU
            // ==================================

            Label secaoMenu = new Label();

            secaoMenu.Text = "MENU PRINCIPAL";

            secaoMenu.ForeColor =
                Color.FromArgb(156, 192, 180);

            secaoMenu.Font = new Font(
                "Segoe UI", 8F, FontStyle.Bold);

            secaoMenu.Dock = DockStyle.Fill;

            secaoMenu.Padding =
                new Padding(22, 8, 0, 0);

            estruturaLateral.Controls.Add(
                secaoMenu, 0, 1);

            // ==================================
            // BOTÕES
            // ==================================

            FlowLayoutPanel listaMenu =
                new FlowLayoutPanel();

            listaMenu.Dock = DockStyle.Fill;

            listaMenu.FlowDirection =
                FlowDirection.TopDown;

            listaMenu.WrapContents = false;
            listaMenu.AutoScroll = true;

            listaMenu.Margin = new Padding(0);

            listaMenu.Padding =
                new Padding(12, 0, 8, 0);

            listaMenu.BackColor = verdeEscuro;

            estruturaLateral.Controls.Add(
                listaMenu, 0, 2);

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Inicio,
                "Início");

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Clientes,
                "Clientes");

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Animais,
                "Animais");

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Consultas,
                "Consultas");

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Estoque,
                "Estoque");

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Financeiro,
                "Fluxo financeiro");

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Perfil,
                "Meu perfil");

            // ==================================
            // USUÁRIO NO RODAPÉ
            // ==================================

            Panel painelUsuario = new Panel();

            painelUsuario.Dock = DockStyle.Fill;

            painelUsuario.Margin =
                new Padding(0);

            estruturaLateral.Controls.Add(
                painelUsuario, 0, 3);

            Label tituloUsuario = new Label();

            tituloUsuario.Text =
                "USUÁRIO CONECTADO";

            tituloUsuario.ForeColor =
                Color.FromArgb(156, 192, 180);

            tituloUsuario.Font = new Font(
                "Segoe UI", 8F, FontStyle.Bold);

            tituloUsuario.SetBounds(
                22, 12, 200, 22);

            painelUsuario.Controls.Add(
                tituloUsuario);

            lblUsuarioLateral = new Label();

            lblUsuarioLateral.ForeColor =
                Color.White;

            lblUsuarioLateral.Font = new Font(
                "Segoe UI", 11F, FontStyle.Bold);

            lblUsuarioLateral.AutoEllipsis = true;

            lblUsuarioLateral.SetBounds(
                22, 37, 195, 29);

            painelUsuario.Controls.Add(
                lblUsuarioLateral);

            Label rodapeCargo = new Label();

            rodapeCargo.Text = cargoSessao;

            rodapeCargo.ForeColor =
                Color.FromArgb(160, 199, 187);

            rodapeCargo.Font =
                new Font("Segoe UI", 9F);

            rodapeCargo.SetBounds(
                22, 67, 195, 23);

            painelUsuario.Controls.Add(
                rodapeCargo);

            // ==================================
            // CABEÇALHO
            // ==================================

            painelTopo = new Panel();

            painelTopo.BackColor = Color.White;

            this.Controls.Add(painelTopo);

            lblTitulo = new Label();

            lblTitulo.Text = "Visão geral";

            lblTitulo.Font = new Font(
                "Segoe UI", 18F, FontStyle.Bold);

            lblTitulo.ForeColor = texto;

            lblTitulo.SetBounds(
                28, 10, 460, 39);

            painelTopo.Controls.Add(
                lblTitulo);

            lblSubtitulo = new Label();

            lblSubtitulo.Text =
                "Painel principal do sistema";

            lblSubtitulo.ForeColor = cinza;

            lblSubtitulo.Font =
                new Font("Segoe UI", 9F);

            lblSubtitulo.SetBounds(
                30, 52, 470, 23);

            painelTopo.Controls.Add(
                lblSubtitulo);

            lblCargo = new Label();

            lblCargo.Text = cargoSessao;

            lblCargo.Font =
                new Font("Segoe UI", 9F);

            lblCargo.ForeColor = cinza;

            lblCargo.TextAlign =
                ContentAlignment.MiddleRight;

            painelTopo.Controls.Add(lblCargo);

            lblUsuario = new Label();

            lblUsuario.Font = new Font(
                "Segoe UI", 10F, FontStyle.Bold);

            lblUsuario.ForeColor = texto;

            lblUsuario.TextAlign =
                ContentAlignment.MiddleRight;

            painelTopo.Controls.Add(lblUsuario);

            btnPerfilTopo = new Button();

            btnPerfilTopo.Text = "Meu perfil";

            btnPerfilTopo.Size = new Size(123, 42);

            btnPerfilTopo.BackColor =
                Color.FromArgb(225, 248, 240);

            btnPerfilTopo.ForeColor =
                Color.FromArgb(12, 125, 90);

            btnPerfilTopo.FlatStyle =
                FlatStyle.Flat;

            btnPerfilTopo.FlatAppearance.BorderSize = 0;

            btnPerfilTopo.Cursor = Cursors.Hand;

            btnPerfilTopo.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            btnPerfilTopo.Click += (s, e) =>
            {
                AbrirModulo(Modulo.Perfil);
            };

            painelTopo.Controls.Add(btnPerfilTopo);

            AjustarLayout();
        }

        // ======================================
        // ADICIONAR BOTÃO
        // ======================================

        private void AdicionarBotaoMenu(
            FlowLayoutPanel painel,
            Modulo modulo,
            string titulo)
        {
            Button botao = new Button();

            botao.Text = "   " + titulo;

            botao.Width = 205;
            botao.Height = 48;

            botao.Margin =
                new Padding(0, 3, 0, 5);

            botao.BackColor = verdeEscuro;

            botao.ForeColor =
                Color.FromArgb(211, 232, 225);

            botao.FlatStyle = FlatStyle.Flat;

            botao.FlatAppearance.BorderSize = 0;

            botao.FlatAppearance.MouseOverBackColor =
                verdeSelecionado;

            botao.UseVisualStyleBackColor = false;

            botao.Font =
                new Font("Segoe UI", 11F);

            botao.TextAlign =
                ContentAlignment.MiddleLeft;

            botao.Cursor = Cursors.Hand;

            botao.Click += (s, e) =>
            {
                AbrirModulo(modulo);
            };

            painel.Controls.Add(botao);

            botoesMenu[modulo] = botao;
        }

        // ======================================
        // AJUSTAR POSIÇÕES
        // ======================================

        private void AjustarLayout()
        {
            if (painelLateral == null ||
                painelTopo == null ||
                areaMdi == null)
                return;

            int largura =
                Math.Max(
                    0,
                    this.ClientSize.Width - LARGURA_MENU);

            int altura =
                Math.Max(
                    0,
                    this.ClientSize.Height - ALTURA_TOPO);

            painelLateral.Bounds =
                new Rectangle(
                    0, 0,
                    LARGURA_MENU,
                    this.ClientSize.Height);

            painelTopo.Bounds =
                new Rectangle(
                    LARGURA_MENU, 0,
                    largura, ALTURA_TOPO);

            areaMdi.Bounds =
                new Rectangle(
                    LARGURA_MENU,
                    ALTURA_TOPO,
                    largura,
                    altura);

            btnPerfilTopo.Location =
                new Point(
                    Math.Max(
                        0,
                        painelTopo.Width - 148),
                    20);

            lblUsuario.SetBounds(
                Math.Max(0, btnPerfilTopo.Left - 190),
                14,
                175,
                28);

            lblCargo.SetBounds(
                Math.Max(0, btnPerfilTopo.Left - 190),
                41,
                175,
                23);

            painelLateral.BringToFront();
            painelTopo.BringToFront();
        }

        // ======================================
        // CARGOS E NÍVEIS
        // ======================================

        private int NivelAtual()
        {
            return DSProjectVet.Properties.Settings
                .Default.NivelUsuarioLogado;
        }

        private static string CargoPadrao(int nivel)
        {
            switch (nivel)
            {
                case 1:
                    return "Administrador";

                case 2:
                    return "Atendente";

                case 3:
                    return "Veterinário";

                default:
                    return "Não definido";
            }
        }

        private static string NormalizarCargo(string cargo)
        {
            return (cargo ?? "")
                .Trim()
                .ToUpperInvariant()
                .Replace("Á", "A")
                .Replace("À", "A")
                .Replace("Â", "A")
                .Replace("Ã", "A")
                .Replace("É", "E")
                .Replace("Ê", "E")
                .Replace("Í", "I")
                .Replace("Ó", "O")
                .Replace("Ô", "O")
                .Replace("Õ", "O")
                .Replace("Ú", "U")
                .Replace("Ç", "C");
        }

        // ======================================
        // SISTEMA DE PERMISSÕES
        // ======================================

        private bool PodeAcessar(Modulo modulo)
        {
            // Todos podem acessar essas telas.
            if (modulo == Modulo.Inicio ||
                modulo == Modulo.Perfil)
            {
                return true;
            }

            int nivel = NivelAtual();

            string cargo =
                NormalizarCargo(cargoSessao);

            // NÍVEL 1 - ADMINISTRADOR
            if (nivel == 1 &&
                (cargo == "ADMINISTRADOR" ||
                 cargo == "GERENTE"))
            {
                return true;
            }

            // NÍVEL 2 - GERENTE
            if (nivel == 2 && cargo == "GERENTE")
            {
                return true;
            }

            // NÍVEL 2 - ATENDIMENTO
            if (nivel == 2 &&
                (cargo == "ATENDENTE" ||
                 cargo == "RECEPCIONISTA"))
            {
                return modulo == Modulo.Clientes ||
                       modulo == Modulo.Animais ||
                       modulo == Modulo.Consultas;
            }

            // NÍVEL 2 - FINANCEIRO
            if (nivel == 2 &&
                (cargo == "FINANCEIRO" ||
                 cargo == "CAIXA"))
            {
                return modulo == Modulo.Financeiro;
            }

            // NÍVEL 2 - ESTOQUE
            if (nivel == 2 &&
                (cargo == "ESTOQUISTA" ||
                 cargo == "ALMOXARIFE"))
            {
                return modulo == Modulo.Estoque;
            }

            // NÍVEL 3 - VETERINÁRIO
            if (nivel == 3 &&
                cargo == "VETERINARIO")
            {
                return modulo == Modulo.Animais ||
                       modulo == Modulo.Consultas;
            }

            // Não reconheceu a combinação?
            // Não libera módulos operacionais.
            return false;
        }

        private void AplicarPermissoes()
        {
            foreach (KeyValuePair<Modulo, Button>
                entrada in botoesMenu)
            {
                entrada.Value.Visible =
                    PodeAcessar(entrada.Key);
            }

            // Mantém a compatibilidade com o
            // menu antigo do Designer.
            clientesToolStripMenuItem.Visible =
                PodeAcessar(Modulo.Clientes);

            string nome =
                DSProjectVet.Properties.Settings
                    .Default.NomeUsuarioLogado;

            if (string.IsNullOrWhiteSpace(nome))
                nome = "Usuário";

            lblUsuario.Text = nome;
            lblUsuarioLateral.Text = nome;
            lblCargo.Text = cargoSessao;
        }

        // ======================================
        // ABRIR MÓDULO
        // ======================================

        private void AbrirModulo(Modulo modulo)
        {
            // Impede a abertura mesmo se alguém
            // chamar o evento diretamente.
            if (!PodeAcessar(modulo))
            {
                MessageBox.Show(
                    this,
                    "Você não possui permissão " +
                    "para acessar este módulo.\n\n" +
                    "Nível: " + NivelAtual() +
                    "\nCargo: " + cargoSessao,
                    "Acesso restrito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            Form existente;

            if (formulariosAbertos.TryGetValue(
                modulo, out existente))
            {
                if (existente != null &&
                    !existente.IsDisposed)
                {
                    existente.WindowState =
                        FormWindowState.Maximized;

                    existente.Activate();

                    DestacarModulo(modulo);
                    return;
                }

                formulariosAbertos.Remove(modulo);
            }

            try
            {
                Form formulario =
                    CriarFormulario(modulo);

                if (formulario == null)
                    return;

                formulario.FormBorderStyle =
                    FormBorderStyle.None;

                formulario.MdiParent = this;

                formulariosAbertos[modulo] =
                    formulario;

                formulario.FormClosed += (s, e) =>
                {
                    Form registrado;

                    if (formulariosAbertos.TryGetValue(
                        modulo, out registrado) &&
                        ReferenceEquals(
                            registrado, formulario))
                    {
                        formulariosAbertos.Remove(modulo);
                    }
                };

                formulario.Show();

                formulario.WindowState =
                    FormWindowState.Maximized;

                formulario.Activate();

                DestacarModulo(modulo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Não foi possível abrir o módulo.\n\n" +
                    ex.Message,
                    "Med Vet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ======================================
        // CRIAR FORMULÁRIOS
        // ======================================

        private Form CriarFormulario(Modulo modulo)
        {
            switch (modulo)
            {
                case Modulo.Inicio:
                    return CriarTelaInicio();

                case Modulo.Clientes:
                    return new frmClientes();

                case Modulo.Animais:
                    return new frmAnimais();

                case Modulo.Estoque:
                    return new frmEstoque();

                case Modulo.Perfil:
                    return new Perfil();

                case Modulo.Consultas:
                    return CriarTelaConsultas();

                case Modulo.Financeiro:
                    return new TelaFluxoFinanceiro();

                default:
                    return null;
            }
        }

        // ======================================
        // INTEGRAÇÃO COM FORMULÁRIO DE CONSULTAS
        // ======================================

        private Form CriarTelaConsultas()
        {
            // Procura um formulário de consultas
            // existente no projeto.

            string[] nomesPossiveis =
            {
                "DSProjectVet.Forms.SCREENS.frmConsultas",
                "DSProjectVet.Forms.frmConsultas",
                "DSProjectVet.Forms.SCREENS.frmConsulta",
                "DSProjectVet.Forms.frmConsulta"
            };

            Assembly projeto =
                typeof(MDI_Menu).Assembly;

            foreach (string nome in nomesPossiveis)
            {
                Type tipo = projeto.GetType(
                    nome, false);

                if (tipo == null)
                    continue;

                if (!typeof(Form).IsAssignableFrom(tipo))
                    continue;

                if (tipo.GetConstructor(Type.EmptyTypes) ==
                    null)
                    continue;

                return (Form)Activator.CreateInstance(tipo);
            }

            // Caso a tela ainda não tenha sido
            // criada, abre a estrutura provisória.
            return CriarTelaProvisoria(
                "Consultas",
                "Gerenciamento de consultas veterinárias",
                "O módulo está preparado para receber " +
                "o formulário de consultas do projeto.");
        }

        // ======================================
        // TELA INICIAL
        // ======================================

        private Form CriarTelaInicio()
        {
            Form inicio = new Form();

            inicio.Text = "Início";
            inicio.BackColor = fundo;

            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.Padding =
                new Padding(28, 25, 28, 25);

            layout.ColumnCount = 1;
            layout.RowCount = 3;

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 175F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 65F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            inicio.Controls.Add(layout);

            Panel boasVindas = new Panel();

            boasVindas.BackColor = Color.White;
            boasVindas.Dock = DockStyle.Fill;

            boasVindas.Margin =
                new Padding(0, 0, 0, 12);

            layout.Controls.Add(
                boasVindas, 0, 0);

            Label titulo = new Label();

            titulo.Text = "Bem-vindo ao Med Vet";

            titulo.Font = new Font(
                "Segoe UI", 26F, FontStyle.Bold);

            titulo.ForeColor = texto;

            titulo.SetBounds(
                25, 23, 660, 55);

            boasVindas.Controls.Add(titulo);

            Label descricao = new Label();

            descricao.Text =
                "Gerencie os principais módulos " +
                "da clínica em um só lugar.";

            descricao.Font =
                new Font("Segoe UI", 11F);

            descricao.ForeColor = cinza;

            descricao.SetBounds(
                28, 87, 660, 32);

            boasVindas.Controls.Add(descricao);

            Label sistema = new Label();

            sistema.Text = "●  SISTEMA DISPONÍVEL";

            sistema.ForeColor = verde;

            sistema.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            sistema.SetBounds(
                28, 132, 280, 23);

            boasVindas.Controls.Add(sistema);

            Label atalhosTitulo = new Label();

            atalhosTitulo.Text = "Acesso rápido";

            atalhosTitulo.Dock = DockStyle.Fill;

            atalhosTitulo.Font = new Font(
                "Segoe UI", 17F, FontStyle.Bold);

            atalhosTitulo.ForeColor = texto;

            layout.Controls.Add(
                atalhosTitulo, 0, 1);

            FlowLayoutPanel atalhos =
                new FlowLayoutPanel();

            atalhos.Dock = DockStyle.Fill;
            atalhos.AutoScroll = true;
            atalhos.WrapContents = true;

            atalhos.Padding = new Padding(0);
            atalhos.Margin = new Padding(0);

            layout.Controls.Add(atalhos, 0, 2);

            AdicionarAtalho(
                atalhos, Modulo.Clientes, "Clientes");

            AdicionarAtalho(
                atalhos, Modulo.Animais, "Animais");

            AdicionarAtalho(
                atalhos, Modulo.Consultas, "Consultas");

            AdicionarAtalho(
                atalhos, Modulo.Estoque, "Estoque");

            AdicionarAtalho(
                atalhos, Modulo.Financeiro,
                "Fluxo financeiro");

            return inicio;
        }

        private void AdicionarAtalho(
            FlowLayoutPanel painel,
            Modulo modulo,
            string titulo)
        {
            if (!PodeAcessar(modulo))
                return;

            Button botao = new Button();

            botao.Text = titulo + "\n\nAbrir módulo";

            botao.Size = new Size(210, 125);

            botao.Margin =
                new Padding(0, 0, 14, 14);

            botao.BackColor = Color.White;

            botao.ForeColor = texto;

            botao.Font = new Font(
                "Segoe UI", 12F, FontStyle.Bold);

            botao.FlatStyle = FlatStyle.Flat;

            botao.FlatAppearance.BorderColor =
                Color.FromArgb(226, 234, 230);

            botao.FlatAppearance.BorderSize = 1;

            botao.Cursor = Cursors.Hand;

            botao.Click += (s, e) =>
            {
                AbrirModulo(modulo);
            };

            painel.Controls.Add(botao);
        }

        // ======================================
        // TELA PROVISÓRIA
        // ======================================

        private Form CriarTelaProvisoria(
            string titulo,
            string subtitulo,
            string descricao)
        {
            Form formulario = new Form();

            formulario.Text = titulo;
            formulario.BackColor = fundo;

            Panel cartao = new Panel();

            cartao.BackColor = Color.White;

            cartao.Dock = DockStyle.Top;
            cartao.Height = 210;

            cartao.Padding =
                new Padding(28);

            formulario.Controls.Add(cartao);

            Label lblTitulo = new Label();

            lblTitulo.Text = titulo;

            lblTitulo.Font = new Font(
                "Segoe UI", 25F, FontStyle.Bold);

            lblTitulo.ForeColor = texto;
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Height = 58;

            cartao.Controls.Add(lblTitulo);

            Label lblSubtitulo = new Label();

            lblSubtitulo.Text =
                subtitulo + "\n\n" + descricao;

            lblSubtitulo.Font =
                new Font("Segoe UI", 11F);

            lblSubtitulo.ForeColor = cinza;
            lblSubtitulo.Dock = DockStyle.Bottom;
            lblSubtitulo.Height = 100;

            cartao.Controls.Add(lblSubtitulo);

            return formulario;
        }

        // ======================================
        // DESTACAR MÓDULO ATIVO
        // ======================================

        private void DestacarModulo(Modulo modulo)
        {
            foreach (KeyValuePair<Modulo, Button>
                entrada in botoesMenu)
            {
                bool ativo =
                    entrada.Key == modulo;

                entrada.Value.BackColor =
                    ativo ? verdeSelecionado : verdeEscuro;

                entrada.Value.ForeColor =
                    ativo
                    ? Color.White
                    : Color.FromArgb(211, 232, 225);
            }

            if (lblTitulo == null)
                return;

            switch (modulo)
            {
                case Modulo.Inicio:
                    lblTitulo.Text = "Visão geral";
                    lblSubtitulo.Text =
                        "Painel principal do sistema";
                    break;

                case Modulo.Clientes:
                    lblTitulo.Text = "Clientes";
                    lblSubtitulo.Text =
                        "Gestão de clientes da clínica";
                    break;

                case Modulo.Animais:
                    lblTitulo.Text = "Animais";
                    lblSubtitulo.Text =
                        "Gerenciamento dos animais";
                    break;

                case Modulo.Consultas:
                    lblTitulo.Text = "Consultas";
                    lblSubtitulo.Text =
                        "Atendimentos veterinários";
                    break;

                case Modulo.Estoque:
                    lblTitulo.Text = "Estoque";
                    lblSubtitulo.Text =
                        "Produtos e medicamentos";
                    break;

                case Modulo.Financeiro:
                    lblTitulo.Text = "Fluxo financeiro";
                    lblSubtitulo.Text =
                        "Entradas, saídas e movimentações";
                    break;

                case Modulo.Perfil:
                    lblTitulo.Text = "Meu perfil";
                    lblSubtitulo.Text =
                        "Informações do usuário";
                    break;
            }
        }

        private void AtualizarSelecaoPeloFormulario()
        {
            Form atual = this.ActiveMdiChild;

            if (atual == null)
                return;

            foreach (KeyValuePair<Modulo, Form>
                entrada in formulariosAbertos)
            {
                if (ReferenceEquals(
                    entrada.Value, atual))
                {
                    DestacarModulo(entrada.Key);
                    return;
                }
            }
        }

        // ======================================
        // FORMULÁRIO FINANCEIRO CENTRALIZADO
        // ======================================

        private class TelaFluxoFinanceiro : Form
        {
            public TelaFluxoFinanceiro()
            {
                this.Text = "Fluxo financeiro";

                this.BackColor =
                    Color.FromArgb(246, 250, 248);

                this.Font =
                    new Font("Segoe UI", 10F);

                TableLayoutPanel layout =
                    new TableLayoutPanel();

                layout.Dock = DockStyle.Fill;
                layout.Padding =
                    new Padding(24);

                layout.RowCount = 2;
                layout.ColumnCount = 1;

                layout.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent, 100F));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 110F));

                layout.RowStyles.Add(
                    new RowStyle(
                        SizeType.Percent, 100F));

                Controls.Add(layout);

                Panel cabecalho = new Panel();

                cabecalho.Dock = DockStyle.Fill;
                cabecalho.BackColor = Color.White;

                layout.Controls.Add(
                    cabecalho, 0, 0);

                Label titulo = new Label();

                titulo.Text = "Gestão Financeira";

                titulo.Font = new Font(
                    "Segoe UI", 21F, FontStyle.Bold);

                titulo.ForeColor =
                    Color.FromArgb(35, 49, 45);

                titulo.SetBounds(
                    23, 13, 580, 44);

                cabecalho.Controls.Add(titulo);

                Label subtitulo = new Label();

                subtitulo.Text =
                    "Central de entradas, saídas e " +
                    "movimentações financeiras.";

                subtitulo.Font =
                    new Font("Segoe UI", 10F);

                subtitulo.ForeColor = Color.Gray;

                subtitulo.SetBounds(
                    25, 63, 650, 28);

                cabecalho.Controls.Add(subtitulo);

                TabControl abas = new TabControl();

                abas.Dock = DockStyle.Fill;
                abas.Font =
                    new Font("Segoe UI", 11F);

                abas.Margin =
                    new Padding(0, 16, 0, 0);

                layout.Controls.Add(abas, 0, 1);

                CriarAba(
                    abas,
                    "Resumo",
                    "Visão geral do fluxo financeiro",
                    "Aqui serão apresentados o saldo, " +
                    "as entradas, as saídas e os totais.");

                CriarAba(
                    abas,
                    "Entradas",
                    "Entradas financeiras",
                    "Recebimentos e outras entradas " +
                    "serão centralizados nesta aba.");

                CriarAba(
                    abas,
                    "Saídas",
                    "Saídas financeiras",
                    "Despesas e pagamentos serão " +
                    "centralizados nesta aba.");

                CriarAba(
                    abas,
                    "Movimentações",
                    "Histórico de movimentações",
                    "O histórico financeiro ficará " +
                    "disponível nesta aba.");
            }

            private void CriarAba(
                TabControl abas,
                string nome,
                string titulo,
                string descricao)
            {
                TabPage pagina = new TabPage(nome);

                pagina.BackColor =
                    Color.FromArgb(246, 250, 248);

                abas.TabPages.Add(pagina);

                Panel cartao = new Panel();

                cartao.Dock = DockStyle.Top;
                cartao.Height = 170;

                cartao.BackColor = Color.White;

                cartao.Padding =
                    new Padding(24);

                pagina.Controls.Add(cartao);

                Label lblTitulo = new Label();

                lblTitulo.Text = titulo;

                lblTitulo.Font = new Font(
                    "Segoe UI", 17F, FontStyle.Bold);

                lblTitulo.ForeColor =
                    Color.FromArgb(35, 49, 45);

                lblTitulo.Dock = DockStyle.Top;
                lblTitulo.Height = 50;

                cartao.Controls.Add(lblTitulo);

                Label lblDescricao = new Label();

                lblDescricao.Text =
                    descricao +
                    "\n\nAguardando integração com o banco.";

                lblDescricao.Font =
                    new Font("Segoe UI", 10F);

                lblDescricao.ForeColor = Color.Gray;

                lblDescricao.Dock = DockStyle.Fill;

                cartao.Controls.Add(lblDescricao);

                lblDescricao.BringToFront();
            }
        }

        // ======================================
        // EVENTOS DO DESIGNER
        // ======================================

        private void MDI_Menu_Load(
            object sender, EventArgs e)
        {
            AplicarPermissoes();
        }

        private void perfilToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            AbrirModulo(Modulo.Perfil);
        }

        private void clientesToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            AbrirModulo(Modulo.Clientes);
        }

        private void animaisToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            AbrirModulo(Modulo.Animais);
        }

        private void estoqueToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            AbrirModulo(Modulo.Estoque);
        }

        // Para vincular se o Designer usar
        // esses nomes de eventos.
        private void consultasToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            AbrirModulo(Modulo.Consultas);
        }

        private void fluxoFinanceiroToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            AbrirModulo(Modulo.Financeiro);
        }

        private void toolStripMenuItem1_Click(
            object sender, EventArgs e)
        {
        }

        private void menuDashBoard_ItemClicked(
            object sender,
            ToolStripItemClickedEventArgs e)
        {
        }
    }
}
