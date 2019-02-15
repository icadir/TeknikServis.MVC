﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using TeknikServis.DAL;
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
                    return "/assets/img/avatars/avatar3.jpg";

                user = NewUserManager().FindById(id);
            }
            else
            {
                user = NewUserManager().FindById(userId);
                if (user == null)
                    return "/assets/img/avatars/avatar3.jpg";
            }

            return $"{user.AvatarPath}";
        }

        public static User GetMusteri(string UserId)
        {
            //if (string.IsNullOrEmpty(UserId))
            //    //return "/assets/img/avatars/avatar3.jpg";

          var MusteriUser = NewUserManager().FindById(UserId);

          return MusteriUser;
        }
    }
}
