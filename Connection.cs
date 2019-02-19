using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Data;

namespace GestorSqlServerCE
{
    class Connection
    {
        private SqlCeConnection ligacao;
        private SqlCeCommand comando;
        private SqlCeDataAdapter adaptador;
        private string conn = "";
        private string path;

        public Connection(string path)
        {
            this.path = path;
        }

        public bool createDb(bool override_bd = false)
        {
            conn = "Data source = " + this.path;

            if (override_bd && File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }

            try
            {
                SqlCeEngine motor = new SqlCeEngine(conn);
                motor.CreateDatabase();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool createDb(string password, bool override_bd = false)
        {
            conn = "Data source = " + this.path + "; Password=" + password;
            
            if (override_bd && File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }

            try
            {
                SqlCeEngine motor = new SqlCeEngine(conn);
                motor.CreateDatabase();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool createTable(string table_name, params string[] columns)
        {
            bool criado = false;

            StringBuilder str = new StringBuilder();
            str.Append("CREATE TABLE " + table_name + " (");
            for (int i = 0; i < columns.Length; i++)
            {
                str.Append(columns[i]);
                if (i < columns.Length - 1)
                {
                    str.Append(", ");
                }
                else
                {
                    str.Append(")");
                }
            }

            try
            {
                ligacao = new SqlCeConnection(conn);
                ligacao.Open();
                comando = new SqlCeCommand();
                comando.Connection = ligacao;

                comando.CommandText = str.ToString();
                comando.ExecuteNonQuery();
                criado = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Erro: " + e.Message);
                criado = false;
            }
            finally
            {
                comando.Dispose();
                ligacao.Dispose();
            }

            return criado;
        }

        public bool dropTable(string table_name)
        {
            bool criado;
            try
            {
                ligacao = new SqlCeConnection(conn);
                ligacao.Open();
                comando = new SqlCeCommand();
                comando.Connection = ligacao;

                comando.CommandText = "DROP TABLE " + table_name;
                comando.ExecuteNonQuery();
                criado = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Erro: " + e.Message);
                criado = false;
            }
            finally
            {
                comando.Dispose();
                ligacao.Dispose();
            }
            return criado;
        }

        public bool insert(string query, params object[] values)
        {

            bool inserido = false;

            ligacao = new SqlCeConnection(conn);
            ligacao.Open();
            comando = new SqlCeCommand(query, ligacao);
            comando.Parameters.Clear();

            string[] names = formatQuery(query, values);

            if (names == null)
                return false;

            for (int i = 0; i < names.Length; i++)
            {
                comando.Parameters.AddWithValue(names[i], values[i]);
            }

            try
            {
                comando.ExecuteNonQuery();
                inserido = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERRO: " + ex.Message);
            }
            finally
            {
                comando.Dispose();
                ligacao.Dispose();
            }
            return inserido;
        }

        public int lastInsertedId(string table_name, string column)
        {
            int lastId = -1;

            string query = "SELECT MAX(" + column + ") AS MaxID FROM " + table_name;
            DataTable dados = new DataTable();

            adaptador = new SqlCeDataAdapter(query, conn);
            adaptador.Fill(dados);

            //verifica se é DBNull
            if (dados.Rows.Count != 0)
            {
                if (!DBNull.Value.Equals(dados.Rows[0][0]))
                    lastId = Convert.ToInt16(dados.Rows[0][0]);
            }
            return lastId;
        }

        public DataTable select(string query, params object[] clauses)
        {
            DataTable dados = new DataTable();
            adaptador = new SqlCeDataAdapter(query, conn);
            adaptador.SelectCommand.Parameters.Clear();

            try
            {
                if (clauses != null)
                {
                    string[] names = formatQuery(query, clauses);

                    if (names == null)
                        return null;

                    for (int i = 0; i < names.Length; i++)
                    {
                        adaptador.SelectCommand.Parameters.AddWithValue(names[i], clauses[i]);
                    }
                }
                adaptador.Fill(dados);
                dados = dados;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERRO: " + ex.Message);
                dados = null;
            }
            finally
            {
                adaptador.Dispose();
            }
            return dados;
        }
        //delete,update
        public int simpleQuery(string query, params object[] values)
        {
            int affected_rows = 0;
            ligacao = new SqlCeConnection(conn);
            ligacao.Open();
            comando = new SqlCeCommand(query, ligacao);

            if (values != null)
            {
                string[] names = formatQuery(query, values);

                comando.Parameters.Clear();

                if (names == null)
                    return affected_rows;

                for (int i = 0; i < names.Length; i++)
                {
                    comando.Parameters.AddWithValue(names[i], values[i]);
                }
            }
            try
            {
                affected_rows = comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERRO: " + ex.Message);
            }
            finally
            {
                comando.Dispose();
                ligacao.Dispose();
            }

            return affected_rows;
        }

        private string[] formatQuery(string query, params object[] values)
        {
            string[] names = null;
            int total = query.Split(new char[] { '?' }).Length - 1;

            
            if (total == 0 || total != values.Length)
            {
                MessageBox.Show("Quantidade inconsistente de parâmetros");
                return names;
            }

            names = new string[total];

            for (int i = 0; i < total; i++)
            {
                int ocurr = query.IndexOf('?');
                query = query.Remove(ocurr, 1);
                query = query.Insert(ocurr, "@" + i.ToString());

                names[i] = "@" + i.ToString();
            }
            
            return names;
        }

    }
}