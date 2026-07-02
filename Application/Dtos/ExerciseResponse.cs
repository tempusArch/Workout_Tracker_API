namespace WorkoutTrackerApi.Application;

public class ExerciseResponse {
    public string Name {get; set;}
    public string Type {get; set;}
    public string Description {get; set;}

    public int Sets {get; set;}
    public int Repetitions {get; set;}
    public double Weight {get; set;}

}