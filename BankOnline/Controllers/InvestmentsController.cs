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
    public class InvestmentsController : Controller
    {
        private BankContext db = new BankContext();

        // GET: Investments
        [Authorize]
        public ActionResult Index()
        {
            IQueryable<Investment> investments;
            if (User.IsInRole("ADMIN"))
                investments = db.Investments.Include(i => i.BankAccount).Include(i => i.InvestmentType);
            else
                investments = db.Investments.Include(i => i.BankAccount).Include(i => i.InvestmentType).Where(e => e.BankAccount.Profile.UserName == User.Identity.Name);

            investments.ToList().ForEach(e =>
            {
                DateTime date = DateTime.Now;
                TimeSpan span = date - e.VisitDate;
                float timeElapsed = Convert.ToSingle(span.TotalSeconds);
                e.Balance = (float)Math.Round(e.Balance + ((((e.InvestmentType.Percentage * e.BaseBalance) / 100) * (timeElapsed) / 10)), 2);
                e.VisitDate = date;
            });
            db.SaveChanges();
            return View(investments.ToList());
        }


        // GET: Investments/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Investment investment = db.Investments.Find(id);
            if (investment == null)
            {
                return HttpNotFound();
            }
            return View(investment);
        }

        // GET: Investments/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.BankAccountID = new SelectList(db.BankAccounts.Where(e => e.Profile.UserName == User.Identity.Name).ToList(), "ID", "Number");
            ViewBag.InvestmentTypeID = new SelectList(db.InvestmentTypes, "ID", "Name");
            return View();
        }

        // POST: Investments/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,BankAccountID,VisitDate,Balance,InvestmentTypeID")] Investment investment)
        {
            if (ModelState.IsValid)
            {
                BankAccount from = db.BankAccounts.Single(e => e.ID == investment.BankAccountID);
                if (from.Balance >= investment.Balance)
                {
                    investment.BaseBalance = investment.Balance;
                    from.Balance -= investment.Balance;
                    investment.VisitDate = DateTime.Now;
                    db.Investments.Add(investment);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BankAccountID = new SelectList(db.BankAccounts, "ID", "Number", investment.BankAccountID);
            ViewBag.InvestmentTypeID = new SelectList(db.InvestmentTypes, "ID", "Name", investment.InvestmentTypeID);
            return View(investment);
        }

        // GET: Investments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Investment investment = db.Investments.Find(id);
            if (investment == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankAccountID = new SelectList(db.BankAccounts, "ID", "Number", investment.BankAccountID);
            ViewBag.InvestmentTypeID = new SelectList(db.InvestmentTypes, "ID", "Name", investment.InvestmentTypeID);
            return View(investment);
        }

        // POST: Investments/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BankAccountID,VisitDate,Balance,InvestmentTypeID")] Investment investment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(investment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankAccountID = new SelectList(db.BankAccounts, "ID", "Number", investment.BankAccountID);
            ViewBag.InvestmentTypeID = new SelectList(db.InvestmentTypes, "ID", "Name", investment.InvestmentTypeID);
            return View(investment);
        }

        // GET: Investments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Investment investment = db.Investments.Find(id);
            if (investment == null)
            {
                return HttpNotFound();
            }
            return View(investment);
        }

        // POST: Investments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Investment investment = db.Investments.Find(id);
            BankAccount bankAccount = db.BankAccounts.Single(e => e.ID == investment.BankAccountID);
            bankAccount.Balance += investment.Balance;
            db.Investments.Remove(investment);
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
