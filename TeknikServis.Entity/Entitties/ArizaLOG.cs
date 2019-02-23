using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Web.UI.Abstracts;

namespace TeknikServis.Entity.Entitties
{
   public class ArizaLOG:BaseEntity<int>
    {
        public DateTime? ArizaOlusturulmaTarihi { get; set; }
        public DateTime? OperatorKabulTarihi { get; set; }
        public DateTime? TeknisyenAtandıgıTarih { get; set; }
        public DateTime? TeknisyenYolaCikti { get; set; }
        public string IslemAciklamalari { get; set; }

        public int ArızaId { get; set; }

        [ForeignKey("ArızaId")]
        public ArızaKayıt Ariza { get; set; }

    }
}
