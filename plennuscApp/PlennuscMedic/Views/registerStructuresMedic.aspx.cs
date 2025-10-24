using Plennusc.Core.Models.ModelsGestao.modelsStructure;
using Plennusc.Core.Service.ServiceGestao.TipoEstrutura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscMedic.Views
{
    public partial class registerStructuresMedic : System.Web.UI.Page
    {
        private StructureTypeService _service = new StructureTypeService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarViews();
                // Esconde os painéis inicialmente
                pnlMensagemEstruturaExistente.Visible = false;
                pnlGridEstruturas.Visible = false;
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
                // Se não tem view selecionada, esconde tudo
                pnlMensagemEstruturaExistente.Visible = false;
                pnlGridEstruturas.Visible = false;
            }
        }

        private void VerificarEstruturasExistentes(int codTipoEstrutura)
        {
            var estruturasExistentes = _service.GetTodasEstruturasPorTipo(codTipoEstrutura);

            if (estruturasExistentes.Count > 0)
            {
                // EXISTEM ESTRUTURAS - MOSTRA GRID
                pnlMensagemEstruturaExistente.Visible = true;
                pnlGridEstruturas.Visible = true;

                gvEstruturas.DataSource = estruturasExistentes;
                gvEstruturas.DataBind();

                // Mostra mensagem no toast
                MostrarMensagem($"Encontradas {estruturasExistentes.Count} estruturas para esta View.", "info");
            }
            else
            {
                // NÃO EXISTEM ESTRUTURAS - MOSTRA APENAS MENSAGEM
                pnlMensagemEstruturaExistente.Visible = true;
                pnlGridEstruturas.Visible = false;

                // Mostra mensagem no toast
                MostrarMensagem("Nenhuma estrutura encontrada para esta View. Você pode adicionar as primeiras estruturas abaixo.", "warning");
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
                    int? codEstruturaPai = null;

                    foreach (var estrutura in estruturas)
                    {
                        if (!string.IsNullOrEmpty(estrutura.nome.Trim()))
                        {
                            // PRIMEIRA ESTRUTURA É O PAI
                            if (codEstruturaPai == null)
                            {
                                var modelEstruturaPai = new structureModel
                                {
                                    CodTipoEstrutura = codTipoEstrutura,
                                    DescEstrutura = estrutura.nome.Trim(),
                                    CodEstruturaPai = null, // É o pai
                                    Conf_IsDefault = true,  // Pai é sempre principal
                                    ValorPadrao = estrutura.ordem
                                };

                                // Salva o pai e pega o código
                                int codigoPai = _service.SalvarEstrutura(modelEstruturaPai);
                                codEstruturaPai = codigoPai; // Este será o pai para TODAS as filhas
                                estruturasSalvas++;
                            }
                            else
                            {
                                // DEMAIS ESTRUTURAS SÃO FILHAS DO PRIMEIRO PAI
                                var modelEstruturaFilha = new structureModel
                                {
                                    CodTipoEstrutura = codTipoEstrutura,
                                    DescEstrutura = estrutura.nome.Trim(),
                                    CodEstruturaPai = codEstruturaPai, // Sempre o código do PRIMEIRO pai
                                    Conf_IsDefault = false, // Filhas não são principais
                                    ValorPadrao = estrutura.ordem
                                };

                                _service.SalvarEstrutura(modelEstruturaFilha);
                                estruturasSalvas++;
                            }
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

        // MÉTODOS PARA EXIBIR MENSAGENS COM SWEETALERT2
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
    }
}