using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

namespace ProjetoLancheriaOficial
{
    public partial class Form5 : Form
    {
        private MySqlConnection conexao;
        private string data_source =
        "datasource=localhost;username=root;password='';database=lanchonete";
        public Form5()
        {
            InitializeComponent();

            comboBox1.Items.Add("produtos");
            comboBox1.Items.Add("matéria-prima");

            dataGridView1.Columns.Add("nome", "Nome");
            dataGridView1.Columns.Add("descricao", "Descricao");
            dataGridView1.Columns.Add("preco", "Preço");

            




        }

        private void button1_Click(object sender, EventArgs e)
        {
            string busca = textBox1.Text;
            dataGridView1.Rows.Clear();

            try
            {
                using (conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();

                    if (busca == "")
                    {
                        string query = "SELECT nome, descricao, preco FROM produtos " +
                       "UNION " +
                       "SELECT nome, NULL AS descricao, NULL AS preco FROM materia_prima;";
                        using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader["nome"].ToString(),
                                                           reader["descricao"].ToString(),
                                                           reader["preco"].ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        string query = "SELECT nome, descricao, preco FROM produtos WHERE nome LIKE CONCAT('%', @busca, '%')" +
                       "UNION " +
                       "SELECT nome, NULL AS descricao, NULL AS preco FROM materia_prima WHERE nome LIKE CONCAT('%', @busca, '%')";
                        using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                        {
                            cmd.Parameters.AddWithValue("@busca", busca);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(reader["nome"].ToString(),
                                                           reader["descricao"].ToString(),
                                                           reader["preco"].ToString());
                                }
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

        private void button3_Click(object sender, EventArgs e)
        {

            string nomeProduto = textBox3.Text;
            string descricaoProduto = textBox2.Text;
            string precoProduto = textBox4.Text;
            string categoria = comboBox1.SelectedItem?.ToString();

            try
            {
                using (conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();

                    if (categoria == "produtos")
                    {
                        string query = "INSERT INTO produtos (nome, descricao, preco) " +
                            "VALUES (@nome, @descricao, @preco);";

                        using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                        {
                            cmd.Parameters.AddWithValue("@nome", nomeProduto);
                            cmd.Parameters.AddWithValue("@descricao", descricaoProduto);
                            cmd.Parameters.AddWithValue("@preco", precoProduto);

                            int rows = cmd.ExecuteNonQuery();
                            limpaCampos();

                            if (rows > 0)
                            {
                                MessageBox.Show("Produto cadastrado com sucesso!");
                            }
                            else
                            {
                                MessageBox.Show("Erro ao cadastrar produto!");
                            }



                        }
                    }
                    else
                    {
                        string query = "INSERT INTO materia_prima (nome) " +
                            "VALUES (@nome);";

                        using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                        {
                            cmd.Parameters.AddWithValue("@nome", nomeProduto);

                            int rows = cmd.ExecuteNonQuery();
                            limpaCampos();

                            if (rows > 0)
                            {
                                MessageBox.Show("Item cadastrado com sucesso!");
                            }
                            else
                            {
                                MessageBox.Show("Erro ao cadastrar Item!");
                            }

                        }



                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao cadastrar os dados: " + ex.Message);
            }

        }
        private void limpaCampos()
        {
            textBox3.Clear();
            textBox2.Clear();
            textBox4.Clear();
            comboBox1.SelectedIndex = -1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um item para alterar.");
                return;
            }

            string nomeProduto = textBox3.Text.Trim();
            string descricaoProduto = textBox2.Text.Trim();
            string precoProduto = textBox4.Text.Trim();
            string categoria = comboBox1.SelectedItem?.ToString()?.ToLower();

            if (string.IsNullOrEmpty(nomeProduto) || string.IsNullOrEmpty(descricaoProduto) || string.IsNullOrEmpty(precoProduto))
            {
                MessageBox.Show("Preencha todos os campos corretamente.");
                return;
            }

            string descricaoOriginal = dataGridView1.SelectedRows[0].Cells["descricao"].Value.ToString();

            try
            {
                using (var conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();
                    string query = "";
                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conexao;

                        if (categoria == "produtos")
                        {
                            query = "UPDATE produtos SET nome = @nome, descricao = @descricao, preco = @preco WHERE descricao = @descricaoOriginal;";
                            cmd.Parameters.AddWithValue("@descricao", descricaoProduto);
                            cmd.Parameters.AddWithValue("@preco", precoProduto);
                        }
                        else if (categoria == "matéria-prima")
                        {
                            query = "UPDATE materia_prima SET nome = @nome WHERE nome = @descricaoOriginal;";
                        }
                        else
                        {
                            MessageBox.Show("Categoria inválida. Selecione 'produtos' ou 'materia-prima'.");
                            return;
                        }

                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@descricaoOriginal", descricaoOriginal);
                        cmd.Parameters.AddWithValue("@nome", nomeProduto);

                        int rows = cmd.ExecuteNonQuery();
                        limpaCampos();

                        if (rows > 0)
                        {
                            MessageBox.Show("Registro alterado com sucesso!");
                        }
                        else
                        {
                            MessageBox.Show("Erro ao alterar o registro!");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao alterar os dados: " + ex.Message);
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica se a linha clicada é válida
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex]; // Pega a linha selecionada

                // Preenche os TextBox com os valores da linha
                textBox3.Text = row.Cells["nome"].Value?.ToString() ?? "";
                textBox2.Text = row.Cells["descricao"].Value?.ToString() ?? "";
                textBox4.Text = row.Cells["preco"].Value?.ToString() ?? "";
            }
        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um item para excluir.");
                return;
            }

            SolicitarCategoria();
        }

        private void SolicitarCategoria()
        {
            string mensagem = "Selecione a categoria:\nSim - Produto\nNão - Matéria-prima";
            string titulo = "Escolha de Categoria";
            DialogResult resultado = MessageBox.Show(mensagem, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                ExecutarAcao("produto");
            }
            else if (resultado == DialogResult.No)
            {
                ExecutarAcao("materia-prima");
            }
            else
            {
                MessageBox.Show("Nenhuma categoria selecionada. Operação cancelada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ExecutarAcao(string categoria)
        {
            DialogResult confirm = MessageBox.Show($"Deseja excluir o item/produto?",
                                                   "Confirmação",
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (var conexao = new MySqlConnection(data_source))
                    {
                        conexao.Open();
                        string query = "";
                        using (var cmd = new MySqlCommand(query, conexao))
                        {
                            cmd.Connection = conexao;

                            string descricaoOriginal = dataGridView1.SelectedRows[0].Cells["descricao"].Value.ToString();
                            string nomeOriginal = dataGridView1.SelectedRows[0].Cells["nome"].Value.ToString();

                            if (categoria == "produto")
                            {
                                query = "DELETE FROM produtos WHERE descricao = @descricaoOriginal;";
                            }
                            else if (categoria == "materia-prima")
                            {
                                query = "DELETE FROM materia_prima WHERE nome = @nomeOriginal;";
                            }
                            else
                            {
                                MessageBox.Show("Categoria inválida. Selecione 'produtos' ou 'materia-prima'.");
                                return;
                            }

                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@descricaoOriginal", descricaoOriginal);

                            int rows = cmd.ExecuteNonQuery();

                            if (rows > 0)
                            {
                                MessageBox.Show("Item excluído com sucesso!");
                            }
                            else
                            {
                                MessageBox.Show("Erro ao excluir o item!");
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Erro ao excluir os dados: " + ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6();
            this.Visible = false;
            form6.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
