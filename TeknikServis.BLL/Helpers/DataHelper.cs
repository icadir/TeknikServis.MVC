using System.Collections.Generic;
using System.Linq;
using TeknikServis.BLL.Identity;
using TeknikServis.Entity.IdentityModels;

namespace TeknikServis.BLL.Helpers
{
    public class DataHelper
    {
        public static async void DataEkle()
        {
            var usermanager = MembershipTools.NewUserManager();
            var userstore = MembershipTools.NewUserStore();

            var Users = new List<User>();

            for (int i = 0; i < 4; i++)
            {
                var admin = new User()
                {
                    Email = "mvcservisproje123@gmail.com",
                    UserName = $"admin{i + 1}",
                    Name = $"admin{i + 1}",
                    Surname = $"adminx{i + 1}",

                };
                Users.Add(admin);
                var musteri = new User()
                {
                    Email = "mvcservisproje123@gmail.com",
                    UserName = $"musteri{i + 1}",
                    Name = $"musteri{i + 1}",
                    Surname = $"musterix{i + 1}",

                };
                Users.Add(musteri);
                var teknisyen = new User()
                {
                    Email = "mvcservisproje123@gmail.com",
                    UserName = $"teknisyen{i + 1}",
                    Name = $"teknisyen{i + 1}",
                    Surname = $"teknisyenx{i + 1}",

                };
                Users.Add(teknisyen);
                var operatorx = new User()
                {
                    Email = "mvcservisproje123@gmail.com",
                    UserName = $"operatorx{i + 1}",
                    Name = $"operatorx{i + 1}",
                    Surname = $"operatorx{i + 1}",

                };
                Users.Add(operatorx);
            }

            foreach (var user in Users)
            {
                var newPassword = "123123";
                var result = await usermanager.CreateAsync(user, newPassword);
                user.AvatarPath = "/Image/Furki.JPG";
                user.EmailConfirmed = true;
                user.PhoneNumber = "0 587 55 88";
                user.PhoneNumberConfirmed = true;
                if (result.Succeeded)
                {
                    switch (userstore.Users.Count())
                    {

                        case 1:
                        case 5:
                        case 9:
                            await usermanager.AddToRoleAsync(user.Id, "Admin");
                            break;
                        case 2:
                        case 6:
                        case 10:
                            await usermanager.AddToRoleAsync(user.Id, "Operator");
                            break;
                        case 3:
                        case 7:
                        case 11:
                            await usermanager.AddToRoleAsync(user.Id, "Teknisyen");
                            break;
                        default:
                            await usermanager.AddToRoleAsync(user.Id, "Musteri");
                            break;
                    }
                }
            }
        }
    }
}
