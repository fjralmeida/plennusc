using appWhatsapp.Service;
using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Data;
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

                    var colunas = linha.Split(delimitador);
                    if (cabecalho == null || cabecalho.Length == 0) continue;

                    var dado = new DadosMensagensCsvPlanium();

                    for (int i = 0; i < cabecalho.Length; i++)
                    {
                        string nomeCampo = cabecalho[i].Trim();

                        //Remove acentos e caracteres especiais
                        nomeCampo = nomeCampo.Normalize(System.Text.NormalizationForm.FormD);
                        nomeCampo = new string(nomeCampo
                            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                            .ToArray());
                        nomeCampo = nomeCampo.Replace(" ", "_").Replace("-", "_");
                        nomeCampo = System.Text.RegularExpressions.Regex.Replace(nomeCampo, @"[^\w]", "_");

                        string valor = colunas.ElementAtOrDefault(i)?.Trim();

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

            #region FOREACH PARA INSERÇÃO DOS DADOS POR TABELA
            //foreach (var item in DadosCsv)
            //{
            //    // ===========================
            //    // TMP1000_NET
            //    // ===========================
            //    var nomeAssociado = item.NOME;
            //    var rg = item.ID;
            //    var orgaoEmissorRg = item.ORG_EXP;
            //    var cpf = item.CPF;
            //    var pis = item.PIS;
            //    var cns = item.CNS;
            //    var dataNascimento = item.DT_NASC;
            //    var naturalidade = item.NATURALIDADE;
            //    var sexo = item.SEXO;
            //    var estadoCivil = item.EST_CIVIL;

            //    // ===========================
            //    // TMP1001_NET (Endereço)
            //    // ===========================
            //    var tipoLogradouro = item.TIPO_LOGRADOURO;
            //    var rua = item.RUA;
            //    var numero = item.NUMERO_LOGRADOURO;
            //    var complemento = item.COMPLEMENTO;
            //    var bairro = item.BAIRRO;
            //    var cidade = item.CIDADE;
            //    var uf = item.UF;
            //    var cep = item.CEP;

            //    // ===========================
            //    // TMP1006_NET (Telefones)
            //    // ===========================
            //    var dddFixo = item.DDD_TEL;
            //    var telefoneFixo = item.TEL;
            //    var dddTel2 = item.DDD_TEL_2;
            //    var telefone2 = item.TEL_2;
            //    var dddCel = item.DDD_CEL;
            //    var celular = item.CELULAR;

            //    // ===========================
            //    // TMP1000_NET
            //    // ===========================
            //    var codPlano = item.COD_PLANO;
            //    var dataInclusao = item.DT_INCL;
            //    var dataVigencia = item.DT_VIGENCIA;
            //    var codEmpresa = item.COD_EMPRESA ?? "400";
            //    var codUnidade = item.COD_UNID;
            //    var matricula = item.MAT;
            //    var dataAdmissao = item.ADMISSAO;
            //    var nomeMae = item.NOME_MAE;
            //    var nomePai = item.NOME_PAI;
            //    var email = item.EMAIL;
            //    var codResp = item.COD_RESP;
            //    var parentesco = item.PARENT;
            //    var univer = item.UNIVER;
            //    var declaracaoNascidoVivo = item.NR_DEC_NASC_VIVO;
            //    var agregado = item.AGREGADO;
            //    var deficiencia = item.DEF_INVALIDO;
            //    var codLotacao = item.COD_LOTACAO;
            //    var tipoMov = item.TIPO_MOVIMENTACAO;
            //    var dataExclusao = item.DATA_EXCLUSAO;
            //    var motivoExclusao = item.MOTIVO_EXCLUSAO;
            //    var codOutro = item.COD_OUTRO;
            //    var codGseg = item.COD_GSEG;
            //    var codVend = item.COD_VEND;
            //    var codProp = item.COD_PROP;
            //    var observacoes = item.OBS;
            //    var obsTecnicas = item.OBS_TEC;
            //    var mostraLib = item.MOSTRA_LIB;
            //    var codForma = item.COD_FORMA;
            //    var diaVenc = item.DIA_VENC;
            //    var codTabCom = item.COD_TABCOM;
            //    var cargo = item.CARGO;

            //    // ===========================
            //    // TMP1002_NET (Contrato)
            //    // ===========================
            //    var nomeResponsavel = item.RESPONSAVEL;
            //    var dataCasamento = item.DT_CASAMENTO;
            //    var ufOrgaoEmissor = item.UF_ORGAO;
            //    var codGrupo = item.COD_GRP;
            //    var codSupervisor = item.COD_SUPERV;
            //    var codProfissao = item.COD_PROFISSAO;
            //    var cnpjOperadora = item.CNPJ_OPERADORA;
            //    var codAnsOperadora = item.COD_ANS_OPERADORA;
            //    var nomeOperadora = item.NOME_OPERADORA;
            //    var idade = item.IDADE;
            //    var respfMae = item.RESPF_MAE;
            //    var respfNascimento = item.RESPF_NASCIMENTO;
            //    var emailResponsavel = item.RESPF_EMAIL;
            //    var respfIdade = item.RESPF_IDADE;
            //    var respfSexo = item.RESPF_SEXO;
            //    var respfEstadoCivil = item.RESPF_ESTADOCIVIL;
            //    var rgResponsavel = item.RESPF_RG;
            //    var orgaoEmissorResponsavel = item.RESPF_ORG_EXP;
            //    var cnsResponsavel = item.RESPF_CNS;

            //    // ===========================
            //    // Dados do Vendedor
            //    // ===========================
            //    var nomeVendedor = item.NOME_VENDEDOR;
            //    var cpfVendedor = item.CPF_VENDEDOR;
            //    var emailVendedor = item.EMAIL_VENDEDOR;
            //    var telefoneVendedor = item.TELEFONE_VENDEDOR;

            //    // ===========================
            //    // TMP1020_NET (Fatura Titular)
            //    // ===========================
            //    var mensalidadeTitular = item.MENSALIDADE_TIT;

            //    // ===========================
            //    // TMP1021_NET (Fatura Dependente)
            //    // ===========================
            //    var mensalidadeDependente = item.MENSALIDADE_DEP;

            //    // ===========================
            //    // TMP1000_NET - Acessórios
            //    // ===========================
            //    var acessorios = item.ACESSORIOS;

            //    // Aqui no futuro você pode montar objetos ou INSERTs para cada tabela
            //}
            #endregion

            foreach (var item in DadosCsv)
            {
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
                var codEmpresa = item.COD_EMPRESA ?? "400";
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
                var respfEstadoCivil = item.RESPF_ESTADOCIVIL;
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

                // Aqui você pode montar suas lógicas futuras
            }
        }
    }
}