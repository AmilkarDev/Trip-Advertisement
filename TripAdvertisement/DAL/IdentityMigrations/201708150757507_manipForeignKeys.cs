namespace TripAdvertisement.DAL.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manipForeignKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Images", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.FeedBacks", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.CustomersTags", "Customers_Id", "dbo.Customers");
            DropForeignKey("dbo.CustomersTags", "Tags_Id", "dbo.Tags");
            DropForeignKey("dbo.TagsLocations", "Tags_Id", "dbo.Tags");
            DropForeignKey("dbo.TagsLocations", "Locations_Id", "dbo.Locations");
            DropIndex("dbo.Images", new[] { "LocationId" });
            DropIndex("dbo.FeedBacks", new[] { "Customer_Id" });
            DropIndex("dbo.CustomersTags", new[] { "Customers_Id" });
            DropIndex("dbo.CustomersTags", new[] { "Tags_Id" });
            DropIndex("dbo.TagsLocations", new[] { "Tags_Id" });
            DropIndex("dbo.TagsLocations", new[] { "Locations_Id" });
            DropTable("dbo.Images");
            DropTable("dbo.Locations");
            DropTable("dbo.Tags");
            DropTable("dbo.Customers");
            DropTable("dbo.FeedBacks");
            DropTable("dbo.CustomersTags");
            DropTable("dbo.TagsLocations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TagsLocations",
                c => new
                    {
                        Tags_Id = c.Int(nullable: false),
                        Locations_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tags_Id, t.Locations_Id });
            
            CreateTable(
                "dbo.CustomersTags",
                c => new
                    {
                        Customers_Id = c.Int(nullable: false),
                        Tags_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Customers_Id, t.Tags_Id });
            
            CreateTable(
                "dbo.RoleViewModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(),
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.TagsLocations", "Locations_Id");
            CreateIndex("dbo.TagsLocations", "Tags_Id");
            CreateIndex("dbo.CustomersTags", "Tags_Id");
            CreateIndex("dbo.CustomersTags", "Customers_Id");
            CreateIndex("dbo.FeedBacks", "Customer_Id");
            CreateIndex("dbo.Images", "LocationId");
            AddForeignKey("dbo.TagsLocations", "Locations_Id", "dbo.Locations", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TagsLocations", "Tags_Id", "dbo.Tags", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomersTags", "Tags_Id", "dbo.Tags", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomersTags", "Customers_Id", "dbo.Customers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FeedBacks", "Customer_Id", "dbo.Customers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Images", "LocationId", "dbo.Locations", "Id", cascadeDelete: true);
        }
    }
}
