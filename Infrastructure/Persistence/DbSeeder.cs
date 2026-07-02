using WorkoutTrackerApi.Domain;

namespace WorkoutTrackerApi.Infrastructure;

public class DbSeeder {
    private readonly WorkoutTrackerApiDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly IConfiguration _config;
    public DbSeeder(WorkoutTrackerApiDbContext context, PasswordHasher passwordHasher, IConfiguration configuration) {
        _context = context;
        _passwordHasher = passwordHasher;
        _config = configuration;
    }

    /*public async Task SeedAdmin() {
        if (_context.UserTable.Any(u => u.Role == "Admin"))
            return;

        var adminEmail = _config["Admin:Email"];
        var adminPassword = _config["Admin:Password"];

        if (string.IsNullOrEmpty(adminPassword))
            throw new Exception("Missing ADMIN_PASSWORD");

        var theAdminOne = new User {
            Email = adminEmail,
            PasswordHashed = _passwordHasher.HashPassword(adminPassword),
            Role = "Admin",
            Name = "theChosenOne"
        };

        _context.UserTable.Add(theAdminOne);
        await _context.SaveChangesAsync();
    }*/

    public async Task SeedExercise() {
        if (_context.ExerciseTable.Any())
            return;
        
        var exercises = new List<Exercise>() {
			new Exercise {Name = "Bench Press", Type = "Chest", Description = "Lay flat on a bench and push the barbell up", Sets = 3, Repetitions = 10, Weight = 135 },
			new Exercise {Name = "Squats", Type = "Legs", Description = "Stand with the barbell on your shoulders and squat down", Sets = 3, Repetitions = 10, Weight = 225 },
			new Exercise {Name = "Deadlifts", Type = "Back", Description = "Stand with the barbell on the ground and lift it up", Sets = 3, Repetitions = 10, Weight = 315 },
			new Exercise {Name = "Chest Pull", Type = "Back", Description = "Lie across a bench or flat on it and hold one dumbbell with both hands under the top plate.", Sets = 4, Repetitions = 10, Weight = 70 },
			new Exercise {Name = "Pull-ups", Type = "Back", Description = "Grab a pull-up bar with hands slightly wider than shoulder-width", Sets = 4, Repetitions = 8, Weight = 0 },
			new Exercise {Name = "Incline Dumbbell Press", Type = "Chest", Description = "Set an incline bench to 30–45 degrees and lie back with dumbbells at shoulder level. With feet planted and core tight, press the weights directly up until arms are fully extended, then slowly lower them to upper chest level, maintaining a controlled motion to target the upper pectorals", Sets = 4, Repetitions = 10, Weight = 30 },
		};

        _context.ExerciseTable.AddRange(exercises);
        await _context.SaveChangesAsync();
    }
}