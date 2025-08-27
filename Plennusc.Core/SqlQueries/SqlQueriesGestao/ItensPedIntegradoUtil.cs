using appWhatsapp.Data_Bd;
using appWhatsapp.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;

namespace appWhatsapp.SqlQueries
{
    public class ItensPedIntegradoUtil
    {
        public DataTable ConsultaAssociados(DateTime? dataIni = null, DateTime? dataFim = null, int? codigoOperadora = null)
        {
            if (!dataIni.HasValue || !dataFim.HasValue)
            {
                // ATENÇÃO: você precisa garantir que esse caminho não traga tudo
                return new DataTable(); // ← não retorna nada
            }

            string sql = @"
                            SELECT 
                                A.NUMERO_REGISTRO, 
                                A.CODIGO_ASSOCIADO, 
                                B.NOME_ASSOCIADO,
                                A.DATA_VENCIMENTO, 
                                A.VALOR_FATURA,
                                C.NOME_OPERADORA,
                                D.NOME_PLANO_ABREVIADO,
                                T.NUMERO_TELEFONE
                            FROM PS1020 A
                            LEFT JOIN PS1000 B ON A.CODIGO_ASSOCIADO = B.CODIGO_ASSOCIADO
                            LEFT JOIN ESP0002 C ON B.CODIGO_GRUPO_CONTRATO = C.CODIGO_GRUPO_CONTRATO
                            LEFT JOIN PS1030 D ON D.CODIGO_PLANO = B.CODIGO_PLANO
                            OUTER APPLY (

                                SELECT TOP 1 NUMERO_TELEFONE 
                                FROM PS1006 
                                WHERE PS1006.CODIGO_ASSOCIADO = A.CODIGO_ASSOCIADO
                            ) T
                            WHERE 
                                A.DATA_VENCIMENTO BETWEEN @DataIni AND @DataFim
                                AND A.DATA_PAGAMENTO IS NULL
                                AND (VALOR_PAGO <> '0.00' OR VALOR_PAGO IS NOT NULL)
                                AND A.DATA_CANCELAMENTO IS NULL
                                AND A.CODIGO_EMPRESA = 400
                                AND B.CODIGO_MOTIVO_EXCLUSAO IS NULL
                                AND B.DATA_EXCLUSAO IS NULL
                                AND (@CodigoOperadora IS NULL OR C.CODIGO_GRUPO_CONTRATO = @CodigoOperadora)
                        ";

            var parametros = new Dictionary<string, object>
            {
                ["@DataIni"] = dataIni.Value.Date,
                ["@DataFim"] = dataFim.Value.Date,
                ["@CodigoOperadora"] = (object)codigoOperadora ?? DBNull.Value
            };

            return new Banco_Dados_SQLServer().LerAlianca(sql, parametros);
        }


        public DataTable ConsultaAssociadosPME(DateTime? dataIni = null, DateTime? dataFim = null, int? codigoOperadora = null)
        {
            if (!dataIni.HasValue || !dataFim.HasValue)
            {
                // ATENÇÃO: você precisa garantir que esse caminho não traga tudo
                return new DataTable(); // ← não retorna nada
            }
            string sql = @"
SELECT
    A.NUMERO_REGISTRO,
    A.CODIGO_EMPRESA,
    E.NOME_EMPRESA AS NOME_ASSOCIADO,            
    A.DATA_VENCIMENTO,
    A.VALOR_FATURA,
    C.NOME_OPERADORA,
    D.NOME_PLANO_EMPRESAS AS NOME_PLANO_EMPRESA,   
    T.NUMERO_TELEFONE
FROM PS1020 A
INNER JOIN PS1010 E                      
        ON E.CODIGO_EMPRESA = A.CODIGO_EMPRESA
LEFT JOIN ESP0002 C                          
        ON C.CODIGO_GRUPO_CONTRATO = E.codigo_grupo_contrato
OUTER APPLY (                                  
    SELECT TOP 1 P1.CODIGO_PLANO
    FROM PS1000 P1
    WHERE P1.CODIGO_EMPRESA = A.CODIGO_EMPRESA
      AND P1.CODIGO_MOTIVO_EXCLUSAO IS NULL
      AND P1.DATA_EXCLUSAO IS NULL
    ORDER BY 
      CASE WHEN P1.CODIGO_TITULAR IS NULL OR P1.CODIGO_TITULAR = P1.CODIGO_ASSOCIADO THEN 0 ELSE 1 END,
      P1.DATA_ULTIMA_ALTERACAO DESC
) PL
LEFT JOIN PS1030 D                            
       ON D.CODIGO_PLANO = PL.CODIGO_PLANO
OUTER APPLY (                                  
    SELECT TOP 1 P.NUMERO_TELEFONE
    FROM PS1006 P
    WHERE P.CODIGO_EMPRESA = A.CODIGO_EMPRESA
    ORDER BY P.INDICE_TELEFONE DESC, P.DATA_ALTERACAO_CADASTRAL DESC
) T
WHERE
      A.CODIGO_EMPRESA <> 400             
  AND A.DATA_VENCIMENTO BETWEEN @DataIni AND @DataFim
  AND A.DATA_CANCELAMENTO IS NULL
  AND A.DATA_PAGAMENTO IS NULL
  AND (A.VALOR_PAGO IS NULL OR A.VALOR_PAGO = 0)   
  AND E.CODIGO_MOTIVO_EXCLUSAO IS NULL       
  AND E.DATA_EXCLUSAO IS NULL
  AND (@CodigoOperadora IS NULL OR C.CODIGO_GRUPO_CONTRATO = @CodigoOperadora)

                        ";

            var parametros = new Dictionary<string, object>
            {
                ["@DataIni"] = dataIni.Value.Date,
                ["@DataFim"] = dataFim.Value.Date,
                ["@CodigoOperadora"] = (object)codigoOperadora ?? DBNull.Value
            };

            return new Banco_Dados_SQLServer().LerAlianca(sql, parametros);
        }

