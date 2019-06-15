using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BankOnline;
using BankOnline.Models;

namespace BankOnline.Controllers
{
    public class CreditsController : Controller
    {
        private BankContext db = new BankContext();

        // GET: Credits
        [Authorize(Roles = "ADMIN")]
        public ActionResult Index()
        {
            var credits = db.Credits.Include(c => c.BankAccount);
            return View(credits.ToList());
        }

        // GET: Credits/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Credit credit = db.Credits.Find(id);
            if (credit == null)
            {
                return HttpNotFound();
            }
            return View(credit);
        }

        [Authorize]
        public ActionResult My()
        {
            var credits = db.Credits.Include(e => e.BankAccount).Where(e => e.BankAccount.Profile.UserName == User.Identity.Name);
            return View(credits);
        }

        [Authorize]
        public ActionResult Status(int? id, string status)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Credit credit = db.Credits.Find(id);
            if (credit != null)
            {
                if (credit.CreditType == CreditType.AWAITING)
                {
                    if (status == "acc")
                    {
                        credit.BankAccount.Balance += credit.Balance;
                        credit.CreditType = CreditType.ACCEPTED;
                    }
                    else if (status == "rej")
                    {
                        credit.CreditType = CreditType.REJECTED;
                    }
                }
                credit.StatusDate = DateTime.Now;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Pay(int creditID, float payin)
        {
            if (payin > 0)
            {
                Credit credit = db.Credits.Single(e => e.ID == creditID);
                if (credit != null)
                {
                    if (credit.BalancePaid + payin > credit.Balance)
                    {
                        payin = credit.Balance - credit.BalancePaid;
                    }

                    BankAccount bankAccount = credit.BankAccount;
                    if(bankAccount.Balance >= payin)
                    {
                        bankAccount.Balance -= payin;
                        credit.BalancePaid += payin;
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("My");

        }

        // GET: Credits/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.BankAccountID = new SelectList(db.BankAccounts.Where(e => e.Profile.UserName == User.Identity.Name).ToList(), "ID", "Number");
            return View();
        }

        // POST: Credits/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ID,ProfileID,BankAccountID,Balance,BalancePaid,StatusDate,CreditType")] Credit credit)
        {
            if (ModelState.IsValid)
            {
                credit.Balance += credit.Balance * 0.1f;
                credit.BankAccount = db.BankAccounts.Find(credit.BankAccountID);
                credit.CreditType = CreditType.AWAITING;
                credit.BalancePaid = 0;
                credit.StatusDate = DateTime.Now;
                db.Credits.Add(credit);
                db.SaveChanges();
                return RedirectToAction("My");
            }

            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", credit.BankAccount);
            return View(credit);
        }

        // GET: Credits/Edit/5
        [Authorize(Roles = "ADMIN")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Credit credit = db.Credits.Find(id);
            if (credit == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankAccountID = new SelectList(db.BankAccounts.Where(e => e.Profile.UserName == User.Identity.Name).ToList(), "ID", "Number");

            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", credit.BankAccount);
            return View(credit);
        }

        // POST: Credits/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public ActionResult Edit([Bind(Include = "ID,ProfileID,Balance,BalancePaid,StatusDate,CreditType")] Credit credit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(credit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", credit.BankAccount);
            return View(credit);
        }

        [Authorize]
        public ActionResult UserCredits()
        {
            Profile profile = db.Profiles.Single(p => p.UserName == User.Identity.Name);
            ICollection<BankAccount> accounts = profile.BankAccounts;
            var credits = accounts.SelectMany(e => e.Credits);
            return View(credits);
        }

        // GET: Credits/Delete/5
        [Authorize(Roles = "ADMIN")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Credit credit = db.Credits.Find(id);
            if (credit == null)
            {
                return HttpNotFound();
            }
            return View(credit);
        }

        // POST: Credits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public ActionResult DeleteConfirmed(int id)
        {
            Credit credit = db.Credits.Find(id);
            db.Credits.Remove(credit);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
