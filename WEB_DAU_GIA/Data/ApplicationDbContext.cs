using Microsoft.EntityFrameworkCore;
using WEB_DAU_GIA.Models.Entities;

namespace WEB_DAU_GIA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }  // ❌ THIẾU: Thêm Roles
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionItem> AuctionItems { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // ❌ THIẾU: OnModelCreating - Rất quan trọng!
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============ USER CONFIGURATION ============
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                // Unique constraints
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();

                // Default values
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============ ROLE CONFIGURATION ============
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.HasIndex(e => e.Name).IsUnique();

                // Seed default roles
                entity.HasData(
      new Role
      {
          RoleId = 1,
          Name = "Admin",
          Description = "Administrator",
          CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)  // ✅ Giá trị cố định
      },
      new Role
      {
          RoleId = 2,
          Name = "User",
          Description = "Regular User",
          CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)  // ✅ Giá trị cố định
      }
  );
            });

            // ============ AUCTION CONFIGURATION ============
            modelBuilder.Entity<Auction>(entity =>
            {
                entity.HasKey(e => e.AuctionId);

                // Indexes
                entity.HasIndex(e => e.SellerId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsLive);
                entity.HasIndex(e => e.EndTime);

                // Decimal precision
                entity.Property(e => e.StartingBid)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.WinningBid)
                    .HasColumnType("decimal(18,2)");

                // Default values
                entity.Property(e => e.Status)
                    .HasDefaultValue("pending");

                entity.Property(e => e.IsLive)
                    .HasDefaultValue("no");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(e => e.Seller)
                    .WithMany(u => u.Auctions)
                    .HasForeignKey(e => e.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);  // Không xóa auction khi xóa seller

                entity.HasOne(e => e.Winner)
                    .WithMany()
                    .HasForeignKey(e => e.WinnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============ AUCTION ITEM CONFIGURATION ============
            modelBuilder.Entity<AuctionItem>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.HasIndex(e => e.AuctionId);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(e => e.Auction)
                    .WithMany(a => a.AuctionItems)
                    .HasForeignKey(e => e.AuctionId)
                    .OnDelete(DeleteBehavior.Cascade);  // Xóa items khi xóa auction
            });

            // ============ BID CONFIGURATION ============
            modelBuilder.Entity<Bid>(entity =>
            {
                entity.HasKey(e => e.BidId);

                // Indexes for performance
                entity.HasIndex(e => e.AuctionId);
                entity.HasIndex(e => e.BidderId);
                entity.HasIndex(e => new { e.AuctionId, e.BidAmount });
                entity.HasIndex(e => e.Status);

                // Decimal precision
                entity.Property(e => e.BidAmount)
                    .HasColumnType("decimal(18,2)");

                // Default values
                entity.Property(e => e.Status)
                    .HasDefaultValue("active");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(e => e.Auction)
                    .WithMany(a => a.Bids)
                    .HasForeignKey(e => e.AuctionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Bidder)
                    .WithMany(u => u.Bids)
                    .HasForeignKey(e => e.BidderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============ PAYMENT CONFIGURATION ============
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);

                // Indexes
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.AuctionId);
                entity.HasIndex(e => e.PaymentStatus);
                entity.HasIndex(e => e.Type);

                // Decimal precision
                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18,2)");

                // Default values
                entity.Property(e => e.PaymentMethod)
                    .HasDefaultValue("Card");

                entity.Property(e => e.PaymentStatus)
                    .HasDefaultValue("pending");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Payments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Auction)
                    .WithMany(a => a.Payments)
                    .HasForeignKey(e => e.AuctionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============ NOTIFICATION CONFIGURATION ============
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId);

                // Indexes
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedAt);

                // Default values
                entity.Property(e => e.IsRead)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);  // Xóa notifications khi xóa user
            });

            // ============ QUERY FILTERS (Optional) ============
            // Auto-filter soft-deleted records (if implementing soft delete)
            // modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        }

        // ❌ THIẾU: Override SaveChanges để tự động cập nhật UpdatedAt
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is User user)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is Auction auction)
                {
                    auction.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is Payment payment)
                {
                    payment.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}