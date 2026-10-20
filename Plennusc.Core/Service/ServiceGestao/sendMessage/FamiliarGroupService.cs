using Plennusc.Core.Models.ModelsGestao.modelsSendMessage;
using Plennusc.Core.SqlQueries.SqlQueriesGestao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using appWhatsapp.Service;

namespace Plennusc.Core.Service.ServiceGestao
{
    public class FamiliarGroupService
    {
        private readonly GrupoFamiliarQuery _query = new GrupoFamiliarQuery();
        private readonly WhatsappService _api = new WhatsappService();

        // POPULAR OS FILTROS
        public DataTable ObterOperadoras()
        {
            return _query.ObterOperadoras();
        }

        public DataTable ObterTodasEntidades()
        {
            return _query.ObterTodasEntidades();
        }

        public Dictionary<string, string> ObterGruposFaturamento()
        {
            return new Dictionary<string, string>
            {
                { "400", "Boleto pago pelo beneficiário (400)" },
                { "0", "Boleto pago por empresa/entidade" }
            };
        }

        // BUSCAR E ENVIAR
        public List<GrupoFamiliarEnvioModel> BuscarBeneficiarios(FiltroEnvioModel filtro)
        {
            var lista = _query.BuscarBeneficiarios(filtro, "PS1000");

            foreach (var item in lista)
            {
                // TRAVA DE SEGURANÇA PARA HOMOLOGAÇÃO! (SEU TELEFONE)
                item.Telefone = "5531971274471";

                if (item.DataVencimento != DateTime.MinValue)
                {
                    item.MesReferencia = item.DataVencimento.ToString("MMMM", new System.Globalization.CultureInfo("pt-BR")).ToUpper();
                }
            }
            return lista;
        }

        public List<GrupoFamiliarEnvioModel> BuscarPorCodigos(List<string> codigos, FiltroEnvioModel filtro)
        {
            var todos = BuscarBeneficiarios(filtro);
            return todos.Where(x => codigos.Contains(x.CodigoAssociado)).ToList();
        }

        public async Task<int> EnviarMensagensEmMassa(List<GrupoFamiliarEnvioModel> lista, string tipoMensagem, DateTime dataSuspensao, int codAutenticacao)
        {
            int enviados = 0;

            foreach (var item in lista)
            {
                string textoMensagem = MontarMensagemTexto(item, tipoMensagem, dataSuspensao);
                List<string> telefones = new List<string> { item.Telefone };
                string resultado = string.Empty;

                try
                {
                    switch (tipoMensagem)
                    {
                        case "MSG_1":
                            // Chamar API MSG 1
                            break;
                        case "MSG_2":
                            resultado = await _api.ConexaoApiSuspensao(
                                telefones,
                                item.NumeroRegistroBoleto.ToString(),
                                item.NomeTitular,
                                item.GrupoContrato,
                                item.DataVencimento.ToString("dd/MM/yyyy"),
                                item.ValorBoleto.ToString("N2"),
                                textoMensagem,
                                item.CodigoAssociado,
                                codAutenticacao
                            );
                            break;
                        case "MSG_4":
                            // Chamar API MSG 4
                            break;
                        case "MSG_5":
                            // Chamar API MSG 5
                            break;
                    }

                    await Task.Delay(500);
                    enviados++;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao enviar para {item.CodigoAssociado}: {ex.Message}");
                }
            }
            return enviados;
        }

        public string MontarMensagemTexto(GrupoFamiliarEnvioModel bene, string tipoMensagem, DateTime dataSuspensao)
        {
            string nome = bene.NomeTitular.ToUpper();
            string mes = bene.MesReferencia;
            string operadora = bene.GrupoContrato.ToUpper();
            string vencimento = bene.DataVencimento.ToString("dd/MM/yyyy");
            string valor = bene.ValorBoleto.ToString("N2");
            string dataCancelamento = dataSuspensao.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));

            switch (tipoMensagem)
            {
                case "MSG_1":
                    return $"Boa tarde!\nPrezado(a) Sr.(a) {nome},\n... mensalidade de {mes} do seu plano de saúde da operadora {operadora}, com vencimento em {vencimento}...";
                case "MSG_2":
                    return $"Olá, {nome}! ... pagamento no valor de R$ {valor}, referente à mensalidade do mês de {mes} do seu plano {operadora}... a partir do dia {dataCancelamento} o serviço poderá ser cancelado...";
                case "MSG_4":
                    return $"Olá, {nome}! ... pagamento no valor de R$ {valor}, referente à mensalidade do mês de {mes} do seu plano {operadora}... a partir do dia {dataCancelamento}, o serviço poderá ser cancelado em definitivo...";
                case "MSG_5":
                    return $"Boa tarde!\nPrezado(a) Sr.(a) {nome},\n... seguem anexos os boletos referentes às mensalidades em atraso do seu plano de saúde da operadora {operadora}... deverão ser pagos até o dia {dataCancelamento}...";
                default:
                    return "Mensagem não implementada.";
            }
        }
    }
}