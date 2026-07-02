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
            .FirstOrDefaultAsync(e => e.Id == exerciseId, cancellationToken);   

        if (theOne == null)  
            throw new NotFoundException("Exercise not found");

        return _mapper.Map<ExerciseResponse>(theOne);
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

        if (source == null)
            throw new NotFoundException("No exercise found");

        if (limit > 100) 
            limit = 100;
            
        var arranged = await source
            .OrderBy(p => p.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<IEnumerable<ExerciseResponse>>(arranged);

        return new ExerciseListResponse {Items = result};
    }

}