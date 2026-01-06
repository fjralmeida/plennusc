using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.SysPlatform
{
    public static class companyMenuManagementSystemQueries
    {
        public static string ListarEmpresasAtivas = @"
            SELECT 
                CodEmpresa,
                NomeFantasia
            FROM Empresa
            WHERE Conf_Ativo = 1
            ORDER BY NomeFantasia";

        public static string ListarSistemasPorEmpresa = @"
            SELECT 
                se.CodSistemaEmpresa,
                s.CodSistema,
                s.NomeDisplay
            FROM SistemaEmpresa se
            INNER JOIN Sistema s ON se.CodSistema = s.CodSistema
            WHERE se.CodEmpresa = @CodEmpresa
            AND se.Conf_LiberaAcesso = 1
            ORDER BY s.NomeDisplay";

        // MODIFIQUE a query ListarMenusParaVincular para:
        public static string ListarMenusParaVincular = @"
SELECT 
    m.CodMenu,
    m.NomeMenu,
    m.NomeDisplay,
    m.NomeObjeto,
    m.Conf_Nivel,
    m.Conf_Ordem,
    m.CodMenuPai,
    CASE WHEN sem.CodSistemaEmpresaMenu IS NOT NULL THEN 1 ELSE 0 END as Vinculado
FROM Menu m
INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = @CodSistemaEmpresa
LEFT JOIN SistemaEmpresaMenu sem ON sem.CodMenu = m.CodMenu 
    AND sem.CodSistemaEmpresa = @CodSistemaEmpresa
WHERE m.Conf_Habilitado = 1
AND m.CodMenu NOT IN (1, 2, 3, 4)
-- Filtro por sistema usando lógica de inferência
AND (
    -- Sistema 1 (Gestão)
    (se.CodSistema = 1 AND (
        m.NomeObjeto NOT LIKE '%Medic%' AND 
        m.NomeObjeto NOT LIKE '%Finance%' AND
        m.NomeObjeto NOT LIKE '%la' -- Exclui interviewla
    ))
    OR
    -- Sistema 2 (Finance)
    (se.CodSistema = 2 AND m.NomeObjeto LIKE '%Finance%')
    OR
    -- Sistema 3 (Medic)
    (se.CodSistema = 3 AND (
        m.NomeObjeto LIKE '%Medic%' OR 
        m.NomeObjeto LIKE '%la' -- Inclui interviewla
    ))
)
ORDER BY 
    COALESCE(m.CodMenuPai, m.CodMenu),
    m.Conf_Ordem";

        //public static string ListarMenusParaVincular = @"
        //   SELECT 
        //    m.CodMenu,
        //    m.NomeMenu,
        //    m.NomeDisplay,
        //    m.NomeObjeto,
        //    m.Conf_Nivel,
        //    m.Conf_Ordem,
        //    m.CodMenuPai,
        //    CASE WHEN sem.CodSistemaEmpresaMenu IS NOT NULL THEN 1 ELSE 0 END as Vinculado
        //FROM Menu m
        //-- Junta com os menus já vinculados a algum SistemaEmpresa
        //INNER JOIN (
        //    -- DISTINCT para evitar duplicatas se um menu estiver em múltiplas empresas do mesmo sistema
        //    SELECT DISTINCT 
        //        se.CodSistema,
        //        sem.CodMenu
        //    FROM SistemaEmpresaMenu sem
        //    INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = sem.CodSistemaEmpresa
        //    WHERE sem.Conf_Habilitado = 1
        //) sistema_menus ON m.CodMenu = sistema_menus.CodMenu
        //-- Filtra apenas para o sistema atual
        //INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = @CodSistemaEmpresa
        //    AND sistema_menus.CodSistema = se.CodSistema
        //-- Left join para verificar se já está vinculado à empresa específica
        //LEFT JOIN SistemaEmpresaMenu sem ON sem.CodMenu = m.CodMenu 
        //    AND sem.CodSistemaEmpresa = @CodSistemaEmpresa
        //WHERE m.Conf_Habilitado = 1
        //AND m.CodMenu NOT IN (1, 2, 3, 4) -- Remove os duplicados/antigos
        //ORDER BY 
        //    COALESCE(m.CodMenuPai, m.CodMenu),
        //    m.Conf_Ordem
        //";



        public static string InserirSistemaEmpresaMenu = @"
            INSERT INTO SistemaEmpresaMenu (CodSistemaEmpresa, CodMenu, Conf_Habilitado, Conf_Exibir, Informacoes_log_i)
            VALUES (@CodSistemaEmpresa, @CodMenu, 1, 1, GETDATE())";


        public static string ExcluirSistemaEmpresaMenuUsuario = @"
            DELETE FROM SistemaEmpresaMenuUsuario 
            WHERE CodSistemaEmpresaMenu IN (
                SELECT CodSistemaEmpresaMenu 
                FROM SistemaEmpresaMenu 
                WHERE CodSistemaEmpresa = @CodSistemaEmpresa
                AND CodMenu = @CodMenu
            )";

        public static string ExcluirSistemaEmpresaMenu = @"
            DELETE FROM SistemaEmpresaMenu
            WHERE CodSistemaEmpresa = @CodSistemaEmpresa
            AND CodMenu = @CodMenu";

        public static string ExcluirTodosSistemaEmpresaMenuUsuario = @"
            DELETE FROM SistemaEmpresaMenuUsuario 
            WHERE CodSistemaEmpresaMenu IN (
                SELECT CodSistemaEmpresaMenu 
                FROM SistemaEmpresaMenu 
                WHERE CodSistemaEmpresa = @CodSistemaEmpresa
            )";

        public static string ExcluirTodosSistemaEmpresaMenu = @"
            DELETE FROM SistemaEmpresaMenu
            WHERE CodSistemaEmpresa = @CodSistemaEmpresa";
    }
}
