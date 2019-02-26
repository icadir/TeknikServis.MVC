using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using TeknikServis.BLL.Repository;
using TeknikServis.DAL;
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.IdentityModels;

namespace TeknikServis.BLL.Identity
{
    public class MembershipTools
    {
        private static MyContext _db;

        public static UserStore<User> NewUserStore() => new UserStore<User>(_db ?? new MyContext());
        public static UserManager<User> NewUserManager() => new UserManager<User>(NewUserStore());

        public static RoleStore<Role> NewRoleStore() => new RoleStore<Role>(_db ?? new MyContext());
        public static RoleManager<Role> NewRoleManager() => new RoleManager<Role>(NewRoleStore());


        public static string GetNameSurname(string userId)
        {
            User user;
            if (string.IsNullOrEmpty(userId))
            {
                var id = HttpContext.Current.User.Identity.GetUserId();
                if (string.IsNullOrEmpty(id))
                    return "";

                user = NewUserManager().FindById(id);
            }
            else
            {
                user = NewUserManager().FindById(userId);
                if (user == null)
                    return null;
            }

            return $"{user.Name} {user.Surname}";
        }

        public static string GetAvatarPath(string userId)
        {
            User user;
            if (string.IsNullOrEmpty(userId))
            {
                var id = HttpContext.Current.User.Identity.GetUserId();
                if (string.IsNullOrEmpty(id))
                    return "/Image/fitech1.png";

                user = NewUserManager().FindById(id);
            }
            else
            {
                user = NewUserManager().FindById(userId);
                if (user == null)
                    return "/Image/fitech1.png";
            }

            return $"{user.AvatarPath}";
        }

        public static User GetMusteri(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return null;
            }
            else
            {
                var MusteriUser = NewUserManager().FindById(UserId);
                return MusteriUser;
            }

        }

        public static string TeknisyenBilgiPuanıGetir(string TeknisyenId)
        {
            var ToplamPuan = 0.0;
            var teknisyen = NewUserManager().FindById(TeknisyenId);
            if (teknisyen == null)
                return "0";
            var arizalar = new ArizaKayitRepo().GetAll(x => x.TeknisyenId == TeknisyenId && x.AnketYapildimi == true)
                .ToList();
            if (arizalar.Count <= 0)
                return "Teknisyenin Puanı Yok";

            foreach (var ariza in arizalar)
            {

                ToplamPuan += (int)ariza.TeknisyenBilgiPuani;

            }

            ToplamPuan = ToplamPuan / arizalar.Count();
            return $"{ToplamPuan}";
        }
        public static string TeknisyenDavranisPuaniGetir(string TeknisyenId)
        {
            var ToplamPuan = 0.0;
            var teknisyen = NewUserManager().FindById(TeknisyenId);
            if (teknisyen == null)
                return "0";
            var arizalar = new ArizaKayitRepo().GetAll(x => x.TeknisyenId == TeknisyenId && x.AnketYapildimi == true)
                .ToList();
            if (arizalar.Count <= 0)
                return "Teknisyenin Puanı Yok";

            foreach (var ariza in arizalar)
            {

                ToplamPuan += (int)ariza.FitechDavranisPuani;

            }

            ToplamPuan = ToplamPuan / arizalar.Count();
            return $"{ToplamPuan}";
        }

        public static string FitechDavranisPuanOrtalamasi()
        {
            var arizalar = new ArizaKayitRepo().GetAll(x => x.AnketYapildimi == true).ToList();
            if (arizalar.Count == 0)
                return "Yapılan Anket Yok";
            var ToplamPuan = 0.0;
            foreach (var ariza in arizalar)
            {
                ToplamPuan += (int)ariza.FitechDavranisPuani;

            }

            ToplamPuan = ToplamPuan / arizalar.Count();
            return $"Fitech Davranış Ortalaması : {ToplamPuan}";
        }
        public static string FitechHizmetPuaniOrtalamasi()
        {
            var arizalar = new ArizaKayitRepo().GetAll(x => x.AnketYapildimi == true).ToList();
            if (arizalar.Count == 0)
                return "Yapılan Anket Yok";
            var ToplamPuan = 0.0;
            foreach (var ariza in arizalar)
            {
                ToplamPuan += (int)ariza.HizmetPuanı;

            }

            ToplamPuan = ToplamPuan / arizalar.Count();
            return $"Fitech Hizmet Puani Ortalaması : {ToplamPuan}";
        }

        public static string FitechTeknisyenBilgiPuanOrtalamasi()
        {
            var arizalar = new ArizaKayitRepo().GetAll(x => x.AnketYapildimi == true).ToList();
            if (arizalar.Count == 0)
                return "Yapılan Anket Yok";
            var ToplamPuan = 0.0;
            foreach (var ariza in arizalar)
            {
                ToplamPuan += (int)ariza.TeknisyenDavranisPuani;

            }

            ToplamPuan = ToplamPuan / arizalar.Count();
            return $"Teknisyenlerin Toplam Bilgi Puan Ortalaması : {ToplamPuan}";
        }
        public static string FitechTeknisyenDavranisPuanOrtalamasi()
        {
            var arizalar = new ArizaKayitRepo().GetAll(x => x.AnketYapildimi == true).ToList();
            if (arizalar.Count == 0)
                return "Yapılan Anket Yok";
            var ToplamPuan = 0.0;
            foreach (var ariza in arizalar)
            {
                ToplamPuan += (int)ariza.TeknisyenDavranisPuani;

            }

            ToplamPuan = ToplamPuan / arizalar.Count();
            return $"Teknisyenlerin Toplam Davranış Puanı Ortalaması : {ToplamPuan}";
        }

        public static string OrtalamaArizaCozumSuresi()
        {
            double ToplamGun = 0.0;
            var cozulenArizalar = new ArizaKayitRepo().GetAll(x => x.TeknisyenArizaDurum == TeknisyenArizaDurum.Çözüldü).ToList();
            foreach (var arizalar in cozulenArizalar)
            {
                TimeSpan Gun = arizalar.ArizaCozulduguTarih.Value -
                               arizalar.ArizaOlusturmaTarihi;
                ToplamGun += Gun.Seconds;
            }

            ToplamGun = ToplamGun / cozulenArizalar.Count();
            return $"{ToplamGun.ToString("##.000")}";

        }

    }
}
