using appWhatsapp.Data_Bd;
using appWhatsapp.SqlQueries;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.Views
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ItensPedIntegradoUtil util = new ItensPedIntegradoUtil();
                DataTable dtPerfil = util.ConsultaInfoPerfil();

                if (dtPerfil.Rows.Count > 0)
                {
                    //lblNomeEmpresa.Text = dtPerfil.Rows[0]["Nome"]?.ToString();

                    string simbolo = dtPerfil.Rows[0]["Conf_Simbolo"]?.ToString();
                    if (!string.IsNullOrEmpty(simbolo))
                    {
                        imgLogo.ImageUrl = ResolveUrl("~/Uploads/" + simbolo);
                    }
                }
                CarregarSistemas();
            }
        }

        private void CarregarSistemas()
        {
            SistemaUtil sistemaUtil = new SistemaUtil();
            DataTable dtSistema = sistemaUtil.ConsultaSistema();

            ddlSistema.DataSource = dtSistema;
            ddlSistema.DataTextField = "NomeDisplay";
            ddlSistema.DataValueField = "CodSistema";
            ddlSistema.DataBind();

            ddlSistema.Items.Insert(0, new ListItem("-- Selecione --", ""));
        }

        protected void ButtonSignIn_Click(object sender, EventArgs e)
        {
            string login = TextBoxEmail.Text.Trim();
            string senha = TextBoxPassword.Text;
            string codSistemaSelecionado = ddlSistema.SelectedValue;

            if (string.IsNullOrEmpty(codSistemaSelecionado))
            {
                LabelErro.Text = "Selecione um sistema.";
                LabelErro.Visible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
            {
                LabelErro.Text = "Informe o login e a senha.";
                LabelErro.Visible = true;
                return;
            }

            ItensPedIntegradoUtil util = new ItensPedIntegradoUtil();
            DataTable dtUser = util.ConsultaLoginComEmpresa(login, senha, codSistemaSelecionado);

            if (dtUser.Rows.Count > 0)
            {
                var row = dtUser.Rows[0]; // ✅ já é do sistema certo por causa do filtro na SQL

                // Verifica se está liberado para uso
                bool usuarioAtivo = Convert.ToBoolean(row["UsuarioAtivo"]);
                bool empresaAtiva = Convert.ToBoolean(row["EmpresaAtiva"]);
                bool empresaLiberaAcesso = Convert.ToBoolean(row["Conf_LiberaAcesso"]);
                bool liberacaoVinculo = Convert.ToBoolean(row["LiberacaoVinculoSistemaEmpresa"]);
                bool liberacaoUsuario = Convert.ToBoolean(row["LiberacaoUsuarioSistema"]);
                bool bloqueioUsuario = Convert.ToBoolean(row["BloqueioUsuarioSistema"]);

                if (!usuarioAtivo)
                {
                    LabelErro.Text = "Usuário está desativado.";
                    LabelErro.Visible = true;
                    return;
                }

                if (!empresaAtiva || !empresaLiberaAcesso)
                {
                    LabelErro.Text = "Empresa desativada ou acesso bloqueado.";
                    LabelErro.Visible = true;
                    return;
                }

                if (!liberacaoVinculo || !liberacaoUsuario || bloqueioUsuario)
                {
                    LabelErro.Text = "Usuário sem permissão para o sistema selecionado.";
                    LabelErro.Visible = true;
                    return;
                }

                // Login validado, salvar dados na sessão
                Session["CodUsuario"] = row["CodAutenticacaoAcesso"];
                Session["NomeUsuario"] = row["NomeUsuario"];
                Session["CodEmpresa"] = row.Field<int>("CodEmpresa");
                Session["NomeEmpresa"] = row["NomeFantasia"];
                Session["CodSistema"] = row["CodSistema"];
                Session["CodPessoa"] = row["CodPessoa"];


                // Redireciona para a home correta
                switch (codSistemaSelecionado)
                {
                    case "1": // Gestão
                        Response.Redirect("~/PlennuscGestao/Views/homeManagement.aspx");
                        break;

                    case "2": // Finance
                        Response.Redirect("~/PlennuscFinance/Views/homeFinance.aspx");
                        break;

                    case "3": // Medic
                        Response.Redirect("~/PlennuscMedic/Views/homeDoctor.aspx");
                        break;

                    default:
                        LabelErro.Text = "Sistema não identificado.";
                        LabelErro.Visible = true;
                        break;
                }
            }
            else
            {
                LabelErro.Text = "Usuário ou senha inválidos.";
                LabelErro.Visible = true;
            }
        }
    }
}