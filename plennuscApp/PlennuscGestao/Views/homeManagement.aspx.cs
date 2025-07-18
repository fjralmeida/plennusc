using Plennusc.Core.SqlQueries.SqlQueriesGestao.department;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.position;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlennuscGestao.Views
{
    public partial class HomeGestao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BuscarUsuarios();
                BuscarDepartamentos();
                CarregarCargos();
            }
        }

        private void BuscarUsuarios()
        {
            PessoaDAO profile = new PessoaDAO();
            DataTable dt = profile.GetTotalUsuarios();

            if (dt != null && dt.Rows.Count > 0)
                lblTotalColaboradores.Text = dt.Rows[0]["Total"].ToString();
        }

        private void BuscarDepartamentos()
        {
            EmpDepartment profile = new EmpDepartment();
            DataTable dt = profile.GetTotalDepartamentos();
            if (dt != null && dt.Rows.Count > 0)
                lblTotalDepartamentos.Text = dt.Rows.Count.ToString();
        }

        private void CarregarCargos()
        {
            EmpPosition profile = new EmpPosition();
            DataTable dt = profile.GetTotalCargos();
            if(dt != null && dt.Rows.Count > 0)
                lblTotalCargos.Text = dt.Rows[0]["Total"].ToString();
        }
    }
}