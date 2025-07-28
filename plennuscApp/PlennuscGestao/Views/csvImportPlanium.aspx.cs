using appWhatsapp.Service;
using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class csvImportPlanuim : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected List<DadosMensagensCsvPlanium> DadosCsv
        {
            get => (List<DadosMensagensCsvPlanium>)Session["CsvImportado"];
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
        protected List<DadosMensagensCsvPlanium> ObterLinhasSelecionadasFixo(Stream csvStream)
        {
            var lista = new List<DadosMensagensCsvPlanium>();

            var unusedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase) // Comparação insensível a maiúsculas/minúsculas
            {
                "MAT", "ADMISSÃO", "AGREGADO", "DEF_INVALIDO", "COD_LOTACAO",
                "DATA_EXCLUSAO", "MOTIVO_EXCLUSAO", "COD_OUTRO", "COD_GSEG",
                "OBS", "OBS_TEC", "MOSTRA_LIB", "COD_FORMA", "COD_TABCOM",
                "CARGO", "DT_CASAMENTO", "UF_ORGAO", "COD_GRP", "NOME_PAI",
                "UNIVER", "PIS", "NATURALIDADE"
            };

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
                        return new List<DadosMensagensCsvPlanium>();
                    }

                    var colunas = linha.Split(delimitador);
                    if (cabecalho == null || cabecalho.Length == 0) continue;

                    var dado = new DadosMensagensCsvPlanium();

                    for (int i = 0; i < cabecalho.Length; i++)
                    {
                        string nomeCampo = cabecalho[i].Trim();

                        // Log para verificação do nome da coluna
                        Console.WriteLine($"Coluna: '{nomeCampo}'");

                        // Ignora as colunas que não são necessárias
                        if (unusedColumns.Contains(nomeCampo))
                        {
                            Console.WriteLine($"Ignorando a coluna: '{nomeCampo}'");
                            continue; // Ignora essa coluna
                        }

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

                        var prop = typeof(DadosMensagensCsvPlanium).GetProperty(nomeCampo);
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

        protected async void btnEnviar_Click(object sender, EventArgs e)
        {
            if (DadosCsv == null || DadosCsv.Count == 0)
                return;

            foreach (var item in DadosCsv)
            {
                var nome = item.NOME; // TMP1000.NET -> NOME_ASSOCIADO
                var id = item.ID; // TMP1000.NET -> NUMERO_RG
                var orgExp = item.ORG_EXP; // TMP1000.NET -> ORGAO_EMISSOR_RG
                var cpf = item.CPF; // TMP1000.NET -> NUMERO_CPF
                var cns = item.CNS; // TMP1000.NET -> CODIGO_CNS
                var dtNasc = item.DT_NASC; // TMP1000.NET -> DATA_NASCIMENTO
                var sexo = item.SEXO; // TMP1000.NET -> SEXO
                var estCivil = item.EST_CIVIL; // TMP1000.NET -> CODIGO_ESTADO_CIVIL

                var tipoLogradouro = string.IsNullOrWhiteSpace(item.TIPO_LOGRADOURO) ? "Rua" : item.TIPO_LOGRADOURO.Trim();
                var rua = item.RUA?.Trim() ?? "";
                var numero = item.NUMERO_LOGRADOURO?.Trim() ?? "";
                var complemento = item.COMPLEMENTO?.Trim() ?? "";
                var endereco = $"{tipoLogradouro} {rua}, {numero} - {complemento}".Trim();
                item.RUA = endereco; // TMP1001 -> ENDERECO

                var bairro = item.BAIRRO; // TMP1001 -> BAIRRO
                var cidade = item.CIDADE; // TMP1001 -> CIDADE
                var uf = item.UF; // TMP1001 -> ESTADO
                var cep = item.CEP; // TMP1001 -> CEP

                item.TEL = $"{item.DDD_TEL?.Trim()}{item.TEL?.Trim()}"; // TMP1006 -> NUMERO_TELEFONE (Tipo 1)
                item.TEL_2 = $"{item.DDD_TEL_2?.Trim()}{item.TEL_2?.Trim()}"; // TMP1006 -> NUMERO_TELEFONE (Tipo 2)
                item.CELULAR = $"{item.DDD_CEL?.Trim()}{item.CELULAR?.Trim()}"; // TMP1006 -> NUMERO_TELEFONE (Tipo Celular)

                var codPlano = item.COD_PLANO; // TMP1000.NET -> CODIGO_PLANO
                var dtIncl = item.DT_INCL; // TMP1000.NET -> DATA_ADMISSAO
                var dtVigencia = item.DT_VIGENCIA; // TMP1000.NET -> DATA_VALIDA_CARENCIA
                var codEmpresa = item.COD_EMPRESA ?? "400"; // TMP1000.NET -> CODIGO_EMPRESA
                var codUnid = item.COD_UNID; // SEM DESTINO DEFINIDO
                var nomeMae = item.NOME_MAE; // TMP1000.NET -> NOME_MAE
                var email = item.EMAIL; // TMP1001 -> ENDERECO_EMAIL
                var codResp = item.COD_RESP; // TMP1000.NET -> CODIGO_TITULAR

                var parentMap = new Dictionary<string, string>
                {
                    { "1", "5" },
                    { "2", "3" },
                    { "3", "11" },
                    { "4", "6" },
                    { "5", "13" },
                    { "6", "4" },
                    { "7", "9" },
                    { "8", "7" },
                    { "9", "8" },
                    { "10", "10" }
                };
                var parent = item.PARENT;
                if (string.IsNullOrWhiteSpace(parent))
                {
                    parent = "2";
                }
                else if (parentMap.TryGetValue(parent, out var novoParent))
                {
                    parent = novoParent;
                }
                item.PARENT = parent; // TMP1000.NET -> CODIGO_PARENTESCO

                var nrDecNascVivo = item.NR_DEC_NASC_VIVO; // TMP1000.NET -> NUMERO_DECLARACAO_NASC_VIVO
                var tipoMovimentacao = item.TIPO_MOVIMENTACAO; // SEM DESTINO DEFINIDO
                var codVend = item.COD_VEND; // TMP1000.NET -> CODIGO_VENDEDOR
                var codProp = item.COD_PROP; // SEM DESTINO DEFINIDO
                var diaVenc = item.DIA_VENC; // TMP1002 -> DIA_VENCIMENTO
                var responsavel = item.RESPONSAVEL; // SEM DESTINO DEFINIDO
                var codSuperv = item.COD_SUPERV; // SEM DESTINO DEFINIDO
                var codProfissao = item.COD_PROFISSAO; // TMP1000.NET -> PROFISSAO
                var cnpjOperadora = item.CNPJ_OPERADORA; // SEM DESTINO DEFINIDO
                var codAnsOperadora = item.COD_ANS_OPERADORA; // TMP1000.NET -> CODIGO_CADASTRO_ANS
                var nomeOperadora = item.NOME_OPERADORA; // SEM DESTINO DEFINIDO
                var idade = item.IDADE; // SEM DESTINO DEFINIDO
                var respfMae = item.RESPF_MAE; // SEM DESTINO DEFINIDO
                var respfNascimento = item.RESPF_NASCIMENTO; // SEM DESTINO DEFINIDO
                var respfEmail = item.RESPF_EMAIL; // SEM DESTINO DEFINIDO
                var respfIdade = item.RESPF_IDADE; // SEM DESTINO DEFINIDO
                var respfSexo = item.RESPF_SEXO; // SEM DESTINO DEFINIDO

                var estadoCivilMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "casado", "3" },
                    { "solteiro", "2" },
                    { "viuvo", "8" },
                    { "viúvo", "8" },
                    { "divorciado", "5" },
                    { "desquitado", "4" },
                    { "marital", "6" },
                    { "não informado", "7" },
                    { "nao informado", "7" }
                };

                var estadoCivil = item.RESPF_ESTADOCIVIL?.Trim();
                if (string.IsNullOrEmpty(estadoCivil) || !estadoCivilMap.TryGetValue(estadoCivil, out var codigo))
                {
                    item.RESPF_ESTADOCIVIL = "7";
                }
                else
                {
                    item.RESPF_ESTADOCIVIL = codigo;
                }
                // item.RESPF_ESTADOCIVIL -> SEM DESTINO DEFINIDO

                var respfRg = item.RESPF_RG; // SEM DESTINO DEFINIDO
                var respfOrgExp = item.RESPF_ORG_EXP; // SEM DESTINO DEFINIDO
                var respfCns = item.RESPF_CNS; // SEM DESTINO DEFINIDO
                var nomeVendedor = item.NOME_VENDEDOR; // SEM DESTINO DEFINIDO
                var cpfVendedor = item.CPF_VENDEDOR; // SEM DESTINO DEFINIDO
                var emailVendedor = item.EMAIL_VENDEDOR; // SEM DESTINO DEFINIDO
                var telefoneVendedor = item.TELEFONE_VENDEDOR; // SEM DESTINO DEFINIDO
                var mensalidadeTit = item.MENSALIDADE_TIT; // SEM DESTINO DEFINIDO
                var mensalidadeDep = item.MENSALIDADE_DEP; // SEM DESTINO DEFINIDO
                var acessorios = item.ACESSORIOS; // SEM DESTINO DEFINIDO

                // Aqui você pode montar suas lógicas futuras
            }
        }
    }
}