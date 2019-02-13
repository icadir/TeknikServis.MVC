using Microsoft.AspNet.Identity.EntityFramework;
using System;
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

        public DateTime InstanceDate { get; set; }
    }
}
