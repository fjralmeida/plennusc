using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Models.ModelsMedic;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.import;
using Plennusc.Core.SqlQueries.SqlQueriesMedic.import;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscMedic.Views
{
    public partial class csvImportPlaniumMedic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected List<DadosMensagensCsvPlaniumMedic> DadosCsv
        {
            get => (List<DadosMensagensCsvPlaniumMedic>)Session["CsvImportado"];
            set => Session["CsvImportado"] = value;
        }

        protected void btnLerCsv_Click(object sender, EventArgs e)
        {
            if (fileUploadCsv.HasFile)
            {
                var dados = ObterLinhasSelecionadasFixo(fileUploadCsv.FileContent);
                DadosCsv = dados;

                gridCsv.DataSource = dados;
                gridCsv.DataBind();

                btnEnviar.Enabled = gridCsv.Rows.Count > 0;
            }
        }
        protected List<DadosMensagensCsvPlaniumMedic> ObterLinhasSelecionadasFixo(Stream csvStream)
        {
            var lista = new List<DadosMensagensCsvPlaniumMedic>();

            using (var reader = new StreamReader(csvStream, Encoding.UTF8))
            {
                string linha;
                string[] cabecalho = null;
                char delimitador = '\t'; // padrão: TAB

                if (!reader.EndOfStream)
                {
                    linha = reader.ReadLine();

                    // Detecta delimitador com base na primeira linha
                    if (linha.Contains(";")) delimitador = ';';
                    else if (linha.Contains(",")) delimitador = ',';

                    cabecalho = linha.Split(delimitador);
                }

                while (!reader.EndOfStream)
                {
                    linha = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(linha)) continue;

                    if (linha.IndexOf("E+", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    linha.IndexOf("E-", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "ErroNotacao",
                            "Swal.fire('Erro!', 'O arquivo contém valores em notação científica. Por favor, rebaixe o CSV e não abra no Excel.', 'error');", true);
                        return new List<DadosMensagensCsvPlaniumMedic>();
                    }

                    var colunas = linha.Split(delimitador);
                    if (cabecalho == null || cabecalho.Length == 0) continue;

                    var dado = new DadosMensagensCsvPlaniumMedic();

                    for (int i = 0; i < cabecalho.Length; i++)
                    {
                        string nomeCampo = cabecalho[i].Trim();

                        // Remove acentos e caracteres especiais
                        nomeCampo = nomeCampo.Normalize(System.Text.NormalizationForm.FormD);
                        nomeCampo = new string(nomeCampo
                            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                            .ToArray());
                        nomeCampo = nomeCampo.Replace(" ", "_").Replace("-", "_");
                        nomeCampo = System.Text.RegularExpressions.Regex.Replace(nomeCampo, @"[^\w]", "_");

                        string valor = colunas.ElementAtOrDefault(i)?.Trim();

                        // Remover aspas duplas, se existirem
                        if (valor.StartsWith("\"") && valor.EndsWith("\""))
                        {
                            valor = valor.Substring(1, valor.Length - 2); // Remove as aspas duplas
                        }
                        // Detecta se é valor com notação científica
                        if (valor.IndexOf('E') > 0 || valor.IndexOf('e') > 0)
                        {
                            try
                            {
                                // Tenta converter como decimal para preservar precisão e zeros
                                decimal d = decimal.Parse(valor, NumberStyles.Float, CultureInfo.InvariantCulture);
                                valor = d.ToString("0", CultureInfo.InvariantCulture); // Converte com todos os dígitos
                            }
                            catch
                            {
                                // Se não conseguir converter, mantém valor original em notação científica
                                Console.WriteLine($"Valor em notação científica mantido: {valor}");
                            }
                        }

                        var prop = typeof(DadosMensagensCsvPlaniumMedic).GetProperty(nomeCampo);
                        if (prop != null && prop.CanWrite)
                        {
                            prop.SetValue(dado, valor);
                        }
                    }
                    lista.Add(dado);
                }
            }
            return lista;
        }

        #region TENTATIVA DE IMPORTAÇÃO PARA O BANCO DO ALIANCA
        //protected async void btnEnviar_Click(object sender, EventArgs e)
        //{
        //    importCVSPlanniumMedic insertCsv = new importCVSPlanniumMedic();
        //    if (DadosCsv == null || DadosCsv.Count == 0)
        //        return;
        //    // Buscar o menor código apenas uma vez
        //    int proximoCodigo = insertCsv.BuscarProximoCodigoAssociado() - 1;

        //    foreach (var item in DadosCsv)
        //    {
        //        item.CODIGO_ASSOCIADO = proximoCodigo.ToString();
        //        proximoCodigo++; // Vai para 345, 346, etc.

        //        var nome = item.NOME; // TMP1000.NET -> NOME_ASSOCIADO
        //        var id = item.ID; // TMP1000.NET -> NUMERO_RG
        //        var orgExp = item.ORG_EXP; // TMP1000.NET -> ORGAO_EMISSOR_RG
        //        var cpf = item.CPF; // TMP1000.NET -> NUMERO_CPF
        //        var cns = item.CNS; // TMP1000.NET -> CODIGO_CNS
        //        var dtNasc = item.DT_NASC; // TMP1000.NET -> DATA_NASCIMENTO
        //        var sexo = item.SEXO; // TMP1000.NET -> SEXO
        //        var estCivil = item.EST_CIVIL; // TMP1000.NET -> CODIGO_ESTADO_CIVIL

        //        var tipoLogradouro = string.IsNullOrWhiteSpace(item.TIPO_LOGRADOURO) ? "Rua" : item.TIPO_LOGRADOURO.Trim();
        //        var rua = item.RUA?.Trim() ?? "";
        //        var numero = item.NUMERO_LOGRADOURO?.Trim() ?? "";
        //        var complemento = item.COMPLEMENTO?.Trim() ?? "";
        //        var endereco = $"{tipoLogradouro} {rua}, {numero} - {complemento}".Trim();
        //        item.RUA = endereco; // TMP1001 -> ENDERECO

        //        var bairro = item.BAIRRO; // TMP1001 -> BAIRRO
        //        var cidade = item.CIDADE; // TMP1001 -> CIDADE
        //        var uf = item.UF; // TMP1001 -> ESTADO
        //        var cep = item.CEP; // TMP1001 -> CEP

        //        item.TEL = $"{item.DDD_TEL?.Trim()}{item.TEL?.Trim()}"; // TMP1006 -> NUMERO_TELEFONE (Tipo 1)
        //        item.TEL_2 = $"{item.DDD_TEL_2?.Trim()}{item.TEL_2?.Trim()}"; // TMP1006 -> NUMERO_TELEFONE (Tipo 2)
        //        item.CELULAR = $"{item.DDD_CEL?.Trim()}{item.CELULAR?.Trim()}"; // TMP1006 -> NUMERO_TELEFONE (Tipo Celular)

        //        var codPlano = item.COD_PLANO; // TMP1000.NET -> CODIGO_PLANO
        //        var dtIncl = item.DT_INCL; // TMP1000.NET -> DATA_ADMISSAO
        //        var dtVigencia = item.DT_VIGENCIA; // TMP1000.NET -> DATA_VALIDA_CARENCIA
        //        var codEmpresa = item.COD_EMPRESA ?? "400"; // TMP1000.NET -> CODIGO_EMPRESA
        //        var codUnid = item.COD_UNID; // SEM DESTINO DEFINIDO
        //        var nomeMae = item.NOME_MAE; // TMP1000.NET -> NOME_MAE
        //        var email = item.EMAIL; // TMP1001 -> ENDERECO_EMAIL
        //        var codResp = item.COD_RESP; // TMP1000.NET -> CODIGO_TITULAR

        //        var parentMap = new Dictionary<string, string>
        //        {
        //            { "1", "5" },
        //            { "2", "3" },
        //            { "3", "11" },
        //            { "4", "6" },
        //            { "5", "13" },
        //            { "6", "4" },
        //            { "7", "9" },
        //            { "8", "7" },
        //            { "9", "8" },
        //            { "10", "10" }
        //        };
        //        var parent = item.PARENT;
        //        if (string.IsNullOrWhiteSpace(parent))
        //        {
        //            parent = "2";
        //        }
        //        else if (parentMap.TryGetValue(parent, out var novoParent))
        //        {
        //            parent = novoParent;
        //        }
        //        item.PARENT = parent; // TMP1000.NET -> CODIGO_PARENTESCO

        //        var nrDecNascVivo = item.NR_DEC_NASC_VIVO; // TMP1000.NET -> NUMERO_DECLARACAO_NASC_VIVO
        //        var tipoMovimentacao = item.TIPO_MOVIMENTACAO; // SEM DESTINO DEFINIDO
        //        var codVend = item.COD_VEND; // TMP1000.NET -> CODIGO_VENDEDOR
        //        var codProp = item.COD_PROP; // SEM DESTINO DEFINIDO
        //        var diaVenc = item.DIA_VENC; // TMP1002 -> DIA_VENCIMENTO
        //        var responsavel = item.RESPONSAVEL; // SEM DESTINO DEFINIDO
        //        var codSuperv = item.COD_SUPERV; // SEM DESTINO DEFINIDO
        //        var codProfissao = item.COD_PROFISSAO; // TMP1000.NET -> PROFISSAO
        //        var cnpjOperadora = item.CNPJ_OPERADORA; // SEM DESTINO DEFINIDO
        //        var codAnsOperadora = item.COD_ANS_OPERADORA; // TMP1000.NET -> CODIGO_CADASTRO_ANS
        //        var nomeOperadora = item.NOME_OPERADORA; // SEM DESTINO DEFINIDO
        //        var idade = item.IDADE; // SEM DESTINO DEFINIDO
        //        var respfMae = item.RESPF_MAE; // SEM DESTINO DEFINIDO
        //        var respfNascimento = item.RESPF_NASCIMENTO; // SEM DESTINO DEFINIDO
        //        var respfEmail = item.RESPF_EMAIL; // SEM DESTINO DEFINIDO
        //        var respfIdade = item.RESPF_IDADE; // SEM DESTINO DEFINIDO
        //        var respfSexo = item.RESPF_SEXO; // SEM DESTINO DEFINIDO

        //        // Mapeamento de estado civil
        //        var estadoCivilMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        //        {
        //            { "casado", "3" },
        //            { "solteiro", "2" },
        //            { "viuvo", "8" },
        //            { "viúvo", "8" },
        //            { "divorciado", "5" },
        //            { "desquitado", "4" },
        //            { "marital", "6" },
        //            { "não informado", "7" },
        //            { "nao informado", "7" }
        //        };

        //        var estadoCivil = item.RESPF_ESTADOCIVIL?.Trim().ToLower();

        //        // Remove "(a)" se existir
        //        if (!string.IsNullOrEmpty(estadoCivil))
        //            estadoCivil = estadoCivil.Replace("(a)", "").Trim();

        //        // Aplica código numérico
        //        if (string.IsNullOrWhiteSpace(estadoCivil) || !estadoCivilMap.TryGetValue(estadoCivil, out var codigoEstadoCivil))
        //            item.RESPF_ESTADOCIVIL = "7";
        //        else
        //            item.RESPF_ESTADOCIVIL = codigoEstadoCivil;

        //        // Converte para smallint no banco
        //        item.RESPF_ESTADOCIVIL = Convert.ToInt16(item.RESPF_ESTADOCIVIL).ToString();

        //        // item.RESPF_ESTADOCIVIL -> SEM DESTINO DEFINIDO

        //        var respfRg = item.RESPF_RG; // SEM DESTINO DEFINIDO
        //        var respfOrgExp = item.RESPF_ORG_EXP; // SEM DESTINO DEFINIDO
        //        var respfCns = item.RESPF_CNS; // SEM DESTINO DEFINIDO
        //        var nomeVendedor = item.NOME_VENDEDOR; // SEM DESTINO DEFINIDO
        //        var cpfVendedor = item.CPF_VENDEDOR; // SEM DESTINO DEFINIDO
        //        var emailVendedor = item.EMAIL_VENDEDOR; // SEM DESTINO DEFINIDO
        //        var telefoneVendedor = item.TELEFONE_VENDEDOR; // SEM DESTINO DEFINIDO
        //        var mensalidadeTit = item.MENSALIDADE_TIT; // SEM DESTINO DEFINIDO
        //        var mensalidadeDep = item.MENSALIDADE_DEP; // SEM DESTINO DEFINIDO
        //        var acessorios = item.ACESSORIOS; // SEM DESTINO DEFINIDO

        //        //VND1000_ON dados do grid 
        //        insertCsv.InserirVND1000_ON(item);

        //        // TMP1000 – Dados principais
        //        insertCsv.InserirTMP1000Net(item);

        //        // TMP1001 – Endereço e e-mail
        //        insertCsv.InserirTMP1001(item);

        //        // TMP1002 – Dia de vencimento
        //        insertCsv.InserirTMP1002(item);

        //        // TMP1006 – Telefones
        //        insertCsv.InserirTMP1006(item);
        //    }
        //}
        #endregion



        protected  void btnEnviar_Click(object sender, EventArgs e)
        {
            importCVSPlanniumMedic insertCsv = new importCVSPlanniumMedic();

            int sucesso = 0;
            int falhas = 0;

            foreach (var item in DadosCsv)
            {
                try
                {
                    // Chamada explícita de todos os campos
                    var codigoAssociado = item.CODIGO_ASSOCIADO;
                    var nome = item.NOME;
                    var id = item.ID;
                    var orgExp = item.ORG_EXP;
                    var cpf = item.CPF;
                    var pis = item.PIS;
                    var cns = item.CNS;
                    var dtNasc = item.DT_NASC;
                    var naturalidade = item.NATURALIDADE;
                    var sexo = item.SEXO;
                    var estCivil = item.EST_CIVIL;
                    var tipoLogradouro = item.TIPO_LOGRADOURO;
                    var rua = item.RUA;
                    var numeroLogradouro = item.NUMERO_LOGRADOURO;
                    var complemento = item.COMPLEMENTO;
                    var bairro = item.BAIRRO;
                    var cidade = item.CIDADE;
                    var uf = item.UF;
                    var cep = item.CEP;
                    var dddTel = item.DDD_TEL;
                    var tel = item.TEL;
                    var dddTel2 = item.DDD_TEL_2;
                    var tel2 = item.TEL_2;
                    var dddCel = item.DDD_CEL;
                    var celular = item.CELULAR;
                    var codPlano = item.COD_PLANO;
                    var dtIncl = item.DT_INCL;
                    var dtVigencia = item.DT_VIGENCIA;
                    var codEmpresa = item.COD_EMPRESA;
                    var codUnid = item.COD_UNID;
                    var mat = item.MAT;
                    var admissao = item.ADMISSAO;
                    var nomeMae = item.NOME_MAE;
                    var nomePai = item.NOME_PAI;
                    var email = item.EMAIL;
                    var codResp = item.COD_RESP;
                    var parent = item.PARENT;
                    var univer = item.UNIVER;
                    var nrDecNascVivo = item.NR_DEC_NASC_VIVO;
                    var agregado = item.AGREGADO;
                    var defInvalido = item.DEF_INVALIDO;
                    var codLotacao = item.COD_LOTACAO;
                    var tipoMovimentacao = item.TIPO_MOVIMENTACAO;
                    var dataExclusao = item.DATA_EXCLUSAO;
                    var motivoExclusao = item.MOTIVO_EXCLUSAO;
                    var codOutro = item.COD_OUTRO;
                    var codGseg = item.COD_GSEG;
                    var codVend = item.COD_VEND;
                    var codProp = item.COD_PROP;
                    var obs = item.OBS;
                    var obsTec = item.OBS_TEC;
                    var mostraLib = item.MOSTRA_LIB;
                    var codForma = item.COD_FORMA;
                    var diaVenc = item.DIA_VENC;
                    var codTabcom = item.COD_TABCOM;
                    var cargo = item.CARGO;
                    var responsavel = item.RESPONSAVEL;
                    var dtCasamento = item.DT_CASAMENTO;
                    var ufOrgao = item.UF_ORGAO;
                    var codGrp = item.COD_GRP;
                    var codSuperv = item.COD_SUPERV;
                    var codProfissao = item.COD_PROFISSAO;
                    var cnpjOperadora = item.CNPJ_OPERADORA;
                    var codAnsOperadora = item.COD_ANS_OPERADORA;
                    var nomeOperadora = item.NOME_OPERADORA;
                    var idade = item.IDADE;
                    var respfMae = item.RESPF_MAE;
                    var respfNascimento = item.RESPF_NASCIMENTO;
                    var respfEmail = item.RESPF_EMAIL;
                    var respfIdade = item.RESPF_IDADE;
                    var respfSexo = item.RESPF_SEXO;
                    var respfEstadocivil = item.RESPF_ESTADOCIVIL;
                    var respfRg = item.RESPF_RG;
                    var respfOrgExp = item.RESPF_ORG_EXP;
                    var respfCns = item.RESPF_CNS;
                    var nomeVendedor = item.NOME_VENDEDOR;
                    var cpfVendedor = item.CPF_VENDEDOR;
                    var emailVendedor = item.EMAIL_VENDEDOR;
                    var telefoneVendedor = item.TELEFONE_VENDEDOR;
                    var mensalidadeTit = item.MENSALIDADE_TIT;
                    var mensalidadeDep = item.MENSALIDADE_DEP;
                    var acessorios = item.ACESSORIOS;

                    // Chamada para o insert
                    insertCsv.InserirProposta(item);

                }
                catch (Exception ex)
                {
                    falhas++;
                    // Opcional: logar o erro para cada registro com problema
                    Console.WriteLine($"Erro ao importar registro {item.CPF}: {ex.Message}");
                }
            }
            // Exibe resultado final
            if (falhas == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "Swal.fire('Sucesso', 'Todos os registros foram importados com sucesso!', 'success');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"Swal.fire('Aviso', '{sucesso} registros importados com sucesso e {falhas} falharam.', 'warning');", true);
            }
        }
    }
} 