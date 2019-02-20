using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.Enums;
using TeknikServis.Web.UI.Abstracts;

namespace TeknikServis.Entity.Anket
{
    [Table("Anket")]
   public class Anket:BaseEntity<int>
    {
        
        [DisplayName("Teknisyenin Konu Hakkında Bilgisi Yeterli miydi ?")]
        public AnketEnum TeknisyenBilgiPuani { get; set; }
        [DisplayName("Teknisyenin Size Karşı Davranışı nasıldı ?")]
        public AnketEnum TeknisyenDavranisPuani { get; set; }
        [DisplayName("Çözüm Sürecinde Fitech Çalışanlarının iletişimi Nasıldı ?")]
        public AnketEnum FitechDavranisPuani { get; set; }
        [DisplayName("FiTechten Memnun Kaldınız mı ?")]
        public AnketEnum HizmetPuanı { get; set; }
        [DisplayName("FİTech Hakkındaki Görüşleriniz.")]
        public string FitechHakkindakiGorusler { get; set; }

        public virtual  ICollection<ArızaKayıt> ArızaKayıts { get; set; } = new HashSet<ArızaKayıt>();

    }
}
