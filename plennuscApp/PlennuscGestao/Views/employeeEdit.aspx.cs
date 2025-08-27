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
    public partial class employeeEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CodUsuario"] == null)
                {
                    Response.Redirect("SignIn.aspx");
                    return;
                }

                CarregarPerfilPessoa();
                CarregarCargo();
                CarregarDepartamento();

                if (!int.TryParse(Request.QueryString["id"], out var codPessoa))
                {
                    Response.Redirect("employeeManagement.aspx");
                }

                hfCodPessoa.Value = codPessoa.ToString();

                CarregarDadosColaborador(codPessoa);
            }
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

        private void CarregarDadosColaborador(int codPessoa)
        {
            var dao = new PessoaDAO();
            var dt = dao.ObterPessoaCompleta(codPessoa);

            if(dt == null || dt.Rows.Count == 0)
            {
                Response.Redirect("employeeManagement.aspx");
                return;
            }

            var r = dt.Rows[0];

            // DADOS PESSOAIS
            txtNome.Text = r["Nome"]?.ToString();
            txtSobrenome.Text = r["Sobrenome"]?.ToString();
            txtApelido.Text = r["Apelido"]?.ToString();
            ddlSexo.SelectedValue = r["Sexo"]?.ToString() ?? "";

            if (r["DataNasc"] != DBNull.Value)
                txtDataNasc.Text = Convert.ToDateTime(r["DataNasc"]).ToString("yyyy-MM-dd");

            // DOCUMENTOS
            txtDocCPF.Text = r["DocCPF"]?.ToString();   // se quiser com máscara, aplique no JS
            txtDocRG.Text = r["DocRG"]?.ToString();

            // ELEITOR
            txtTitulo.Text = r["TituloEleitor"]?.ToString();
            txtZona.Text = r["ZonaEleitor"]?.ToString();
            txtSecao.Text = r["SecaoEleitor"]?.ToString();

            // TRABALHISTAS
            txtCTPS.Text = r["NumCTPS"]?.ToString();
            txtCTPSSerie.Text = r["NumCTPSSerie"]?.ToString();
            txtCTPSUf.Text = r["NumCTPSUf"]?.ToString();
            txtPis.Text = r["NumPIS"]?.ToString();
            txtMatricula.Text = r["Matricula"]?.ToString();

            if (r["DataAdmissao"] != DBNull.Value)
                txtDataAdmissao.Text = Convert.ToDateTime(r["DataAdmissao"]).ToString("yyyy-MM-dd");
            // if (r["DataDemissao"] != DBNull.Value)
            //     txtDataDemissao.Text = Convert.ToDateTime(r["DataDemissao"]).ToString("yyyy-MM-dd");

            // FILIAÇÃO
            txtFiliacao1.Text = r["NomeFiliacao1"]?.ToString();
            txtFiliacao2.Text = r["NomeFiliacao2"]?.ToString();

            // CONTATO
            txtTelefone1.Text = r["Telefone1"]?.ToString();
            txtTelefone2.Text = r["Telefone2"]?.ToString();
            txtTelefone3.Text = r["Telefone3"]?.ToString();
            txtEmail.Text = r["Email"]?.ToString();
            txtEmailAlt.Text = r["EmailAlt"]?.ToString();

            // CARGO/DEPTO (combos já carregados)
            if (r["CodCargo"] != DBNull.Value)
                ddlCargo.SelectedValue = r["CodCargo"].ToString();
            if (r["CodDepartamento"] != DBNull.Value)
                ddlDepartamento.SelectedValue = r["CodDepartamento"].ToString();

            // CONFIG
            chkCriaContaAD.Checked = ToBool(r["Conf_CriaContaAD"]);
            chkCadastraPonto.Checked = ToBool(r["Conf_CadastraPonto"]);
            chkAtivo.Checked = ToBool(r["Conf_Ativo"]);
            chkPermiteAcesso.Checked = ToBool(r["PermiteAcesso"]);

            // OBS
            txtObservacao.Text = r["Observacao"]?.ToString();
        }

        private bool ToBool(object o)
        {
            if (o == null || o == DBNull.Value) return false;
            // aceita 1/0, true/false
            if (o is bool b) return b;
            return Convert.ToInt32(o) == 1;
        }
        protected void btnSalvarUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(hfCodPessoa.Value, out int codPessoa))
                    throw new Exception("Código da pessoa inválido.");

                int codSistema = Convert.ToInt32(Session["CodSistema"]);
                int codUsuario = Convert.ToInt32(Session["CodUsuario"]);
                int codEmpresa = Convert.ToInt32(Session["CodEmpresa"]); // se quiser usar em log

                string nome = txtNome.Text.Trim();
                string sobrenome = txtSobrenome.Text.Trim();
                string apelido = txtApelido.Text.Trim();
                string sexo = ddlSexo.SelectedValue.Trim();
                string cpf = txtDocCPF.Text.Trim();
                // remove máscara do CPF
                cpf = cpf.Replace(".", "").Replace("-", "");

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
                string telefone1 = txtTelefone1.Text.Trim();
                string telefone2 = txtTelefone2.Text.Trim();
                string telefone3 = txtTelefone3.Text.Trim();
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

                if (DateTime.TryParse(txtDataNasc.Text.Trim(), out DateTime parsedDataNasc))
                    dataNascDt = parsedDataNasc;

                if (DateTime.TryParse(txtDataAdmissao.Text.Trim(), out DateTime parsedAdmissao))
                    dataAdmissaoDt = parsedAdmissao;

                var pessoa = new PessoaDAO();
                pessoa.UpdatePersonSystem(
                    codPessoa,
                    codEstrutura, nome, sobrenome, apelido, sexo, dataNascDt, cpf, rg,
                    tituloEleitor, zona, secao, ctps, serie, uf, pis, matricula,
                    dataAdmissaoDt, filiacao1, filiacao2,
                    telefone1, telefone2, telefone3,
                    email, emailAlt, codCargo, codDepartamento,
                    criaContaAD, cadastraPonto, ativo, permiteAcesso,
                    codSistema, codUsuario, observacao
                );

                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateOK", @"
                    Swal.fire({
                        icon: 'success',
                        title: 'Alterações salvas!',
                        text: 'O colaborador foi atualizado com sucesso.',
                        confirmButtonText: 'OK',
                        customClass: { confirmButton: 'btn btn-success' }
                    });", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "erroUpdate", $@"
                    Swal.fire({{
                        icon: 'error',
                        title: 'Erro ao atualizar!',
                        html: '{ex.Message.Replace("'", "\\'")}',
                        confirmButtonText: 'Fechar'
                    }});", true);
            }
        }

        protected void btnCriarLogin_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfCodPessoa.Value, out int codPessoa))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "NoId",
                     "Swal.fire('Aviso','Nenhuma pessoa selecionada.','warning');", true);
                return;
            }

            var authDao = new AutenticacaoDAO();
            var pessoaDao = new PessoaDAO();

            var existente = authDao.ObterLoginPorPessoa(codPessoa);
            if (existente != null)
            {
                string usr = existente["UsrNomeLogin"].ToString();
                ScriptManager.RegisterStartupScript(this, GetType(), "JaTem",
                    $"Swal.fire('Já possui login','Usuário já cadastrado: <b>{usr}</b>.','info');", true);
                return;
            }

            string nome = p["Nome"]?.ToString() ?? "";
        }
    }
}