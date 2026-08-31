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
    public partial class MDI_Menu : Form
    {
        public MDI_Menu()
        {
            InitializeComponent();
        }

        void MDI_Warning_Message()
        {
            MessageBox.Show("Uma janela já foi aberta. Feche-a para abrir uma nova!",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void MDI_Menu_Load(object sender, EventArgs e)
        {
            if (DSProjectVet.Properties.Settings.Default.NivelUsuarioLogado == 2)
            {
                // caso o nivel de acesso seja "2", muda a visibilidade do botão de cadastrar cliente
                clientesToolStripMenuItem.Visible = false;
            }
        }

        private bool FormAberto(Type tipoForm)
        {
            foreach (Form form in MdiChildren)
            {
                if (form.GetType() == tipoForm) //tipoForm retorna qual instância atual
                {
                    
                    form.Focus(); // traz para frente
                    return true; // retorna true caso já exita um form do mesmo type aberto. 
                }
            }
            return false;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void perfilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!FormAberto(typeof(Perfil))) // executa apenas se MdiChildren estiver vazio.
            {
                Perfil frmCalculos = new Perfil();
                frmCalculos.MdiParent = this; // indica se essa nova instancia é parente de MDI
                                              //this indica o form MDI atual

                // new frmPerfilUsuario() cria/instância uma nova janela da calculadora


                frmCalculos.Show();
                // ShowDialog() impede de clicar na tela de fundo enquando frmPerfilUsuario estiver aberto
            }
            else
            {
                MDI_Warning_Message();
            }
        }
    }
}
