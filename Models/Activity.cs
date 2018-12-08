using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId {get;set;}

        [Required]
        [MinLength(2)]
        public string Title {get;set;}

        [Required]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date {get;set;}

        [Required]
        [DataType(DataType.Time), DisplayFormat(DataFormatString = "{hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime Time {get;set;}

        [Required]
        public int Duration {get;set;}

        [Required]
        public string DurationType {get;set;}

        [Required]
        [MinLength(5)]
        public string Description {get;set;}

        public int UserId {get;set;}

        public User Coordinator {get;set;}

        public List<Participant> Participants {get;set;}
    }
}