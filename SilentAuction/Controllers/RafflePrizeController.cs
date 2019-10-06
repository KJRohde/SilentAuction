using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class RafflePrizeController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: RafflePrize
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddItem(int id)
        {
            RafflePrize rafflePrize = new RafflePrize();
            return View(rafflePrize);
        }

        [HttpPost]
        public ActionResult AddItem([Bind(Include = "RafflePrizeId,Description,Value,RaffleId,Category,CurrentTickets,WinnerId,Name")] RafflePrize rafflePrize, int id)
        {
            Raffle raffle = context.Raffles.FirstOrDefault(a => a.RaffleId == id);
            rafflePrize.RaffleId = raffle.RaffleId;
            rafflePrize.CurrentTickets = 0;
            rafflePrize.WinnerId = null;


            context.RafflePrizes.Add(rafflePrize);
            context.SaveChanges();


            return RedirectToAction("Continue", new { id = rafflePrize.RaffleId });
        }
        public ActionResult Continue(int id)
        {
            Raffle raffle = context.Raffles.FirstOrDefault(a => a.RaffleId == id);
            return View(raffle);
        }
    }
}