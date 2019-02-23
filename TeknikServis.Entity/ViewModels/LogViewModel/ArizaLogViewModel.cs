using System;
using TeknikServis.Entity.IdentityModels;

namespace TeknikServis.Entity.ViewModels.LogViewModel
{
    public class ArizaLogViewModel
    {
        public int ArızaId { get; set; }
        public string Aciklama { get; set; }
        public IdentityRoles YapanınRolu { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
