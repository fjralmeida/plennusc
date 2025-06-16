using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace bdmysql
{
    public class Banco
    {

        #region ConexaoODBC
        //MySqlConnection con = new MySqlConnection();
        ///// <summary>
        ///// esse método é usado para criar conexão
        ///// </summary>
        ///// <returns>  retorna uma conexao aberta</returns>
        //public MySqlConnection abreconexao()
        //{
        //    string scon = ConfigurationSettings.AppSettings["conexao"];

        //    con.ConnectionString = scon;
        //    con.Open();
        //    return con;
        //}

        //public void atualiza(string sql)
        //{
        //    while (con.State != ConnectionState.Open)
        //    {

        //        abreconexao();
        //        //comand.ExecuteNonQuery();
        //        //con.Close();
        //        //con.Dispose();

        //    }
        //    if (con.State == ConnectionState.Open)
        //    {
        //        MySqlCommand comand = new MySqlCommand();
        //        comand.CommandText = sql;
        //        comand.Connection = con;
        //        comand.ExecuteNonQuery();
        //        con.Close();
        //        con.Dispose();
        //    }
        //}

        //public DataTable consulta(string sql)
        //{

        //    DataTable dt = new DataTable();
        //    MySqlDataAdapter da = new MySqlDataAdapter(sql, abreconexao());

        //    while (con.State != ConnectionState.Open)
        //    {

        //        abreconexao();
        //    }
        //    if (con.State == ConnectionState.Open)
        //    {
        //        //MySqlDataAdapter da = new MySqlDataAdapter(sql, abreconexao());
        //        //MySqlDataAdapter da = new MySqlDataAdapter(sql, con);
        //        da.Fill(dt);
        //        con.Close();
        //        con.Dispose();


        //    }

        //    return dt;


        //}

        //public OdbcDataReader consultadr(string sql)
        //{

        //    while (con.State != ConnectionState.Open)
        //    {

        //        abreconexao();
        //        //comand.ExecuteNonQuery();
        //        //con.Close();
        //        //con.Dispose();

        //    }


        //    MySqlCommand comand = new MySqlCommand();


        //    comand.CommandText = sql;
        //    comand.Connection = con;

        //    return comand.ExecuteReader();


        //    //O DATA READER É FECHADO NA PROPRIA APLICAÇÃO

        //}

        //public  MySqlTransaction IniciarTransacao(MySqlConnection con)
        //{
        //     MySqlTransaction trans = con.BeginTransaction();

        //    return trans;
        //}

        //public void ConfirmarTransacao( MySqlTransaction transatual)
        //{
        //    transatual.Commit();
        //    con.Close();
        //}

        //public void CancelarTransacao( MySqlTransaction transatual)
        //{
        //    transatual.Rollback();
        //    transatual.Dispose();
        //    con.Close();
        //}

        #endregion

        #region smartben
        MySqlConnection con = new MySqlConnection();
        public MySqlConnection abreconexao()
        {
            string scon = System.Configuration.ConfigurationSettings.AppSettings["conexao"];

            con.ConnectionString = scon;
            con.Open();
            return con;
        }

        //public void atualiza(string sql)
        //{

        //    MySqlCommand comand = new MySqlCommand();
        //    comand.CommandText = sql;
        //    comand.Connection = abreconexao();
        //    comand.ExecuteNonQuery();
        //    con.Close();
        //    con.Dispose();

        //}

        public void atualiza(string sql)
        {
            try
            {
                using (MySqlConnection con = abreconexao()) // Certifique-se de que a função abreconexao() retorna uma conexão válida
                {

                    if (con.State != ConnectionState.Open)
                    {
                        con.Open(); // Garante que a conexão está aberta
                    }

                    using (MySqlCommand comand = new MySqlCommand(sql, con))
                    {
                        comand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao executar atualização: " + ex.Message);
            }
        }

        public void atualizaComParametro(string sql, Dictionary<string, object> parametros)
        {

            MySqlCommand comand = new MySqlCommand();
            comand.CommandText = sql;
            comand.Connection = abreconexao();

          
                // Adiciona os parâmetros à query
                foreach (var param in parametros)
                {
                    comand.Parameters.AddWithValue(param.Key, param.Value);
                }

            comand.ExecuteNonQuery(); // Executa a query
            con.Close();
            con.Dispose();

        }

        public void atualizaTranacionamente(string sql, MySqlConnection con)
        {

            MySqlCommand comand = new MySqlCommand();
            comand.Connection = con;
            comand.CommandText = sql;
            comand.ExecuteNonQuery();

        }




        public DataTable consulta(string sql)
        {

            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(sql, abreconexao());

            while (con.State != ConnectionState.Open)
            {

                abreconexao();
            }
            if (con.State == ConnectionState.Open)
            {
                //MySqlDataAdapter da = new MySqlDataAdapter(sql, abreconexao());
                //MySqlDataAdapter da = new MySqlDataAdapter(sql, con);
                da.Fill(dt);
                con.Close();
                con.Dispose();
            }

            return dt;


        }

        public DataTable Consultadt(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection con = abreconexao()) // Certifique-se de que a função abreconexao() retorna uma conexão válida
                {
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open(); // Garante que a conexão está aberta
                    }

                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro na consulta: " + ex.Message);
            }

            return dt;
        }

        public DataTable consultaTransacionalmente(string sql, MySqlConnection con)
        {

            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(sql, con);
            da.Fill(dt);


            return dt;


        }

        public MySqlDataReader consultadr(string sql)
        {

            while (con.State != ConnectionState.Open)
            {

                abreconexao();
                //comand.ExecuteNonQuery();
                //con.Close();
                //con.Dispose();

            }


            MySqlCommand comand = new MySqlCommand();


            comand.CommandText = sql;
            comand.Connection = con;

            return comand.ExecuteReader();


            //O DATA READER É FECHADO NA PROPRIA APLICAÇÃO

        }

        public MySqlTransaction IniciarTransacao(MySqlConnection con)
        {
            MySqlTransaction trans = con.BeginTransaction();

            return trans;
        }

        public void ConfirmarTransacao(MySqlTransaction transatual)
        {
            transatual.Commit();
            con.Close();
        }

        public void CancelarTransacao(MySqlTransaction transatual)
        {
            transatual.Rollback();
            transatual.Dispose();
            con.Close();
        }
        #endregion

    }

}