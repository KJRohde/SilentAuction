using MailKit.Net.Smtp;
using Microsoft.AspNet.Identity;
using MimeKit;
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
        public async System.Threading.Tasks.Task<ActionResult> EmailWinnersAsync(int auctionId)
        {
            var auction = context.Auctions.FirstOrDefault(u => u.AuctionId == auctionId);
            var prizes = context.AuctionPrizes.Where(p => p.AuctionId == auction.AuctionId).ToList();
            foreach (AuctionPrize prize in prizes)
            {
                Participant winner = context.Participants.FirstOrDefault(w => w.ParticipantId == prize.WinnerId);
                try
                {
                    //From Address    
                    string FromAddress = "DCCSilentAuction";
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
                            "myname@company.com",
                            "MYPassword"
                            );
                        await client.SendAsync(mimeMessage);
                        Console.WriteLine("The mail has been sent successfully !!");
                        Console.ReadLine();
                        await client.DisconnectAsync(true);
                        return RedirectToAction("SentEmail", "Manager");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    
                }
            }
            return View();
        }
    }
}