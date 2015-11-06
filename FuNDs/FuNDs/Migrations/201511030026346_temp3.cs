namespace FuNDs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temp3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FundRaisers", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FundRaisers", "Image");
        }
    }
}
