using Plennusc.Core.SqlQueries.SqlQueriesMedic.interview;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscMedic.Views
{
    public partial class medicalInterview : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CarregarDados();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CarregarDados();
        }

        private void CarregarDados()
        {
            string status = ddlStatus.SelectedValue;
            //string empresa = ddlEmpresa.SelectedValue;
            //string plano = ddlPlano.SelectedValue;
            string busca = txtBusca.Text.Trim();


            MedicalInterview InterviewMedical = new MedicalInterview();
            DataTable dtInterview = InterviewMedical.GetInterview(status, busca);

            gvAssociados.DataSource = dtInterview;
            gvAssociados.DataBind();
        }

        public string GetStatusClass(string status)
        {
            switch (status)
            {
                case "AGUARDANDO_AVALIACAO": return "status-aguardando";
                case "EM_ESPERA": return "status-em-espera";
                case "APROVADO": return "status-aprovado";
                default: return "";
            }
        }

        protected void gvAssociados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "VerDetalhes")
            {
                int codigoAssociado = Convert.ToInt32(e.CommandArgument);
                CarregarDetalhesAssociado(codigoAssociado);

                //ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "abrirModalDetalhes();", true);
            }
        }
        private void CarregarDetalhesAssociado(int codigoAssociado)
        {
            MedicalInterview interviewMedical = new MedicalInterview();
            DataTable dtDetalhes = interviewMedical.GetDetalhesAssociado(codigoAssociado);

            if (dtDetalhes.Rows.Count > 0)
            {
                DataRow row = dtDetalhes.Rows[0];

                // Montar HTML com os detalhes
                StringBuilder html = new StringBuilder();
                foreach (DataColumn col in dtDetalhes.Columns)
                {
                    html.Append($"<p><strong>{col.ColumnName}:</strong> {row[col] ?? ""}</p>");
                }

                //detalhesConteudo.InnerHtml = html.ToString();
            }
            else
            {
                //detalhesConteudo.InnerHtml = "<p class='text-danger'>Nenhum detalhe encontrado.</p>";
            }
        }
    }
}
