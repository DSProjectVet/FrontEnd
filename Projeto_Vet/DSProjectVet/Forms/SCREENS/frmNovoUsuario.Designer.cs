namespace DSProjectVet.Forms.SCREENS
{
    partial class frmNovoUsuario
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNovoUsuario));
            this.btnCadastrar = new System.Windows.Forms.Button();
            this.txtSenha = new System.Windows.Forms.TextBox();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.lblNivel = new System.Windows.Forms.Label();
            this.lblSenha = new System.Windows.Forms.Label();
            this.lblLogin = new System.Windows.Forms.Label();
            this.lblNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.CmbNivel = new System.Windows.Forms.ComboBox();
            this.txtCrmv = new System.Windows.Forms.TextBox();
            this.lblCrmv = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCadastrar
            // 
            this.btnCadastrar.Location = new System.Drawing.Point(93, 395);
            this.btnCadastrar.Margin = new System.Windows.Forms.Padding(2);
            this.btnCadastrar.Name = "btnCadastrar";
            this.btnCadastrar.Size = new System.Drawing.Size(118, 39);
            this.btnCadastrar.TabIndex = 28;
            this.btnCadastrar.Text = "Cadastrar";
            this.btnCadastrar.UseVisualStyleBackColor = true;
            this.btnCadastrar.UseWaitCursor = true;
            this.btnCadastrar.Click += new System.EventHandler(this.btnCadastrar_Click);
            // 
            // txtSenha
            // 
            this.txtSenha.BackColor = System.Drawing.SystemColors.Window;
            this.txtSenha.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSenha.Location = new System.Drawing.Point(37, 203);
            this.txtSenha.Margin = new System.Windows.Forms.Padding(2);
            this.txtSenha.MaxLength = 15;
            this.txtSenha.Multiline = true;
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.PasswordChar = '*';
            this.txtSenha.Size = new System.Drawing.Size(256, 30);
            this.txtSenha.TabIndex = 26;
            this.txtSenha.UseWaitCursor = true;
            // 
            // txtLogin
            // 
            this.txtLogin.BackColor = System.Drawing.SystemColors.Window;
            this.txtLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogin.Location = new System.Drawing.Point(37, 131);
            this.txtLogin.Margin = new System.Windows.Forms.Padding(2);
            this.txtLogin.MaxLength = 15;
            this.txtLogin.Multiline = true;
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(256, 30);
            this.txtLogin.TabIndex = 25;
            this.txtLogin.UseWaitCursor = true;
            // 
            // lblNivel
            // 
            this.lblNivel.AutoSize = true;
            this.lblNivel.Font = new System.Drawing.Font("Microsoft Tai Le", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNivel.Location = new System.Drawing.Point(41, 322);
            this.lblNivel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNivel.Name = "lblNivel";
            this.lblNivel.Size = new System.Drawing.Size(47, 18);
            this.lblNivel.TabIndex = 23;
            this.lblNivel.Text = "Nível:";
            this.lblNivel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblNivel.UseWaitCursor = true;
            // 
            // lblSenha
            // 
            this.lblSenha.AutoSize = true;
            this.lblSenha.Font = new System.Drawing.Font("Microsoft Tai Le", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSenha.Location = new System.Drawing.Point(37, 177);
            this.lblSenha.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Size = new System.Drawing.Size(52, 18);
            this.lblSenha.TabIndex = 22;
            this.lblSenha.Text = "Senha:";
            this.lblSenha.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblSenha.UseWaitCursor = true;
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Font = new System.Drawing.Font("Microsoft Tai Le", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogin.Location = new System.Drawing.Point(34, 107);
            this.lblLogin.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(46, 18);
            this.lblLogin.TabIndex = 21;
            this.lblLogin.Text = "login:";
            this.lblLogin.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblLogin.UseWaitCursor = true;
            // 
            // lblNome
            // 
            this.lblNome.AutoSize = true;
            this.lblNome.Font = new System.Drawing.Font("Microsoft Tai Le", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNome.Location = new System.Drawing.Point(34, 32);
            this.lblNome.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(53, 18);
            this.lblNome.TabIndex = 20;
            this.lblNome.Text = "Nome:";
            this.lblNome.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblNome.UseWaitCursor = true;
            // 
            // txtNome
            // 
            this.txtNome.BackColor = System.Drawing.SystemColors.Window;
            this.txtNome.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNome.Location = new System.Drawing.Point(37, 59);
            this.txtNome.Margin = new System.Windows.Forms.Padding(2);
            this.txtNome.MaxLength = 15;
            this.txtNome.Multiline = true;
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(256, 30);
            this.txtNome.TabIndex = 24;
            this.txtNome.UseWaitCursor = true;
            // 
            // CmbNivel
            // 
            this.CmbNivel.FormattingEnabled = true;
            this.CmbNivel.Items.AddRange(new object[] {
            "1;",
            "2;",
            "3;"});
            this.CmbNivel.Location = new System.Drawing.Point(42, 352);
            this.CmbNivel.Margin = new System.Windows.Forms.Padding(2);
            this.CmbNivel.Name = "CmbNivel";
            this.CmbNivel.Size = new System.Drawing.Size(251, 21);
            this.CmbNivel.TabIndex = 29;
            this.CmbNivel.UseWaitCursor = true;
            // 
            // txtCrmv
            // 
            this.txtCrmv.BackColor = System.Drawing.SystemColors.Window;
            this.txtCrmv.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCrmv.Location = new System.Drawing.Point(40, 276);
            this.txtCrmv.Margin = new System.Windows.Forms.Padding(2);
            this.txtCrmv.MaxLength = 15;
            this.txtCrmv.Multiline = true;
            this.txtCrmv.Name = "txtCrmv";
            this.txtCrmv.Size = new System.Drawing.Size(256, 30);
            this.txtCrmv.TabIndex = 30;
            this.txtCrmv.UseWaitCursor = true;
            // 
            // lblCrmv
            // 
            this.lblCrmv.AutoSize = true;
            this.lblCrmv.Font = new System.Drawing.Font("Microsoft Tai Le", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCrmv.Location = new System.Drawing.Point(41, 252);
            this.lblCrmv.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCrmv.Name = "lblCrmv";
            this.lblCrmv.Size = new System.Drawing.Size(52, 18);
            this.lblCrmv.TabIndex = 31;
            this.lblCrmv.Text = "CRMV:";
            this.lblCrmv.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblCrmv.UseWaitCursor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // frmNovoUsuario
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 465);
            this.Controls.Add(this.lblCrmv);
            this.Controls.Add(this.txtCrmv);
            this.Controls.Add(this.CmbNivel);
            this.Controls.Add(this.btnCadastrar);
            this.Controls.Add(this.txtSenha);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.lblNivel);
            this.Controls.Add(this.lblSenha);
            this.Controls.Add(this.lblLogin);
            this.Controls.Add(this.lblNome);
            this.Controls.Add(this.txtNome);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmNovoUsuario";
            this.Text = "Novo Usuário";
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.frmNovoUsuario_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCadastrar;
        private System.Windows.Forms.TextBox txtSenha;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Label lblNivel;
        private System.Windows.Forms.Label lblSenha;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.ComboBox CmbNivel;
        private System.Windows.Forms.TextBox txtCrmv;
        private System.Windows.Forms.Label lblCrmv;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}