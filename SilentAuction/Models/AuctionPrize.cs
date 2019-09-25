using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class AuctionPrize
    {
        [Key]
        public int Id { get; set; }
        public double ActualValue { get; set; }
        public double MinimumBid { get; set; }
        public double CurrentBid { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public Participant Winner { get; set; }
        [ForeignKey("Auction")]
        public int AuctionId { get; set; }
    }
}