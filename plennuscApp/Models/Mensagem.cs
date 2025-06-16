using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace appWhatsapp.Models
{
    public class Mensagem
    {
        public string Numero { get; set; }
        public string Texto { get; set; }
        public string HsmId { get; set; }
        public string CampaignId { get; set; }
    }
}