﻿using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class TransactionsController : Controller
    {
        private BankContext db = new BankContext();

        // GET: Transactions
        [Authorize]
        public ActionResult Index(string sortOrder, string currentFilter, DateTime? DateFrom, DateTime? DateTo, float? BalanceFrom, float? BalanceTo, string Incoming, int? page,
         DateTime? DateFrom2, DateTime? DateTo2, float? BalanceFrom2, float? BalanceTo2, string Incoming2)
        {
            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;
            ViewBag.BalanceFrom = BalanceFrom;
            ViewBag.BalanceTo = BalanceTo;
            ViewBag.Incoming = Incoming;
            ViewBag.DateSort = sortOrder == "date" ? "date" : "date_desc";
            IQueryable<Transaction> transactions;
            if (User.IsInRole("ADMIN"))
                transactions = db.Transactions.Include(t => t.From).Include(t => t.To);
            else
                transactions = db.Transactions.Include(t => t.From).Include(t => t.To)
            .Where(e => e.To.Profile.UserName == User.Identity.Name || e.From.Profile.UserName == User.Identity.Name);

            if (currentFilter != null || DateFrom != null || DateTo != null || BalanceFrom != null || BalanceTo != null || Incoming != null)
            {
                page = 1;
            }
            else
            {
                DateFrom = DateFrom2;
                DateTo = DateTo2;
                BalanceFrom = BalanceFrom2;
                BalanceTo = BalanceTo2;
                Incoming = Incoming2;
            }

            if (!String.IsNullOrEmpty(Incoming) && Incoming != "all")
            {
                if (Incoming == "incoming")
                {
                    transactions = transactions.Where(e => e.To.Profile.UserName == User.Identity.Name);
                }
                else if (Incoming == "outcoming")
                {
                    transactions = transactions.Where(e => e.From.Profile.UserName == User.Identity.Name);
                }
            }

            if (BalanceFrom != null)
                transactions = transactions.Where(e => e.Amount >= BalanceFrom);

            if (BalanceTo != null)
                transactions = transactions.Where(e => e.Amount <= BalanceTo);

            if (DateFrom != null)
                transactions = transactions.Where(e => e.TransactionDate >= DateFrom);

            if (DateTo != null)
                transactions = transactions.Where(e => e.TransactionDate <= DateTo);

            switch (sortOrder)
            {
                case "date":
                    transactions = transactions.OrderByDescending(s => s.TransactionDate);
                    break;
                case "date_desc":
                    transactions = transactions.OrderBy(s => s.TransactionDate);
                    break;
                default:
                    transactions = transactions.OrderByDescending(s => s.TransactionDate);
                    break;
            }
            
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(transactions.ToPagedList(pageNumber, pageSize));
        }


        // GET: Transactions/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.History = db.BankAccounts.Where(e => e.Profile.UserName == User.Identity.Name).Select(e => e.TransactionFrom);
            ViewBag.FromID = new SelectList(db.BankAccounts.Where(e => e.Profile.UserName == User.Identity.Name), "ID", "Number");
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number");
            return View();
        }


        // POST: Transactions/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TransactionDate,FromID,ToID,Amount,Defined,Title")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.TransactionDate = DateTime.Now;
                BankAccount fr = db.BankAccounts.Single(e => e.ID == transaction.FromID);
                BankAccount target = db.BankAccounts.Single(e => e.ID == transaction.ToID);
                if (fr.Balance < transaction.Amount)
                    return RedirectToAction("Index");
                target.Balance += transaction.Amount;
                fr.Balance -= transaction.Amount;

                if (transaction.Defined)
                {
                    db.DefinedTransfers.Add(new DefinedTransfer
                    {
                        From = fr,
                        To = target,
                        Amount = transaction.Amount,
                        Title = transaction.Title,
                        Profile = db.Profiles.Single(e => e.UserName == User.Identity.Name)
                    });
                }

                db.Transactions.Add(transaction);
                db.SaveChanges();
                Profile current = db.Profiles.Single(e => e.UserName == User.Identity.Name);
                if (current.EnableMail)
                    SendMail(target.Profile.UserName, "You received transfer", "You have new transaction. Visit us to check. Virtual bank");
                return RedirectToAction("Index");
            }

            ViewBag.FromID = new SelectList(db.BankAccounts, "ID", "Number", transaction.FromID);
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number", transaction.ToID);
            return View(transaction);
        }

        public ActionResult DeleteDefined(int? id)
        {
            db.Transactions.Find(id).Defined = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Transactions/Defined/5
        public ActionResult Defined(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            Transaction newTrans = new Transaction();
            newTrans.From = transaction.From;
            newTrans.To = transaction.To;
            newTrans.Title = transaction.Title;

            newTrans.Defined = false;

            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.FromID = new SelectList(db.BankAccounts.Where(e => e.Profile.UserName == User.Identity.Name), "ID", "Number", transaction.FromID);
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number", transaction.ToID);
            return View(newTrans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Defined([Bind(Include = "ID,TransactionDate,FromID,ToID,Amount,Defined,Title")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.Defined = false;
                transaction.TransactionDate = DateTime.Now;
                BankAccount fr = db.BankAccounts.Single(e => e.ID == transaction.FromID);
                BankAccount target = db.BankAccounts.Single(e => e.ID == transaction.ToID);
                if (fr.Balance < transaction.Amount)
                    return RedirectToAction("Index");
                target.Balance += transaction.Amount;
                fr.Balance -= transaction.Amount;
                db.Transactions.Add(transaction);
                db.SaveChanges();
                Profile current = db.Profiles.Single(e => e.UserName == User.Identity.Name);
                if (current.EnableMail)
                    SendMail(target.Profile.UserName, "You received transfer", "You have new transaction. Visit us to check. Virtual bank");

                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FromID = new SelectList(db.BankAccounts, "ID", "Number", transaction.FromID);
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number", transaction.ToID);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.FromID = new SelectList(db.BankAccounts, "ID", "Number", transaction.FromID);
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number", transaction.ToID);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TransactionDate,FromID,ToID,Amount")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FromID = new SelectList(db.BankAccounts, "ID", "Number", transaction.FromID);
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number", transaction.ToID);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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

        public static void SendMail(string to, string subject, string body)
        {
            var message = new System.Net.Mail.MailMessage(ConfigurationManager.AppSettings["sender"], to)
            {
                Subject = subject,
                Body = body
            };

            var smtpClient = new System.Net.Mail.SmtpClient
            {
                Host = ConfigurationManager.AppSettings["smtpHost"],
                Credentials = new System.Net.NetworkCredential(
                    ConfigurationManager.AppSettings["sender"],
                    ConfigurationManager.AppSettings["passwd"]),
                EnableSsl = true
            };
            smtpClient.Port = 587;
            smtpClient.Send(message);
        }
    }
}
