namespace DSProjectVet.Forms.SCREENS
{
    partial class testeCadastro_Func
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
            this.lblLogin = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.txtSenha = new System.Windows.Forms.TextBox();
            this.lblSenha = new System.Windows.Forms.Label();
            this.btnCadastrar = new System.Windows.Forms.Button();
            this.lblHash = new System.Windows.Forms.Label();
            this.lblLoginGerado = new System.Windows.Forms.Label();
            this.txtLoginGerado = new System.Windows.Forms.TextBox();
            this.lblSenhaGerada = new System.Windows.Forms.Label();
            this.txtSenhaHash = new System.Windows.Forms.TextBox();
            this.btnTestar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(123, 60);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(33, 13);
            this.lblLogin.TabIndex = 0;
            this.lblLogin.Text = "Login";
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(126, 76);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(100, 20);
            this.txtLogin.TabIndex = 1;
            // 
            // txtSenha
            // 
            this.txtSenha.Location = new System.Drawing.Point(126, 165);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.Size = new System.Drawing.Size(100, 20);
            this.txtSenha.TabIndex = 3;
            // 
            // lblSenha
            // 
            this.lblSenha.AutoSize = true;
            this.lblSenha.Location = new System.Drawing.Point(123, 149);
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Size = new System.Drawing.Size(38, 13);
            this.lblSenha.TabIndex = 2;
            this.lblSenha.Text = "Senha";
            // 
            // btnCadastrar
            // 
            this.btnCadastrar.Location = new System.Drawing.Point(126, 234);
            this.btnCadastrar.Name = "btnCadastrar";
            this.btnCadastrar.Size = new System.Drawing.Size(75, 23);
            this.btnCadastrar.TabIndex = 4;
            this.btnCadastrar.Text = "Cadastrar";
            this.btnCadastrar.UseVisualStyleBackColor = true;
            this.btnCadastrar.Click += new System.EventHandler(this.btnCadastrar_Click);
            // 
            // lblHash
            // 
            this.lblHash.AutoSize = true;
            this.lblHash.Location = new System.Drawing.Point(126, 293);
            this.lblHash.Name = "lblHash";
            this.lblHash.Size = new System.Drawing.Size(0, 13);
            this.lblHash.TabIndex = 5;
            // 
            // lblLoginGerado
            // 
            this.lblLoginGerado.AutoSize = true;
            this.lblLoginGerado.Location = new System.Drawing.Point(413, 60);
            this.lblLoginGerado.Name = "lblLoginGerado";
            this.lblLoginGerado.Size = new System.Drawing.Size(33, 13);
            this.lblLoginGerado.TabIndex = 0;
            this.lblLoginGerado.Text = "Login";
            // 
            // txtLoginGerado
            // 
            this.txtLoginGerado.Location = new System.Drawing.Point(416, 76);
            this.txtLoginGerado.Name = "txtLoginGerado";
            this.txtLoginGerado.Size = new System.Drawing.Size(100, 20);
            this.txtLoginGerado.TabIndex = 1;
            // 
            // lblSenhaGerada
            // 
            this.lblSenhaGerada.AutoSize = true;
            this.lblSenhaGerada.Location = new System.Drawing.Point(413, 149);
            this.lblSenhaGerada.Name = "lblSenhaGerada";
            this.lblSenhaGerada.Size = new System.Drawing.Size(38, 13);
            this.lblSenhaGerada.TabIndex = 2;
            this.lblSenhaGerada.Text = "Senha";
            // 
            // txtSenhaHash
            // 
            this.txtSenhaHash.Location = new System.Drawing.Point(416, 165);
            this.txtSenhaHash.Name = "txtSenhaHash";
            this.txtSenhaHash.Size = new System.Drawing.Size(100, 20);
            this.txtSenhaHash.TabIndex = 3;
            // 
            // btnTestar
            // 
            this.btnTestar.Location = new System.Drawing.Point(416, 234);
            this.btnTestar.Name = "btnTestar";
            this.btnTestar.Size = new System.Drawing.Size(75, 23);
            this.btnTestar.TabIndex = 6;
            this.btnTestar.Text = "Testar";
            this.btnTestar.UseVisualStyleBackColor = true;
            this.btnTestar.Click += new System.EventHandler(this.btnTestar_Click);
            // 
            // testeCadastro_Func
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnTestar);
            this.Controls.Add(this.lblHash);
            this.Controls.Add(this.btnCadastrar);
            this.Controls.Add(this.txtSenhaHash);
            this.Controls.Add(this.txtSenha);
            this.Controls.Add(this.lblSenhaGerada);
            this.Controls.Add(this.lblSenha);
            this.Controls.Add(this.txtLoginGerado);
            this.Controls.Add(this.lblLoginGerado);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.lblLogin);
            this.Name = "testeCadastro_Func";
            this.Text = "testeCadastro_Func";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.TextBox txtSenha;
        private System.Windows.Forms.Label lblSenha;
        private System.Windows.Forms.Button btnCadastrar;
        private System.Windows.Forms.Label lblHash;
        private System.Windows.Forms.Label lblLoginGerado;
        private System.Windows.Forms.TextBox txtLoginGerado;
        private System.Windows.Forms.Label lblSenhaGerada;
        private System.Windows.Forms.TextBox txtSenhaHash;
        private System.Windows.Forms.Button btnTestar;
    }
}