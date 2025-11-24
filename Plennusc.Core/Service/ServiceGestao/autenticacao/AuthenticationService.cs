using appWhatsapp.SqlQueries;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.autenticacao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Plennusc.Core.Service.ServiceGestao.autenticacao
{
    public class AuthenticationService
    {
        private PermissaoQuery permissaoQuery = new PermissaoQuery();

        public void CarregarPermissoesDinamicas()
        {
            try
            {
                if (HttpContext.Current.Session["CodUsuario"] == null ||
                    HttpContext.Current.Session["CodSistema"] == null ||
                    HttpContext.Current.Session["CodEmpresa"] == null)
                    return;

                int codUsuario = Convert.ToInt32(HttpContext.Current.Session["CodUsuario"]);
                int codSistema = Convert.ToInt32(HttpContext.Current.Session["CodSistema"]);
                int codEmpresa = Convert.ToInt32(HttpContext.Current.Session["CodEmpresa"]);

                // Carrega estrutura completa do menu
                DataTable dtMenus = permissaoQuery.ObterEstruturaMenuCompleta(codUsuario, codSistema, codEmpresa);

                // Carrega permissões das páginas
                var permissoesPaginas = permissaoQuery.ObterPermissoesPaginas(codUsuario, codSistema, codEmpresa);

                // Armazena na sessão
                HttpContext.Current.Session["EstruturaMenus"] = dtMenus;
                HttpContext.Current.Session["PermissoesPaginas"] = permissoesPaginas;
                HttpContext.Current.Session["PermissoesCarregadas"] = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro CarregarPermissoesDinamicas: {ex.Message}");
            }
        }

        public bool ValidarAcessoPaginaDinamico()
        {
            try
            {
                if (HttpContext.Current.Session["CodUsuario"] == null)
                {
                    HttpContext.Current.Response.Redirect("~/ViewsApp/SignIn.aspx");
                    return false;
                }

                if (HttpContext.Current.Session["PermissoesPaginas"] == null)
                    CarregarPermissoesDinamicas();

                var permissoesPaginas = HttpContext.Current.Session["PermissoesPaginas"] as Dictionary<string, bool>;
                string paginaAtual = ObterNomePaginaAtual();

                // Se não encontrou a página nas permissões, nega acesso
                if (!permissoesPaginas.ContainsKey(paginaAtual))
                    return false;

                return permissoesPaginas[paginaAtual];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ValidarAcessoPaginaDinamico: {ex.Message}");
                return false;
            }
        }

        private string ObterNomePaginaAtual()
        {
            string url = HttpContext.Current.Request.Url.AbsolutePath;
            string[] partes = url.Split('/');
            string pagina = partes[partes.Length - 1];

            // Remove .aspx se existir
            if (pagina.EndsWith(".aspx"))
                pagina = pagina.Substring(0, pagina.Length - 5);

            return pagina;
        }
    }
}