using WorkoutTrackerApi.Domain;

namespace WorkoutTrackerApi.Application;

public class PlanResponse {
    public string Name {get; set;}
    public string Description {get; set;}
    
    public DateTime StartTime {get; set;}
    public TimeSpan Duration {get; set;}
    public PlanState PlanState {get; set;}

    public List<ExerciseResponse> ExerciseRisuto {get; set;} = new List<ExerciseResponse>();
}