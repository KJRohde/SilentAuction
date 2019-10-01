using MailKit.Net.Smtp;
using Microsoft.AspNet.Identity;
using MimeKit;
using SilentAuction.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class AuctionPrizeController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: AuctionPrize
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Bid(int id)
        {
            AuctionPrize auctionprize = context.AuctionPrizes.FirstOrDefault(a => a.AuctionPrizeId == id);
            return View(auctionprize);
        }
        [HttpPost]
        public ActionResult Bid([Bind(Include = "AuctionPrizeId,ActualValue,MinimumBid,BidIncrement,CurrentBid,Description,TopParticipant,Category,WinnerId,AuctionId,Participant,Paid")]AuctionPrize auctionPrize, int id)
        {
            try
            {
                auctionPrize = context.AuctionPrizes.FirstOrDefault(a => a.AuctionPrizeId == id);
                var currentUserId = User.Identity.GetUserId();
                Participant participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUserId);
                auctionPrize.CurrentBid += auctionPrize.BidIncrement;
                auctionPrize.TopParticipant = participant.ParticipantId;
                context.SaveChanges();
                return RedirectToAction("Index", "Participant");
            }
            catch
            {
                return RedirectToAction("Index", "Participant");
            }
        }
        public ActionResult AddItem(int id)
        {
            AuctionPrize auctionPrize = new AuctionPrize();
            return View(auctionPrize);
        }

        [HttpPost]
        public ActionResult AddItem([Bind(Include = "AuctionPrizeId,Name,Description,ActualValue,MinimumBid,BidIncrement,CurrentBid,Picture,Participant,AuctionId,WinnerId,Category,Paid")] AuctionPrize auctionPrize, int id)
        {
            Auction auction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            auctionPrize.AuctionId = auction.AuctionId;
            auctionPrize.CurrentBid = auctionPrize.MinimumBid - auctionPrize.BidIncrement;
            auctionPrize.TopParticipant = null;
            auctionPrize.WinnerId = null;


            context.AuctionPrizes.Add(auctionPrize);
            context.SaveChanges();


            return RedirectToAction("Continue", new { id = auctionPrize.AuctionId });
        }
        public ActionResult Continue(int id)
        {
            Auction auction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            return View(auction);
        }
        public ActionResult Pay(int id)
        {
            {
                var auctionPrize = context.AuctionPrizes.FirstOrDefault(m => m.AuctionPrizeId == id);
                return View(auctionPrize);
            }
        }

        [HttpPost]
        public ActionResult Pay(string stripeEmail, string stripeToken, AuctionPrize auctionPrize)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();
            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var currentUserId = User.Identity.GetUserId().ToString();
            var participant = context.Participants.FirstOrDefault(m => m.ApplicationUserId == currentUserId);

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = Convert.ToInt64(auctionPrize.CurrentBid),
                Description = auctionPrize.Name,
                Currency = "usd",
                CustomerId = customer.Id
            });
            auctionPrize.Paid = true;
            context.SaveChanges();
            return View();
        }
    }
}