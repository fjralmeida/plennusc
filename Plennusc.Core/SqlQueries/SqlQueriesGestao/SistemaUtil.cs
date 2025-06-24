using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace appWhatsapp.SqlQueries
{
    public class SistemaUtil
    {
        public DataTable ConsultaSistema()
        {
            string sql = "SELECT CodSistema, NomeDisplay FROM Sistema WHERE Conf_LiberaUtilizacao = 1";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql);
        }
    }
}