using System.ComponentModel.DataAnnotations;
using WorkoutTrackerApi.Domain;

namespace WorkoutTrackerApi.Application;

public class UpdatePlanDto {
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string Name {get; set;}
    public string Description {get; set;}
    
    [Required]
    public TimeSpan Duration {get; set;}

    [Required]
    public List<int> ExerciseIdRisuto {get; set;} = new();
}