        public DataTable ConsultaLoginComEmpresa(string login, string senha, string codSistema)
        {
            string senhaHash = CriptografiaUtil.CalcularHashSHA512(senha);

            string sql = @"
                SELECT 
                     AA.CodAutenticacaoAcesso,
                     AA.CodPessoa,
                     AA.NomeUsuario,
                     AA.UsrNomeLogin,
                     AA.Conf_Ativo AS UsuarioAtivo,

                     SE.CodEmpresa,
                     E.RazaoSocial,
                     E.NomeFantasia,
                     E.Conf_Ativo AS EmpresaAtiva,
                     E.Conf_LiberaAcesso,
                     SE.Conf_LiberaAcesso AS LiberacaoVinculoSistemaEmpresa,
                     SEU.Conf_LiberaAcesso AS LiberacaoUsuarioSistema,
                     SEU.Conf_BloqueiaAcesso AS BloqueioUsuarioSistema, 

                     SI.CodSistema,
                     SI.Nome        AS NomeSistema,
                     SI.NomeDisplay AS NomeDisplaySistema,

                     P.CodDepartamento,
                     D.Nome AS NomeDepartamento,

                     -- [ADICIONADO] cargo e flag de gestor
                     P.CodCargo,
                     C.Nome AS NomeCargo,
                     C.Conf_TipoGestor
                 FROM AutenticacaoAcesso AA
                 INNER JOIN SistemaEmpresaUsuario SEU 
                     ON SEU.CodAutenticacaoAcesso = AA.CodAutenticacaoAcesso
                 INNER JOIN SistemaEmpresa SE 
                     ON SE.CodSistemaEmpresa = SEU.CodSistemaEmpresa
                 INNER JOIN Empresa E 
                     ON E.CodEmpresa = SE.CodEmpresa
                 INNER JOIN Sistema SI 
                     ON SI.CodSistema = SE.CodSistema

                 LEFT JOIN Pessoa P
                     ON P.CodPessoa = AA.CodPessoa
                 LEFT JOIN Departamento D
                     ON D.CodDepartamento = P.CodDepartamento
                 LEFT JOIN Cargo C
                     ON C.CodCargo = P.CodCargo

                 WHERE 
                     AA.UsrNomeLogin = @login
                     AND AA.UsrPasswd  = @senhaHash
                     AND SI.CodSistema = @CodSistema;
            ";

            var parametros = new Dictionary<string, object>
            {
                { "@login", login },
                { "@senhaHash", senhaHash },
                { "@CodSistema", codSistema }
            };

    Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
    return db.LerPlennus(sql, parametros);
        }



        public DataTable ConsultaInfoPerfil()
        {
            string sql = @"
                            SELECT Nome, Conf_Simbolo
                            FROM Sistema
                            WHERE CodSistema = 1
                        ";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql);
        }

        public DataTable ConsultaInfoEmpresa(int CodSistema)
        {
            string sql = @"
                            SELECT NomeDisplay, Conf_Logo
                            FROM Sistema
                            WHERE CodSistema = @CodSistema
                        ";

                                var parametros = new Dictionary<string, object>
                        {
                            { "@CodSistema", CodSistema }
                        };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql, parametros);
        }
        public int GravarEnvioMensagem(string telefoneDestino, string codAssociado, string mensagemFinal, int codUsuarioEnvio, int codEmpresa = 400)
        {
            string sql = @"
                            INSERT INTO API_EnvioMensagemWpp  (
                                CodigoEmpresa,
                                CodigoAssociado,
                                NumTelefoneDestino,
                                DataEnvio,
                                Mensagem,
                                StatusEnvio,
                                CodUsuarioEnvio,
                                Informacoes_Log_I
                            )
                            OUTPUT INSERTED.CodEnvioMensagemWpp
                           VALUES (
                                @CodEmpresa,
                                @CodAssociado,
                                @TelefoneDestino,
                                GETDATE(),
                                @MensagemFinal,
                                'ENVIADO',
                                @CodUsuarioEnvio,
                                GETDATE()
                            )";

            var parametros = new Dictionary<string, object>
            {
                ["@CodEmpresa"] = codEmpresa,
                ["@CodAssociado"] = codAssociado,
                ["@TelefoneDestino"] = telefoneDestino,
                ["@MensagemFinal"] = mensagemFinal ?? "",
                ["@CodUsuarioEnvio"] = codUsuarioEnvio
            };

            return Convert.ToInt32(new Banco_Dados_SQLServer().ExecutarPlennusScalar(sql, parametros));
        }

