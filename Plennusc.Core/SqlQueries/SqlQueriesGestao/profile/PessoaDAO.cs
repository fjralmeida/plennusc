using appWhatsapp.Data_Bd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
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
            string query = "SELECT CodEstrutura, DescEstrutura FROM VW_PERFIL_PESSOA ORDER BY DescEstrutura";

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

        public void InsertPersonSystem(
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
                ) VALUES (
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
            db.ExecutarPlennus(sql, parametros);
        }
        public DataTable BuscarUsuarioPorNome(string nome)
        {
            string query = $@"
                            SELECT 
                                CodPessoa,
                                (Nome + ' ' + Sobrenome) AS NomeCompleto,
                                DocRG AS RG,
                                DocCPF AS CPF,
                                Telefone1,
                                Email,
                                Conf_Ativo,
                                CodCargo AS Cargo
                            FROM Pessoa
                            WHERE Nome LIKE '%{nome}%' OR Sobrenome LIKE '%{nome}%'
                            ORDER BY Nome
                        ";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(query);
        }
        public DataTable BuscarUsuarioPorCPF(string cpf)
        {
            string query = $@"
                            SELECT 
                                CodPessoa,
                                (Nome + ' ' + Sobrenome) AS NomeCompleto,
                                DocRG AS RG,
                                DocCPF AS CPF,
                                Email,
                                Telefone1,
                                Conf_Ativo,
                                CodCargo AS Cargo
                            FROM Pessoa
                            WHERE DocCPF = '{cpf}'
                        ";

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
            string sql = "UPDATE Pessoa SET Conf_Ativo = 0, Observacao = @Motivo WHERE CodPessoa = @CodPessoa";

            Dictionary<string, object> parametros = new Dictionary<string, object>
            {
                { "@Motivo", motivo },
                { "@CodPessoa", codPessoa }
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
    }
}
