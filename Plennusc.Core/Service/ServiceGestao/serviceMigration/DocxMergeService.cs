using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.serviceMigration
{
    /// <summary>
    /// Exceção lançada quando não é encontrado, na pasta dos ~700 arquivos,
    /// nenhum .docx cujo nome bata com o e-mail do responsável.
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
    /// Junta vários .docx em um único arquivo, um atrás do outro, sem
    /// quebra de página forçada extra (cada documento entra exatamente
    /// como termina o anterior — se quiser separação, é o próprio
    /// conteúdo/estilo de cada docx que deve terminar/começar com quebra).
    ///
    /// Usa a técnica de AltChunk: cada documento extra é embutido como um
    /// "sub-documento" dentro do principal. É a forma mais segura de unir
    /// arquivos de origens diferentes, porque o Word resolve por conta
    /// própria conflitos de estilo, numeração de lista e numeração de
    /// página — não precisa copiar XML manualmente nem se preocupar com
    /// IDs de relacionamento colidindo.
    /// </summary>
    public class DocxMergeService
    {
        /// <param name="caminhoPrincipal">
        /// O primeiro documento da sequência (ex.: a Proposta já preenchida
        /// pelo DocxServiceSetCemg). Esse arquivo é copiado e vira a base;
        /// os outros são anexados dentro dele.
        /// </param>
        /// <param name="caminhosAdicionais">
        /// Os demais documentos, na ordem em que devem aparecer.
        /// </param>
        /// <param name="caminhoSaida">Caminho do arquivo final já unido.</param>
        public void Juntar(string caminhoPrincipal, List<string> caminhosAdicionais, string caminhoSaida)
        {
            File.Copy(caminhoPrincipal, caminhoSaida, true);

            using (var doc = WordprocessingDocument.Open(caminhoSaida, true))
            {
                var mainPart = doc.MainDocumentPart;
                var body = mainPart.Document.Body;

                int contador = 1;
                foreach (var caminhoExtra in caminhosAdicionais)
                {
                    string altChunkId = "altChunk" + contador++;

                    // Embute os bytes do docx extra como uma AlternativeFormatImportPart
                    var chunkPart = mainPart.AddAlternativeFormatImportPart(
                        AlternativeFormatImportPartType.WordprocessingML, altChunkId);

                    using (var fs = new FileStream(caminhoExtra, FileMode.Open, FileAccess.Read))
                    {
                        chunkPart.FeedData(fs);
                    }

                    // Insere a referência ao chunk no fim do corpo do documento
                    // principal — o Word abre o chunk "colado" no ponto onde
                    // o elemento AltChunk está.
                    var altChunk = new AltChunk { Id = altChunkId };

                    // Insere antes do SectionProperties final (se existir),
                    // senão só acrescenta no fim.
                    var sectPr = body.Elements<SectionProperties>().FirstOrDefault();
                    if (sectPr != null)
                        body.InsertBefore(altChunk, sectPr);
                    else
                        body.AppendChild(altChunk);
                }

                mainPart.Document.Save();
            }
        }

        /// <summary>
        /// Procura, dentro de <paramref name="pastaArquivos"/>, o .docx cujo
        /// nome (sem extensão) bate com o e-mail informado — comparação
        /// sem diferenciar maiúsculas/minúsculas, já que e-mail não é
        /// case-sensitive na prática (e o CSV pode vir em CAIXA ALTA).
        /// Lança <see cref="ArquivoComplementarNaoEncontradoException"/> se
        /// não encontrar nada, conforme decidido: não gerar nada e avisar
        /// o erro na tela.
        /// </summary>
        public string LocalizarArquivoPorEmail(string pastaArquivos, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArquivoComplementarNaoEncontradoException(email ?? "(vazio)");

            // O padrão real dos arquivos é o e-mail SEM o "@", em maiúsculo:
            // warlenrodrigues385@gmail.com  ->  WARLENRODRIGUES385GMAIL.COM.docx
            string emailNormalizado = email.Trim().Replace("@", "").ToUpperInvariant();

            var encontrado = Directory.EnumerateFiles(pastaArquivos, "*.docx")
                .FirstOrDefault(arquivo =>
                    Path.GetFileNameWithoutExtension(arquivo).Trim().Replace("@", "").ToUpperInvariant() == emailNormalizado);

            if (encontrado == null)
                throw new ArquivoComplementarNaoEncontradoException(email);

            return encontrado;
        }

        /// <summary>
        /// Fluxo completo: localiza o arquivo pelo e-mail do responsável e
        /// junta com a proposta já preenchida. Se o arquivo não existir,
        /// a exceção sobe e NADA é gerado (nem a proposta sozinha) —
        /// é responsabilidade de quem chama capturar e mostrar o erro
        /// na tela antes desse ponto, ou deixar a exceção propagar.
        /// </summary>
        public void JuntarPorEmail(string caminhoPropostaGerada, string pastaArquivosComplementares,
            string emailResponsavel, string caminhoSaida)
        {
            string arquivoComplementar = LocalizarArquivoPorEmail(pastaArquivosComplementares, emailResponsavel);

            Juntar(caminhoPropostaGerada, new List<string> { arquivoComplementar }, caminhoSaida);
        }
    }
}
