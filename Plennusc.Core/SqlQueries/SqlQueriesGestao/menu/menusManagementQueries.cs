using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.menu
{
    public static class MenuManagementQueries
    {
        public static string ListarTodosMenus = @"
            SELECT 
                m.CodMenu,
                m.NomeMenu,
                m.NomeDisplay,
                m.NomeObjeto,
                m.CaptionObjeto,
                m.HttpRouter,
                m.CodMenuPai,
                m.Conf_Ordem,
                m.Conf_Nivel,
                m.Conf_Habilitado,
                mp.NomeMenu as NomeMenuPai
            FROM Menu m
            LEFT JOIN Menu mp ON m.CodMenuPai = mp.CodMenu
            ORDER BY m.Conf_Nivel, m.Conf_Ordem, m.NomeMenu";

        public static string ListarMenusPrincipais = @"
            SELECT 
                CodMenu,
                NomeMenu,
                NomeDisplay
            FROM Menu 
            WHERE CodMenuPai IS NULL 
            AND Conf_Habilitado = 1
            ORDER BY Conf_Ordem, NomeMenu";

        public static string ObterMenuPorCodigo = @"
            SELECT 
                CodMenu,
                NomeMenu,
                NomeDisplay,
                NomeObjeto,
                CaptionObjeto,
                HttpRouter,
                CodMenuPai,
                Conf_Ordem,
                Conf_Nivel,
                Conf_Habilitado
            FROM Menu 
            WHERE CodMenu = @CodMenu";

        public static string InserirMenu = @"
            INSERT INTO Menu (
                NomeMenu, NomeDisplay, NomeObjeto, CaptionObjeto, 
                HttpRouter, CodMenuPai, Conf_Ordem, Conf_Nivel, Conf_Habilitado, Informacoes_log_i
            ) VALUES (
                @NomeMenu, @NomeDisplay, @NomeObjeto, @CaptionObjeto, 
                @HttpRouter, @CodMenuPai, @Conf_Ordem, @Conf_Nivel, @Conf_Habilitado, GETDATE()
            )";

        public static string AtualizarMenu = @"
            UPDATE Menu SET
                NomeMenu = @NomeMenu,
                NomeDisplay = @NomeDisplay,
                NomeObjeto = @NomeObjeto,
                CaptionObjeto = @CaptionObjeto,
                HttpRouter = @HttpRouter,
                CodMenuPai = @CodMenuPai,
                Conf_Ordem = @Conf_Ordem,
                Conf_Nivel = @Conf_Nivel,
                Conf_Habilitado = @Conf_Habilitado,
                Informacoes_log_a = GETDATE()
            WHERE CodMenu = @CodMenu";

        public static string ExcluirMenu = @"
            DELETE FROM Menu 
            WHERE CodMenu = @CodMenu
            AND NOT EXISTS (
                SELECT 1 FROM SistemaEmpresaMenu WHERE CodMenu = @CodMenu
            )";
    }
}