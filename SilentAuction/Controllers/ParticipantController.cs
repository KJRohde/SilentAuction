using Microsoft.AspNet.Identity;
using SilentAuction.Content;
using SilentAuction.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class ParticipantController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Participants
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUserId);
            var myModel = new ViewModel
            {
                Auctions = context.Auctions.Where(s => s.StartTime <= DateTime.Now && s.EndTime >= DateTime.Now && s.Day == DateTime.Today).ToList(),
                Raffles = context.Raffles.Where(s => s.StartTime <= DateTime.Now && s.EndTime >= DateTime.Now && s.Day == DateTime.Today).ToList(),
                ParticipantActions = context.ParticipantActions.OrderByDescending(s => s.Time).Where(s => s.ParticipantId == participant.ParticipantId).Take(3)
            };
            return View(myModel);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = context.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }
        public ActionResult Create()
        {
            Participant participant = new Participant();
            return View(participant);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "FirstName,LastName,EmailAddress,ApplicationUserId")] Participant participant)
        {
            var currentUserId = User.Identity.GetUserId();
            participant.ApplicationUserId = currentUserId;
            participant.EmailAddress = User.Identity.GetUserName();
            if (participant.ApplicationUserId == currentUserId)
            {
                context.Participants.Add(participant);
                context.SaveChanges();
                return RedirectToAction("Index", new { id = participant.ParticipantId });
            }

            return View(participant);
        }
        public ActionResult ViewPrizesWon()
        {
            var currentUserId = User.Identity.GetUserId();
            var participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUserId);
            var auctionPrizesWon = context.AuctionPrizes.Where(a => a.ParticipantId == participant.ParticipantId);
            return View(auctionPrizesWon);
        }
        public ActionResult GetActionHistory()
        {
            var currentUserId = User.Identity.GetUserId();
            var participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUserId);
            var fullHistory = context.ParticipantActions.Where(a => a.ParticipantId == participant.ParticipantId);
            return View(fullHistory);
        }
        public ActionResult BuyTickets(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            var participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUserId);
            var myModel = new ViewModel
            {
                Raffle = context.Raffles.FirstOrDefault(r => r.RaffleId == id),
                Participant = context.Participants.FirstOrDefault(s => s.ParticipantId == participant.ParticipantId)
            };
            return View(myModel);
        }
        [HttpPost]
        public ActionResult BuyTickets(int id, int tickets)
        {

        }
        public ActionResult Pay( int id)
        {
            {
                var key = Keys.StripePublishableKey;
                ViewBag.StripePublishableKey = key;
                var raffle = context.Raffles.FirstOrDefault(r => r.RaffleId == id);
                return View(raffle);
            }
        }

        [HttpPost]
        public ActionResult Pay(string stripeEmail, string stripeToken, int id, int tickets)
        {
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.RaffleId == id);
            double cost = tickets * raffle.CostPerTicket;
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
                Amount = Convert.ToInt64(cost),
                Description = raffle.Name,
                Currency = "usd",
                CustomerId = customer.Id
            });
            participant.RaffleTickets = tickets;
            context.SaveChanges();
            return RedirectToAction("Index", "Participant");
        }
        public Data AddDataPoint(Auction auction)
        {
            Data data = new Data();
            data.Time = DateTime.Now.Ticks;
            data.Money = auction.TotalRaised;
            data.AuctionId = auction.AuctionId;
            context.Data.Add(data);
            context.SaveChanges();
            return data;
        }
    }
}