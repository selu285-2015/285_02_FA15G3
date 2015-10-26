using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FuNDs.Models
{
    public class Campaign
    {
        
        public int CampaignId { get; set; }

        public string CampaignTitle { get; set; }
        public string CampaignDescription { get; set; }

        public double CampaignAmount { get; set; }

        [Column(TypeName ="datetime2")]
        public DateTime? StartingDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? EndingDate { get; set; }

        // public FundRaisers owner { get; set; }

        //[ForeignKey("FundRaisers")]
        //public int? FundRaisersId { get; set; }



        [Display(Name = "FundRaisers*")]
        [Required(ErrorMessage = "Who is the fundRaiserss")]
       
        public int FundRaisersId { get; set; }


        public virtual FundRaisers FundRaisers { get; set; }
    }
}