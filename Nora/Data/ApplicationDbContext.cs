using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nora.Models;

namespace Nora.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
        options)
        : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Channel> Channels { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<CategoryChannel> CategoryChannels { get; set; }
		public DbSet<UserChannel> UserChannels { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define for UserChannel
            modelBuilder.Entity<UserChannel>()
                .HasKey(uc => new { uc.Id, uc.UserId, uc.ChannelId });

            modelBuilder.Entity<UserChannel>()
                .HasOne(uc => uc.User)
                .WithMany(user => user.UserChannels)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserChannel>()
                .HasOne(uc => uc.Channel)
                .WithMany(channel => channel.UserChannels)
                .HasForeignKey(uc => uc.ChannelId);


            // Define for CategoryChannel
            modelBuilder.Entity<CategoryChannel>()
                .HasKey(cc => new { cc.Id, cc.CategoryId, cc.ChannelId });

            modelBuilder.Entity<CategoryChannel>()
                .HasOne(cc => cc.Category)
                .WithMany(category => category.CategoryChannels)
                .HasForeignKey(cc => cc.CategoryId);

            modelBuilder.Entity<CategoryChannel>()
                .HasOne(cc => cc.Channel)
                .WithMany(channel => channel.CategoryChannels)
                .HasForeignKey(cc => cc.ChannelId);
        }

    }

}
