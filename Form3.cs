using System;
using System.Windows.Forms;

namespace ProjetoLancheriaOficial
{
    public partial class Form3 : Form
    {
       
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
        }

        private void btnRequisicao_Click(object sender, EventArgs e)
        {
            Form1 Form1 = new Form1();
            this.Visible = false;
            Form1.ShowDialog();
        }

        private void btnEstoque_Click(object sender, EventArgs e)
        {
            Form4 Form4 = new Form4();
            this.Visible = false;
            Form4.ShowDialog();
        }

        private void btnCadastro_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            this.Visible = false;
            form6.ShowDialog();
        }
    }
}
