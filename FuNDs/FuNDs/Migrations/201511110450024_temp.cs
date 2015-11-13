namespace FuNDs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temp : DbMigration
    {
        public override void Up()
        {



            CreateTable(
              "dbo.Campaigns",
              c => new
              {
                  CampaignId = c.Int(nullable: false, identity: true),
                  CampaignTitle = c.String(),
                  CampaignDescription = c.String(),
                  CampaignAmount = c.Double(nullable: false),
                  StartingDate = c.DateTime(precision: 7, storeType: "datetime2"),
                  EndingDate = c.DateTime(precision: 7, storeType: "datetime2"),
                  FundRaisersId = c.Int(nullable: false),
              })
              .PrimaryKey(t => t.CampaignId)
              .ForeignKey("dbo.FundRaisers", t => t.FundRaisersId, cascadeDelete: true)
              .Index(t => t.FundRaisersId);


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
                    RememberMe = c.Boolean(nullable: false),
                    verified = c.Boolean(nullable: false),
                    verificationToken = c.String(),
                })
                .PrimaryKey(t => t.FundRaisersId);

          
            CreateTable(
                "dbo.PasswordResets",
                c => new
                    {
                        PasswordResetID = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false, maxLength: 100),
                        ConfirmPassword = c.String(),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.PasswordResetID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Campaigns", "FundRaisersId", "dbo.FundRaisers");
            DropIndex("dbo.Campaigns", new[] { "FundRaisersId" });
            DropTable("dbo.PasswordResets");
            DropTable("dbo.FundRaisers");
            DropTable("dbo.Campaigns");
        }
    }
}
