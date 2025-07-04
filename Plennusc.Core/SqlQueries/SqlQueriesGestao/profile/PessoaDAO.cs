using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
                    P.CodPessoa, P.Nome, P.Sobrenome, P.Apelido, P.Sexo, P.DataNasc, 
                    P.CodEstr_TipoPessoa, P.ImagemFoto, P.DocRG, P.DocCPF, P.TituloEleitor, 
                    P.ZonaEleitor, P.SecaoEleitor, P.NumCTPS, P.NumCTPSSerie, P.NumCTPSUf, 
                    P.NumPIS, P.Matricula, P.DataAdmissao, P.DataDemissao, 
                    P.NomeFiliacao1, P.NomeFiliacao2, P.Telefone1, P.Telefone2, P.Telefone3, 
                    P.Email, P.EmailAlt, P.CodCargo, P.CodDepartamento, 
                    P.Conf_CriaContaAD, P.DataHora_CriaContaAD, 
                    P.Conf_CadastraPonto, P.DataHora_CadastraPonto, 
                    P.Conf_Ativo, P.PermiteAcesso, P.AcessoSite, P.DataUltimoAcesso, 
                    P.Observacao, P.Informacoes_Log_I, P.Informacoes_Log_A, P.Informacoes_Log_E
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

        public void UpdateImgPerfil(int codPessoa, string nomeImagem)
        {
            string sql = "UPDATE Pessoa SET ImagemFoto = @ImagemFoto WHERE CodPessoa = @CodPessoa";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();

            var parametros = new Dictionary<string, object>
            {
                { "@ImagemFoto", nomeImagem },
                { "@CodPessoa", codPessoa }
            };

            db.ExecutarPlennus(sql, parametros);
        }
    }
}
