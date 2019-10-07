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
        public ActionResult Create([Bind(Include = "FirstName,LastName,EmailAddress,ApplicationUserId,RaffleTickets")] Participant participant)
        {
            var currentUserId = User.Identity.GetUserId();
            participant.ApplicationUserId = currentUserId;
            participant.EmailAddress = User.Identity.GetUserName();
            participant.RaffleTickets = 0;
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
            var myModel = new ViewModel
            {
                AuctionPrizes = context.AuctionPrizes.Where(s => s.ParticipantId == participant.ParticipantId).ToList(),
                RafflePrizes = context.RafflePrizes.Where(r => r.WinnerId == participant.ParticipantId).ToList()
            };
            return View(myModel);
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
            Participant participant = context.Participants.FirstOrDefault(p => p.ParticipantId == id);
            return View(participant);
        }
        [HttpPost]
        public ActionResult BuyTickets([Bind(Include = "FirstName,LastName,EmailAddress,ApplicationUserId,RaffleTickets")] Participant participant, int id)
        {
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.RaffleId == id);
            var currentUserId = User.Identity.GetUserId();
            var buyingParticipant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUserId);
            int tickets = (participant.RaffleTickets);
            double amount = (tickets * raffle.CostPerTicket);
            raffle.TotalRaised += amount;
            buyingParticipant.RaffleTickets += participant.RaffleTickets;
            context.SaveChanges();
            AddDataPoint(raffle);
            AddTransaction(raffle, amount, tickets);
            AddAction(raffle, tickets);
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
        public Data AddDataPoint(Raffle raffle)
        {
            Data data = new Data();
            data.Time = DateTime.Now.Ticks;
            data.Money = raffle.TotalRaised;
            data.RaffleId = raffle.RaffleId;
            context.Data.Add(data);
            context.SaveChanges();
            return data;
        }
        public Transaction AddTransaction(Raffle raffle, double amount, int tickets)
        {
            var currentUserId = User.Identity.GetUserId();
            Participant participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUserId);
            Transaction transaction = new Transaction();
            transaction.Money = amount;
            transaction.ManagerId = raffle.ManagerId;
            transaction.ParticipantId = participant.ParticipantId;
            transaction.Paid = false;
            transaction.Description = "Purchasing " + tickets + " tickets from " + raffle.Name + ".";
            context.Transactions.Add(transaction);
            context.SaveChanges();
            return transaction;

        }
        public ActionResult AddTickets(int id)
        {
            var rafflePrize = context.RafflePrizes.FirstOrDefault(p => p.RafflePrizeId == id);
            return View(rafflePrize);
        }
        [HttpPost]
        public ActionResult AddTickets([Bind(Include = "RafflePrizeId,Description,Value,RaffleId,Category,CurrentTickets,WinnerId,Name")] RafflePrize rafflePrize, int id)
        {
            
            var currentUser = User.Identity.GetUserId();
            var participant = context.Participants.FirstOrDefault(c => c.ApplicationUserId == currentUser);
            if (rafflePrize.CurrentTickets <= participant.RaffleTickets)
            {
                var prizeToEdit = context.RafflePrizes.FirstOrDefault(p => p.RafflePrizeId == id);
                prizeToEdit.CurrentTickets += rafflePrize.CurrentTickets;
                participant.RaffleTickets -= rafflePrize.CurrentTickets;
                for (int i = 1; i <= rafflePrize.CurrentTickets; i++)
                {
                    Ticket ticket = new Ticket();
                    ticket.RafflePrizeId = rafflePrize.RafflePrizeId;
                    ticket.ParticipantId = participant.ParticipantId;
                    context.Tickets.Add(ticket);
                }
                AddAction(rafflePrize, rafflePrize.CurrentTickets);
                context.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "You cannot enter this many tickets.");
                return View(rafflePrize);
            }
            return RedirectToAction("Participate", "Raffle", new { id = rafflePrize.RaffleId });
        }
        public ParticipantAction AddAction(Raffle raffle, int tickets)
        {
            ParticipantAction action = new ParticipantAction();
            var currentUser = User.Identity.GetUserId();
            Participant participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUser);
            action.Action = "Acquired " + tickets + " for " + raffle.Name + ", please pay for your tickets in the transactions tab.";
            action.Time = DateTime.Now;
            action.ParticipantId = participant.ParticipantId;
            context.ParticipantActions.Add(action);
            context.SaveChanges();
            return (action);
        }
        public ParticipantAction AddAction(RafflePrize rafflePrize, int tickets)
        {
            ParticipantAction action = new ParticipantAction();
            var currentUser = User.Identity.GetUserId();
            Participant participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUser);
            action.Action = "Placed " + tickets + " for " + rafflePrize.Name + ".";
            action.Time = DateTime.Now;
            action.ParticipantId = participant.ParticipantId;
            context.ParticipantActions.Add(action);
            context.SaveChanges();
            return (action);
        }
    }
}