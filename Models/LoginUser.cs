using System;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models
{
    public class LoginUser 
    {
        [Required]
        [EmailAddress]
        public string LoginEmail {get;set;}
        
        [DataType(DataType.Password)]
        [Required]
        [MinLength(8)]
        public string LoginPassword {get;set;}
    }
}