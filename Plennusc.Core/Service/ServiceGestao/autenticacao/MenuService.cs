using Plennusc.Core.SqlQueries.SqlQueriesGestao.autenticacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Plennusc.Core.Service.ServiceGestao.autenticacao
{
    public class MenuService
    {
        private PermissaoQuery permissaoQuery = new PermissaoQuery();

        public bool HasMenuAccess(int codMenu)
        {
            try
            {
                if (HttpContext.Current.Session["CodUsuario"] == null ||
                    HttpContext.Current.Session["CodSistema"] == null ||
                    HttpContext.Current.Session["CodEmpresa"] == null)
                    return false;

                int codUsuario = Convert.ToInt32(HttpContext.Current.Session["CodUsuario"]);
                int codSistema = Convert.ToInt32(HttpContext.Current.Session["CodSistema"]);
                int codEmpresa = Convert.ToInt32(HttpContext.Current.Session["CodEmpresa"]);

                return permissaoQuery.VerificarAcessoMenu(codUsuario, codSistema, codEmpresa, codMenu);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro HasMenuAccess: {ex.Message}");
                return false;
            }
        }

        public bool ValidarAcessoSistemaEmpresa(int codSistema, int codEmpresa)
        {
            try
            {
                if (HttpContext.Current.Session["CodUsuario"] == null) return false;
                int codUsuario = Convert.ToInt32(HttpContext.Current.Session["CodUsuario"]);

                return permissaoQuery.VerificarAcessoSistemaEmpresa(codUsuario, codSistema, codEmpresa);
            }
            catch
            {
                return false;
            }
        }
    }
}