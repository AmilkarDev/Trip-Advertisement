namespace TripAdvertisement.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usersTOcustomers : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LocationsTags", newName: "TagsLocations");
            DropPrimaryKey("dbo.TagsLocations");
            AddColumn("dbo.Locations", "customer_Id", c => c.Int());
            AddPrimaryKey("dbo.TagsLocations", new[] { "Tags_Id", "Locations_Id" });
            CreateIndex("dbo.Locations", "customer_Id");
            AddForeignKey("dbo.Locations", "customer_Id", "dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Locations", "customer_Id", "dbo.Customers");
            DropIndex("dbo.Locations", new[] { "customer_Id" });
            DropPrimaryKey("dbo.TagsLocations");
            DropColumn("dbo.Locations", "customer_Id");
            AddPrimaryKey("dbo.TagsLocations", new[] { "Locations_Id", "Tags_Id" });
            RenameTable(name: "dbo.TagsLocations", newName: "LocationsTags");
        }
    }
}
