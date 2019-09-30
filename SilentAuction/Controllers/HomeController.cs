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
    public class HomeController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        public ActionResult Index(string email)
        {
            if (User.IsInRole("Manager"))
            {
                Manager manager = context.Managers.FirstOrDefault(m => m.EmailAddress == email);
                return RedirectToAction("Index", "Manager", new { id = manager.ManagerId });
            }
            if (User.IsInRole("Participant"))
            {
                Participant participant = context.Participants.FirstOrDefault(p => p.EmailAddress == email);
                return RedirectToAction("Index", "Participant", new { id = participant.ParticipantId });
            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //[HttpGet]
        //public ActionResult ContactForm(AuctionPrize auctionPrize)
        //{
        //    string FileName = Path.GetFileNameWithoutExtension(auctionPrize.ImageFile.FileName);
        //    string FileExtension = Path.GetExtension(auctionPrize.ImageFile.FileName);
        //    string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
        //    auctionPrize.ImagePath = UploadPath + FileName;
        //    auctionPrize.ImageFile.SaveAs(auctionPrize.ImagePath);

        //    var db = new SilentAuctionDataClassesDataContext();
        //    tblAuctionPrize _auctionPrize = new tblAuctionPrize();

        //    _auctionPrize.ImagePath = auctionPrize.ImagePath;
        //    _auctionPrize.AuctionId = auctionPrize.AuctionId;
        //    _auctionPrize.ActualValue = auctionPrize.ActualValue;
        //    _auctionPrize.Description = auctionPrize.Description;
        //    _auctionPrize.WinnerId = auctionPrize.WinnerId;
        //    _auctionPrize.CurrentBid = auctionPrize.CurrentBid;
        //    _auctionPrize.Category = auctionPrize.Category;
        //    _auctionPrize.MinimumBid = auctionPrize.MinimumBid;
        //    _auctionPrize.TopParticipant = auctionPrize.TopParticipant;
        //    _auctionPrize.BidIncrement = auctionPrize.BidIncrement;

        //    db.tblAuctionPrizes.InsertOnSubmit(_auctionPrize);
        //    db.SubmitChanges();
        //    return View();
        //}
        //[HttpGet]
        //public ActionResult ContactForm(RafflePrize rafflePrize)
        //{
        //    string FileName = Path.GetFileNameWithoutExtension(rafflePrize.ImageFile.FileName);
        //    string FileExtension = Path.GetExtension(rafflePrize.ImageFile.FileName);
        //    string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
        //    rafflePrize.ImagePath = UploadPath + FileName;
        //    rafflePrize.ImageFile.SaveAs(rafflePrize.ImagePath);

        //    var db = new SilentAuctionDataClassesDataContext();
        //    tblRafflePrize _rafflePrize = new tblRafflePrize();

        //    _rafflePrize.ImagePath = rafflePrize.ImagePath;
        //    _rafflePrize.RaffleId = rafflePrize.RaffleId;
        //    _rafflePrize.Value = rafflePrize.Value;
        //    _rafflePrize.Description = rafflePrize.Description;
        //    _rafflePrize.WinnerId = rafflePrize.WinnerId;
        //    _rafflePrize.CurrentTickets = rafflePrize.CurrentTickets;
        //    _rafflePrize.Category = rafflePrize.Category;

        //    db.tblRafflePrizes.InsertOnSubmit(_rafflePrize);
        //    db.SubmitChanges();

        //    return View();
        //}
    }
}