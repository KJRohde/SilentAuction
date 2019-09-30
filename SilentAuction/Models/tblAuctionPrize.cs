using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class tblAuctionPrize
    {
        [Key]
        public int AuctionPrizeId { get; set; }
        public double ActualValue { get; set; }
        public double MinimumBid { get; set; }
        public double BidIncrement { get; set; }
        public double CurrentBid { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public int? TopParticipant { get; set; }
        public string Category { get; set; }
        public int? WinnerId { get; set; }
        [ForeignKey("Auction")]
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }
        [DisplayName("Upload Image File")]
        public string ImagePath { get; set; }

    }
}