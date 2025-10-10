using Plennusc.Core.Models.ModelsGestao.modelsStructure;
using Plennusc.Core.Service.ServiceGestao.TipoEstrutura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class registerStructures : System.Web.UI.Page
    {
        private StructureTypeService _service = new StructureTypeService();

        // PROPRIEDADE PARA PERSISTIR O CÓDIGO DA ESTRUTURA PRINCIPAL
        private int CodEstruturaPrincipal
        {
            get { return ViewState["CodEstruturaPrincipal"] != null ? (int)ViewState["CodEstruturaPrincipal"] : 0; }
            set { ViewState["CodEstruturaPrincipal"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarViews();
            }
        }

        private void CarregarViews()
        {
            var views = _service.GetTodosTiposEstrutura();
            ddlView.DataSource = views;
            ddlView.DataTextField = "NomeView";
            ddlView.DataValueField = "CodTipoEstrutura";
            ddlView.DataBind();
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlView.SelectedValue))
            {
                int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);
                VerificarEstruturaPrincipalExistente(codTipoEstrutura);
            }
        }

        private void VerificarEstruturaPrincipalExistente(int codTipoEstrutura)
        {
            var estruturasPrincipais = _service.GetEstruturasPai(codTipoEstrutura);

            if (estruturasPrincipais.Count > 0)
            {
                // JÁ EXISTE ESTRUTURA PRINCIPAL - NÃO MOSTRA CAMPO PARA PREENCHER
                CodEstruturaPrincipal = estruturasPrincipais[0].CodEstrutura;
                pnlEstruturaPrincipal.Visible = false;
                pnlMensagemEstruturaExistente.Visible = true;
                lblMensagem.Text = $"Já existe estrutura principal: {estruturasPrincipais[0].DescEstrutura}. Adicione os subtipos abaixo.";
                lblMensagem.CssClass = "text-success";
            }
            else
            {
                // NÃO EXISTE ESTRUTURA PRINCIPAL - MOSTRA CAMPO PARA PREENCHER
                CodEstruturaPrincipal = 0;
                pnlEstruturaPrincipal.Visible = true;
                pnlMensagemEstruturaExistente.Visible = false;
                lblMensagem.Text = "Cadastre a estrutura principal primeiro";
                lblMensagem.CssClass = "text-info";
            }
        }

        protected void btnSalvarTudo_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlView.SelectedValue))
                {
                    lblMensagem.Text = "Selecione uma View";
                    lblMensagem.CssClass = "text-danger";
                    return;
                }

                int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);

                // VERIFICA NOVAMENTE SE EXISTE ESTRUTURA PRINCIPAL (PARA GARANTIR)
                if (CodEstruturaPrincipal == 0)
                {
                    var estruturasPrincipais = _service.GetEstruturasPai(codTipoEstrutura);
                    if (estruturasPrincipais.Count > 0)
                    {
                        CodEstruturaPrincipal = estruturasPrincipais[0].CodEstrutura;
                        pnlEstruturaPrincipal.Visible = false;
                        pnlMensagemEstruturaExistente.Visible = true;
                    }
                }

                // SE AINDA NÃO EXISTIR ESTRUTURA PRINCIPAL, SALVAR PRIMEIRO
                if (CodEstruturaPrincipal == 0)
                {
                    if (string.IsNullOrEmpty(txtEstruturaPrincipal.Text.Trim()))
                    {
                        lblMensagem.Text = "Informe a estrutura principal";
                        lblMensagem.CssClass = "text-danger";
                        return;
                    }

                    var modelPrincipal = new structureModel
                    {
                        CodTipoEstrutura = codTipoEstrutura,
                        DescEstrutura = txtEstruturaPrincipal.Text.Trim(),
                        Conf_IsDefault = false,
                        ValorPadrao = 0
                    };

                    CodEstruturaPrincipal = _service.SalvarEstrutura(modelPrincipal);

                    if (CodEstruturaPrincipal > 0)
                    {
                        lblMensagem.Text = $"Estrutura principal '{txtEstruturaPrincipal.Text}' salva com sucesso! ";
                        lblMensagem.CssClass = "text-success";
                    }
                    else
                    {
                        lblMensagem.Text = "Erro ao salvar estrutura principal";
                        lblMensagem.CssClass = "text-danger";
                        return;
                    }
                }

                // SALVAR SUBTIPOS DO CAMPO HIDDEN (ENVIADOS VIA JAVASCRIPT)
                var serializer = new JavaScriptSerializer();
                var subtiposJson = hdnSubtipos.Value;

                if (!string.IsNullOrEmpty(subtiposJson))
                {
                    var subtipos = serializer.Deserialize<List<string>>(subtiposJson);
                    int subtiposSalvos = 0;

                    foreach (string subtipo in subtipos)
                    {
                        if (!string.IsNullOrEmpty(subtipo.Trim()))
                        {
                            var modelSubtipo = new structureModel
                            {
                                CodTipoEstrutura = codTipoEstrutura,
                                DescEstrutura = subtipo.Trim(),
                                CodEstruturaPai = CodEstruturaPrincipal,
                                Conf_IsDefault = false,
                                ValorPadrao = 0
                            };

                            _service.SalvarEstrutura(modelSubtipo);
                            subtiposSalvos++;
                        }
                    }

                    lblMensagem.Text += $"{subtiposSalvos} subtipos salvos com sucesso!";

                    // Limpa os campos
                    hdnSubtipos.Value = "";
                    txtEstruturaPrincipal.Text = "";

                    // Limpa os campos dinâmicos via JavaScript
                    ScriptManager.RegisterStartupScript(this, GetType(), "limparCampos", "limparCamposSubtipos();", true);
                }
                else
                {
                    lblMensagem.Text += " Nenhum subtipo para salvar.";
                }
            }
            catch (Exception ex)
            {
                lblMensagem.Text = "Erro: " + ex.Message;
                lblMensagem.CssClass = "text-danger";
            }
            finally
            {
                // 🔥 ADICIONE ESTAS LINHAS NO FINAL DO MÉTODO
                pnlMensagem.Visible = true;
                divMensagem.Attributes["class"] = lblMensagem.CssClass.Contains("success") ? "message message-success" :
                                                  lblMensagem.CssClass.Contains("danger") ? "message message-error" :
                                                  "message message-info";
            }
        }
    }
}