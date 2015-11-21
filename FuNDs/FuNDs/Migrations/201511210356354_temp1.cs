namespace FuNDs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temp1 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Donors");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Donors",
                c => new
                    {
                        DonorId = c.Int(nullable: false, identity: true),
                        donateAmount = c.Double(nullable: false),
                        donateDate = c.String(),
                    })
                .PrimaryKey(t => t.DonorId);
            
        }
    }
}
