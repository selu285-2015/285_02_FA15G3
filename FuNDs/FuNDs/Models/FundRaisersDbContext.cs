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
    }
}