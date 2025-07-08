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

        protected async void btnEnviar_Click(object sender, EventArgs e)
        {
           
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

    }
}