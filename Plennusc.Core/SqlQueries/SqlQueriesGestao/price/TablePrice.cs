using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.price
{
    public class TablePrice
    {
        public void InserirPriceTable(DadosMensagemPriceTable item)
        {
            string sql = @"
            INSERT INTO PS1032 (
                CODIGO_PLANO,
                CODIGO_TABELA_PRECO,
                IDADE_MINIMA,
                IDADE_MAXIMA,
                VALOR_PLANO,
                Tipo_Relacao_Dependencia,
                INFORMACOES_LOG_I,
                INFORMACOES_LOG_A,
                VALOR_NET,
                CODIGO_GRUPO_CONTRATO,
                NOME_TABELA,
                TIPO_CONTRATO_ESTIPULADO,
                ID_INSTANCIA_PROCESSO
            )
            VALUES (
                @CODIGO_PLANO,
                @CODIGO_TABELA_PRECO,
                @IDADE_MINIMA,
                @IDADE_MAXIMA,
                @VALOR_PLANO,
                @Tipo_Relacao_Dependencia,
                GETDATE(),
                NULL,
                @VALOR_NET,
                @CODIGO_GRUPO_CONTRATO,
                @NOME_TABELA,
                @TIPO_CONTRATO_ESTIPULADO,
                NULL
            )";

            var parametros = new Dictionary<string, object>
            {
                ["@CODIGO_PLANO"] = item.CODIGO_PLANO,
                ["@CODIGO_TABELA_PRECO"] = item.CODIGO_TABELA_PRECO,
                ["@IDADE_MINIMA"] = item.IDADE_MINIMA,
                ["@IDADE_MAXIMA"] = item.IDADE_MAXIMA,
                ["@VALOR_PLANO"] = item.VALOR_PLANO,
                ["@Tipo_Relacao_Dependencia"] = item.TIPO_RELACAO_DEPENDENCIA,
                ["@VALOR_NET"] = item.VALOR_NET,
                ["@CODIGO_GRUPO_CONTRATO"] = item.CODIGO_GRUPO_CONTRATO,
                ["@NOME_TABELA"] = item.NOME_TABELA,
                ["@TIPO_CONTRATO_ESTIPULADO"] = item.TIPO_CONTRATO_ESTIPULADO
            };

            new Banco_Dados_SQLServer().ExecutarAlianca(sql, parametros);
        }

    }
}
