using Plennusc.Core.SqlQueries.SqlQueriesGestao.position;
using Plennusc.Core.SqlQueries.SqlQueriesMedic.position;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscMedic.Views
{
    public partial class employeePositionMedic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CarregarCargos();
        }

        private void CarregarCargos()
        {
            EmpPositionMedic position = new EmpPositionMedic();
            DataTable dt = position.GetPositions();

            gvPositions.DataSource = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                gvPositions.DataSource = dt;
                gvPositions.DataBind();
            }
            else
            {
                gvPositions.DataSource = null;
                gvPositions.DataBind();
            }
        }
    }
}