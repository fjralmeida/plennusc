using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao
{
    public static class userSystemMenuManagementQueries
    {
        public static string ObterUsuarios = @"
            SELECT 
                aa.CodAutenticacaoAcesso,
                p.Nome + ' ' + p.Sobrenome as NomeCompleto,
                aa.UsrNomeLogin
            FROM AutenticacaoAcesso aa
            INNER JOIN Pessoa p ON aa.CodPessoa = p.CodPessoa
            WHERE aa.Conf_Ativo = 1 AND p.Conf_Ativo = 1
            ORDER BY p.Nome";

        public static string ObterSistemaEmpresas = @"
            SELECT 
                se.CodSistemaEmpresa,
                s.NomeDisplay + ' - ' + e.NomeFantasia as SistemaEmpresaDisplay,
                CASE WHEN seu.CodSistemaEmpresaUsuario IS NOT NULL THEN 1 ELSE 0 END as JaVinculado
            FROM SistemaEmpresa se
            INNER JOIN Sistema s ON se.CodSistema = s.CodSistema
            INNER JOIN Empresa e ON se.CodEmpresa = e.CodEmpresa
            LEFT JOIN SistemaEmpresaUsuario seu ON se.CodSistemaEmpresa = seu.CodSistemaEmpresa 
                AND seu.CodAutenticacaoAcesso = @CodAutenticacao
            WHERE se.Conf_LiberaAcesso = 1
            AND s.Conf_LiberaUtilizacao = 1
            AND e.Conf_LiberaAcesso = 1
            ORDER BY s.NomeDisplay, e.NomeFantasia";

        public static string ObterMenusPorSistemaEmpresa = @"
        SELECT 
            sem.CodSistemaEmpresaMenu,
            m.CodMenu,
            m.NomeDisplay,
            m.Conf_Nivel,
            m.Conf_Ordem,
            m.CodMenuPai,  -- DEVE vir preenchido corretamente do JOIN
            CASE WHEN semu.CodSistemaEmpresaMenuUsuario IS NOT NULL THEN 1 ELSE 0 END as MenuJaVinculado
        FROM SistemaEmpresaMenu sem
        INNER JOIN Menu m ON sem.CodMenu = m.CodMenu
        LEFT JOIN SistemaEmpresaMenuUsuario semu ON sem.CodSistemaEmpresaMenu = semu.CodSistemaEmpresaMenu
            AND semu.CodAutenticacaoAcesso = @CodAutenticacao
        WHERE sem.CodSistemaEmpresa = @CodSistemaEmpresa
        AND sem.Conf_Habilitado = 1
        AND m.Conf_Habilitado = 1
        ORDER BY 
            m.CodMenuPai,
            m.Conf_Ordem,
            m.NomeDisplay";

        public static string VincularUsuarioSistemaEmpresa = @"
            IF NOT EXISTS (SELECT 1 FROM SistemaEmpresaUsuario 
                          WHERE CodSistemaEmpresa = @CodSistemaEmpresa 
                          AND CodAutenticacaoAcesso = @CodAutenticacao)
            BEGIN
                INSERT INTO SistemaEmpresaUsuario 
                (CodSistemaEmpresa, CodAutenticacaoAcesso, Conf_LiberaAcesso, DataHoraLiberacao)
                VALUES (@CodSistemaEmpresa, @CodAutenticacao, 1, GETDATE())
            END
            ELSE
            BEGIN
                UPDATE SistemaEmpresaUsuario 
                SET Conf_LiberaAcesso = 1, DataHoraLiberacao = GETDATE()
                WHERE CodSistemaEmpresa = @CodSistemaEmpresa 
                AND CodAutenticacaoAcesso = @CodAutenticacao
            END";

        public static string DesvincularUsuarioSistemaEmpresa = @"
            DELETE FROM SistemaEmpresaMenuUsuario 
            WHERE CodSistemaEmpresaMenu IN (
                SELECT sem.CodSistemaEmpresaMenu 
                FROM SistemaEmpresaMenu sem
                WHERE sem.CodSistemaEmpresa = @CodSistemaEmpresa
            )
            AND CodAutenticacaoAcesso = @CodAutenticacao;

            DELETE FROM SistemaEmpresaUsuario 
            WHERE CodSistemaEmpresa = @CodSistemaEmpresa 
            AND CodAutenticacaoAcesso = @CodAutenticacao";
        public static string VincularMenuUsuario = @"
            -- ✅ VERIFICA SE JÁ EXISTE ANTES DE INSERIR
            IF NOT EXISTS (
                SELECT 1 
                FROM SistemaEmpresaMenuUsuario semu
                INNER JOIN SistemaEmpresaMenu sem ON semu.CodSistemaEmpresaMenu = sem.CodSistemaEmpresaMenu
                WHERE sem.CodSistemaEmpresa = @CodSistemaEmpresa 
                AND sem.CodMenu = @CodMenu
                AND semu.CodAutenticacaoAcesso = @CodAutenticacao
            )
            BEGIN
                INSERT INTO SistemaEmpresaMenuUsuario 
                (CodSistemaEmpresaUsuario, CodAutenticacaoAcesso, CodSistemaEmpresaMenu, DataHoraLiberacao, Conf_LiberaAcesso)
                SELECT 
                    seu.CodSistemaEmpresaUsuario,
                    @CodAutenticacao,
                    sem.CodSistemaEmpresaMenu,
                    GETDATE(),
                    1
                FROM SistemaEmpresaUsuario seu
                INNER JOIN SistemaEmpresaMenu sem ON seu.CodSistemaEmpresa = sem.CodSistemaEmpresa
                WHERE seu.CodSistemaEmpresa = @CodSistemaEmpresa
                AND seu.CodAutenticacaoAcesso = @CodAutenticacao
                AND sem.CodMenu = @CodMenu
            END";

        public static string DesvincularTodosMenusUsuario = @"
            DELETE FROM SistemaEmpresaMenuUsuario 
            WHERE CodAutenticacaoAcesso = @CodAutenticacao
            AND CodSistemaEmpresaMenu IN (
                SELECT sem.CodSistemaEmpresaMenu 
                FROM SistemaEmpresaMenu sem
                WHERE sem.CodSistemaEmpresa = @CodSistemaEmpresa
            )";
    }
}