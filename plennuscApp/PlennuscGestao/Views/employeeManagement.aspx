<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeManagement.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Gestão de Funcionários</title>

    <!-- jQuery e plugins -->
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>

    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.all.min.js"></script>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />

    <link href="../../Content/Css/projects/gestao/structuresCss/employee-Management.css" rel="stylesheet" />

    <!-- ============================================
         INLINE: tudo que depende de ClientID do servidor
         Coloquei aqui as funções que precisam dos controles ASP.NET
         ============================================ -->
    <script type="text/javascript">
        (function () {
            // ---------- HELPERS que usam ClientIDs do servidor ----------
            var selectors = {
                hfCodPessoaInativa: '#<%= hfCodPessoaInativa.ClientID %>',
                lblNomeUsuarioInativa: '#lblNomeUsuarioInativa', // span estático no markup
                modalInativarId: 'modalInativarUsuario',

                txtDocCPF: '#<%= txtDocCPF.ClientID %>',
                txtBuscaCPF: '#<%= txtBuscaCPF.ClientID %>',
                txtTelefone1: '#<%= txtTelefone1.ClientID %>',
                txtTelefone2: '#<%= txtTelefone2.ClientID %>',
                txtTelefone3: '#<%= txtTelefone3.ClientID %>',

                panelCadastroId: '<%= PanelCadastro.ClientID %>'
            };

            // ---------- ABRE MODAL DE INATIVAÇÃO ----------
            window.abrirModalInativarBtn = function (btn) {
                try {
                    if (!btn) return;
                    var codPessoa = btn.getAttribute('data-codpessoa');
                    var nome = btn.getAttribute('data-nome');

                    // Preenche HiddenField (server control)
                    if ($(selectors.hfCodPessoaInativa).length) {
                        $(selectors.hfCodPessoaInativa).val(codPessoa);
                    }

                    // Preenche label do modal (span estático)
                    if ($(selectors.lblNomeUsuarioInativa).length) {
                        $(selectors.lblNomeUsuarioInativa).text(nome);
                    }

                    // Mostra modal Bootstrap
                    var modalEl = document.getElementById(selectors.modalInativarId);
                    if (modalEl) {
                        var m = new bootstrap.Modal(modalEl);
                        m.show();
                    }
                } catch (err) {
                    console.error('abrirModalInativarBtn error:', err);
                }
            };

            // ---------- APLICA MÁSCARAS (jQuery Mask) ----------
            window.aplicarMascaras = function () {
                try {
                    var $cpf = $(selectors.txtDocCPF || '');
                    var $cpfBusca = $(selectors.txtBuscaCPF || '');

                    if ($cpf.length) $cpf.unmask().mask('000.000.000-00', { reverse: true });
                    if ($cpfBusca.length) $cpfBusca.unmask().mask('000.000.000-00', { reverse: true });

                    function phoneBehavior(val) {
                        var nums = val.replace(/\D/g, '').slice(0, 11);
                        return nums.length === 11 ? '(00) 00000-0000' : '(00) 0000-00009';
                    }
                    var phoneOptions = {
                        onKeyPress: function (val, e, field, options) {
                            field.mask(phoneBehavior.apply({}, arguments), options);
                        }
                    };

                    var $tel1 = $(selectors.txtTelefone1 || '');
                    var $tel2 = $(selectors.txtTelefone2 || '');
                    var $tel3 = $(selectors.txtTelefone3 || '');

                    if ($tel1.length) $tel1.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
                    if ($tel2.length) $tel2.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
                    if ($tel3.length) $tel3.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
                } catch (err) {
                    console.error('aplicarMascaras error:', err);
                }
            };

            // ---------- TOAST DE ERRO PARA VALIDAÇÃO ----------
            window.showToastErroObrigatorio = function () {
                try {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'error',
                        title: 'Preencha todos os campos obrigatórios.',
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: true
                    });
                } catch (err) {
                    console.error('showToastErroObrigatorio error:', err);
                }
            };

            // ---------- ROLAR PARA O PRIMEIRO CAMPO INVÁLIDO DO ValidationGroup ----------
            window.rolarParaPrimeiroCampoInvalido = function (validationGroup) {
                try {
                    var validators = window.Page_Validators || [];
                    for (var i = 0; i < validators.length; i++) {
                        var v = validators[i];
                        if (v.validationGroup === validationGroup && !v.isvalid) {
                            var campo = document.getElementById(v.controltovalidate);
                            if (campo) {
                                campo.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                campo.focus();
                                break;
                            }
                        }
                    }
                } catch (err) {
                    console.error('rolarParaPrimeiroCampoInvalido error:', err);
                }
            };

            // No JavaScript inline, atualize a função aplicarMascaras:
            window.aplicarMascaras = function () {
                try {
                    var $cpf = $(selectors.txtDocCPF || '');
                    var $cpfBusca = $(selectors.txtBuscaCPF || '');

                    // Máscara para CPF (com auto-tab)
                    if ($cpf.length) {
                        $cpf.unmask().mask('000.000.000-00', {
                            reverse: true,
                            onComplete: function (val) {
                                // Chama o postback automaticamente após preencher o CPF
                                if (val.replace(/\D/g, '').length === 11) {
                                    setTimeout(function () {
                                        __doPostBack($cpf.attr('name'), '');
                                    }, 500);
                                }
                            }
                        });
                    }

                    if ($cpfBusca.length) $cpfBusca.unmask().mask('000.000.000-00', { reverse: true });

                    // ... resto do código das máscaras de telefone
                } catch (err) {
                    console.error('aplicarMascaras error:', err);
                }
            };

            // ---------- BINDs e inicializações ----------
            $(document).ready(function () {
                // aplica máscaras no load
                try { window.aplicarMascaras(); } catch (e) { console.error(e); }

                // se PanelCadastro estiver visível no carregamento, reaplica máscaras (caso o Panel seja mostrado via server)
                try {
                    var panel = document.getElementById(selectors.panelCadastroId);
                    if (panel && $(panel).is(':visible')) {
                        window.aplicarMascaras();
                    }
                } catch (e) { /*silent*/ }
            });

            // Reaplica máscaras após postback parcial (UpdatePanel) se estiver presente
            if (typeof (Sys) !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                try {
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                        try { window.aplicarMascaras(); } catch (e) { console.error(e); }
                    });
                } catch (e) { /*silent*/ }
            }

        })();
    </script>

