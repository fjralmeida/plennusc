using Plennusc.Core.Models.ModelsGestao.modelsOperator;     // ✅ único using para DTOs
using Plennusc.Core.SqlQueries.SqlQueriesGestao.operatorRegistration;             // OperatorRegistrationQueries
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

            var agora = DateTime.Now;

            using (var con = OpenPlennus())
            using (var tran = con.BeginTransaction())
            {
                try
                {
                    foreach (var op in pendentes)
                    {
                        using (var cmd = new SqlCommand(OperatorRegistrationQueries.InserirOperadora, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@RegistroANS", (object)op.RegistroANS ?? DBNull.Value);
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

        public string BuscarNomePessoa(int codPessoa)
        {
            using (var con = OpenPlennus())
            using (var cmd = new SqlCommand(OperatorRegistrationQueries.BuscarNomePessoa, con))
            {
                cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);

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
    }
}