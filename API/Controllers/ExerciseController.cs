using WorkoutTrackerApi.Infrastructure;
using WorkoutTrackerApi.Domain;
using WorkoutTrackerApi.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WorkoutTrackerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ExerciseController : ControllerBase {
    private readonly ExerciseService _exerciseService;
    private readonly WorkoutTrackerApiDbContext _context;
    public ExerciseController(ExerciseService exerciseService, WorkoutTrackerApiDbContext context) {
        _exerciseService = exerciseService;
        _context = context;
    }

    [HttpGet("{exerciseId}")]
    public async Task<ActionResult<ExerciseResponse>> GetOneExercise(int exerciseId, CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var result = await _exerciseService.GetOneExercise(exerciseId, cancellationToken);

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<ExerciseListResponse>> GetAllExercises(
        CancellationToken cancellationToken,
        [FromQuery] string? typeName,
        [FromQuery] string? exerciseName,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10
    ) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var result = await _exerciseService.GetAllExercises(typeName, exerciseName, page, limit, cancellationToken);

        return Ok(result);
    }
}