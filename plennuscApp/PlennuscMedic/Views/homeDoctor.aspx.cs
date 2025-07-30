using Plennusc.Core.SqlQueries.SqlQueriesGestao.department;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.position;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using Plennusc.Core.SqlQueries.SqlQueriesMedic.department;
using Plennusc.Core.SqlQueries.SqlQueriesMedic.position;
using Plennusc.Core.SqlQueries.SqlQueriesMedic.profileMedic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscMedic.Views
{
    public partial class HomeMedic : System.Web.UI.Page
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
            PessoaDAOMedic profile = new PessoaDAOMedic();
            DataTable dt = profile.GetTotalUsuarios();

            if (dt != null && dt.Rows.Count > 0)
                lblTotalColaboradores.Text = dt.Rows[0]["Total"].ToString();
        }

        private void BuscarDepartamentos()
        {
            EmpDepartmentMedic profile = new EmpDepartmentMedic();
            DataTable dt = profile.GetTotalDepartamentos();
            if (dt != null && dt.Rows.Count > 0)
                lblTotalDepartamentos.Text = dt.Rows[0]["Total"].ToString();
        }

        private void CarregarCargos()
        {
            EmpPositionMedic profile = new EmpPositionMedic();
            DataTable dt = profile.GetTotalCargos();
            if (dt != null && dt.Rows.Count > 0)
                lblTotalCargos.Text = dt.Rows[0]["Total"].ToString();
        }
    }
}