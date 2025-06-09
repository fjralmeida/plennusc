using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace appWhatsapp.Service
{
    public class WhatsappService
    {
        public async Task<string> ConexaoApiAsync(List<string> telefones, string pdfUrl)
        {
            var apiUrl = "https://vallorbeneficios.vollsc.com/api/mailings";
            var apiKey = "280e3e7ea39279d70108384cabf81df7";
            var resultadoFinal = new StringBuilder();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("voll-api-key", apiKey);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                foreach (var telefone in telefones)
                {
                    var jsonBody = $@"
                        {{
                          ""media_hsm_configuration_id"": ""2daf52d0-a086-43b5-8033-72dd51dd4ea8"",
                          ""hsm_type"": ""media_hsm"",
                          ""campaign_id"": ""94149ef1-e3fd-408d-a864-ed0ecbad9849"",
                          ""system"": ""whatsapp"",
                          ""contacts"": [ 
                            {{ 
                              ""phone_number"": ""{telefone}"",
                              ""field_1"": ""Leonardo"",
                              ""field_2"": ""maio"",
                              ""field_3"": ""julho"",
                              ""field_4"": ""AMIL"",
                              ""field_5"": ""22/12/2025"",
                              ""field_6"": ""23/12/2025"",
                              ""field_7"":  ""{pdfUrl}""
                            }}
                          ]
                        }}";

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await client.PostAsync(apiUrl, content);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        resultadoFinal.AppendLine($"✅ {telefone}: {response.StatusCode} - {responseBody}");
                    }
                    catch (Exception ex)
                    {
                        resultadoFinal.AppendLine($"❌ {telefone}: Erro - {ex.Message}");
                    }
                    // Aguarda 10 segundos antes de enviar para o próximo número
                    await Task.Delay(10000);
                }
            }
            return resultadoFinal.ToString();
        }
    }
}