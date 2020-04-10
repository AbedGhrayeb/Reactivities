using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Persistence
{
    public class DataContext:IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
            
        }
        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Value>()
                .HasData(
                    new Value{Id=1,Name="Value 101"},
                    new Value{Id=2,Name="Value 102"},
                    new Value{Id=3,Name="Value 103"}
                );
              builder.Entity<UserActivity>(x=>
                x.HasKey(ua=>new{ua.UserId,ua.ActivityId}));

            builder.Entity<UserActivity>()
                .HasOne(u => u.User)
                .WithMany(ua => ua.UserActivities)
                .HasForeignKey(u => u.UserId);
            builder.Entity<UserActivity>()
                .HasOne(a => a.Activity)
                .WithMany(ua => ua.UserActivities)
                .HasForeignKey(a => a.ActivityId);

        }
    }
}
