using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace doomsday_scut
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void new_game_Click(object sender, EventArgs e)
        {
            Game g = new Game();
            g.Show();
            g.Draw();
            this.Hide();
        }

        private void help_about_Click(object sender, EventArgs e)
        {

        }

        private void exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit?", "Exit", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Dispose();
                Application.Exit();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit?", "Exit", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Dispose();
                Application.Exit();
            }
        }
    }
}
