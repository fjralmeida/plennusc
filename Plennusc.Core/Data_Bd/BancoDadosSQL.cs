using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
namespace appWhatsapp.Data_Bd
{
    class Banco_Dados_SQLServer
    {
        private SqlConnection conAlianca;
        private SqlConnection conPlennus;

        private void ConectarAlianca()
        {
            if (conAlianca == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;
                conAlianca = new SqlConnection(connectionString);
            }

            if (conAlianca.State != ConnectionState.Open)
                conAlianca.Open();
        }

        private void ConectarPlennus()
        {
            if (conPlennus == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Plennus"].ConnectionString;
                conPlennus = new SqlConnection(connectionString);
            }

            if (conPlennus.State != ConnectionState.Open)
                conPlennus.Open();
        }

        private void DesconectarAlianca()
        {
            if (conAlianca != null && conAlianca.State == ConnectionState.Open)
                conAlianca.Close();
        }

        private void DesconectarPlennus()
        {
            if (conPlennus != null && conPlennus.State == ConnectionState.Open)
                conPlennus.Close();
        }

        // ==== Métodos de Acesso Simples ====

        public DataTable LerAlianca(string cmdsql)
        {
            ConectarAlianca();
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(cmdsql, conAlianca))
            {
                da.Fill(dt);
            }
            DesconectarAlianca();
            return dt;
        }

        public void ExecutarAlianca(string cmdsql)
        {
            ConectarAlianca();
            using (SqlCommand comando = new SqlCommand(cmdsql, conAlianca))
            {
                comando.ExecuteNonQuery();
            }
            DesconectarAlianca();
        }

        public DataTable LerPlennus(string cmdsql)
        {
            ConectarPlennus();
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(cmdsql, conPlennus))
            {
                da.Fill(dt);
            }
            DesconectarPlennus();
            return dt;
        }

        public void ExecutarPlennus(string cmdsql)
        {
            ConectarPlennus();
            using (SqlCommand comando = new SqlCommand(cmdsql, conPlennus))
            {
                comando.ExecuteNonQuery();
            }
            DesconectarPlennus();
        }

        // ==== Métodos de Acesso com Parâmetros ====

        public DataTable LerAlianca(string sql, Dictionary<string, object> parametros)
        {
            ConectarAlianca();
            DataTable dt = new DataTable();

            using (SqlCommand cmd = new SqlCommand(sql, conAlianca))
            {
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            DesconectarAlianca();
            return dt;
        }

        public void ExecutarAlianca(string sql, Dictionary<string, object> parametros)
        {
            ConectarAlianca();

            using (SqlCommand cmd = new SqlCommand(sql, conAlianca))
            {
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                cmd.ExecuteNonQuery();
            }

            DesconectarAlianca();
        }

        public int ExecutarAliancaLinhasAfetadas(string sql, Dictionary<string, object> parametros)
        {
            ConectarAlianca();

            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, conAlianca))
                {
                    foreach (var p in parametros)
                        cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);

                    return cmd.ExecuteNonQuery(); // ← retorna 0 ou 1+
                }
            }
            finally
            {
                DesconectarAlianca();
            }
        }


        public DataTable LerPlennus(string sql, Dictionary<string, object> parametros)
        {
            ConectarPlennus();
            DataTable dt = new DataTable();

            using (SqlCommand cmd = new SqlCommand(sql, conPlennus))
            {
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            DesconectarPlennus();
            return dt;
        }

        public void ExecutarPlennus(string sql, Dictionary<string, object> parametros)
        {
            ConectarPlennus();

            using (SqlCommand cmd = new SqlCommand(sql, conPlennus))
            {
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                cmd.ExecuteNonQuery();
            }

            DesconectarPlennus();
        }
        public async Task ExecutarPlennusAsync(string sql, Dictionary<string, object> parametros)
        {
            ConectarPlennus();

            using (SqlCommand cmd = new SqlCommand(sql, conPlennus))
            {
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                await cmd.ExecuteNonQueryAsync();
            }

            DesconectarPlennus();
        }
    }
}
