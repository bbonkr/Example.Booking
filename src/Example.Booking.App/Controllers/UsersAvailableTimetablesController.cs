using Example.Booking.App.Models;
using Example.Booking.Data;
using Example.Booking.Entities;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Models;
using kr.bbon.AspNetCore.Mvc;
using kr.bbon.Core;
using kr.bbon.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Example.Booking.Extensions;

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

    /// <summary>
    /// Add timetable item
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpPost]
    [ProducesResponseType(typeof(AvailableTimeTableModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddTimetable([FromRoute] Guid userId, [FromBody] AddAvailableTimeTableModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Payload is invaild");
        }

        var checkUserExists = await dbContext.Users.Where(x => x.Id == userId).AnyAsync();

        if (!checkUserExists)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the user");
        }

        if (!model.Start.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Start must be between 00:00 and 23:59");
        }

        if (!model.End.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "End must be between 00:00 and 23:59");
        }


        var timetables = await dbContext.UserAvailableTimetables
            .Where(x => x.UserId == userId)
            .Where(x => x.DayOfWeek == model.DayOfWeek)
            .OrderBy(x => x.Start)
            .ToListAsync();

        if (timetables.Any())
        {
            foreach (var item in timetables)
            {
                if (model.Start.IsIncluded(item.Start, item.End))
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, $"Start is included {item.Start}~{item.End}");
                }

                if (model.End.IsIncluded(item.Start, item.End))
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, $"End is included {item.Start}~{item.End}");
                }
            }
        }

        var newItem = new UserAvailableTimetable
        {
            Id = Guid.NewGuid(),
            DayOfWeek = model.DayOfWeek,
            Start = model.Start,
            End = model.End,
            UserId = userId,
        };

        var addedEntry = dbContext.UserAvailableTimetables.Add(newItem);
        var addedEntryId = addedEntry.Entity.Id;

        await dbContext.SaveChangesAsync();

        var result = new AvailableTimeTableModel
        {
            Id = addedEntryId,
            DayOfWeek = model.DayOfWeek,
            Start = model.Start,
            End = model.End,
        };

        return Created($"/users/{userId}/timetables/{addedEntryId}", result);
    }

    /// <summary>
    /// Update timetable item
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AvailableTimeTableModel), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTimetable([FromRoute] Guid userId, [FromRoute] Guid id, [FromBody] UpdateAvailableTimeTableModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Payload is invaild");
        }

        var checkUserExists = await dbContext.Users.Where(x => x.Id == userId).AnyAsync();

        if (!checkUserExists)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the user");
        }

        if (!model.Start.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Start must be between 00:00 and 23:59");
        }

        if (!model.End.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "End must be between 00:00 and 23:59");
        }

        var currentTimetable = await dbContext.UserAvailableTimetables
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (currentTimetable == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the timetable item");
        }

        var timetables = await dbContext.UserAvailableTimetables
            .Where(x => x.UserId == userId)
            .Where(x => x.DayOfWeek == currentTimetable.DayOfWeek)
            .Where(x => x.Id != currentTimetable.Id)
            .OrderBy(x => x.Start)
            .ToListAsync();

        if (timetables.Any())
        {
            foreach (var item in timetables)
            {
                if (model.Start.IsIncluded(item.Start, item.End))
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, $"Start is included {item.Start}~{item.End}");
                }

                if (model.End.IsIncluded(item.Start, item.End))
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, $"End is included {item.Start}~{item.End}");
                }
            }
        }

        currentTimetable.Start = model.Start;
        currentTimetable.End = model.End;

        await dbContext.SaveChangesAsync();

        var result = new AvailableTimeTableModel
        {
            Id = id,
            DayOfWeek = DayOfWeek.Friday,
            Start = model.Start,
            End = model.End,
        };

        return Accepted($"/users/{userId}/timetables/{id}", result);
    }

    /// <summary>
    /// Delete timetable item
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTimetable([FromRoute] Guid userId, [FromRoute] Guid id)
    {
        var checkUserExists = await dbContext.Users.Where(x => x.Id == userId).AnyAsync();

        if (!checkUserExists)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find user");
        }

        var currentTimetable = await dbContext.UserAvailableTimetables
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (currentTimetable == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the timetable item");
        }

        dbContext.UserAvailableTimetables.Remove(currentTimetable);

        await dbContext.SaveChangesAsync();

        return Accepted();
    }

    [HttpPost("batch")]
    [ProducesResponseType(typeof(BatchAvailableTimeTableResultModel), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Batch([FromRoute] Guid userId, [FromBody] BatchAvailableTimeTableModel model)
    {
        BatchAvailableTimeTableResultModel result = new();
        var checkUserExists = await dbContext.Users.Where(x => x.Id == userId).AnyAsync();

        if (!checkUserExists)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find user");
        }

        if (!model.Add.Any() && !model.Update.Any() && !model.Delete.Any())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Payload is invalid");
        }

        if (model.Add.Any())
        {
            foreach (var item in model.Add)
            {
                if (!item.Start.IsValidTimeOnlyString())
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, "Start must be between 00:00 and 23:59");
                }

                if (!item.End.IsValidTimeOnlyString())
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, "End must be between 00:00 and 23:59");
                }
            }
        }

        if (model.Update.Any())
        {
            foreach (var item in model.Update)
            {
                if (!item.Start.IsValidTimeOnlyString())
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, "Start must be between 00:00 and 23:59");
                }

                if (!item.End.IsValidTimeOnlyString())
                {
                    throw new ApiException(StatusCodes.Status400BadRequest, "End must be between 00:00 and 23:59");
                }
            }
        }

        using (var transaction = dbContext.Database.BeginTransaction())
        {
            try
            {
                if (model.Delete.Any())
                {
                    foreach (var itemToDelete in model.Delete)
                    {
                        var currentTimetable = await dbContext.UserAvailableTimetables
                          .Where(x => x.Id == itemToDelete)
                          .FirstOrDefaultAsync();

                        if (currentTimetable == null)
                        {
                            logger.LogWarning($"Could not find the timetable item. (id={itemToDelete})");
                            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the timetable item");
                        }

                        dbContext.UserAvailableTimetables.Remove(currentTimetable);

                        await dbContext.SaveChangesAsync();

                        result.Deleted.Add(itemToDelete);
                    }
                }

                if (model.Update.Any())
                {
                    foreach (var itemToUpdate in model.Update)
                    {
                        var currentTimetable = await dbContext.UserAvailableTimetables
                           .Where(x => x.Id == itemToUpdate.Id)
                           .FirstOrDefaultAsync();

                        if (currentTimetable == null)
                        {
                            logger.LogWarning($"Could not find the timetable item. id={itemToUpdate.Id}");
                            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the timetable item.");
                        }

                        var timetables = await dbContext.UserAvailableTimetables
                            .Where(x => x.UserId == userId)
                            .Where(x => x.DayOfWeek == currentTimetable.DayOfWeek)
                            .Where(x => x.Id != currentTimetable.Id)
                            .OrderBy(x => x.Start)
                            .ToListAsync();

                        if (timetables.Any())
                        {
                            foreach (var item in timetables)
                            {
                                if (itemToUpdate.Start.IsIncluded(item.Start, item.End))
                                {
                                    throw new ApiException(StatusCodes.Status400BadRequest, $"Start is included {item.Start}~{item.End}");
                                }

                                if (itemToUpdate.End.IsIncluded(item.Start, item.End))
                                {
                                    throw new ApiException(StatusCodes.Status400BadRequest, $"End is included {item.Start}~{item.End}");
                                }
                            }
                        }

                        currentTimetable.Start = itemToUpdate.Start;
                        currentTimetable.End = itemToUpdate.End;

                        await dbContext.SaveChangesAsync();

                        result.Updated.Add(new AvailableTimeTableModel
                        {
                            Id = itemToUpdate.Id,
                            DayOfWeek = itemToUpdate.DayOfWeek,
                            Start = itemToUpdate.Start,
                            End = itemToUpdate.End,
                        });
                    }
                }

                if (model.Add.Any())
                {
                    foreach (var itemToAdd in model.Add)
                    {
                        var timetables = await dbContext.UserAvailableTimetables
                              .Where(x => x.UserId == userId)
                              .Where(x => x.DayOfWeek == itemToAdd.DayOfWeek)
                              .OrderBy(x => x.Start)
                              .ToListAsync();

                        if (timetables.Any())
                        {
                            foreach (var item in timetables)
                            {
                                if (itemToAdd.Start.IsIncluded(item.Start, item.End))
                                {
                                    throw new ApiException(StatusCodes.Status400BadRequest, $"Start is included {item.Start}~{item.End}");
                                }

                                if (itemToAdd.End.IsIncluded(item.Start, item.End))
                                {
                                    throw new ApiException(StatusCodes.Status400BadRequest, $"End is included {item.Start}~{item.End}");
                                }
                            }
                        }

                        var newItem = new UserAvailableTimetable
                        {
                            Id = Guid.NewGuid(),
                            DayOfWeek = itemToAdd.DayOfWeek,
                            Start = itemToAdd.Start,
                            End = itemToAdd.End,
                            UserId = userId,
                        };

                        var addedEntry = dbContext.UserAvailableTimetables.Add(newItem);
                        var addedEntryId = addedEntry.Entity.Id;

                        await dbContext.SaveChangesAsync();

                        result.Added.Add(new AvailableTimeTableModel
                        {
                            Id = addedEntryId,
                            DayOfWeek = itemToAdd.DayOfWeek,
                            Start = itemToAdd.Start,
                            End = itemToAdd.End,
                        });
                    }
                }

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                logger.LogError(ex, ex.Message);

                throw;
            }
        }

        return Accepted(result);
    }

    private readonly AppDbContext dbContext;
    private readonly ILogger logger;
}