        public void GravarRetornoMensagem(int codEnvioMensagemWpp, string statusEnvio, string idResposta, string conteudoApi, int codUsuarioEnvio)
        {
            // --- filtra o texto: pega apenas "Enviado: dd/MM/yyyy HH:mm:ss" e "Recebido: dd/MM/yyyy HH:mm:ss"
            string filtrado = null;
            if (!string.IsNullOrWhiteSpace(conteudoApi))
            {
                var mEnviado = Regex.Match(conteudoApi, @"Enviado:\s*\d{2}/\d{2}/\d{4}\s+\d{2}:\d{2}:\d{2}");
                var mRecebido = Regex.Match(conteudoApi, @"Recebido:\s*\d{2}/\d{2}/\d{4}\s+\d{2}:\d{2}:\d{2}");

                var partes = new List<string>(2);
                if (mEnviado.Success) partes.Add(mEnviado.Value);
                if (mRecebido.Success) partes.Add(mRecebido.Value);

                // quebra de linha entre eles (troque para " | " se preferir)
                filtrado = partes.Count == 0 ? null : string.Join(Environment.NewLine, partes);
            }

            // garante tamanho do ID (coluna VARCHAR(50))
            string idResp = string.IsNullOrWhiteSpace(idResposta) ? null : idResposta.Trim();
            if (!string.IsNullOrEmpty(idResp) && idResp.Length > 50)
                idResp = idResp.Substring(0, 50);

            string sql = @"
                        INSERT INTO [dbo].[API_RetornoMensagemWpp] (
                            CodEnvioMensagemWpp,
                            DataConfirmacao,
                            StatusEnvio,
                            ID_RESPOSTA_API,
                            STATUS_API_JSON,
                            CodUsuarioEnvio,
                            Informacoes_Log_I
                        )
                        VALUES (
                            @CodEnvioMensagemWpp,
                            GETDATE(),
                            @StatusEnvio,
                            @IdResposta,
                            CASE 
                              WHEN @StatusApiJson IS NULL THEN NULL
                              ELSE LEFT(
                                     @StatusApiJson,
                                     CASE 
                                       WHEN (SELECT DATA_TYPE 
                                             FROM INFORMATION_SCHEMA.COLUMNS 
                                             WHERE TABLE_SCHEMA='dbo' 
                                               AND TABLE_NAME='API_RetornoMensagemWpp' 
                                               AND COLUMN_NAME='STATUS_API_JSON') LIKE N'n%' 
                                       THEN COL_LENGTH('dbo.API_RetornoMensagemWpp','STATUS_API_JSON')/2
                                       ELSE COL_LENGTH('dbo.API_RetornoMensagemWpp','STATUS_API_JSON')
                                     END
                                   )
                            END,
                            @CodUsuarioEnvio,
                            GETDATE()
                        )";


            var parametros = new Dictionary<string, object>
            {
                ["@CodEnvioMensagemWpp"] = codEnvioMensagemWpp,
                ["@StatusEnvio"] = statusEnvio ?? (object)DBNull.Value,
                ["@IdResposta"] = (idResp ?? (object)DBNull.Value),
                ["@StatusApiJson"] = (filtrado ?? (object)DBNull.Value),
                ["@CodUsuarioEnvio"] = codUsuarioEnvio
            };

            new Banco_Dados_SQLServer().ExecutarPlennus(sql, parametros);
        }

        public bool EnviadosUltimas24H(string codigoAssociado, string template)
        {
            string sql = @"
                        SELECT TOP 1 1
                        FROM API_EnvioMensagemWpp
                        WHERE CodigoAssociado = @CodigoAssociado
                          AND Mensagem = @Template
                          AND DataEnvio >= DATEADD(HOUR, -24, GETDATE())";

            var parametros = new Dictionary<string, object>
            {
                ["@CodigoAssociado"] = codigoAssociado,
                ["@Template"] = template
            };

            object result = new Banco_Dados_SQLServer().LerPlennus(sql, parametros);
            return result != null;
        }
    }
}