using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);

        //     modelBuilder.Entity<Role>();
        // }

        // public DbSet<Activity> Activites { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }
        // public DbSet<Employee> Employee { get; set; }
        public DbSet<TrackingUserEditActivity> TrackingUserEditActivity { get; set; }
        public DbSet<SessionActivity> SessionActivity { get; set; }
        public DbSet<InactiveSessionActivity> InactiveSessionActivity { get; set; }
        public DbSet<Plant> Plant { get; set; }
        public DbSet<TrackingPlantActivity> TrackingPlantActivity { get; set; }
        public DbSet<ProductManagement> ProductManagement { get; set; }
        public DbSet<TrackingProductActivity> TrackingProductActivity { get; set; }
        public DbSet<ProductMedia> ProductMedia { get; set; }
        public DbSet<TrackingProductMediaActivity> TrackingProductMediaActivity { get; set; }
        public DbSet<ProductPlantMapping> ProductPlantMapping { get; set; }
        public DbSet<TrackingProductPlantMapActivity> TrackingProductPlantMapActivity { get; set; }
        public DbSet<RequestProductAccess> RequestProductAccess { get; set; }
        public DbSet<QrManagement> QrManagement { get; set; }
        public DbSet<TrackingQrManagement> TrackingQrManagement { get; set; }
        public DbSet<QrReadActivity> QrReadActivity { get; set; }
        public DbSet<UserInformation> UserInformation { get; set; }
        public DbSet<TrackerEmail> TrackerEmail { get; set; }
        public DbSet<TrackerOtp> TrackerOtp { get; set; }
        public DbSet<CounterfietManagement> CounterfietManagement { get; set; }
        public DbSet<TrackingCounterfeitManagement> TrackingCounterfeitManagement { get; set; }
        public DbSet<SmtpConfig> SmtpConfig { get; set; }
        public DbSet<TrackingEditSmtpConfig> TrackingEditSmtpConfig { get; set; }
        public DbSet<RateLimits> RateLimits { get; set; }
        public DbSet<PlantKeyManagement> PlantKeyManagement { get; set; }
        public DbSet<TrackingPlantKeyManagement> TrackingPlantKeyManagement { get; set; }
        public DbSet<PlantSessionManagement> PlantSessionManagement { get; set; }
        public DbSet<NotificationTypeManagement> NotificationTypeManagement { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<NotificationActivity> NotificationActivity { get; set; }
        public DbSet<TrackingNotificationManagement> TrackingNotificationManagement { get; set; }
        public DbSet<TrackingNotification> TrackingNotification { get; set; }
        public DbSet<TrackingApi> TrackingApi { get; set; }
        public DbSet<TrackingRateLimit> TrackingRateLimit { get; set; }
        public DbSet<TrackingActivity> TrackingActivity { get; set; }
        public DbSet<RangeTable> RangeTable { get; set; }
    }
}