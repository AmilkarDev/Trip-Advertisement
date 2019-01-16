namespace TripAdvertisement.DAL.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateForeignKeys : DbMigration
    {
        public override void Up()
        {
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
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        InterestCount = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.CustomersTags",
                c => new
                    {
                        Customers_Id = c.Int(nullable: false),
                        Tags_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Customers_Id, t.Tags_Id })
                .ForeignKey("dbo.Customers", t => t.Customers_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.Tags_Id, cascadeDelete: true)
                .Index(t => t.Customers_Id)
                .Index(t => t.Tags_Id);
            
            CreateTable(
                "dbo.TagsLocations",
                c => new
                    {
                        Tags_Id = c.Int(nullable: false),
                        Locations_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tags_Id, t.Locations_Id })
                .ForeignKey("dbo.Tags", t => t.Tags_Id, cascadeDelete: true)
                .ForeignKey("dbo.Locations", t => t.Locations_Id, cascadeDelete: true)
                .Index(t => t.Tags_Id)
                .Index(t => t.Locations_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagsLocations", "Locations_Id", "dbo.Locations");
            DropForeignKey("dbo.TagsLocations", "Tags_Id", "dbo.Tags");
            DropForeignKey("dbo.CustomersTags", "Tags_Id", "dbo.Tags");
            DropForeignKey("dbo.CustomersTags", "Customers_Id", "dbo.Customers");
            DropForeignKey("dbo.FeedBacks", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.Images", "LocationId", "dbo.Locations");
            DropIndex("dbo.TagsLocations", new[] { "Locations_Id" });
            DropIndex("dbo.TagsLocations", new[] { "Tags_Id" });
            DropIndex("dbo.CustomersTags", new[] { "Tags_Id" });
            DropIndex("dbo.CustomersTags", new[] { "Customers_Id" });
            DropIndex("dbo.FeedBacks", new[] { "Customer_Id" });
            DropIndex("dbo.Images", new[] { "LocationId" });
            DropTable("dbo.TagsLocations");
            DropTable("dbo.CustomersTags");
            DropTable("dbo.FeedBacks");
            DropTable("dbo.Customers");
            DropTable("dbo.Tags");
            DropTable("dbo.Locations");
            DropTable("dbo.Images");
        }
    }
}
