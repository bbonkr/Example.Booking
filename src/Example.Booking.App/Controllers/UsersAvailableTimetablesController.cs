using Example.Booking.App.Models;
using Example.Booking.Data;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Models;
using kr.bbon.AspNetCore.Mvc;
using kr.bbon.Core;
using kr.bbon.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.Booking.App.Controllers;

/// <summary>
/// User available timetable
/// </summary>
[ApiController]
[ApiVersion(DefaultValues.ApiVersion)]
[Route("[area]/v{version:apiVersion}/users/{userId:guid}/timetables")]
[Area(DefaultValues.AreaName)]
[Produces("application/json")]
public class UsersAvailableTimetablesController : ApiControllerBase
{
    public UsersAvailableTimetablesController(
        AppDbContext dbContext,
        ILogger<UsersAvailableTimetablesController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Get available timetables for user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserAvailableTimetableModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTimetables([FromRoute] Guid userId)
    {
        if (!dbContext.Users.Where(x => x.Id == userId).Any())
        {
            throw new ApiException(StatusCodes.Status404NotFound, "User could not find");
        }

        var result = await dbContext.UserAvailableTimetables
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.Start)
            .Select(x => new UserAvailableTimetableModel
            {
                Id = x.Id,
                DayOfWeek = x.DayOfWeek,
                Start = x.Start,
                End = x.End,
            })
            .AsNoTracking()
            .ToListAsync();

        return Ok(result);
    }





    private readonly AppDbContext dbContext;
    private readonly ILogger logger;
}
