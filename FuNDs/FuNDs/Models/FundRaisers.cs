using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace FuNDs.Models
{
    public class FundRaisers
    {
        [Key]
        public int FundRaisersId { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "Oops! First Name must have at least 2 characters.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "Oops! Last Name must have at least 2 characters.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Remote("doesEmailExist", "FundRaisers", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }



        [DataType(DataType.Password)]
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public bool verified { get; set; }
        public string verificationToken { get; set; }

        public virtual ICollection<Campaign> Campaigns { get; set; }
    
    }
}
   