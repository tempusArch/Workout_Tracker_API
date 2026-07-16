using WorkoutTrackerApi.Domain;
using WorkoutTrackerApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace WorkoutTrackerApi.Application;

public class ExerciseService {
    private readonly WorkoutTrackerApiDbContext _context;
    private readonly IMapper _mapper;

    public ExerciseService(WorkoutTrackerApiDbContext context, IMapper mapper) {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ExerciseResponse> GetOneExercise(int exerciseId, CancellationToken cancellationToken) {
        var theOne = await _context.ExerciseTable
            .AsNoTracking()
            .Where(e => e.Id == exerciseId)
            .Select(e => new ExerciseResponse {
                Name = e.Name,
                Type = e.Type,
                Description = e.Description,

                Sets = e.Sets,
                Repetitions = e.Repetitions,
                Weight = e.Weight
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (theOne == null)  
            throw new NotFoundException("Exercise not found");

        return theOne;
    }

    public async Task<ExerciseListResponse> GetAllExercises(string? typeName, string? exerciseName, int page, int limit, CancellationToken cancellationToken) {
        IQueryable<Exercise> source = _context.ExerciseTable.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(typeName))
            source = source.Where(e => e.Type.Contains(typeName));

        if (!string.IsNullOrWhiteSpace(exerciseName)) {
            var nameFiltered = source.Where(p => p.Name.Contains(exerciseName));
            var descriptionFiltered = source.Where(p => p.Description.Contains(exerciseName));

            source = nameFiltered.Union(descriptionFiltered);
        }

        var result = source
            .Select(e => new ExerciseResponse {
                Name = e.Name,
                Type = e.Type,
                Description = e.Description,

                Sets = e.Sets,
                Repetitions = e.Repetitions,
                Weight = e.Weight
            });

        limit = Math.Min(limit, 100);
            
        var arranged = await result
            .OrderBy(p => p.Type)
            .ThenBy(p => p.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return new ExerciseListResponse {Items = arranged};
    }

}