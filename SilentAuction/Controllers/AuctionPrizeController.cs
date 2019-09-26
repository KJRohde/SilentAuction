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
        public ActionResult Create(int id)
        {
            AuctionPrize auctionPrize = new AuctionPrize();
            return View(auctionPrize);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Description,ActualValue,MinimumBid,BidIncrement,CurrentBid,Picture,Winner,AuctionId,WinnerId")] AuctionPrize auctionPrize, int id)
        {
            var currentAuction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            auctionPrize.CurrentBid = 0;
            auctionPrize.Winner = null;
            

                context.AuctionPrizes.Add(auctionPrize);
                context.SaveChanges();
                return RedirectToAction("Continue", new { id = currentAuction.AuctionId });
        }
    }
}