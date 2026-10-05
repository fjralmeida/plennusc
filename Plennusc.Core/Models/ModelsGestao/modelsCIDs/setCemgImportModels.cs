using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Models.ModelsGestao.modelsCIDs
{

    /// <summary>
    /// Uma linha da aba "Planilha Padrao" (colunas A..AX), na mesma ordem da planilha.
    /// Todos os campos ficam como texto para preservar zeros à esquerda (datas, CPF, CEP...).
    /// </summary>
    public class setCemgImportModels
    {
        public int LinhaExcel { get; set; }

        public string Seq { get; set; }                     // A
        public string NumeroAuxiliar { get; set; }          // B
        public string CodigoCliente { get; set; }           // C
        public string CodigoTitular { get; set; }           // D
        public string Divisao { get; set; }                 // E
        public string TipoMovimentacao { get; set; }        // F
        public string DataVigencia { get; set; }            // G  DDMMAAAA
        public string NomeCliente { get; set; }             // H
        public string Sexo { get; set; }                    // I
        public string Parentesco { get; set; }              // J
        public string EstadoCivil { get; set; }             // K
        public string DataNascimento { get; set; }          // L  DDMMAAAA
        public string Cpf { get; set; }                     // M
        public string Nacionalidade { get; set; }           // N
        public string NomeMae { get; set; }                 // O
        public string Matricula { get; set; }               // P
        public string Cep { get; set; }                     // Q
        public string Logradouro { get; set; }              // R
        public string Numero { get; set; }                  // S
        public string Complemento { get; set; }             // T
        public string Bairro { get; set; }                  // U
        public string Cidade { get; set; }                  // V
        public string Uf { get; set; }                      // W
        public string Telefone { get; set; }                // X
        public string Celular { get; set; }                 // Y
        public string Email { get; set; }                   // Z
        public string Produto { get; set; }                 // AA
        public string Aeromedico { get; set; }              // AB
        public string RedeDental { get; set; }              // AC
        public string Odontoprev { get; set; }              // AD
        public string UnimedOdonto { get; set; }            // AE
        public string ProteseDentaria { get; set; }         // AF
        public string Modulo5 { get; set; }                 // AG
        public string MotivoExclusao { get; set; }          // AH
        public string Motivo2ViaCartao { get; set; }        // AI
        public string CodigoUnimedRepasse { get; set; }     // AJ
        public string CodigoOrigem { get; set; }            // AK
        public string DataAdmissaoEvento { get; set; }      // AL DDMMAAAA
        public string CodigoAnteriorCliente { get; set; }   // AM
        public string CodigoLocalTrabalho { get; set; }     // AN
        public string DeclaracaoNascidoVivo { get; set; }   // AO
        public string CartaoNacionalSaude { get; set; }     // AP
        public string ContribuicaoPlano { get; set; }       // AQ
        public string PeriodoContribuicao { get; set; }     // AR
        public string ValorContribuicao { get; set; }       // AS
        public string AdesaoExFuncionario { get; set; }     // AT
        public string DataObito { get; set; }               // AU DDMMAAAA
        public string ND { get; set; }                      // AV
        public string CodigoCpt { get; set; }               // AW
        public string TipoCliente { get; set; }             // AX (não vai para o TXT original)

        /// <summary>Monta a linha a partir de um array de 50 posições (índice 0 = coluna A).</summary>
        public static setCemgImportModels De(int linhaExcel, string[] v)
        {
            return new setCemgImportModels
            {
                LinhaExcel = linhaExcel,
                Seq = v[0],
                NumeroAuxiliar = v[1],
                CodigoCliente = v[2],
                CodigoTitular = v[3],
                Divisao = v[4],
                TipoMovimentacao = v[5],
                DataVigencia = v[6],
                NomeCliente = v[7],
                Sexo = v[8],
                Parentesco = v[9],
                EstadoCivil = v[10],
                DataNascimento = v[11],
                Cpf = v[12],
                Nacionalidade = v[13],
                NomeMae = v[14],
                Matricula = v[15],
                Cep = v[16],
                Logradouro = v[17],
                Numero = v[18],
                Complemento = v[19],
                Bairro = v[20],
                Cidade = v[21],
                Uf = v[22],
                Telefone = v[23],
                Celular = v[24],
                Email = v[25],
                Produto = v[26],
                Aeromedico = v[27],
                RedeDental = v[28],
                Odontoprev = v[29],
                UnimedOdonto = v[30],
                ProteseDentaria = v[31],
                Modulo5 = v[32],
                MotivoExclusao = v[33],
                Motivo2ViaCartao = v[34],
                CodigoUnimedRepasse = v[35],
                CodigoOrigem = v[36],
                DataAdmissaoEvento = v[37],
                CodigoAnteriorCliente = v[38],
                CodigoLocalTrabalho = v[39],
                DeclaracaoNascidoVivo = v[40],
                CartaoNacionalSaude = v[41],
                ContribuicaoPlano = v[42],
                PeriodoContribuicao = v[43],
                ValorContribuicao = v[44],
                AdesaoExFuncionario = v[45],
                DataObito = v[46],
                ND = v[47],
                CodigoCpt = v[48],
                TipoCliente = v[49]
            };
        }
    }

    /// <summary>Erro ou aviso encontrado durante a leitura.</summary>
    public class setCemgImportErro
    {
        public int LinhaExcel { get; set; }
        public string Campo { get; set; }
        public string Mensagem { get; set; }
    }

    /// <summary>Resultado completo da leitura da planilha.</summary>
    public class setCemgImportResultado
    {
        public string CnpjEmpresa { get; set; }  
        public List<setCemgImportModels> Linhas { get; } = new List<setCemgImportModels>();
        public List<setCemgImportErro> Erros { get; } = new List<setCemgImportErro>();
    }
}
