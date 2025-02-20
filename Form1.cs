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
    public partial class Form1 : Form
    {
        // Conexão com o MySQL
        private MySqlConnection conexao;
        private string data_source = "datasource=localhost;username=root;password='';database=lanchonete";

        public Form1()
        {
            InitializeComponent();

            dataGridView1.Columns.Clear();
            dataGridView1.ReadOnly = true;  // Impedir edição
            dataGridView1.AllowUserToAddRows = false; // Evitar linhas vazias

            dataGridView1.Columns.Add("id", "ID");
            dataGridView1.Columns.Add("Nome", "Nome");
            dataGridView1.Columns.Add("unidadeMedida", "Unidade de Medida");
            dataGridView1.Columns.Add("Quantidade", "Quantidade");
            dataGridView1.Columns.Add("UnidadeTempo", "Unidade de Tempo");
            dataGridView1.Columns.Add("TempoEntrega", "Tempo de Entrega");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CarregarComboBox(checkedListBox2, "SELECT DISTINCT nome FROM materia_prima;");
            CarregarComboBox(comboBox1, "SELECT DISTINCT nome FROM almoxarifados;");
        }

        private void CarregarComboBox(CheckedListBox checkedListBox, string query)
        {
            try
            {
                using (conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            checkedListBox.Items.Add(reader["nome"].ToString());
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao carregar os dados: " + ex.Message);
            }
        }

        private void CarregarComboBox(ComboBox comboBox, string query)
        {
            try
            {
                using (conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox.Items.Add(reader["nome"].ToString());
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao carregar os dados: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Lista de ingredientes selecionados
            List<string> ingredientesSelecionados = new List<string>();

            foreach (var item in checkedListBox2.CheckedItems)
            {
                ingredientesSelecionados.Add(item.ToString());
            }

            if (ingredientesSelecionados.Count == 0)
            {
                MessageBox.Show("Nenhum ingrediente selecionado!");
                return;
            }

            int quant = (int)numericUpDown1.Value; // Quantidade a ser reduzida

            try
            {
                using (var conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();

                    foreach (string ingrediente in ingredientesSelecionados)
                    {
                        int quantidadeFinal = 0;
                        string unidadeMedida = "";
                        int quantidadeInicial = 0;
                        int indiceSelecionado = comboBox1.SelectedIndex + 1;
                        string querySelect = "SELECT Quantidade, unidadeMedida FROM estoquecentral WHERE nome = @nome";
                        using (var cmdSelect = new MySqlCommand(querySelect, conexao))
                        {
                            cmdSelect.Parameters.AddWithValue("@nome", ingrediente);
                            using (var reader = cmdSelect.ExecuteReader())
                            {
                                if (reader.Read()) // Verifica se há resultados
                                {
                                    quantidadeInicial = reader.GetInt32("Quantidade");
                                    unidadeMedida = reader.GetString("unidadeMedida");

                                    quantidadeFinal = quantidadeInicial - quant;

                                    if (quantidadeFinal < 0)
                                    {
                                        MessageBox.Show($"Estoque insuficiente para '{ingrediente}'!");
                                        continue;
                                    }

                                    // Aqui você pode continuar com o processamento
                                    Console.WriteLine($"Ingrediente: {ingrediente}, Quantidade: {quantidadeInicial}, Unidade: {unidadeMedida}");
                                }
                                else
                                {
                                    MessageBox.Show($"Ingrediente '{ingrediente}' não encontrado no estoque.");
                                }
                            }
                            string queryUpdate = "UPDATE estoquecentral SET Quantidade = @quantidadeFinal WHERE nome = @nome";
                            
                            using (var cmdUpdate = new MySqlCommand(queryUpdate, conexao))
                            {
                                cmdUpdate.Parameters.AddWithValue("@nome", ingrediente);
                                cmdUpdate.Parameters.AddWithValue("@quantidadeFinal", quantidadeFinal);

                                int linhasAfetadas = cmdUpdate.ExecuteNonQuery();

                                if (linhasAfetadas > 0)
                                {
                                    MessageBox.Show($"Estoque de '{ingrediente}' atualizado para {quantidadeFinal}.");
                                }
                                else
                                {
                                    MessageBox.Show($"Erro ao atualizar o estoque de '{ingrediente}'.");
                                }
                            }

                            string queryInsert = "INSERT INTO estoque_almoxarifados (id_almoxarifados, nome, unidadeMedida, Quantidade) VALUES (@id_almoxarifado, @nome, @unidade, @quantidadeFinal);";
                            using (var cmdInsert = new MySqlCommand(queryInsert, conexao))
                            {
                                cmdInsert.Parameters.AddWithValue("@nome", ingrediente);
                                cmdInsert.Parameters.AddWithValue("@quantidadeFinal", quantidadeInicial - quantidadeFinal);
                                cmdInsert.Parameters.AddWithValue("@unidade", unidadeMedida);
                                cmdInsert.Parameters.AddWithValue("@id_almoxarifado", indiceSelecionado);

                                int linhasAfetadas = cmdInsert.ExecuteNonQuery();

                                if (linhasAfetadas > 0 )
                                {
                                    MessageBox.Show("Estoque atualizado");
                                }
                                else
                                {
                                    MessageBox.Show("Erro ao atualizar estoque");
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao alterar os dados: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            this.Visible = false;
            form2.ShowDialog();
        }

        private void CarregarDados()
        {
            string busca = textBox1.Text;

            dataGridView1.Rows.Clear(); // Limpa o DataGridView

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(data_source))
                {
                    conexao.Open();

                    string query = string.IsNullOrWhiteSpace(busca)
                        ? "SELECT * FROM estoquecentral;"
                        : "SELECT * FROM estoquecentral WHERE nome LIKE CONCAT('%', @busca, '%');";

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
                                dataGridView1.Rows.Add(reader["id"].ToString(),
                                                       reader["Nome"].ToString(),
                                                       reader["unidadeMedida"].ToString(),
                                                       reader["Quantidade"].ToString(),
                                                       reader["unidadeTempo"].ToString(),
                                                       reader["TempoEntrega"].ToString());
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
            CarregarDados();
        }
    }
}
