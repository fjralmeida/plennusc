using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarDadosUsuarios();
            }
        }
        private void CarregarDadosUsuarios()
        {
            int codUsuario = Convert.ToInt32(Session["codUsuario"]);

            PessoaDAO dao = new PessoaDAO();
            DataRow pessoa = dao.ObterPessoaPorUsuario(codUsuario);
            if(pessoa != null)
            {
                txtNome.Text = pessoa["NOme"].ToString();
                txtNome.Text = pessoa["Nome"].ToString();
                txtSobrenome.Text = pessoa["Sobrenome"].ToString();
                txtApelido.Text = pessoa["Apelido"].ToString();
                txtSexo.Text = pessoa["Sexo"].ToString();
                txtDataNasc.Text = Convert.ToDateTime(pessoa["DataNasc"]).ToString("dd/MM/yyyy");
                txtCpf.Text = pessoa["DocCPF"].ToString();
                txtEmail.Text = pessoa["Email"].ToString();
                txtTelefone1.Text = pessoa["Telefone1"].ToString();
                txtCargo.Text = pessoa["CodCargo"].ToString(); // Você pode transformar em nome via join

                string foto = pessoa["ImagemFoto"].ToString();
                imgFotoPerfil.ImageUrl = string.IsNullOrEmpty(foto) ? "~/Content/Img/default-user.png" : ResolveUrl("~/Uploads/" + foto);
            }
        }
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            // Atualizar os dados do perfil (implementação futura)
        }

        protected void btnAlterarFoto_Click(object sender, EventArgs e)
        {
           
        }

        protected void btnAlterarFoto_Click1(object sender, EventArgs e)
        {
            if (fuFoto.HasFile)
            {
                string fileName = System.IO.Path.GetFileName(fuFoto.FileName);
                string path = Server.MapPath("~/Uploads/") + fileName;
                fuFoto.SaveAs(path);

                imgFotoPerfil.ImageUrl = "~/Uploads/" + fileName;

                // Aqui você atualiza no banco também
            }
        }
    }
}