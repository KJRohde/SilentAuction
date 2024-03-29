﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class AuctionPrize
    {
        [Key]
        public int AuctionPrizeId { get; set; }
        public string Name { get; set; }
        [Display(Name = "Actual Value")]
        public double ActualValue { get; set; }
        [Display(Name = "Minimum Bid")]
        public double MinimumBid { get; set; }
        [Display(Name = "Bid Increment")]
        public double BidIncrement { get; set; }
        public double CustomBid { get; set; }
        [Display(Name = "Current Bid")]
        public double CurrentBid { get; set; }
        public string Description { get; set; }
        public int? TopParticipant { get; set; }
        public string Category { get; set; }
        [ForeignKey("Participant")]
        public int? ParticipantId { get; set; }
        public Participant Participant { get; set; }
        [ForeignKey("Auction")]
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }
        //[DisplayName("Upload Image File")]
        //public string ImagePath { get; set; }
        //[NotMapped]
        //public HttpPostedFileBase ImageFile { get; set; }

    }
    public enum ItemCategory
    {
        Sports,
        Booze,
        Misc,
        Entertainment,
        Kids
    }
}