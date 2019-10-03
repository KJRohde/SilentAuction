using MailKit.Net.Smtp;
using Microsoft.AspNet.Identity;
using MimeKit;
using Newtonsoft.Json;
using SilentAuction.Content;
using SilentAuction.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using static SilentAuction.Models.LineChart;

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
                Participant outbidParticipant = context.Participants.FirstOrDefault(o => o.ParticipantId == auctionPrize.TopParticipant);
                Auction auction = context.Auctions.FirstOrDefault(u => u.AuctionId == auctionPrize.AuctionId);
                if (auctionPrize.CurrentBid == 0)
                {
                    auctionPrize.CurrentBid += auctionPrize.MinimumBid;
                    auction.TotalRaised += auctionPrize.MinimumBid;
                }
                else
                {
                    auctionPrize.CurrentBid += auctionPrize.BidIncrement;
                    auction.TotalRaised += auctionPrize.BidIncrement;
                    outbidParticipant.RecentActionThree = outbidParticipant.RecentActionTwo;
                    outbidParticipant.RecentActionTwo = outbidParticipant.RecentActionOne;
                    outbidParticipant.RecentActionOne = "You have been outbid on" + auctionPrize.Name + ".";
                }
                participant.RecentActionThree = participant.RecentActionTwo;
                participant.RecentActionTwo = participant.RecentActionOne;
                participant.RecentActionOne = "You have bid $" + auctionPrize.CurrentBid + " on" + auctionPrize.Name + ".";
                AddDataPoint(auction);
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
            auctionPrize.CurrentBid = 0;
            auctionPrize.TopParticipant = null;
            auctionPrize.ParticipantId = null;


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
                var key = Keys.StripePublishableKey;
                ViewBag.StripePublishableKey = key;
                var auctionPrize = context.AuctionPrizes.FirstOrDefault(m => m.AuctionPrizeId == id);
                return View(auctionPrize);
            }
        }

        [HttpPost]
        public ActionResult Pay(string stripeEmail, string stripeToken, int id)
        {
            var auctionPrize = context.AuctionPrizes.FirstOrDefault(m => m.AuctionPrizeId == id);
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
            var bid = auctionPrize.CurrentBid * 100;

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = Convert.ToInt64(bid),
                Description = auctionPrize.Name,
                Currency = "usd",
                CustomerId = customer.Id
            });
            auctionPrize.Paid = true;
            context.SaveChanges();
            return RedirectToAction("ViewPrizesWon", "Participant");
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