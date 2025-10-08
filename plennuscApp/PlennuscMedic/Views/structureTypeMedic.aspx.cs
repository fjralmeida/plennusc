using Plennusc.Core.Service.ServiceGestao;
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
    public partial class structureTypeMedic : System.Web.UI.Page
    {
        private DemandaService _demandaSvc = new DemandaService();
        private StructureTypeService _structureSvc = new StructureTypeService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarSetores();
                pnlEstruturas.Visible = false;
            }
        }

        private void CarregarSetores()
        {
            ddlSetor.DataSource = _demandaSvc.GetDepartamentos();
            ddlSetor.DataValueField = "Value";
            ddlSetor.DataTextField = "Text";
            ddlSetor.DataBind();
            ddlSetor.Items.Insert(0, new ListItem("Selecione um setor", ""));
        }

        protected void ddlSetor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlSetor.SelectedValue))
            {
                pnlEstruturas.Visible = true;
                //CarregarEstruturasDoSetor();
                LimparFormulario();
            }
            else
            {
                pnlEstruturas.Visible = false;
            }
        }

        protected void btnSalvarEstrutura_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlSetor.SelectedValue))
            {
                MostrarMensagem("Selecione um setor primeiro.", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNomeEstrutura.Text))
            {
                MostrarMensagem("Informe o nome da categoria.", false);
                return;
            }

            try
            {
                int codSetor = int.Parse(ddlSetor.SelectedValue);
                string nomeEstrutura = txtNomeEstrutura.Text.Trim();

                // Coletar serviços do hidden field (JSON)
                List<string> subtipos = new List<string>();
                if (!string.IsNullOrEmpty(hdnSubtipos.Value))
                {
                    var serializer = new JavaScriptSerializer();
                    subtipos = serializer.Deserialize<List<string>>(hdnSubtipos.Value);
                }

                // Validar se tem pelo menos um serviço
                if (subtipos.Count == 0)
                {
                    MostrarMensagem("Adicione pelo menos um serviço.", false);
                    return;
                }

                // PASSO 1: Salvar categoria pai no banco de dados (SEM CodSetor)
                int codEstruturaPai = _structureSvc.InserirEstruturaPai(nomeEstrutura, "");

                if (codEstruturaPai > 0)
                {
                    // PASSO 2: Inserir serviços (subtipos)
                    foreach (string subtipo in subtipos)
                    {
                        if (!string.IsNullOrWhiteSpace(subtipo))
                        {
                            _structureSvc.InserirSubtipo(codEstruturaPai, subtipo.Trim());
                        }
                    }

                    // PASSO 3: Vincular categoria ao setor na tabela SetorTipoDemanda
                    _structureSvc.VincularSetorEstrutura(codSetor, codEstruturaPai);

                    MostrarMensagem("Categoria cadastrada com sucesso!", true);
                    LimparFormulario();
                    //CarregarEstruturasDoSetor();
                }
                else
                {
                    MostrarMensagem("Erro ao cadastrar categoria.", false);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao salvar categoria: {ex.Message}", false);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            pnlEstruturas.Visible = false;
        }

        private void LimparFormulario()
        {
            txtNomeEstrutura.Text = "";
            hdnSubtipos.Value = "";

            // Limpar a lista de serviços visualmente
            ScriptManager.RegisterStartupScript(this, GetType(), "LimparServicos",
                "servicos = []; atualizarListaServicos(); atualizarContador();", true);
        }

        private void MostrarMensagem(string mensagem, bool sucesso)
        {
            string script = sucesso
                ? $"alert('{mensagem}');"
                : $"alert('{mensagem}');";

            ScriptManager.RegisterStartupScript(this, GetType(), "Mensagem", script, true);
        }
    }
}