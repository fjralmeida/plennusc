using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Plennusc.Core.Service.ServiceGestao.serviceMigration
{
    /// <summary>
    /// Exceção lançada quando não é encontrado, na pasta dos arquivos
    /// complementares, nenhum arquivo cujo nome bata com o e-mail do
    /// responsável.
    /// </summary>
    public class ArquivoComplementarNaoEncontradoException : Exception
    {
        public string EmailBuscado { get; }

        public ArquivoComplementarNaoEncontradoException(string email)
            : base($"Nenhum arquivo encontrado na pasta para o e-mail '{email}'. " +
                   "Verifique se o arquivo existe e se o nome bate exatamente com o e-mail do responsável.")
        {
            EmailBuscado = email;
        }
    }

    /// <summary>
    /// Junta PDFs já prontos, um atrás do outro — cada página é só copiada
    /// como está, sem nenhum recálculo de layout. Isso resolve o problema
    /// dos documentos com caixas de texto flutuantes: como o conteúdo já
    /// está "congelado" em página de PDF, juntar aqui nunca desloca nada.
    /// </summary>
    public class PdfMergeService
    {
        /// <summary>
        /// Procura, dentro de <paramref name="pastaArquivos"/>, o arquivo
        /// (docx OU pdf) cujo nome (sem extensão) bate com o e-mail
        /// informado, no padrão usado na pasta PROPOSTAS_UNIMED_SETCEMG:
        /// o e-mail SEM o "@", comparado sem diferenciar maiúsculas/minúsculas
        /// (ex.: warlenrodrigues385@gmail.com -> WARLENRODRIGUES385GMAIL.COM).
        /// Lança <see cref="ArquivoComplementarNaoEncontradoException"/> se
        /// não encontrar nada — nesse caso, nada deve ser gerado.
        /// </summary>
        /// <summary>
        /// Procura, dentro de <paramref name="pastaArquivos"/>, TODOS os
        /// arquivos (docx OU pdf) cujo nome (sem extensão) começa com o
        /// CNPJ informado — pode haver mais de um contrato pro mesmo CNPJ.
        /// Comparação usa só os dígitos do CNPJ (ignora pontuação/barra),
        /// pra não depender de como o arquivo foi salvo.
        /// Lança <see cref="ArquivoComplementarNaoEncontradoException"/> se
        /// não encontrar NENHUM — nesse caso, nada deve ser gerado.
        /// </summary>
        public List<string> LocalizarArquivosPorCnpj(string pastaArquivos, string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ArquivoComplementarNaoEncontradoException(cnpj ?? "(vazio)");

            string cnpjNormalizado = SoDigitos(cnpj);

            var encontrados = Directory.EnumerateFiles(pastaArquivos)
                .Where(f => f.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                .Where(arquivo => SoDigitos(Path.GetFileNameWithoutExtension(arquivo)).Contains(cnpjNormalizado))
                .OrderBy(arquivo => arquivo) // ordem estável e previsível
                .ToList();

            if (encontrados.Count == 0)
                throw new ArquivoComplementarNaoEncontradoException(cnpj);

            return encontrados;
        }

        private string SoDigitos(string texto)
        {
            return new string((texto ?? "").Where(char.IsDigit).ToArray());
        }

        // Mantido por compatibilidade, caso ainda seja usado em algum lugar.
        public string LocalizarArquivoPorEmail(string pastaArquivos, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArquivoComplementarNaoEncontradoException(email ?? "(vazio)");

            string emailNormalizado = email.Trim().Replace("@", "").ToUpperInvariant();

            var encontrado = Directory.EnumerateFiles(pastaArquivos)
                .Where(f => f.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(arquivo =>
                    Path.GetFileNameWithoutExtension(arquivo).Trim().Replace("@", "").ToUpperInvariant() == emailNormalizado);

            if (encontrado == null)
                throw new ArquivoComplementarNaoEncontradoException(email);

            return encontrado;
        }

        /// <param name="caminhosPdf">
        /// Lista de PDFs na ordem em que devem aparecer no arquivo final
        /// (ex.: [proposta.pdf, complementar.pdf]).
        /// </param>
        public void Juntar(List<string> caminhosPdf, string caminhoSaida)
        {
            using (var documentoFinal = new PdfDocument())
            {
                foreach (var caminho in caminhosPdf)
                {
                    using (var origem = PdfReader.Open(caminho, PdfDocumentOpenMode.Import))
                    {
                        foreach (var pagina in origem.Pages)
                        {
                            documentoFinal.AddPage(pagina);
                        }
                    }
                }

                documentoFinal.Save(caminhoSaida);
            }
        }

        /// <summary>
        /// Junta a proposta com UM OU MAIS complementares, cobrindo com um
        /// retângulo branco a faixa de cabeçalho (logos) de TODAS as
        /// páginas de TODOS os complementares — pra não duplicar o
        /// cabeçalho que a proposta já tem na própria página. Use essa
        /// versão quando o mesmo CNPJ tiver mais de um contrato/arquivo
        /// pra anexar.
        /// </summary>
        /// <param name="pdfProposta">PDF da proposta (gerada pelo nosso template).</param>
        /// <param name="pdfsComplementares">PDFs dos termos já assinados, na ordem em que devem aparecer.</param>
        /// <param name="alturaCabecalhoPontos">
        /// Altura, em pontos (1 pt = 1/72 polegada), da faixa a cobrir no
        /// topo de cada página dos complementares. Ajuste esse número
        /// olhando o PDF real — comece testando com 90 e vá calibrando.
        /// </param>
        public void JuntarOcultandoCabecalhoComplementar(
            string pdfProposta, List<string> pdfsComplementares, string caminhoSaida, double alturaCabecalhoPontos = 90)
        {
            using (var documentoFinal = new PdfDocument())
            {
                // páginas da proposta entram sem alteração
                using (var origemProposta = PdfReader.Open(pdfProposta, PdfDocumentOpenMode.Import))
                {
                    foreach (var pagina in origemProposta.Pages)
                        documentoFinal.AddPage(pagina);
                }

                // páginas de CADA complementar entram com o cabeçalho mascarado
                foreach (var pdfComplementar in pdfsComplementares)
                {
                    using (var origemComplementar = PdfReader.Open(pdfComplementar, PdfDocumentOpenMode.Import))
                    {
                        foreach (var paginaOrigem in origemComplementar.Pages)
                        {
                            var novaPagina = documentoFinal.AddPage(paginaOrigem);

                            using (var gfx = XGraphics.FromPdfPage(novaPagina))
                            {
                                var pincel = XBrushes.White;
                                gfx.DrawRectangle(pincel, 0, 0, novaPagina.Width.Point, alturaCabecalhoPontos);
                            }
                        }
                    }
                }

                documentoFinal.Save(caminhoSaida);
            }
        }

        /// <summary>
        /// Sobrecarga de compatibilidade pra um único complementar.
        /// </summary>
        public void JuntarOcultandoCabecalhoComplementar(
            string pdfProposta, string pdfComplementar, string caminhoSaida, double alturaCabecalhoPontos = 90)
        {
            JuntarOcultandoCabecalhoComplementar(
                pdfProposta, new List<string> { pdfComplementar }, caminhoSaida, alturaCabecalhoPontos);
        }
    }
}