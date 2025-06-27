using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.profile
{
    public class PessoaDAO
    {
        public DataRow ObterPessoaPorUsuario(int codUsuario)
        {
            string sql = @"
            SELECT TOP 1
                P.*
            FROM Pessoa P
            INNER JOIN AutenticacaoAcesso AA ON AA.CodPessoa = P.CodPessoa
            WHERE AA.CodAutenticacaoAcesso = @CodUsuario
        ";

            var parametros = new Dictionary<string, object>
        {
            { "@CodUsuario", codUsuario }
        };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            DataTable dt = db.LerPlennus(sql, parametros);

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
    }
}
