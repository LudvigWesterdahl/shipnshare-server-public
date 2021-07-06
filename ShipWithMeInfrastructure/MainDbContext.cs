using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShipWithMeCore.SharedKernel;
using ShipWithMeInfrastructure.Models;

namespace ShipWithMeInfrastructure
{
    public sealed class MainDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<UserBlock> UserBlocks { get; set; }

        public DbSet<InternalNotification> InternalNotifications { get; set; }

        public DbSet<InternalNotificationMessage> InternalNotificationMessages { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<ReportedPost> ReportedPosts { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<ChatRequest> ChatRequests { get; set; }

        public DbSet<UserReview> UserReviews { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
            // empty
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sets the decimal precision for all decimal values stored in the database.
            foreach (var entityTypes in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityTypes.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetPrecision(38);
                        property.SetScale(6);
                    }
                }
            }

            var timeZoneInfoValueConverter = new ValueConverter<TimeZoneInfo, string>(
                tzi => tzi.Id,
                id => TimeZoneInfo.FindSystemTimeZoneById(id));

            var uriValueConverter = new ValueConverter<Uri, string>(
                uri => uri.AbsoluteUri,
                absUri => new Uri(absUri));

            modelBuilder.Entity<Post>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

                b.HasKey(p => p.Id);

                b.Property(p => p.CreatedAt)
                    .HasConversion(new DateTimeToStringConverter());

                b.Property(p => p.CreatedAtTimeZone)
                    .HasConversion(timeZoneInfoValueConverter);

                b.Property(p => p.StoreOrProductUri)
                    .HasConversion(uriValueConverter);

                b.HasOne(p => p.Owner)
                    .WithMany(a => a.Posts)
                    .HasForeignKey(p => p.OwnerId);
            });

            modelBuilder.Entity<Tag>(b =>
            {
                b.HasKey(t => t.Id);

                b.HasMany(t => t.Posts)
                    .WithMany(p => p.Tags);
            });

            modelBuilder.Entity<UserBlock>(b =>
            {
                b.HasKey(ub => ub.Id);

                b.HasOne(ub => ub.User)
                    .WithMany(u => u.UserBlocks)
                    .HasForeignKey(bu => bu.UserId);

                b.Property(ub => ub.From)
                    .HasConversion(new DateTimeToStringConverter());

                b.Property(ub => ub.To)
                    .HasConversion(new DateTimeToStringConverter());
            });

            modelBuilder.Entity<InternalNotification>(b =>
            {
                b.HasKey(i => i.Id);

                b.Property(i => i.CreatedAt)
                    .HasConversion(new DateTimeToStringConverter());
            });

            modelBuilder.Entity<InternalNotificationMessage>(b =>
            {
                b.HasKey(inm => new { inm.InternalNotificationId, inm.LanguageCode });

                b.HasOne(inm => inm.InternalNotification)
                    .WithMany(inm => inm.InternalNotificationMessages)
                    .HasForeignKey(inm => inm.InternalNotificationId);
            });

            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.HasKey(rt => rt.Token);

                b.Property(rt => rt.Expires)
                    .HasConversion(new DateTimeToStringConverter());

                b.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId);
            });

            modelBuilder.Entity<ReportedPost>(b =>
            {
                b.HasKey(rp => rp.Id);

                b.HasOne(rp => rp.Post)
                    .WithMany()
                    .HasForeignKey(rp => rp.PostId)
                    .OnDelete(DeleteBehavior.ClientCascade);

                b.HasOne(rp => rp.User)
                    .WithMany()
                    .HasForeignKey(rp => rp.UserId);

                b.Property(rp => rp.CreatedAt)
                    .HasConversion(new DateTimeToStringConverter());
            });

            modelBuilder.Entity<Chat>(b =>
            {
                b.HasKey(c => c.Id);

                b.Property(c => c.CreatedAt)
                    .HasConversion(new DateTimeToStringConverter());

                b.HasOne(c => c.Post)
                    .WithMany()
                    .HasForeignKey(c => c.PostId);
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasKey(cm => cm.Id);

                b.Property(cm => cm.CreatedAt)
                    .HasConversion(new DateTimeToStringConverter());

                b.HasOne(cm => cm.FromUser)
                    .WithMany()
                    .HasForeignKey(cm => cm.FromUserId);

                b.HasOne(cm => cm.Chat)
                    .WithMany(c => c.ChatMessages)
                    .HasForeignKey(cm => cm.ChatId);
            });

            modelBuilder.Entity<ChatRequest>(b =>
            {
                b.HasKey(cr => cr.Id);

                b.Property(cr => cr.CreatedAt)
                    .HasConversion(new DateTimeToStringConverter());

                b.HasOne(cr => cr.FromUser)
                    .WithMany()
                    .HasForeignKey(cm => cm.FromUserId);

                b.HasOne(cr => cr.ToUser)
                    .WithMany()
                    .HasForeignKey(cr => cr.ToUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(cr => cr.Chat)
                    .WithMany(c => c.ChatRequests)
                    .HasForeignKey(cr => cr.ChatId);
            });

            modelBuilder.Entity<User>(b => {
                b.Property(u => u.ResetPasswordKeyCreatedAt)
                    .HasConversion(new DateTimeToStringConverter());
            });

            modelBuilder.Entity<ChatUser>(b =>
            {
                b.HasKey(cu => new { cu.UserId, cu.ChatId });

                b.HasOne(cu => cu.User)
                    .WithMany()
                    .HasForeignKey(cu => cu.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(cu => cu.Chat)
                    .WithMany(c => c.ChatUsers)
                    .HasForeignKey(cu => cu.ChatId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
