namespace WorkoutTrackerApi.Domain;

public class Plan {
    public int Id {get; set;}

    public string Name {get; set;}
    public string Description {get; set;}
    
    public DateTime StartTime {get; set;}
    public TimeSpan Duration {get; set;}
    public PlanState PlanState {get; set;} = PlanState.Pending;

    public int UserId {get; set;}
    public User User {get; set;}

    public List<Exercise> ExerciseRisuto {get; set;} = new List<Exercise>();
}