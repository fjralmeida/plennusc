using appWhatsapp.Models;
using appWhatsapp.Service;
using Newtonsoft.Json.Linq;
using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class FixedMessageSending : System.Web.UI.Page
    {
        protected List<DadosMensagemCsv> DadosCsv
        {
            get => (List<DadosMensagemCsv>)Session["CsvImportado"];
            set => Session["CsvImportado"] = value;
        }

        protected void btnLerCsv_Click(object sender, EventArgs e)
        {
            if (fileUploadCsv.HasFile)
            {
                var dados = ObterLinhasSelecionadasFixo(fileUploadCsv.FileContent);
                DadosCsv = dados;

                // Configura a GridView baseada no formato detectado
                ConfigurarGridView(dados);

                gridCsv.DataSource = dados;
                gridCsv.DataBind();

                btnEnviar.Enabled = gridCsv.Rows.Count > 0;

                // Feedback visual
                if (dados.Count > 0)
                {
                    string formato = VerificarFormato(dados[0]);
                    litResultado.Text = $"<div class='alert alert-info alert-dismissible fade show' role='alert'>" +
                                       $"<i class='fa-solid fa-check-circle me-2'></i>" +
                                       $"<strong>{dados.Count} contatos</strong> carregados no formato <strong>{formato}</strong>." +
                                       $"<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>" +
                                       $"</div>";
                }
            }
            else
            {
                litResultado.Text = "<div class='alert alert-warning'>Selecione um arquivo CSV primeiro.</div>";
            }
        }

        private void ConfigurarGridView(List<DadosMensagemCsv> dados)
        {
            if (dados.Count == 0) return;

            gridCsv.Columns.Clear();
            gridCsv.AutoGenerateColumns = false;

            // Verifica o formato do primeiro item
            var primeiroItem = dados[0];

            if (IsModeloCompleto(primeiroItem))
            {
                gridCsv.Columns.Add(new BoundField { DataField = "Sexo", HeaderText = "Sexo" });
                gridCsv.Columns.Add(new BoundField { DataField = "Field3", HeaderText = "Nome" });
                gridCsv.Columns.Add(new BoundField { DataField = "Field4", HeaderText = "Data" });
                gridCsv.Columns.Add(new BoundField { DataField = "Telefone", HeaderText = "Telefone" });
                gridCsv.Columns.Add(new BoundField { DataField = "Field5", HeaderText = "CPF" });
            }
            else
            {
                // Novo Plano (4 colunas)
                gridCsv.Columns.Add(new BoundField { DataField = "Field3", HeaderText = "Nome" });
                gridCsv.Columns.Add(new BoundField { DataField = "Telefone", HeaderText = "Telefone" });
                gridCsv.Columns.Add(new BoundField { DataField = "NomeOperador", HeaderText = "Operador" });
                gridCsv.Columns.Add(new BoundField { DataField = "Field5", HeaderText = "Antigo Cliente" });
            }
        }

        private bool IsModeloCompleto(DadosMensagemCsv item)
        {
            // Se Field5 for CPF (contém apenas números e tem 11 dígitos) é Modelo Completo
            // Se for "SIM" ou "NAO" é Novo Plano
            return !string.IsNullOrEmpty(item.Field5) &&
                   item.Field5.All(char.IsDigit) &&
                   item.Field5.Length == 11;
        }

        private string VerificarFormato(DadosMensagemCsv item)
        {
            return IsModeloCompleto(item) ? "Modelo Completo (5 colunas)" : "Novo Plano (4 colunas)";
        }

        protected async void btnEnviar_Click(object sender, EventArgs e)
        {
            var mensagens = DadosCsv;

            if (mensagens == null || !mensagens.Any())
            {
                litResultado.Text = "<span style='color:red;'>Nenhum dado para enviar.</span>";
                return;
            }

            var api = new WhatsappService();
            var resultadoFinal = new StringBuilder();

            // Detecta formato
            bool isModeloCompleto = IsModeloCompleto(mensagens[0]);

            // Tabela para armazenar o resultado
            DataTable resultadoCsv = new DataTable();

            if (isModeloCompleto)
            {
                resultadoCsv.Columns.Add("Nome");
                resultadoCsv.Columns.Add("Data");
                resultadoCsv.Columns.Add("Telefone");
                resultadoCsv.Columns.Add("CPF");
                resultadoCsv.Columns.Add("StatusEnvio");
            }
            else
            {
                resultadoCsv.Columns.Add("Nome");
                resultadoCsv.Columns.Add("Telefone");
                resultadoCsv.Columns.Add("Operador");
                resultadoCsv.Columns.Add("Antigo Cliente");
                resultadoCsv.Columns.Add("StatusEnvio");
            }

            foreach (var mensagem in mensagens)
            {
                string status = "";
                string retornoApi = "";

                try
                {
                    if (isModeloCompleto)
                    {
                        retornoApi = await api.ConexaoApifixo(
                            new List<string> { mensagem.Telefone },
                            mensagem.Field1,
                            mensagem.Field2,
                            mensagem.Field3,
                            mensagem.Field4
                        );
                        status = "OK";
                    }
                    else
                    {
                        // VALIDAÇÃO: Se AntigoCliente for nulo ou vazio, trata como "NAO"
                        string antigoCliente = mensagem.Field5.ToString();

                        antigoCliente.ToUpper();

                        if (antigoCliente == "SIM")
                        {
                            retornoApi = await api.ConexaoApiNovoPlano(
                                new List<string> { mensagem.Telefone },
                                mensagem.Field3,          // Nome do beneficiário
                                mensagem.NomeOperador     // Nome do operador
                            );
                            status = "OK";
                        }
                        else // Qualquer outra coisa (NAO, vazio, etc) trata como NOVO CLIENTE
                        {
                            retornoApi = await api.ConexaoApiNovoCliente(
                                new List<string> { mensagem.Telefone },
                                mensagem.Field3,          // Nome do beneficiário
                                mensagem.NomeOperador     // Nome do operador
                            );
                            status = "OK";
                        }
                    }
                }
                catch (Exception ex)
                {
                    retornoApi = $"Erro ao enviar para {mensagem.Telefone}: {ex.Message}";
                    status = "Erro";
                }

                resultadoFinal.AppendLine(retornoApi);

                // Registrar na tabela
                if (isModeloCompleto)
                {
                    resultadoCsv.Rows.Add(
                        mensagem.Field3,      // Nome beneficiário
                        mensagem.Field4,      // Data
                        mensagem.Telefone,
                        mensagem.Field5,      // CPF
                        status
                    );
                }
                else
                {
                    resultadoCsv.Rows.Add(
                        mensagem.Field3,          // Nome beneficiário
                        mensagem.Telefone,
                        mensagem.NomeOperador,
                        string.IsNullOrEmpty(mensagem.AntigoCliente) ? "NAO" : mensagem.AntigoCliente,
                        status
                    );
                }

                await Task.Delay(1000);
            }

            ViewState["ResultadoEnvio"] = resultadoCsv;
            ViewState["Formato"] = isModeloCompleto ? "Completo" : "NovoPlano";

            string resultadoFinalTexto = resultadoFinal.ToString();
            string resultadoEscapado = HttpUtility.JavaScriptStringEncode(resultadoFinalTexto);

            ScriptManager.RegisterStartupScript(
                Page, Page.GetType(), "MostrarResultado",
                $"mostrarResultadoModal('{resultadoEscapado}');", true);
        }

        protected List<DadosMensagemCsv> ObterLinhasSelecionadasFixo(Stream csvStream)
        {
            var lista = new List<DadosMensagemCsv>();

            using (var reader = new StreamReader(csvStream, Encoding.UTF8))
            {
                bool primeiraLinha = true;
                int? formatoDetectado = null; // null = não determinado, 5 = completo, 4 = novo plano

                while (!reader.EndOfStream)
                {
                    var linha = reader.ReadLine();

                    if (primeiraLinha)
                    {
                        primeiraLinha = false;
                        continue; // Pula cabeçalho
                    }

                    if (string.IsNullOrWhiteSpace(linha))
                        continue;

                    string[] campos = linha.Split('\t');
                    if (campos.Length < 2)
                    {
                        campos = linha.Split(';');
                        if (campos.Length < 2)
                            continue;
                    }

                    // Determina o formato na primeira linha de dados válida
                    if (formatoDetectado == null)
                    {
                        formatoDetectado = campos.Length;
                    }

                    // Processa de acordo com o formato
                    if (formatoDetectado == 5) // Modelo Completo
                    {
                        ProcessarModeloCompleto(campos, lista);
                    }
                    else if (formatoDetectado == 4) // Novo Plano (agora 4 colunas)
                    {
                        ProcessarNovoPlano(campos, lista);
                    }
                }
            }

            return lista;
        }

        private void ProcessarModeloCompleto(string[] campos, List<DadosMensagemCsv> lista)
        {
            if (campos.Length < 5) return;

            string sexo = campos[0].Trim().ToUpper();
            string nome = campos[1].Trim();
            string dataTexto = campos[2].Trim();
            string telefoneBruto = campos[3].Trim();
            string cpf = campos[4].Trim();
            cpf = new string(cpf.Where(char.IsDigit).ToArray()).PadLeft(11, '0');

            string telefone = FormatTelefone(telefoneBruto);
            if (string.IsNullOrEmpty(telefone))
                return;

            string saudacao = sexo == "M" ? "Prezado" : "Prezada";
            string papel = sexo == "M" ? "Beneficiário" : "Beneficiária";

            lista.Add(new DadosMensagemCsv
            {
                Telefone = telefone,
                Field1 = saudacao,
                Field2 = papel,
                Field3 = nome,
                Field4 = dataTexto,
                Field5 = cpf,
                Sexo = sexo 
            });
        }

        private void ProcessarNovoPlano(string[] campos, List<DadosMensagemCsv> lista)
        {
            if (campos.Length < 4) return;

            string nome = campos[0].Trim();
            string telefoneBruto = campos[1].Trim();
            string nomeOperador = campos[2].Trim();
            string antigoCliente = campos[3].Trim().ToUpper();

            string telefone = FormatTelefone(telefoneBruto);
            if (string.IsNullOrEmpty(telefone))
                return;

            // Define saudação baseada no tipo de cliente
            string saudacao = antigoCliente == "SIM" ? "Prezado Cliente" : "Prezado(a)";
            string papel = antigoCliente == "SIM" ? "Cliente" : "Beneficiário(a)";

            lista.Add(new DadosMensagemCsv
            {
                Telefone = telefone,
                Field1 = saudacao,
                Field2 = papel,
                Field3 = nome,
                Field4 = DateTime.Now.ToString("dd/MM/yyyy"),
                Field5 = antigoCliente,
                NomeOperador = nomeOperador // Novo campo
            });
        }

        private DataTable ResultadoEnvio
        {
            get => (DataTable)Session["ResultadoEnvio"];
            set => Session["ResultadoEnvio"] = value;
        }

        private string FormatTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return null;

            // Remove tudo que não for número
            var clean = new string(telefone.Where(char.IsDigit).ToArray());

            // Ignora fixo (8 dígitos iniciando com 3)
            if (clean.Length == 8 && clean.StartsWith("3"))
                return null;

            // Caso tenha 9 dígitos (sem DDD), remover o 9 e adicionar DDD 31
            if (clean.Length == 9 && clean.StartsWith("9"))
            {
                return "55" + "31" + clean.Substring(1);
            }

            // Caso tenha 10 dígitos (DDD + número sem 9) — já está ok
            if (clean.Length == 10)
            {
                return "55" + clean;
            }

            // Caso tenha 11 dígitos (DDD + 9 + número), remover o 9
            if (clean.Length == 11 && clean[2] == '9')
            {
                return "55" + clean.Substring(0, 2) + clean.Substring(3);
            }

            // Se não bater com nenhum formato válido
            return null;
        }

        protected void btnDownloadCsvStatus_Click(object sender, EventArgs e)
        {
            if (!(ViewState["ResultadoEnvio"] is DataTable dt) || dt.Rows.Count == 0)
                return;

            string formato = ViewState["Formato"] as string ?? "Completo";

            var sb = new StringBuilder();

            // Cabeçalhos
            var colunas = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
            sb.AppendLine(string.Join(";", colunas));

            // Linhas
            foreach (DataRow row in dt.Rows)
            {
                var valores = row.ItemArray.Select(v => v.ToString());
                sb.AppendLine(string.Join(";", valores));
            }

            string nomeArquivo = $"ResultadoEnvio_{formato}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", $"attachment;filename={nomeArquivo}");
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(sb.ToString());
            Response.End();
        }

        protected void btnModeloCompleto_Click(object sender, EventArgs e)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Sexo;Nome;Data;Telefone;Cpf");
            csv.AppendLine("M;João Silva;15/07/2025;31999999999;12345678901");
            csv.AppendLine("F;Maria Santos;16/07/2025;31988888888;98765432100");

            DownloadCsvModelo(csv.ToString(), "Modelo_Completo_5_colunas.csv");
        }

        protected void btnModeloSimples_Click(object sender, EventArgs e)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("Nome;Telefone;Nome_Operador;Antigo_Cliente");
            csv.AppendLine("Leonardo Ambrosio;31973069983;Laryssa Moraes;SIM");
            csv.AppendLine("Maria Santos;31973069983;Bruno Lopes;NAO");

            DownloadCsvModelo(csv.ToString(), "Novo_Plano_4_colunas.csv");
        }

        private void DownloadCsvModelo(string conteudoCsv, string nomeArquivo)
        {
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", $"attachment;filename={nomeArquivo}");
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(conteudoCsv);
            Response.End();
        }
    }
}