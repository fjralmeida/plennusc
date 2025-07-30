using Plennusc.Core.SqlQueries.SqlQueriesGestao.department;
using Plennusc.Core.SqlQueries.SqlQueriesMedic.department;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscMedic.Views
{
    public partial class employeeDepartmentMedic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CarregarDepartamentos();
        }

        private void CarregarDepartamentos()
        {
            EmpDepartmentMedic department = new EmpDepartmentMedic();
            DataTable dt = department.GetDepartments();

            if (dt != null && dt.Rows.Count > 0)
            {
                gvDepartments.DataSource = dt;
                gvDepartments.DataBind();
            }
            else
            {
                gvDepartments.DataSource = null;
                gvDepartments.DataBind();
            }
        }
    }
}