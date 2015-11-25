using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FuNDs.Models
{
    public class FundRaisersDbContext : DbContext
    {
        public FundRaisersDbContext() : base("name=DefaultConnection")

        {
        }
        public DbSet<FundRaisers> FundRaisers { get; set; }

        public System.Data.Entity.DbSet<FuNDs.Models.Campaign> Campaigns { get; set; }

        public System.Data.Entity.DbSet<FuNDs.Models.Donor> Donors { get; set; }
    }
}