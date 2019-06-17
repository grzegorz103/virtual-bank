using BankOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;

namespace BankOnline.Controllers
{
    public class HomeController : Controller
    {
        BankContext db = new BankContext();

        public ActionResult Index()
        {
            if (User.Identity.Name != "")
            {
                Profile profile = db.Profiles.Single(e => e.UserName == User.Identity.Name);
                return View(profile.BankAccounts);
            }
            return View();
        }

        public ActionResult About()
        {
            IQueryable<Investment> investments = db.Investments
             .Include(i => i.BankAccount)
             .Include(i => i.InvestmentType);
            investments.ToList().ForEach(e =>
            {
                DateTime date = DateTime.Now;
                TimeSpan span = date - e.VisitDate;
                float timeElapsed = Convert.ToSingle(span.TotalSeconds);
                e.Balance = (float)Math.Round(e.Balance + ((((e.InvestmentType.Percentage * e.BaseBalance) / 100) * (timeElapsed) / 10)), 2);
                e.VisitDate = date;
            });
            db.SaveChanges();

            IQueryable<InvestmentGroup> data = from investment in db.Investments
                                               group investment by investment.InvestmentType into invGroup
                                               select new InvestmentGroup()
                                               {
                                                   Type = invGroup.Key.Name,
                                                   Balance = invGroup.Sum(e => e.Balance)
                                               };
            return View(data.ToList());
        }

        public ActionResult Summary()
        {
            ViewBag.BankAccounts = db.BankAccounts.Where(e => e.Profile.UserName == User.Identity.Name).ToList() ;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}