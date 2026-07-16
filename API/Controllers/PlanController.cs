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
public class PlanController : ControllerBase {
    private readonly PlanService _planService;
    private readonly WorkoutTrackerApiDbContext _context;
    public PlanController(PlanService planService, WorkoutTrackerApiDbContext context) {
        _planService = planService;
        _context = context;
    }

    [HttpGet("{planId}")]
    public async Task<ActionResult<PlanResponse>> GetOnePlan(int planId, CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var result = await _planService.GetOnePlan(planId, int.Parse(userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlanResponse>>> GetOneUsersAllPlans(CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var result = await _planService.GetOneUsersAllPlans(int.Parse(userId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PlanResponse>> CreatePlan(CreatePlanDto dto, CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var result = await _planService.CreatePlan(dto, int.Parse(userId), cancellationToken);
        return Created(string.Empty, result);
    }

    [HttpPut("{planId}")]
    public async Task<ActionResult<PlanResponse>> UpdatePlan(CreatePlanDto dto, int planId, CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var result = await _planService.UpdatePlan(dto, int.Parse(userId), planId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{planId}")]
    public async Task<IActionResult> DeletePlan(int planId, CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        if (await _planService.DeletePlan(planId, int.Parse(userId), cancellationToken))
            return NoContent();

        return NotFound("Plan not found");
    }

    [HttpPut("schedule/{planId}")]
    public async Task<IActionResult> ChangePlanState(int planId, DateTime dateTime, CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        if (await _planService.SchedulePlan(planId, int.Parse(userId), dateTime, cancellationToken))
            return NoContent();

        return NotFound("Plan not found");
    }

    [HttpPut("changeState/{planId}")]
    public async Task<IActionResult> ChangePlanState(int planId, PlanState planState, CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        if (await _planService.ChangePlanState(planId, int.Parse(userId), planState, cancellationToken))
            return NoContent();

        return NotFound("Plan not found");
    }

    [HttpGet("state")]
    public async Task<ActionResult<IEnumerable<PlanResponse>>> GetPlansByState(PlanState planState, CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var result = await _planService.GetPlansByState(int.Parse(userId), planState, cancellationToken);

        return Ok(result);
    }

    [HttpGet("date")]
    public async Task<ActionResult<IEnumerable<PlanResponse>>> GetPlansByDateRange(DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        var result = await _planService.GetPlansByDateRange(int.Parse(userId), startDate, endDate, cancellationToken);

        return Ok(result);
    }

    [HttpGet("report")]
    public async Task<ActionResult> GetUserPlanReport(CancellationToken cancellationToken) {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID claim is missing");

        return Ok(await _planService.GetUserPlanReport(int.Parse(userId), cancellationToken));
    }
}