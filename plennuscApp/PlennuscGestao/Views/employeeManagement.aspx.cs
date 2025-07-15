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
                CarregarPerfilPessoa();
                CarregarCargo();
                CarregarDepartamento();
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

        protected void btnIncluirUsuario_Click(object sender, EventArgs e)
        {
            PanelCadastro.Visible = true;
            lblTitGestao.Visible = false;
            btnConsultarUsuario.Visible = false;
            //btnDesativarUsuario.Visible = false;
            btnIncluirUsuario.Visible = false;
        }

        protected void btnConsultarUsuario_Click(object sender, EventArgs e)
        {
            PanelConsulta.Visible = true;
            lblTitGestao.Visible = false;
            btnConsultarUsuario.Visible = false;
            //btnDesativarUsuario.Visible = false;
            btnIncluirUsuario.Visible = false;
        }

        //protected void btnDesativarUsuario_Click(object sender, EventArgs e)
        //{
        //    // Abrir tela/modal de seleção para desativação
        //}

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
                string cpf = txtDocCPF.Text.Trim();
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
        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            PessoaDAO dao = new PessoaDAO();

            int codPessoa;
            if (!int.TryParse(hfCodPessoa.Value, out codPessoa))
                return;

            string nomeCompleto = txtModalNome.Text.Trim();
            string cpf = txtModalCPF.Text.Trim();
            string rg = txtModalRG.Text.Trim();
            string email = txtModalEmail.Text.Trim();
            string telefone = txtModalTelefone.Text.Trim();
            string cargo = txtModalCargo.Text.Trim();

            try
            {
                dao.AtualizarUsuario(codPessoa, nomeCompleto, cpf, rg, email, telefone, cargo);

                // Verifica qual campo foi usado na busca
                if (!string.IsNullOrWhiteSpace(txtBuscaNome.Text))
                {
                    btnBuscarPorNome_Click(null, null);
                }
                else if (!string.IsNullOrWhiteSpace(txtBuscaCPF.Text))
                {
                    btnBuscarPorCPF_Click(null, null);
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "Sucesso",
                    "Swal.fire('Sucesso', 'Usuário atualizado com sucesso.', 'success');", true);
            }
            catch (Exception ex)
            {
                // Logar erro, se necessário
                ScriptManager.RegisterStartupScript(this, GetType(), "Erro",
                    "Swal.fire('Erro', 'Erro ao atualizar o usuário.', 'error');", true);
            }
        }
        protected void btnConfirmarInativar_Click(object sender, EventArgs e)
        {
            int codPessoa;
            if (!int.TryParse(hfCodPessoaInativa.Value, out codPessoa))
                return;

            string motivo = txtMotivoInativacao.Text.Trim();

            PessoaDAO dao = new PessoaDAO();

            try
            {
                dao.InactivateUser(codPessoa, motivo);

                // Verifica qual campo foi usado na busca
                if (!string.IsNullOrWhiteSpace(txtBuscaNome.Text))
                {
                    btnBuscarPorNome_Click(null, null);
                }
                else if (!string.IsNullOrWhiteSpace(txtBuscaCPF.Text))
                {
                    btnBuscarPorCPF_Click(null, null);
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "Sucesso",
                    "Swal.fire('Inativado', 'Usuário inativado com sucesso.', 'success');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Erro",
                    "Swal.fire('Erro', 'Erro ao inativar o usuário.', 'error');", true);
            }
        }
    }
}