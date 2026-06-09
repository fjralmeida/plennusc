using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.planiumApi
{
    public class PlanoService
    {
        private readonly string _cs;
        public PlanoService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        private SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        public List<PlanoListDto> ListarPlanos(PlanoFiltro filtro)
        {
            var lista = new List<PlanoListDto>();

            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;

                var sql = new StringBuilder(@"
                    SELECT
                       p.CodigoPlano,
                       p.CodigoProduto,      
                       p.RegistroANS,        
                       p.Num_CNPJ_Operadora, 
                       p.TipoContratacao,    
                       p.NomePlanoComercial, 
                       p.Segmentacao,        
                       p.Abrangencia,        
                       p.Coparticipacao,    
                       p.Acomodacao,         
                       p.DecSau,             
                       p.Promocional,        
                       p.Conf_Ativo,
                       o.NomeComercial
                    FROM dbo.API_Venda_Plano p
                    INNER JOIN dbo.API_Venda_Operadora o ON p.Num_CNPJ_Operadora = o.Numero_CNPJ
                    WHERE 1 = 1");

                if (!string.IsNullOrWhiteSpace(filtro?.NomePlanoComercial))
                {
                    sql.Append(" AND p.NomePlanoComercial LIKE @NomePlanoComercial");
                    cmd.Parameters.AddWithValue("@NomePlanoComercial", "%" + filtro.NomePlanoComercial.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.Segmentacao))
                {
                    sql.Append(" AND p.Segmentacao LIKE @Segmentacao");
                    cmd.Parameters.AddWithValue("@Segmentacao", "%" + filtro.Segmentacao.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.Abrangencia))
                {
                    sql.Append(" AND p.Abrangencia LIKE @Abrangencia");
                    cmd.Parameters.AddWithValue("@Abrangencia", "%" + filtro.Abrangencia.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.Coparticipacao))
                {
                    sql.Append(" AND p.Coparticipacao LIKE @Coparticipacao");
                    cmd.Parameters.AddWithValue("@Coparticipacao", "%" + filtro.Coparticipacao.Trim() + "%");
                }

                sql.Append(" ORDER BY p.CodigoPlano");
                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new PlanoListDto
                        {
                            CodigoPlano = rd.GetInt32(rd.GetOrdinal("CodigoPlano")),      
                            CodigoProduto = rd.GetInt32(rd.GetOrdinal("CodigoProduto")),

                            RegistroANS = rd.IsDBNull(rd.GetOrdinal("RegistroANS")) ? null : rd.GetString(rd.GetOrdinal("RegistroANS")),

                            NomeComercial = rd.IsDBNull(rd.GetOrdinal("NomeComercial")) ? null : rd.GetString(rd.GetOrdinal("NomeComercial")),
                            TipoContratacao = rd.IsDBNull(rd.GetOrdinal("TipoContratacao")) ? null : rd.GetString(rd.GetOrdinal("TipoContratacao")),
                            NomePlanoComercial = rd.IsDBNull(rd.GetOrdinal("NomePlanoComercial")) ? null : rd.GetString(rd.GetOrdinal("NomePlanoComercial")),
                            Segmentacao = rd.IsDBNull(rd.GetOrdinal("Segmentacao")) ? null : rd.GetString(rd.GetOrdinal("Segmentacao")),
                            Abrangencia = rd.IsDBNull(rd.GetOrdinal("Abrangencia")) ? null : rd.GetString(rd.GetOrdinal("Abrangencia")),
                            Coparticipacao = rd.IsDBNull(rd.GetOrdinal("Coparticipacao")) ? null : rd.GetString(rd.GetOrdinal("Coparticipacao")),
                            Acomodacao = rd.IsDBNull(rd.GetOrdinal("Acomodacao")) ? null : rd.GetString(rd.GetOrdinal("Acomodacao")),
                            DecSau = rd.IsDBNull(rd.GetOrdinal("DecSau")) ? null : rd.GetString(rd.GetOrdinal("DecSau")),
                            Promocional = rd.IsDBNull(rd.GetOrdinal("Promocional")) ? null : rd.GetString(rd.GetOrdinal("Promocional")),

                            Conf_Ativo = rd.GetBoolean(rd.GetOrdinal("Conf_Ativo")) 
                        });
                    }
                }
            }

            return lista;
        }
    }
}

