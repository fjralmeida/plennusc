using Plennusc.Core.Service.ServiceGestao.PlatformSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class listPlatform : System.Web.UI.Page
    {
        private listPlatformService _service;

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new listPlatformService();

            if (!IsPostBack)
            {
                CarregarTiposEstrutura();
            }
        }

        private void CarregarTiposEstrutura()
        {
            try
            {
                var listaTiposEstrutura = _service.GetSistema();
                gvSistemas.DataSource = listaTiposEstrutura;
                gvSistemas.DataBind();

            
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar sistemas: {ex.Message}", "error");
            }
        }

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