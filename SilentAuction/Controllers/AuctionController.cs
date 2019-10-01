using MailKit.Net.Smtp;
using Microsoft.AspNet.Identity;
using MimeKit;
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
        public ActionResult Create([Bind(Include = "Name,Description,StartTime,EndTime,Donors,AuctionId,ManagerId,Day,Message")] Auction auction)
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
        public async System.Threading.Tasks.Task<ActionResult> CloseAuctionAsync(int auctionId)
        {
            var auction = context.Auctions.FirstOrDefault(u => u.AuctionId == auctionId);
            auction.EndTime = DateTime.Now;
            var prizes = context.AuctionPrizes.Where(p => p.AuctionId == auction.AuctionId).ToList();

            foreach (AuctionPrize prize in prizes)
            {
                Participant winner = context.Participants.FirstOrDefault(w => w.ParticipantId == prize.TopParticipant);
                try
                {
                    //From Address    
                    string FromAddress = "DCCSilentAuction@gmail.com";
                    string FromAdressTitle = "Silent Auction App";
                    //To Address    
                    string ToAddress = winner.EmailAddress;
                    string ToAdressTitle = "Winner";
                    string Subject = "You're a Winner!";
                    string BodyContent = prize.Auction.Message;

                    //Smtp Server    
                    string SmtpServer = "smtp.gmail.com";
                    //Smtp Port Number    
                    int SmtpPortNumber = 587;

                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress
                                            (FromAdressTitle,
                                             FromAddress
                                             ));
                    mimeMessage.To.Add(new MailboxAddress
                                             (ToAdressTitle,
                                             ToAddress
                                             ));
                    mimeMessage.Subject = Subject; //Subject  
                    mimeMessage.Body = new TextPart("plain")
                    {
                        Text = BodyContent
                    };

                    using (var client = new SmtpClient())
                    {
                        client.Connect(SmtpServer, SmtpPortNumber, false);
                        client.Authenticate(
                            "DCCSilentAuction@gmail.com",
                            "!234Qwer"
                            );
                        await client.SendAsync(mimeMessage);
                        await client.DisconnectAsync(true);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
            foreach (AuctionPrize prize in prizes)
            {
                prize.ParticipantId = prize.TopParticipant;
                prize.Paid = false;
            }
            await context.SaveChangesAsync();
            return RedirectToAction("CompletedAuctions", "Manager");
        }
    }
}