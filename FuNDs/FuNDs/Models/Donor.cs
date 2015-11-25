using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuNDs.Models
{
    public class Donor
    {
        public int DonorId { get; set; }



      //  public string donorFirstName { get; set; }

        //    public string? donorLastName { get; set; }

        public double donateAmount { get; set; }

        public string donateDate { get; set; }

        /*   public string? donorMessage { get; set; */

            // creating a foreign campaign ID/KEy
      //  public int CampaignId { get; set; }

       // public virtual Campaign Campaign { get; set; }
        
    }


}

