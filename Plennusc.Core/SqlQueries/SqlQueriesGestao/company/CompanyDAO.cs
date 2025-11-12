using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao.modelsCompany;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.company
{
    public class CompanyDAO
    {
        public bool CNPJExiste(string cnpj)
        {
            string sql = "SELECT 1 FROM Empresa WHERE Doc_CNPJ = @CNPJ";
            var parametros = new Dictionary<string, object> { { "@CNPJ", cnpj } };
            var db = new Banco_Dados_SQLServer();
            var dt = db.LerPlennus(sql, parametros);
            return dt != null && dt.Rows.Count > 0;
        }

        public int InserirEmpresa(CompanyModel company)
        {
            string sql = @"
                INSERT INTO Empresa 
                    (RazaoSocial, NomeFantasia, Doc_CNPJ, Conf_LiberaAcesso, Conf_Ativo, Informacoes_log_i)
                VALUES 
                    (@RazaoSocial, @NomeFantasia, @CNPJ, @LiberaAcesso, @Ativo, GETDATE());
    
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        var parametros = new Dictionary<string, object>
            {
                { "@RazaoSocial", company.RazaoSocial },
                { "@NomeFantasia", company.NomeFantasia },
                { "@CNPJ", company.CNPJ },
                { "@LiberaAcesso", company.LiberaAcesso ? 1 : 0 },
                { "@Ativo", company.Ativo ? 1 : 0 }
            };

            var db = new Banco_Dados_SQLServer();
            var result = db.LerPlennus(sql, parametros);

            if (result != null && result.Rows.Count > 0)
                return Convert.ToInt32(result.Rows[0][0]);

            return 0;
        }

        public DataTable ListarEmpresas()
        {
            string sql = @"
                SELECT CodEmpresa, RazaoSocial, NomeFantasia, Doc_CNPJ, 
                       Conf_LiberaAcesso, Conf_Ativo, Informacoes_log_i
                FROM Empresa 
                ORDER BY RazaoSocial";

            var db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql, new Dictionary<string, object>());
        }
    }
}