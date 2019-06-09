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
using PagedList;

namespace BankOnline.Controllers
{
    public class BankAccountsController : Controller
    {
        private BankContext db = new BankContext();

        // GET: BankAccounts
        [Authorize(Roles = "ADMIN")]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NumberSortParm = String.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewBag.BalanceSortParam = sortOrder == "balance" ? "balance_desc" : "balance";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var bankAccounts = from s in db.BankAccounts select s;
            bankAccounts = bankAccounts.Include(e => e.Profile);
            if (!String.IsNullOrEmpty(searchString))
            {
                bankAccounts = bankAccounts.Where(s => s.Number.Contains(searchString)
                                       || s.Profile.UserName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "number_desc":
                    bankAccounts = bankAccounts.OrderByDescending(s => s.Number);
                    break;
                case "balance":
                    bankAccounts = bankAccounts.OrderBy(s => s.Balance);
                    break;
                case "balance_desc":
                    bankAccounts = bankAccounts.OrderByDescending(s => s.Balance);
                    break;
                default:
                    bankAccounts = bankAccounts.OrderBy(s => s.Number);
                    break;
            }


            int pageSize = 2;
            int pageNumber = (page ?? 1);
            return View(bankAccounts.ToPagedList(pageNumber, pageSize));
        }


        [Authorize(Roles = "USER")]
        public ActionResult MyList(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NumberSortParm = String.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewBag.BalanceSortParam = sortOrder == "balance" ? "balance_desc" : "balance";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var bankAccounts = from s in db.BankAccounts select s;
            bankAccounts = bankAccounts.Include(e => e.Profile);
            if (!String.IsNullOrEmpty(searchString))
            {
                bankAccounts = bankAccounts.Where(s => s.Number.Contains(searchString)
                                       || s.Profile.UserName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "number_desc":
                    bankAccounts = bankAccounts.OrderByDescending(s => s.Number);
                    break;
                case "balance":
                    bankAccounts = bankAccounts.OrderBy(s => s.Balance);
                    break;
                case "balance_desc":
                    bankAccounts = bankAccounts.OrderByDescending(s => s.Balance);
                    break;
                default:
                    bankAccounts = bankAccounts.OrderBy(s => s.Number);
                    break;
            }


            int pageSize = 2;
            int pageNumber = (page ?? 1);
            return View(bankAccounts.ToPagedList(pageNumber, pageSize));
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public ActionResult Create()
        {
            ViewBag.CreditCardID = new SelectList(db.CreditCards, "ID", "Image");
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName");
            return View();
        }

        // POST: BankAccounts/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Number,MyProperty,Balance,ProfileID,CreditCardID")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreditCardID = new SelectList(db.CreditCards, "ID", "Image", bankAccount.CreditCardID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", bankAccount.ProfileID);
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreditCardID = new SelectList(db.CreditCards, "ID", "Image", bankAccount.CreditCardID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", bankAccount.ProfileID);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Number,MyProperty,Balance,ProfileID,CreditCardID")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreditCardID = new SelectList(db.CreditCards, "ID", "Image", bankAccount.CreditCardID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", bankAccount.ProfileID);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if (bankAccount == null)
            {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankAccount);
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
