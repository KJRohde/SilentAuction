﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class Raffle
    {
        [Key]
        public int RaffleId { get; set; }
        [DataType(DataType.Date)]
        public DateTime Day { get; set; }
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
        public double TotalRaised { get; set; }
        public string Donors { get; set; }
        public string Name { get; set; }
        [ForeignKey("Manager")]
        public int ManagerId { get; set; }
        public Manager Manager { get; set; }
        public string Description { get; set; }
        public int CostPerTicket { get; set; }
    }
}