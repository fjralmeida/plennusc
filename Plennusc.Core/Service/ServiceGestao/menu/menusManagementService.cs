using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao.modelsMenu;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.menu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.menu
{
    public class menusManagementService
    {
        private Banco_Dados_SQLServer _db;

        public menusManagementService()
        {
            _db = new Banco_Dados_SQLServer();
        }

        public List<menusManagementModels> ListarTodosMenus()
        {
            var menus = new List<menusManagementModels>();

            try
            {
                var resultado = _db.LerPlennus(MenuManagementQueries.ListarTodosMenus);

                foreach (DataRow row in resultado.Rows)
                {
                    menus.Add(new menusManagementModels
                    {
                        CodMenu = Convert.ToInt32(row["CodMenu"]),
                        NomeMenu = row["NomeMenu"].ToString(),
                        NomeDisplay = row["NomeDisplay"].ToString(),
                        NomeObjeto = row["NomeObjeto"].ToString(),
                        CaptionObjeto = row["CaptionObjeto"].ToString(),
                        HttpRouter = row["HttpRouter"]?.ToString(),
                        CodMenuPai = row["CodMenuPai"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CodMenuPai"]),
                        Conf_Ordem = Convert.ToInt32(row["Conf_Ordem"]),
                        Conf_Nivel = Convert.ToInt32(row["Conf_Nivel"]),
                        Conf_Habilitado = Convert.ToBoolean(row["Conf_Habilitado"]),
                        NomeMenuPai = row["NomeMenuPai"]?.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar menus: {ex.Message}", ex);
            }

            return menus;
        }

        public List<menusManagementModels> ListarMenusPrincipais()
        {
            var menus = new List<menusManagementModels>();

            try
            {
                var resultado = _db.LerPlennus(MenuManagementQueries.ListarMenusPrincipais);

                foreach (DataRow row in resultado.Rows)
                {
                    menus.Add(new menusManagementModels
                    {
                        CodMenu = Convert.ToInt32(row["CodMenu"]),
                        NomeMenu = row["NomeMenu"].ToString(),
                        NomeDisplay = row["NomeDisplay"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar menus principais: {ex.Message}", ex);
            }

            return menus;
        }

        public menusManagementModels ObterMenuPorCodigo(int codMenu)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@CodMenu", codMenu } };
                var resultado = _db.LerPlennus(MenuManagementQueries.ObterMenuPorCodigo, parametros);

                if (resultado.Rows.Count > 0)
                {
                    var row = resultado.Rows[0];
                    return new menusManagementModels
                    {
                        CodMenu = Convert.ToInt32(row["CodMenu"]),
                        NomeMenu = row["NomeMenu"].ToString(),
                        NomeDisplay = row["NomeDisplay"].ToString(),
                        NomeObjeto = row["NomeObjeto"].ToString(),
                        CaptionObjeto = row["CaptionObjeto"].ToString(),
                        HttpRouter = row["HttpRouter"]?.ToString(),
                        CodMenuPai = row["CodMenuPai"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CodMenuPai"]),
                        Conf_Ordem = Convert.ToInt32(row["Conf_Ordem"]),
                        Conf_Nivel = Convert.ToInt32(row["Conf_Nivel"]),
                        Conf_Habilitado = Convert.ToBoolean(row["Conf_Habilitado"])
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter menu: {ex.Message}", ex);
            }
        }

        public bool InserirMenu(menusManagementModels menu)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@NomeMenu", menu.NomeMenu },
                    { "@NomeDisplay", menu.NomeDisplay },
                    { "@NomeObjeto", menu.NomeObjeto },
                    { "@CaptionObjeto", menu.CaptionObjeto },
                    { "@HttpRouter", menu.HttpRouter ?? (object)DBNull.Value },
                    { "@CodMenuPai", menu.CodMenuPai ?? (object)DBNull.Value },
                    { "@Conf_Ordem", menu.Conf_Ordem },
                    { "@Conf_Nivel", menu.Conf_Nivel },
                    { "@Conf_Habilitado", menu.Conf_Habilitado ? 1 : 0 }
                };

                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(MenuManagementQueries.InserirMenu, parametros);
                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir menu: {ex.Message}", ex);
            }
        }

        public bool AtualizarMenu(menusManagementModels menu)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CodMenu", menu.CodMenu },
                    { "@NomeMenu", menu.NomeMenu },
                    { "@NomeDisplay", menu.NomeDisplay },
                    { "@NomeObjeto", menu.NomeObjeto },
                    { "@CaptionObjeto", menu.CaptionObjeto },
                    { "@HttpRouter", menu.HttpRouter ?? (object)DBNull.Value },
                    { "@CodMenuPai", menu.CodMenuPai ?? (object)DBNull.Value },
                    { "@Conf_Ordem", menu.Conf_Ordem },
                    { "@Conf_Nivel", menu.Conf_Nivel },
                    { "@Conf_Habilitado", menu.Conf_Habilitado ? 1 : 0 }
                };

                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(MenuManagementQueries.AtualizarMenu, parametros);
                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar menu: {ex.Message}", ex);
            }
        }

        public bool ExcluirMenu(int codMenu)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@CodMenu", codMenu } };
                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(MenuManagementQueries.ExcluirMenu, parametros);
                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir menu: {ex.Message}", ex);
            }
        }
    }
}