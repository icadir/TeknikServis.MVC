using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeknikServis.Web.UI.Abstracts;

namespace TeknikServis.Entity.Entitties
{
    [Table("Fotograflar")]
    public class Fotograf:BaseEntity<int>
    {
        [Required]
        public string Yol { get; set; }
        public int ArizaId { get; set; }
        [ForeignKey("ArizaId")]
        public virtual ArızaKayıt ArızaKayıt { get; set; }
    }
}
