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
    public class DefinedTransfersController : Controller
    {
        private BankContext db = new BankContext();

        // GET: DefinedTransfers
        public ActionResult Index()
        {
            var definedTransfers = db.DefinedTransfers.Include(d => d.From).Include(d => d.Profile).Include(d => d.To)
                .Where(e=>e.Profile.UserName== User.Identity.Name);
            return View(definedTransfers.ToList());
        }

        // GET: DefinedTransfers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DefinedTransfer definedTransfer = db.DefinedTransfers.Find(id);
            if (definedTransfer == null)
            {
                return HttpNotFound();
            }
            return View(definedTransfer);
        }

        public ActionResult NewTrans(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DefinedTransfer defined = db.DefinedTransfers.Find(id);

            Transaction newTrans = new Transaction();
            newTrans.From = defined.From;
            newTrans.To = defined.To;
            newTrans.Amount = defined.Amount;
            newTrans.Title = defined.Title;
            newTrans.Defined = false;

            if (defined == null)
            {
                return HttpNotFound();
            }
            ViewBag.FromID = new SelectList(db.BankAccounts.Where(e => e.Profile.UserName == User.Identity.Name), "ID", "Number", defined.FromID);
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number", defined.ToID);
            return View(newTrans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewTrans([Bind(Include = "ID,TransactionDate,FromID,ToID,Amount,Defined,Title")] Transaction transaction)
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

        // GET: DefinedTransfers/Create
        public ActionResult Create()
        {
            ViewBag.FromID = new SelectList(db.BankAccounts.Where(e=>e.Profile.UserName==User.Identity.Name), "ID", "Number");
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName");
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number");
            return View();
        }

        // POST: DefinedTransfers/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FromID,ToID,Amount,Title,ProfileID")] DefinedTransfer definedTransfer)
        {
            if (ModelState.IsValid)
            {
                definedTransfer.Profile = db.Profiles.Single(e => User.Identity.Name == e.UserName);
                db.DefinedTransfers.Add(definedTransfer);
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FromID = new SelectList(db.BankAccounts.Where(e=>e.Profile.UserName==User.Identity.Name), "ID", "Number", definedTransfer.FromID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", definedTransfer.ProfileID);
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number", definedTransfer.ToID);
            return View(definedTransfer);
        }

        // GET: DefinedTransfers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DefinedTransfer definedTransfer = db.DefinedTransfers.Find(id);
            if (definedTransfer == null)
            {
                return HttpNotFound();
            }
            ViewBag.FromID = new SelectList(db.BankAccounts.Where(e=>e.Profile.UserName==User.Identity.Name), "ID", "Number", definedTransfer.FromID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", definedTransfer.ProfileID);
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number", definedTransfer.ToID);
            return View(definedTransfer);
        }

        // POST: DefinedTransfers/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FromID,ToID,Amount,Title,ProfileID")] DefinedTransfer definedTransfer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(definedTransfer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FromID = new SelectList(db.BankAccounts, "ID", "Number", definedTransfer.FromID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", definedTransfer.ProfileID);
            ViewBag.ToID = new SelectList(db.BankAccounts, "ID", "Number", definedTransfer.ToID);
            return View(definedTransfer);
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

        // GET: DefinedTransfers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DefinedTransfer definedTransfer = db.DefinedTransfers.Find(id);
            if (definedTransfer == null)
            {
                return HttpNotFound();
            }
            return View(definedTransfer);
        }

        // POST: DefinedTransfers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DefinedTransfer definedTransfer = db.DefinedTransfers.Find(id);
            db.DefinedTransfers.Remove(definedTransfer);
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
