using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Junta a proposta com o complementar, mas cobre com um retângulo
        /// branco a faixa de cabeçalho (logos) de TODAS as páginas do
        /// complementar, pra não duplicar o cabeçalho que a proposta já
        /// tem na própria página.
        /// </summary>
        /// <param name="pdfProposta">PDF da proposta (gerada pelo nosso template).</param>
        /// <param name="pdfComplementar">PDF do termo já assinado (nativo do DocuSign).</param>
        /// <param name="alturaCabecalhoPontos">
        /// Altura, em pontos (1 pt = 1/72 polegada), da faixa a cobrir no
        /// topo de cada página do complementar. Ajuste esse número olhando
        /// o PDF real — comece testando com 90 e vá calibrando.
        /// </param>
        public void JuntarOcultandoCabecalhoComplementar(
            string pdfProposta, string pdfComplementar, string caminhoSaida, double alturaCabecalhoPontos = 90)
        {
            using (var documentoFinal = new PdfDocument())
            {
                // páginas da proposta entram sem alteração
                using (var origemProposta = PdfReader.Open(pdfProposta, PdfDocumentOpenMode.Import))
                {
                    foreach (var pagina in origemProposta.Pages)
                        documentoFinal.AddPage(pagina);
                }

                // páginas do complementar entram com o cabeçalho mascarado
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

                documentoFinal.Save(caminhoSaida);
            }
        }
    }
}