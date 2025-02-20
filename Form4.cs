using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProjetoLancheriaOficial
{
    public partial class Form4 : Form

    {
        //Conexao com o sql
        private MySqlConnection conexao;
        private string data_source =
        "datasource=localhost;username=root;password='';database=lanchonete";
        public Form4()
        {
            InitializeComponent();
            checkedListBox1.Items.Add("xis-casa");
            checkedListBox1.Items.Add("xis-salada");
            checkedListBox1.Items.Add("xis-frango");
            checkedListBox1.Items.Add("xis-coracao");
            checkedListBox1.Items.Add("xis-tudo");

            // Configuração inicial do ListView
            // lvContatos.
            listView1.GridLines = true;
            listView1.AllowColumnReorder = true;
            listView1.FullRowSelect = true;

            listView1.View = View.Details;
            listView1.Columns.Add("Produto", 150);
            listView1.Columns.Add("Máteria Prima", 150);
            listView1.Columns.Add("Unidade Medida", 100);
            listView1.Columns.Add("Qtd Máteria Prima", 100);
            listView1.Columns.Add("QtdNoEstoque", 100);
            listView1.Columns.Add("TempoDaEntrega", 100);
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            CarregarComboBox(checkedListBox2, "SELECT DISTINCT nome FROM materia_prima;");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            requisitar();
        }
        private void requisitar()
        {
            List<string> produtosSelecionados = new List<string>();
            List<string> ingredientesSelecionados = new List<string>();

            foreach (var item in checkedListBox1.CheckedItems)
            {
                produtosSelecionados.Add(item.ToString());
            }
            foreach (var item in checkedListBox2.CheckedItems)
            {
                ingredientesSelecionados.Add(item.ToString());
            }

            MessageBox.Show("Itens Selecionado!");

            AtualizarListView(produtosSelecionados);

        }

        private void AtualizarListView(List<string> produtos)
        {
            listView1.Items.Clear();

            try
            {
                using (conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conexao;

                        // Montando a query com múltiplos produtos selecionados
                        string query = "SELECT p.nome AS produto, mp.nome AS materia_prima, mp.unidade_medida, pi.quantidade " +
                                       "FROM produtos_ingredientes pi " +
                                       "JOIN produtos p ON pi.id_produto2 = p.id_produto " +
                                       "JOIN materia_prima mp ON pi.id_materiaPrima2 = mp.id_materiaPrima " +
                                       "WHERE p.nome IN (";

                        for (int i = 0; i < produtos.Count; i++)
                        {
                            string paramName = "@nome" + i;
                            query += (i == 0 ? paramName : ", " + paramName);
                            cmd.Parameters.AddWithValue(paramName, produtos[i]);
                        }
                        query += ");";

                        cmd.CommandText = query;

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["produto"].ToString());
                                item.SubItems.Add(reader["materia_prima"].ToString());
                                item.SubItems.Add(reader["unidade_medida"].ToString());
                                item.SubItems.Add(reader["quantidade"].ToString());

                                listView1.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao atualizar a lista: " + ex.Message);
            }

        }

        private void CarregarComboBox(CheckedListBox checkedListBox, string query)
        {
            try
            {
                using (conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                checkedListBox.Items.Add(reader["nome"].ToString());
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao carregar os dados: " + ex.Message);
            }
        }



    }
}