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

        public DataTable ObterOperadoras() => _query.ObterOperadoras();
        public DataTable ObterTodasEntidades() => _query.ObterTodasEntidades();
        public DataTable ObterGruposFaturamento() => _query.ObterGruposFaturamento();

        public List<GrupoFamiliarEnvioModel> BuscarBeneficiarios(FiltroEnvioModel filtro)
        {
            // 1. Traz os dados do Aliança (sem data de vencimento convertida no banco)
            var lista = _query.BuscarBeneficiarios(filtro, "PS1000");

            // 2. Filtro de Data no C#
            if (filtro.VencimentoInicio.HasValue)
                lista = lista.Where(x => x.DataVencimento >= filtro.VencimentoInicio.Value).ToList();

            if (filtro.VencimentoFim.HasValue)
                lista = lista.Where(x => x.DataVencimento <= filtro.VencimentoFim.Value).ToList();

            // 3. Regras das Mensagens
            if (filtro.Mensagem == "MSG_1")
            {
                lista = lista.Where(x => x.DataVencimento >= DateTime.Today).ToList();
            }
            else if (filtro.Mensagem == "MSG_2" || filtro.Mensagem == "MSG_4" || filtro.Mensagem == "MSG_5")
            {
                var agrupado = lista.GroupBy(x => x.CodigoAssociado).ToList();
                var listaFiltrada = new List<GrupoFamiliarEnvioModel>();

                foreach (var grupo in agrupado)
                {
                    int boletosVencidos = grupo.Count(x => x.DataVencimento < DateTime.Today);

                    if (filtro.Mensagem == "MSG_2" || filtro.Mensagem == "MSG_4")
                    {
                        if (boletosVencidos == 1) listaFiltrada.AddRange(grupo);
                    }
                    else if (filtro.Mensagem == "MSG_5")
                    {
                        if (boletosVencidos >= 2) listaFiltrada.AddRange(grupo);
                    }
                }
                lista = listaFiltrada;
            }

            // 4. Filtro do Plennus (Quem já recebeu)
            var chavesJaEnviadas = _query.ObterCodigosEnviadosNoPeriodo(filtro.EnvioInicio, filtro.EnvioFim, filtro.Mensagem);
            lista = lista.Where(x => !chavesJaEnviadas.Contains(x.ChaveEnvio)).ToList();

            // 5. Trava de segurança e Mês de referência
            foreach (var item in lista)
            {
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
            return todos.Where(x => codigos.Contains(x.ChaveEnvio)).ToList();
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
                        case "MSG_1": break;
                        case "MSG_2":
                            resultado = await _api.ConexaoApiSuspensao(
                                telefones, item.NumeroRegistroBoleto.ToString(), item.NomeTitular,
                                item.GrupoContrato, item.DataVencimento.ToString("dd/MM/yyyy"),
                                item.ValorBoleto.ToString("N2"), textoMensagem, item.ChaveEnvio, codAutenticacao
                            );
                            break;
                        case "MSG_4": break;
                        case "MSG_5": break;
                    }

                    await Task.Delay(500);
                    enviados++;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao enviar para {item.ChaveEnvio}: {ex.Message}");
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
                case "MSG_1": return $"Boa tarde!\nPrezado(a) Sr.(a) {nome},\n... mensalidade de {mes} do seu plano de saúde da operadora {operadora}, com vencimento em {vencimento}...";
                case "MSG_2": return $"Olá, {nome}! ... pagamento no valor de R$ {valor}, referente à mensalidade do mês de {mes} do seu plano {operadora}... a partir do dia {dataCancelamento} o serviço poderá ser cancelado...";
                case "MSG_4": return $"Olá, {nome}! ... pagamento no valor de R$ {valor}, referente à mensalidade do mês de {mes} do seu plano {operadora}... a partir do dia {dataCancelamento}, o serviço poderá ser cancelado em definitivo...";
                case "MSG_5": return $"Boa tarde!\nPrezado(a) Sr.(a) {nome},\n... seguem anexos os boletos referentes às mensalidades em atraso do seu plano de saúde da operadora {operadora}... deverão ser pagos até o dia {dataCancelamento}...";
                default: return "Mensagem não implementada.";
            }
        }
    }
}