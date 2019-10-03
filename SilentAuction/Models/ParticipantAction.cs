using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class ParticipantAction
    {
        [Key]
        public int ParticipantActionId { get; set; }
        public string Action { get; set; }
        [ForeignKey("Participant")]
        public int ParticipantId { get; set; }
        public Participant Participant { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }
    }
}