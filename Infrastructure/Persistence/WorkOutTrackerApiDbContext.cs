using WorkoutTrackerApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace WorkoutTrackerApi.Infrastructure;

public class WorkoutTrackerApiDbContext : DbContext {
    public WorkoutTrackerApiDbContext(DbContextOptions<WorkoutTrackerApiDbContext> options) : base(options) {

    }
    public DbSet<User> UserTable {get; set;}
    public DbSet<RefreshToken> RefreshTokenTable {get; set;}
    public DbSet<Exercise> ExerciseTable {get; set;}
    public DbSet<Plan> PlanTable {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Exercise>().HasIndex(p => p.Name);
        modelBuilder.Entity<Exercise>().HasIndex(p => p.Description);

        modelBuilder.Entity<User>()
            .HasMany(u => u.PlanRisuto)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Plan>()
            .HasMany(p => p.ExerciseRisuto)
            .WithMany(c => c.PlanRisuto)
            .UsingEntity<Dictionary<string, object>>(
                "PlanExercise",
                j => j
                    .HasOne<Exercise>()
                    .WithMany()
                    .HasForeignKey("ExerciseId")
                    .OnDelete(DeleteBehavior.Cascade),
                    
                j => j
                    .HasOne<Plan>()
                    .WithMany()
                    .HasForeignKey("PlanId")
                    .OnDelete(DeleteBehavior.Cascade),
                    
                j => {
                    j.HasKey("PlanId", "ExerciseId");
                    j.ToTable("PlanExerciseTable");
                });
    }
    
}