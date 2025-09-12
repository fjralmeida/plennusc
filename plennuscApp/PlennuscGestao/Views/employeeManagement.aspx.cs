using Microsoft.Ajax.Utilities;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class employeeManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var acao = (Request.QueryString["acao"] ?? "").ToLowerInvariant();
                switch (acao)
                {
                    case "incluir":
                        AbrirInclusaoUsuario();
                        break;
                    case "consultar":
                        AbrirConsultaUsuario();
                        break;
                }

                CarregarPerfilSessao();
                CarregarPerfilPessoa();
                CarregarCargo();
                CarregarDepartamento();
            }
        }

        private void CarregarPerfilSessao()
        {
            var codPessoa = Session["CodPessoa"];
            var codCargo = Session["CodCargo"];
            var nomeCargo = Session["NomeCargo"];
            var isGestor = Session["IsGestor"];
        }

        private static int ToInt(object v)
        {
            if (v == null) return 0;
            if (v is int i) return i;
            int.TryParse(v.ToString(), out var n);
            return n;
        }

        private static bool ToBool(object v)
        {
            if (v == null) return false;
            if (v is bool b) return b;
            var s = v.ToString().Trim();
            if (int.TryParse(s, out var n)) return n != 0;
            return s.Equals("true", StringComparison.OrdinalIgnoreCase)
               || s.Equals("sim", StringComparison.OrdinalIgnoreCase)
               || s.Equals("yes", StringComparison.OrdinalIgnoreCase)
               || s.Equals("y", StringComparison.OrdinalIgnoreCase);
        }

        private bool UsuarioAtualEhAdmin()
        {
            // considere qualquer uma dessas chaves de sessão (use as que existir no seu login)
            var codPerfil = ToInt(Session["CodPerfilUsuario"]);           // 1 = Administrador
            var confMaster = ToBool(Session["Conf_Master"]);              // master tem poderes de admin
            var nomeCargo = (Session["NomeCargo"] ?? "").ToString();      // "Administrador"
            var perfilNome = (Session["PerfilUsuarioNome"] ?? "").ToString();

            return codPerfil == 1
                || confMaster
                || nomeCargo.Equals("Administrador", StringComparison.OrdinalIgnoreCase)
                || perfilNome.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ParseBool(object v)
        {
            if (v == null) return false;
            if (v is bool b) return b;
            if (v is byte by) return by != 0;
            if (v is short sh) return sh != 0;
            if (v is int i) return i != 0;

            var s = v.ToString().Trim();
            if (int.TryParse(s, out var n)) return n != 0;
            return s.Equals("true", StringComparison.OrdinalIgnoreCase)
                || s.Equals("sim", StringComparison.OrdinalIgnoreCase)
                || s.Equals("yes", StringComparison.OrdinalIgnoreCase)
                || s.Equals("y", StringComparison.OrdinalIgnoreCase);
        }
        protected void gvUsuarios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var phEditar = e.Row.FindControl("phEditar") as PlaceHolder;
            var phInativar = e.Row.FindControl("phInativar") as PlaceHolder;

            // --- admin ---
            int codPerfilUsuario = 0; int.TryParse(Convert.ToString(Session["CodPerfilUsuario"]), out codPerfilUsuario);
            bool confMaster = ParseBool(Session["Conf_Master"]);
            bool flagIsAdmin = ParseBool(Session["IsAdmin"]);
            string perfilNome = Convert.ToString(Session["PerfilUsuarioNome"] ?? "");
            string nomeCargo = Convert.ToString(Session["NomeCargo"] ?? "");
            bool isAdmin = (codPerfilUsuario == 1) || confMaster || flagIsAdmin
                        || (!string.IsNullOrWhiteSpace(perfilNome) && perfilNome.IndexOf("admin", StringComparison.OrdinalIgnoreCase) >= 0)
                        || (!string.IsNullOrWhiteSpace(nomeCargo) && nomeCargo.IndexOf("admin", StringComparison.OrdinalIgnoreCase) >= 0);

            // --- gestor também pode ---
            bool isGestor = ParseBool(Session["IsGestor"]);
            bool canManage = isAdmin || isGestor;

            int codPessoaLogado = 0; int.TryParse(Convert.ToString(Session["CodPessoa"]), out codPessoaLogado);
            int codPessoaLinha = 0; int.TryParse(Convert.ToString(DataBinder.Eval(e.Row.DataItem, "CodPessoa")), out codPessoaLinha);
            bool ativoLinha = ParseBool(DataBinder.Eval(e.Row.DataItem, "Conf_Ativo"));
            bool ehProprioReg = (codPessoaLinha == codPessoaLogado);

            if (phEditar != null) phEditar.Visible = canManage;
            if (phInativar != null) phInativar.Visible = canManage && !ehProprioReg && ativoLinha;
        }
        private void CarregarPerfilPessoa()
        {
            PessoaDAO daoPessoa = new PessoaDAO();
            DataTable dt = daoPessoa.TipoEstrutura();

            ddlPerfilPessoa.DataSource = dt;
            ddlPerfilPessoa.DataTextField = "DescEstrutura";
            ddlPerfilPessoa.DataValueField = "CodEstrutura";
            ddlPerfilPessoa.DataBind();
            ddlPerfilPessoa.Items.Insert(0, new ListItem("Selecione", ""));
        }

        private void CarregarCargo()
        {
            PessoaDAO daoCargo = new PessoaDAO();
            DataTable dt = daoCargo.TipoCargo();

            ddlCargo.DataSource = dt;
            ddlCargo.DataTextField = "Nome";
            ddlCargo.DataValueField = "CodCargo";
            ddlCargo.DataBind();
            ddlCargo.Items.Insert(0, new ListItem("Selecione", ""));
        }

        private void CarregarDepartamento()
        {
            PessoaDAO daoDepartamento = new PessoaDAO();
            DataTable dt = daoDepartamento.TipoDepartamento();
            ddlDepartamento.DataSource = dt;
            ddlDepartamento.DataTextField = "Nome";
            ddlDepartamento.DataValueField = "CodDepartamento";
            ddlDepartamento.DataBind();
            ddlDepartamento.Items.Insert(0, new ListItem("Selecione", ""));
        }

        private void AbrirInclusaoUsuario()
        {
            // mostra painel de cadastro
            PanelCadastro.Visible = true;
            // esconde os outros
            PanelConsulta.Visible = false;

        }

        private void AbrirConsultaUsuario()
        {
            // mostra painel de consulta
            PanelConsulta.Visible = true;
            // esconde os outros
            PanelCadastro.Visible = false;

        }

       

        protected void btnSalvarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                int codSistema = Convert.ToInt32(Session["CodSistema"]);
                int codUsuario = Convert.ToInt32(Session["CodUsuario"]);
                int codEmpresa = Convert.ToInt32(Session["CodEmpresa"]);

                string nome = txtNome.Text.Trim();
                string sobrenome = txtSobrenome.Text.Trim();
                string apelido = txtApelido.Text.Trim();
                string sexo = ddlSexo.SelectedValue.Trim();
                string cpf = txtDocCPF.Text.Replace("." ," ").Replace("-", "").Trim();
                string rg = txtDocRG.Text.Trim();
                string tituloEleitor = txtTitulo.Text.Trim();
                string zona = txtZona.Text.Trim();
                string secao = txtSecao.Text.Trim();
                string ctps = txtCTPS.Text.Trim();
                string serie = txtCTPSSerie.Text.Trim();
                string uf = txtCTPSUf.Text.Trim();
                string pis = txtPis.Text.Trim();
                string matricula = txtMatricula.Text.Trim();
                string filiacao1 = txtFiliacao1.Text.Trim();
                string filiacao2 = txtFiliacao2.Text.Trim();
                string telefone1 = txtTelefone1.Text
                    .Replace(".", "")
                    .Replace("-", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(" ", "")
                    .Trim();

                string telefone2 = txtTelefone2.Text
                    .Replace(".", "")
                    .Replace("-", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(" ", "")
                    .Trim();

                string telefone3 = txtTelefone3.Text
                    .Replace(".", "")
                    .Replace("-", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace(" ", "")
                    .Trim();
                string email = txtEmail.Text.Trim();
                string emailAlt = txtEmailAlt.Text.Trim();
                string observacao = txtObservacao.Text.Trim();

                int codCargo = int.TryParse(ddlCargo.SelectedValue, out int cargoResult) ? cargoResult : 0;
                int codDepartamento = int.TryParse(ddlDepartamento.SelectedValue, out int depResult) ? depResult : 0;
                int codEstrutura = int.TryParse(ddlPerfilPessoa.SelectedValue, out int estruturaResult) ? estruturaResult : 0;

                bool criaContaAD = chkCriaContaAD.Checked;
                bool cadastraPonto = chkCadastraPonto.Checked;
                bool ativo = chkAtivo.Checked;
                bool permiteAcesso = chkPermiteAcesso.Checked;

                DateTime? dataNascDt = null;
                DateTime? dataAdmissaoDt = null;
                //DateTime? dataDemissaoDt = null;

                if (DateTime.TryParse(txtDataNasc.Text.Trim(), out DateTime parsedDataNasc))
                    dataNascDt = parsedDataNasc;

                if (DateTime.TryParse(txtDataAdmissao.Text.Trim(), out DateTime parsedAdmissao))
                    dataAdmissaoDt = parsedAdmissao;

                //if (DateTime.TryParse(txtDataDemissao.Text.Trim(), out DateTime parsedDemissao))
                //    dataDemissaoDt = parsedDemissao;

                PessoaDAO pessoa = new PessoaDAO();
                pessoa.InsertPersonSystem(
                    codEstrutura, nome, sobrenome, apelido, sexo, dataNascDt, cpf, rg,
                    tituloEleitor, zona, secao, ctps, serie, uf, pis, matricula,
                    dataAdmissaoDt, filiacao1, filiacao2,
                    telefone1, telefone2, telefone3,
                    email, emailAlt, codCargo, codDepartamento,
                    criaContaAD, cadastraPonto, ativo, permiteAcesso,
                    codSistema, codUsuario, observacao
                );

                // Mostrar toast moderno
                ScriptManager.RegisterStartupScript(this, GetType(), "CadastroOK", @"
                    Swal.fire({
                        icon: 'success',
                        title: 'Cadastro realizado!',
                        text: 'O colaborador foi salvo com sucesso.',
                        confirmButtonText: 'OK',
                        customClass: {
                            confirmButton: 'btn btn-success'
                        }
                    });", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "erroCadastro", $@"
            Swal.fire({{
                icon: 'error',
                title: 'Erro ao cadastrar!',
                html: '{ex.Message.Replace("'", "\\'")}',
                confirmButtonText: 'Fechar'
            }});", true);
            }

        }

        protected void btnBuscarPorNome_Click(object sender, EventArgs e)
        {
            PessoaDAO dao = new PessoaDAO();
            DataTable dt = dao.BuscarUsuarioPorNome(txtBuscaNome.Text.Trim());

            if (dt != null && dt.Rows.Count > 0)
            {
                gvUsuarios.DataSource = dt;
                gvUsuarios.DataBind();
                PanelResultado.Visible = true;
            }
            else
            {
                gvUsuarios.DataSource = null;
                gvUsuarios.DataBind();
                PanelResultado.Visible = false;
            }
        }
        protected void btnBuscarPorCPF_Click(object sender, EventArgs e)
        {
            PessoaDAO dao = new PessoaDAO();
            DataTable dt = dao.BuscarUsuarioPorCPF(txtBuscaCPF.Text.Trim()); 

            if (dt != null && dt.Rows.Count > 0)
            {
                gvUsuarios.DataSource = dt;
                gvUsuarios.DataBind();
                PanelResultado.Visible = true;
            }
            else
            {
                gvUsuarios.DataSource = null;
                gvUsuarios.DataBind();
                PanelResultado.Visible = false;
            }
        }

        protected void btnBuscarDepartamento_Click(object sender, EventArgs e)
        {
            PessoaDAO dao = new PessoaDAO();
            DataTable dt = dao.BuscarUsuarioPorDepartamento(TxtBuscaDepartamento.Text.Trim());

            if (dt != null && dt.Rows.Count > 0)
            {
                gvUsuarios.DataSource = dt;
                gvUsuarios.DataBind();
                PanelResultado.Visible = true;
            }
            else
            {
                gvUsuarios.DataSource = null;
                gvUsuarios.DataBind();
                PanelResultado.Visible = false;
            }
        }
        //protected void btnSaveUser_Click(object sender, EventArgs e)
        //{
        //    PessoaDAO dao = new PessoaDAO();

        //    int codPessoa;
        //    if (!int.TryParse(hfCodPessoa.Value, out codPessoa))
        //        return;

        //    string nomeCompleto = txtModalNome.Text.Trim();
        //    string cpf = txtModalCPF.Text.Trim();
        //    string rg = txtModalRG.Text.Trim();
        //    string email = txtModalEmail.Text.Trim();
        //    string telefone = txtModalTelefone.Text.Trim();
        //    string cargo = txtModalCargo.Text.Trim();

        //    try
        //    {
        //        dao.AtualizarUsuario(codPessoa, nomeCompleto, cpf, rg, email, telefone, cargo);

        //        // Verifica qual campo foi usado na busca
        //        if (!string.IsNullOrWhiteSpace(txtBuscaNome.Text))
        //        {
        //            btnBuscarPorNome_Click(null, null);
        //        }
        //        else if (!string.IsNullOrWhiteSpace(txtBuscaCPF.Text))
        //        {
        //            btnBuscarPorCPF_Click(null, null);
        //        }

        //        ScriptManager.RegisterStartupScript(this, GetType(), "Sucesso",
        //            "Swal.fire('Sucesso', 'Usuário atualizado com sucesso.', 'success');", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Logar erro, se necessário
        //        ScriptManager.RegisterStartupScript(this, GetType(), "Erro",
        //            "Swal.fire('Erro', 'Erro ao atualizar o usuário.', 'error');", true);
        //    }
        //}
        protected void btnConfirmarInativar_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfCodPessoaInativa.Value, out int codPessoaAlvo) || codPessoaAlvo <= 0) return;

            int codPessoaLogado = 0; int.TryParse(Convert.ToString(Session["CodPessoa"]), out codPessoaLogado);
            if (codPessoaAlvo == codPessoaLogado)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "BloqueioProprio",
                    "Swal.fire('Ação não permitida','Você não pode inativar o próprio usuário.','warning');", true);
                return;
            }

            // admin OU gestor podem inativar
            int codPerfilUsuario = 0; int.TryParse(Convert.ToString(Session["CodPerfilUsuario"]), out codPerfilUsuario);
            bool confMaster = ParseBool(Session["Conf_Master"]);
            bool flagIsAdmin = ParseBool(Session["IsAdmin"]);
            string perfilNome = Convert.ToString(Session["PerfilUsuarioNome"] ?? "");
            string nomeCargo = Convert.ToString(Session["NomeCargo"] ?? "");
            bool isAdmin = (codPerfilUsuario == 1) || confMaster || flagIsAdmin
                        || (!string.IsNullOrWhiteSpace(perfilNome) && perfilNome.IndexOf("admin", StringComparison.OrdinalIgnoreCase) >= 0)
                        || (!string.IsNullOrWhiteSpace(nomeCargo) && nomeCargo.IndexOf("admin", StringComparison.OrdinalIgnoreCase) >= 0);
            bool isGestor = ParseBool(Session["IsGestor"]);
            bool canManage = isAdmin || isGestor;

            if (!canManage)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SomenteAdminOuGestor",
                    "Swal.fire('Acesso negado','Apenas administradores ou gestores podem inativar usuários.','error');", true);
                return;
            }

            string motivo = txtMotivoInativacao.Text.Trim();

            try
            {
                var dao = new PessoaDAO();
                dao.InactivateUser(codPessoaAlvo, motivo);

                ScriptManager.RegisterStartupScript(this, GetType(), "fecharModalInativar",
                    "var el=document.getElementById('modalInativarUsuario');if(el){var m=bootstrap.Modal.getInstance(el)||new bootstrap.Modal(el);m.hide();}", true);

                AtualizarGridPosAcao();

                ScriptManager.RegisterStartupScript(this, GetType(), "Sucesso",
                    "Swal.fire('Inativado','Usuário inativado com sucesso.','success');", true);
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Erro",
                    "Swal.fire('Erro','Erro ao inativar o usuário.','error');", true);
            }
        }


        private void AtualizarGridPosAcao()
        {
            if (!string.IsNullOrWhiteSpace(txtBuscaNome.Text))
            {
                btnBuscarPorNome_Click(null, null);
            }
            else if (!string.IsNullOrWhiteSpace(txtBuscaCPF.Text))
            {
                btnBuscarPorCPF_Click(null, null);
            }
            else if (!string.IsNullOrWhiteSpace(TxtBuscaDepartamento.Text))
            {
                btnBuscarDepartamento_Click(null, null);
            }
            else
            {
                var dao = new PessoaDAO();
                var dt = dao.BuscarUsuarioPorNome("");
                gvUsuarios.DataSource = dt;
                gvUsuarios.DataBind();
                PanelResultado.Visible = dt != null && dt.Rows.Count > 0;
            }
            var up = this.FindControl("upResultado") as System.Web.UI.UpdatePanel;
            if (up != null) up.Update();
        }
    }
}