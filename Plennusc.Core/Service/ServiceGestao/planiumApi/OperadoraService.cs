using Plennusc.Core.Models.ModelsGestao.modelsOperator;     // ✅ único using para DTOs
using Plennusc.Core.SqlQueries.SqlQueriesGestao.operatorRegistration;             // OperatorRegistrationQueries
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Plennusc.Core.Service.ServiceGestao.planiumApi
{
    public class OperadoraService
    {
        private readonly string _csPlennus;   // Banco 2
        private readonly string _csAlianca;   // Banco 1

        public OperadoraService(
            string connectionPlennus = "Plennus",
            string connectionAlianca = "Alianca")
        {
            _csPlennus = ConfigurationManager.ConnectionStrings[connectionPlennus].ConnectionString;
            _csAlianca = ConfigurationManager.ConnectionStrings[connectionAlianca].ConnectionString;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private SqlConnection OpenPlennus()
        {
            var c = new SqlConnection(_csPlennus);
            c.Open();
            return c;
        }

        private SqlConnection OpenAlianca()
        {
            var c = new SqlConnection(_csAlianca);
            c.Open();
            return c;
        }

        // ─────────────────────────────────────────────────────────────────────
        // LISTAGEM — Banco 2
        // ─────────────────────────────────────────────────────────────────────

        public List<OperadoraListDto> ListarOperadoras(OperadoraFiltro filtro)
        {
            var lista = new List<OperadoraListDto>();
            var sbNome = new StringBuilder();
            var sbAns = new StringBuilder();
            var sbCnpj = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(filtro?.NomeOperadora))
                sbNome.Append("AND o.NomeComercial LIKE @NomeOperadora");

            if (!string.IsNullOrWhiteSpace(filtro?.RegistroANS))
                sbAns.Append("AND CAST(o.RegistroANS AS VARCHAR) LIKE @RegistroANS");

            if (!string.IsNullOrWhiteSpace(filtro?.Numero_CNPJ))
                sbCnpj.Append("AND o.Numero_CNPJ LIKE @Numero_CNPJ");

            var sql = OperatorRegistrationQueries.ListarOperadoras
                .Replace("{FILTRO_NOME}", sbNome.ToString())
                .Replace("{FILTRO_ANS}", sbAns.ToString())
                .Replace("{FILTRO_CNPJ}", sbCnpj.ToString());

            using (var con = OpenPlennus())
            using (var cmd = new SqlCommand(sql, con))
            {
                if (!string.IsNullOrWhiteSpace(filtro?.NomeOperadora))
                    cmd.Parameters.AddWithValue("@NomeOperadora", "%" + filtro.NomeOperadora.Trim() + "%");

                if (!string.IsNullOrWhiteSpace(filtro?.RegistroANS))
                    cmd.Parameters.AddWithValue("@RegistroANS", "%" + filtro.RegistroANS.Trim() + "%");

                if (!string.IsNullOrWhiteSpace(filtro?.Numero_CNPJ))
                    cmd.Parameters.AddWithValue("@Numero_CNPJ", "%" + filtro.Numero_CNPJ.Trim() + "%");

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new OperadoraListDto
                        {
                            CodigoOperadora = rd.GetInt32(rd.GetOrdinal("CodigoOperadora")),
                            RegistroAns = rd.IsDBNull(rd.GetOrdinal("RegistroANS")) ? null : rd.GetInt32(rd.GetOrdinal("RegistroANS")).ToString(),
                            Numero_CNPJ = rd.IsDBNull(rd.GetOrdinal("Numero_CNPJ")) ? null : rd.GetString(rd.GetOrdinal("Numero_CNPJ")),
                            RazaoSocial = rd.IsDBNull(rd.GetOrdinal("RazaoSocial")) ? null : rd.GetString(rd.GetOrdinal("RazaoSocial")),
                            NomeComercial = rd.IsDBNull(rd.GetOrdinal("NomeComercial")) ? null : rd.GetString(rd.GetOrdinal("NomeComercial")),
                            CodPessoaCadastro = rd.IsDBNull(rd.GetOrdinal("CodPessoaCadastro")) ? (int?)null : rd.GetInt32(rd.GetOrdinal("CodPessoaCadastro")),
                            CodPessoaAlteracao = rd.IsDBNull(rd.GetOrdinal("CodPessoaAlteracao")) ? (int?)null : rd.GetInt32(rd.GetOrdinal("CodPessoaAlteracao")),
                            Informacoes_log_i = rd.IsDBNull(rd.GetOrdinal("Informacoes_log_i")) ? (DateTime?)null : rd.GetDateTime(rd.GetOrdinal("Informacoes_log_i")),
                            Informacoes_log_a = rd.IsDBNull(rd.GetOrdinal("Informacoes_log_a")) ? (DateTime?)null : rd.GetDateTime(rd.GetOrdinal("Informacoes_log_a"))
                        });
                    }
                }
            }

            return lista;
        }

        // ─────────────────────────────────────────────────────────────────────
        // SYNC — detectar pendentes (compara Banco 1 x Banco 2 por CNPJ em memória)
        // ─────────────────────────────────────────────────────────────────────

        public List<OperadoraSyncDto> BuscarOperadorasPendentes()
        {
            // 1. CNPJs já no Banco 2
            var cnpjsCadastrados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var con = OpenPlennus())
            using (var cmd = new SqlCommand(OperatorRegistrationQueries.BuscarCNPJsCadastrados, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    if (!rd.IsDBNull(0))
                        cnpjsCadastrados.Add(rd.GetString(0).Trim());
            }

            // 2. Todas do Banco 1 → filtra quem não está no Banco 2
            var pendentes = new List<OperadoraSyncDto>();

            using (var con = OpenAlianca())
            using (var cmd = new SqlCommand(OperatorRegistrationQueries.BuscarTodasOperadorasAlianca, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    var cnpj = rd.IsDBNull(rd.GetOrdinal("Numero_CNPJ"))
                        ? null
                        : rd.GetString(rd.GetOrdinal("Numero_CNPJ")).Trim();

                    if (string.IsNullOrWhiteSpace(cnpj) || cnpjsCadastrados.Contains(cnpj))
                        continue;

                    pendentes.Add(new OperadoraSyncDto
                    {
                        CodigoGrupo = rd.GetInt32(rd.GetOrdinal("CodigoGrupo")),
                        Numero_CNPJ = cnpj,
                        RegistroANS = rd.IsDBNull(rd.GetOrdinal("RegistroANS")) ? null : rd.GetString(rd.GetOrdinal("RegistroANS")),
                        RazaoSocial = rd.IsDBNull(rd.GetOrdinal("RazaoSocial")) ? null : rd.GetString(rd.GetOrdinal("RazaoSocial")),
                        NomeComercial = rd.IsDBNull(rd.GetOrdinal("NomeComercial")) ? null : rd.GetString(rd.GetOrdinal("NomeComercial"))
                    });
                }
            }

            return pendentes;
        }

        // ─────────────────────────────────────────────────────────────────────
        // SYNC — confirmar: INSERT no Banco 2 com Informacoes_log_i + CodPessoa
        // ─────────────────────────────────────────────────────────────────────

        public void ConfirmarSincronizacao(List<OperadoraSyncDto> pendentes, int codPessoa)
        {
            if (pendentes == null || pendentes.Count == 0) return;

            // Proteção extra: nunca insere operadora com ANS inválido,
            // mesmo que tenha chegado até aqui por algum bug no front-end.
            var validas = pendentes.Where(p => p.AnsValido).ToList();

            if (validas.Count == 0) return;

            var agora = DateTime.Now;

            using (var con = OpenPlennus())
            using (var tran = con.BeginTransaction())
            {
                try
                {
                    foreach (var op in validas)
                    {
                        using (var cmd = new SqlCommand(OperatorRegistrationQueries.InserirOperadora, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@RegistroANS", int.Parse(op.RegistroANS.Trim()));
                            cmd.Parameters.AddWithValue("@Numero_CNPJ", (object)op.Numero_CNPJ ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@RazaoSocial", (object)op.RazaoSocial ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@NomeComercial", (object)op.NomeComercial ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CodPessoaCadastro", codPessoa);
                            cmd.Parameters.AddWithValue("@DataLog", agora);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // PESSOA — nome do usuário logado para exibição no modal
        // ─────────────────────────────────────────────────────────────────────

        public string BuscarNomePessoaPorAutenticacao(int codAutenticacaoAcesso)
        {
            using (var con = OpenPlennus())
            using (var cmd = new SqlCommand(OperatorRegistrationQueries.BuscarNomePessoaPorAutenticacao, con))
            {
                cmd.Parameters.AddWithValue("@CodAutenticacaoAcesso", codAutenticacaoAcesso);

                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        var apelido = rd.IsDBNull(rd.GetOrdinal("Apelido"))
                            ? string.Empty
                            : rd.GetString(rd.GetOrdinal("Apelido")).Trim();

                        if (!string.IsNullOrWhiteSpace(apelido))
                            return apelido;

                        return rd.IsDBNull(rd.GetOrdinal("NomeCompleto"))
                            ? string.Empty
                            : rd.GetString(rd.GetOrdinal("NomeCompleto")).Trim();
                    }
                }
            }

            return string.Empty;
        }

        // ─────────────────────────────────────────────────────────────────────
        // ALTERAÇÕES — detectar divergências entre Aliança e Plennus
        // ─────────────────────────────────────────────────────────────────────

        public List<OperadoraAlteracoesDto> BuscarOperadorasComAlteracoes()
        {
            var existentesPlennus = new Dictionary<string, OperadoraAlteracoesDto>(StringComparer.OrdinalIgnoreCase);

            using (var con = OpenPlennus())
            using (var cmd = new SqlCommand(OperatorRegistrationQueries.BuscarOperadorasExistentesPlennus, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    var cnpj = rd.GetString(rd.GetOrdinal("Numero_CNPJ")).Trim();

                    existentesPlennus[cnpj] = new OperadoraAlteracoesDto
                    {
                        CodigoOperadora = rd.GetInt32(rd.GetOrdinal("CodigoOperadora")),
                        Numero_CNPJ = cnpj,
                        RegistroANS_Atual = rd.IsDBNull(rd.GetOrdinal("RegistroANS")) ? null : rd.GetInt32(rd.GetOrdinal("RegistroANS")).ToString(),
                        RazaoSocial_Atual = rd.IsDBNull(rd.GetOrdinal("RazaoSocial")) ? null : rd.GetString(rd.GetOrdinal("RazaoSocial")),
                        NomeComercial_Atual = rd.IsDBNull(rd.GetOrdinal("NomeComercial")) ? null : rd.GetString(rd.GetOrdinal("NomeComercial"))
                    };
                }
            }

            var divergentes = new List<OperadoraAlteracoesDto>();

            using (var con = OpenAlianca())
            using (var cmd = new SqlCommand(OperatorRegistrationQueries.BuscarTodasOperadorasAlianca, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    var cnpj = rd.IsDBNull(rd.GetOrdinal("Numero_CNPJ"))
                        ? null
                        : rd.GetString(rd.GetOrdinal("Numero_CNPJ")).Trim();

                    if (string.IsNullOrWhiteSpace(cnpj) || !existentesPlennus.ContainsKey(cnpj))
                        continue;

                    var dto = existentesPlennus[cnpj];

                    dto.RegistroANS_Novo = rd.IsDBNull(rd.GetOrdinal("RegistroANS")) ? null : rd.GetString(rd.GetOrdinal("RegistroANS"));
                    dto.RazaoSocial_Novo = rd.IsDBNull(rd.GetOrdinal("RazaoSocial")) ? null : rd.GetString(rd.GetOrdinal("RazaoSocial"));
                    dto.NomeComercial_Novo = rd.IsDBNull(rd.GetOrdinal("NomeComercial")) ? null : rd.GetString(rd.GetOrdinal("NomeComercial"));

                    if (dto.TemDivergenciaValida)
                        divergentes.Add(dto);
                }
            }

            return divergentes;
        }

        // ─────────────────────────────────────────────────────────────────────
        // ALTERAÇÕES — confirmar: UPDATE no Banco 2
        // ─────────────────────────────────────────────────────────────────────

        public void ConfirmarAlteracoes(List<OperadoraAlteracoesDto> alteracoes, int codAutenticacaoAcesso)
        {
            if (alteracoes == null || alteracoes.Count == 0) return;

            var agora = DateTime.Now;

            using (var con = OpenPlennus())
            using (var tran = con.BeginTransaction())
            {
                try
                {
                    foreach (var alt in alteracoes)
                    {
                        string setAns = alt.DivergeAns ? "RegistroANS = @RegistroANS," : "";
                        var sql = OperatorRegistrationQueries.AtualizarOperadoraDinamica.Replace("{SET_ANS}", setAns);

                        using (var cmd = new SqlCommand(sql, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@RazaoSocial",
                                (object)(alt.DivergeRazaoSocial ? alt.RazaoSocial_Novo : alt.RazaoSocial_Atual) ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@NomeComercial",
                                (object)(alt.DivergeNomeComercial ? alt.NomeComercial_Novo : alt.NomeComercial_Atual) ?? DBNull.Value);

                            if (alt.DivergeAns)
                                cmd.Parameters.AddWithValue("@RegistroANS", int.Parse(alt.RegistroANS_Novo.Trim()));

                            cmd.Parameters.AddWithValue("@CodPessoaAlteracao", codAutenticacaoAcesso);
                            cmd.Parameters.AddWithValue("@DataLog", agora);
                            cmd.Parameters.AddWithValue("@Numero_CNPJ", alt.Numero_CNPJ);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        // ───────────────────────────────────────────────────────────────────────────
        //  Edição de Operadora — Banco 2 (Plennus) — Razão Social e Nome Comercial
        // ───────────────────────────────────────────────────────────────────────────

        // No front já está retornando o nome e a razão social, de acordo com o código da operadora. Aqui é só atualizar no banco Plennus, caso o usuário altere algum campo.

        public bool AtualizarOperadora(int codigoOperadora, string razaoSocial, string nomeComercial)
        {
            // Valida se pelo menos um campo foi preenchido
            if (string.IsNullOrWhiteSpace(razaoSocial) && string.IsNullOrWhiteSpace(nomeComercial))
                return false;

            try
            {
                using (var con = OpenPlennus())
                {
                    // Primeiro verifica se a operadora existe usando a query do arquivo operatorRegistrationQueries.cs, na variável VerificarOperadoraExiste
                    using (var checarOperadora = new SqlCommand(OperatorRegistrationQueries.VerificarOperadoraExiste, con))
                    {
                        // Adiciona o valor do retorno do código da operadora (obtido por @CodigoOperadora), que é passado como parâmetro codigoOperadora
                        checarOperadora.Parameters.AddWithValue("@CodigoOperadora", codigoOperadora);
                        // Executa a query e obtém o resultado (quantidade de registros encontrados)
                        int count = Convert.ToInt32(checarOperadora.ExecuteScalar());
                        if (count == 0)
                            return false;
                    }

                    // Decide qual query usar baseado no que foi preenchido no campo
                    string sql;
                    bool temRazaoSocial = !string.IsNullOrWhiteSpace(razaoSocial);
                    bool temNomeComercial = !string.IsNullOrWhiteSpace(nomeComercial);

                    if (temRazaoSocial && temNomeComercial)
                    {
                        // Se tiver ambos, decide atualizar os dois campos
                        sql = OperatorRegistrationQueries.AtualizarOperadoraEdit;
                    }
                    else if (temRazaoSocial)
                    {
                        // Decide atualizar só Razão Social 
                        sql = OperatorRegistrationQueries.AtualizarRazaoSocial;
                    }
                    else
                    {
                        // Decide atualizar só Nome Comercial
                        sql = OperatorRegistrationQueries.AtualizarNomeComercial;
                    }

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        // Depois da decisão / preenchimento do(s) campo(s), executa a query para atualizar no banco Plennus, e atribui para a query
                        if (temRazaoSocial)
                            cmd.Parameters.AddWithValue("@RazaoSocial", razaoSocial.Trim());

                        if (temNomeComercial)
                            cmd.Parameters.AddWithValue("@NomeComercial", nomeComercial.Trim());

                        cmd.Parameters.AddWithValue("@CodPessoaAlteracao", ObterCodUsuarioLogado());
                        cmd.Parameters.AddWithValue("@DataLog", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CodigoOperadora", codigoOperadora);

                        int linhasAfetadas = cmd.ExecuteNonQuery();
                        return linhasAfetadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log do erro
                throw new Exception($"Erro ao atualizar operadora: {ex.Message}", ex);
            }
        }

        ////////////////////////////////// Obtém o código do usuário logado
        private int ObterCodUsuarioLogado()
        {
            // TODO: Implementar a lógica para pegar o usuário logado
            // Exemplo de implementação:
            // if (System.Web.HttpContext.Current?.Session?["CodUsuario"] != null)
            //     return Convert.ToInt32(System.Web.HttpContext.Current.Session["CodUsuario"]);
            // return 1; // Fallback

            // IMPORTANTE: Substitua isso pela sua lógica real de autenticação
            return 1; // Temporário - substituir pela implementação real
        }
    }
}