<!-- ============================================
     JAVASCRIPT DO WIZARD DE CADASTRO
     ============================================ -->
<script type="text/javascript">
    (function() {
        'use strict';
        
        // Configurações do wizard
        var wizard = {
            currentStep: 1,
            totalSteps: 8,
            isInitialized: false,
            
            // Elementos
            elements: {
                panel: null,
                steps: null,
                indicators: null,
                prevBtn: null,
                nextBtn: null,
                saveBtn: null,
                stepCounter: null
            },
            
            // Inicializar
            init: function() {
                if (this.isInitialized) return;
                
                // Encontrar elementos
                this.elements.panel = document.getElementById('<%= PanelCadastro.ClientID %>');
                if (!this.elements.panel || this.elements.panel.offsetParent === null) return;
                
                this.elements.steps = this.elements.panel.querySelectorAll('.wizard-step');
                this.elements.indicators = this.elements.panel.querySelectorAll('.wizard-step-indicator');
                this.elements.prevBtn = this.elements.panel.querySelector('#wizardPrevBtn');
                this.elements.nextBtn = this.elements.panel.querySelector('#wizardNextBtn');
                this.elements.saveBtn = this.elements.panel.querySelector('#<%= btnSalvarUsuario.ClientID %>');
                this.elements.stepCounter = this.elements.panel.querySelector('#currentStepNumber');
                
                // Configurar eventos
                this.setupEvents();
                
                // Atualizar UI inicial
                this.updateUI();
                
                this.isInitialized = true;
            },
            
            // Configurar eventos
            setupEvents: function() {
                var self = this;
                
                // Botões de navegação
                if (this.elements.prevBtn) {
                    this.elements.prevBtn.addEventListener('click', function() {
                        self.prevStep();
                    });
                }
                
                if (this.elements.nextBtn) {
                    this.elements.nextBtn.addEventListener('click', function() {
                        self.nextStep();
                    });
                }
                
                // Clicar nos indicadores para voltar
                if (this.elements.indicators) {
                    this.elements.indicators.forEach(function(indicator) {
                        indicator.addEventListener('click', function() {
                            var step = parseInt(this.getAttribute('data-step'));
                            if (step < self.currentStep) {
                                self.goToStep(step);
                            }
                        });
                    });
                }
                
                // Tecla Enter avança (exceto em textareas)
                document.addEventListener('keydown', function(e) {
                    if (e.key === 'Enter' && e.target.tagName !== 'TEXTAREA') {
                        if (self.elements.nextBtn && self.elements.nextBtn.offsetParent !== null) {
                            e.preventDefault();
                            self.nextStep();
                        }
                    }
                });
            },
            
            // Ir para passo anterior
            prevStep: function() {
                if (this.currentStep > 1) {
                    this.goToStep(this.currentStep - 1);
                }
            },
            
            // Ir para próximo passo
            nextStep: function() {
                if (this.currentStep < this.totalSteps) {
                    this.goToStep(this.currentStep + 1);
                }
            },
            
            // Ir para um passo específico
            goToStep: function(step) {
                if (step < 1 || step > this.totalSteps) return;
                
                // Esconder passo atual
                var currentStepEl = this.elements.steps[this.currentStep - 1];
                if (currentStepEl) {
                    currentStepEl.classList.remove('active');
                }
                
                var currentIndicator = this.elements.indicators[this.currentStep - 1];
                if (currentIndicator) {
                    currentIndicator.classList.remove('active');
                }
                
                // Atualizar passos anteriores como completos
                for (var i = 0; i < step - 1; i++) {
                    if (this.elements.indicators[i]) {
                        this.elements.indicators[i].classList.add('completed');
                        this.elements.indicators[i].classList.remove('active');
                    }
                }
                
                // Atualizar passo atual
                this.currentStep = step;
                
                // Mostrar novo passo
                var newStepEl = this.elements.steps[step - 1];
                if (newStepEl) {
                    newStepEl.classList.add('active');
                }
                
                var newIndicator = this.elements.indicators[step - 1];
                if (newIndicator) {
                    newIndicator.classList.add('active');
                    newIndicator.classList.remove('completed');
                }
                
                // Limpar completos dos passos à frente
                for (var i = step; i < this.totalSteps; i++) {
                    if (this.elements.indicators[i]) {
                        this.elements.indicators[i].classList.remove('completed');
                    }
                }
                
                // Atualizar UI
                this.updateUI();
                
                // Scroll suave para o topo
                setTimeout(function() {
                    wizard.elements.panel.scrollIntoView({ behavior: 'smooth', block: 'start' });
                }, 100);
                
                // Reaplicar máscaras se necessário
                setTimeout(function() {
                    if (window.aplicarMascaras) {
                        window.aplicarMascaras();
                    }
                }, 50);
            },
            
            // Atualizar interface
           updateUI: function() {
                // Botão Anterior
                if (this.elements.prevBtn) {
                    this.elements.prevBtn.disabled = (this.currentStep === 1);
                }
    
                // Botão Próximo vs Salvar
                if (this.elements.nextBtn && this.elements.saveBtn) {
                    if (this.currentStep === this.totalSteps) {
                        // Última etapa: mostrar botão Salvar
                        this.elements.nextBtn.style.display = 'none';
                        this.elements.saveBtn.style.display = 'inline-block';
                    } else {
                        // Etapas normais: mostrar botão Próximo
                        this.elements.nextBtn.style.display = 'inline-block';
                        this.elements.saveBtn.style.display = 'none';
            
                        // Mudar estilo do botão Próximo se for penúltima etapa
                        if (this.currentStep === this.totalSteps - 1) {
                            this.elements.nextBtn.innerHTML = 'Finalizar <i class="fas fa-check ms-2"></i>';
                            this.elements.nextBtn.className = 'btn btn-finalize';
                        } else {
                            this.elements.nextBtn.innerHTML = 'Próximo <i class="fas fa-arrow-right ms-2"></i>';
                            this.elements.nextBtn.className = 'btn btn-next';
                        }
                    }
                }
    
                // Atualizar contador
                if (this.elements.stepCounter) {
                    this.elements.stepCounter.textContent = this.currentStep;
                }
            }
        };
        
        // Inicializar quando o DOM estiver pronto
        document.addEventListener('DOMContentLoaded', function() {
            // Verificar se o painel está visível
            var panel = document.getElementById('<%= PanelCadastro.ClientID %>');
            if (panel && panel.offsetParent !== null) {
                setTimeout(function() {
                    wizard.init();
                }, 100);
            }
        });
        
        // Observar postbacks do ASP.NET
        if (typeof(Sys) !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
                setTimeout(function() {
                    // Resetar wizard
                    wizard.isInitialized = false;
                    
                    // Verificar se o painel está visível
                    var panel = document.getElementById('<%= PanelCadastro.ClientID %>');
                    if (panel && panel.offsetParent !== null) {
                        wizard.init();
                    }
                }, 300);
            });
        }
        
        // Função pública para resetar o wizard (se necessário)
        window.resetCadastroWizard = function() {
            wizard.currentStep = 1;
            wizard.isInitialized = false;
            
            // Resetar visual
            var panel = document.getElementById('<%= PanelCadastro.ClientID %>');
            if (panel) {
                var steps = panel.querySelectorAll('.wizard-step');
                var indicators = panel.querySelectorAll('.wizard-step-indicator');
                
                steps.forEach(function(step) {
                    step.classList.remove('active');
                });
                
                indicators.forEach(function(indicator) {
                    indicator.classList.remove('active', 'completed');
                });
                
                // Ativar primeira etapa
                if (steps[0]) steps[0].classList.add('active');
                if (indicators[0]) indicators[0].classList.add('active');
                
                // Re-inicializar
                setTimeout(function() {
                    wizard.init();
                }, 50);
            }
        };
        
        // Função para ir para uma etapa específica (pública)
        window.goToCadastroStep = function(stepNumber) {
            if (stepNumber >= 1 && stepNumber <= wizard.totalSteps) {
                wizard.goToStep(stepNumber);
            }
        };
        
    })();
