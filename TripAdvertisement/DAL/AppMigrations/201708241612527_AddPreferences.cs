namespace TripAdvertisement.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPreferences : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Preferences",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Country = c.String(),
                        city = c.String(),
                        Tag = c.String(),
                        customer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.customer_Id)
                .Index(t => t.customer_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Preferences", "customer_Id", "dbo.Customers");
            DropIndex("dbo.Preferences", new[] { "customer_Id" });
            DropTable("dbo.Preferences");
        }
    }
}
