using Plennusc.Core.Models.ModelsGestao.modelsStructure;
using Plennusc.Core.Service.ServiceGestao.TipoEstrutura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class registerStructureType : System.Web.UI.Page
    {
        private StructureTypeService _service;

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new StructureTypeService();
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
          {
                if (string.IsNullOrEmpty(txtDescricao.Text.Trim()))
                {
                    lblMensagem.Text = "Informe a descrição";
                    lblMensagem.CssClass = "text-danger";
                    return;
                }

                var nomeView = "VW_" + txtDescricao.Text.Trim().ToUpper().Replace(" ", "_");
                var descricao = txtDescricao.Text.Trim();

                // Valida se view já existe
                if (_service.ViewExiste(nomeView))
                {
                    lblMensagem.Text = $"Erro: A view '{nomeView}' já existe no banco!";
                    lblMensagem.CssClass = "text-danger";
                    return;
                }

                // Cria o model
                var model = new structureTypeModel
                {
                    DescTipoEstrutura = descricao,
                    Editavel = chkEditavel.Checked,
                    NomeView = nomeView
                };

                // Salva o TipoEstrutura
                int codigo = _service.SalvarTipoEstrutura(model);

                if (codigo > 0)
                {
                    // Cria a View
                    bool viewCriada = _service.CriarView(nomeView, descricao);

                    if (viewCriada)
                    {
                        lblMensagem.Text = $"Sucesso! Tipo Estrutura salvo (Código: {codigo}) e View '{nomeView}' criada.";
                        lblMensagem.CssClass = "text-success";

                        // Limpa o formulário
                        txtDescricao.Text = "";
                        chkEditavel.Checked = false;
                    }
                    else
                    {
                        lblMensagem.Text = $"Tipo Estrutura salvo (Código: {codigo}), mas houve erro ao criar a view.";
                        lblMensagem.CssClass = "text-warning";
                    }
                }
                else
                {
                    lblMensagem.Text = "Erro ao salvar Tipo Estrutura.";
                    lblMensagem.CssClass = "text-danger";
                }
            }
            catch (Exception ex)
            {
                lblMensagem.Text = "Erro: " + ex.Message;
                lblMensagem.CssClass = "text-danger";
            }
            finally
            {
                // 🔥 ADICIONE ESTAS LINHAS NO FINAL
                pnlMensagem.Visible = true;
                divMensagem.Attributes["class"] = lblMensagem.CssClass.Contains("success") ? "message message-success" :
                                                  lblMensagem.CssClass.Contains("danger") ? "message message-error" :
                                                  "message message-info";
            }
        }
    }
}