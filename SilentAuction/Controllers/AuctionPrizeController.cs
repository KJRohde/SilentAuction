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
        public ActionResult Bid([Bind(Include = "AuctionPrizeId,ActualValue,MinimumBid,BidIncrement,CurrentBid,Description,TopParticipant,Category,WinnerId,AuctionId,Participant,Paid,CustomBid")]AuctionPrize auctionPrize, int id)
        {
            try
            {
                var currentUserId = User.Identity.GetUserId();
                var auctionPrizeToEdit = context.AuctionPrizes.FirstOrDefault(a => a.AuctionPrizeId == id);
                Participant participant = context.Participants.FirstOrDefault(p => p.ApplicationUserId == currentUserId);
                Auction auction = context.Auctions.FirstOrDefault(u => u.AuctionId == auctionPrizeToEdit.AuctionId);
                Participant outbidParticipant = context.Participants.FirstOrDefault(o => o.ParticipantId == auctionPrizeToEdit.TopParticipant);
                if (auctionPrize.CustomBid == 0)
                {
                    if (auctionPrizeToEdit.CurrentBid == 0)
                    {
                        auctionPrizeToEdit.CurrentBid += auctionPrizeToEdit.MinimumBid;
                        auction.TotalRaised += auctionPrizeToEdit.MinimumBid;
                    }
                    else
                    {
                        auctionPrizeToEdit.CurrentBid += auctionPrizeToEdit.BidIncrement;
                        auction.TotalRaised += auctionPrizeToEdit.BidIncrement;
                        ParticipantAction participantActionOutBid = new ParticipantAction();
                        participantActionOutBid.ParticipantId = auctionPrizeToEdit.TopParticipant;
                        participantActionOutBid.Action = "You have been outbid on " + auctionPrizeToEdit.Name + ".";
                        participantActionOutBid.Time = DateTime.Now;
                        context.ParticipantActions.Add(participantActionOutBid);
                    }
                }
                else if (auctionPrize.CustomBid != 0 && auctionPrize.CustomBid >= auctionPrizeToEdit.BidIncrement + auctionPrizeToEdit.CurrentBid)
                {
                    if (auctionPrizeToEdit.CurrentBid == 0)
                    {
                        auctionPrizeToEdit.CurrentBid = auctionPrize.CustomBid;
                        auction.TotalRaised += auctionPrize.CustomBid;
                        auctionPrizeToEdit.CustomBid = 0;
                    }
                    else
                    {
                        auctionPrizeToEdit.CurrentBid = auctionPrize.CustomBid;
                        auction.TotalRaised += auctionPrize.CustomBid;
                        auctionPrizeToEdit.CustomBid = 0;
                        ParticipantAction participantActionOutBid = new ParticipantAction();
                        participantActionOutBid.ParticipantId = auctionPrizeToEdit.TopParticipant;
                        participantActionOutBid.Action = "You have been outbid on " + auctionPrizeToEdit.Name + ".";
                        participantActionOutBid.Time = DateTime.Now;
                        context.ParticipantActions.Add(participantActionOutBid);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "You must bid more than the minimum bid increment");
                    return View(auctionPrizeToEdit);
                }
                ParticipantAction participantActionBid = new ParticipantAction();
                participantActionBid.ParticipantId = participant.ParticipantId;
                participantActionBid.Action = "You have bid $" + auctionPrizeToEdit.CurrentBid + " on " + auctionPrizeToEdit.Name + ".";
                participantActionBid.Time = DateTime.Now;
                context.ParticipantActions.Add(participantActionBid);
                AddDataPoint(auction);
                auctionPrizeToEdit.TopParticipant = participant.ParticipantId;
                context.SaveChanges();
                return RedirectToAction("Participate", "Auction", new { id = auctionPrizeToEdit.AuctionId });
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
        public ActionResult AddItem([Bind(Include = "AuctionPrizeId,Name,Description,ActualValue,MinimumBid,BidIncrement,CurrentBid,Picture,Participant,AuctionId,WinnerId,Category,Paid, CustomBid")] AuctionPrize auctionPrize, int id)
        {
            Auction auction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            auctionPrize.AuctionId = auction.AuctionId;
            auctionPrize.CurrentBid = 0;
            auctionPrize.CustomBid = 0;
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