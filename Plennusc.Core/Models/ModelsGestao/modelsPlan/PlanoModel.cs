using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsPlan
{// ============================================================
    //  MODEL PARA O GRID PRINCIPAL (API_VENDA_PLANO)
    // ============================================================
    public class PlanoModel
    {
        public int CodigoPlano { get; set; }
        public int CodigoProduto { get; set; }
        public string RegistroANS { get; set; }
        public string Num_CNPJ_Operadora { get; set; }
        public string TipoContratacao { get; set; }
        public string Nome { get; set; }
        public string NomePlanoComercial { get; set; }
        public string Segmentacao { get; set; }
        public string Abrangencia { get; set; }
        public string Coparticipacao { get; set; }
        public string Acomodacao { get; set; }
        public string DecSau { get; set; }
        public string Promocional { get; set; }
        public string Conf_Ativo { get; set; }
        public int CodPessoaCadastro { get; set; }
        public int CodPessoaAlteracao { get; set; }
        public DateTime? Informacoes_log_i { get; set; }
        public DateTime? Informacoes_log_a { get; set; }
        public string Informacoes_log_e { get; set; }
    }

    // ============================================================
    //  MODEL PARA PLANOS PENDENTES (PS1030)
    // ============================================================
    public class PlanoPendenteAliancaModel
    {
        public int CodigoPlano { get; set; }
        public string NomePlanoFamiliar { get; set; }
        public string NomePlanoEmpresa { get; set; }
        public string RegistroANS { get; set; }
        public int? TipoContratacao { get; set; }      // 3=PJ, 4=AD
        public int? CodigoAcomodacao { get; set; }      // 1=I, 2=C
        public int? CodigoAbrangencia { get; set; }
        public string Coparticipacao { get; set; }
        public string Segmentacao { get; set; }      // CODIGO_TIPO_COBERTURA
        public string Conf_Ativo { get; set; }      // FLAG_LIBERAVENDA

        // Descrições calculadas para exibição
        public string TipoContratacaoDescricao { get; set; }
        public string AcomodacaoDescricao { get; set; }

        // Campos editáveis no modal
        public string DecSau { get; set; } = "S";
        public string Promocional { get; set; } = "N";
        public int? CodigoGrupoContrato { get; set; }

        public string CnpjOperadora { get; set; }
        public string NomeOperadora { get; set; }
    }

    // ============================================================
    //  FILTRO PARA O GRID
    // ============================================================
    public class PlanoFiltro
    {
        public string NomePlanoComercial { get; set; }
        public string Segmentacao { get; set; }
        public string Abrangencia { get; set; }
        public string Coparticipacao { get; set; }
    }

    // ============================================================
    //  OPERADORA (dropdown)
    // ============================================================
    public class OperadoraModel
    {
        public int CodigoOperadora { get; set; }
        public string NomeOperadora { get; set; }   // alias de NomeComercial
        public string Num_CNPJ_Operadora { get; set; }   // alias de Numero_CNPJ
        public string RegistroANS { get; set; }
    }

    // ============================================================
    //  PAYLOAD PARA IMPORTAÇÃO
    // ============================================================
    public class PlanoImportacaoPayload
    {
        public int CodPessoaCadastro { get; set; }
        public List<PlanoPendenteAliancaModel> PlanosSelecionados { get; set; }
        public string CnpjOperadora { get; set; }
        public string NomeOperadora { get; set; }
    }
}
