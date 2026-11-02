
using DSProjectVet.Forms.SCREENS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;


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
            Relatorios,
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

            sinal.Text = "🐾";
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
                "Início",
                Properties.Resources.iconInicio);

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Clientes,
                "Clientes",
                Properties.Resources.iconClientes);

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Animais,
                "Animais",
                Properties.Resources.iconAnimais);

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Consultas,
                "Consultas",
                Properties.Resources.iconConsultas);

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Estoque,
                "Estoque",
                Properties.Resources.iconEstoque);

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Financeiro,
                "Fluxo financeiro",
                Properties.Resources.iconFinanceiro);

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Relatorios,
                "Relatórios",
                Properties.Resources.iconRelatorios);

            AdicionarBotaoMenu(
                listaMenu,
                Modulo.Perfil,
                "Meu perfil",
                Properties.Resources.iconPerfil);

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
            string titulo,
            Image icone)
        {
            Button botao = new Button();

            botao.Image = new Bitmap(icone, new Size(24, 24));

            botao.ImageAlign =
                ContentAlignment.MiddleLeft;

            botao.TextImageRelation =
                TextImageRelation.ImageBeforeText;

            botao.TextAlign =
                ContentAlignment.MiddleCenter;

            botao.Padding =
                new Padding(16, 0, 12, 0);

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

            // A central pode ser aberta pelos três níveis.
            // Cada relatório terá suas permissões próprias.
            if (modulo == Modulo.Relatorios)
            {
                return NivelAtual() >= 1 &&
                       NivelAtual() <= 3;
            }

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
                    return new frmFluxoFinanceiro();

                case Modulo.Relatorios:
                    return new frmRelatorios();

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
            inicio.AutoScroll = true;
            inicio.Font = new Font("Segoe UI", 10F);

            // ==========================================
            // INFORMAÇÕES DO USUÁRIO
            // ==========================================

            string nomeUsuario =
                Properties.Settings.Default.NomeUsuarioLogado;

            if (string.IsNullOrWhiteSpace(nomeUsuario))
                nomeUsuario = "Colaborador";

            int nivel =
                Properties.Settings.Default.NivelUsuarioLogado;

            string cargo = "Colaborador";

            switch (nivel)
            {
                case 1:
                    cargo = "Administrador";
                    break;

                case 2:
                    cargo = "Atendente";
                    break;

                case 3:
                    cargo = "Veterinário";
                    break;
            }

            string saudacao;

            if (DateTime.Now.Hour < 12)
                saudacao = "Bom dia";
            else if (DateTime.Now.Hour < 18)
                saudacao = "Boa tarde";
            else
                saudacao = "Boa noite";

            string primeiroNome = nomeUsuario.Split(' ')[0];

            string dataHoje = DateTime.Now.ToString(
                "dddd, dd 'de' MMMM 'de' yyyy",
                new System.Globalization.CultureInfo("pt-BR"));

            // ==========================================
            // LAYOUT PRINCIPAL
            // ==========================================

            TableLayoutPanel layout =
                new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(24, 22, 24, 15);
            layout.ColumnCount = 1;
            layout.RowCount = 5;

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 180F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 130F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 60F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 42F));

            inicio.Controls.Add(layout);

            // ==========================================
            // 1. BOAS-VINDAS
            // ==========================================

            TableLayoutPanel painelBoasVindas =
                new TableLayoutPanel();

            painelBoasVindas.Dock = DockStyle.Fill;
            painelBoasVindas.ColumnCount = 2;
            painelBoasVindas.RowCount = 1;

            painelBoasVindas.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 70F));

            painelBoasVindas.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 30F));

            painelBoasVindas.Margin =
                new Padding(0, 0, 0, 15);

            layout.Controls.Add(painelBoasVindas, 0, 0);

            // CARTÃO PRINCIPAL
            Panel cardBoasVindas = new Panel();

            cardBoasVindas.Dock = DockStyle.Fill;
            cardBoasVindas.BackColor = Color.White;
            cardBoasVindas.Padding = new Padding(23);

            cardBoasVindas.Margin =
                new Padding(0, 0, 12, 0);

            painelBoasVindas.Controls.Add(
                cardBoasVindas, 0, 0);

            TableLayoutPanel textosBoasVindas =
                new TableLayoutPanel();

            textosBoasVindas.Dock = DockStyle.Fill;
            textosBoasVindas.RowCount = 4;
            textosBoasVindas.ColumnCount = 1;

            textosBoasVindas.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 23));

            textosBoasVindas.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 52));

            textosBoasVindas.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            textosBoasVindas.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 26));

            cardBoasVindas.Controls.Add(textosBoasVindas);

            Label lblEtiqueta = new Label();

            lblEtiqueta.Text = "PAINEL INICIAL";
            lblEtiqueta.Dock = DockStyle.Fill;

            lblEtiqueta.ForeColor = verdeEscuro;

            lblEtiqueta.Font = new Font(
                "Segoe UI", 8.5F, FontStyle.Bold);

            textosBoasVindas.Controls.Add(
                lblEtiqueta, 0, 0);

            Label lblBoasVindas = new Label();

            lblBoasVindas.Text =
                saudacao + ", " + primeiroNome + "!";

            lblBoasVindas.Dock = DockStyle.Fill;

            lblBoasVindas.AutoEllipsis = true;

            lblBoasVindas.Font = new Font(
                "Segoe UI", 23F, FontStyle.Bold);

            lblBoasVindas.ForeColor = texto;

            textosBoasVindas.Controls.Add(
                lblBoasVindas, 0, 1);

            Label lblDescricao = new Label();

            lblDescricao.Text =
                "Bem-vindo ao Med Vet. " +
                "Acompanhe a clínica e acesse os " +
                "módulos do sistema em um só lugar.";

            lblDescricao.Dock = DockStyle.Fill;

            lblDescricao.ForeColor = cinza;
            lblDescricao.Font = new Font("Segoe UI", 10F);

            textosBoasVindas.Controls.Add(
                lblDescricao, 0, 2);

            Label lblStatusSistema = new Label();

            lblStatusSistema.Text =
                "●  CARREGANDO INDICADORES";

            lblStatusSistema.Dock = DockStyle.Fill;

            lblStatusSistema.ForeColor = verdeEscuro;

            lblStatusSistema.Font = new Font(
                "Segoe UI", 8.5F, FontStyle.Bold);

            textosBoasVindas.Controls.Add(
                lblStatusSistema, 0, 3);

            // CARTÃO DATA E PERFIL
            Panel cardData = new Panel();

            cardData.Dock = DockStyle.Fill;
            cardData.BackColor = verdeEscuro;

            painelBoasVindas.Controls.Add(
                cardData, 1, 0);

            TableLayoutPanel infoData =
                new TableLayoutPanel();

            infoData.Dock = DockStyle.Fill;
            infoData.Padding = new Padding(16, 13, 12, 12);
            infoData.RowCount = 4;
            infoData.ColumnCount = 1;

            infoData.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 29));

            infoData.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

            infoData.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 25));

            infoData.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            cardData.Controls.Add(infoData);

            Label lblDataTitulo = new Label();

            lblDataTitulo.Text = "HOJE";
            lblDataTitulo.Dock = DockStyle.Fill;

            lblDataTitulo.ForeColor =
                Color.FromArgb(199, 240, 221);

            lblDataTitulo.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            infoData.Controls.Add(lblDataTitulo, 0, 0);

            Label lblDia = new Label();

            lblDia.Text =
                DateTime.Now.ToString("dd/MM/yyyy");

            lblDia.Dock = DockStyle.Fill;

            lblDia.ForeColor = Color.White;

            lblDia.Font = new Font(
                "Segoe UI", 20F, FontStyle.Bold);

            lblDia.AutoEllipsis = true;

            infoData.Controls.Add(lblDia, 0, 1);

            Label lblCargoTitulo = new Label();

            lblCargoTitulo.Text = "SEU ACESSO";
            lblCargoTitulo.Dock = DockStyle.Fill;

            lblCargoTitulo.ForeColor =
                Color.FromArgb(199, 240, 221);

            lblCargoTitulo.Font = new Font(
                "Segoe UI", 8.5F, FontStyle.Bold);

            infoData.Controls.Add(lblCargoTitulo, 0, 2);

            Label lblCargo = new Label();

            lblCargo.Text = cargo;
            lblCargo.Dock = DockStyle.Fill;

            lblCargo.ForeColor = Color.White;

            lblCargo.Font = new Font(
                "Segoe UI", 13F, FontStyle.Bold);

            lblCargo.AutoEllipsis = true;

            infoData.Controls.Add(lblCargo, 0, 3);

            // ==========================================
            // 2. INDICADORES DO SISTEMA
            // ==========================================

            TableLayoutPanel indicadores =
                new TableLayoutPanel();

            indicadores.Dock = DockStyle.Fill;
            indicadores.ColumnCount = 3;

            indicadores.Margin = new Padding(0);

            for (int i = 0; i < 3; i++)
            {
                indicadores.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 33.3333F));
            }

            layout.Controls.Add(indicadores, 0, 1);

            Label lblConsultasHoje;
            Label lblPacientes;
            Label lblConsultasMes;

            indicadores.Controls.Add(
                CriarIndicadorInicio(
                    "Consultas hoje",
                    "Atendimentos agendados para hoje",
                    out lblConsultasHoje),
                0, 0);

            indicadores.Controls.Add(
                CriarIndicadorInicio(
                    "Pacientes cadastrados",
                    "Animais registrados na clínica",
                    out lblPacientes),
                1, 0);

            indicadores.Controls.Add(
                CriarIndicadorInicio(
                    "Consultas neste mês",
                    "Atendimentos do mês atual",
                    out lblConsultasMes),
                2, 0);

            // ==========================================
            // 3. TÍTULO DE ACESSO RÁPIDO
            // ==========================================

            Panel cabecalhoAtalhos = new Panel();

            cabecalhoAtalhos.Dock = DockStyle.Fill;

            layout.Controls.Add(cabecalhoAtalhos, 0, 2);

            Label lblAtalhos = new Label();

            lblAtalhos.Text = "Acesso rápido";
            lblAtalhos.Font = new Font(
                "Segoe UI", 17F, FontStyle.Bold);

            lblAtalhos.ForeColor = texto;

            lblAtalhos.SetBounds(0, 8, 320, 42);

            cabecalhoAtalhos.Controls.Add(lblAtalhos);

            // ==========================================
            // 4. CONTEÚDO PRINCIPAL
            // ==========================================

            TableLayoutPanel areaPrincipal =
                new TableLayoutPanel();

            areaPrincipal.Dock = DockStyle.Fill;
            areaPrincipal.ColumnCount = 2;
            areaPrincipal.RowCount = 1;

            areaPrincipal.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 72F));

            areaPrincipal.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 28F));

            areaPrincipal.Margin = new Padding(0);

            layout.Controls.Add(areaPrincipal, 0, 3);

            // ==========================================
            // ATALHOS
            // ==========================================

            FlowLayoutPanel atalhos =
                new FlowLayoutPanel();

            atalhos.Dock = DockStyle.Fill;

            atalhos.FlowDirection =
                FlowDirection.LeftToRight;

            atalhos.WrapContents = true;
            atalhos.AutoScroll = true;

            atalhos.Margin =
                new Padding(0, 0, 12, 0);

            atalhos.Padding = new Padding(0);

            areaPrincipal.Controls.Add(
                atalhos, 0, 0);

            AdicionarAtalhoInicio(
                atalhos,
                Modulo.Clientes,
                "Clientes",
                "Cadastros e responsáveis",
                Properties.Resources.iconClientes);

            AdicionarAtalhoInicio(
                atalhos,
                Modulo.Animais,
                "Animais",
                "Pacientes cadastrados",
                Properties.Resources.iconAnimais);

            AdicionarAtalhoInicio(
                atalhos,
                Modulo.Consultas,
                "Consultas",
                "Agenda e atendimentos",
                Properties.Resources.iconConsultas);

            AdicionarAtalhoInicio(
                atalhos,
                Modulo.Estoque,
                "Estoque",
                "Produtos e medicamentos",
                Properties.Resources.iconEstoque);

            AdicionarAtalhoInicio(
                atalhos,
                Modulo.Financeiro,
                "Fluxo financeiro",
                "Entradas e saídas",
                Properties.Resources.iconFinanceiro);

            // Relatórios não exige criar outro recurso.
            AdicionarAtalhoInicio(
                atalhos,
                Modulo.Relatorios,
                "Relatórios",
                "Indicadores e análises",
                Properties.Resources.iconRelatorios);

            // ==========================================
            // PAINEL LATERAL
            // ==========================================

            TableLayoutPanel lateral =
                new TableLayoutPanel();

            lateral.Dock = DockStyle.Fill;
            lateral.RowCount = 2;
            lateral.ColumnCount = 1;

            lateral.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 165F));

            lateral.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            areaPrincipal.Controls.Add(lateral, 1, 0);

            // CARTÃO DE USUÁRIO
            Panel cardPerfil = new Panel();

            cardPerfil.Dock = DockStyle.Fill;
            cardPerfil.BackColor = Color.White;

            cardPerfil.Padding = new Padding(16);

            cardPerfil.Margin =
                new Padding(0, 0, 0, 12);

            lateral.Controls.Add(cardPerfil, 0, 0);

            Label tituloPerfil = new Label();

            tituloPerfil.Text = "Sua sessão";

            tituloPerfil.Font = new Font(
                "Segoe UI", 12F, FontStyle.Bold);

            tituloPerfil.ForeColor = texto;

            tituloPerfil.SetBounds(14, 13, 210, 32);

            cardPerfil.Controls.Add(tituloPerfil);

            Label nomePerfil = new Label();

            nomePerfil.Text = nomeUsuario;

            nomePerfil.Font = new Font(
                "Segoe UI", 11F, FontStyle.Bold);

            nomePerfil.ForeColor = texto;

            nomePerfil.AutoEllipsis = true;

            nomePerfil.SetBounds(14, 54, 230, 32);

            cardPerfil.Controls.Add(nomePerfil);

            Label nivelPerfil = new Label();

            nivelPerfil.Text = cargo;

            nivelPerfil.Font = new Font(
                "Segoe UI", 10F);

            nivelPerfil.ForeColor = verdeEscuro;

            nivelPerfil.SetBounds(14, 87, 225, 28);

            cardPerfil.Controls.Add(nivelPerfil);

            Label acessoPerfil = new Label();

            acessoPerfil.Text =
                "Os módulos são exibidos de acordo " +
                "com as permissões da sua conta.";

            acessoPerfil.Font = new Font(
                "Segoe UI", 8.5F);

            acessoPerfil.ForeColor = cinza;

            acessoPerfil.SetBounds(
                14, 118, 225, 37);

            cardPerfil.Controls.Add(acessoPerfil);

            // CARTÃO DE ORIENTAÇÕES
            Panel cardAjuda = new Panel();

            cardAjuda.Dock = DockStyle.Fill;
            cardAjuda.BackColor = Color.White;

            cardAjuda.Padding = new Padding(16);

            lateral.Controls.Add(cardAjuda, 0, 1);

            Label tituloAjuda = new Label();

            tituloAjuda.Text = "Para começar";

            tituloAjuda.Font = new Font(
                "Segoe UI", 12F, FontStyle.Bold);

            tituloAjuda.ForeColor = texto;

            tituloAjuda.SetBounds(14, 13, 230, 33);

            cardAjuda.Controls.Add(tituloAjuda);

            Label textoAjuda = new Label();

            textoAjuda.Text =
                "01  Selecione um módulo.\n\n" +
                "02  Consulte ou cadastre dados.\n\n" +
                "03  Utilize relatórios para " +
                "acompanhar os resultados.";

            textoAjuda.Font = new Font(
                "Segoe UI", 9F);

            textoAjuda.ForeColor = cinza;

            textoAjuda.SetBounds(
                14, 55, 225, 145);

            cardAjuda.Controls.Add(textoAjuda);

            // ==========================================
            // 5. RODAPÉ
            // ==========================================

            TableLayoutPanel rodape =
                new TableLayoutPanel();

            rodape.Dock = DockStyle.Fill;

            rodape.ColumnCount = 2;

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            rodape.ColumnStyles.Add(
                new ColumnStyle(SizeType.AutoSize));

            layout.Controls.Add(rodape, 0, 4);

            Label lblRodape = new Label();

            lblRodape.Text =
                "Med Vet  |  Sistema de gestão veterinária";

            lblRodape.ForeColor = cinza;

            lblRodape.Font = new Font(
                "Segoe UI", 8.5F);

            lblRodape.Dock = DockStyle.Fill;

            lblRodape.TextAlign =
                ContentAlignment.MiddleLeft;

            rodape.Controls.Add(lblRodape, 0, 0);

            Label lblDataRodape = new Label();

            lblDataRodape.Text = dataHoje;

            lblDataRodape.ForeColor = cinza;

            lblDataRodape.Font = new Font(
                "Segoe UI", 8.5F);

            lblDataRodape.AutoSize = true;

            lblDataRodape.Margin =
                new Padding(0, 10, 0, 0);

            rodape.Controls.Add(lblDataRodape, 1, 0);

            // ==========================================
            // CONSULTAR INDICADORES REAIS
            // ==========================================

            inicio.Shown += async (s, e) =>
            {
                await AtualizarIndicadoresInicioAsync(
                    inicio,
                    lblConsultasHoje,
                    lblPacientes,
                    lblConsultasMes,
                    lblStatusSistema);
            };

            return inicio;
        }


        private async Task AtualizarIndicadoresInicioAsync(
            Form tela,
            Label lblConsultasHoje,
            Label lblPacientes,
            Label lblConsultasMes,
            Label lblStatusSistema)
        {
            try
            {
                string connectionString =
                    ObterConnectionStringInicio();

                int[] indicadores = await Task.Run(() =>
                {
                    string sql = @"
                SELECT
                    (
                        SELECT COUNT(*)
                        FROM dbo.Consultas
                        WHERE DataHora >=
                            CAST(GETDATE() AS DATE)
                          AND DataHora <
                            DATEADD(
                                DAY, 1,
                                CAST(GETDATE() AS DATE))
                    ) AS ConsultasHoje,

                    (
                        SELECT COUNT(*)
                        FROM dbo.Animais
                    ) AS Pacientes,

                    (
                        SELECT COUNT(*)
                        FROM dbo.Consultas
                        WHERE DataHora >=
                            DATEADD(
                                MONTH,
                                DATEDIFF(
                                    MONTH, 0, GETDATE()),
                                0)
                          AND DataHora <
                            DATEADD(
                                MONTH, 1,
                                DATEADD(
                                    MONTH,
                                    DATEDIFF(
                                        MONTH, 0, GETDATE()),
                                    0))
                    ) AS ConsultasMes;
            ";

                    using (SqlConnection conexao =
                        new SqlConnection(connectionString))
                    using (SqlCommand comando =
                        new SqlCommand(sql, conexao))
                    {
                        conexao.Open();

                        using (SqlDataReader leitor =
                            comando.ExecuteReader())
                        {
                            if (!leitor.Read())
                                throw new Exception(
                                    "Sem dados retornados.");

                            return new int[]
                            {
                        Convert.ToInt32(
                            leitor["ConsultasHoje"]),

                        Convert.ToInt32(
                            leitor["Pacientes"]),

                        Convert.ToInt32(
                            leitor["ConsultasMes"])
                            };
                        }
                    }
                });

                if (tela.IsDisposed)
                    return;

                lblConsultasHoje.Text =
                    indicadores[0].ToString("N0");

                lblPacientes.Text =
                    indicadores[1].ToString("N0");

                lblConsultasMes.Text =
                    indicadores[2].ToString("N0");

                lblStatusSistema.Text =
                    "●  INDICADORES ATUALIZADOS";

                lblStatusSistema.ForeColor = verdeEscuro;
            }
            catch (Exception)
            {
                if (tela.IsDisposed)
                    return;

                lblConsultasHoje.Text = "—";
                lblPacientes.Text = "—";
                lblConsultasMes.Text = "—";

                lblStatusSistema.Text =
                    "●  INDICADORES INDISPONÍVEIS";

                lblStatusSistema.ForeColor =
                    Color.FromArgb(190, 115, 40);

                // A falha nos indicadores não impede
                // o usuário de navegar pelo sistema.
            }
        }

        private string ObterConnectionStringInicio()
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
                "Connection string não encontrada.");
        }



        private Panel CriarIndicadorInicio(
            string titulo,
            string descricao,
            out Label valor)
        {
            Panel card = new Panel();

            card.Dock = DockStyle.Fill;
            card.BackColor = Color.White;

            card.Margin = new Padding(0, 0, 12, 15);
            card.Padding = new Padding(17, 13, 12, 12);

            TableLayoutPanel conteudo =
                new TableLayoutPanel();

            conteudo.Dock = DockStyle.Fill;
            conteudo.ColumnCount = 1;
            conteudo.RowCount = 3;

            conteudo.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 26F));

            conteudo.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            conteudo.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 23F));

            card.Controls.Add(conteudo);

            Label labelTitulo = new Label();

            labelTitulo.Text = titulo;
            labelTitulo.Dock = DockStyle.Fill;

            labelTitulo.ForeColor = cinza;
            labelTitulo.Font = new Font(
                "Segoe UI", 9F, FontStyle.Bold);

            conteudo.Controls.Add(labelTitulo, 0, 0);

            valor = new Label();

            valor.Text = "—";
            valor.Dock = DockStyle.Fill;

            valor.ForeColor = verdeEscuro;
            valor.Font = new Font(
                "Segoe UI", 23F, FontStyle.Bold);

            valor.AutoEllipsis = true;

            conteudo.Controls.Add(valor, 0, 1);

            Label labelDescricao = new Label();

            labelDescricao.Text = descricao;
            labelDescricao.Dock = DockStyle.Fill;

            labelDescricao.ForeColor = cinza;
            labelDescricao.Font = new Font(
                "Segoe UI", 8F);

            labelDescricao.AutoEllipsis = true;

            conteudo.Controls.Add(labelDescricao, 0, 2);

            return card;
        }


        private void AbrirAtalhoInicio(Modulo modulo)
        {
            if (!PodeAcessar(modulo))
            {
                MessageBox.Show(
                    this,
                    "Seu usuário não possui acesso a este módulo.",
                    "Acesso restrito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            Form destino = CriarFormulario(modulo);

            if (destino == null)
                return;

            // Verifica se já existe uma janela desse tipo.
            foreach (Form aberto in MdiChildren)
            {
                if (aberto.GetType() == destino.GetType())
                {
                    destino.Dispose();

                    if (aberto.WindowState ==
                        FormWindowState.Minimized)
                    {
                        aberto.WindowState =
                            FormWindowState.Normal;
                    }

                    aberto.Activate();
                    aberto.BringToFront();

                    return;
                }
            }

            // Abre dentro do MDI principal.
            destino.MdiParent = this;

            destino.WindowState =
                FormWindowState.Maximized;

            destino.Show();
            destino.Activate();
        }





        private void AdicionarAtalhoInicio(
            FlowLayoutPanel painel,
            Modulo modulo,
            string titulo,
            string descricao,
            Image icone)
        {
            // Não mostra módulos não autorizados.
            if (!PodeAcessar(modulo))
                return;

            Panel card = new Panel();

            card.Size = new Size(235, 125);

            card.BackColor = Color.White;

            card.Margin =
                new Padding(0, 0, 12, 12);

            card.BorderStyle =
                BorderStyle.FixedSingle;



            card.Cursor = Cursors.Hand;

            // ======================================
            // ÁREA DO ÍCONE
            // ======================================

            Panel fundoIcone = new Panel();

            fundoIcone.BackColor =
                Color.FromArgb(225, 248, 240);

            fundoIcone.Size = new Size(52, 52);

            fundoIcone.Location =
                new Point(15, 18);

            fundoIcone.Cursor = Cursors.Hand;

            card.Controls.Add(fundoIcone);

            PictureBox imagem = new PictureBox();

            imagem.Image = icone;

            imagem.SizeMode =
                PictureBoxSizeMode.Zoom;

            imagem.Size = new Size(32, 32);

            imagem.Location =
                new Point(10, 10);

            imagem.BackColor =
                Color.ForestGreen;

            imagem.Cursor = Cursors.Hand;

            fundoIcone.Controls.Add(imagem);

            // ======================================
            // TÍTULO DO ATALHO
            // ======================================

            Label lblTitulo = new Label();

            lblTitulo.Text = titulo;

            lblTitulo.Font = new Font(
                "Segoe UI", 11F, FontStyle.Bold);

            lblTitulo.ForeColor = texto;

            lblTitulo.SetBounds(
                78, 19, 145, 34);

            lblTitulo.AutoEllipsis = true;

            lblTitulo.Cursor = Cursors.Hand;

            card.Controls.Add(lblTitulo);

            // ======================================
            // DESCRIÇÃO
            // ======================================

            Label lblDescricao = new Label();

            lblDescricao.Text = descricao;

            lblDescricao.Font =
                new Font("Segoe UI", 8.5F);

            lblDescricao.ForeColor = cinza;

            lblDescricao.SetBounds(
                78, 53, 145, 39);

            lblDescricao.AutoEllipsis = true;

            lblDescricao.Cursor = Cursors.Hand;

            card.Controls.Add(lblDescricao);

            // ======================================
            // TEXTO INFERIOR
            // ======================================

            Label lblAcessar = new Label();

            lblAcessar.Text = "ACESSAR MÓDULO";

            lblAcessar.Font = new Font(
                "Segoe UI", 8F, FontStyle.Bold);

            lblAcessar.ForeColor = verdeEscuro;

            lblAcessar.SetBounds(
                15, 96, 200, 20);

            lblAcessar.Cursor = Cursors.Hand;

            card.Controls.Add(lblAcessar);

            // ======================================
            // NAVEGAÇÃO
            // ======================================

            EventHandler abrir = (s, e) =>
            {
                AbrirAtalhoInicio(modulo);
            };

            card.Click += abrir;
            fundoIcone.Click += abrir;
            imagem.Click += abrir;

            lblTitulo.Click += abrir;
            lblDescricao.Click += abrir;
            lblAcessar.Click += abrir;

            // ======================================
            // EFEITO HOVER
            // ======================================

            card.MouseEnter += (s, e) =>
            {
                card.BackColor =
                    Color.FromArgb(244, 253, 248);
            };

            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.White;
            };

            painel.Controls.Add(card);
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

                case Modulo.Relatorios:
                    lblTitulo.Text = "Relatórios";
                    lblSubtitulo.Text =
                        "Indicadores e análises da clínica";
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
