namespace FuNDs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temp2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.FundRaisers", "image");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FundRaisers", "image", c => c.String());
        }
    }
}
