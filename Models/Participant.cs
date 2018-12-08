using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId {get;set;}

        public int UserId {get;set;}

        public User Attendee;

        public int ActivityId {get;set;}

        public Activity Activity {get;set;}
    }
}