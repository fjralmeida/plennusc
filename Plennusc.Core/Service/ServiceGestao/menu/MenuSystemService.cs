using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao.modelsMenu;
using Plennusc.Core.Service.ServiceGestao.PlatformSys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.menu
{
    public class MenuSystemService
    {
        private Banco_Dados_SQLServer _db;
        private companyMenuManagementSystemService _companyMenuService;

        public MenuSystemService()
        {
            _db = new Banco_Dados_SQLServer();
            _companyMenuService = new companyMenuManagementSystemService();
        }

        public List<MenuModel> ListarTodosMenusParaConfiguracao()
        {
            return _companyMenuService.ListarTodosMenusParaConfiguracao();
        }

        public bool EstaVinculadoEmAlgumSistema(int codMenu)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM SistemaEmpresaMenu sem
                INNER JOIN SistemaEmpresa se ON sem.CodSistemaEmpresa = se.CodSistemaEmpresa
                WHERE sem.CodMenu = @CodMenu AND se.CodEmpresa = 1";

            var parametros = new Dictionary<string, object> { { "@CodMenu", codMenu } };

            try
            {
                var resultado = _db.LerPlennus(query, parametros);
                return Convert.ToInt32(resultado.Rows[0][0]) > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool EstaVinculadoAoSistema(int codMenu, int codSistema)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM SistemaEmpresaMenu sem
                INNER JOIN SistemaEmpresa se ON sem.CodSistemaEmpresa = se.CodSistemaEmpresa
                WHERE sem.CodMenu = @CodMenu AND se.CodSistema = @CodSistema AND se.CodEmpresa = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@CodMenu", codMenu },
                { "@CodSistema", codSistema }
            };

            try
            {
                var resultado = _db.LerPlennus(query, parametros);
                return Convert.ToInt32(resultado.Rows[0][0]) > 0;
            }
            catch
            {
                return false;
            }
        }

        public List<int> GetSistemasVinculados(int codMenu)
        {
            var sistemas = new List<int>();

            string query = @"
                SELECT DISTINCT se.CodSistema
                FROM SistemaEmpresaMenu sem
                INNER JOIN SistemaEmpresa se ON sem.CodSistemaEmpresa = se.CodSistemaEmpresa
                WHERE sem.CodMenu = @CodMenu AND se.CodEmpresa = 1";

            var parametros = new Dictionary<string, object> { { "@CodMenu", codMenu } };

            try
            {
                var resultado = _db.LerPlennus(query, parametros);
                foreach (DataRow row in resultado.Rows)
                {
                    sistemas.Add(Convert.ToInt32(row["CodSistema"]));
                }
            }
            catch
            {
                // Retorna lista vazia se houver erro
            }

            return sistemas;
        }

        public void SalvarConfiguracoesMenu(int codMenu, bool gestao, bool finance, bool medic, bool ouvidoria)
        {
            // Primeiro, precisamos verificar se existem outros menus com o mesmo NomeObjeto
            string queryNomeObjeto = "SELECT NomeObjeto FROM Menu WHERE CodMenu = @CodMenu";
            var parametrosNome = new Dictionary<string, object> { { "@CodMenu", codMenu } };
            var resultadoNome = _db.LerPlennus(queryNomeObjeto, parametrosNome);

            if (resultadoNome.Rows.Count == 0) return;

            string nomeObjeto = resultadoNome.Rows[0]["NomeObjeto"].ToString();

            // Buscar todos os menus com o mesmo NomeObjeto
            string queryDuplicados = "SELECT CodMenu FROM Menu WHERE NomeObjeto = @NomeObjeto AND CodMenu != @CodMenu";
            var parametrosDup = new Dictionary<string, object>
            {
                { "@NomeObjeto", nomeObjeto },
                { "@CodMenu", codMenu }
            };
            var resultadoDup = _db.LerPlennus(queryDuplicados, parametrosDup);

            List<int> menusDuplicados = new List<int>();
            foreach (DataRow row in resultadoDup.Rows)
            {
                menusDuplicados.Add(Convert.ToInt32(row["CodMenu"]));
            }

            // Para cada sistema, atualizar o vínculo para ESTE menu E para os duplicados
            AtualizarVinculoSistemaEMDuplicados(codMenu, menusDuplicados, 1, gestao);
            AtualizarVinculoSistemaEMDuplicados(codMenu, menusDuplicados, 2, finance);
            AtualizarVinculoSistemaEMDuplicados(codMenu, menusDuplicados, 3, medic);
            AtualizarVinculoSistemaEMDuplicados(codMenu, menusDuplicados, 4, ouvidoria);
        }

        private void AtualizarVinculoSistemaEMDuplicados(int codMenu, List<int> menusDuplicados, int codSistema, bool deveEstarVinculado)
        {
            // Atualizar para o menu principal
            AtualizarVinculoSistema(codMenu, codSistema, deveEstarVinculado);

            // Atualizar para cada menu duplicado
            foreach (int codMenuDuplicado in menusDuplicados)
            {
                AtualizarVinculoSistema(codMenuDuplicado, codSistema, deveEstarVinculado);
            }
        }

        private void AtualizarVinculoSistema(int codMenu, int codSistema, bool deveEstarVinculado)
        {
            int? codSistemaEmpresa = GetCodSistemaEmpresa(codSistema);

            if (!codSistemaEmpresa.HasValue)
            {
                throw new Exception($"Sistema {codSistema} não encontrado para empresa 1");
            }

            bool jaVinculado = VerificarVinculo(codMenu, codSistemaEmpresa.Value);

            if (deveEstarVinculado && !jaVinculado)
            {
                _companyMenuService.VincularMenuSistemaEmpresa(codSistemaEmpresa.Value, codMenu);
            }
            else if (!deveEstarVinculado && jaVinculado)
            {
                _companyMenuService.DesvincularMenuSistemaEmpresa(codSistemaEmpresa.Value, codMenu);
            }
        }

        private int? GetCodSistemaEmpresa(int codSistema)
        {
            string query = "SELECT CodSistemaEmpresa FROM SistemaEmpresa WHERE CodSistema = @CodSistema AND CodEmpresa = 1";
            var parametros = new Dictionary<string, object> { { "@CodSistema", codSistema } };

            try
            {
                var resultado = _db.LerPlennus(query, parametros);
                if (resultado.Rows.Count > 0)
                {
                    return Convert.ToInt32(resultado.Rows[0]["CodSistemaEmpresa"]);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private bool VerificarVinculo(int codMenu, int codSistemaEmpresa)
        {
            string query = "SELECT COUNT(*) FROM SistemaEmpresaMenu WHERE CodSistemaEmpresa = @CodSistemaEmpresa AND CodMenu = @CodMenu";
            var parametros = new Dictionary<string, object>
            {
                { "@CodSistemaEmpresa", codSistemaEmpresa },
                { "@CodMenu", codMenu }
            };

            try
            {
                var resultado = _db.LerPlennus(query, parametros);
                return Convert.ToInt32(resultado.Rows[0][0]) > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}