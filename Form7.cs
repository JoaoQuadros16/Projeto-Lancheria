using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProjetoLancheriaOficial
{
    public partial class Form7 : Form
    {
        private MySqlConnection conexao;
        private string data_source =
        "datasource=localhost;username=root;password='';database=lanchonete";
        public Form7()
        {
            InitializeComponent();
            dataGridView1.Columns.Clear();
            dataGridView1.ReadOnly = false;  // Permitir edição
            dataGridView1.AllowUserToAddRows = false; // Evitar linhas vazias



            dataGridView1.Columns.Add("id_produto", "id_produto");
            dataGridView1.Columns.Add("id_materiaPrima", "id_materiaPrima");
            dataGridView1.Columns.Add("produto", "Produto");
            dataGridView1.Columns.Add("materia_prima", "Matéria-Prima");
            dataGridView1.Columns.Add("unidade_medida", "Unidade de Medida");
            dataGridView1.Columns.Add("quantidade", "Quantidade");
            


        }

        private void Form7_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
          
           CarregarDados();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            this.Visible = false;
            form6.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // Verifica se a linha não é nova e se os campos obrigatórios não estão vazios
                        if (row.IsNewRow) continue;

                        var idProduto = row.Cells["id_produto2"].Value;
                        var idMateriaPrima = row.Cells["id_materiaPrima2"].Value;
                        var quantidade = row.Cells["quantidade"].Value;

                        // Exibe os valores capturados para depuração
                        MessageBox.Show($"Produto: {idProduto}, Matéria-Prima: {idMateriaPrima}, Quantidade: {quantidade}");

                        if (idProduto != null && idMateriaPrima != null && quantidade != null)
                        {
                            string query = "UPDATE produtos_ingredientes SET quantidade=@quantidade " +
                                           "WHERE id_produto2=@id_produto2 AND id_materiaPrima2=@id_materiaPrima2";

                            using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                            {
                                cmd.Parameters.AddWithValue("@quantidade", quantidade);
                                cmd.Parameters.AddWithValue("@id_produto2", idProduto);
                                cmd.Parameters.AddWithValue("@id_materiaPrima2", idMateriaPrima);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    MessageBox.Show("Dados atualizados com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarDados(); // Atualiza o DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar os dados: " + ex.Message);
            }
        }



        private void CarregarDados()
        {
            //buscar
            string busca = textBox1.Text;

            dataGridView1.Rows.Clear(); // limpa o datagrid 

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(data_source)) // conexao com o banco
                {
                    conexao.Open();

                    string query;

                    if (string.IsNullOrWhiteSpace(busca)) // Verifica se a busca está vazia
                    {
                        query = "SELECT p.id_produto AS id_produto2, mp.id_materiaPrima AS id_materiaPrima2 , p.nome AS produto, mp.nome AS materia_prima, mp.unidade_medida, pi.quantidade " +
                                "FROM produtos_ingredientes pi " +
                                "JOIN produtos p ON pi.id_produto2 = p.id_produto " +
                                "JOIN materia_prima mp ON pi.id_materiaPrima2 = mp.id_materiaPrima";
                    }
                    else
                    {
                        query = "SELECT p.id_produto AS id_produto2,mp.id_materiaPrima AS id_materiaPrima2,p.nome AS produto, mp.nome AS materia_prima, mp.unidade_medida, pi.quantidade " +
                                "FROM produtos_ingredientes pi " +
                                "JOIN produtos p ON pi.id_produto2 = p.id_produto " +
                                "JOIN materia_prima mp ON pi.id_materiaPrima2 = mp.id_materiaPrima " +
                                "WHERE p.nome LIKE CONCAT('%', @busca, '%') OR mp.nome LIKE CONCAT('%', @busca, '%')";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                    {
                        if (!string.IsNullOrWhiteSpace(busca))
                        {
                            cmd.Parameters.AddWithValue("@busca", busca);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dataGridView1.Rows.Add(reader["id_produto2"].ToString(),
                                                       reader["id_materiaPrima2"].ToString(),
                                                       reader["produto"].ToString(),
                                                       reader["materia_prima"].ToString(),
                                                       reader["unidade_medida"].ToString(),
                                                       reader["quantidade"].ToString());
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
