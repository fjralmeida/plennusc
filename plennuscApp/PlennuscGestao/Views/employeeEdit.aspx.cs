using appWhatsapp.Models.Utils;
using Microsoft.Ajax.Utilities;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                //CarregarEmpresas();

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

        //private void CarregarEmpresas()
        //{
        //    PessoaDAO daoEmpresa = new PessoaDAO();
        //    DataTable dt = daoEmpresa.TipoEmpresa();

        //    ddlEmpresa.DataSource = dt;
        //    ddlEmpresa.DataTextField = "NomeFantasia"; // ou "RazaoSocial"
        //    ddlEmpresa.DataValueField = "CodEmpresa";
        //    ddlEmpresa.DataBind();
        //}

        private void CarregarDadosColaborador(int codPessoa)
        {
            var dao = new PessoaDAO();
            var dt = dao.ObterPessoaCompleta(codPessoa);

            if (dt == null || dt.Rows.Count == 0)
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
            txtDocCPF.Text = r["DocCPF"]?.ToString();
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

            // FILIAÇÃO
            txtFiliacao1.Text = r["NomeFiliacao1"]?.ToString();
            txtFiliacao2.Text = r["NomeFiliacao2"]?.ToString();

            // CONTATO
            txtTelefone1.Text = r["Telefone1"]?.ToString();
            txtTelefone2.Text = r["Telefone2"]?.ToString();
            txtTelefone3.Text = r["Telefone3"]?.ToString();
            txtEmail.Text = r["Email"]?.ToString();
            txtEmailAlt.Text = r["EmailAlt"]?.ToString();

            // CARGO/DEPTO
            if (r["CodCargo"] != DBNull.Value)
                ddlCargo.SelectedValue = r["CodCargo"].ToString();
            if (r["CodDepartamento"] != DBNull.Value)
                ddlDepartamento.SelectedValue = r["CodDepartamento"].ToString();

            // 🔥 PERFIL PESSOA — SEM IF, SEM CONDIÇÃO, SÓ SETA ESSA PORRA 🔥
            ddlPerfilPessoa.SelectedValue = r["CodEstr_TipoPessoa"].ToString();

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
                //int codEmpresa = Convert.ToInt32(Session["CodEmpresa"]); // se quiser usar em log

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
                //int codEmpresa = int.TryParse(ddlEmpresa.SelectedValue, out int empResult) ? empResult : 0;

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

                //// VINCULAR USUÁRIO À EMPRESA (usando o método atualizado)
                //if (codEmpresa > 0)
                //        {
                //            bool vinculacaoSucesso = pessoa.VincularUsuarioEmpresa(codPessoa, codEmpresa);

                //            if (!vinculacaoSucesso)
                //            {
                //        ScriptManager.RegisterStartupScript(this, GetType(), "SemAutenticacao", @"
                //            Swal.fire({
                //                icon: 'success',
                //                title: 'Cadastro realizado! ✅',
                //                html: 'Colaborador salvo com sucesso!<br><br>' +
                //                      '<strong>Status:</strong> Já possui acesso ao sistema<br>' +
                //                      'O colaborador já está cadastrado e pode acessar normalmente.',
                //                confirmButtonText: 'Perfeito!',
                //                customClass: { confirmButton: 'btn btn-success' }
                //            });", true);
                //        return;
                //            }
                //        }

                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateOK", @"
                Swal.fire({
                    icon: 'success',
                    title: 'Alterações salvas!',
                    text: 'O colaborador foi atualizado e vinculado à empresa com sucesso.',
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
            // precisa estar editando alguém
            if (!int.TryParse(hfCodPessoa.Value, out int codPessoa))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "NoId",
                    "Swal.fire('Aviso','Nenhuma pessoa selecionada.','warning');", true);
                return;
            }

            var authDao = new AutenticacaoDAO();
            var pessoaDao = new PessoaDAO();

            // 1) verifica se já tem login
            var existente = authDao.ObterLoginPorPessoa(codPessoa);
            if (existente != null)
            {
                string usr = existente["UsrNomeLogin"].ToString();
                ScriptManager.RegisterStartupScript(this, GetType(), "JaTem",
                    $"Swal.fire('Já possui login','Usuário já cadastrado: <b>{usr}</b>.','info');", true);
                return;
            }

            // 2) carrega dados básicos da pessoa
            var p = pessoaDao.ObterPessoaBasico(codPessoa); // Nome, Sobrenome, Email
            if (p == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SemPessoa",
                    "Swal.fire('Erro','Não foi possível carregar os dados da pessoa.','error');", true);
                return;
            }

            string nome = p["Nome"]?.ToString() ?? "";
            string sobrenome = p["Sobrenome"]?.ToString() ?? "";
            string email = p["EmailAlt"]?.ToString() ?? "";

            // 3) sugere login (nome.sobrenome, minúsculo, sem acentos) e garante unicidade
            string baseLogin = GerarSlugLogin(nome, sobrenome, email);
            string sugestao = authDao.SugerirLoginDisponivel(baseLogin);

            // 4) preenche os campos do modal
            hfCodPessoaLogin.Value = codPessoa.ToString();
            txtLoginNome.Text = nome;
            txtLoginSobrenome.Text = sobrenome;
            txtLoginEmail.Text = email;
            txtLoginUsuario.Text = sugestao;

            //// NOVO: Preencher a empresa
            //int codEmpresa = pessoaDao.ObterEmpresaDoUsuario(codPessoa);
            //string nomeEmpresa = pessoaDao.ObterNomeEmpresa(codEmpresa);

            //txtLoginEmpresa.Text = nomeEmpresa;
            //hfCodEmpresa.Value = codEmpresa.ToString();

            ddlPerfilUsuario.SelectedValue = "";
            chkLoginAtivo.Checked = true;
            chkLoginPermiteAcesso.Checked = true;

            CarregarSistemasParaEmpresa();
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowCriarLogin",
                "new bootstrap.Modal(document.getElementById('modalCriarLogin')).show();", true);
        }
        private void CarregarSistemasParaEmpresa()
        {
            var acessoDao = new AutenticacaoDAO();
            // lista TODOS os sistemas ativos (Conf_LiberaUtilizacao = 1)
            var dt = acessoDao.ListarSistemasAtivos();

            cblSistemas.DataSource = dt;
            cblSistemas.DataTextField = "NomeDisplay";
            cblSistemas.DataValueField = "CodSistema";
            cblSistemas.DataBind();

            // (opcional) pré-selecionar o sistema atual da sessão
            if (Session["CodSistema"] != null)
            {
                var item = cblSistemas.Items.FindByValue(Session["CodSistema"].ToString());
                if (item != null) item.Selected = true;
            }
        }


        // gera base do login: nome.sobrenome (sem acento, minúsculo); fallback para parte antes do @ do e-mail
        private string GerarSlugLogin(string nome, string sobrenome, string email)
        {
            string Limpar(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return "";

                input = input.Trim().ToLowerInvariant();

                var normalized = input.Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder();

                foreach (var ch in normalized)
                {
                    var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                    if (uc != UnicodeCategory.NonSpacingMark)
                    {
                        if (char.IsLetterOrDigit(ch) || ch == '.')
                            sb.Append(ch);
                        else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_')
                            sb.Append('.');
                    }
                }

                var res = sb.ToString().Normalize(NormalizationForm.FormC);

                while (res.Contains("..")) res = res.Replace("..", ".");
                return res.Trim('.');
            }

            // primeiro nome
            var partesNome = (nome ?? "").Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string n = Limpar(partesNome.Length > 0 ? partesNome[0] : "");

            // último sobrenome (sem LINQ)
            var partesSobrenome = (sobrenome ?? "").Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string ultimoSobrenome = partesSobrenome.Length > 0
                ? partesSobrenome[partesSobrenome.Length - 1]
                : "";
            string sob = Limpar(ultimoSobrenome);

            string baseLogin;
            if (string.IsNullOrEmpty(n) && string.IsNullOrEmpty(sob))
            {
                // fallback: parte antes do @ do e-mail
                var userEmail = (email ?? "").Split('@')[0];
                baseLogin = Limpar(userEmail);
            }
            else
            {
                baseLogin = string.IsNullOrEmpty(sob) ? n : $"{n}.{sob}";
            }

            return string.IsNullOrEmpty(baseLogin) ? "usuario" : baseLogin;
        }

        protected void btnConfirmarCriarLogin_Click(object sender, EventArgs e)
        {
            // 1) Ler campos do modal
            if (!int.TryParse(hfCodPessoaLogin.Value, out int codPessoa) || codPessoa <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SemPessoa",
                    "Swal.fire('Aviso','Pessoa inválida para criar login.','warning');", true);
                return;
            }

            string nome = (txtLoginNome.Text ?? "").Trim();
            string sobrenome = (txtLoginSobrenome.Text ?? "").Trim();
            string emailAlt = (txtLoginEmail.Text ?? "").Trim();
            string usuario = (txtLoginUsuario.Text ?? "").Trim().ToLowerInvariant();
            bool ativo = chkLoginAtivo.Checked;
            bool permite = chkLoginPermiteAcesso.Checked;
            string cpf = txtDocCPF.Text.Trim();
            //int codEmpresa = int.TryParse(hfCodEmpresa.Value, out int emp) ? emp : 0;

            if (emailAlt == null || emailAlt == "")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Sem Email",
                    "Swal.fire('Aviso','Informe um email.','warning');", true);
                return;
            }

            if (!int.TryParse(ddlPerfilUsuario.SelectedValue, out int codPerfilUsuario) || codPerfilUsuario <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SemPerfil",
                    "Swal.fire('Aviso','Selecione um perfil de acesso.','warning');", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(usuario))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SemUsuario",
                    "Swal.fire('Aviso','Informe um nome de usuário (login).','warning');", true);
                return;
            }

            var sistemasSelecionados = cblSistemas.Items
              .Cast<ListItem>()
              .Where(i => i.Selected)
              .Select(i => int.Parse(i.Value))
              .ToList();
            if (sistemasSelecionados.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SemSistema",
                    "Swal.fire('Aviso','Selecione ao menos um sistema para liberar o acesso.','warning');", true);
                return;
            }

            try
            {
                var authDao = new AutenticacaoDAO();


                // 2) Revalidar: já tem login para essa pessoa?
                var existentePorPessoa = authDao.ObterLoginPorPessoa(codPessoa);
                if (existentePorPessoa != null)
                {
                    string usr = existentePorPessoa["UsrNomeLogin"].ToString();
                    ScriptManager.RegisterStartupScript(this, GetType(), "JaTem",
                        $"Swal.fire('Já possui login','Usuário já cadastrado: <b>{usr}</b>.','info');", true);
                    return;
                }

                // 3) Revalidar: esse login já existe?
                if (authDao.LoginExiste(usuario))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "LoginDuplicado",
                        "Swal.fire('Atenção','Este nome de usuário já está em uso. Escolha outro.','warning');", true);
                    return;
                }

                // 4) Gerar senha temporária + hash
                string senhaTemp = GerarSenhaComSobrenomeCpf(sobrenome, cpf);
                string senhaHash = CriptografiaUtil.CalcularHashSHA512(senhaTemp);

                 //5) Inserir
                int novoId = authDao.InserirLogin(
                    codPerfilUsuario: codPerfilUsuario,
                    nomeUsuario: nome,
                    sobrenomeUsuario: sobrenome,
                    usrNomeLogin: usuario,
                    usrPasswdHash: senhaHash,
                    confPermiteAcesso: permite ? 1 : 0,
                    confAtivo: ativo ? 1 : 0,
                    confRestrito: 0,
                    confMaster: 0,
                    codPessoa: codPessoa
                );

                if (novoId > 0)
                {
                    //// 5.1) vínculos com sistemas
                    //int codEmpresa = 0;
                    //int.TryParse(Convert.ToString(Session["CodEmpresa"]), out codEmpresa);

                    //authDao.AtualizarAutenticacaoUsuario(codPessoa, novoId);

                    authDao.ConcederAcessoSistemas(
                        codAutenticacaoAcesso: novoId,
                        codPessoa: codPessoa,
                        codSistemas: sistemasSelecionados
                    );

                    // 5.2) e-mail
                    string assunto = $"[Plennusc] Novo acesso criado para {nome} {sobrenome}";
                    string corpoHtml = $@"
                    <html>
                      <body>
                        <h2>Olá, {nome}!</h2>
                        <p>Seu acesso ao sistema Plennusc foi criado com sucesso.</p>
                        <p><b>Usuário:</b> {usuario}</p>
                        <p><b>Senha temporária:</b> {senhaTemp}</p>
                        <p>Recomendamos que altere sua senha no primeiro login.</p>
                        <br>
                        <p>Atenciosamente,<br>Equipe Vallor Benefícios</p>
                      </body>
                    </html>";

                    authDao.EnviarEmailNovoAcesso(emailAlt, assunto, corpoHtml);

                    // 5.3) feedback com a senha
                    string safeUser = usuario.Replace("'", "\\'");
                    string safePwd = senhaTemp.Replace("'", "\\'");
                    ScriptManager.RegisterStartupScript(this, GetType(), "CriarLoginOK", $@"
                        Swal.fire({{
                          icon: 'success',
                          title: 'Login criado!',
                          html: 'Usuário: <b>{safeUser}</b><br/>Senha temporária: <b>{safePwd}</b>',
                          confirmButtonText: 'Ok'
                        }});", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "FalhaInsert",
                        "Swal.fire('Erro','Não foi possível criar o login.','error');", true);
                }
            }
            catch (Exception ex)
            {
                string msg = (ex.Message ?? "").Replace("'", "\\'");
                ScriptManager.RegisterStartupScript(this, GetType(), "ErroCriarLogin",
                    $"Swal.fire('Erro','{msg}','error');", true);
            }
        }
        private string GerarSenhaComSobrenomeCpf(string sobrenome, string cpf)
        {
            if (string.IsNullOrWhiteSpace(sobrenome) || string.IsNullOrWhiteSpace(cpf))
                return "SenhaTemo123";

            string ultimoNome = sobrenome.Trim().Split(' ').Last();

            cpf = System.Text.RegularExpressions.Regex.Replace(cpf, @"\D", "");

            string ultimosDig = cpf.Length >= 4 ? cpf.Substring(cpf.Length - 4) : cpf;

           return ultimoNome + ultimosDig;

        }
    }
}