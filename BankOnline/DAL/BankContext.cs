using BankOnline.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace BankOnline
{
    public class BankContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<InvestmentType> InvestmentTypes { get; set; }
        public DbSet<Investment> Investments { get; set; }

        public BankContext() : base("DefaultConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .HasOptional<BankAccount>(t=>t.From)
                .WithMany(b => b.TransactionFrom)
                .HasForeignKey(t => t.FromID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
            .HasOptional<BankAccount>(t => t.To)
            .WithMany(b => b.TransactionTo)
            .HasForeignKey(t => t.ToID)
            .WillCascadeOnDelete(false);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}