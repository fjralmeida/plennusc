using Plennusc.Core.SqlQueries.SqlQueriesFinance.profileFinance;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscFinance.Views
{
    public partial class profileFinance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarDadosUsuarios();
                lblUsuario.Text = Session["NomeUsuario"].ToString();
            }
        }
        private void CarregarDadosUsuarios()
        {
            int codUsuario = Convert.ToInt32(Session["codUsuario"]);

            PessoaDAOFinance dao = new PessoaDAOFinance();
            DataRow pessoa = dao.ObterPessoaPorUsuario(codUsuario);
            if (pessoa != null)
            {
                txtNome.Text = pessoa["Nome"].ToString();
                txtSobrenome.Text = pessoa["Sobrenome"].ToString();
                txtApelido.Text = pessoa["Apelido"].ToString();
                txtSexo.Text = pessoa["Sexo"].ToString();
                if (pessoa["DataNasc"] != DBNull.Value)
                {
                    txtDataNasc.Text = Convert.ToDateTime(pessoa["DataNasc"]).ToString("dd/MM/yyyy");
                }
                else
                {
                    txtDataNasc.Text = string.Empty; // ou deixe como quiser
                }
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
                if (pessoa["DataAdmissao"] != DBNull.Value)
                {
                    txtDataAdmissao.Text = Convert.ToDateTime(pessoa["DataAdmissao"]).ToString("dd/MM/yyyy");
                }
                else
                {
                    txtDataAdmissao.Text = string.Empty; // ou deixe como quiser
                }
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
                txtCodPessoa.Text = pessoa["CodPessoa"].ToString();


                string foto = pessoa["ImagemFoto"].ToString().Trim();
                imgFotoPerfil.ImageUrl = string.IsNullOrWhiteSpace(foto)
                    ? "~/Content/Img/default-user.png"
                    : ResolveUrl("~/public/uploadgestao/images/" + foto);
            }
        }

        protected void btnAlterarFoto_Click1(object sender, EventArgs e)
        {
            try
            {
                int codUsuario = Convert.ToInt32(Session["codUsuario"]);
                int codPessoa = Convert.ToInt32(txtCodPessoa.Text); // Recupera o CodPessoa da tela

                if (!fuFoto.HasFile)
                    throw new Exception("Nenhuma imagem foi selecionada.");

                // Limpa e prepara o nome do arquivo
                string fileName = Path.GetFileName(fuFoto.FileName).Trim();
                string folderVirtualPath = "~/public/uploadgestao/images/";
                string folderPhysicalPath = Server.MapPath(folderVirtualPath);

                // Garante que a pasta existe
                if (!Directory.Exists(folderPhysicalPath))
                    Directory.CreateDirectory(folderPhysicalPath);

                // Caminho físico para salvar
                string fullFilePath = Path.Combine(folderPhysicalPath, fileName);

                // Salva a imagem fisicamente
                fuFoto.SaveAs(fullFilePath);

                PessoaDAOFinance dao = new PessoaDAOFinance();
                dao.UpdateImgPerfil(codPessoa, fileName.Trim());// Passa CodPessoa + nome da imagem

                // Atualiza o caminho da imagem no front
                imgFotoPerfil.ImageUrl = ResolveUrl(folderVirtualPath + fileName);

                ScriptManager.RegisterStartupScript(this, GetType(), "CadastroOK", @"
                    Swal.fire({
                        icon: 'success',
                        title: 'Imagem Salva!',
                        text: 'A imagem do colaborador foi salva com sucesso.',
                        confirmButtonText: 'OK',
                        customClass: {
                            confirmButton: 'btn btn-success'
                        }
                    });", true);
            }
            catch (Exception ex)
            {
                lblErro.Text = "Erro ao atualizar a imagem: " + ex.Message;
                lblErro.Visible = true;
            }
        }
    }
}