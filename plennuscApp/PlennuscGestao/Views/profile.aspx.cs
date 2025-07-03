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
                txtNome.Text = pessoa["Nome"].ToString();
                txtSobrenome.Text = pessoa["Sobrenome"].ToString();
                txtApelido.Text = pessoa["Apelido"].ToString();
                txtSexo.Text = pessoa["Sexo"].ToString();
                txtDataNasc.Text = Convert.ToDateTime(pessoa["DataNasc"]).ToString("dd/MM/yyyy");
                txtDocCPF.Text = pessoa["DocCPF"].ToString();
                txtDocRG.Text = pessoa["DocRG"].ToString();
                txtTitulo.Text = pessoa["TituloEleitor"].ToString();
                txtZona.Text = pessoa["ZonaEleitor"].ToString();
                txtSecao.Text = pessoa["SecaoEleitor"].ToString();
                txtCTPS.Text = pessoa["NumCTPS"].ToString();
                txtCTPSSerie.Text = pessoa["NumCTPSSerie"].ToString();
                txtCTPSUf.Text = pessoa["NumCTPSUf"].ToString();
                txtPis.Text = pessoa["NumPIS"].ToString();
                txtMatricula.Text = pessoa["Matricula"].ToString();
                txtDataAdmissao.Text = Convert.ToDateTime(pessoa["DataAdmissao"]).ToString("dd/MM/yyyy");
                txtDataDemissao.Text = pessoa["DataDemissao"] == DBNull.Value ? "" : Convert.ToDateTime(pessoa["DataDemissao"]).ToString("dd/MM/yyyy");
                txtFiliacao1.Text = pessoa["NomeFiliacao1"].ToString();
                txtFiliacao2.Text = pessoa["NomeFiliacao2"].ToString();
                txtTelefone1.Text = pessoa["Telefone1"].ToString();
                txtTelefone2.Text = pessoa["Telefone2"].ToString();
                txtTelefone3.Text = pessoa["Telefone3"].ToString();
                txtEmail.Text = pessoa["Email"].ToString();
                txtEmailAlt.Text = pessoa["EmailAlt"].ToString();
                txtCargo.Text = pessoa["CodCargo"].ToString();
                txtDepartamento.Text = pessoa["CodDepartamento"].ToString();


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