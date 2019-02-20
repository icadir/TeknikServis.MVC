using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using TeknikServis.Entity.Anket;
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.IdentityModels;

namespace TeknikServis.DAL
{
    public class MyContext:IdentityDbContext<User>
    {
        public MyContext()
            : base("name=MyCon")
        {
            this.InstanceDate = DateTime.Now;
        }

        public virtual DbSet<ArızaKayıt> ArizaKayitlari { get; set; }
        public DateTime InstanceDate { get; set; }
    }
}
