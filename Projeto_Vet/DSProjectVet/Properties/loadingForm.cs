using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_Agenda.Formularios
{
    public partial class loadingForm : Form
    {
        public loadingForm()
        {
            InitializeComponent();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            panelLoading.Left = (this.ClientSize.Width - panelLoading.Width) / 2;
            panelLoading.Top = (this.ClientSize.Height - panelLoading.Height) / 2;
        } // centraliza os componentes

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            OnResize(EventArgs.Empty);
        } // essa função faz o programa iniciar centralizado
    }
}