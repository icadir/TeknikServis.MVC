using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using TeknikServis.Entity.Entitties;

namespace TeknikServis.Entity.IdentityModels
{
    public class User : IdentityUser
    {
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        [StringLength(60)]
        [Required]
        public string Surname { get; set; }

        public string ActivationCode { get; set; }
        public string AvatarPath { get; set; }
        public virtual ICollection<ArızaKayıt> ArizaKayitlari { get; set; } = new HashSet<ArızaKayıt>();
    }
}
