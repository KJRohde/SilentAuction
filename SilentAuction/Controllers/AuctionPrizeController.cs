using Microsoft.AspNet.Identity;
using SilentAuction.Models;
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
        public ActionResult Bid([Bind(Include = "AuctionPrizeId,ActualValue,MinimumBid,BidIncrement,CurrentBid,Description,TopParticipant,Category,WinnerId,AuctionId")]AuctionPrize auctionPrize, int id)
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
        public ActionResult AddItem([Bind(Include = "AuctionPrizeId,Name,Description,ActualValue,MinimumBid,BidIncrement,CurrentBid,Picture,Winner,AuctionId,WinnerId,Category")] AuctionPrize auctionPrize, int id)
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
    }
}