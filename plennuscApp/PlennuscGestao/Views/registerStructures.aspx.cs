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
            ddlView.DataTextField = "DescTipoEstrutura";
            ddlView.DataValueField = "CodTipoEstrutura";
            ddlView.DataBind();
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlView.SelectedValue))
            {
                int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);
                VerificarEstruturasExistentes(codTipoEstrutura);
            }
            else
            {
                pnlMensagemEstruturaExistente.Visible = false;
                pnlGridEstruturas.Visible = false;
            }
        }

        private void VerificarEstruturasExistentes(int codTipoEstrutura)
        {
            var estruturasExistentes = _service.GetTodasEstruturasPorTipo(codTipoEstrutura);

            if (estruturasExistentes.Count > 0)
            {
                pnlMensagemEstruturaExistente.Visible = true;
                pnlGridEstruturas.Visible = true;
                gvEstruturas.DataSource = estruturasExistentes;
                gvEstruturas.DataBind();
                MostrarMensagem($"Encontradas {estruturasExistentes.Count} estruturas para esta View.", "info");
            }
            else
            {
                pnlMensagemEstruturaExistente.Visible = true;
                pnlGridEstruturas.Visible = false;
                MostrarMensagem("Nenhuma estrutura encontrada para esta View. Você pode adicionar as primeiras estruturas abaixo.", "warning");
            }
        }

        protected void gvEstruturas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Excluir")
            {
                int codEstrutura = Convert.ToInt32(e.CommandArgument);
                ExcluirEstrutura(codEstrutura);
            }
        }


        protected void btnSalvarEdicao_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid && !string.IsNullOrEmpty(hdnCodEstruturaEditar.Value))
                {
                    var estrutura = new structureModel
                    {
                        CodEstrutura = Convert.ToInt32(hdnCodEstruturaEditar.Value),
                        DescEstrutura = txtDescEstruturaEditar.Text.Trim(),
                        ValorPadrao = Convert.ToInt32(txtValorPadraoEditar.Text),
                        MemoEstrutura = string.IsNullOrEmpty(txtMemoEstruturaEditar.Text) ? null : txtMemoEstruturaEditar.Text.Trim(),
                        InfoEstrutura = string.IsNullOrEmpty(txtInfoEstruturaEditar.Text) ? null : txtInfoEstruturaEditar.Text.Trim()
                    };

                    bool atualizado = _service.AtualizarEstrutura(estrutura);

                    if (atualizado)
                    {
                        //// MENSAGEM SIMPLES QUE SEMPRE FUNCIONA
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "sucesso",
                        //    "alert('Estrutura atualizada com sucesso!');", true);

                        // Recarrega o grid
                        if (!string.IsNullOrEmpty(ddlView.SelectedValue))
                        {
                            int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);
                            VerificarEstruturasExistentes(codTipoEstrutura);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "erro",
                            "alert('Erro ao atualizar estrutura!');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "erro2",
                    $"alert('Erro: {ex.Message.Replace("'", "")}');", true);
            }
        }
        protected void gvEstruturas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Lógica adicional se necessário
            }
        }

        private void ExcluirEstrutura(int codEstrutura)
        {
            try
            {
                bool temFilhos = _service.VerificarEstruturasFilhas(codEstrutura);

                if (temFilhos)
                {
                    // Usa SweetAlert para mensagem de erro
                    string script = @"Swal.fire({icon: 'error', title: 'Erro', text: 'Não é possível excluir: existem estruturas filhas vinculadas.'});";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErroExclusao", script, true);
                    return;
                }

                bool excluido = _service.ExcluirEstrutura(codEstrutura);

                if (excluido)
                {
                    // Usa SweetAlert para mensagem de sucesso
                    string script = @"Swal.fire({icon: 'success', title: 'Sucesso', text: 'Estrutura excluída com sucesso!'});";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "SucessoExclusao", script, true);

                    // Recarrega o grid se houver uma view selecionada
                    if (!string.IsNullOrEmpty(ddlView.SelectedValue))
                    {
                        int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);
                        VerificarEstruturasExistentes(codTipoEstrutura);
                    }
                }
                else
                {
                    string script = @"Swal.fire({icon: 'error', title: 'Erro', text: 'Erro ao excluir a estrutura.'});";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ErroExclusao2", script, true);
                }
            }
            catch (Exception ex)
            {
                string script = $@"Swal.fire({{icon: 'error', title: 'Erro', text: 'Erro ao excluir: {ex.Message.Replace("'", "\\'")}'}});";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErroExclusao3", script, true);
            }
        }

        protected void btnSalvarTudo_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlView.SelectedValue))
                {
                    MostrarMensagem("Selecione uma View", "error");
                    return;
                }

                int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);

                var serializer = new JavaScriptSerializer();
                var estruturasJson = hdnSubtipos.Value;

                if (!string.IsNullOrEmpty(estruturasJson))
                {
                    var estruturas = serializer.Deserialize<List<subTypeDate>>(estruturasJson);
                    int estruturasSalvas = 0;

                    int codEstruturaPai = _service.BuscarPaiPorTipoEstrutura(codTipoEstrutura);

                    foreach (var estrutura in estruturas)
                    {
                        if (!string.IsNullOrEmpty(estrutura.nome.Trim()))
                        {
                            var modelEstruturaFilha = new structureModel
                            {
                                CodTipoEstrutura = codTipoEstrutura,
                                DescEstrutura = estrutura.nome.Trim(),
                                CodEstruturaPai = codEstruturaPai,
                                Conf_IsDefault = false,
                                ValorPadrao = estrutura.ordem
                            };

                            _service.SalvarEstrutura(modelEstruturaFilha);
                            estruturasSalvas++;
                        }
                    }

                    if (estruturasSalvas > 0)
                    {
                        MostrarMensagemSucesso($"{estruturasSalvas} estruturas salvas com sucesso!");
                        hdnSubtipos.Value = "";
                        ScriptManager.RegisterStartupScript(this, GetType(), "limparCampos", "limparCamposSubtipos();", true);
                        VerificarEstruturasExistentes(codTipoEstrutura);
                    }
                    else
                    {
                        MostrarMensagem("Nenhuma estrutura válida para salvar.", "warning");
                    }
                }
                else
                {
                    MostrarMensagem("Nenhuma estrutura para salvar.", "warning");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro: {ex.Message}", "error");
            }
        }

        private void MostrarMensagemSucesso(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Sucesso',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastSucesso", script, true);
        }

        private void MostrarMensagem(string mensagem, string tipo = "success")
        {
            string titulo;
            switch (tipo.ToLower())
            {
                case "success":
                    titulo = "Sucesso";
                    break;
                case "error":
                    titulo = "Erro";
                    break;
                case "warning":
                    titulo = "Atenção";
                    break;
                case "info":
                    titulo = "Informação";
                    break;
                default:
                    titulo = "Mensagem";
                    break;
            }

            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: '{tipo}',
                    title: '{titulo}',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 4000,
                    timerProgressBar: true
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastMsg", script, true);
        }

        protected void btnConfirmarExclusao_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnCodEstruturaExcluir.Value))
            {
                int codEstrutura = Convert.ToInt32(hdnCodEstruturaExcluir.Value);
                ExcluirEstrutura(codEstrutura);

                // Fecha o modal via JavaScript
                string script = @"var modal = bootstrap.Modal.getInstance(document.getElementById('modalConfirmarExclusao')); modal.hide();";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModal", script, true);
            }
        }
    }
}