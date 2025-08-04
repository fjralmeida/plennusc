using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesMedic.interview
{
    public class MedicalInterview
    {
        public DataTable GetInterview(string status, string busca)
        {
            string sql = @"
        SELECT 
            CODIGO_ASSOCIADO,
            NOME_ASSOCIADO AS NOME,
            NUMERO_CPF AS CPF,
            DATA_NASCIMENTO AS DATA_NASC,
            ULTIMO_STATUS AS STATUS
        FROM VND1000_ON
        WHERE 1=1";

            if (!string.IsNullOrEmpty(status))
                sql += " AND ULTIMO_STATUS = @STATUS";

          

            if (!string.IsNullOrEmpty(busca))
                sql += " AND (NOME_ASSOCIADO LIKE @BUSCA OR NUMERO_CPF LIKE @BUSCA)";

            Banco_Dados_SQLServer bd = new Banco_Dados_SQLServer();
            var parametros = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(status)) parametros.Add("@STATUS", status);  
            if (!string.IsNullOrEmpty(busca)) parametros.Add("@BUSCA", "%" + busca + "%");

            return bd.LerAlianca(sql, parametros);
        }

        public DataTable GetDetalhesAssociado(int codigoAssociado)
        {
            string sql = @"
        SELECT 
            CODIGO_ASSOCIADO,
            NOME_ASSOCIADO AS NOME,
            NUMERO_CPF AS CPF,
            CODIGO_PLANO AS PLANO,
            CODIGO_EMPRESA AS EMPRESA,
            DATA_NASCIMENTO AS DATA_NASC,
            ULTIMO_STATUS AS STATUS,
            NOME_MAE,
            DATA_ADMISSAO
        FROM VND1000_ON
        WHERE CODIGO_ASSOCIADO = @CODIGO";

            Banco_Dados_SQLServer bd = new Banco_Dados_SQLServer();
            var parametros = new Dictionary<string, object>
    {
        { "@CODIGO", codigoAssociado }
    };

            return bd.LerAlianca(sql, parametros);
        }

    }
}
