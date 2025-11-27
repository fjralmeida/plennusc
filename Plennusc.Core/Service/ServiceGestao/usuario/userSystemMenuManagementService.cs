using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao.modelsUser;
using Plennusc.Core.SqlQueries.SqlQueriesGestao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.usuario
{
    public class userSystemMenuManagementService
    {
        private Banco_Dados_SQLServer _db;

        public userSystemMenuManagementService()
        {
            _db = new Banco_Dados_SQLServer();
        }

        public List<UsuarioSistemaEmpresaMenu> ObterUsuarios()
        {
            var usuarios = new List<UsuarioSistemaEmpresaMenu>();

            var resultado = _db.LerPlennus(userSystemMenuManagementQueries.ObterUsuarios);

            foreach (DataRow row in resultado.Rows)
            {
                usuarios.Add(new UsuarioSistemaEmpresaMenu
                {
                    CodAutenticacaoAcesso = Convert.ToInt32(row["CodAutenticacaoAcesso"]),
                    NomeCompleto = row["NomeCompleto"].ToString(),
                    UsrNomeLogin = row["UsrNomeLogin"].ToString()
                });
            }

            return usuarios;
        }

        public List<UsuarioSistemaEmpresaMenu> ObterSistemaEmpresas(int codAutenticacao)
        {
            var sistemaEmpresas = new List<UsuarioSistemaEmpresaMenu>();

            var parametros = new Dictionary<string, object>
            {
                { "@CodAutenticacao", codAutenticacao }
            };

            var resultado = _db.LerPlennus(userSystemMenuManagementQueries.ObterSistemaEmpresas, parametros);

            foreach (DataRow row in resultado.Rows)
            {
                sistemaEmpresas.Add(new UsuarioSistemaEmpresaMenu
                {
                    CodSistemaEmpresa = Convert.ToInt32(row["CodSistemaEmpresa"]),
                    SistemaEmpresaDisplay = row["SistemaEmpresaDisplay"].ToString(),
                    JaVinculado = Convert.ToInt32(row["JaVinculado"]) == 1
                });
            }

            return sistemaEmpresas;
        }

        public List<UsuarioSistemaEmpresaMenu> ObterMenusPorSistemaEmpresa(int codSistemaEmpresa, int codAutenticacao)
        {
            var menus = new List<UsuarioSistemaEmpresaMenu>();

            var parametros = new Dictionary<string, object>
            {
                { "@CodSistemaEmpresa", codSistemaEmpresa },
                { "@CodAutenticacao", codAutenticacao }
            };

            var resultado = _db.LerPlennus(userSystemMenuManagementQueries.ObterMenusPorSistemaEmpresa, parametros);

            foreach (DataRow row in resultado.Rows)
            {
                // VERIFIQUE OS NOMES DAS COLUNAS NO DATAROW
                menus.Add(new UsuarioSistemaEmpresaMenu
                {
                    CodSistemaEmpresaMenu = Convert.ToInt32(row["CodSistemaEmpresaMenu"]),
                    CodMenu = Convert.ToInt32(row["CodMenu"]),
                    NomeDisplay = row["NomeDisplay"].ToString(),
                    Conf_Nivel = Convert.ToInt32(row["Conf_Nivel"]),
                    Conf_Ordem = Convert.ToInt32(row["Conf_Ordem"]), 
                    CodMenuPai = row["CodMenuPai"] == DBNull.Value ? 0 : Convert.ToInt32(row["CodMenuPai"]), 
                    MenuJaVinculado = Convert.ToInt32(row["MenuJaVinculado"]) == 1
                });
            }

            return menus;
        }

        public bool VincularUsuarioSistemaEmpresa(int codSistemaEmpresa, int codAutenticacao)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CodSistemaEmpresa", codSistemaEmpresa },
                    { "@CodAutenticacao", codAutenticacao }
                };

                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(
                    userSystemMenuManagementQueries.VincularUsuarioSistemaEmpresa, parametros);

                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao vincular usuário ao sistema×empresa: {ex.Message}");
                return false;
            }
        }

        public bool DesvincularUsuarioSistemaEmpresa(int codSistemaEmpresa, int codAutenticacao)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CodSistemaEmpresa", codSistemaEmpresa },
                    { "@CodAutenticacao", codAutenticacao }
                };

                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(
                    userSystemMenuManagementQueries.DesvincularUsuarioSistemaEmpresa, parametros);

                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao desvincular usuário do sistema×empresa: {ex.Message}");
                return false;
            }
        }

        public bool VincularMenuUsuario(int codSistemaEmpresa, int codAutenticacao, int codMenu)
        {
            try
            {
                // PRIMEIRO OBTÉM O CodSistemaEmpresaMenu CORRETO
                int? codSistemaEmpresaMenu = ObterCodSistemaEmpresaMenu(codSistemaEmpresa, codMenu);

                if (!codSistemaEmpresaMenu.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine($"ERRO: Não encontrou CodSistemaEmpresaMenu para CodSistemaEmpresa={codSistemaEmpresa}, CodMenu={codMenu}");
                    return false;
                }

                var parametros = new Dictionary<string, object>
                {
                    { "@CodSistemaEmpresa", codSistemaEmpresa },
                    { "@CodAutenticacao", codAutenticacao },
                    { "@CodSistemaEmpresaMenu", codSistemaEmpresaMenu.Value } // ✅ AGORA CORRETO!
                };

                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(
                    userSystemMenuManagementQueries.VincularMenuUsuario, parametros);

                System.Diagnostics.Debug.WriteLine($"VincularMenuUsuario: CodSistemaEmpresa={codSistemaEmpresa}, CodAutenticacao={codAutenticacao}, CodMenu={codMenu}, CodSistemaEmpresaMenu={codSistemaEmpresaMenu.Value}, LinhasAfetadas={registrosAfetados}");

                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao vincular menu ao usuário: {ex.Message}");
                return false;
            }
        }

        // ADICIONA ESSE MÉTODO NO SERVICE, SEU DESGRAÇADO!
        public int? ObterCodSistemaEmpresaMenu(int codSistemaEmpresa, int codMenu)
        {
            try
            {
                var parametros = new Dictionary<string, object>
        {
            { "@CodSistemaEmpresa", codSistemaEmpresa },
            { "@CodMenu", codMenu }
        };

                string query = "SELECT CodSistemaEmpresaMenu FROM SistemaEmpresaMenu WHERE CodSistemaEmpresa = @CodSistemaEmpresa AND CodMenu = @CodMenu";

                // USA O MÉTODO QUE EXISTE, SEU ANIMAL!
                var resultado = _db.ExecutarPlennusScalar(query, parametros);

                if (resultado != null && resultado != DBNull.Value)
                    return Convert.ToInt32(resultado);
                else
                    return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter CodSistemaEmpresaMenu: {ex.Message}");
                return null;
            }
        }

        public bool DesvincularTodosMenusUsuario(int codSistemaEmpresa, int codAutenticacao)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CodSistemaEmpresa", codSistemaEmpresa },
                    { "@CodAutenticacao", codAutenticacao }
                };

                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(
                    userSystemMenuManagementQueries.DesvincularTodosMenusUsuario, parametros);

                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao desvincular menus do usuário: {ex.Message}");
                return false;
            }
        }
    }
}