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
                                                    string field4, string mensagemFinal, string codigoAssociado, int codAutenticacao)
        {
            var util = new ItensPedIntegradoUtil();

            //if (util.EnviadosUltimas24H(codigoAssociado, mensagemFinal))
            //{
            //    return $"⛔ Associado {codigoAssociado}: já recebeu '{mensagemFinal}' nas últimas 24h.";
            //}

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
                      ""media_hsm_configuration_id"": ""1c0fa96f-0173-4d5c-84d8-693242fda363"",
                      ""hsm_type"": ""media_hsm"",
                      ""campaign_id"": ""94149ef1-e3fd-408d-a864-ed0ecbad9849"",
                      ""system"": ""whatsapp_enterprise"",
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
                            int codEnvio = util.GravarEnvioMensagem(
                                telefoneDestino: telefone,
                               codAssociado: codigoAssociado,
                                mensagemFinal: mensagemFinal,
                                codEmpresa: 400,
                                codUsuarioEnvio: codAutenticacao
                            );

                            util.GravarRetornoMensagem(
                                codEnvioMensagemWpp: codEnvio,
                                statusEnvio: "ENVIADO",
                                idResposta: id,
                                conteudoApi: statusResponse,
                                codUsuarioEnvio: codAutenticacao
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


        public async Task<string> ConexaoApiNotaFiscal(
                   List<string> telefones,
                   string notafiscal,
                   string field1,
                   string field2,
                   string field3,
                   string field4,
                   string mensagemFinal,
                   string codigoAssociado,
                   int codAutenticacao
               )
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
                    string jsonBody = $@"
                    {{
                        ""media_hsm_configuration_id"": ""5fbfce1c-a0cb-4233-9257-5adf76973b7c"",
                        ""hsm_type"": ""media_hsm"",
                        ""campaign_id"": ""81815174-dd16-4468-9a70-37996dc48f8c"",
                        ""system"": ""whatsapp_enterprise"",    
                        ""contacts"": [ 
                            {{ 
                                ""phone_number"": ""{telefone}"",
                                ""field_1"": ""{field1}"",
                                ""field_2"": ""{field2}"",
                                ""field_3"": ""{field3}"",
                                ""field_4"": ""{field4}"",
                                ""field_5"": ""{notafiscal}""
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

                            int codEnvio = util.GravarEnvioMensagem(
                                telefoneDestino: telefone,
                                codAssociado: codigoAssociado,
                                mensagemFinal: mensagemFinal,
                                codEmpresa: 400,
                                codUsuarioEnvio: codAutenticacao
                            );

                            util.GravarRetornoMensagem(
                                codEnvioMensagemWpp: codEnvio,
                                statusEnvio: "ENVIADO",
                                idResposta: id,
                                conteudoApi: statusResponse,
                                codUsuarioEnvio: codAutenticacao
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

        //public async Task<string> ConexaoApiDoisBoletos(List<string> telefones, string pdfUrl,
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


        public async Task<string> ConexaoApifixo(List<string> telefones,
                                           string field1, string field2, string field3, string field4)
        {
          
            var apiUrl = "https://vallorbeneficios.vollsc.com/api/mailings";

            //if (util.EnviadosUltimas24H(codigoAssociado, mensagemFinal))
            //{
            //    return $"⛔ Associado {codigoAssociado}: já recebeu '{mensagemFinal}' nas últimas 24h.";
            //}

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
                      ""media_hsm_configuration_id"": ""9e0bb6b4-939c-4e0f-861f-c01c0819d215"",
                      ""hsm_type"": ""media_hsm"",
                      ""campaign_id"": ""e76061ad-be13-4d72-9974-ca092fb5a5a4"",
                      ""system"":  ""whatsapp_enterprise"",
                      ""contacts"": [ 
                        {{ 
                          ""phone_number"": ""{telefone}"",
                          ""field_1"": ""{field1}"",
                          ""field_2"": ""{field2}"",
                          ""field_3"": ""{field3}"",
                          ""field_4"": ""{field4}"",
                          ""field_5"": ""{field4}""
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

        public async Task<string> ConexaoApiNovoPlano(List<string> telefones, string nomeBeneficiario, string nomeOperador)
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
                    ""media_hsm_configuration_id"": ""f28c1193-59c2-4a6b-a7c2-ba742d4c4d38"",
                            ""hsm_type"": ""media_hsm"",
                            ""campaign_id"": ""5ce46cf9-68fa-46cd-91db-542b503b8121"",
                            ""system"": ""whatsapp_enterprise"",
                            ""contacts"": [
                                {{
                                ""phone_number"": ""{telefone}"",
                                ""field_1"": ""{nomeBeneficiario}"", 
                                ""field_2"": ""{nomeOperador}"",  
                                ""field_3"": """",
                                ""field_4"": """",
                                ""field_5"": """"
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

                    await Task.Delay(5000);
                }
            }

            return resultadoFinal.ToString();
        }

        public async Task<string> ConexaoApiNovoCliente(List<string> telefones, string nomeBeneficiario, string nomeOperador)
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
                    ""media_hsm_configuration_id"": ""ca819cea-fbbe-42f2-ac65-cca4eb61620a"",
                            ""hsm_type"": ""media_hsm"",
                            ""campaign_id"": ""5ce46cf9-68fa-46cd-91db-542b503b8121"",
                            ""system"": ""whatsapp_enterprise"",
                            ""directed_campaigns_attributes"": [
                                {{
                                    ""campaign_id"": ""5ce46cf9-68fa-46cd-91db-542b503b8121""
                                }}
                            ],

                            ""contacts"": [
                                {{
                                ""phone_number"": ""{telefone}"",
                                ""field_1"": ""{nomeBeneficiario}"", 
                                ""field_2"": ""{nomeOperador}"",  
                                ""field_3"": """",
                                ""field_4"": """",
                                ""field_5"": """"
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

                    await Task.Delay(5000);
                }
            }

            return resultadoFinal.ToString();
        }

        public async Task<string> ConexaoApiAVencer(List<string> telefones, string pdfUrl, string notaFiscalUrl,
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
                    // Se não tiver nota fiscal, envia string vazia
                    string notaFiscalField = string.IsNullOrEmpty(notaFiscalUrl) ? "" : notaFiscalUrl;

                    var jsonBody = $@"
                    {{
                        ""media_hsm_configuration_id"": ""fc2efc30-9e63-4e17-901c-d472d7b0fb8e"",
                        ""hsm_type"": ""media_hsm"",
                        ""campaign_id"": ""94149ef1-e3fd-408d-a864-ed0ecbad9849"",
                        ""system"": ""whatsapp_enterprise"",
                        ""contacts"": [ 
                            {{ 
                                ""phone_number"": ""{telefone}"",
                                ""field_1"": ""{field1}"",
                                ""field_2"": ""{field2}"",
                                ""field_3"": ""{field3}"",
                                ""field_4"": ""{field4}"",
                                ""field_5"": ""{pdfUrl}"",   // URL do boleto
                                ""field_6"": ""{notaFiscalField}"" // URL da nota fiscal
                               
                            }}
                        ]
                    }}";

                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    try
                    {
                        var response = await client.PostAsync(apiUrl, content);
                        var responseBody = await response.Content.ReadAsStringAsync();

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