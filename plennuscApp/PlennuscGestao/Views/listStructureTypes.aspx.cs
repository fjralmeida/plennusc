using Plennusc.Core.Models.ModelsGestao.modelsStructure;
using Plennusc.Core.Service.ServiceGestao.TipoEstrutura;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class listStructureTypes : System.Web.UI.Page
    {
        private StructureTypeService _service;

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new StructureTypeService();

            if (!IsPostBack)
            {
                CarregarTiposEstrutura();
            }
        }

        private void CarregarTiposEstrutura(string filtro = "")
        {
            try
            {
                var listaTiposEstrutura = _service.GetTipoEstrutura(filtro);
                gvTiposEstrutura.DataSource = listaTiposEstrutura;
                gvTiposEstrutura.DataBind();

                // Armazena apenas o dicionário com os nomes dos pais
                var dicionarioPais = listaTiposEstrutura.ToDictionary(
                    t => t.CodTipoEstrutura,
                    t => t.DescTipoEstrutura
                );
                ViewState["DicionarioPais"] = dicionarioPais;
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar tipos de estrutura: {ex.Message}", "error");
            }
        }

        public string GetNomePai(object codTipoEstruturaPai)
        {
            if (codTipoEstruturaPai == null || codTipoEstruturaPai == DBNull.Value)
                return "-- Não tem --";

            try
            {
                int codPai = Convert.ToInt32(codTipoEstruturaPai);
                var dicionarioPais = ViewState["DicionarioPais"] as Dictionary<int, string>;

                if (dicionarioPais != null && dicionarioPais.ContainsKey(codPai))
                {
                    return dicionarioPais[codPai];
                }

                return $"Código: {codPai}";
            }
            catch
            {
                return "-- Não tem --";
            }
        }

        protected void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            CarregarTiposEstrutura(txtFiltro.Text.Trim());
        }

        protected void gvTiposEstrutura_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTiposEstrutura.PageIndex = e.NewPageIndex;
            CarregarTiposEstrutura(txtFiltro.Text.Trim());
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("registerStructureType.aspx");
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                int codTipoEstrutura = Convert.ToInt32(btn.CommandArgument);
                Response.Redirect($"registerStructureType.aspx?cod={codTipoEstrutura}");
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao editar: {ex.Message}", "error");
            }
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;

                // Obtém os dados da linha clicada
                int codigo = Convert.ToInt32(btn.CommandArgument);

                // Obtém os valores das células de forma segura
                string descricao = row.Cells[1].Text; // Coluna DescTipoEstrutura

                // Para a coluna NomeView (BoundField), precisamos usar DataBinder
                string view = DataBinder.Eval(row.DataItem, "NomeView")?.ToString() ?? "";

                // Chama o JavaScript para abrir o modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "AbrirModal",
                    $"abrirModalExclusao('{codigo}', '{descricao.Replace("'", "\\'")}', '{view.Replace("'", "\\'")}');", true);
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao preparar exclusão: {ex.Message}", "error");
            }
        }

        protected void btnConfirmarExclusao_Click(object sender, EventArgs e)
        {
            try
            {
                int codigo = Convert.ToInt32(hdnCodigoExclusao.Value);
                string descricao = hdnDescricaoExclusao.Value;
                string view = hdnViewExclusao.Value;

                // Verifica se existem estruturas vinculadas a este tipo
                if (_service.VerificarEstruturasVinculadas(codigo))
                {
                    MostrarMensagem($"Não é possível excluir '{descricao}' porque existem estruturas vinculadas a este tipo.", "error");
                    return;
                }

                // Verifica se existem tipos filhos (sub-tipos)
                if (_service.VerificarTiposFilhos(codigo))
                {
                    MostrarMensagem($"Não é possível excluir '{descricao}' porque existem tipos de estrutura vinculados a este.", "error");
                    return;
                }

                // Tenta excluir a view primeiro (se existir)
                bool viewExcluida = _service.ExcluirView(view);

                // Exclui o tipo de estrutura
                bool excluido = _service.ExcluirTipoEstrutura(codigo);

                if (excluido)
                {
                    string mensagemSucesso = $"Tipo de estrutura '{descricao}' excluído com sucesso!";
                    if (viewExcluida)
                    {
                        mensagemSucesso += " A view associada também foi removida.";
                    }

                    MostrarMensagemSucesso(mensagemSucesso);
                    CarregarTiposEstrutura(txtFiltro.Text.Trim());

                    // Fecha o modal via JavaScript
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModal", "fecharModal();", true);
                }
                else
                {
                    MostrarMensagem($"Erro ao excluir o tipo de estrutura '{descricao}'.", "error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao excluir: {ex.Message}", "error");
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
    }
}