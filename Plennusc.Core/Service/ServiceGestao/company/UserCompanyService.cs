using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao.modelsCompany;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.company;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.company
{
    public class UserCompanyService
    {
        private Banco_Dados_SQLServer _db;

        public UserCompanyService()
        {
            _db = new Banco_Dados_SQLServer();
        }

        public List<UserCompanyModel> ListarUsuariosComAcesso()
        {
            var usuarios = new List<UserCompanyModel>();

            var resultado = _db.LerPlennus(UserCompanyQueries.ListarUsuariosComAcesso);

            foreach (DataRow row in resultado.Rows)
            {
                usuarios.Add(new UserCompanyModel
                {
                    CodPessoa = Convert.ToInt32(row["CodPessoa"]),
                    Nome = row["Nome"].ToString(),
                    Sobrenome = row["Sobrenome"].ToString(),
                    CPF = row["CPF"].ToString(),
                    Email = row["Email"].ToString(),
                    Login = row["Login"].ToString(),
                    CodAutenticacaoAcesso = Convert.ToInt32(row["CodAutenticacaoAcesso"])
                });
            }

            return usuarios;
        }

        public List<CompanySelectionModel> ListarEmpresasParaSelecao(int codPessoa)
        {
            var empresas = new List<CompanySelectionModel>();

            // Busca empresas ativas
            var resultadoEmpresas = _db.LerPlennus(UserCompanyQueries.ListarEmpresasAtivas);

            // Busca empresas vinculadas
            var parametros = new Dictionary<string, object> { { "@CodPessoa", codPessoa } };
            var resultadoVinculadas = _db.LerPlennus(UserCompanyQueries.ListarEmpresasVinculadasUsuario, parametros);

            var empresasVinculadas = resultadoVinculadas.Rows
                .Cast<DataRow>()
                .Select(row => Convert.ToInt32(row["CodEmpresa"]))
                .ToHashSet();

            // Monta lista final
            foreach (DataRow row in resultadoEmpresas.Rows)
            {
                int codEmpresa = Convert.ToInt32(row["CodEmpresa"]);
                bool jaVinculada = empresasVinculadas.Contains(codEmpresa);

                empresas.Add(new CompanySelectionModel
                {
                    CodEmpresa = codEmpresa,
                    NomeFantasia = row["NomeFantasia"].ToString(),
                    RazaoSocial = row["RazaoSocial"].ToString(),
                    CNPJ = row["CNPJ"].ToString(),
                    Selecionada = jaVinculada,
                    JaVinculada = jaVinculada
                });
            }

            return empresas;
        }

        public bool VincularUsuarioEmpresa(int codPessoa, int codAutenticacaoAcesso, int codEmpresa)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CodPessoa", codPessoa },
                    { "@CodAutenticacaoAcesso", codAutenticacaoAcesso },
                    { "@CodEmpresa", codEmpresa }
                };

                int registrosInseridos = _db.ExecutarPlennusLinhasAfetadas(UserCompanyQueries.VincularUsuarioEmpresa, parametros);
                return registrosInseridos > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao vincular usuário à empresa: {ex.Message}");
                return false;
            }
        }

        public bool DesvincularUsuarioEmpresa(int codPessoa, int codEmpresa)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                {
                    { "@CodPessoa", codPessoa },
                    { "@CodEmpresa", codEmpresa }
                };

                int registrosAfetados = _db.ExecutarPlennusLinhasAfetadas(UserCompanyQueries.DesvincularUsuarioEmpresa, parametros);
                return registrosAfetados > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao desvincular usuário da empresa: {ex.Message}");
                return false;
            }
        }
    }
}