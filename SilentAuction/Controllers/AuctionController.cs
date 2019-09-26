using Microsoft.AspNet.Identity;
using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class AuctionController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Auction
        public ActionResult Index(int id)
        {
            Auction auction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            return View(auction);
        }
        public ActionResult Create()
        {
            Auction auction = new Auction();
            return View(auction);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Name,Description,StartTime,EndTime,Donors,AuctionId,ManagerId,Day")] Auction auction)
        {
            var currentUserId = User.Identity.GetUserId();
            Manager manager = context.Managers.FirstOrDefault(m => m.ApplicationUserId == currentUserId);
            auction.ManagerId = manager.ManagerId;
            if (manager.ApplicationUserId == currentUserId)
            {
                context.Auctions.Add(auction);
                context.SaveChanges();
                return RedirectToAction("Index", new { id = auction.AuctionId });
            }

            return View(auction);
        }

        public ActionResult AddItem(int id)
        {
            AuctionPrize auctionPrize = new AuctionPrize();
            return View(auctionPrize);
        }

        [HttpPost]
        public ActionResult AddItem([Bind(Include = "Description,ActualValue,MinimumBid,BidIncrement,CurrentBid,Picture,Winner,AuctionId,WinnerId")] AuctionPrize auctionPrize, int id)
        {
            Auction auction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            auctionPrize.AuctionId = auction.AuctionId;
            auctionPrize.CurrentBid = 0;
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