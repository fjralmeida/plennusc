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

                // SALVAR AS ESTRUTURAS (AGORA TODAS SÃO "PAI")
                var serializer = new JavaScriptSerializer();
                var estruturasJson = hdnSubtipos.Value;

                if (!string.IsNullOrEmpty(estruturasJson))
                {
                    var estruturas = serializer.Deserialize<List<subTypeDate>>(estruturasJson);
                    int estruturasSalvas = 0;
                    bool algumPrincipal = false;

                    foreach (var estrutura in estruturas)
                    {
                        if (!string.IsNullOrEmpty(estrutura.nome.Trim()))
                        {
                            // Verifica se já existe algum marcado como principal
                            if (estrutura.isDefault)
                            {
                                if (algumPrincipal)
                                {
                                    // Já existe um principal, não marca este
                                    estrutura.isDefault = false;
                                }
                                else
                                {
                                    algumPrincipal = true;
                                }
                            }

                            var modelEstrutura = new structureModel
                            {
                                CodTipoEstrutura = codTipoEstrutura,
                                DescEstrutura = estrutura.nome.Trim(),
                                CodEstruturaPai = null, // Agora sempre NULL
                                Conf_IsDefault = estrutura.isDefault,
                                ValorPadrao = estrutura.ordem
                            };

                            _service.SalvarEstrutura(modelEstrutura);
                            estruturasSalvas++;
                        }
                    }

                    if (estruturasSalvas > 0)
                    {
                        MostrarMensagemSucesso($"{estruturasSalvas} estruturas salvas com sucesso!");

                        // Limpa os campos e recarrega o grid
                        hdnSubtipos.Value = "";
                        ScriptManager.RegisterStartupScript(this, GetType(), "limparCampos", "limparCamposSubtipos();", true);

                        // Recarrega as estruturas existentes
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