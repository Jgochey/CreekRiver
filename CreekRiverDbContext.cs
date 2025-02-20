using Microsoft.EntityFrameworkCore;
using CreekRiver.Models;

public class CreekRiverDbContext : DbContext
{

  public DbSet<Reservation> Reservations { get; set; }
  public DbSet<UserProfile> UserProfiles { get; set; }
  public DbSet<Campsite> Campsites { get; set; }
  public DbSet<CampsiteType> CampsiteTypes { get; set; }

  public CreekRiverDbContext(DbContextOptions<CreekRiverDbContext> context) : base(context)
  {

  }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // seed data with campsite types
    modelBuilder.Entity<CampsiteType>().HasData(new CampsiteType[]
    {
        new CampsiteType {Id = 1, CampsiteTypeName = "Tent", FeePerNight = 15.99M, MaxReservationDays = 7},
        new CampsiteType {Id = 2, CampsiteTypeName = "RV", FeePerNight = 26.50M, MaxReservationDays = 14},
        new CampsiteType {Id = 3, CampsiteTypeName = "Primitive", FeePerNight = 10.00M, MaxReservationDays = 3},
        new CampsiteType {Id = 4, CampsiteTypeName = "Hammock", FeePerNight = 12M, MaxReservationDays = 7}
    });

    modelBuilder.Entity<Campsite>().HasData(new Campsite[]
    {
        new Campsite {Id = 1, CampsiteTypeId = 1, Nickname = "Barred Owl", ImageUrl="https://tnstateparks.com/assets/images/content-images/campgrounds/249/colsp-area2-site73.jpg"},
        new Campsite {Id = 2, CampsiteTypeId = 2, Nickname = "Waterfowl", ImageUrl="https://tnstateparks.com/assets/images/content-images/campgrounds/249/colsp-area2-site73.jpg"},
        new Campsite {Id = 3, CampsiteTypeId = 3, Nickname = "Spinning Crane", ImageUrl="https://tnstateparks.com/assets/images/content-images/campgrounds/249/colsp-area2-site73.jpg"},
        new Campsite {Id = 4, CampsiteTypeId = 4, Nickname = "Flightless Bird", ImageUrl="https://tnstateparks.com/assets/images/content-images/campgrounds/249/colsp-area2-site73.jpg"},
        new Campsite {Id = 5, CampsiteTypeId = 1, Nickname = "Mighty Duck", ImageUrl="https://tnstateparks.com/assets/images/content-images/campgrounds/249/colsp-area2-site73.jpg"},
    });

    modelBuilder.Entity<Reservation>().HasData(new Reservation[]
{
            new Reservation {Id = 1, CampsiteId = 1, UserProfileId = 1, CheckinDate = new DateTime(2025, 1, 2), CheckoutDate = new DateTime(2025, 1, 7)}
});

    modelBuilder.Entity<UserProfile>().HasData(new UserProfile[]
    {
            new UserProfile {Id = 1, FirstName = "John", LastName = "Doe", Email="john.doe@example.com"}
    });
  }
}
