using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class ViewModel
    {
        public IEnumerable<Auction> Auctions { get; set; }
        public IEnumerable<Raffle> Raffles { get; set; }
        public IEnumerable<ParticipantAction> ParticipantActions { get; set; }
        public IEnumerable<Participant> Participants { get; set; }
        public Participant Participant { get; set; }
        public Raffle Raffle { get; set; }
    }
}