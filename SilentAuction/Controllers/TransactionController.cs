using Microsoft.AspNet.Identity;
using SilentAuction.Content;
using SilentAuction.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class TransactionController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: Transaction
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Pay(int id)
        {
            {
                var key = Keys.StripePublishableKey;
                ViewBag.StripePublishableKey = key;
                var transaction = context.Transactions.FirstOrDefault(r => r.TransactionId == id);
                return View(transaction);
            }
        }

        [HttpPost]
        public ActionResult Pay(string stripeEmail, string stripeToken, int id)
        {
            Transaction transaction = context.Transactions.FirstOrDefault(r => r.TransactionId == id);
            var customers = new CustomerService();
            var charges = new ChargeService();
            StripeConfiguration.ApiKey = Keys.StripeSecretKey;
            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var currentUserId = User.Identity.GetUserId().ToString();
            var participant = context.Participants.FirstOrDefault(m => m.ApplicationUserId == currentUserId);

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = Convert.ToInt64(transaction.Money * 100),
                Description = "Silent Auction App",
                Currency = "usd",
                CustomerId = customer.Id
            });
            transaction.Paid = true;
            AddAction(transaction);
            context.SaveChanges();
            return RedirectToAction("GetTransactions", "Transaction");
        }
        public ActionResult GetTransactions()
        {
            var currentUser = User.Identity.GetUserId();
            if (User.IsInRole("Manager"))
            {
                var manager = context.Managers.FirstOrDefault(m => m.ApplicationUserId == currentUser);
                var transactions = context.Transactions.Where(t => t.ManagerId == manager.ManagerId);
                return View(transactions);
            }
            else if (User.IsInRole("Participant"))
            {
                var participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUser);
                var transactions = context.Transactions.Where(t => t.ParticipantId == participant.ParticipantId);
                return View(transactions);
            }
            return View();
        }
        public ParticipantAction AddAction(Transaction transaction)
        {
            ParticipantAction action = new ParticipantAction();
            action.Action = "Paid $" + transaction.Money + " for " + transaction.Description + ".";
            action.Time = DateTime.Now;
            action.ParticipantId = transaction.ParticipantId;
            context.ParticipantActions.Add(action);
            return (action);
        }
    }
}