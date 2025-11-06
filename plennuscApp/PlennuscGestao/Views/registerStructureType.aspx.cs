using Plennusc.Core.Models.ModelsGestao.modelsStructure;
using Plennusc.Core.Service.ServiceGestao.TipoEstrutura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class registerStructureType : System.Web.UI.Page
    {
        private StructureTypeService _service;

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new StructureTypeService();

            if (!IsPostBack)
            {
                CarregarTiposEstruturaPai();
            }
        }

        private void CarregarTiposEstruturaPai()
        {
            try
            {
                var tiposEstrutura = _service.GetTodosTiposEstrutura();

                ddlTipoEstruturaPai.DataSource = tiposEstrutura;
                ddlTipoEstruturaPai.DataTextField = "DescTipoEstrutura";
                ddlTipoEstruturaPai.DataValueField = "CodTipoEstrutura";
                ddlTipoEstruturaPai.DataBind();

                ddlTipoEstruturaPai.Items.Insert(0, new ListItem("-- Não tem --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar tipos de estrutura: {ex.Message}", "error");
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtTipoEstrutura.Text.Trim()))
                {
                    MostrarMensagem("Informe o Tipo Estrutura", "error");
                    return;
                }
                var nomeView = "VW_" + RemoveAcentos(txtTipoEstrutura.Text.Trim().ToUpper().Replace(" ", "_"));
                var tipoEstrutura = txtTipoEstrutura.Text.Trim();
                // Valida se view já existe
                if (_service.ViewExiste(nomeView))
                {
                    MostrarMensagem($"Erro: A view '{nomeView}' já existe no banco!", "error");
                    return;
                }

                // Obtém o CodTipoEstruturaPai do dropdown
                int? codTipoEstruturaPai = null;
                if (!string.IsNullOrEmpty(ddlTipoEstruturaPai.SelectedValue))
                {
                    codTipoEstruturaPai = Convert.ToInt32(ddlTipoEstruturaPai.SelectedValue);
                }

                // Cria o model
                var model = new structureTypeModel
                {
                    DescTipoEstrutura = tipoEstrutura,
                    NomeView = nomeView,
                    CodTipoEstruturaPai = codTipoEstruturaPai,
                    Definicao = txtDescricao.Text.Trim(), // Descrição vai para Definicao
                    Utilizacao = txtUtilizacao.Text.Trim()
                };

                // 1. SALVA APENAS NO TipoEstrutura
                int codigo = _service.SalvarTipoEstrutura(model);

                if (codigo > 0)
                {
                    //// 2. CRIA O PAI NA ESTRUTURA SEPARADAMENTE
                    //bool estruturaPaiCriada = _service.CriarEstruturaPai(codigo, tipoEstrutura);

                    bool viewCriada = _service.CriarView(nomeView, codigo);

                    if (viewCriada)
                    {
                        string tipoHierarquia = codTipoEstruturaPai.HasValue ?
                            $"Sub-tipo vinculado ao código {codTipoEstruturaPai.Value}" :
                            "Tipo Estrutura Pai/Raiz";

                        MostrarMensagemSucesso($"Sucesso! Tipo Estrutura salvo (Código: {codigo}), {tipoHierarquia} e View '{nomeView}' criada.");
                        LimparCampos();
                    }
                    else
                    {
                        MostrarMensagem($"Tipo Estrutura salvo (Código: {codigo}), mas houve erro ao criar a view.", "warning");
                    }
                }
                else
                {
                    MostrarMensagem($"Tipo Estrutura salvo (Código: {codigo}), mas houve erro ao criar estrutura pai.", "warning");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro: {ex.Message}", "error");
            }
        }

        // Método para remover acentuações
        private string RemoveAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i], semAcentos[i]);
            }

            // Remove caracteres especiais e mantém apenas letras, números e underscore
            return System.Text.RegularExpressions.Regex.Replace(texto, @"[^a-zA-Z0-9_]", "");
        }
        private void LimparCampos()
        {
            txtTipoEstrutura.Text = "";
            txtUtilizacao.Text = "";
            txtDescricao.Text = "";
            ddlTipoEstruturaPai.SelectedIndex = 0;
        }

        // MÉTODOS PARA EXIBIR MENSAGENS COM SWEETALERT2
        private void MostrarMensagemSucesso(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Sucesso',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                }});
            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastSucesso", script, true);
        }

        private void MostrarMensagem(string mensagem, string tipo = "success")
        {
            string titulo;
            switch (tipo.ToLower())
            {
                case "success":
                    titulo = "Sucesso";
                    break;
                case "error":
                    titulo = "Erro";
                    break;
                case "warning":
                    titulo = "Atenção";
                    break;
                case "info":
                    titulo = "Informação";
                    break;
                default:
                    titulo = "Mensagem";
                    break;
            }

            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: '{tipo}',
                    title: '{titulo}',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 4000,
                    timerProgressBar: true
                }});
            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastMsg", script, true);
        }
    }
}