</script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">

<asp:Panel ID="PanelCadastro" runat="server" CssClass="form-panel mt-4" Visible="false">
    <h4 class="titulo-cadastro">Cadastro de Novo Colaborador</h4>
    

    <!-- Dentro do PanelCadastro, adicione estes HiddenFields -->
<asp:HiddenField ID="hdnColaboradorId" runat="server" />
<asp:HiddenField ID="hdnCPFValidado" runat="server" Value="false" />
<asp:HiddenField ID="hdnCPFExistente" runat="server" Value="" />

<!-- Botão escondido para preencher dados -->
<asp:Button ID="btnPreencherDados" runat="server" style="display:none" 
    OnClick="btnPreencherDados_Click" />

    <!-- INDICADOR DE ETAPAS -->
    <div class="wizard-steps">

<div class="wizard-step-indicator active" data-step="2">
    <div class="wizard-step-number">1</div>
    <div class="wizard-step-label">Documentos</div>
</div>

<div class="wizard-step-indicator" data-step="1">
    <div class="wizard-step-number">2</div>
    <div class="wizard-step-label">Dados Pessoais</div>
</div>


        <div class="wizard-step-indicator" data-step="3">
            <div class="wizard-step-number">3</div>
            <div class="wizard-step-label">Eleitorais</div>
        </div>
        <div class="wizard-step-indicator" data-step="4">
            <div class="wizard-step-number">4</div>
            <div class="wizard-step-label">Trabalhistas</div>
        </div>
        <div class="wizard-step-indicator" data-step="5">
            <div class="wizard-step-number">5</div>
            <div class="wizard-step-label">Filiação</div>
        </div>
        <div class="wizard-step-indicator" data-step="6">
            <div class="wizard-step-number">6</div>
            <div class="wizard-step-label">Contato</div>
        </div>
        <div class="wizard-step-indicator" data-step="7">
            <div class="wizard-step-number">7</div>
            <div class="wizard-step-label">Cargo/Depto</div>
        </div>
        <div class="wizard-step-indicator" data-step="8">
            <div class="wizard-step-number">8</div>
            <div class="wizard-step-label">Finalizar</div>
        </div>
    </div>
    
    <div class="wizard-step-counter">
        Etapa <span id="currentStepNumber">1</span> de <span>8</span>
    </div>
    
    <!-- CONTAINER DOS PASSOS -->
    <div class="wizard-container">
        
        
        <!-- PASSO 2: DOCUMENTOS -->
        <div class="wizard-step active" data-step="1">
            <div class="section-block bg-gray-section">
                <h5>Documentos</h5>
                <div class="row g-3">
                  <!-- No arquivo employeeManagement.aspx, ajuste o campo CPF: -->
                   <div class="col-md-6">
                         <label>CPF *</label>
                        <div class="input-group">
                            <span class="input-group-text">
                                <i class="fas fa-id-card"></i> <!-- Ícone opcional -->
                            </span>
                            <asp:TextBox ID="txtDocCPF" runat="server" CssClass="form-control" 
                                placeholder="000.000.000-00" MaxLength="14"
                                AutoPostBack="true"
                                OnTextChanged="txtDocCPF_TextChanged" />
                        </div>
                        <asp:RequiredFieldValidator ID="rfvCPF" runat="server" ControlToValidate="txtDocCPF" 
                            ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" 
                            ValidationGroup="Cadastro" />
                        <!-- Mensagem de validação -->
                        <asp:Panel ID="pnlCPFMessage" runat="server" CssClass="mt-2" Visible="false">
                            <div class="alert alert-warning alert-dismissible fade show" role="alert">
                                <i class="fas fa-exclamation-triangle me-2"></i>
                                <asp:Literal ID="litCPFMessage" runat="server" />
                                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                            </div>
                        </asp:Panel>
                    </div>
                    <div class="col-md-6">
                        <label>RG *</label>
                        <asp:TextBox ID="txtDocRG" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDocRG" 
                            ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" 
                            ValidationGroup="Cadastro" />
                    </div>
                </div>
            </div>
        </div>

          <!-- PASSO 1: DADOS PESSOAIS -->
      <div class="wizard-step" data-step="2">
          <div class="section-block bg-white-section">
              <h5>Dados Pessoais</h5>
              <div class="row g-3">
                  <div class="col-md-6">
                      <label>Nome *</label>
                      <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" />
                      <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome" 
                          ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" 
                          ValidationGroup="Cadastro" />
                  </div>
                  <div class="col-md-6">
                      <label>Sobrenome *</label>
                      <asp:TextBox ID="txtSobrenome" runat="server" CssClass="form-control" />
                      <asp:RequiredFieldValidator ID="rfvSobrenome" runat="server" ControlToValidate="txtSobrenome" 
                          ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" 
                          ValidationGroup="Cadastro" />
                  </div>
                  <div class="col-md-4">
                      <label>Apelido</label>
                      <asp:TextBox ID="txtApelido" runat="server" CssClass="form-control" />
                  </div>
                  <div class="col-md-4">
                      <label>Sexo *</label>
                      <asp:DropDownList ID="ddlSexo" runat="server" CssClass="form-control">
                          <asp:ListItem Value="">Selecione</asp:ListItem>
                          <asp:ListItem Value="M">Masculino</asp:ListItem>
                          <asp:ListItem Value="F">Feminino</asp:ListItem>
                      </asp:DropDownList>
                      <asp:RequiredFieldValidator ID="rfvSexo" runat="server" ControlToValidate="ddlSexo" 
                          InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" 
                          Display="Dynamic" ValidationGroup="Cadastro" />
                  </div>
                  <div class="col-md-4">
                      <label>Data de Nascimento *</label>
                      <asp:TextBox ID="txtDataNasc" runat="server" CssClass="form-control" TextMode="Date" />
                      <asp:RequiredFieldValidator ID="rfvDataNasc" runat="server" ControlToValidate="txtDataNasc" 
                          ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" 
                          ValidationGroup="Cadastro" />
                  </div>
              </div>
          </div>
      </div>
        
        <!-- PASSO 3: DADOS ELEITORAIS -->
        <div class="wizard-step" data-step="3">
            <div class="section-block bg-white-section">
                <h5>Dados Eleitorais</h5>
                <div class="row g-3">
                    <div class="col-md-4">
                        <label>Título de Eleitor</label>
                        <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label>Zona</label>
                        <asp:TextBox ID="txtZona" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label>Seção</label>
                        <asp:TextBox ID="txtSecao" runat="server" CssClass="form-control" />
                    </div>
                </div>
            </div>
        </div>
        
        <!-- PASSO 4: DADOS TRABALHISTAS -->
        <div class="wizard-step" data-step="4">
            <div class="section-block bg-gray-section">
                <h5>Dados Trabalhistas</h5>
                <div class="row g-3">
                    <div class="col-md-4">
                        <label>CTPS</label>
                        <asp:TextBox ID="txtCTPS" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label>Série</label>
                        <asp:TextBox ID="txtCTPSSerie" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label>UF</label>
                        <asp:TextBox ID="txtCTPSUf" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label>PIS</label>
                        <asp:TextBox ID="txtPis" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label>Matrícula</label>
                        <asp:TextBox ID="txtMatricula" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label>Admissão *</label>
                        <asp:TextBox ID="txtDataAdmissao" runat="server" CssClass="form-control" TextMode="Date" />
                        <asp:RequiredFieldValidator ID="rfvAdmissao" runat="server" ControlToValidate="txtDataAdmissao" 
                            ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" 
                            ValidationGroup="Cadastro" />
                    </div>
                </div>
            </div>
        </div>
        
        <!-- PASSO 5: FILIAÇÃO -->
        <div class="wizard-step" data-step="5">
            <div class="section-block bg-white-section">
                <h5>Filiação</h5>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label>Nome da Filiação 1</label>
                        <asp:TextBox ID="txtFiliacao1" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label>Nome da Filiação 2</label>
                        <asp:TextBox ID="txtFiliacao2" runat="server" CssClass="form-control" />
                    </div>
                </div>
            </div>
        </div>
        
        <!-- PASSO 6: CONTATO -->
        <div class="wizard-step" data-step="6">
            <div class="section-block bg-gray-section">
                <h5>Contato</h5>
                <div class="row g-3">
                    <div class="col-md-4">
                        <label>Telefone 1 *</label>
                        <asp:TextBox ID="txtTelefone1" runat="server" CssClass="form-control" MaxLength="15" />
                        <asp:RequiredFieldValidator ID="rfvTel1" runat="server" ControlToValidate="txtTelefone1"
                            ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" 
                            ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-4">
                        <label>Telefone 2</label>
                        <asp:TextBox ID="txtTelefone2" runat="server" CssClass="form-control" MaxLength="15" />
                    </div>
                    <div class="col-md-4">
                        <label>Telefone 3</label>
                        <asp:TextBox ID="txtTelefone3" runat="server" CssClass="form-control" MaxLength="15" />
                    </div>
                    <div class="col-md-6">
                        <label>Email *</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" 
                            ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" 
                            ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-6">
                        <label>Email Alternativo</label>
                        <asp:TextBox ID="txtEmailAlt" runat="server" CssClass="form-control" TextMode="Email" />
                    </div>
                </div>
            </div>
        </div>
        
        <!-- PASSO 7: CARGO E DEPARTAMENTO -->
        <div class="wizard-step" data-step="7">
            <div class="section-block bg-white-section">
                <h5>Cargo e Departamento</h5>
                <div class="row g-3">
                    <div class="col-md-12">
                        <label>Perfil Pessoa *</label>
                        <asp:DropDownList ID="ddlPerfilPessoa" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvPerfilPessoa" runat="server" ControlToValidate="ddlPerfilPessoa" 
                            InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" 
                            Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-6">
                        <label>Cargo *</label>
                        <asp:DropDownList ID="ddlCargo" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Selecione" Value="" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvCargo" runat="server" ControlToValidate="ddlCargo" 
                            InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" 
                            Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-6">
                        <label>Departamento *</label>
                        <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvDepartamento" runat="server" ControlToValidate="ddlDepartamento" 
                            InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" 
                            Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                </div>
            </div>
        </div>
        
        <!-- PASSO 8: CONFIGURAÇÕES E OBSERVAÇÕES -->
        <div class="wizard-step" data-step="8">
            <div class="section-block bg-gray-section">
                <h5>Configurações</h5>
                <div class="row px-2">
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkCriaContaAD" runat="server" />
                            Criar conta no AD
                        </label>
                    </div>
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkCadastraPonto" runat="server" />
                            Cadastrar no ponto
                        </label>
                    </div>
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkAtivo" runat="server" Checked="true" />
                            Ativo *
                        </label>
                    </div>
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkPermiteAcesso" runat="server" />
                            Permite Acesso
                        </label>
                    </div>
                </div>
            </div>
            
            <div class="section-block bg-white-section mt-4">
                <h5>Observações</h5>
                <asp:TextBox ID="txtObservacao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
            </div>
            
            <div class="wizard-final-alert">
                <i class="fas fa-info-circle me-2 text-primary"></i>
                <strong>Pronto para finalizar!</strong> Revise todas as informações preenchidas antes de salvar.
            </div>
        </div>
        
    </div>
    
   <!-- BOTÕES DE NAVEGAÇÃO -->
        <div class="wizard-navigation">
            <button type="button" class="btn btn-prev" id="wizardPrevBtn" disabled>
                <i class="fas fa-arrow-left me-2"></i> Anterior
            </button>
    
            <div class="nav-buttons-right">
                <button type="button" class="btn btn-next" id="wizardNextBtn">
                    Próximo <i class="fas fa-arrow-right ms-2"></i>
                </button>
        
                <asp:Button ID="btnSalvarUsuario" runat="server" Text="Salvar Usuário"
                    CssClass="btn btn-save"
                    ValidationGroup="Cadastro"
                    OnClick="btnSalvarUsuario_Click"
                    style="display: none;"
                    OnClientClick="if (!Page_ClientValidate('Cadastro')) { showToastErroObrigatorio(); rolarParaPrimeiroCampoInvalido('Cadastro'); return false; }" />
            </div>
        </div>
