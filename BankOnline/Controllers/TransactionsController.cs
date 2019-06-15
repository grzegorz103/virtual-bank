using System;
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

namespace BankOnline.Controllers
{
    public class TransactionsController : Controller
    {
        private BankContext db = new BankContext();

        // GET: Transactions
        [Authorize]
        public ActionResult Index()
        {
            IQueryable transactions;
            if (User.IsInRole("ADMIN"))
                transactions = db.Transactions.Include(t => t.From).Include(t => t.To);
            else
                transactions = db.Transactions.Include(t => t.From).Include(t => t.To)
            .Where(e => e.To.Profile.UserName == User.Identity.Name || e.From.Profile.UserName == User.Identity.Name);

            return View(transactions);
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
            ViewBag.FromID = new SelectList(db.BankAccounts.Where(e => e.Profile.UserName == User.Identity.Name), "ID", "Number");
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number");
            return View();
        }

        // POST: Transactions/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TransactionDate,FromID,ToID,Amount")] Transaction transaction)
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
                db.Transactions.Add(transaction);
                db.SaveChanges();
                SendMail(target.Profile.UserName, "You received transfer", "You have new transaction. Visit us to check. Virtual bank");
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
