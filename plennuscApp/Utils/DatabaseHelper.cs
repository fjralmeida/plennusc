using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;

namespace appWhatsapp.Utils
{
    public static class DatabaseHelper
    {
        public static DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
                return db.LerPlennus(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao executar consulta: {ex.Message}", ex);
            }
        }

        public static object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
                // Se você tiver um método para executar scalar, use aqui
                // Se não, vamos adaptar
                DataTable dt = db.LerPlennus(sql, parameters);
                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                    return dt.Rows[0][0];
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao executar scalar: {ex.Message}", ex);
            }
        }
    }
}