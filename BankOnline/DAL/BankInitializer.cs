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
                new Profile{Name = "Jan", Surname = "Kowalski", UserName="493385326"},
                new Profile{Name = "Michal", Surname = "Michalski",   UserName="243265254"},
                new Profile{Name = "Anna", Surname = "Annowska", UserName="678634125"},
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

         
            var user = new ApplicationUser { UserName = "493385326" };
            var user2 = new ApplicationUser { UserName = "243265254" };
            var user3 = new ApplicationUser { UserName = "678634125" };
            userManager.Create(user, "dssdSDk@422!");
            userManager.Create(user2, "Ksdc@)32");
            userManager.Create(user3, "sdS@!sc3");

            userManager.AddToRole(user.Id, "ADMIN");
            userManager.AddToRole(user2.Id, "ADMIN");
            userManager.AddToRole(user3.Id, "ADMIN");


            context.SaveChanges();
        }
    }
}