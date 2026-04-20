using appWhatsapp.PlennuscGestao.Services;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Ajax.Utilities;
using Plennusc.Core.Models.ModelsGestao.modelsButYou;
using Plennusc.Core.Service.ServiceGestao.serviceYouBut;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.butYouQueries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class enterpriseButYou : System.Web.UI.Page
    {
        private EnterpriseDocx _enterpriseDocx = new EnterpriseDocx();
        private DocxService _docxService = new DocxService();
        private ExcelService _excelService = new ExcelService();

        protected void Page_Load(object sender, EventArgs e) { }

        protected void propostaEmpresa_Click(object sender, EventArgs e)
        {
            try
            {
                // ==================== CRIAÇÃO DO DOCUMENTO ====================
                string templatePath = Server.MapPath("~/public/uploadgestao/docs/youBut/MAIS_VOCE_ESTIPULADO_PME_BA_MIGRAÇÃO_ENFERMARIA.docx");
                string tempDir = Server.MapPath("~/temp/");
                Directory.CreateDirectory(tempDir);
                string outputPath = Path.Combine(tempDir, $"PROPOSTA_EMPRESA_{DateTime.Now:yyyyMMddHHmmss}.docx");

                File.Copy(templatePath, outputPath, true);

                // ==================== STRING DE CONEXÃO ====================
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

                // ==================== BUSCAR DADOS DA EMPRESA ====================
                var empresaQueries = new enterpriseButYouQueries(connectionString);
                var empresas = empresaQueries.BuscarDadosEmpresa();
                var empresa = empresas.FirstOrDefault();

                if (empresa == null)
                    throw new Exception("Nenhuma empresa encontrada para os códigos informados.");

                var dadosEmpresa = new Dictionary<string, string>
{
    { "RAZAO_SOCIAL", empresa.RazaoSocial },
    { "NOME_FANTASIA", empresa.NomeFantasia },
    { "ENDERECO", empresa.Endereco },
    { "CNPJ", empresa.Cnpj },
};

                _enterpriseDocx.FillDadosEmpresa(outputPath, dadosEmpresa);
                Thread.Sleep(100);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // ==================== BUSCAR ASSOCIADOS DA EMPRESA ====================
                int codigoEmpresaInt = int.Parse(empresa.CodigoEmpresa);
                var todosAssociados = empresaQueries.BuscarAssociadosPorEmpresa(codigoEmpresaInt);

                // --- MONTAR A LISTA DE TITULARES COM SEUS DEPENDENTES E VALORES REAIS ---
                var titulares = new List<Tuple<Dictionary<string, string>, List<Dictionary<string, string>>>>();

                // Agrupar por código do titular (se não tiver, é o próprio)
                var grupos = todosAssociados.GroupBy(a => string.IsNullOrEmpty(a.CodigoTitular) ? a.CodigoAssociado : a.CodigoTitular);

                foreach (var grupo in grupos)
                {
                    var titular = grupo.FirstOrDefault(a => a.CodigoAssociado == grupo.Key);
                    if (titular == null) continue;

                    var dependentes = grupo.Where(a => a.CodigoAssociado != grupo.Key).ToList();

                    // Dicionário do titular (inclui VALOR_PLANO)
                    var dictTitular = new Dictionary<string, string>
            {
                { "NOME_COMPLETO", titular.NomeCompleto ?? "" },
                { "CPF_TITULAR", titular.CpfTitular ?? "" },
                { "DATA_NASCIMENTO", titular.DataNascimento ?? "" },
                { "RG", titular.Rg ?? "" },
                { "FILIACAO", "" },
                { "IDADE", titular.Idade ?? "" },
                { "SEXO", titular.Sexo ?? "" },
                { "ESTADO_CIVIL", titular.EstadoCivil ?? "" },
                { "ORGAO_EXPEDIDOR", titular.OrgaoExpedidor ?? "" },
                { "CARTAO_SUS", titular.CartaoSus ?? "" },
                { "ENDERECO", titular.Endereco ?? "" },
                { "NUMERO", titular.Numero ?? "" },
                { "COMPLEMENTO", titular.Complemento ?? "" },
                { "CEP", titular.Cep ?? "" },
                { "BAIRRO", titular.Bairro ?? "" },
                { "CIDADE", titular.Cidade ?? "" },
                { "UF", titular.Uf ?? "" },
                { "EMAIL", titular.Email ?? "" },
                { "TELEFONE_CELULAR", titular.TelefoneCelular ?? "" },
                { "VALOR_PLANO", titular.ValorPlano?.ToString("C2") ?? "R$ 0,00" },
                { "DIA_VENCIMENTO", titular.DiaVencimentoPadrao?.ToString() ?? "20" },
                { "INICIO_VIGENCIA", titular.DataInicioVigencia?.ToString("dd/MM/yyyy") ?? "" }
            };

                    // Lista de dicionários dos dependentes (inclui VALOR_PLANO)
                    var dictDependentes = dependentes.Select(d => new Dictionary<string, string>
            {
                { "NOME_COMPLETO", d.NomeCompleto ?? "" },
                { "DATA_NASCIMENTO", d.DataNascimento ?? "" },
                { "SEXO", d.Sexo ?? "" },
                { "ESTADO_CIVIL", d.EstadoCivil ?? "" },
                { "IDADE", d.Idade ?? "" },
                { "CPF", d.CpfTitular ?? "" },
                { "FILIACAO", "" },
                { "RG", d.Rg ?? "" },
                { "ORGAO_EXPEDIDOR", d.OrgaoExpedidor ?? "" },
                { "CARTAO_SUS", d.CartaoSus ?? "" },
                { "PARENTESCO", d.NomeParentesco ?? "" },
                { "ENDERECO", d.Endereco ?? "" },
                { "NUMERO", d.Numero ?? "" },
                { "COMPLEMENTO", d.Complemento ?? "" },
                { "CEP", d.Cep ?? "" },
                { "BAIRRO", d.Bairro ?? "" },
                { "CIDADE", d.Cidade ?? "" },
                { "UF", d.Uf ?? "" },
                { "EMAIL", d.Email ?? "" },
                { "TELEFONE_CELULAR", d.TelefoneCelular ?? "" },
                { "VALOR_PLANO", d.ValorPlano?.ToString("C2") ?? "R$ 0,00" }
            }).ToList();

                    titulares.Add(Tuple.Create(dictTitular, dictDependentes));
                }

                //// Variável para guardar os dados do primeiro titular (para nome do arquivo)
                //Dictionary<string, string> primeiroTitularDict = null;

                // ==================== PROCESSAR CADA TITULAR ====================
                for (int i = 0; i < titulares.Count; i++)
                {
                    var dadosTitular = titulares[i].Item1;
                    var dependentes = titulares[i].Item2;

                    if (i == 0)
                    {
                        int diaVencOriginal = 1; // padrão

                        // Verificar se o titular tem DataInicioVigencia
                        if (dadosTitular.ContainsKey("INICIO_VIGENCIA") && !string.IsNullOrWhiteSpace(dadosTitular["INICIO_VIGENCIA"]))
                        {
                            if (DateTime.TryParseExact(dadosTitular["INICIO_VIGENCIA"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataVigenciaTitular))
                            {
                                diaVencOriginal = dataVigenciaTitular.Day;
                            }
                        }

                        // Aplicar regra: 5 vira 10, 15 vira 20
                        int diaVencFinal = diaVencOriginal;
                        if (diaVencOriginal == 5)
                            diaVencFinal = 10;
                        else if (diaVencOriginal == 15)
                            diaVencFinal = 20;

                        // Data de início da vigência usa o dia final (ajustado)
                        string dataVigenciaFormatada = $"{diaVencFinal:D2}/05/2026";

                        // Força o valor da data de vigência no dicionário do titular
                        dadosTitular["INICIO_VIGENCIA"] = dataVigenciaFormatada;

                        // Dicionário para os checkboxes de vencimento
                        var dadosVigencia = new Dictionary<string, string>
    {
        { "VENCIMENTO_DIA_01", (diaVencFinal == 1).ToString().ToLower() },
        { "VENCIMENTO_DIA_10", (diaVencFinal == 10).ToString().ToLower() },
        { "VENCIMENTO_DIA_20", (diaVencFinal == 20).ToString().ToLower() }
    };

                        // Mescla os dados de vigência com os dados do titular
                        var dadosCompletos = new Dictionary<string, string>(dadosTitular);
                        foreach (var kv in dadosVigencia)
                        {
                            if (!dadosCompletos.ContainsKey(kv.Key))
                                dadosCompletos.Add(kv.Key, kv.Value);
                            else
                                dadosCompletos[kv.Key] = kv.Value;
                        }

                        _enterpriseDocx.FillTitularBasico(outputPath, outputPath, dadosCompletos, 0);
                        Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                        if (dependentes.Count > 0)
                        {
                            _enterpriseDocx.FillDependentes(outputPath, outputPath, dependentes, 0);
                            Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();
                        }
                    }
                    else if (i == 1)
                    {
                        _enterpriseDocx.FillTitularBasico(outputPath, outputPath, dadosTitular, 1);
                        Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                        if (dependentes.Count > 0)
                        {
                            _enterpriseDocx.FillDependentes(outputPath, outputPath, dependentes, 1);
                            Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();
                        }
                    }
                    else
                    {
                        _enterpriseDocx.DuplicarPaginaBeneficiario(outputPath, templatePath);
                        Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                        _enterpriseDocx.FillTitularBasico(outputPath, outputPath, dadosTitular, i);
                        Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                        if (dependentes.Count > 0)
                        {
                            _enterpriseDocx.FillDependentes(outputPath, outputPath, dependentes, i);
                            Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();
                        }
                    }

                    // ========== VALORES REAIS ==========
                    var valores = new Dictionary<string, string>();
                    valores["VALOR_TITULAR"] = dadosTitular["VALOR_PLANO"];
                    for (int j = 0; j < dependentes.Count; j++)
                    {
                        valores[$"VALOR_DEPENDENTE_{j + 1}"] = dependentes[j]["VALOR_PLANO"];
                    }



                    _enterpriseDocx.FillTabelaValoresPorBloco(outputPath, valores, i);
                    Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                    _enterpriseDocx.FillTotalContraprestacaoPorBloco(outputPath, valores, i);
                    Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                    // ==================== PREENCHER TERMO DE AUTORIZAÇÃO ====================
                    var primeiroTitularDict = titulares[0].Item1;
                    string nomeServidor = primeiroTitularDict.ContainsKey("NOME_COMPLETO") ? primeiroTitularDict["NOME_COMPLETO"] : "";
                    string assinatura = nomeServidor;

                    _enterpriseDocx.FillTermoAutorizacao(outputPath, nomeServidor, assinatura);

                }



                // ========== GERAR NOME DO ARQUIVO COM NOME E EMAIL DA EMPRESA ==========
                // Pega o primeiro titular da lista (ou o que você quiser)
                var primeiroTitular = titulares[0]; // ou use um índice específico

                // Acessa o dicionário do titular (Item1 da tupla)
                var dadosTitularArq = primeiroTitular.Item1;

                // Agora sim pode acessar as chaves do dicionário
                string nomeEmpresa = dadosTitularArq.ContainsKey("NOME_COMPLETO") && !string.IsNullOrWhiteSpace(dadosTitularArq["NOME_COMPLETO"])
                    ? dadosTitularArq["NOME_COMPLETO"]
                    : dadosTitularArq.ContainsKey("RAZAO_SOCIAL") ? dadosTitularArq["RAZAO_SOCIAL"] : "Empresa";

                string emailEmpresa = dadosTitularArq.ContainsKey("EMAIL") ? dadosTitularArq["EMAIL"] : "sememail@empresa.com";


                string nomeArquivo = GerarNomeArquivo(nomeEmpresa, emailEmpresa);

                // ========== DEFINIR PASTA DE DESTINO ==========
                string pastaDestino = @"C:\inetpub\wwwroot\plennusc\plennuscApp\public\uploadgestao\docs\dadosReaisYouBut";
                Directory.CreateDirectory(pastaDestino); // garante que a pasta existe

                string caminhoDestino = Path.Combine(pastaDestino, nomeArquivo);

                // Se já existir um arquivo com o mesmo nome, remove ou renomeia? Vamos remover para substituir
                if (File.Exists(caminhoDestino))
                    File.Delete(caminhoDestino);

                // Move o arquivo da pasta temporária para a pasta de destino
                File.Move(outputPath, caminhoDestino);

                // ========== EXIBIR MENSAGEM DE SUCESSO ==========
                lblErro.Text = $"Arquivo gerado com sucesso!<br/>Salvo em: {caminhoDestino}";
                lblErro.ForeColor = System.Drawing.Color.Green;
                lblErro.Visible = true;

                // Opcional: link para abrir a pasta
                string pastaUrl = "file:///" + pastaDestino.Replace("\\", "/");
                lblErro.Text += $"<br/><a href='{pastaUrl}' target='_blank'>Abrir pasta de destino</a>";
            }
            catch (Exception ex)
            {
                lblErro.Text = $"ERRO: {ex.Message}<br/>{ex.StackTrace}";
                lblErro.Visible = true;
            }
        }

        // Métodos auxiliares para gerar nome do arquivo
        private string GerarNomeArquivo(string nomeEmpresa, string email)
        {
            // Limpar nome da empresa: remover acentos, substituir não alfanuméricos por espaço, depois underscore
            string nomeLimpo = RemoverAcentos(nomeEmpresa);
            nomeLimpo = Regex.Replace(nomeLimpo, @"[^a-zA-Z0-9]", " ");
            nomeLimpo = Regex.Replace(nomeLimpo, @"\s+", "_");
            nomeLimpo = nomeLimpo.Trim('_');

            // Email: não substituir @ e ., apenas remover caracteres proibidos em nomes de arquivo
            char[] invalidos = Path.GetInvalidFileNameChars();
            string emailLimpo = email;
            foreach (char c in invalidos)
            {
                emailLimpo = emailLimpo.Replace(c, '_');
            }
            // Se quiser garantir que não haja espaços (emails não têm espaço, mas segurança)
            emailLimpo = emailLimpo.Replace(' ', '_');

            string nomeArquivo = $"{emailLimpo}__{nomeLimpo}.docx";

            // Limitar tamanho
            if (nomeArquivo.Length > 200)
            {
                nomeArquivo = nomeArquivo.Substring(0, 200);
                if (!nomeArquivo.EndsWith(".docx"))
                    nomeArquivo += ".docx";
            }

            return SanitizarNomeArquivo(nomeArquivo); // já temos esse método
        }
        private string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            string comAcentos = "áàâãäéèêëíìîïóòôõöúùûüçÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇ";
            string semAcentos = "aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC";
            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i], semAcentos[i]);
            }
            return texto;
        }

        private string SanitizarNomeArquivo(string nomeArquivo)
        {
            if (string.IsNullOrEmpty(nomeArquivo)) return "documento.docx";
            char[] invalidos = Path.GetInvalidFileNameChars();
            foreach (char c in invalidos)
            {
                nomeArquivo = nomeArquivo.Replace(c, '_');
            }
            return nomeArquivo;
        }
    }
}