using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace appWhatsapp.Utils
{
    public static class MenuAuthorization
    {
        public static List<int> GetUserMenuPermissions(int codUsuario, int codSistema, int codEmpresa)
        {
            string sql = @"
                SELECT DISTINCT M.CodMenu
                FROM SistemaEmpresaMenuUsuario SEMU
                INNER JOIN SistemaEmpresaMenu SEM ON SEMU.CodSistemaEmpresaMenu = SEM.CodSistemaEmpresaMenu
                INNER JOIN SistemaEmpresa SE ON SEM.CodSistemaEmpresa = SE.CodSistemaEmpresa
                INNER JOIN Menu M ON SEM.CodMenu = M.CodMenu
                WHERE SEMU.CodAutenticacaoAcesso = @CodUsuario
                AND SE.CodSistema = @CodSistema
                AND SE.CodEmpresa = @CodEmpresa
                AND SEMU.Conf_LiberaAcesso = 1
                AND SEM.Conf_Habilitado = 1
                AND M.Conf_Habilitado = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parametros);

            return dt.AsEnumerable()
                    .Select(row => row.Field<int>("CodMenu"))
                    .ToList();
        }

        public static bool HasAccessToPage(int codUsuario, int codSistema, int codEmpresa, string pageName)
        {
            string cleanPageName = pageName.Replace(".aspx", "").Split('?')[0];

            string sql = @"
                SELECT COUNT(1) as HasAccess
                FROM SistemaEmpresaMenuUsuario SEMU
                INNER JOIN SistemaEmpresaMenu SEM ON SEMU.CodSistemaEmpresaMenu = SEM.CodSistemaEmpresaMenu
                INNER JOIN SistemaEmpresa SE ON SEM.CodSistemaEmpresa = SE.CodSistemaEmpresa
                INNER JOIN Menu M ON SEM.CodMenu = M.CodMenu
                WHERE SEMU.CodAutenticacaoAcesso = @CodUsuario
                AND SE.CodSistema = @CodSistema
                AND SE.CodEmpresa = @CodEmpresa
                AND M.NomeObjeto = @PageName
                AND SEMU.Conf_LiberaAcesso = 1
                AND SEM.Conf_Habilitado = 1
                AND M.Conf_Habilitado = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa },
                { "@PageName", cleanPageName }
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(sql, parametros);

            return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["HasAccess"]) > 0;
        }
    }
}