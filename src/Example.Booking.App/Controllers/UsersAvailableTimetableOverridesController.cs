using Example.Booking.App.Models;
using Example.Booking.Data;
using Example.Booking.Entities;
using Example.Booking.Extensions;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Models;
using kr.bbon.AspNetCore.Mvc;
using kr.bbon.Core;
using kr.bbon.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.Booking.App.Controllers;

/// <summary>
/// User available timetable override
/// </summary>
[ApiController]
[ApiVersion(DefaultValues.ApiVersion)]
[Route("[area]/v{version:apiVersion}/users/{userId:guid}/timetableOverrides")]
[Area(DefaultValues.AreaName)]
[Produces("application/json")]
public class UsersAvailableTimetableOverridesController : ApiControllerBase
{
    public UsersAvailableTimetableOverridesController(
        AppDbContext dbContext,
        ILogger<UsersAvailableTimetableOverridesController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Get user's overrided timetable 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TimeTableOverrideModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOverrides([FromRoute] Guid userId)
    {
        if (!dbContext.Users.Where(x => x.Id == userId).Any())
        {
            throw new ApiException(StatusCodes.Status404NotFound, "User could not find");
        }

        var result = await dbContext.UserTimeTableOverrides
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Date)
                .ThenBy(x => x.Start)
            .Select(x => new TimeTableOverrideModel
            {
                Id = x.Id,
                Date = x.Date,
                Start = x.Start,
                End = x.End,
            })
            .AsNoTracking()
            .ToListAsync();

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TimeTableOverrideModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddOverridedTimetable([FromRoute] Guid userId, [FromBody] AddTimetableOverrideModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Payload is invalid");
        }

        if (!dbContext.Users.Where(x => x.Id == userId).Any())
        {
            throw new ApiException(StatusCodes.Status404NotFound, "User could not find");
        }

        if (!model.Start.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Start must be between 00:00 and 23:59");
        }

        if (!model.End.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "End must be between 00:00 and 23:59");
        }

        var timetables = dbContext.UserTimeTableOverrides
            .Where(x => x.UserId == userId)
            .Where(x => x.Date.Date == model.Date.Date)
            .AsNoTracking();

        if (timetables.Any())
        {
            var isIncluded = false;
            foreach (var timetable in timetables)
            {
                isIncluded = model.Start.IsIncluded(timetable.Start, timetable.End) || model.End.IsIncluded(timetable.Start, timetable.End);
                if (isIncluded) { break; }
            }

            if (isIncluded)
            {
                throw new ApiException(StatusCodes.Status406NotAcceptable, "Timetable has been duplicated");
            }
        }

        var newItem = new UserTimeTableOverride
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = model.Date,
            Start = model.Start,
            End = model.End,
        };

        var addedEntry = dbContext.UserTimeTableOverrides.Add(newItem);

        await dbContext.SaveChangesAsync();

        var addedId = addedEntry.Entity.Id;

        var result = new TimeTableOverrideModel
        {
            Id = addedId,
            Date = newItem.Date,
            Start = newItem.Start,
            End = newItem.End,
        };

        return Created($"/users/{userId}/timetableOverrides/{addedId}", result);
    }

    /// <summary>
    /// Update overrided timetable
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TimeTableOverrideModel), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOverridedTimetable([FromRoute] Guid userId, [FromRoute] Guid id, [FromBody] UpdateTimetableOverrideModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Payload is invalid");
        }

        if (!dbContext.Users.Where(x => x.Id == userId).Any())
        {
            throw new ApiException(StatusCodes.Status404NotFound, "User could not find");
        }

        if (!model.Start.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Start must be between 00:00 and 23:59");
        }

        if (!model.End.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "End must be between 00:00 and 23:59");
        }

        var current = await dbContext.UserTimeTableOverrides
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (current == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the overrided timetable");
        }

        var timetables = dbContext.UserTimeTableOverrides
            .Where(x => x.UserId == userId)
            .Where(x => x.Date.Date == model.Date.Date)
            .AsNoTracking();

        if (timetables.Any())
        {
            var isIncluded = false;
            foreach (var timetable in timetables)
            {
                isIncluded = model.Start.IsIncluded(timetable.Start, timetable.End) || model.End.IsIncluded(timetable.Start, timetable.End);
                if (isIncluded) { break; }
            }

            if (isIncluded)
            {
                throw new ApiException(StatusCodes.Status406NotAcceptable, "Timetable has been duplicated");
            }
        }

        current.Date = model.Date;
        current.Start = model.Start;
        current.End = model.End;

        await dbContext.SaveChangesAsync();

        var result = new TimeTableOverrideModel
        {
            Id = current.Id,
            Date = current.Date,
            Start = current.Start,
            End = current.End,
        };

        return Accepted($"/users/{userId}/timetableOverrides/{result.Id}", result);
    }

    /// <summary>
    /// Delete overrided titmetable
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid userId, [FromRoute] Guid id)
    {
        if (!dbContext.Users.Where(x => x.Id == userId).Any())
        {
            throw new ApiException(StatusCodes.Status404NotFound, "User could not find");
        }

        var result = await dbContext.UserTimeTableOverrides
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (result == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find overrided timetable");
        }

        dbContext.UserTimeTableOverrides.Remove(result);

        await dbContext.SaveChangesAsync();

        return Accepted();
    }

    private readonly AppDbContext dbContext;
    private readonly ILogger logger;
}