using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace appWhatsapp.SqlQueries
{
    public class OperadoraUtil
    {
        public DataTable consultaOperadora()
        {
            string sql = @"
                           SELECT CODIGO_GRUPO_CONTRATO, DESCRICAO_GRUPO_CONTRATO FROM ESP0002 WHERE NUMERO_ANS_OPERADORA IS NOT NULL ORDER BY DESCRICAO_GRUPO_CONTRATO
                        ";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerAlianca(sql);
        }
    }
}