using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoLancheriaOficial
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            this.Visible = false;
            form5.ShowDialog();
        }

        private void btnCadastro_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            this.Visible = false;
            form7.ShowDialog();
        }

        private void Form6_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            this.Visible = false;
            form3.ShowDialog();
        }
    }
}
