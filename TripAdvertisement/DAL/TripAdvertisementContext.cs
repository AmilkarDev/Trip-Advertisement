using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using TripAdvertisement.Models;

namespace TripAdvertisement.DAL
{
    public class TripAdvertisementContext : DbContext
    {
        public TripAdvertisementContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Locations> Locations { get; set; }
        public DbSet<FeedBacks> FeedBacks { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}

        //public System.Data.Entity.DbSet<TripAdvertisement.Models.RoleViewModel> RoleViewModels { get; set; }
    }
}