using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DSProjectVet.Forms
{
    public partial class Perfil : Form
    {
        public Perfil()
        {
            InitializeComponent();
        }

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

        // Evento executado quando o programa COMPILA/ABRE
        private void Perfil_Load(object sender, EventArgs e)
        {
            lbl_Username.Text = DSProjectVet.Properties.Settings.Default.
                NomeUsuarioLogado;

            switch(DSProjectVet.Properties.Settings.Default.NivelUsuarioLogado)
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
                    lbl_cargo.Text = "Desconhecido";
                    break;
            }


            // Pega o último acesso salvo
            DateTime UltimaExecucao = Properties.Settings.Default.UltimaExecucao;

            // Exibe no Label
            lbl_lastAccess.Text = UltimaExecucao.ToString("dd/MM/yyyy HH:mm");

            // Atualiza para o acesso atual
            Properties.Settings.Default.UltimaExecucao = DateTime.Now;

            // Salva a alteração
            Properties.Settings.Default.Save();

            lbl_desktop.Text = Environment.MachineName;
        }

        private void lbl_desktop_Click(object sender, EventArgs e)
        {

        }
    }
}
