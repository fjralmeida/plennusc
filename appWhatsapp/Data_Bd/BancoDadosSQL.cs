using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

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

        // ==== Métodos de Acesso ====

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
    }
}
