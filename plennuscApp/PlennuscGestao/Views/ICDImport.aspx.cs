using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Plennusc.Core.Service.ServiceGestao.CIDsService;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class ICDImport : System.Web.UI.Page
    {
        private const string SESSION_KEY = "ICDResult";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Nada especial aqui. Paginação nativa não precisa de recriação manual.
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            if (!fileUploadExcel.HasFile)
            {
                ExibirErro("Selecione um arquivo Excel.");
                return;
            }
            if (!DateTime.TryParse(txtVigencia.Text, out var vigencia))
            {
                ExibirErro("Vigência inválida.");
                return;
            }

            var connSettings = ConfigurationManager.ConnectionStrings["Alianca"];
            if (connSettings == null)
            {
                ExibirErro("Connection string 'Alianca' não encontrada.");
                return;
            }

            var service = new serviceCIDs(connSettings.ConnectionString);
            var resultados = service.ProcessarImportacao(fileUploadExcel.PostedFile.InputStream, vigencia);

            // Guarda na SESSION (não usa ViewState para a lista)
            Session[SESSION_KEY] = resultados;

            // Aplica o tamanho de página atual (vindo do DropDown)
            gridTodos.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gridTodos.PageIndex = 0;
            gridTodos.DataSource = resultados;
            gridTodos.DataBind();

            gridTodos.PageIndex = 0;
            gridTodos.DataSource = resultados;
            gridTodos.DataBind();

            litCountTodos.Text = resultados.Count.ToString();

            var importados = resultados.Where(r => r.Sucesso).ToList();
            gridImportados.DataSource = importados;
            gridImportados.DataBind();
            litCountImportados.Text = importados.Count.ToString();

            var jaCadastrados = resultados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("Já cadastrado")).ToList();
            gridJaCadastrados.DataSource = jaCadastrados;
            gridJaCadastrados.DataBind();
            litCountJaCadastrados.Text = jaCadastrados.Count.ToString();

            var divergencia = resultados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("Data de admissão")).ToList();
            gridDivergencia.DataSource = divergencia;
            gridDivergencia.DataBind();
            litCountDivergencia.Text = divergencia.Count.ToString();

            var cidInvalido = resultados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("tabela de domínio")).ToList();
            gridCidInvalido.DataSource = cidInvalido;
            gridCidInvalido.DataBind();
            litCountCidInvalido.Text = cidInvalido.Count.ToString();

            var naoEncontrado = resultados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("não encontrado na PS1000")).ToList();
            gridNaoEncontrado.DataSource = naoEncontrado;
            gridNaoEncontrado.DataBind();
            litCountNaoEncontrado.Text = naoEncontrado.Count.ToString();

            pnlResultado.Visible = true;
        }

        // ============================================================
        //  PAGINAÇÃO NATIVA DO GRIDVIEW
        // ============================================================

        protected void gridTodos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridTodos.PageIndex = e.NewPageIndex;
            BindGridTodos();
        }

        private void BindGridTodos()
        {
            var dados = Session[SESSION_KEY] as System.Collections.IList;
            if (dados == null)
            {
                ExibirErro("Os dados da importação expiraram. Realize uma nova importação.");
                pnlResultado.Visible = false;
                return;
            }

            // Sincroniza o DropDown com o PageSize atual (caso seja alterado por código)
            if (ddlPageSize.Items.FindByValue(gridTodos.PageSize.ToString()) != null)
                ddlPageSize.SelectedValue = gridTodos.PageSize.ToString();

            gridTodos.DataSource = dados;
            gridTodos.DataBind();
        }

        protected void gridTodos_DataBound(object sender, EventArgs e)
        {
            var dados = Session[SESSION_KEY] as System.Collections.IList;
            int total = dados?.Count ?? 0;
            int pagina = gridTodos.PageIndex;
            int tamanho = gridTodos.PageSize;
            int inicio = total == 0 ? 0 : (pagina * tamanho) + 1;
            int fim = Math.Min(inicio + gridTodos.Rows.Count - 1, total);

            lblPagerInfo.Text = total == 0
                ? ""
                : $"<strong>{inicio} - {fim}</strong> de <strong>{total}</strong> itens";
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Altera o PageSize da grid
            gridTodos.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            // Volta para a primeira página (opcional, mas recomendado)
            gridTodos.PageIndex = 0;
            // Rebind com os dados da SESSION
            BindGridTodos();
        }

        // ============================================================
        //  AUXILIARES
        // ============================================================

        protected string GetStatusCss(object sucesso, object motivo)
        {
            if (sucesso != null && sucesso.Equals(true))
                return "status-ok";

            var motivoTexto = motivo?.ToString() ?? string.Empty;
            if (motivoTexto.Contains("Já cadastrado"))
                return "status-nao-encontrado";
            if (motivoTexto.Contains("Data de admissão"))
                return "status-divergencia-tolerada";
            if (motivoTexto.Contains("tabela de domínio"))
                return "status-divergente";
            if (motivoTexto.Contains("não encontrado na PS1000"))
                return "status-nao-encontrado";
            return "status-divergente";
        }

        protected void gridTodos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Não pinta linha
        }

        private void ExibirErro(string mensagem)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "erro", $"alert('{mensagem}');", true);
        }
    }
}