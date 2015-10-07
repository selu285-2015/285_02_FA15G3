namespace FuNDs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class anything : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FundRaisers",
                c => new
                    {
                        FundRaisersId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        ConfirmPassword = c.String(),
                    })
                .PrimaryKey(t => t.FundRaisersId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FundRaisers");
        }
    }
}
