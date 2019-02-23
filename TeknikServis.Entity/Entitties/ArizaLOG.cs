using System.ComponentModel.DataAnnotations.Schema;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Web.UI.Abstracts;

namespace TeknikServis.Entity.Entitties
{
    public class ArizaLOG:BaseEntity<int>
    {
        public string Aciklama { get; set; }
        public IdentityRoles YapanınRolu { get; set; }
        public int ArızaId { get; set; }

        [ForeignKey("ArızaId")]
        public ArızaKayıt Ariza { get; set; }

    }
}
