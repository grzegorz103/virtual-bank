using BankOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            IQueryable<InvestmentGroup> data = from investment in db.Investments
                                               group investment by investment.InvestmentType into invGroup
                                               select new InvestmentGroup()
                                               {
                                                   Type = invGroup.Key.Name,
                                                   Balance = invGroup.Sum(e => e.Balance)
                                               };
            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}