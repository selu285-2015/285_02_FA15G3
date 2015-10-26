namespace FuNDs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temp1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Campaigns", "StartingDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Campaigns", "EndingDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Campaigns", "EndingDate", c => c.String());
            AlterColumn("dbo.Campaigns", "StartingDate", c => c.String());
        }
    }
}
