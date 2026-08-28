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
    public partial class frm_SplashScreen : Form
    {
        private Timer timer;
        public frm_SplashScreen()
        {
            InitializeComponent();
        }

        public void frm_SplashScreen_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = 1000; // tempo
            timer.Tick += Timer_Tick;
            // Adiciona o evento Tick ao timer, que será chamado a cada intervalo de tempo definido
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //this.Opacity -= 0.05; // Reduz a opacidade em 0,10 a cada tick
            timer.Interval += 1000;
            if (timer.Interval == 3000)
            {
                // caso o timer seja igual a 3s, para o timer e fecha o form splash screen
                timer.Stop();
                this.Hide();
                frm_Login menu = new frm_Login();
                menu.Show();

                this.Hide();


            }
        }
    }
}
