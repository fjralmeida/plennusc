using Microsoft.Ajax.Utilities;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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
                //CarregarEmpresa();
            }
        }

        //private void CarregarEmpresa()
        //{
        //    PessoaDAO daoEmpresa = new PessoaDAO();
        //    DataTable dt = daoEmpresa.TipoEmpresa();

        //    ddlEmpresa.DataSource = dt;
        //    ddlEmpresa.DataTextField = "NomeFantasia"; // ou "RazaoSocial"
        //    ddlEmpresa.DataValueField = "CodEmpresa";
        //    ddlEmpresa.DataBind();
        //}

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
                //int codEmpresa = Convert.ToInt32(Session["CodEmpresa"]);

                string nome = txtNome.Text.Trim();
                string sobrenome = txtSobrenome.Text.Trim();
                string apelido = txtApelido.Text.Trim();
                string sexo = ddlSexo.SelectedValue.Trim();
                string cpf = txtDocCPF.Text.Replace("." ," ").Replace("-", "").Replace(" ", "").Trim();
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

                //int codEmpresa = int.TryParse(ddlEmpresa.SelectedValue, out int empResult) ? empResult : 0;

              //  if(codEmpresa == 0)
              //  {
              //      ScriptManager.RegisterStartupScript(this, GetType(), "EmpresaObrigatoria",
              //"Swal.fire('Atenção','Selecione uma empresa.','warning');", true);
              //      return;
              //  }

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
                int novoCodPessoa = pessoa.InsertPersonSystem(
                    codEstrutura, nome, sobrenome, apelido, sexo, dataNascDt, cpf, rg,
                    tituloEleitor, zona, secao, ctps, serie, uf, pis, matricula,
                    dataAdmissaoDt, filiacao1, filiacao2,
                    telefone1, telefone2, telefone3,
                    email, emailAlt, codCargo, codDepartamento,
                    criaContaAD, cadastraPonto, ativo, permiteAcesso,
                    codSistema, codUsuario, observacao
                );


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

                LimparFormulario();


                //if (novoCodPessoa > 0)
                //{
                //    // AGORA VAI FUNCIONAR - método público no PessoaDAO
                //    pessoa.VincularUsuarioEmpresa(novoCodPessoa, codEmpresa);

                //    LimparFormulario();
                //    ScriptManager.RegisterStartupScript(this, GetType(), "CadastroOK", @"
                //    Swal.fire({
                //        icon: 'success',
                //        title: 'Cadastro realizado!',
                //        text: 'O colaborador foi salvo e vinculado à empresa com sucesso.',
                //        confirmButtonText: 'OK',
                //        customClass: {
                //            confirmButton: 'btn btn-success'
                //        }
                //    });", true);
                //}
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
        private void LimparFormulario()
        {
            txtNome.Text = "";
            txtSobrenome.Text = "";
            txtApelido.Text = "";
            ddlSexo.SelectedIndex = 0;
            txtDataNasc.Text = "";
            txtDocCPF.Text = "";
            txtDocRG.Text = "";
            txtTitulo.Text = "";
            txtZona.Text = "";
            txtSecao.Text = "";
            txtCTPS.Text = "";
            txtCTPSSerie.Text = "";
            txtCTPSUf.Text = "";
            txtPis.Text = "";
            txtMatricula.Text = "";
            txtDataAdmissao.Text = "";
            txtFiliacao1.Text = "";
            txtFiliacao2.Text = "";
            txtTelefone1.Text = "";
            txtTelefone2.Text = "";
            txtTelefone3.Text = "";
            txtEmail.Text = "";
            txtEmailAlt.Text = "";
            txtObservacao.Text = "";

            ddlCargo.SelectedIndex = 0;
            ddlDepartamento.SelectedIndex = 0;
            ddlPerfilPessoa.SelectedIndex = 0;

            chkCriaContaAD.Checked = false;
            chkCadastraPonto.Checked = false;
            chkAtivo.Checked = true;
            chkPermiteAcesso.Checked = true;
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


        protected void txtDocCPF_TextChanged(object sender, EventArgs e)
        {
            string cpf = txtDocCPF.Text.Trim();

            if (string.IsNullOrWhiteSpace(cpf))
            {
                pnlCPFMessage.Visible = false;
                return;
            }

            // Remove pontos e traços para validação
            string cpfLimpo = cpf.Replace(".", "").Replace("-", "").Trim();

            // Validação básica do CPF
            if (cpfLimpo.Length != 11)
            {
                MostrarMensagemCPF("CPF inválido. Deve conter 11 dígitos.", false);
                return;
            }

            // Verifica se é um CPF válido (opcional, mas recomendado)
            if (!ValidarCPF(cpfLimpo))
            {
                MostrarMensagemCPF("CPF inválido.", false);
                return;
            }

            // Verifica se já existe na base
            PessoaDAO dao = new PessoaDAO();
            bool cpfExistente = dao.VerificarCPFExistente(cpfLimpo);

            if (cpfExistente)
            {
                // Busca os dados do usuário existente
                DataTable dtUsuario = dao.BuscarUsuarioPorCPF(cpfLimpo);

                if (dtUsuario != null && dtUsuario.Rows.Count > 0)
                {
                    string nomeCompleto = dtUsuario.Rows[0]["NomeCompleto"].ToString();
                    string status = dtUsuario.Rows[0]["Conf_Ativo"].ToString();
                    string departamento = dtUsuario.Rows[0]["NomeDepartamento"].ToString();
                    string cargo = dtUsuario.Rows[0]["NomeCargo"].ToString();

                    string mensagem = $"CPF já cadastrado!<br/><br/>" +
                                     $"<strong>Colaborador:</strong> {nomeCompleto}<br/>" +
                                     $"<strong>Status:</strong> {status}<br/>" +
                                     $"<strong>Departamento:</strong> {departamento}<br/>" +
                                     $"<strong>Cargo:</strong> {cargo}<br/><br/>" +
                                     $"Deseja continuar com o cadastro?";

                    // Mostra confirmação
                    ScriptManager.RegisterStartupScript(this, GetType(), "ConfirmarCPFExistente",
                        $@"Swal.fire({{
                    title: 'CPF Já Cadastrado',
                    html: '{mensagem.Replace("'", "\\'")}',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Sim, continuar',
                    cancelButtonText: 'Não, cancelar',
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#3085d6'
                }}).then((result) => {{
                    if (!result.isConfirmed) {{
                        // Limpa o campo CPF se o usuário cancelar
                        document.getElementById('{txtDocCPF.ClientID}').value = '';
                        document.getElementById('{pnlCPFMessage.ClientID}').style.display = 'none';
                    }}
                }});", true);
                }
                else
                {
                    MostrarMensagemCPF("CPF já cadastrado no sistema.", false);
                }
            }
            else
            {
                MostrarMensagemCPF("CPF disponível para cadastro.", true);
            }
        }

        private void MostrarMensagemCPF(string mensagem, bool sucesso)
        {
            pnlCPFMessage.Visible = true;
            litCPFMessage.Text = mensagem;

            // Atualiza a classe CSS baseada no sucesso
            if (pnlCPFMessage.FindControl("alert") != null)
            {
                var alertDiv = (HtmlGenericControl)pnlCPFMessage.FindControl("alert");
                alertDiv.Attributes["class"] = sucesso ?
                    "alert alert-success alert-dismissible fade show" :
                    "alert alert-warning alert-dismissible fade show";
            }
        }

        // Método auxiliar para validar CPF (opcional, mas recomendado)
        private bool ValidarCPF(string cpf)
        {
            // Remove caracteres não numéricos
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            // CPFs conhecidos como inválidos
            string[] cpfsInvalidos =
            {
                "00000000000", "11111111111", "22222222222", "33333333333",
                "44444444444", "55555555555", "66666666666", "77777777777",
                "88888888888", "99999999999"
            };

            if (cpfsInvalidos.Contains(cpf))
                return false;

            // Validação do primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * (10 - i);

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cpf[9].ToString()) != digito1)
                return false;

            // Validação do segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * (11 - i);

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return int.Parse(cpf[10].ToString()) == digito2;
        }
    }
}