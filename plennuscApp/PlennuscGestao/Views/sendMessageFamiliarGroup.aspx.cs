using Plennusc.Core.Models.ModelsGestao.modelsSendMessage;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization; // Necessário para o JSON
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlennuscApp.PlennuscGestao.Views
{
    public partial class EnvioMensagemBeneficiario : System.Web.UI.Page
    {
        private readonly FamiliarGroupService _service = new FamiliarGroupService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarFiltros();
            }
        }

        private void CarregarFiltros()
        {
            try
            {
                // 1. Operadoras
                DataTable dtOperadoras = _service.ObterOperadoras();
                ddlOperadora.DataSource = dtOperadoras;
                ddlOperadora.DataTextField = "DESCRICAO_GRUPO_CONTRATO";
                ddlOperadora.DataValueField = "CODIGO_GRUPO_CONTRATO";
                ddlOperadora.DataBind();
                ddlOperadora.Items.Insert(0, new ListItem("Todas", ""));

                // 2. Faturamento
                Dictionary<string, string> dictFaturamento = _service.ObterGruposFaturamento();
                ddlFaturamento.DataSource = dictFaturamento;
                ddlFaturamento.DataTextField = "Value";
                ddlFaturamento.DataValueField = "Key";
                ddlFaturamento.DataBind();
                ddlFaturamento.Items.Insert(0, new ListItem("Todos", ""));

                // 3. Entidades (Carrega tudo de uma vez e serializa para JSON)
                DataTable dtEntidades = _service.ObterTodasEntidades();

                var listaEntidades = new List<object>();
                foreach (DataRow row in dtEntidades.Rows)
                {
                    listaEntidades.Add(new
                    {
                        Codigo = row["CODIGO_GRUPO_PESSOAS"].ToString(),
                        Nome = row["NOME_GRUPO_PESSOAS"].ToString()
                    });
                }

                // Serializa a lista e joga no HiddenField para o JavaScript ler
                var serializer = new JavaScriptSerializer();
                hfTodosEntidades.Value = serializer.Serialize(listaEntidades);
            }
            catch (Exception ex)
            {
                MostrarToast("Erro ao carregar filtros: " + ex.Message, "error");
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfTemplateEscolhido.Value))
            {
                MostrarToast("Escolha uma mensagem antes de filtrar.", "warning");
                return;
            }

            if (string.IsNullOrEmpty(ddlTipoPlano.SelectedValue))
            {
                MostrarToast("Selecione o Tipo de Plano (Adesão ou Empresarial).", "warning");
                return;
            }

            hfSelecionados.Value = "";
            GridAssociados.PageIndex = 0;
            CarregarGrid();
        }

        private void CarregarGrid()
        {
            try
            {
                var filtro = new FiltroEnvioModel
                {
                    TipoPlano = ddlTipoPlano.SelectedValue,
                    Operadora = ddlOperadora.SelectedValue,
                    GrupoPessoas = hfEntidadeId.Value, // ID da entidade escolhida no Dropdown
                    GrupoFaturamento = ddlFaturamento.SelectedValue,
                    VencimentoInicio = !string.IsNullOrEmpty(txtDataVencimentoInicio.Text) ? Convert.ToDateTime(txtDataVencimentoInicio.Text) : (DateTime?)null,
                    VencimentoFim = !string.IsNullOrEmpty(txtDataVencimentoFim.Text) ? Convert.ToDateTime(txtDataVencimentoFim.Text) : (DateTime?)null,
                    EnvioInicio = !string.IsNullOrEmpty(txtEnvioInicio.Text) ? Convert.ToDateTime(txtEnvioInicio.Text) : (DateTime?)null,
                    EnvioFim = !string.IsNullOrEmpty(txtEnvioFim.Text) ? Convert.ToDateTime(txtEnvioFim.Text) : (DateTime?)null,
                    Mensagem = hfTemplateEscolhido.Value
                };

                var lista = _service.BuscarBeneficiarios(filtro);

                GridAssociados.DataSource = lista;
                GridAssociados.DataBind();

                lblTotalRegistros.Text = lista.Count.ToString();
                hfTodosCodigos.Value = string.Join(",", lista.Select(x => x.CodigoAssociado));
                btnTestarApi.Enabled = lista.Count > 0 && !string.IsNullOrEmpty(hfTemplateEscolhido.Value);

                if (lista.Count == 0)
                    MostrarToast("Nenhum beneficiário encontrado.", "info");
            }
            catch (Exception ex)
            {
                MostrarToast("Erro ao filtrar: " + ex.Message, "error");
            }
        }

        protected async void btnTestarApi_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSelecionados.Value))
            {
                MostrarToast("Selecione pelo menos um beneficiário.", "warning");
                return;
            }

            string mensagemEscolhida = hfTemplateEscolhido.Value;
            string dataSuspensaoStr = "";

            switch (mensagemEscolhida)
            {
                case "MSG_1": dataSuspensaoStr = txtDataMsg1.Text; break;
                case "MSG_2": dataSuspensaoStr = txtDataMsg2.Text; break;
                case "MSG_4": dataSuspensaoStr = txtDataMsg4.Text; break;
                case "MSG_5": dataSuspensaoStr = txtDataMsg5.Text; break;
            }

            try
            {
                DateTime dataSuspensao = DateTime.MinValue;
                if (!DateTime.TryParse(dataSuspensaoStr, out dataSuspensao))
                {
                    MostrarToast("Preencha a Data no card da mensagem escolhida.", "warning");
                    return;
                }

                var codigosSelecionados = hfSelecionados.Value.Split(',').ToList();
                var filtro = new FiltroEnvioModel
                {
                    TipoPlano = ddlTipoPlano.SelectedValue,
                    Operadora = ddlOperadora.SelectedValue,
                    GrupoPessoas = hfEntidadeId.Value,
                    GrupoFaturamento = ddlFaturamento.SelectedValue,
                    VencimentoInicio = !string.IsNullOrEmpty(txtDataVencimentoInicio.Text) ? Convert.ToDateTime(txtDataVencimentoInicio.Text) : (DateTime?)null,
                    VencimentoFim = !string.IsNullOrEmpty(txtDataVencimentoFim.Text) ? Convert.ToDateTime(txtDataVencimentoFim.Text) : (DateTime?)null,
                    Mensagem = mensagemEscolhida
                };

                var listaParaEnvio = _service.BuscarPorCodigos(codigosSelecionados, filtro);

                if (listaParaEnvio.Count == 0)
                {
                    MostrarToast("Nenhum registro válido encontrado.", "warning");
                    return;
                }

                int codUsuario = Convert.ToInt32(Session["codUsuario"]);
                int sucessos = await _service.EnviarMensagensEmMassa(listaParaEnvio, mensagemEscolhida, dataSuspensao, codUsuario);

                MostrarToast($"{sucessos} mensagens processadas!", "success");
                CarregarGrid();
            }
            catch (Exception ex)
            {
                MostrarToast("Erro durante o envio: " + ex.Message, "error");
            }
        }

        protected void GridAssociados_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridAssociados.PageIndex = e.NewPageIndex;
            CarregarGrid();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridAssociados.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            GridAssociados.PageIndex = 0;
            CarregarGrid();
        }

        protected void GridAssociados_PreRender(object sender, EventArgs e)
        {
            if (GridAssociados.HeaderRow != null)
            {
                GridAssociados.UseAccessibleHeader = true;
                GridAssociados.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void GridAssociados_RowDataBound(object sender, GridViewRowEventArgs e) { }

        private void MostrarToast(string mensagem, string tipo)
        {
            string script = $@"
            Swal.fire({{
                toast: true,
                position: 'top-end',
                icon: '{tipo}',
                title: '{mensagem.Replace("'", "\\'")}',
                showConfirmButton: false,
                timer: 4000
            }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Toast", script, true);
        }
    }
}