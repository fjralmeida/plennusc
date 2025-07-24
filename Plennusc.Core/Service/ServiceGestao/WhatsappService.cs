using appWhatsapp.SqlQueries;
using Newtonsoft.Json.Linq;
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
        #region EXEMPLO DE API FUNCIONAL
        //public async Task<string> ConexaoApiSuspensao(List<string> telefones, string pdfUrl,
        //                                           string field1, string field2, string field3,
        //                                           string field4, string field5, string field6, string field7)
        //{
        //    var apiUrl = "https://vallorbeneficios.vollsc.com/api/mailings";
        //    var apiKey = "280e3e7ea39279d70108384cabf81df7";
        //    var resultadoFinal = new StringBuilder();

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Add("voll-api-key", apiKey);
        //        client.DefaultRequestHeaders.Add("Accept", "application/json");

        //        foreach (var telefone in telefones)
        //        {
        //            var jsonBody = $@"
        //            {{
        //              ""media_hsm_configuration_id"": ""2daf52d0-a086-43b5-8033-72dd51dd4ea8"",
        //              ""hsm_type"": ""media_hsm"",
        //              ""campaign_id"": ""94149ef1-e3fd-408d-a864-ed0ecbad9849"",
        //              ""system"": ""whatsapp"",
        //              ""contacts"": [ 
        //                {{ 
        //                  ""phone_number"": ""{telefone}"",
        //                  ""field_1"": ""{field1}"",
        //                  ""field_2"": ""{field2}"",
        //                  ""field_3"": ""{field3}"",
        //                  ""field_4"": ""{field4}"",
        //                  ""field_5"": ""{field5}"",
        //                  ""field_6"": ""{field6}"",
        //                  ""field_7"":  ""{field7}"",
        //                  ""field_8"":  ""{pdfUrl}""
        //                }}
        //              ]
        //            }}";

        //            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        //            try
        //            {
        //                var response = await client.PostAsync(apiUrl, content);
        //                var responseBody = await response.Content.ReadAsStringAsync();

        //                //resultadoFinal.AppendLine($"✅ {telefone}: {response.StatusCode} - {responseBody}");

        //                // Tenta extrair o ID do envio
        //                var json = JObject.Parse(responseBody);
        //                var id = json["id"]?.ToString();

        //                if (!string.IsNullOrEmpty(id))
        //                {
        //                    // Faz o GET para verificar status da mensagem enviada
        //                    var statusResponse = await ConsultarStatusEnvioAsync(id, telefone, apiKey);
        //                    resultadoFinal.AppendLine(statusResponse);
        //                }
        //                else
        //                {
        //                    resultadoFinal.AppendLine($"⚠️ {telefone}: ID não encontrado na resposta.");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                resultadoFinal.AppendLine($"❌ {telefone}: Erro - {ex.Message}");
        //            }

        //            await Task.Delay(5000);
        //        }
        //    }

        //    return resultadoFinal.ToString();
        //}

        #endregion

        public async Task<string> ConexaoApiSuspensao(List<string> telefones, string pdfUrl,
                                                    string field1, string field2, string field3,
                                                    string field4, string mensagemFinal)
        {
            var util = new ItensPedIntegradoUtil();
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
                      ""media_hsm_configuration_id"": ""6a8f51e9-90ce-4e3a-aa24-fae141a21b44"",
                      ""hsm_type"": ""media_hsm"",
                      ""campaign_id"": ""94149ef1-e3fd-408d-a864-ed0ecbad9849"",
                      ""system"": ""whatsapp"",
                      ""contacts"": [ 
                        {{ 
                          ""phone_number"": ""{telefone}"",
                          ""field_1"": ""{field1}"",
                          ""field_2"": ""{field2}"",
                          ""field_3"": ""{field3}"",
                          ""field_4"": ""{field4}"",
                          ""field_5"":  ""{pdfUrl}""
                        }}
                      ]
                    }}";

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await client.PostAsync(apiUrl, content);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        //resultadoFinal.AppendLine($"✅ {telefone}: {response.StatusCode} - {responseBody}");

                        // Tenta extrair o ID do envio
                        var json = JObject.Parse(responseBody);
                        var id = json["id"]?.ToString();

                        if (!string.IsNullOrEmpty(id))
                        {
                            var statusResponse = await ConsultarStatusEnvioAsync(id, telefone, apiKey);

                            // ✅ Grava no banco a mensagem enviada
                            util.GravarLogEnvio(
                                telefoneDestino: telefone,
                                codAssociado: null,                 // se tiver, preencha
                                statusEnvio: "ENVIADO",
                                idResposta: id,
                                conteudoApi: statusResponse,
                                mensagemFinal: mensagemFinal,     // <<< a mensagem do template já montada
                                codEmpresa: 400,
                                numeroEnvio: null                   // ou o número do remetente, se aplicável
                            );

                            resultadoFinal.AppendLine(statusResponse);
                        }
                        else
                        {
                            resultadoFinal.AppendLine($"⚠️ {telefone}: ID não encontrado na resposta.");
                        }
                    }
                    catch (Exception ex)
                    {
                        resultadoFinal.AppendLine($"❌ {telefone}: Erro - {ex.Message}");
                    }

                    await Task.Delay(5000);
                }
            }

            return resultadoFinal.ToString();
        }


        public async Task<string> ConexaoApiDefinitivo(List<string> telefones, string pdfUrl,
                                                    string field1, string field2, string field3,
                                                    string field4, string mensagemFinal)
        {
            var util = new ItensPedIntegradoUtil();
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
                      ""media_hsm_configuration_id"": ""f9acd546-1d81-4b51-b12f-765625600c8b"",
                      ""hsm_type"": ""media_hsm"",
                      ""campaign_id"": ""94149ef1-e3fd-408d-a864-ed0ecbad9849"",
                      ""system"": ""whatsapp"",
                      ""contacts"": [ 
                        {{ 
                          ""phone_number"": ""{telefone}"",
                          ""field_1"": ""{field1}"",
                          ""field_2"": ""{field2}"",
                          ""field_3"": ""{field3}"",
                          ""field_4"": ""{field4}"",
                          ""field_5"":  ""{pdfUrl}""
                        }}
                      ]
                    }}";

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await client.PostAsync(apiUrl, content);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        //resultadoFinal.AppendLine($"✅ {telefone}: {response.StatusCode} - {responseBody}");

                        // Tenta extrair o ID do envio
                        var json = JObject.Parse(responseBody);
                        var id = json["id"]?.ToString();

                        if (!string.IsNullOrEmpty(id))
                        {
                            var statusResponse = await ConsultarStatusEnvioAsync(id, telefone, apiKey);

                            // ✅ Grava no banco a mensagem enviada
                            util.GravarLogEnvio(
                                telefoneDestino: telefone,
                                codAssociado: null,                 // se tiver, preencha
                                statusEnvio: "ENVIADO",
                                idResposta: id,
                                conteudoApi: statusResponse,
                                mensagemFinal: mensagemFinal,     // <<< a mensagem do template já montada
                                codEmpresa: 400,
                                numeroEnvio: null                   // ou o número do remetente, se aplicável
                            );

                            resultadoFinal.AppendLine(statusResponse);
                        }
                        else
                        {
                            resultadoFinal.AppendLine($"⚠️ {telefone}: ID não encontrado na resposta.");
                        }
                    }
                    catch (Exception ex)
                    {
                        resultadoFinal.AppendLine($"❌ {telefone}: Erro - {ex.Message}");
                    }

                    await Task.Delay(5000);
                }
            }

            return resultadoFinal.ToString();
        }

        public async Task<string> ConexaoApiDoisBoletos(List<string> telefones, string pdfUrl,
                                                   string field1, string field2, string field3,
                                                   string field4, string field5, string field6, string field7)
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
                          ""field_1"": ""{field1}"",
                          ""field_2"": ""{field2}"",
                          ""field_3"": ""{field3}"",
                          ""field_4"": ""{field4}"",
                          ""field_5"": ""{field5}"",
                          ""field_6"": ""{field6}"",
                          ""field_7"":  ""{field7}"",
                          ""field_8"":  ""{pdfUrl}""
                        }}
                      ]
                    }}";

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await client.PostAsync(apiUrl, content);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        //resultadoFinal.AppendLine($"✅ {telefone}: {response.StatusCode} - {responseBody}");

                        // Tenta extrair o ID do envio
                        var json = JObject.Parse(responseBody);
                        var id = json["id"]?.ToString();

                        if (!string.IsNullOrEmpty(id))
                        {
                            // Faz o GET para verificar status da mensagem enviada
                            var statusResponse = await ConsultarStatusEnvioAsync(id, telefone, apiKey);
                            resultadoFinal.AppendLine(statusResponse);
                        }
                        else
                        {
                            resultadoFinal.AppendLine($"⚠️ {telefone}: ID não encontrado na resposta.");
                        }
                    }
                    catch (Exception ex)
                    {
                        resultadoFinal.AppendLine($"❌ {telefone}: Erro - {ex.Message}");
                    }

                    await Task.Delay(5000);
                }
            }

            return resultadoFinal.ToString();
        }


        public async Task<string> ConexaoApifixo(List<string> telefones,
                                           string field1, string field2, string field3, string field4)
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
                      ""media_hsm_configuration_id"": ""019896de-1b32-4f22-ad48-40f12dbb64ca"",
                      ""hsm_type"": ""media_hsm"",
                      ""campaign_id"": ""94149ef1-e3fd-408d-a864-ed0ecbad9849"",
                      ""system"": ""whatsapp"",
                      ""contacts"": [ 
                        {{ 
                          ""phone_number"": ""{telefone}"",
                          ""field_1"": ""{field1}"",
                          ""field_2"": ""{field2}"",
                          ""field_3"": ""{field3}"",
                          ""field_4"": ""{field4}""
                        }}
                      ]
                    }}";

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await client.PostAsync(apiUrl, content);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        var json = JObject.Parse(responseBody);
                        var id = json["id"]?.ToString();

                        if (!string.IsNullOrEmpty(id))
                        {
                            var statusResponse = await ConsultarStatusEnvioAsync(id, telefone, apiKey);
                            resultadoFinal.AppendLine(statusResponse);
                        }
                        else
                        {
                            resultadoFinal.AppendLine($"⚠️ {telefone}: ID não encontrado na resposta.");
                        }
                    }
                    catch (Exception ex)
                    {
                        resultadoFinal.AppendLine($"❌ {telefone}: Erro - {ex.Message}");
                    }

                    await Task.Delay(5000); // 5 segundos entre os envios
                }
            }

            return resultadoFinal.ToString();
        }


        private async Task<string> ConsultarStatusEnvioAsync(string id, string telefone, string apiKey)
        {
            await Task.Delay(10000);

            var resultado = new StringBuilder();
            var url = $"https://vallorbeneficios.vollsc.com/api/mailings/{id}?per=100&page=1";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("voll-api-key", apiKey);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                try
                {
                    await Task.Delay(5000);
                    var response = await client.GetAsync(url);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var json = JObject.Parse(responseBody);
                        var unidades = json["items"];

                        if (unidades != null)
                        {
                            foreach (var unidade in unidades)
                            {
                                var numero = unidade["phone_number"]?.ToString();
                                var enviado = unidade["sent"]?.ToString();
                                var falhou = unidade["failed"]?.ToString();
                                var recebido = unidade["recvd"]?.ToString();

                                string statusFalha = string.IsNullOrEmpty(falhou) ? "Sem informações" : falhou;
                                string statusRecebido = string.IsNullOrEmpty(recebido) ? "null" : recebido;

                                resultado.AppendLine($"📞 Telefone: {numero}\n   ✅ Enviado: {enviado}\n   📥 Recebido: {statusRecebido}\n   ❌ Falhou: {statusFalha}\n");
                            }
                        }
                        else
                        {
                            resultado.AppendLine($"⚠️ Nenhuma unidade retornada para o ID {id}");
                        }
                    }
                    else
                    {
                        resultado.AppendLine($"⚠️ Falha ao consultar status: {response.StatusCode} - {responseBody}");
                    }
                }
                catch (Exception ex)
                {
                    resultado.AppendLine($"❌ Erro ao consultar status: {ex.Message}");
                }
            }

            return resultado.ToString();
        }
    }
}