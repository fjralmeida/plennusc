using Plennusc.Core.Models.ModelsGestao.modelsSendMessage;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
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
                DataTable dtOperadoras = _service.ObterOperadoras();
                ddlOperadora.DataSource = dtOperadoras;
                ddlOperadora.DataTextField = "DESCRICAO_GRUPO_CONTRATO";
                ddlOperadora.DataValueField = "CODIGO_GRUPO_CONTRATO";
                ddlOperadora.DataBind();
                ddlOperadora.Items.Insert(0, new ListItem("Todas", ""));

                DataTable dtFaturamento = _service.ObterGruposFaturamento();
                ddlFaturamento.DataSource = dtFaturamento;
                ddlFaturamento.DataTextField = "DESCRICAO_GP_FATURAMENTO";
                ddlFaturamento.DataValueField = "CODIGO_GRUPO_FATURAMENTO";
                ddlFaturamento.DataBind();
                ddlFaturamento.Items.Insert(0, new ListItem("Todos", ""));

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
                var serializer = new JavaScriptSerializer();
                hfTodosEntidades.Value = serializer.Serialize(listaEntidades);
            }
            catch (Exception ex)
            {
                MostrarToast("Erro ao carregar filtros: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine("ERRO CARREGAR FILTROS: " + ex.Message);
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
                DateTime vencInicio = new DateTime(1900, 1, 1);
                if (!string.IsNullOrEmpty(txtDataVencimentoInicio.Text))
                    vencInicio = Convert.ToDateTime(txtDataVencimentoInicio.Text);

                DateTime vencFim = new DateTime(2099, 12, 31);
                if (!string.IsNullOrEmpty(txtDataVencimentoFim.Text))
                    vencFim = Convert.ToDateTime(txtDataVencimentoFim.Text);

                var filtro = new FiltroEnvioModel
                {
                    TipoPlano = ddlTipoPlano.SelectedValue,
                    Operadora = ddlOperadora.SelectedValue,
                    GrupoPessoas = hfEntidadeId.Value,
                    GrupoFaturamento = ddlFaturamento.SelectedValue,
                    VencimentoInicio = vencInicio,
                    VencimentoFim = vencFim,
                    EnvioInicio = null,
                    EnvioFim = null,
                    Mensagem = hfTemplateEscolhido.Value
                };

                var lista = _service.BuscarBeneficiarios(filtro);

                GridAssociados.DataSource = lista;
                GridAssociados.DataBind();

                lblTotalRegistros.Text = lista.Count.ToString();
                hfTodosCodigos.Value = string.Join(",", lista.Select(x => x.ChaveEnvio));
                btnTestarApi.Enabled = lista.Count > 0 && !string.IsNullOrEmpty(hfTemplateEscolhido.Value);

                if (lista.Count == 0)
                    MostrarToast("Nenhum beneficiário encontrado para os filtros selecionados.", "info");
            }
            catch (Exception ex)
            {
                MostrarToast("Erro ao filtrar: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine("ERRO FILTRAR: " + ex.Message);
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

                DateTime vencInicio = new DateTime(1900, 1, 1);
                if (!string.IsNullOrEmpty(txtDataVencimentoInicio.Text))
                    vencInicio = Convert.ToDateTime(txtDataVencimentoInicio.Text);

                DateTime vencFim = new DateTime(2099, 12, 31);
                if (!string.IsNullOrEmpty(txtDataVencimentoFim.Text))
                    vencFim = Convert.ToDateTime(txtDataVencimentoFim.Text);

                var filtro = new FiltroEnvioModel
                {
                    TipoPlano = ddlTipoPlano.SelectedValue,
                    Operadora = ddlOperadora.SelectedValue,
                    GrupoPessoas = hfEntidadeId.Value,
                    GrupoFaturamento = ddlFaturamento.SelectedValue,
                    VencimentoInicio = vencInicio,
                    VencimentoFim = vencFim,
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