using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Plennusc.Core.Service.ServiceGestao.serviceMigration
{
    public class DocxToPdfConverter
    {
        private readonly string _caminhoSoffice;

        /// <summary>
        /// Construtor que tenta localizar o LibreOffice em locais padrão.
        /// Se quiser forçar um caminho, configure no web.config (chave "CaminhoLibreOffice").
        /// </summary>
        public DocxToPdfConverter(string caminhoSoffice = null)
        {
            if (!string.IsNullOrEmpty(caminhoSoffice))
            {
                _caminhoSoffice = caminhoSoffice;
                return;
            }

            // 1. Tenta ler do web.config / app.config (prioridade máxima)
            try
            {
                string caminhoConfig = System.Configuration.ConfigurationManager.AppSettings["CaminhoLibreOffice"];
                if (!string.IsNullOrEmpty(caminhoConfig) && File.Exists(caminhoConfig))
                {
                    _caminhoSoffice = caminhoConfig;
                    return;
                }
            }
            catch { /* se não tiver configuração, segue o jogo */ }

            // 2. Procura nos locais MAIS COMUNS (sem varrer pastas protegidas)
            _caminhoSoffice = LocalizarLibreOffice();

            if (string.IsNullOrEmpty(_caminhoSoffice) || !File.Exists(_caminhoSoffice))
            {
                throw new FileNotFoundException(
                    "LibreOffice não encontrado nos locais padrão. " +
                    "Instale o LibreOffice OU configure o caminho exato no web.config (chave 'CaminhoLibreOffice'). " +
                    "Exemplo: <add key='CaminhoLibreOffice' value='C:\\Program Files\\LibreOffice\\program\\soffice.exe' />");
            }
        }

        private string LocalizarLibreOffice()
        {
            // Lista de caminhos prováveis (sem recursão, sem erro de permissão)
            string[] candidatos = new string[]
            {
                @"C:\Program Files\LibreOffice\program\soffice.exe",
                @"C:\Program Files\LibreOffice\program\soffice.com",
                @"C:\Program Files (x86)\LibreOffice\program\soffice.exe",
                @"C:\Program Files (x86)\LibreOffice\program\soffice.com",
                @"C:\LibreOffice\program\soffice.exe",      // instalação portável
                @"C:\LibreOffice\program\soffice.com",
            };

            foreach (string caminho in candidatos)
            {
                if (File.Exists(caminho))
                    return caminho;
            }

            // 3. Última tentativa: procurar no PATH do sistema (ex: se instalou via chocolatey/winget)
            string[] nomes = { "soffice.exe", "soffice.com" };
            var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
            foreach (var dir in pathEnv.Split(';'))
            {
                if (string.IsNullOrWhiteSpace(dir)) continue;
                foreach (var nome in nomes)
                {
                    string caminho = Path.Combine(dir.Trim(), nome);
                    if (File.Exists(caminho))
                        return caminho;
                }
            }

            return null;
        }

        public string ConverterParaPdf(string caminhoDocx, string pastaSaida)
        {
            if (!File.Exists(_caminhoSoffice))
                throw new FileNotFoundException($"LibreOffice não encontrado em '{_caminhoSoffice}'.");

            if (!File.Exists(caminhoDocx))
                throw new FileNotFoundException($"Arquivo não encontrado: {caminhoDocx}");

            if (!Directory.Exists(pastaSaida))
                Directory.CreateDirectory(pastaSaida);

            var psi = new ProcessStartInfo
            {
                FileName = _caminhoSoffice,
                Arguments = $"--headless --convert-to pdf --outdir \"{pastaSaida}\" \"{caminhoDocx}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            using (var processo = Process.Start(psi))
            {
                string saida = processo.StandardOutput.ReadToEnd();
                string erro = processo.StandardError.ReadToEnd();

                bool terminouATempo = processo.WaitForExit(60000);

                if (!terminouATempo)
                {
                    processo.Kill();
                    throw new TimeoutException($"Conversão de '{caminhoDocx}' para PDF demorou demais.");
                }

                if (processo.ExitCode != 0)
                    throw new InvalidOperationException(
                        $"Erro ao converter (código {processo.ExitCode}). Saída: {saida}. Erro: {erro}");
            }

            string nomeSemExtensao = Path.GetFileNameWithoutExtension(caminhoDocx);
            string caminhoPdfGerado = Path.Combine(pastaSaida, nomeSemExtensao + ".pdf");

            if (!File.Exists(caminhoPdfGerado))
                throw new InvalidOperationException($"PDF não gerado em '{caminhoPdfGerado}'.");

            return caminhoPdfGerado;
        }
    }
}