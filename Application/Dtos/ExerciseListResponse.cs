using WorkoutTrackerApi.Domain;

namespace WorkoutTrackerApi.Application;

public class ExerciseListResponse {
    public IEnumerable<ExerciseResponse> Items {get; set;} = new List<ExerciseResponse>();
    public int TotalCount => Items.Count();
}