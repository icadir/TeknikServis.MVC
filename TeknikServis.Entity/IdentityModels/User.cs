using System.Collections.Generic;
using System.ComponentModel;
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

        [DisplayName("Enlem")]
        public double? Enlem { get; set; }
        [DisplayName("Boylam")]
        public double? Boylam { get; set; }
        public string ActivationCode { get; set; }
        public string AvatarPath { get; set; }
       
    }
}