</asp:Panel>

        <asp:Panel ID="PanelConsulta" runat="server" CssClass="form-panel mt-4" Visible="false">
            <!-- Título principal -->
            <h4 class="titulo-cadastro">Consultar Usuário</h4>

            <div class="filters-block">
                <h5 class="filters-title">Filtros</h5>

                <div class="row g-3 align-items-end">
                    <div class="col-lg-4 col-md-6">
                        <label class="form-label">Nome</label>
                        <asp:TextBox ID="txtBuscaNome" runat="server" CssClass="form-control" placeholder="Digite o nome" />
                    </div>

                    <div class="col-lg-4 col-md-6">
                        <label class="form-label">CPF</label>
                        <asp:TextBox ID="txtBuscaCPF" runat="server" CssClass="form-control" placeholder="Somente números" MaxLength="11" />
                    </div>

                    <div class="col-lg-4">
                        <label class="form-label">Departamento</label>
                        <asp:TextBox ID="TxtBuscaDepartamento" runat="server" CssClass="form-control" placeholder="Informe o departamento" />
                    </div>

                    <!-- barra de botões dentro de coluna -->
                    <div class="col-12">
                        <div class="filters-btnbar">
                            <asp:Button ID="btnBuscarPorNome" runat="server"
                                Text="Buscar Nome" CssClass="btn btn-filter btn-filter-primary"
                                OnClick="btnBuscarPorNome_Click" />
                            <asp:Button ID="btnBuscarPorCPF" runat="server"
                                Text="Buscar CPF" CssClass="btn btn-filter"
                                OnClick="btnBuscarPorCPF_Click" />
                            <asp:Button ID="btnBuscarDepartamento" runat="server"
                                Text="Buscar Depto." CssClass="btn btn-filter"
                                OnClick="btnBuscarDepartamento_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- 📋 RESULTADOS -->
            <asp:Panel ID="PanelResultado" runat="server" CssClass="section-block" Visible="false">
                <h5>Resultados</h5>
                <div class="table-responsive">
                    <asp:GridView
                        ID="gvUsuarios"
                        runat="server"
                        CssClass="table table-modern"
                        AutoGenerateColumns="False"
                        GridLines="None"
                        ShowHeaderWhenEmpty="False"
                        EmptyDataText="Nenhum usuário encontrado."
                        DataKeyNames="CodPessoa,CodDepartamento,CodCargo"
                        OnRowDataBound="gvUsuarios_RowDataBound">

                        <HeaderStyle CssClass="table-custom-header" />
                        <Columns>
                            <asp:BoundField DataField="CodPessoa" HeaderText="CodPessoa" />
                            <asp:BoundField DataField="NomeCompleto" HeaderText="Nome">
                                <ItemStyle CssClass="col-nome-nowrap" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CPF" HeaderText="CPF" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:BoundField DataField="Telefone1" HeaderText="Telefone" />

                            <asp:BoundField DataField="NomeDepartamento" HeaderText="Departamento" />
                            <asp:BoundField DataField="NomeCargo" HeaderText="Cargo" />

                            <asp:BoundField DataField="Conf_Ativo" HeaderText="Ativo">
                                <ItemStyle Width="80px" />
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Editar">
                                <ItemStyle Width="90px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:PlaceHolder ID="phEditar" runat="server">
                                       <a class="btn-editar" href='<%# "employeeEdit?id=" + Eval("CodPessoa") %>'>
                                            <i class="fa-solid fa-pen-to-square"></i>
                                        </a>
                                    </asp:PlaceHolder>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Inativar">
                                <ItemStyle Width="110px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:PlaceHolder ID="phInativar" runat="server">
                                        <button type="button" class="btn-inativar"
                                            data-codpessoa='<%# Eval("CodPessoa") %>'
                                            data-nome='<%# System.Web.HttpUtility.HtmlEncode(Eval("NomeCompleto")) %>'
                                            onclick="abrirModalInativarBtn(this)">
                                            <i class="fa-solid fa-user-slash"></i>
                                        </button>
                                    </asp:PlaceHolder>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                </div>

                <!-- Modal de Inativação -->
                <div class="modal fade" id="modalInativarUsuario" tabindex="-1" aria-labelledby="modalInativarUsuarioLabel" aria-hidden="true">
                    <div class="modal-dialog modal-md modal-dialog-centered">
                        <div class="modal-content rounded-4 shadow">
                            <div class="modal-header bg-danger text-white">
                                <h5 class="modal-title" id="modalInativarUsuarioLabel">
                                    <i class="fa-solid fa-user-slash me-2"></i>Inativar Usuário
                                </h5>
                                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Fechar"></button>
                            </div>

                            <div class="modal-body">
                                <asp:HiddenField ID="hfCodPessoaInativa" runat="server" />
                                <p><strong>Tem certeza que deseja inativar o usuário:</strong> <span id="lblNomeUsuarioInativa" style="color: #d9534f;"></span>?</p>
                                <label class="form-label mt-3">Motivo da Inativação</label>
                                <asp:TextBox ID="txtMotivoInativacao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                            </div>

                            <div class="modal-footer bg-light rounded-bottom-4">
                                <asp:Button ID="btnConfirmarInativar" runat="server"
                                    CssClass="btn btn-danger btn-pill"
                                    Text="Confirmar Inativação"
                                    OnClick="btnConfirmarInativar_Click" />
                                <button type="button" class="btn btn-secondary btn-pill" data-bs-dismiss="modal">
                                    <i class="fa-solid fa-xmark me-1"></i>Cancelar
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
    </div>
</asp:Content>
