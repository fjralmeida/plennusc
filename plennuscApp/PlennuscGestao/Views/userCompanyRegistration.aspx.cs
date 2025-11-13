using Plennusc.Core.Service.ServiceGestao.company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Routing;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class userCompanyRegistration : System.Web.UI.Page
    {
        private UserCompanyService _userCompanyService;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userCompanyService = new UserCompanyService();

            if (!IsPostBack)
            {
                if (Session["CodUsuario"] == null)
                {
                    Response.Redirect("~/SignIn.aspx");
                    return;
                }

                CarregarDadosUsuario();
                CarregarEmpresas();
            }
        }

        private void CarregarDadosUsuario()
        {

            if (Session["Vinculacao_CodPessoa"] == null || Session["Vinculacao_CodAutenticacao"] == null)
            {
                Response.Redirect("~/parametrizacao/usuarios/empresas");
                return;
            }

            string codPessoaStr = Session["Vinculacao_CodPessoa"].ToString();
            string codAutenticacaoStr = Session["Vinculacao_CodAutenticacao"].ToString();

            if (!int.TryParse(codPessoaStr, out int codPessoa) ||
                !int.TryParse(codAutenticacaoStr, out int codAutenticacao))
            {
                Response.Redirect("~/parametrizacao/usuarios/empresas");
                return;
            }

            hfCodPessoa.Value = codPessoa.ToString();
            hfCodAutenticacao.Value = codAutenticacao.ToString();

            var usuarios = _userCompanyService.ListarUsuariosComAcesso();
            var usuario = usuarios.FirstOrDefault(u => u.CodPessoa == codPessoa);

            if (usuario != null)
            {
                litUsuario.Text = $"{usuario.Nome} {usuario.Sobrenome} - {usuario.Login}";
            }
        }

        private void CarregarEmpresas()
        {
            if (!int.TryParse(hfCodPessoa.Value, out int codPessoa))
                return;

            var empresas = _userCompanyService.ListarEmpresasParaSelecao(codPessoa);

            cblEmpresas.DataSource = empresas;
            cblEmpresas.DataTextField = "NomeFantasia";
            cblEmpresas.DataValueField = "CodEmpresa";
            cblEmpresas.DataBind();

            // Marca as checkboxes das empresas já vinculadas
            foreach (ListItem item in cblEmpresas.Items)
            {
                int codEmpresa = int.Parse(item.Value);
                var empresa = empresas.FirstOrDefault(e => e.CodEmpresa == codEmpresa);
                if (empresa != null && empresa.JaVinculada)
                {
                    item.Selected = true;
                }
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(hfCodPessoa.Value, out int codPessoa) ||
                    !int.TryParse(hfCodAutenticacao.Value, out int codAutenticacao))
                {
                    MostrarMensagemErro("Dados do usuário inválidos.");
                    return;
                }

                // Busca empresas atuais
                var empresasAtuais = _userCompanyService.ListarEmpresasParaSelecao(codPessoa);
                var empresasSelecionadas = cblEmpresas.Items.Cast<ListItem>()
                    .Where(item => item.Selected)
                    .Select(item => int.Parse(item.Value))
                    .ToList();

                // Processa vinculações e desvinculações
                foreach (var empresa in empresasAtuais)
                {
                    bool deveEstarVinculada = empresasSelecionadas.Contains(empresa.CodEmpresa);

                    if (deveEstarVinculada && !empresa.JaVinculada)
                    {
                        // Vincular
                        _userCompanyService.VincularUsuarioEmpresa(codPessoa, codAutenticacao, empresa.CodEmpresa);
                    }
                    else if (!deveEstarVinculada && empresa.JaVinculada)
                    {
                        // Desvincular
                        _userCompanyService.DesvincularUsuarioEmpresa(codPessoa, empresa.CodEmpresa);
                    }
                }

                MostrarMensagemSucesso("Vinculações atualizadas com sucesso!");
                CarregarEmpresas(); // Atualiza o estado das checkboxes
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao salvar vinculações: {ex.Message}");
            }
        }

        private void MostrarMensagemSucesso(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    icon: 'success',
                    title: 'Sucesso!',
                    text: '{mensagem}',
                    confirmButtonText: 'OK',
                    customClass: {{ confirmButton: 'btn btn-success' }}
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Sucesso", script, true);
        }

        private void MostrarMensagemErro(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    icon: 'error',
                    title: 'Erro!',
                    text: '{mensagem}',
                    confirmButtonText: 'Fechar'
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Erro", script, true);
        }
    }
}