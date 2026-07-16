using WorkoutTrackerApi.Domain;
using WorkoutTrackerApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using WorkoutTrackerAPI.Application;

namespace WorkoutTrackerApi.Application;

public class PlanService {
    private readonly WorkoutTrackerApiDbContext _context;
    private readonly IMapper _mapper;

    public PlanService(WorkoutTrackerApiDbContext context, IMapper mapper) {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PlanResponse> GetOnePlan(int planId, int userId, CancellationToken cancellationToken) {
        var theOne = await _context.PlanTable
            .AsNoTracking()
            .Where(x => x.Id == planId && x.UserId == userId)
            .Select(x => new PlanResponse {
                Name = x.Name,
                Description = x.Description,

                StartTime = x.StartTime,
                Duration = x.Duration,
                PlanState = x.PlanState,

                ExerciseRisuto = x.ExerciseRisuto
                    .Select(e => new ExerciseResponse {
                        Name = e.Name,
                        Type = e.Type,
                        Description = e.Description,

                        Sets = e.Sets,
                        Repetitions = e.Repetitions,
                        Weight = e.Weight
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (theOne == null)
            throw new NotFoundException("Plan not found");

        return theOne;
    }

    public async Task<IEnumerable<PlanResponse>> GetOneUsersAllPlans(int userId, CancellationToken cancellationToken) {
        var result = await _context.PlanTable
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new PlanResponse {
                Name = x.Name,
                Description = x.Description,

                StartTime = x.StartTime,
                Duration = x.Duration,
                PlanState = x.PlanState,

                ExerciseRisuto = x.ExerciseRisuto
                    .Select(e => new ExerciseResponse {
                        Name = e.Name,
                        Type = e.Type,
                        Description = e.Description,

                        Sets = e.Sets,
                        Repetitions = e.Repetitions,
                        Weight = e.Weight
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return result;    
    }
    
    public async Task<PlanResponse> CreatePlan(CreatePlanDto dto, int userId, CancellationToken cancellationToken) {
        var exerciseRisuto = await _context.ExerciseTable
            .Where(e => dto.ExerciseIdRisuto.Contains(e.Id))
            .ToListAsync(cancellationToken);

        if (!exerciseRisuto.Any())
            throw new NotFoundException("Exercise not found");

        var newPlan = _mapper.Map<Plan>(dto);
        newPlan.ExerciseRisuto = exerciseRisuto;
        newPlan.UserId = userId;

        _context.PlanTable.Add(newPlan);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PlanResponse>(newPlan);
    }

    public async Task<PlanResponse> UpdatePlan(CreatePlanDto dto, int userId, int planId, CancellationToken cancellationToken) {
        var theOne = await _context.PlanTable
            .Include(p => p.ExerciseRisuto)
            .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId, cancellationToken);

        if (theOne == null)
            throw new NotFoundException("Plan not found");

        var exerciseRisuto = await _context.ExerciseTable
            .Where(e => dto.ExerciseIdRisuto.Contains(e.Id))
            .ToListAsync(cancellationToken);

        if (!exerciseRisuto.Any())
            throw new NotFoundException("Exercise not found");

        theOne.ExerciseRisuto.Clear();
        _mapper.Map(dto, theOne);
        theOne.ExerciseRisuto = exerciseRisuto;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PlanResponse>(theOne);
    }

    public async Task<bool> DeletePlan(int planId, int userId, CancellationToken cancellationToken) {
        var theOne = await _context.PlanTable
            .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId, cancellationToken);

        if (theOne == null)
            throw new NotFoundException("Plan not found");

        _context.PlanTable.Remove(theOne);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SchedulePlan(int planId, int userId, DateTime dateTime, CancellationToken cancellationToken) {
        var theOne = await _context.PlanTable
            .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId, cancellationToken);

        if (theOne == null)
            throw new NotFoundException("Plan not found");

        theOne.StartTime = dateTime;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ChangePlanState(int planId, int userId, PlanState planState, CancellationToken cancellationToken) {
        var theOne = await _context.PlanTable
            .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId, cancellationToken);

        if (theOne == null)
            throw new NotFoundException("Plan not found");

        theOne.PlanState = planState;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<PlanResponse>> GetPlansByState(int userId, PlanState planState,CancellationToken cancellationToken) {
        var result = await _context.PlanTable
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.PlanState == planState)
            .OrderByDescending(x => x.StartTime)
            .Select(x => new PlanResponse {
                Name = x.Name,
                Description = x.Description,

                StartTime = x.StartTime,
                Duration = x.Duration,
                PlanState = x.PlanState,

                ExerciseRisuto = x.ExerciseRisuto
                    .Select(e => new ExerciseResponse {
                        Name = e.Name,
                        Type = e.Type,
                        Description = e.Description,

                        Sets = e.Sets,
                        Repetitions = e.Repetitions,
                        Weight = e.Weight
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return result;    
    }

    public async Task<IEnumerable<PlanResponse>> GetPlansByDateRange(int userId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken) {
        IQueryable<Plan> source = _context.PlanTable
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (startDate.HasValue)
            source = source.Where(x => x.StartTime >= startDate);

        if (endDate.HasValue)
            source = source.Where(x => x.StartTime <= endDate);

        var result = await source
            .Select(x => new PlanResponse {
                Name = x.Name,
                Description = x.Description,

                StartTime = x.StartTime,
                Duration = x.Duration,
                PlanState = x.PlanState,

                ExerciseRisuto = x.ExerciseRisuto
                    .Select(e => new ExerciseResponse {
                        Name = e.Name,
                        Type = e.Type,
                        Description = e.Description,

                        Sets = e.Sets,
                        Repetitions = e.Repetitions,
                        Weight = e.Weight
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    

        return result;    
    }

    public async Task<ReportResonse> GetUserPlanReport(int userId, CancellationToken cancellationToken) {
        int totalCount = await _context.PlanTable
            .CountAsync(p => p.UserId == userId, cancellationToken);
               
        int completedCount = await _context.PlanTable
            .CountAsync(p => 
                p.UserId == userId && 
                p.PlanState == PlanState.Completed, 
                cancellationToken
            );

        return new ReportResonse {
            TotalPlans = totalCount,
            CompletedPlans = completedCount,
        };
    }
}