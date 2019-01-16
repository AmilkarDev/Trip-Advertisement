namespace TripAdvertisement.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        UserName = c.String(),
                        Email = c.String(),
                        PhoneNum = c.String(),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FeedBacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.String(),
                        About = c.String(),
                        Description = c.String(),
                        Customer_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id, cascadeDelete: true)
                .Index(t => t.Customer_Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        InterestCount = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ImageLink = c.String(),
                        Address = c.String(),
                        Area = c.String(),
                        Latitude = c.String(),
                        Logtitude = c.String(),
                        Description = c.String(),
                        Country = c.String(),
                        City = c.String(),
                        AverageTime = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Link = c.String(),
                        Name = c.String(maxLength: 255),
                        Description = c.String(),
                        Date = c.String(),
                        LocationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.LocationId, cascadeDelete: true)
                .Index(t => t.LocationId);
            
            CreateTable(
                "dbo.TagsCustomers",
                c => new
                    {
                        Tags_Id = c.Int(nullable: false),
                        Customers_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tags_Id, t.Customers_Id })
                .ForeignKey("dbo.Tags", t => t.Tags_Id, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.Customers_Id, cascadeDelete: true)
                .Index(t => t.Tags_Id)
                .Index(t => t.Customers_Id);
            
            CreateTable(
                "dbo.LocationsTags",
                c => new
                    {
                        Locations_Id = c.Int(nullable: false),
                        Tags_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Locations_Id, t.Tags_Id })
                .ForeignKey("dbo.Locations", t => t.Locations_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.Tags_Id, cascadeDelete: true)
                .Index(t => t.Locations_Id)
                .Index(t => t.Tags_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocationsTags", "Tags_Id", "dbo.Tags");
            DropForeignKey("dbo.LocationsTags", "Locations_Id", "dbo.Locations");
            DropForeignKey("dbo.Images", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.TagsCustomers", "Customers_Id", "dbo.Customers");
            DropForeignKey("dbo.TagsCustomers", "Tags_Id", "dbo.Tags");
            DropForeignKey("dbo.FeedBacks", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.LocationsTags", new[] { "Tags_Id" });
            DropIndex("dbo.LocationsTags", new[] { "Locations_Id" });
            DropIndex("dbo.TagsCustomers", new[] { "Customers_Id" });
            DropIndex("dbo.TagsCustomers", new[] { "Tags_Id" });
            DropIndex("dbo.Images", new[] { "LocationId" });
            DropIndex("dbo.FeedBacks", new[] { "Customer_Id" });
            DropTable("dbo.LocationsTags");
            DropTable("dbo.TagsCustomers");
            DropTable("dbo.Images");
            DropTable("dbo.Locations");
            DropTable("dbo.Tags");
            DropTable("dbo.FeedBacks");
            DropTable("dbo.Customers");
        }
    }
}
