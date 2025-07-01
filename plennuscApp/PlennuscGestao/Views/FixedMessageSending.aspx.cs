using appWhatsapp.Models;
using appWhatsapp.Service;
using Newtonsoft.Json.Linq;
using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        protected void Page_Load(object sender, EventArgs e)
        {
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

            foreach (var mensagem in mensagens)
            {
                var resultado = await api.ConexaoApifixo(
                    new List<string> { mensagem.Telefone },
                    mensagem.Field1, // "Prezado(a)"
                    mensagem.Field2, // "Beneficiário(a)"
                    mensagem.Field3, // Nome
                    mensagem.Field4  // Data
                    
                );

                resultadoFinal.AppendLine(resultado);
                await Task.Delay(1000); // opcional: espera entre cada envio
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal", $"mostrarResultadoModal(`{resultadoFinal.ToString().Replace("`", "'")}`);", true);
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

    }
}
