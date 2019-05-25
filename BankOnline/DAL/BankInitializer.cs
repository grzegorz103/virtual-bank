using BankOnline.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankOnline.DAL
{
    public class BankInitializer : DropCreateDatabaseIfModelChanges<BankContext>
    {
        protected override void Seed(BankContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
           
            roleManager.Create(new IdentityRole("ADMIN"));
            roleManager.Create(new IdentityRole("USER"));

            context.SaveChanges();
            var addresses = new List<Address>
            {
                new Address{ City = "Warszawa", Street = "Warszawska", HouseNumber = 1, PostCode = "21-222"},
                new Address{ City = "Siedlce", Street = "Siedlecka", HouseNumber = 51, PostCode = "12-152"},
                new Address{ City = "Szczecin", Street = "Krakowska", HouseNumber = 21, PostCode = "53-523"},
            };
            addresses.ForEach(e => context.Addresses.Add(e));
            context.SaveChanges();


            var profiles = new List<Profile>
            {
                new Profile{Name = "Jan", Surname = "Kowalski", UserName="jan@kowalski.pl"},
                new Profile{Name = "Michal", Surname = "Michalski",   UserName="michal@michalski.pl"},
                new Profile{Name = "Anna", Surname = "Annowska", UserName="anna@annowska.pl"},
            };

            profiles.ForEach(e => context.Profiles.Add(e));
            context.SaveChanges();

            var bankAccounts = new List<BankAccount>
            {
                new BankAccount{Number = "2249000054516684537431475", Balance = 0, ProfileID = 1},
                new BankAccount{Number = "64249000057186527449208640", Balance =  0, ProfileID = 2},
                new BankAccount{Number = "63249000053283862487685011", Balance = 0, ProfileID = 3},
            };
            bankAccounts.ForEach(e => context.BankAccounts.Add(e));
            context.SaveChanges();


            var user = new ApplicationUser { UserName = "jan@kowalski.pl" };
            var user2 = new ApplicationUser { UserName = "michal@michalski.pl" };
            var user3 = new ApplicationUser { UserName = "anna@annowska.pl" };

            userManager.Create(user, "dssdSDk@422!");
            userManager.Create(user2, "Ksdc@)32");
            userManager.Create(user3, "sdS@!sc3");

            userManager.AddToRole(user.Id, "ADMIN");
            userManager.AddToRole(user2.Id, "ADMIN");
            userManager.AddToRole(user3.Id, "ADMIN");

            context.SaveChanges();

            var investmentTypes = new List<InvestmentType>
            {
                new InvestmentType{Name="Zwykla", Percentage=4}
            };

            investmentTypes.ForEach(e => context.InvestmentTypes.Add(e));
            context.SaveChanges();

            var credits = new List<Credit>
            {
                new Credit{ Balance = 10000, BalancePaid = 2000, CreditType = CreditType.AWAITING, InstallmentNums = 2, BankAccountID = 1,
                StatusDate = DateTime.Now},
                      new Credit{ Balance = 25000, BalancePaid = 3000, CreditType = CreditType.AWAITING, InstallmentNums = 3, BankAccountID = 1,
                StatusDate = DateTime.Now},
            };
            credits.ForEach(e => context.Credits.Add(e));
            context.SaveChanges();
            
        }
    }
}