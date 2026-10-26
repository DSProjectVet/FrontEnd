using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSProjectVet.Forms.SCREENS
{
    using BCrypt.Net;
    public partial class testeCadastro_Func : Form
    {
        public testeCadastro_Func()
        {
            InitializeComponent();
        }

        string hash;

        private string GerarHash(string senha)
        {
            return BCrypt.HashPassword(senha);
        }

        private bool VerificarSenha(string senhaDigitada)
        {
            return BCrypt.Verify(senhaDigitada, hash);
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            hash = GerarHash(txtSenha.Text);
            lblHash.Text = $"Hash gerado: {hash}";
            txtLogin.Text = hash;
        }

        private void btnTestar_Click(object sender, EventArgs e)
        {
            bool hashSalvo = VerificarSenha(txtSenhaHash.Text);

            if (hashSalvo)
            {
                MessageBox.Show("Senha correta!");
            } else
            {
                MessageBox.Show("Senha incorreta!");
            }
        }
    }
}
