namespace FuNDs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temp1 : DbMigration
    {
        public override void Up()
        {
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
            DropTable("dbo.PasswordResets");
        }
    }
}
