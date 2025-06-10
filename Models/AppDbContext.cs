using Final.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext() { }  // ✅ Parameterless constructor for EF Core CLI

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<SysAction> SysActions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Token> Tokens { get; set; }

    // DbSet الجديد
    public DbSet<FavList> FavLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-MBMRAMM;Initial Catalog=FinalDb;Integrated Security=True;Trust Server Certificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Role Enum Conversion
        modelBuilder.Entity<User>()
            .Property(u => u.role)
            .HasConversion<string>();

        // Configuring Address as an owned entity
        modelBuilder.Entity<User>().OwnsOne(u => u.addresses);

        // Project Status Enum Conversion
        modelBuilder.Entity<Project>()
            .Property(p => p.status)
            .HasConversion<string>();

        // SysAction relationship
        modelBuilder.Entity<SysAction>()
            .HasOne(sa => sa.user)
            .WithMany()
            .HasForeignKey(sa => sa.userId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project owner relationship
        modelBuilder.Entity<Project>()
            .HasOne(p => p.owner)
            .WithMany()
            .HasForeignKey(p => p.ownerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment relationships
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.commentOwner)
            .WithMany()
            .HasForeignKey(c => c.commentOwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Project)
            .WithMany(p => p.comments)
            .HasForeignKey(c => c.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Like relationship
        modelBuilder.Entity<Like>()
            .HasOne(l => l.likeOwner)
            .WithMany()
            .HasForeignKey(l => l.likeOwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notification relationship
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.project)
            .WithMany()
            .HasForeignKey(n => n.projectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Chat relationships
        modelBuilder.Entity<Chat>()
            .HasOne(c => c.sender)
            .WithMany()
            .HasForeignKey(c => c.senderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Chat>()
            .HasOne(c => c.receiver)
            .WithMany()
            .HasForeignKey(c => c.receiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Message relationship
        modelBuilder.Entity<Message>()
            .HasOne(m => m.messageOwner)
            .WithMany()
            .HasForeignKey(m => m.messageOwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FavList>()
       .HasKey(fl => new { fl.UserId, fl.ProjectId });

        modelBuilder.Entity<FavList>()
            .HasOne(fl => fl.User)
            .WithMany(u => u.FavLists)
            .HasForeignKey(fl => fl.UserId)
            .OnDelete(DeleteBehavior.NoAction);  // أو DeleteBehavior.Restrict

        modelBuilder.Entity<FavList>()
            .HasOne(fl => fl.Project)
            .WithMany(p => p.FavLists)
            .HasForeignKey(fl => fl.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);  // أو DeleteBehavior.Restrict

    }
}
