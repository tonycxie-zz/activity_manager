using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace BeltExam.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [MinLength(2)]
        [NoNumbers]
        public string FirstName {get;set;}

        [Required]
        [MinLength(2)]
        [NoNumbers]
        public string LastName {get;set;}

        [Required]
        [EmailAddress]
        public string Email {get;set;}

        [DataType(DataType.Password)]
        [Required]
        [MinLength(8)]
        // [PasswordPattern]
        public string Password {get;set;}

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}

        public List<Activity> PlannedActivities {get;set;}

        public List<Participant> AttendedActivities {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }

    public class NoNumbersAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(!Regex.IsMatch((string)value, @"^[a-zA-Z]+$"))
            {
                return new ValidationResult("Name cannot contain numbers");
            }
            return ValidationResult.Success;
        }
    }
}