using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao.modelsCompany;
using Plennusc.Core.Models.ModelsGestao.modelsMenu;
using Plennusc.Core.Models.ModelsGestao.modelsSistema;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.SysPlatform;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.PlatformSys
{
    public class companyMenuManagementSystemService
    {
        private Banco_Dados_SQLServer _db;

        public companyMenuManagementSystemService()
        {
            _db = new Banco_Dados_SQLServer();
        }

        public List<EmpresaModel> ListarEmpresasAtivas()
        {
            var empresas = new List<EmpresaModel>();

            try
            {
                var resultado = _db.LerPlennus(companyMenuManagementSystemQueries.ListarEmpresasAtivas);

                foreach (DataRow row in resultado.Rows)
                {
                    empresas.Add(new EmpresaModel
                    {
                        CodEmpresa = Convert.ToInt32(row["CodEmpresa"]),
                        NomeFantasia = row["NomeFantasia"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar empresas: {ex.Message}", ex);
            }

            return empresas;
        }

        public List<SistemaModel> ListarSistemasPorEmpresa(int codEmpresa)
        {
            var sistemas = new List<SistemaModel>();

            try
            {
                var parametros = new Dictionary<string, object> { { "@CodEmpresa", codEmpresa } };
                var resultado = _db.LerPlennus(companyMenuManagementSystemQueries.ListarSistemasPorEmpresa, parametros);

                foreach (DataRow row in resultado.Rows)
                {
                    sistemas.Add(new SistemaModel
                    {
                        CodSistemaEmpresa = Convert.ToInt32(row["CodSistemaEmpresa"]),
                        CodSistema = Convert.ToInt32(row["CodSistema"]),
                        NomeDisplay = row["NomeDisplay"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar sistemas: {ex.Message}", ex);
            }

            return sistemas;
        }

        public List<MenuModel> ListarMenusParaVincular(int codSistemaEmpresa)
        {
            var menus = new List<MenuModel>();

            try
            {
                var parametros = new Dictionary<string, object> { { "@CodSistemaEmpresa", codSistemaEmpresa } };
                var resultado = _db.LerPlennus(companyMenuManagementSystemQueries.ListarMenusParaVincular, parametros);

                foreach (DataRow row in resultado.Rows)
                {
                    menus.Add(new MenuModel
                    {
                        CodMenu = Convert.ToInt32(row["CodMenu"]),
                        NomeMenu = row["NomeMenu"].ToString(),
                        NomeDisplay = row["NomeDisplay"].ToString(),
                        CodMenuPai = row["CodMenuPai"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CodMenuPai"]),
                        Conf_Nivel = Convert.ToInt32(row["Conf_Nivel"]),
                        Conf_Ordem = Convert.ToInt32(row["Conf_Ordem"]),
                        Vinculado = Convert.ToBoolean(row["Vinculado"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar menus: {ex.Message}", ex);
            }

            return menus;
        }

        public bool VincularMenuSistemaEmpresa(int codSistemaEmpresa, int codMenu)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CodSistemaEmpresa", codSistemaEmpresa },
                    { "@CodMenu", codMenu }
                };

                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(companyMenuManagementSystemQueries.InserirSistemaEmpresaMenu, parametros);
                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao vincular menu: {ex.Message}", ex);
            }
        }

        public bool DesvincularMenuSistemaEmpresa(int codSistemaEmpresa, int codMenu)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CodSistemaEmpresa", codSistemaEmpresa },
                    { "@CodMenu", codMenu }
                };

                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(companyMenuManagementSystemQueries.ExcluirSistemaEmpresaMenu, parametros);
                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao desvincular menu: {ex.Message}", ex);
            }
        }

        public bool DesvincularTodosMenusSistemaEmpresa(int codSistemaEmpresa)
        {
            try
            {
                var parametros = new Dictionary<string, object> { { "@CodSistemaEmpresa", codSistemaEmpresa } };
                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(companyMenuManagementSystemQueries.ExcluirTodosMenusSistemaEmpresa, parametros);
                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao desvincular todos os menus: {ex.Message}", ex);
            }
        }
    }
}