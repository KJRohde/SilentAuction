using Microsoft.AspNet.Identity;
using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
        
        public ActionResult Participate(int id)
        {
            var auctionItems = context.AuctionPrizes.Where(a => a.AuctionId == id).ToList();
            return View(auctionItems);
        }
    }
}