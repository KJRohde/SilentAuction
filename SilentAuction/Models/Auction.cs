﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static SilentAuction.Models.LineChart;

namespace SilentAuction.Models
{
    public class Auction
    {
        [Key]
        public int AuctionId { get; set; }
        [ForeignKey("Manager")]
        public int? ManagerId { get; set; }
        [Display(Name = "Message for winners")]
        public string Message { get; set; }
        public Manager Manager { get; set; }
        [DataType(DataType.Date)]
        public DateTime Day { get; set; }
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }
        public double TotalRaised { get; set; }
        public string Donors { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}