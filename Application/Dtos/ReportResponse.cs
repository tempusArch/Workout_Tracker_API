namespace WorkoutTrackerAPI.Application;

public class ReportResonse {
    public int CompletedPlans {get; set;}
    public int TotalPlans {get; set;}
   
    public double CompletionRate => TotalPlans > 0 ? Math.Round((double) CompletedPlans / TotalPlans * 100, 2) : 0;
}