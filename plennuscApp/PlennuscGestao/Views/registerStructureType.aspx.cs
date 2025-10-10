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
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDescricao.Text.Trim()))
                {
                    MostrarMensagem("Informe a descrição", "error");
                    return;
                }

                var nomeView = "VW_" + txtDescricao.Text.Trim().ToUpper().Replace(" ", "_");
                var descricao = txtDescricao.Text.Trim();

                // Valida se view já existe
                if (_service.ViewExiste(nomeView))
                {
                    MostrarMensagem($"Erro: A view '{nomeView}' já existe no banco!", "error");
                    return;
                }

                // Cria o model
                var model = new structureTypeModel
                {
                    DescTipoEstrutura = descricao,
                    Editavel = chkEditavel.Checked,
                    NomeView = nomeView
                };

                // Salva o TipoEstrutura
                int codigo = _service.SalvarTipoEstrutura(model);

                if (codigo > 0)
                {
                    // Cria a View
                    bool viewCriada = _service.CriarView(nomeView, descricao);

                    if (viewCriada)
                    {
                        MostrarMensagemSucesso($"Sucesso! Tipo Estrutura salvo (Código: {codigo}) e View '{nomeView}' criada.");

                        // Limpa o formulário
                        txtDescricao.Text = "";
                        chkEditavel.Checked = false;
                    }
                    else
                    {
                        MostrarMensagem($"Tipo Estrutura salvo (Código: {codigo}), mas houve erro ao criar a view.", "warning");
                    }
                }
                else
                {
                    MostrarMensagem("Erro ao salvar Tipo Estrutura.", "error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro: {ex.Message}", "error");
            }
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