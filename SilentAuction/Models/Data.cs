using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class Data
    {
        [Key]
        public int DataId { get; set; }
        public double Time { get; set; }
        public double Money { get; set; }
        [ForeignKey("Auction")]
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }

    }
}