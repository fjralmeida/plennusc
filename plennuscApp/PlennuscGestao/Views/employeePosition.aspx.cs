using Plennusc.Core.SqlQueries.SqlQueriesGestao.position;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class employeePosition : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CarregarCargos();
        }

        private void CarregarCargos()
        {
            EmpPosition position = new EmpPosition();
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