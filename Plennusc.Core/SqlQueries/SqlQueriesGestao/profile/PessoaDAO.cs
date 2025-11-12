using appWhatsapp.Data_Bd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.profile
{
    public class PessoaDAO
    {
        public DataRow ObterPessoaPorUsuario(int codUsuario)
        {
            string sql = @"
                SELECT TOP 1
                    P.CodPessoa, P.Nome, P.Sobrenome, P.Apelido, P.Sexo, P.DataNasc, 
                    P.CodEstr_TipoPessoa, P.ImagemFoto, P.DocRG, P.DocCPF, P.TituloEleitor, 
                    P.ZonaEleitor, P.SecaoEleitor, P.NumCTPS, P.NumCTPSSerie, P.NumCTPSUf, 
                    P.NumPIS, P.Matricula, P.DataAdmissao, P.DataDemissao, 
                    P.NomeFiliacao1, P.NomeFiliacao2, P.Telefone1, P.Telefone2, P.Telefone3, 
                    P.Email, P.EmailAlt, P.CodCargo, P.CodDepartamento, 
                    P.Conf_CriaContaAD, P.DataHora_CriaContaAD, 
                    P.Conf_CadastraPonto, P.DataHora_CadastraPonto, 
                    P.Conf_Ativo, P.PermiteAcesso, P.AcessoSite, P.DataUltimoAcesso, 
                    P.Observacao, P.Informacoes_Log_I, P.Informacoes_Log_A, P.Informacoes_Log_E
                FROM Pessoa P
                INNER JOIN AutenticacaoAcesso AA ON AA.CodPessoa = P.CodPessoa
                WHERE AA.CodAutenticacaoAcesso = @CodUsuario
            ";

            var parametros = new Dictionary<string, object>
        {
            { "@CodUsuario", codUsuario }
        };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            DataTable dt = db.LerPlennus(sql, parametros);

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataTable TipoEstrutura()
        {
            string query = @"SELECT 
                        MIN(CodEstrutura) as CodEstrutura, 
                        DescEstrutura 
                     FROM VW_PERFIL_PESSOA 
                     GROUP BY DescEstrutura
                     ORDER BY DescEstrutura";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(query);
        }

        public DataTable TipoCargo()
        {
            string query = "SELECT CodCargo, Nome FROM cargo ORDER BY Nome";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(query);
        }
        public DataTable TipoDepartamento()
        {
            string query = "SELECT CodDepartamento, Nome FROM departamento ORDER BY Nome";
            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(query);
        }

        public void UpdateImgPerfil(int codPessoa, string nomeImagem)
        {
            string sql = "UPDATE Pessoa SET ImagemFoto = @ImagemFoto WHERE CodPessoa = @CodPessoa";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();

            var parametros = new Dictionary<string, object>
            {
                { "@ImagemFoto", nomeImagem },
                { "@CodPessoa", codPessoa }
            };

            db.ExecutarPlennus(sql, parametros);
        }

        public int InsertPersonSystem(
      int codEstrutura, string nome, string sobrenome, string apelido, string sexo, DateTime? dataNasc, string cpf, string rg,
      string tituloEleitor, string zona, string secao, string ctps, string serie, string uf, string pis, string matricula,
      DateTime? dataAdmissao, string filiacao1, string filiacao2,
      string telefone1, string telefone2, string telefone3,
      string email, string emailAlt, int codCargo, int codDepartamento,
      bool criaContaAD, bool cadastraPonto, bool ativo, bool permiteAcesso,
      int codSistema, int codUsuario, string observacao)
        {
            string sql = @"
                INSERT INTO Pessoa (
                    CodEstr_TipoPessoa, Nome, Sobrenome, Apelido, Sexo, DataNasc, DocCPF, DocRG,
                    TituloEleitor, ZonaEleitor, SecaoEleitor, NumCTPS, NumCTPSSerie, NumCTPSUf,
                    NumPIS, Matricula, DataAdmissao,
                    NomeFiliacao1, NomeFiliacao2, Telefone1, Telefone2, Telefone3,
                    Email, EmailAlt, CodCargo, CodDepartamento,
                    Conf_CriaContaAD, DataHora_CriaContaAD,
                    Conf_CadastraPonto, DataHora_CadastraPonto,
                    Conf_Ativo, PermiteAcesso, AcessoSite, DataUltimoAcesso,
                    Observacao, Informacoes_Log_I
                ) 
                OUTPUT INSERTED.CodPessoa
                VALUES (
                    @CodEstr_TipoPessoa, @Nome, @Sobrenome, @Apelido, @Sexo, @DataNasc, @DocCPF, @DocRG,
                    @TituloEleitor, @ZonaEleitor, @SecaoEleitor, @NumCTPS, @NumCTPSSerie, @NumCTPSUf,
                    @NumPIS, @Matricula, @DataAdmissao,
                    @NomeFiliacao1, @NomeFiliacao2, @Telefone1, @Telefone2, @Telefone3,
                    @Email, @EmailAlt, @CodCargo, @CodDepartamento,
                    @Conf_CriaContaAD, @DataHora_CriaContaAD,
                    @Conf_CadastraPonto, @DataHora_CadastraPonto,
                    @Conf_Ativo, @PermiteAcesso, @AcessoSite, @DataUltimoAcesso,
                    @Observacao, @Informacoes_Log_I
                )";

                    DateTime agora = DateTime.Now;
                    DateTime agoraSemSegundos = new DateTime(agora.Year, agora.Month, agora.Day, agora.Hour, agora.Minute, 0);

                    var parametros = new Dictionary<string, object>
            {
                { "@CodEstr_TipoPessoa", codEstrutura },
                { "@Nome", nome },
                { "@Sobrenome", sobrenome },
                { "@Apelido", string.IsNullOrWhiteSpace(apelido) ? "" : apelido },
                { "@Sexo", sexo },
                { "@DataNasc", dataNasc.HasValue ? (object)dataNasc.Value : DBNull.Value },
                { "@DocCPF", cpf },
                { "@DocRG", rg },
                { "@TituloEleitor", string.IsNullOrWhiteSpace(tituloEleitor) ? "" : tituloEleitor },
                { "@ZonaEleitor", string.IsNullOrWhiteSpace(zona) ? "" : zona },
                { "@SecaoEleitor", string.IsNullOrWhiteSpace(secao) ? "" : secao },
                { "@NumCTPS", string.IsNullOrWhiteSpace(ctps) ? "" : ctps },
                { "@NumCTPSSerie", string.IsNullOrWhiteSpace(serie) ? "" : serie },
                { "@NumCTPSUf", string.IsNullOrWhiteSpace(uf) ? "" : uf },
                { "@NumPIS", string.IsNullOrWhiteSpace(pis) ? (object)DBNull.Value : pis },
                { "@Matricula", string.IsNullOrWhiteSpace(matricula) ? "" : matricula },
                { "@DataAdmissao", dataAdmissao.HasValue ? (object)dataAdmissao.Value : DBNull.Value },
                { "@NomeFiliacao1", string.IsNullOrWhiteSpace(filiacao1) ? "Não informado" : filiacao1 },
                { "@NomeFiliacao2", string.IsNullOrWhiteSpace(filiacao2) ? "" : filiacao2 },
                { "@Telefone1", telefone1 },
                { "@Telefone2", string.IsNullOrWhiteSpace(telefone2) ? "" : telefone2 },
                { "@Telefone3", string.IsNullOrWhiteSpace(telefone3) ? "" : telefone3 },
                { "@Email", email },
                { "@EmailAlt", string.IsNullOrWhiteSpace(emailAlt) ? "" : emailAlt },
                { "@CodCargo", codCargo },
                { "@CodDepartamento", codDepartamento },
                { "@Conf_CriaContaAD", criaContaAD ? 1 : 0 },
                { "@DataHora_CriaContaAD", criaContaAD ? (object)agora : DBNull.Value },
                { "@Conf_CadastraPonto", cadastraPonto ? 1 : 0 },
                { "@DataHora_CadastraPonto", cadastraPonto ? (object)agora : DBNull.Value },
                { "@Conf_Ativo", ativo ? 1 : 0 },
                { "@PermiteAcesso", permiteAcesso ? 1 : 0 },
                { "@AcessoSite", 0 },
                { "@DataUltimoAcesso", DBNull.Value },
                { "@Observacao", string.IsNullOrWhiteSpace(observacao) ? "" : observacao },
                { "@Informacoes_Log_I", agoraSemSegundos }
            };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();

            // MUDANÇA AQUI: Use o método que JÁ EXISTE
            var result = db.ExecutarPlennusScalar(sql, parametros);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public void UpdatePersonSystem(
    int codPessoa,
    int codEstrutura, string nome, string sobrenome, string apelido, string sexo, DateTime? dataNasc, string cpf, string rg,
    string tituloEleitor, string zona, string secao, string ctps, string serie, string uf, string pis, string matricula,
    DateTime? dataAdmissao, string filiacao1, string filiacao2,
    string telefone1, string telefone2, string telefone3,
    string email, string emailAlt, int codCargo, int codDepartamento,
    bool criaContaAD, bool cadastraPonto, bool ativo, bool permiteAcesso,
    int codSistema, int codUsuario, string observacao)
        {
            string sql = @"
              UPDATE Pessoa
                SET
                    CodEstr_TipoPessoa = @CodEstr_TipoPessoa,
                    Nome               = @Nome,
                    Sobrenome          = @Sobrenome,
                    Apelido            = @Apelido,
                    Sexo               = @Sexo,
                    DataNasc           = @DataNasc,
                    DocCPF             = @DocCPF,
                    DocRG              = @DocRG,
                    TituloEleitor      = @TituloEleitor,
                    ZonaEleitor        = @ZonaEleitor,
                    SecaoEleitor       = @SecaoEleitor,
                    NumCTPS            = @NumCTPS,
                    NumCTPSSerie       = @NumCTPSSerie,
                    NumCTPSUf          = @NumCTPSUf,
                    NumPIS             = @NumPIS,
                    Matricula          = @Matricula,
                    DataAdmissao       = @DataAdmissao,
                    NomeFiliacao1      = @NomeFiliacao1,
                    NomeFiliacao2      = @NomeFiliacao2,
                    Telefone1          = @Telefone1,
                    Telefone2          = @Telefone2,
                    Telefone3          = @Telefone3,
                    Email              = @Email,
                    EmailAlt           = @EmailAlt,
                    CodCargo           = @CodCargo,
                    CodDepartamento    = @CodDepartamento,
                    Conf_CriaContaAD   = @Conf_CriaContaAD,
                    DataHora_CriaContaAD = CASE 
                        WHEN @Conf_CriaContaAD = 1 AND (DataHora_CriaContaAD IS NULL) THEN @Agora 
                        ELSE DataHora_CriaContaAD 
                    END,
                    Conf_CadastraPonto = @Conf_CadastraPonto,
                    DataHora_CadastraPonto = CASE 
                        WHEN @Conf_CadastraPonto = 1 AND (DataHora_CadastraPonto IS NULL) THEN @Agora 
                        ELSE DataHora_CadastraPonto 
                    END,
                    Conf_Ativo         = @Conf_Ativo,
                    PermiteAcesso      = @PermiteAcesso,
                    Observacao         = @Observacao,
                    Informacoes_Log_A  = @AgoraSemSegundos
                WHERE CodPessoa = @CodPessoa;
            ";

            DateTime agora = DateTime.Now;
            DateTime agoraSemSegundos = new DateTime(agora.Year, agora.Month, agora.Day, agora.Hour, agora.Minute, 0);

            var parametros = new Dictionary<string, object>
            {
                { "@CodPessoa", codPessoa },
                { "@CodEstr_TipoPessoa", codEstrutura },
                { "@Nome", nome },
                { "@Sobrenome", sobrenome },
                { "@Apelido", string.IsNullOrWhiteSpace(apelido) ? "" : apelido },
                { "@Sexo", sexo },
                { "@DataNasc", dataNasc.HasValue ? (object)dataNasc.Value : DBNull.Value },
                { "@DocCPF", cpf },
                { "@DocRG", rg },
                { "@TituloEleitor", string.IsNullOrWhiteSpace(tituloEleitor) ? "" : tituloEleitor },
                { "@ZonaEleitor", string.IsNullOrWhiteSpace(zona) ? "" : zona },
                { "@SecaoEleitor", string.IsNullOrWhiteSpace(secao) ? "" : secao },
                { "@NumCTPS", string.IsNullOrWhiteSpace(ctps) ? "" : ctps },
                { "@NumCTPSSerie", string.IsNullOrWhiteSpace(serie) ? "" : serie },
                { "@NumCTPSUf", string.IsNullOrWhiteSpace(uf) ? "" : uf },
                { "@NumPIS", string.IsNullOrWhiteSpace(pis) ? (object)DBNull.Value : pis },
                { "@Matricula", string.IsNullOrWhiteSpace(matricula) ? "" : matricula },
                { "@DataAdmissao", dataAdmissao.HasValue ? (object)dataAdmissao.Value : DBNull.Value },
                { "@NomeFiliacao1", string.IsNullOrWhiteSpace(filiacao1) ? "Não informado" : filiacao1 },
                { "@NomeFiliacao2", string.IsNullOrWhiteSpace(filiacao2) ? "" : filiacao2 },
                { "@Telefone1", telefone1 },
                { "@Telefone2", string.IsNullOrWhiteSpace(telefone2) ? "" : telefone2 },
                { "@Telefone3", string.IsNullOrWhiteSpace(telefone3) ? "" : telefone3 },
                { "@Email", email },
                { "@EmailAlt", string.IsNullOrWhiteSpace(emailAlt) ? "" : emailAlt },
                { "@CodCargo", codCargo },
                { "@CodDepartamento", codDepartamento },
                { "@Conf_CriaContaAD", criaContaAD ? 1 : 0 },
                { "@Conf_CadastraPonto", cadastraPonto ? 1 : 0 },
                { "@Conf_Ativo", ativo ? 1 : 0 },
                { "@PermiteAcesso", permiteAcesso ? 1 : 0 },
                { "@Observacao", string.IsNullOrWhiteSpace(observacao) ? "" : observacao },
                { "@Agora", agora },                 // para carimbar datas quando ligar flags
                { "@AgoraSemSegundos", agoraSemSegundos }
            };

            var db = new Banco_Dados_SQLServer();
            db.ExecutarPlennus(sql, parametros);
        }

        public DataTable BuscarUsuarioPorNome(string nome)
        {
            string nomeEsc = (nome ?? "").Replace("'", "''");

            string query = $@"
                SELECT 
                    p.CodPessoa,
                    (ISNULL(p.Nome,'') + ' ' + ISNULL(p.Sobrenome,'')) AS NomeCompleto,
                    p.DocCPF      AS CPF,
                    p.Telefone1,
                    p.Email,
                    CASE WHEN p.Conf_Ativo = 1 THEN 'Sim' ELSE 'Não' END AS Conf_Ativo,
                    p.CodCargo,
                    p.CodDepartamento,
                    ISNULL(c.Nome, '') AS NomeCargo,
                    ISNULL(d.Nome, '') AS NomeDepartamento
                FROM Pessoa p
                LEFT JOIN Cargo        c ON c.CodCargo        = p.CodCargo
                LEFT JOIN Departamento d ON d.CodDepartamento = p.CodDepartamento
                WHERE (p.Nome LIKE '%{nomeEsc}%' OR p.Sobrenome LIKE '%{nomeEsc}%')
                  AND (p.CodEstr_TipoPessoa IS NULL OR p.CodEstr_TipoPessoa <> 2)  -- oculta administradores
                ORDER BY p.Nome, p.Sobrenome;";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(query);
        }
        public DataTable BuscarUsuarioPorCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return new DataTable();

            // remove pontos e traço
            string cpfLimpo = (cpf ?? "").Replace(".", "").Replace("-", "").Trim().Replace("'", "''");

            string query = $@"
                SELECT 
                    p.CodPessoa,
                    (ISNULL(p.Nome,'') + ' ' + ISNULL(p.Sobrenome,'')) AS NomeCompleto,
                    p.DocCPF      AS CPF,
                    p.Telefone1,
                    p.Email,
                    CASE WHEN p.Conf_Ativo = 1 THEN 'Sim' ELSE 'Não' END AS Conf_Ativo,
                    p.CodCargo,
                    p.CodDepartamento,
                    ISNULL(c.Nome, '') AS NomeCargo,
                    ISNULL(d.Nome, '') AS NomeDepartamento
                FROM Pessoa p
                LEFT JOIN Cargo        c ON c.CodCargo        = p.CodCargo
                LEFT JOIN Departamento d ON d.CodDepartamento = p.CodDepartamento
                WHERE p.DocCPF = '{cpfLimpo}'
                  AND (p.CodEstr_TipoPessoa IS NULL OR p.CodEstr_TipoPessoa <> 2);  -- oculta administradores";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(query);
        }

        public DataTable BuscarUsuarioPorDepartamento(string departamento)
        {
            if (string.IsNullOrWhiteSpace(departamento))
                return new DataTable(); // evita trazer tudo

            string termo = departamento.Trim();
            bool isCodigo = Regex.IsMatch(termo, @"^\d+$");
            string termoSql = termo.Replace("'", "''");

            // WHERE base conforme o tipo do termo
            string whereBase = isCodigo
                ? $"WHERE p.CodDepartamento = {termoSql}"
                : $"WHERE d.Nome LIKE '%{termoSql}%'";

            // acrescenta o filtro para ocultar administradores (CodEstr_TipoPessoa = 2)
            string where = $"{whereBase} AND (p.CodEstr_TipoPessoa IS NULL OR p.CodEstr_TipoPessoa <> 2)";

            string query = $@"
                SELECT
                    p.CodPessoa,
                    (ISNULL(p.Nome,'') + ' ' + ISNULL(p.Sobrenome,'')) AS NomeCompleto,
                    p.DocCPF     AS CPF,
                    p.Email,
                    p.Telefone1,
                    CASE WHEN p.Conf_Ativo = 1 THEN 'Sim' ELSE 'Não' END AS Conf_Ativo,
                    p.CodDepartamento,
                    p.CodCargo,
                    ISNULL(c.Nome, '') AS NomeCargo,
                    ISNULL(d.Nome, '') AS NomeDepartamento
                FROM Pessoa p
                LEFT JOIN Cargo        c ON c.CodCargo        = p.CodCargo
                LEFT JOIN Departamento d ON d.CodDepartamento = p.CodDepartamento
                {where}
                ORDER BY p.Nome, p.Sobrenome;";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(query);
        }


        public void AtualizarUsuario(int codPessoa, string nomeCompleto, string cpf, string rg, string email, string telefone, string cargo)
        {
            // Separa nome e sobrenome
            string[] partes = nomeCompleto.Trim().Split(' ');
            string nome = partes[0];
            string sobrenome = partes.Length > 1 ? string.Join(" ", partes.Skip(1)) : "";

            string sql = @"
                        UPDATE Pessoa
                        SET 
                            Nome = @Nome,
                            Sobrenome = @Sobrenome,
                            DocCPF = @CPF,
                            DocRG = @RG,
                            Email = @Email,
                            Telefone1 = @Telefone,
                            CodCargo = @Cargo
                        WHERE CodPessoa = @CodPessoa";

                            var parametros = new Dictionary<string, object>
                    {
                        { "@Nome", nome },
                        { "@Sobrenome", sobrenome },
                        { "@CPF", cpf },
                        { "@RG", rg },
                        { "@Email", email },
                        { "@Telefone", telefone },
                        { "@Cargo", cargo },
                        { "@CodPessoa", codPessoa }
                    };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            db.ExecutarPlennus(sql, parametros);
        }

        public void InactivateUser(int codPessoa, string motivo)
        {
            const string sql = @"
BEGIN TRY
    BEGIN TRAN;

    UPDATE P
       SET P.Conf_Ativo = 0,
           P.Observacao =
                COALESCE(NULLIF(RTRIM(P.Observacao), ''), '')
                + CASE WHEN COALESCE(NULLIF(RTRIM(P.Observacao), ''), '') = '' THEN '' ELSE CHAR(13)+CHAR(10) END
                + CONVERT(varchar(19), GETDATE(), 120) + ' - Inativação: ' + @Motivo
     FROM Pessoa P
    WHERE P.CodPessoa = @CodPessoa;

    UPDATE AA
       SET AA.Conf_Ativo = 0,
           AA.Conf_PermiteAcesso = 0
     FROM AutenticacaoAcesso AA
    WHERE AA.CodPessoa = @CodPessoa;

    -- 2.1) Se a coluna de log existir, atualiza via dynamic SQL (evita erro de compilação)
    IF COL_LENGTH('AutenticacaoAcesso','Informacoes_log_a') IS NOT NULL
    BEGIN
        DECLARE @sql1 nvarchar(max) = N'
            UPDATE AA
               SET AA.Informacoes_log_a = SYSDATETIME()
             FROM AutenticacaoAcesso AA
            WHERE AA.CodPessoa = @CodPessoa;';
        EXEC sp_executesql @sql1, N'@CodPessoa int', @CodPessoa=@CodPessoa;
    END

    UPDATE SEU
       SET SEU.Conf_LiberaAcesso = 0,
           SEU.Conf_BloqueiaAcesso = 1
      FROM SistemaEmpresaUsuario SEU
      INNER JOIN AutenticacaoAcesso AA
              ON AA.CodAutenticacaoAcesso = SEU.CodAutenticacaoAcesso
     WHERE AA.CodPessoa = @CodPessoa;

    IF COL_LENGTH('SistemaEmpresaUsuario','Informacoes_log_a') IS NOT NULL
    BEGIN
        DECLARE @sql2 nvarchar(max) = N'
            UPDATE SEU
               SET SEU.Informacoes_log_a = SYSDATETIME()
              FROM SistemaEmpresaUsuario SEU
              INNER JOIN AutenticacaoAcesso AA
                      ON AA.CodAutenticacaoAcesso = SEU.CodAutenticacaoAcesso
             WHERE AA.CodPessoa = @CodPessoa;';
        EXEC sp_executesql @sql2, N'@CodPessoa int', @CodPessoa=@CodPessoa;
    END

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    DECLARE @ErrMsg nvarchar(4000) = ERROR_MESSAGE(),
            @ErrNum int = ERROR_NUMBER();
    RAISERROR('InactivateUser failed (%d): %s', 16, 1, @ErrNum, @ErrMsg);
END CATCH;";

            var parametros = new Dictionary<string, object>
    {
        { "@CodPessoa", codPessoa },
        { "@Motivo", motivo ?? string.Empty }
    };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            db.ExecutarPlennus(sql, parametros);
        }

        public DataTable GetTotalUsuarios()
        {
            string sql = @"
                SELECT COUNT(*) AS Total
                FROM Pessoa
                WHERE Conf_Ativo = 1 
                  AND CodCargo IS NOT NULL 
                  AND CodDepartamento IS NOT NULL
            ";

            Banco_Dados_SQLServer bd = new Banco_Dados_SQLServer();
            return bd.LerPlennus(sql);
        }

        public DataTable ObterPessoaCompleta(int codPessoa)
        {
            string sql = @"
                SELECT 
                    p.CodPessoa,
                    p.Nome,
                    p.Sobrenome,
                    p.Apelido,
                    p.Sexo,
                    p.DataNasc,
                    p.DocRG,
                    p.DocCPF,
                    p.TituloEleitor,
                    p.ZonaEleitor,
                    p.SecaoEleitor,
                    p.NumCTPS,
                    p.NumCTPSSerie,
                    p.NumCTPSUf,
                    p.NumPIS,
                    p.Matricula,
                    p.DataAdmissao,
                    p.DataDemissao,
                    p.NomeFiliacao1,
                    p.NomeFiliacao2,
                    p.Telefone1,
                    p.Telefone2,
                    p.Telefone3,
                    p.Email,
                    p.EmailAlt,
                    p.CodCargo,
                    p.CodDepartamento,
                    p.Conf_CriaContaAD,
                    p.DataHora_CriaContaAD,
                    p.Conf_CadastraPonto,
                    p.DataHora_CadastraPonto,
                    p.Conf_Ativo,
                    p.PermiteAcesso,
                    p.Observacao,
                    c.Nome AS NomeCargo,
                    d.Nome AS NomeDepartamento
                FROM Pessoa p
                LEFT JOIN Cargo        c ON c.CodCargo        = p.CodCargo
                LEFT JOIN Departamento d ON d.CodDepartamento = p.CodDepartamento
                WHERE p.CodPessoa = @CodPessoa;
            ";

            var parametros = new Dictionary<string, object>
            {
                { "@CodPessoa", codPessoa }
            };

            var db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql, parametros);
        }
        public DataRow ObterPessoaBasico(int codPessoa)
        {
            string sql = @"
                SELECT TOP 1 CodPessoa, Nome, Sobrenome, EmailAlt
                FROM Pessoa
                WHERE CodPessoa = @CodPessoa;
            ";
            var pars = new Dictionary<string, object> { { "@CodPessoa", codPessoa } };
            var db = new Banco_Dados_SQLServer();
            var dt = db.LerPlennus(sql, pars);
            return (dt != null && dt.Rows.Count > 0) ? dt.Rows[0] : null;
        }

        public DataTable TipoEmpresa()
        {
            string query = @"SELECT CodEmpresa, NomeFantasia, RazaoSocial 
                     FROM Empresa 
                     WHERE Conf_Ativo = 1 
                     ORDER BY NomeFantasia";

          Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
          return db.LerPlennus(query);
        }

        public bool VincularUsuarioEmpresa(int codPessoa, int codEmpresa)
        {
            try
            {
                // Primeiro, verifica se existe autenticação para esta pessoa
                string sqlVerificaAutenticacao = @"
                    SELECT CodAutenticacaoAcesso 
                    FROM AutenticacaoAcesso 
                    WHERE CodPessoa = @CodPessoa 
                    AND Conf_PermiteAcesso = 1 
                    AND Conf_Ativo = 1";

                var parametrosVerifica = new Dictionary<string, object>
                {
                    { "@CodPessoa", codPessoa }
                };

                Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
                var resultado = db.LerPlennus(sqlVerificaAutenticacao, parametrosVerifica);

                if (resultado == null || resultado.Rows.Count == 0)
                {
                    return false;
                }

                int codAutenticacaoAcesso = Convert.ToInt32(resultado.Rows[0]["CodAutenticacaoAcesso"]);

                // **CORREÇÃO PRINCIPAL**: INSERT que evita duplicação
                string sqlInsert = @"
                    INSERT INTO SistemaEmpresaUsuario 
                    (CodSistemaEmpresa, CodPessoa, CodAutenticacaoAcesso, Conf_LiberaAcesso, Conf_BloqueiaAcesso, DataHoraLiberacao, Informacoes_Log_I)
                    SELECT 
                        se.CodSistemaEmpresa, 
                        @CodPessoa, 
                        @CodAutenticacaoAcesso,
                        1, 0, GETDATE(), GETDATE()
                    FROM SistemaEmpresa se
                    WHERE se.CodEmpresa = @CodEmpresa 
                    AND se.Conf_LiberaAcesso = 1
                    AND NOT EXISTS (
                        SELECT 1 
                        FROM SistemaEmpresaUsuario seu 
                        WHERE seu.CodSistemaEmpresa = se.CodSistemaEmpresa 
                        AND seu.CodPessoa = @CodPessoa
                    )";

                var parametrosInsert = new Dictionary<string, object>
                {
                    { "@CodPessoa", codPessoa },
                    { "@CodEmpresa", codEmpresa },
                    { "@CodAutenticacaoAcesso", codAutenticacaoAcesso }
                };

                // Use o novo método que retorna o número de linhas afetadas
                int registrosInseridos = db.ExecutarPlennusLinhasAfetadas(sqlInsert, parametrosInsert);

                // Se inseriu algum registro, retorna true
                return registrosInseridos > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao vincular usuário à empresa: {ex.Message}");
                return false;
            }
        }

        public int ObterEmpresaDoUsuario(int codPessoa)
        {
            string sql = @"
                SELECT TOP 1 se.CodEmpresa
                FROM SistemaEmpresaUsuario seu
                INNER JOIN SistemaEmpresa se ON seu.CodSistemaEmpresa = se.CodSistemaEmpresa
                WHERE seu.CodPessoa = @CodPessoa
                ORDER BY seu.CodSistemaEmpresaUsuario DESC";

            var parametros = new Dictionary<string, object>
            {
                { "@CodPessoa", codPessoa }
            };

            var db = new Banco_Dados_SQLServer();
            var dt = db.LerPlennus(sql, parametros);

            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["CodEmpresa"] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0]["CodEmpresa"]);
            }

            return 0;
        }

        public string ObterNomeEmpresa(int codEmpresa)
        {
            if (codEmpresa <= 0) return "";

            string sql = @"
                SELECT TOP 1 NomeEmpresa 
                FROM Empresa 
                WHERE CodEmpresa = @CodEmpresa";

            var parametros = new Dictionary<string, object>
            {
                { "@CodEmpresa", codEmpresa }
            };

            var db = new Banco_Dados_SQLServer();
            var dt = db.LerPlennus(sql, parametros);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["NomeEmpresa"]?.ToString() ?? "";
            }

            return "";
        }
    }
}
