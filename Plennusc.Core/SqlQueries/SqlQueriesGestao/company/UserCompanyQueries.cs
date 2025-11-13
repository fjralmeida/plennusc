using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.company
{
    public static class UserCompanyQueries
    {
        public static string ListarUsuariosComAcesso = @"
            SELECT 
                p.CodPessoa,
                p.Nome,
                p.Sobrenome,
                p.DocCPF as CPF,
                p.Email,
                a.UsrNomeLogin as Login,
                a.CodAutenticacaoAcesso
            FROM Pessoa p
            INNER JOIN AutenticacaoAcesso a ON p.CodPessoa = a.CodPessoa
            WHERE a.Conf_PermiteAcesso = 1 
            AND a.Conf_Ativo = 1
            AND p.PermiteAcesso = 1
            ORDER BY p.Nome, p.Sobrenome";

        public static string ListarEmpresasAtivas = @"
            SELECT 
                CodEmpresa,
                NomeFantasia,
                RazaoSocial,
                Doc_CNPJ as CNPJ
            FROM Empresa
            WHERE Conf_Ativo = 1
            ORDER BY NomeFantasia";

        public static string ListarEmpresasVinculadasUsuario = @"
            SELECT DISTINCT se.CodEmpresa
            FROM SistemaEmpresaUsuario seu
            INNER JOIN SistemaEmpresa se ON seu.CodSistemaEmpresa = se.CodSistemaEmpresa
            WHERE seu.CodPessoa = @CodPessoa
            AND seu.Conf_LiberaAcesso = 1";

        public static string VincularUsuarioEmpresa = @"
            INSERT INTO SistemaEmpresaUsuario 
            (CodSistemaEmpresa, CodPessoa, CodAutenticacaoAcesso, Conf_LiberaAcesso, Conf_BloqueiaAcesso, DataHoraLiberacao, Informacoes_Log_I)
            SELECT 
                se.CodSistemaEmpresa, 
                @CodPessoa, 
                @CodAutenticacaoAcesso,
                1, 0, GETDATE(), GETDATE()
            FROM SistemaEmpresa se
            WHERE se.CodEmpresa = @CodEmpresa 
            AND se.Conf_LiberaAcesso = 1
            AND NOT EXISTS (
                SELECT 1 
                FROM SistemaEmpresaUsuario seu 
                WHERE seu.CodSistemaEmpresa = se.CodSistemaEmpresa 
                AND seu.CodPessoa = @CodPessoa
            )";

        public static string DesvincularUsuarioEmpresa = @"
            DELETE FROM SistemaEmpresaUsuario
            WHERE CodPessoa = @CodPessoa
            AND CodSistemaEmpresa IN (
                SELECT CodSistemaEmpresa 
                FROM SistemaEmpresa 
                WHERE CodEmpresa = @CodEmpresa
            )";
    }
}