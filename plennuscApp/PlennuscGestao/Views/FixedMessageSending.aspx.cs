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

                gridCsv.DataSource = dados;
                gridCsv.DataBind();

                btnEnviar.Enabled = gridCsv.Rows.Count > 0;
            }
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

            //Tabela para armazenar o resultado para exportação posterior 
            DataTable resultadoCsv = new DataTable();
            resultadoCsv.Columns.Add("Nome");
            resultadoCsv.Columns.Add("Data");
            resultadoCsv.Columns.Add("Telefone");
            resultadoCsv.Columns.Add("CPF");
            resultadoCsv.Columns.Add("StatusEnvio");

            foreach (var mensagem in mensagens)
            {
                string status;
                string retornoApi;

                try
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
                catch (Exception ex)
                {
                    retornoApi = $"Erro ao enviar para {mensagem.Telefone}: {ex.Message}";
                    status = "Erro";
                }

                resultadoFinal.AppendLine(retornoApi);

                // Registrar na tabela
                resultadoCsv.Rows.Add(
                    mensagem.Field3, // Nome
                    mensagem.Field4, // Data
                    mensagem.Telefone,
                    mensagem.Field5, // CPF
                    status
                );

                await Task.Delay(1000); // delay entre envios
            }

            // Guardar no ViewState para usar depois no botão de download
            ViewState["ResultadoEnvio"] = resultadoCsv;

            //// Exibir o modal com o resultado final e ativar botão de download
            //ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal", $"mostrarResultadoModal(`{resultadoFinal.ToString().Replace("`", "'")}`);", true);

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

                while (!reader.EndOfStream)
                {
                    var linha = reader.ReadLine();

                    if (primeiraLinha)
                    {
                        primeiraLinha = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(linha))
                        continue;

                    string[] campos = linha.Split('\t');
                    if (campos.Length < 5)
                    {
                        campos = linha.Split(';');
                        if (campos.Length < 5)
                            continue;
                    }

                    string sexo = campos[0].Trim().ToUpper(); 
                    string nome = campos[1].Trim();
                    string dataTexto = campos[2].Trim();
                    string telefoneBruto = campos[3].Trim();
                    string cpf = campos[4].Trim();
                    cpf = new string(cpf.Where(char.IsDigit).ToArray()).PadLeft(11, '0');

                    string telefone = FormatTelefone(telefoneBruto);
                    if (string.IsNullOrEmpty(telefone))
                        continue;

                    string saudacao = sexo == "M" ? "Prezado" : "Prezada";
                    string papel = sexo == "M" ? "Beneficiário" : "Beneficiária";


                    lista.Add(new DadosMensagemCsv
                    {
                        Telefone = telefone,
                        Field1 = saudacao,  // "Prezado" ou "Prezada"
                        Field2 = papel,     // "Beneficiário" ou "Beneficiária"
                        Field3 = nome,      // nome do beneficiário
                        Field4 = dataTexto,  // data (ou pode ser data fixa "01/07/2025" se preferir)
                        Field5 = cpf
                    });
                }
            }

            return lista;
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

            string nomeArquivo = "ResultadoEnvio_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", $"attachment;filename={nomeArquivo}");
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(sb.ToString());
            Response.End();
        }
    }
}
