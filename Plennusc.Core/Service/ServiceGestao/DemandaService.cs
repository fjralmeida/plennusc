﻿using Microsoft.Data.Edm.Validation;
using Microsoft.Data.OData.Query.SemanticAst;
using Plennusc.Core.Mappers.MappersGestao;
using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Models.Utils;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.demanda;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Plennusc.Core.Service.ServiceGestao
{
    public class DemandaService
    {
        private readonly string _cs;

        public DemandaService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        private SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        public List<OptionItem> GetDepartamentoUsuario(int usuarioId)
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.DepartamentoUsuario, con))
            {
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }

        public List<OptionItem> GetDepartamentos()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.Departamentos, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
            }
            return list;
        }

        public List<OptionItem> GetPessoasAtivas()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.PessoasAtivas, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
            }
            return list;
        }

        public List<DemandaComLimite> GetPrioridadesComLimites()
        {
            var list = new List<DemandaComLimite>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.PrioridadesComLimites, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.NivelPrioridade);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new DemandaComLimite
                        {
                            Value = rd.GetInt32(0),
                            Text = rd.GetString(1),
                            Limite = rd.GetInt32(2)
                        });
                }
            }
            return list;
        }

        public bool AtualizarPrioridadeDemanda(int codDemanda, int novaPrioridade)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.AlterarPrioridadeDemanda, con))
            {
                cmd.Parameters.AddWithValue("@NovaPrioridade", novaPrioridade);
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public int ContarDemandasPorPrioridade(int codPessoa, int prioridade)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.ContarDemandasPorPrioridade, con))
            {
                cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);
                cmd.Parameters.AddWithValue("@Prioridade", prioridade);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Use a consulta ORIGINAL que funciona com sua tabela atual
        public void SalvarAnexoDemanda(int codDemanda, string nomeArquivo, byte[] conteudo, string contentType, int codPessoa)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.SalvarAnexoDemanda, con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                cmd.Parameters.AddWithValue("@DescArquivo", nomeArquivo);

                cmd.ExecuteNonQuery();
            }
        }
        public bool AceitarDemanda(int codDemanda, int codPessoa)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.AceitarDemanda, con))
            {
                cmd.Parameters.AddWithValue("@CodPessoaExecucao", codPessoa);
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<DemandaListDto> GetDemandasParaListagem(int? codPessoaFiltro = null)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.GetDemandasParaListagem, con))
            {
                if (codPessoaFiltro.HasValue)
                    cmd.Parameters.AddWithValue("@CodPessoaFiltro", codPessoaFiltro.Value);
                else
                    cmd.Parameters.AddWithValue("@CodPessoaFiltro", DBNull.Value);

                con.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    var result = new List<DemandaListDto>();
                    while (reader.Read())
                    {
                        result.Add(new DemandaListDto
                        {
                            CodDemanda = reader.GetInt32(0),
                            Titulo = reader.GetString(1),
                            Categoria = reader.GetString(2),
                            Subtipo = reader.GetString(3),
                            CodPrioridade = reader.GetInt32(4),
                            Prioridade = reader.GetString(5),
                            Status = reader.GetString(6),
                            Solicitante = reader.GetString(7),
                            DataSolicitacao = reader.GetDateTime(8),

                            // NOVOS CAMPOS - Importância (posições 9 e 10)
                            Importancia = reader.IsDBNull(9) ? "Não definida" : reader.GetString(9),
                            CodImportancia = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),

                            // Campos existentes AGORA nas posições 11, 12, 13
                            CodPessoaExecucao = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                            DataAceitacao = reader.IsDBNull(12) ? (DateTime?)null : reader.GetDateTime(12),
                            NomePessoaExecucao = reader.IsDBNull(13) ? null : reader.GetString(13)
                        });
                    }
                    return result;
                }
            }
        }
        public string SalvarAnexoFisico(HttpPostedFile arquivo, int codDemanda)
        {
            try
            {
                // CAMINHO CORRETO - com /public/
                string pastaAnexos = HttpContext.Current.Server.MapPath("~/public/uploadgestao/docs/");
                System.Diagnostics.Debug.WriteLine($"Caminho da pasta: {pastaAnexos}");

                if (!Directory.Exists(pastaAnexos))
                {
                    System.Diagnostics.Debug.WriteLine("Criando pasta...");
                    Directory.CreateDirectory(pastaAnexos);
                }

                // Gera um nome único para o arquivo
                string nomeUnico = $"{codDemanda}_{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(arquivo.FileName)}";
                string caminhoCompleto = Path.Combine(pastaAnexos, nomeUnico);

                System.Diagnostics.Debug.WriteLine($"Salvando arquivo em: {caminhoCompleto}");

                // Salva o arquivo
                arquivo.SaveAs(caminhoCompleto);

                System.Diagnostics.Debug.WriteLine("Arquivo salvo com sucesso!");
                return nomeUnico;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO ao salvar arquivo: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw; // Re-lança a exception para ver o erro na interface
            }
        }

        public List<OptionItem> GetSituacoesParaFechamento()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.SituacoesParaFechamento, con))
            {
                cmd.Parameters.AddWithValue("@TipoSituacao", EstruturaTipos.SituacaoDemanda);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem
                        {
                            Value = rd.GetInt32(0),
                            Text = rd.GetString(1)
                        });
                }
            }
            return list;
        }

        public List<DemandaCriticaInfo> GetDemandasCriticasAbertas(int codPessoa)
        {
            var list = new List<DemandaCriticaInfo>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.DemandasCriticasAbertas, con))
            {
                cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new DemandaCriticaInfo
                        {
                            CodDemanda = rd.GetInt32(0),
                            Titulo = rd.GetString(1),
                            DataDemanda = rd.GetDateTime(2),
                            Situacao = rd.GetString(3)
                        });
                }
            }
            return list;
        }

        public List<DemandaCriticaInfo> GetDemandasAltasAbertas(int codPessoa)
        {
            var list = new List<DemandaCriticaInfo>();
            using (var con = Open())
            using (var cmd = new SqlCommand(@"
        SELECT 
            d.CodDemanda,
            d.Titulo,
            d.DataDemanda,
            s.DescEstrutura AS Situacao
        FROM dbo.Demanda d
        INNER JOIN dbo.Estrutura s ON d.CodEstr_SituacaoDemanda = s.CodEstrutura
        WHERE d.CodPessoaSolicitacao = @CodPessoa
          AND d.CodEstr_NivelPrioridade = 32 -- Alta
          AND d.CodEstr_SituacaoDemanda IN (17, 18, 23)
        ORDER BY d.DataDemanda DESC", con))
            {
                cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new DemandaCriticaInfo
                        {
                            CodDemanda = rd.GetInt32(0),
                            Titulo = rd.GetString(1),
                            DataDemanda = rd.GetDateTime(2),
                            Situacao = rd.GetString(3)
                        });
                }
            }
            return list;
        }

        public List<OptionItem> GetNiveisImportancia()
        {
            var list = new List<OptionItem>();

            try
            {
                using (var con = Open())
                using (var cmd = new SqlCommand(Demanda.SelectNiveisImportancia, con))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new OptionItem
                        {
                            Value = rd.GetInt32(0),
                            Text = rd.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Falha silenciosa - importância é opcional
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar níveis de importância: {ex.Message}");
            }

            return list;
        }

        public int ObterPrioridadeDemanda(int codDemanda)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand("SELECT CodEstr_NivelPrioridade FROM dbo.Demandas WHERE CodDemanda = @CodDemanda", con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        public bool AlterarSituacaoDemanda(int codDemanda, int novaSituacao, int codPessoa)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(@"
                UPDATE dbo.Demandas 
                SET CodEstr_SituacaoDemanda = @NovaSituacao
                WHERE CodDemanda = @CodDemanda 
                AND CodPessoaSolicitacao = @CodPessoa", con))
            {
                cmd.Parameters.AddWithValue("@NovaSituacao", novaSituacao);
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ---------- Categoria/Subtipo ----------
        public List<OptionItem> GetTiposDemandaGrupos()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.TiposDemandaGrupos, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.TipoDemanda); // 6
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }

        public List<OptionItem> GetSubtiposDemanda(int codGrupo)
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.SubtiposPorGrupo, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.TipoDemanda); // 6
                cmd.Parameters.AddWithValue("@Pai", codGrupo);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }


        // (opcional) checar se um tipo está permitido para um setor (tabela SetorTipoDemanda)
        public bool TipoPermitidoParaSetor(int codSetor, int codEstrTipoDemanda)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.TiposPermitidosPorSetor, con))
            {
                cmd.Parameters.AddWithValue("@Setor", codSetor);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        if (rd.GetInt32(0) == codEstrTipoDemanda)
                            return true;
                    }
                }
            }
            return false;
        }

        // ---------- Situação inicial ----------
        public int ResolverSituacaoAberta()
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.SituacaoAbertaPorTipo, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.SituacaoDemanda); // 5
                var o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
            }
        }

        // ---------- Criar Demanda ----------
        public int CriarDemanda(DemandaCreate dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var codSituacaoInicial = ResolverSituacaoAberta();
            if (codSituacaoInicial == 0)
                throw new Exception("Não foi possível localizar a situação inicial 'ABERTA'.");

            using (var con = Open())
            using (var tx = con.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    // Upsert no vínculo Setor x Tipo (grava o tipo que veio do form — subtipo ou grupo)
                    using (var cmdVinc = new SqlCommand(Demanda.UpsertSetorTipoDemanda, con, tx))
                    {
                        cmdVinc.Parameters.AddWithValue("@CodSetor", dto.CodSetorDestino);
                        cmdVinc.Parameters.AddWithValue("@CodEstr_TipoDemanda", dto.CodEstr_TipoDemanda);
                        cmdVinc.ExecuteNonQuery();
                    }

                    int novoId;
                    using (var cmd = new SqlCommand(Demanda.InsertDemanda, con, tx))
                    {
                        cmd.Parameters.AddWithValue("@CodPessoaSolicitacao", dto.CodPessoaSolicitacao);
                        cmd.Parameters.AddWithValue("@CodSetorOrigem", dto.CodSetorOrigem);
                        cmd.Parameters.AddWithValue("@CodSetorDestino", dto.CodSetorDestino);
                        cmd.Parameters.AddWithValue("@CodEstr_TipoDemanda", dto.CodEstr_TipoDemanda);
                        cmd.Parameters.AddWithValue("@CodEstr_SituacaoDemanda", codSituacaoInicial);
                        cmd.Parameters.AddWithValue("@CodEstr_NivelPrioridade", dto.CodEstr_NivelPrioridade);
                        cmd.Parameters.AddWithValue("@Titulo", dto.Titulo ?? "");
                        cmd.Parameters.AddWithValue("@TextoDemanda", dto.TextoDemanda ?? "");
                        cmd.Parameters.AddWithValue("@Conf_RequerAprovacao", dto.Conf_RequerAprovacao);

                        // MODIFICAÇÃO: Se não tem aprovador específico, usa NULL
                        if (dto.CodPessoaAprovacao.HasValue && dto.CodPessoaAprovacao > 0)
                        {
                            cmd.Parameters.AddWithValue("@CodPessoaAprovacao", dto.CodPessoaAprovacao.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CodPessoaAprovacao", DBNull.Value);
                        }

                        var pPrazo = cmd.Parameters.Add("@DataPrazoMaximo", SqlDbType.DateTime);
                        pPrazo.Value = (object)dto.DataPrazoMaximo ?? DBNull.Value;

                        // NOVO PARÂMETRO ADICIONADO: Nível de Importância
                        var pImportancia = cmd.Parameters.Add("@CodEstr_NivelImportancia", SqlDbType.Int);
                        pImportancia.Value = (object)dto.CodEstr_NivelImportancia ?? DBNull.Value;

                        novoId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    using (var cmdHist = new SqlCommand(Demanda.InsertHistoricoInicial, con, tx))
                    {
                        cmdHist.Parameters.AddWithValue("@CodDemanda", novoId);
                        cmdHist.Parameters.AddWithValue("@CodEstr_SituacaoDemanda", codSituacaoInicial);
                        cmdHist.Parameters.AddWithValue("@CodPessoaAlteracao", dto.CodPessoaSolicitacao);
                        cmdHist.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return novoId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
        public List<OptionItem> GetSituacoesDemanda()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.SituacoesDemanda, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.SituacaoDemanda); // 5
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }

        public List<OptionItem> GetCategoriasDemanda()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.CategoriasDemanda, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.TipoDemanda); // 6
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }

        public List<DemandaListDto> ListarDemandas(DemandaFiltro filtro)
        {
            var lista = new List<DemandaListDto>();
            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;
                var sql = new StringBuilder(Demanda.ListarDemandasBase);

                System.Diagnostics.Debug.WriteLine("=== FILTRO APPLICADO ===");
                System.Diagnostics.Debug.WriteLine($"Pessoa: {filtro?.CodPessoa}, Setor: {filtro?.CodSetor}, Visibilidade: {filtro?.Visibilidade}");

                // LÓGICA CORRETA E SIMPLES:
                if (!string.IsNullOrWhiteSpace(filtro?.Visibilidade))
                {
                    var v = filtro.Visibilidade.Trim().ToUpperInvariant();

                    if (v == "M") // MINHAS DEMANDAS - só as que o usuário criou/participa
                    {
                        sql.Append(" AND (d.CodPessoaSolicitacao = @CodPessoa OR d.CodPessoaAprovacao = @CodPessoa OR EXISTS(SELECT 1 FROM dbo.DemandaAcompanhamento da WHERE da.CodDemanda = d.CodDemanda AND da.CodPessoaAcompanhamento = @CodPessoa))");
                        cmd.Parameters.AddWithValue("@CodPessoa", filtro.CodPessoa.Value);
                        System.Diagnostics.Debug.WriteLine("FILTRO: Apenas minhas demandas");
                    }
                    else // MEU SETOR - demandas destinadas ao meu setor + as que eu criei
                    {
                        sql.Append(" AND (d.CodSetorDestino = @CodSetor OR d.CodPessoaSolicitacao = @CodPessoa)");
                        cmd.Parameters.AddWithValue("@CodSetor", filtro.CodSetor.Value);
                        cmd.Parameters.AddWithValue("@CodPessoa", filtro.CodPessoa.Value);
                        System.Diagnostics.Debug.WriteLine("FILTRO: Demandas do meu setor + minhas");
                    }
                }
                else // Default: mostra tudo do setor destino + do usuário
                {
                    sql.Append(" AND (d.CodSetorDestino = @CodSetor OR d.CodPessoaSolicitacao = @CodPessoa)");
                    cmd.Parameters.AddWithValue("@CodSetor", filtro.CodSetor.Value);
                    cmd.Parameters.AddWithValue("@CodPessoa", filtro.CodPessoa.Value);
                    System.Diagnostics.Debug.WriteLine("FILTRO: Default (setor + minhas)");
                }

                // Resto dos filtros (status, categoria, etc)
                if (filtro?.CodStatus != null)
                {
                    sql.Append(" AND d.CodEstr_SituacaoDemanda = @CodStatus");
                    cmd.Parameters.AddWithValue("@CodStatus", filtro.CodStatus.Value);
                }
                if (filtro?.CodCategoria != null)
                {
                    sql.Append(" AND ( (EXISTS(SELECT 1 FROM dbo.Estrutura e WHERE e.CodEstrutura = d.CodEstr_TipoDemanda AND e.CodEstruturaPai = @CodCategoria)) OR (d.CodEstr_TipoDemanda = @CodCategoria) )");
                    cmd.Parameters.AddWithValue("@CodCategoria", filtro.CodCategoria.Value);
                }
                if (!string.IsNullOrWhiteSpace(filtro?.NomeSolicitante))
                {
                    sql.Append(" AND (p.Nome + ' ' + ISNULL(p.Sobrenome,'')) LIKE @Solicitante");
                    cmd.Parameters.AddWithValue("@Solicitante", "%" + filtro.NomeSolicitante.Trim() + "%");
                }

                sql.Append(@"
                     ORDER BY 
                         d.CodEstr_NivelPrioridade DESC,
                         d.DataPrazoMaximo ASC,
                         d.CodEstr_NivelImportancia DESC,
                         d.DataDemanda DESC
                ");
                cmd.CommandText = sql.ToString();

                // DEBUG
                System.Diagnostics.Debug.WriteLine("QUERY FINAL: " + cmd.CommandText);
                foreach (SqlParameter p in cmd.Parameters)
                {
                    System.Diagnostics.Debug.WriteLine($"{p.ParameterName}: {p.Value}");
                } 

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var dto = new DemandaListDto
                        {
                            CodDemanda = rd.GetInt32(0),
                            Titulo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                            Categoria = rd.IsDBNull(2) ? "" : rd.GetString(2),
                            Subtipo = rd.IsDBNull(3) ? null : rd.GetString(3),
                            Status = rd.IsDBNull(4) ? "" : rd.GetString(4),
                            Solicitante = rd.IsDBNull(5) ? "" : rd.GetString(5),
                            DataSolicitacao = rd.IsDBNull(6) ? DateTime.MinValue : rd.GetDateTime(6),

                            DataPrazo = rd.IsDBNull(7) ? (DateTime?)null : rd.GetDateTime(7),

                            Prioridade = rd.IsDBNull(8) ? "Normal" : rd.GetString(8),
                            CodPrioridade = rd.IsDBNull(9) ? 0 : rd.GetInt32(9),

                            Importancia = rd.IsDBNull(10) ? null : rd.GetString(10),
                            CodImportancia = rd.IsDBNull(11) ? (int?)null : rd.GetInt32(11),
                            CodPessoaExecucao = rd.IsDBNull(12) ? (int?)null : rd.GetInt32(12),
                            DataAceitacao = rd.IsDBNull(13) ? (DateTime?)null : rd.GetDateTime(13),
                            NomePessoaExecucao = rd.IsDBNull(14) ? null : rd.GetString(14)
                        };
                        lista.Add(dto);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Total de resultados: {lista.Count}");
            }
            return lista;
        }

        public DemandaDto ObterDemandaPorId(int codDemanda)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.ObterDemandaPorId, con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        // debug opcional: lista colunas retornadas (descomente para investigar)
                        // for (int i = 0; i < rd.FieldCount; i++)
                        //     System.Diagnostics.Debug.WriteLine($"COL[{i}] = {rd.GetName(i)}");

                        var dto = new DemandaDto
                        {
                            CodDemanda = rd.GetInt("CodDemanda", 0),
                            Titulo = rd.GetStringOrNull("Titulo"),
                            TextoDemanda = rd.GetStringOrNull("TextoDemanda"),

                            // status (nome + codigo)
                            StatusNome = rd.GetStringOrNull("StatusNome"),
                            StatusCodigo = rd.GetNullableInt("StatusCodigo"),

                            Solicitante = rd.GetStringOrNull("Solicitante"),
                            DataSolicitacao = rd.GetNullableDateTime("DataSolicitacao"),
                            CodPessoaSolicitacao = rd.GetInt("CodPessoaSolicitacao", 0),

                            // executor / aceite
                            CodPessoaExecucao = rd.GetNullableInt("CodPessoaExecucao"),
                            DataAceitacao = rd.GetNullableDateTime("DataAceitacao"),
                            NomePessoaExecucao = rd.GetStringOrNull("NomePessoaExecucao"),

                            // aprovador
                            CodPessoaAprovacao = rd.GetNullableInt("CodPessoaAprovacao"),

                            // setor destino
                            CodSetorDestino = rd.GetInt("CodSetorDestino", 0)
                        };

                        return dto;
                    }
                }
            }
            return null;
        }

        public IEnumerable<HistoricoDto> ObterHistorico(int codDemanda)
        {
            var list = new List<HistoricoDto>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.ObterHistorico, con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var anterior = rd.IsDBNull(rd.GetOrdinal("CodEstr_SituacaoDemandaAnterior")) ? (int?)null : rd.GetInt32(rd.GetOrdinal("CodEstr_SituacaoDemandaAnterior"));
                        var atual = rd.IsDBNull(rd.GetOrdinal("CodEstr_SituacaoDemandaAtual")) ? (int?)null : rd.GetInt32(rd.GetOrdinal("CodEstr_SituacaoDemandaAtual"));

                        list.Add(new HistoricoDto
                        {
                            CodDemandaHistorico = rd.IsDBNull(rd.GetOrdinal("CodDemandaHistorico")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodDemandaHistorico")),
                            CodDemanda = rd.IsDBNull(rd.GetOrdinal("CodDemanda")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodDemanda")),
                            CodEstr_SituacaoDemandaAnterior = anterior,
                            CodEstr_SituacaoDemandaAtual = atual,
                            SituacaoAnterior = rd.IsDBNull(rd.GetOrdinal("SituacaoAnterior")) ? null : rd.GetString(rd.GetOrdinal("SituacaoAnterior")),
                            SituacaoAtual = rd.IsDBNull(rd.GetOrdinal("SituacaoAtual")) ? null : rd.GetString(rd.GetOrdinal("SituacaoAtual")),
                            CodPessoaAlteracao = rd.IsDBNull(rd.GetOrdinal("CodPessoaAlteracao")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodPessoaAlteracao")),
                            Usuario = rd.IsDBNull(rd.GetOrdinal("Usuario")) ? null : rd.GetString(rd.GetOrdinal("Usuario")),
                            DataAlteracao = rd.IsDBNull(rd.GetOrdinal("DataAlteracao")) ? (DateTime?)null : rd.GetDateTime(rd.GetOrdinal("DataAlteracao"))
                        });
                    }
                }
            }
            return list;
        }

        public IEnumerable<AcompanhamentoDto> ObterAcompanhamentos(int codDemanda)
        {
            var list = new List<AcompanhamentoDto>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.ObterAcompanhamentos, con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new AcompanhamentoDto
                        {
                            CodDemandaAcompanhamento = rd.IsDBNull(rd.GetOrdinal("CodDemandaAcompanhamento")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodDemandaAcompanhamento")),
                            CodDemanda = rd.IsDBNull(rd.GetOrdinal("CodDemanda")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodDemanda")),
                            TextoAcompanhamento = rd.IsDBNull(rd.GetOrdinal("TextoAcompanhamento")) ? null : rd.GetString(rd.GetOrdinal("TextoAcompanhamento")),
                            DataAcompanhamento = rd.IsDBNull(rd.GetOrdinal("DataAcompanhamento")) ? (DateTime?)null : rd.GetDateTime(rd.GetOrdinal("DataAcompanhamento")),
                            CodPessoaAcompanhamento = rd.IsDBNull(rd.GetOrdinal("CodPessoaAcompanhamento")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodPessoaAcompanhamento")),
                            Autor = rd.IsDBNull(rd.GetOrdinal("Autor")) ? null : rd.GetString(rd.GetOrdinal("Autor"))
                        });
                    }
                }
            }
            return list;
        }

        public void AdicionarAcompanhamento(int codDemanda, int codPessoa, string texto)
        {
            const int STATUS_EM_ANDAMENTO = 18;
            const int STATUS_CONCLUIDA = 23;

            using (var con = Open())
            using (var tx = con.BeginTransaction())
            {
                try
                {
                    // 1) Inserir o acompanhamento
                    using (var cmdIns = new SqlCommand(Demanda.InsertAcompanhamento, con, tx))
                    {
                        cmdIns.Parameters.AddWithValue("@CodDemanda", codDemanda);
                        cmdIns.Parameters.AddWithValue("@TextoAcompanhamento", (object)(texto ?? string.Empty));
                        cmdIns.Parameters.AddWithValue("@CodPessoaAcompanhamento", codPessoa);
                        cmdIns.ExecuteNonQuery();
                    }

                    // 2) Ler executor e status atual da demanda
                    int? codPessoaExecucao = null;
                    int statusAtual = 0;
                    using (var cmdSel = new SqlCommand(Demanda.SelectDemandaExecutorStatus, con, tx))
                    {
                        cmdSel.Parameters.AddWithValue("@CodDemanda", codDemanda);
                        using (var rd = cmdSel.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                codPessoaExecucao = rd.IsDBNull(0) ? (int?)null : rd.GetInt32(0);
                                statusAtual = rd.IsDBNull(1) ? 0 : rd.GetInt32(1);
                            }
                        }
                    }

                    // 3) Se quem respondeu é o executor e o status não é já Em Andamento/Concluída -> atualiza
                    if (codPessoaExecucao.HasValue && codPessoaExecucao.Value == codPessoa
                        && statusAtual != STATUS_EM_ANDAMENTO
                        && statusAtual != STATUS_CONCLUIDA)
                    {
                        using (var cmdUpd = new SqlCommand(Demanda.UpdateSituacaoDemanda, con, tx))
                        {
                            cmdUpd.Parameters.AddWithValue("@NovoStatus", STATUS_EM_ANDAMENTO);
                            cmdUpd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                            cmdUpd.ExecuteNonQuery();
                        }

                        using (var cmdHist = new SqlCommand(Demanda.InsertDemandaHistorico, con, tx))
                        {
                            cmdHist.Parameters.AddWithValue("@CodDemanda", codDemanda);
                            cmdHist.Parameters.AddWithValue("@Anterior", statusAtual);
                            cmdHist.Parameters.AddWithValue("@Atual", STATUS_EM_ANDAMENTO);
                            cmdHist.Parameters.AddWithValue("@CodPessoa", codPessoa);
                            cmdHist.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }
                catch
                {
                    try { tx.Rollback(); } catch { /* swallow */ }
                    throw;
                }
            }
        }

        // NO SERVICE - Alterar para retornar o ID do acompanhamento
        public int InserirAcompanhamento(int codDemanda, int codPessoa, string texto)
        {
            const int STATUS_EM_ANDAMENTO = 18;
            const int STATUS_CONCLUIDA = 23;

            using (var con = Open())
            using (var tx = con.BeginTransaction())
            {
                try
                {
                    int idAcompanhamento;

                    // 1) Inserir o acompanhamento e RETORNAR O ID
                    using (var cmdIns = new SqlCommand(Demanda.InsertAcompanhamento + "; SELECT SCOPE_IDENTITY();", con, tx))
                    {
                        cmdIns.Parameters.AddWithValue("@CodDemanda", codDemanda);
                        cmdIns.Parameters.AddWithValue("@TextoAcompanhamento", (object)(texto ?? string.Empty));
                        cmdIns.Parameters.AddWithValue("@CodPessoaAcompanhamento", codPessoa);
                        idAcompanhamento = Convert.ToInt32(cmdIns.ExecuteScalar());
                    }

                    // 2) Ler executor e status atual da demanda (seu código existente)
                    int? codPessoaExecucao = null;
                    int statusAtual = 0;
                    using (var cmdSel = new SqlCommand(Demanda.SelectDemandaExecutorStatus, con, tx))
                    {
                        cmdSel.Parameters.AddWithValue("@CodDemanda", codDemanda);
                        using (var rd = cmdSel.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                codPessoaExecucao = rd.IsDBNull(0) ? (int?)null : rd.GetInt32(0);
                                statusAtual = rd.IsDBNull(1) ? 0 : rd.GetInt32(1);
                            }
                        }
                    }

                    // 3) Sua lógica existente de atualização de status
                    if (codPessoaExecucao.HasValue && codPessoaExecucao.Value == codPessoa
                        && statusAtual != STATUS_EM_ANDAMENTO
                        && statusAtual != STATUS_CONCLUIDA)
                    {
                        using (var cmdUpd = new SqlCommand(Demanda.UpdateSituacaoDemanda, con, tx))
                        {
                            cmdUpd.Parameters.AddWithValue("@NovoStatus", STATUS_EM_ANDAMENTO);
                            cmdUpd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                            cmdUpd.ExecuteNonQuery();
                        }

                        using (var cmdHist = new SqlCommand(Demanda.InsertDemandaHistorico, con, tx))
                        {
                            cmdHist.Parameters.AddWithValue("@CodDemanda", codDemanda);
                            cmdHist.Parameters.AddWithValue("@CodEstr_SituacaoDemandaAnterior", statusAtual); 
                            cmdHist.Parameters.AddWithValue("@CodEstr_SituacaoDemandaAtual", STATUS_EM_ANDAMENTO); 
                            cmdHist.Parameters.AddWithValue("@CodPessoaAlteracao", codPessoa); 
                            cmdHist.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                    return idAcompanhamento; // RETORNA O ID
                }
                catch
                {
                    try { tx.Rollback(); } catch { /* swallow */ }
                    throw;
                }
            }
        }
        public void SalvarAnexoAcompanhamento(int codDemanda, int codDemandaAcompanhamento, string descArquivo)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(@"
        INSERT INTO DemandaAnexos (CodDemanda, CodDemandaAcompanhamento, DescArquivo, DataEnvio) 
        VALUES (@CodDemanda, @CodDemandaAcompanhamento, @DescArquivo, GETDATE())", con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                cmd.Parameters.AddWithValue("@CodDemandaAcompanhamento", codDemandaAcompanhamento);
                cmd.Parameters.AddWithValue("@DescArquivo", descArquivo);
                cmd.ExecuteNonQuery();
            }
        }

        //SOLICITAÇÃO DE APROVAÇÃO DE DEMANDA
        public bool SolicitarAprovacaoDemanda(int codDemanda, int codPessoaSolicitante, out int gestorDesignado)
        {
            gestorDesignado = 0;

            using (var con = Open())
            using (var tx = con.BeginTransaction())
            {
                try
                {
                    int? codSetorDestino = null;
                    int statusAnterior = 0;
                    int statusAtual = 0;
                    int? codPessoaAprovacaoExistente = null;

                    // 1) Buscar setor, status e aprovador atual
                    using (var cmd = new SqlCommand(Demanda.SelectDemanda_Setor_Status, con, tx))
                    {
                        cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                codSetorDestino = rd.HasColumn("CodSetorDestino") && !rd.IsDBNull(rd.GetOrdinal("CodSetorDestino"))
                                    ? (int?)rd.GetInt32(rd.GetOrdinal("CodSetorDestino"))
                                    : null;

                                statusAnterior = rd.HasColumn("CodEstr_SituacaoDemanda") && !rd.IsDBNull(rd.GetOrdinal("CodEstr_SituacaoDemanda"))
                                    ? rd.GetInt32(rd.GetOrdinal("CodEstr_SituacaoDemanda"))
                                    : 0;

                                statusAtual = statusAnterior;

                                codPessoaAprovacaoExistente = rd.HasColumn("CodPessoaAprovacao") && !rd.IsDBNull(rd.GetOrdinal("CodPessoaAprovacao"))
                                    ? (int?)rd.GetInt32(rd.GetOrdinal("CodPessoaAprovacao"))
                                    : null;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                            }
                        }
                    }

                    // Se já tem aprovador
                    if (codPessoaAprovacaoExistente.HasValue && codPessoaAprovacaoExistente.Value > 0)
                    {
                        gestorDesignado = codPessoaAprovacaoExistente.Value;
                        tx.Commit();
                        return true;
                    }

                    if (!codSetorDestino.HasValue)
                    {
                        tx.Rollback();
                        return false;
                    }

                    // 2) Buscar gestor do setor
                    int gestorId = 0;
                    using (var cmd = new SqlCommand(Demanda.SelectGestorPorDepartamento, con, tx))
                    {
                        cmd.Parameters.AddWithValue("@CodDepartamento", codSetorDestino.Value);
                        var o = cmd.ExecuteScalar();
                        if (o != null && o != DBNull.Value)
                            gestorId = Convert.ToInt32(o);
                    }

                    if (gestorId == 0)
                    {
                        tx.Rollback();
                        return false;
                    }

                    // 3) Atualizar aprovador
                    using (var cmd = new SqlCommand(Demanda.UpdateDemanda_SetAprovador, con, tx))
                    {
                        cmd.Parameters.AddWithValue("@CodPessoaAprovacao", gestorId);
                        cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                        cmd.ExecuteNonQuery();
                    }

                    // CORREÇÃO: Usar o código fixo 65 para "Aguardando Aprovação"
                    int codSituacaoAguardando = 65; // Código direto da sua tabela

                    // 4) Atualizar a situação da Demanda para 'Aguardando aprovação'
                    using (var cmd = new SqlCommand(Demanda.UpdateSituacaoDemanda, con, tx))
                    {
                        cmd.Parameters.AddWithValue("@CodEstr_SituacaoDemanda", codSituacaoAguardando);
                        cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                        cmd.ExecuteNonQuery();
                    }

                    // Ajusta statusAtual para o histórico
                    statusAtual = codSituacaoAguardando;

                    // 5) Inserir no histórico
                    using (var cmd = new SqlCommand(Demanda.InsertDemandaHistorico, con, tx))
                    {
                        cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                        cmd.Parameters.AddWithValue("@CodEstr_SituacaoDemandaAnterior", statusAnterior);
                        cmd.Parameters.AddWithValue("@CodEstr_SituacaoDemandaAtual", statusAtual);
                        cmd.Parameters.AddWithValue("@CodPessoaAlteracao", codPessoaSolicitante);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    gestorDesignado = gestorId;
                    return true;
                }
                catch
                {
                    try { tx.Rollback(); } catch { }
                    throw;
                }
            }
        }



        public void AtualizarStatusComHistorico(int codDemanda, int novoStatusCodigo, int codPessoaAlteracao)
        {
            using (var con = Open())
            {
                // 1. PEGA O STATUS ATUAL
                int codStatusAtual = 0;
                using (var cmdStatus = new SqlCommand("SELECT CodEstr_SituacaoDemanda FROM Demanda WHERE CodDemanda = @CodDemanda", con))
                {
                    cmdStatus.Parameters.AddWithValue("@CodDemanda", codDemanda);
                    var resultado = cmdStatus.ExecuteScalar();
                    if (resultado != null && resultado != DBNull.Value)
                        codStatusAtual = Convert.ToInt32(resultado);
                }

                // 2. ATUALIZA O STATUS
                using (var cmdUpdate = new SqlCommand("UPDATE Demanda SET CodEstr_SituacaoDemanda = @NovoStatus WHERE CodDemanda = @CodDemanda", con))
                {
                    cmdUpdate.Parameters.AddWithValue("@NovoStatus", novoStatusCodigo);
                    cmdUpdate.Parameters.AddWithValue("@CodDemanda", codDemanda);
                    cmdUpdate.ExecuteNonQuery();
                }

                // 3. REGISTRA NO HISTÓRICO
                using (var cmdHist = new SqlCommand("INSERT INTO DemandaHistorico (CodDemanda, CodEstr_SituacaoDemandaAnterior, CodEstr_SituacaoDemandaAtual, CodPessoaAlteracao, DataAlteracao) VALUES (@CodDemanda, @CodAnterior, @CodAtual, @CodPessoa, GETDATE())", con))
                {
                    cmdHist.Parameters.AddWithValue("@CodDemanda", codDemanda);
                    cmdHist.Parameters.AddWithValue("@CodAnterior", codStatusAtual);
                    cmdHist.Parameters.AddWithValue("@CodAtual", novoStatusCodigo);
                    cmdHist.Parameters.AddWithValue("@CodPessoa", codPessoaAlteracao);
                    cmdHist.ExecuteNonQuery();
                }
            }
        }

        //ANEXOS DE UMA DEMANDA
        public List<AnexoInfo> GetAnexosDemanda(int codDemanda)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.GetAnexosDemanda, con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);

                var anexos = new List<AnexoInfo>();
                using (var rd = cmd.ExecuteReader())
                {
                    int idxCod = rd.GetOrdinal("CodDemandaAnexo");
                    int idxNome = rd.GetOrdinal("DescArquivo"); 
                    int idxData = rd.GetOrdinal("DataEnvio");
                    int idxNomeUsuario = rd.GetOrdinal("NomeUsuario");

                    while (rd.Read())
                    {
                        anexos.Add(new AnexoInfo
                        {
                            CodAnexo = rd["CodDemandaAnexo"] != DBNull.Value ? Convert.ToInt32(rd["CodDemandaAnexo"]) : 0,
                            NomeArquivo = rd["DescArquivo"] != DBNull.Value ? rd["DescArquivo"].ToString() : string.Empty,
                            DataEnvio = rd["DataEnvio"] != DBNull.Value ? (DateTime)rd["DataEnvio"] : DateTime.MinValue,
                            TamanhoBytes = 0,
                            NomeUsuarioUpload = rd["NomeUsuario"] != DBNull.Value ? rd["NomeUsuario"].ToString() : "Sistema"
                        });
                    }
                }
                return anexos;
            }
        }
        //DEMANDAS EM ABERTO PARA PESSOA (MINHAS DEMANDAS)
        public List<DemandaInfo> GetDemandasEmAbertoPorPessoa(int codPessoa)
        {
            var demandas = new List<DemandaInfo>();

            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.DemandasEmAbertoPorPessoa, con))
            {
                cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // pega ordinais apenas uma vez
                        int oCodDemanda = reader.GetOrdinal("CodDemanda");
                        int oTitulo = reader.GetOrdinal("Titulo");
                        int oCategoria = reader.GetOrdinal("Categoria");
                        int oStatus = reader.GetOrdinal("Status");
                        int oSolicitante = reader.GetOrdinal("Solicitante");
                        int oDataSolicitacao = reader.GetOrdinal("DataSolicitacao");
                        int oDataPrazo = reader.GetOrdinal("DataPrazo");
                        int oPrioridade = reader.GetOrdinal("Prioridade");
                        int oCodPrioridade = reader.GetOrdinal("CodPrioridade");
                        int oImportancia = reader.GetOrdinal("Importancia");
                        int oCodImportancia = reader.GetOrdinal("CodImportancia");
                        int oCodPessoaExecucao = reader.GetOrdinal("CodPessoaExecucao");
                        int oDataAceitacao = reader.GetOrdinal("DataAceitacao");
                        int oNomePessoaExecucao = reader.GetOrdinal("NomePessoaExecucao");

                        var di = new DemandaInfo
                        {
                            CodDemanda = reader.IsDBNull(oCodDemanda) ? 0 : reader.GetInt32(oCodDemanda),
                            Titulo = reader.IsDBNull(oTitulo) ? string.Empty : reader.GetString(oTitulo),
                            Categoria = reader.IsDBNull(oCategoria) ? string.Empty : reader.GetString(oCategoria),
                            Status = reader.IsDBNull(oStatus) ? string.Empty : reader.GetString(oStatus),
                            Solicitante = reader.IsDBNull(oSolicitante) ? string.Empty : reader.GetString(oSolicitante),

                            // DataSolicitacao: se pode ser NULL no seu BD, troque o tipo na DemandaInfo para DateTime?
                            DataSolicitacao = reader.IsDBNull(oDataSolicitacao) ? DateTime.MinValue : reader.GetDateTime(oDataSolicitacao),

                            // novo campo Prazo (nullable)
                            DataPrazo = reader.IsDBNull(oDataPrazo) ? (DateTime?)null : reader.GetDateTime(oDataPrazo),

                            Prioridade = reader.IsDBNull(oPrioridade) ? string.Empty : reader.GetString(oPrioridade),
                            CodPrioridade = reader.IsDBNull(oCodPrioridade) ? 0 : reader.GetInt32(oCodPrioridade),

                            Importancia = reader.IsDBNull(oImportancia) ? null : reader.GetString(oImportancia),
                            CodImportancia = reader.IsDBNull(oCodImportancia) ? (int?)null : reader.GetInt32(oCodImportancia),

                            CodPessoaExecucao = reader.IsDBNull(oCodPessoaExecucao) ? (int?)null : reader.GetInt32(oCodPessoaExecucao),
                            DataAceitacao = reader.IsDBNull(oDataAceitacao) ? (DateTime?)null : reader.GetDateTime(oDataAceitacao),
                            NomePessoaExecucao = reader.IsDBNull(oNomePessoaExecucao) ? null : reader.GetString(oNomePessoaExecucao),
                        };

                        demandas.Add(di);
                    }
                }
            }

            return demandas;
        }


        //DEMANDAS EM ANDAMENTO PARA PESSOA (MINHAS DEMANDAS)
        public List<DemandaInfo> GetDemandasEmAndamentoPorPessoa(int codPessoa)
        {
            var demandas = new List<DemandaInfo>();

            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.DemandasEmAndamentoPorPessoa, con))
            {
                cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // ordinais uma vez
                        int oCodDemanda = reader.GetOrdinal("CodDemanda");
                        int oTitulo = reader.GetOrdinal("Titulo");
                        int oCategoria = reader.GetOrdinal("Categoria");
                        int oStatus = reader.GetOrdinal("Status");
                        int oSolicitante = reader.GetOrdinal("Solicitante");
                        int oDataSolicitacao = reader.GetOrdinal("DataSolicitacao");
                        int oDataPrazo = reader.GetOrdinal("DataPrazo"); 
                        int oPrioridade = reader.GetOrdinal("Prioridade");
                        int oCodPrioridade = reader.GetOrdinal("CodPrioridade");
                        int oImportancia = reader.GetOrdinal("Importancia");
                        int oCodImportancia = reader.GetOrdinal("CodImportancia");
                        int oCodPessoaExecucao = reader.GetOrdinal("CodPessoaExecucao");
                        int oDataAceitacao = reader.GetOrdinal("DataAceitacao");
                        int oNomePessoaExecucao = reader.GetOrdinal("NomePessoaExecucao");

                        var di = new DemandaInfo
                        {
                            CodDemanda = reader.IsDBNull(oCodDemanda) ? 0 : reader.GetInt32(oCodDemanda),
                            Titulo = reader.IsDBNull(oTitulo) ? string.Empty : reader.GetString(oTitulo),
                            Categoria = reader.IsDBNull(oCategoria) ? string.Empty : reader.GetString(oCategoria),
                            Status = reader.IsDBNull(oStatus) ? string.Empty : reader.GetString(oStatus),
                            Solicitante = reader.IsDBNull(oSolicitante) ? string.Empty : reader.GetString(oSolicitante),
                            DataSolicitacao = reader.IsDBNull(oDataSolicitacao) ? DateTime.MinValue : reader.GetDateTime(oDataSolicitacao),

                            // novos campos
                            DataPrazo = reader.IsDBNull(oDataPrazo) ? (DateTime?)null : reader.GetDateTime(oDataPrazo),
                            Importancia = reader.IsDBNull(oImportancia) ? null : reader.GetString(oImportancia),
                            CodImportancia = reader.IsDBNull(oCodImportancia) ? (int?)null : reader.GetInt32(oCodImportancia),

                            Prioridade = reader.IsDBNull(oPrioridade) ? string.Empty : reader.GetString(oPrioridade),
                            CodPrioridade = reader.IsDBNull(oCodPrioridade) ? 0 : reader.GetInt32(oCodPrioridade),

                            CodPessoaExecucao = reader.IsDBNull(oCodPessoaExecucao) ? (int?)null : reader.GetInt32(oCodPessoaExecucao),
                            DataAceitacao = reader.IsDBNull(oDataAceitacao) ? (DateTime?)null : reader.GetDateTime(oDataAceitacao),
                            NomePessoaExecucao = reader.IsDBNull(oNomePessoaExecucao) ? null : reader.GetString(oNomePessoaExecucao),
                        };

                        demandas.Add(di);
                    }
                }
            }

            return demandas;
        }
    }
}