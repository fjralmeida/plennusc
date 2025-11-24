using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.autenticacao
{
    public class PermissaoQuery
    {
        public DataTable ObterPermissoesUsuario(int codUsuario, int codSistema, int codEmpresa)
        {
            var banco = new Banco_Dados_SQLServer();
            string sql = @"
                SELECT 
                    -- Dados do Usuário
                    aa.CodAutenticacaoAcesso,
                    aa.NomeUsuario,
                    p.CodPessoa,
                    p.Nome,
                    p.CodDepartamento,
                    p.CodCargo,
                    
                    -- Dados do Sistema e Empresa
                    se.CodSistemaEmpresa,
                    s.CodSistema,
                    s.NomeDisplay as NomeSistema,
                    e.CodEmpresa, 
                    e.NomeFantasia,
                    
                    -- Permissões de Sistema
                    seu.Conf_LiberaAcesso as LiberacaoUsuarioSistema,
                    seu.Conf_BloqueiaAcesso as BloqueioUsuarioSistema,
                    se.Conf_LiberaAcesso as LiberacaoVinculoSistemaEmpresa,
                    
                    -- Status
                    aa.Conf_Ativo as UsuarioAtivo,
                    e.Conf_Ativo as EmpresaAtiva,
                    s.Conf_LiberaUtilizacao as SistemaAtivo
                    
                FROM AutenticacaoAcesso aa
                INNER JOIN Pessoa p ON p.CodPessoa = aa.CodPessoa
                INNER JOIN SistemaEmpresaUsuario seu ON seu.CodAutenticacaoAcesso = aa.CodAutenticacaoAcesso
                INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = seu.CodSistemaEmpresa
                INNER JOIN Sistema s ON s.CodSistema = se.CodSistema
                INNER JOIN Empresa e ON e.CodEmpresa = se.CodEmpresa
                
                WHERE aa.CodAutenticacaoAcesso = @CodUsuario
                AND s.CodSistema = @CodSistema
                AND e.CodEmpresa = @CodEmpresa
                AND aa.Conf_Ativo = 1
                AND p.Conf_Ativo = 1
                AND e.Conf_Ativo = 1
                AND s.Conf_LiberaUtilizacao = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa }
            };

            return banco.LerPlennus(sql, parametros);
        }

        public DataTable ObterMenusPermitidos(int codUsuario, int codSistema, int codEmpresa)
        {
            var banco = new Banco_Dados_SQLServer();
            string sql = @"
                SELECT DISTINCT 
                    m.CodMenu,
                    m.NomeMenu,
                    m.NomeDisplay,
                    m.NomeObjeto,
                    m.CaptionObjeto,
                    m.HttpRefer,
                    m.HttpRouter,
                    m.CodMenuPai,
                    m.Conf_Ordem,
                    m.Conf_Nivel,
                    m.Conf_Habilitado
                    
                FROM SistemaEmpresaMenuUsuario semu
                INNER JOIN SistemaEmpresaMenu sem ON sem.CodSistemaEmpresaMenu = semu.CodSistemaEmpresaMenu
                INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = sem.CodSistemaEmpresa
                INNER JOIN SistemaEmpresaUsuario seu ON seu.CodSistemaEmpresaUsuario = semu.CodSistemaEmpresaUsuario
                INNER JOIN Menu m ON m.CodMenu = sem.CodMenu
                
                WHERE seu.CodAutenticacaoAcesso = @CodUsuario
                AND se.CodSistema = @CodSistema
                AND se.CodEmpresa = @CodEmpresa
                AND semu.Conf_LiberaAcesso = 1
                AND seu.Conf_LiberaAcesso = 1
                AND sem.Conf_Habilitado = 1
                AND sem.Conf_Exibir = 1
                AND m.Conf_Habilitado = 1
                
                ORDER BY m.Conf_Ordem";

            var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa }
            };

            return banco.LerPlennus(sql, parametros);
        }

        public bool VerificarAcessoPagina(int codUsuario, int codSistema, int codEmpresa, string nomeObjetoPagina)
        {
            var banco = new Banco_Dados_SQLServer();
            string sql = @"
                SELECT COUNT(*) 
                FROM SistemaEmpresaMenuUsuario semu
                INNER JOIN SistemaEmpresaMenu sem ON sem.CodSistemaEmpresaMenu = semu.CodSistemaEmpresaMenu
                INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = sem.CodSistemaEmpresa
                INNER JOIN SistemaEmpresaUsuario seu ON seu.CodSistemaEmpresaUsuario = semu.CodSistemaEmpresaUsuario
                INNER JOIN Menu m ON m.CodMenu = sem.CodMenu
                
                WHERE seu.CodAutenticacaoAcesso = @CodUsuario
                AND se.CodSistema = @CodSistema
                AND se.CodEmpresa = @CodEmpresa
                AND m.NomeObjeto = @NomeObjeto
                AND semu.Conf_LiberaAcesso = 1
                AND seu.Conf_LiberaAcesso = 1
                AND sem.Conf_Habilitado = 1
                AND sem.Conf_Exibir = 1
                AND m.Conf_Habilitado = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa },
                { "@NomeObjeto", nomeObjetoPagina }
            };

            object resultado = banco.ExecutarPlennusScalar(sql, parametros);
            return Convert.ToInt32(resultado) > 0;
        }

        public DataTable ObterPermissoesMenu(int codUsuario, int codSistema, int codEmpresa)
        {
            var banco = new Banco_Dados_SQLServer();
            string sql = @"
                SELECT DISTINCT sem.CodMenu
                FROM SistemaEmpresaMenuUsuario semu
                INNER JOIN SistemaEmpresaMenu sem ON sem.CodSistemaEmpresaMenu = semu.CodSistemaEmpresaMenu
                INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = sem.CodSistemaEmpresa
                INNER JOIN SistemaEmpresaUsuario seu ON seu.CodSistemaEmpresaUsuario = semu.CodSistemaEmpresaUsuario
                WHERE seu.CodAutenticacaoAcesso = @CodUsuario
                AND se.CodSistema = @CodSistema
                AND se.CodEmpresa = @CodEmpresa
                AND semu.Conf_LiberaAcesso = 1
                AND seu.Conf_LiberaAcesso = 1
                AND sem.Conf_Habilitado = 1
                AND sem.Conf_Exibir = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa }
            };

            return banco.LerPlennus(sql, parametros);
        }

        public bool VerificarAcessoMenu(int codUsuario, int codSistema, int codEmpresa, int codMenu)
        {
            var banco = new Banco_Dados_SQLServer();
            string sql = @"
                SELECT COUNT(*) 
                FROM SistemaEmpresaMenuUsuario semu
                INNER JOIN SistemaEmpresaMenu sem ON sem.CodSistemaEmpresaMenu = semu.CodSistemaEmpresaMenu
                INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = sem.CodSistemaEmpresa
                INNER JOIN SistemaEmpresaUsuario seu ON seu.CodSistemaEmpresaUsuario = semu.CodSistemaEmpresaUsuario
                WHERE seu.CodAutenticacaoAcesso = @CodUsuario
                AND se.CodSistema = @CodSistema
                AND se.CodEmpresa = @CodEmpresa
                AND sem.CodMenu = @CodMenu
                AND semu.Conf_LiberaAcesso = 1
                AND seu.Conf_LiberaAcesso = 1
                AND sem.Conf_Habilitado = 1
                AND sem.Conf_Exibir = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa },
                { "@CodMenu", codMenu }
            };

            object resultado = banco.ExecutarPlennusScalar(sql, parametros);
            return Convert.ToInt32(resultado) > 0;
        }

        public bool VerificarAcessoSistemaEmpresa(int codUsuario, int codSistema, int codEmpresa)
        {
            var banco = new Banco_Dados_SQLServer();
            string sql = @"
                SELECT COUNT(*) 
                FROM SistemaEmpresaUsuario seu
                INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = seu.CodSistemaEmpresa
                WHERE seu.CodAutenticacaoAcesso = @CodUsuario
                AND se.CodSistema = @CodSistema
                AND se.CodEmpresa = @CodEmpresa
                AND seu.Conf_LiberaAcesso = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa }
            };

            object resultado = banco.ExecutarPlennusScalar(sql, parametros);
            return Convert.ToInt32(resultado) > 0;
        }

        public DataTable ObterEstruturaMenuCompleta(int codUsuario, int codSistema, int codEmpresa)
        {
            var banco = new Banco_Dados_SQLServer();
            string sql = @"
                SELECT 
                    m.CodMenu,
                    m.NomeMenu,
                    m.NomeDisplay,
                    m.NomeObjeto,
                    m.CaptionObjeto,
                    m.HttpRefer,
                    m.HttpRouter,
                    m.CodMenuPai,
                    m.Conf_Ordem,
                    m.Conf_Nivel,
                    m.Conf_Habilitado,
                    -- Verifica se usuário tem acesso a este menu específico
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 
                            FROM SistemaEmpresaMenu sem
                            INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = sem.CodSistemaEmpresa
                            INNER JOIN SistemaEmpresaMenuUsuario semu ON semu.CodSistemaEmpresaMenu = sem.CodSistemaEmpresaMenu
                            INNER JOIN SistemaEmpresaUsuario seu ON seu.CodSistemaEmpresaUsuario = semu.CodSistemaEmpresaUsuario
                            WHERE sem.CodMenu = m.CodMenu
                            AND se.CodSistema = @CodSistema
                            AND se.CodEmpresa = @CodEmpresa
                            AND seu.CodAutenticacaoAcesso = @CodUsuario
                            AND semu.Conf_LiberaAcesso = 1
                            AND seu.Conf_LiberaAcesso = 1
                            AND sem.Conf_Habilitado = 1
                            AND sem.Conf_Exibir = 1
                        ) THEN 1 
                        ELSE 0 
                    END as TemAcesso
                FROM Menu m
                WHERE m.Conf_Habilitado = 1
                AND EXISTS (
                    SELECT 1 
                    FROM SistemaEmpresaMenu sem
                    INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = sem.CodSistemaEmpresa
                    WHERE sem.CodMenu = m.CodMenu
                    AND se.CodSistema = @CodSistema
                    AND se.CodEmpresa = @CodEmpresa
                    AND sem.Conf_Habilitado = 1
                    AND sem.Conf_Exibir = 1
                )
                ORDER BY m.Conf_Ordem, m.CodMenuPai";

            var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa }
            };

            return banco.LerPlennus(sql, parametros);
        }

        public Dictionary<string, bool> ObterPermissoesPaginas(int codUsuario, int codSistema, int codEmpresa)
        {
            var permissoes = new Dictionary<string, bool>();
            var banco = new Banco_Dados_SQLServer();

            // ✅ QUERY CORRIGIDA - MAIS FLEXÍVEL
            string sql = @"
                SELECT DISTINCT
                    COALESCE(m.NomeObjeto, m.NomeMenu) as NomePagina
                FROM SistemaEmpresaMenu sem
                INNER JOIN SistemaEmpresa se ON se.CodSistemaEmpresa = sem.CodSistemaEmpresa
                INNER JOIN SistemaEmpresaMenuUsuario semu ON semu.CodSistemaEmpresaMenu = sem.CodSistemaEmpresaMenu
                INNER JOIN SistemaEmpresaUsuario seu ON seu.CodSistemaEmpresaUsuario = semu.CodSistemaEmpresaUsuario
                INNER JOIN Menu m ON m.CodMenu = sem.CodMenu
                WHERE seu.CodAutenticacaoAcesso = @CodUsuario
                AND se.CodSistema = @CodSistema
                AND se.CodEmpresa = @CodEmpresa
                AND (m.NomeObjeto IS NOT NULL OR m.NomeMenu IS NOT NULL)
                AND (m.NomeObjeto != '' OR m.NomeMenu != '')
                AND semu.Conf_LiberaAcesso = 1
                AND seu.Conf_LiberaAcesso = 1
                AND sem.Conf_Habilitado = 1
                AND sem.Conf_Exibir = 1
                AND m.Conf_Habilitado = 1";

                    var parametros = new Dictionary<string, object>
            {
                { "@CodUsuario", codUsuario },
                { "@CodSistema", codSistema },
                { "@CodEmpresa", codEmpresa }
            };

            DataTable dt = banco.LerPlennus(sql, parametros);

            foreach (DataRow row in dt.Rows)
            {
                string nomePagina = row["NomePagina"].ToString();
                permissoes[nomePagina] = true;
            }

            return permissoes;
        }
    }
}