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

[ApiController]
[ApiVersion(DefaultValues.ApiVersion)]
[Route(DefaultValues.RouteTemplate)]
[Area(DefaultValues.AreaName)]
[Produces("application/json")]
public class UsersController : ApiControllerBase
{
    public UsersController(
        AppDbContext dbContext,
        ILogger<UsersController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Get users
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await dbContext.Users
            .Include(x => x.AvailableTimeTables)
            .Include(x => x.DateTimeOverrides)
            .Include(x => x.ApprovedAppointments)
                .ThenInclude(x => x.FromUser)
            .Include(x => x.RequestedAppointments)
                .ThenInclude(x => x.ToUser)
            .Select(x => new UserModel
            {
                Id = x.Id,
                Name = x.Name,
                BeforeEventBuffer = x.BeforeEventBuffer,
                AfterEventBuffer = x.AfterEventBuffer,
                AvailableTimeTables = x.AvailableTimeTables
                    .OrderBy(a => a.DayOfWeek)
                    .ThenBy(a => a.Start)
                    .Select(t => new AvailableTimeTableModel
                    {
                        Id = t.Id,
                        DayOfWeek = t.DayOfWeek,
                        Start = t.Start,
                        End = t.End,
                    }).ToList(),
                DateTimeOverrides = x.DateTimeOverrides
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.Start)
                    .Select(o => new TimeTableOverrideModel
                    {
                        Id = o.Id,
                        Date = o.Date,
                        Start = o.Start,
                        End = o.End,
                    }).ToList(),
                ApprovedAppointments = x.ApprovedAppointments
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.Start)
                    .Select(a => new AppointmentModel
                    {
                        Id = a.Id,
                        Date = a.Date,
                        Start = a.Start,
                        End = a.End,
                        FromUser = new UserModel
                        {
                            Id = a.FromUser.Id,
                            Name = a.FromUser.Name,
                        },
                    }).ToList(),
                RequestedAppointments = x.RequestedAppointments
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.Start)
                    .Select(a => new AppointmentModel
                    {
                        Id = a.Id,
                        Date = a.Date,
                        Start = a.Start,
                        End = a.End,
                        ToUser = new UserModel
                        {
                            Id = a.ToUser.Id,
                            Name = a.ToUser.Name,
                        },
                    }).ToList(),
            })
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .ToListAsync();

        return Ok(result);
    }

    /// <summary>
    /// Get users
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser([FromRoute] Guid id)
    {
        var result = await dbContext.Users
            .Include(x => x.AvailableTimeTables)
            .Include(x => x.DateTimeOverrides)
            .Include(x => x.ApprovedAppointments)
                .ThenInclude(x => x.FromUser)
            .Include(x => x.RequestedAppointments)
                .ThenInclude(x => x.ToUser)
            .Where(x => x.Id == id)
            .Select(x => new UserModel
            {
                Id = x.Id,
                Name = x.Name,
                BeforeEventBuffer = x.BeforeEventBuffer,
                AfterEventBuffer = x.AfterEventBuffer,
                AvailableTimeTables = x.AvailableTimeTables
                    .OrderBy(a => a.DayOfWeek)
                    .ThenBy(a => a.Start)
                    .Select(t => new AvailableTimeTableModel
                    {
                        Id = t.Id,
                        DayOfWeek = t.DayOfWeek,
                        Start = t.Start,
                        End = t.End,
                    }).ToList(),
                DateTimeOverrides = x.DateTimeOverrides
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.Start)
                    .Select(o => new TimeTableOverrideModel
                    {
                        Id = o.Id,
                        Date = o.Date,
                        Start = o.Start,
                        End = o.End,
                    }).ToList(),
                ApprovedAppointments = x.ApprovedAppointments
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.Start)
                    .Select(a => new AppointmentModel
                    {
                        Id = a.Id,
                        Date = a.Date,
                        Start = a.Start,
                        End = a.End,
                        FromUser = new UserModel
                        {
                            Id = a.FromUser.Id,
                            Name = a.FromUser.Name,
                        },
                    }).ToList(),
                RequestedAppointments = x.RequestedAppointments
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.Start)
                    .Select(a => new AppointmentModel
                    {
                        Id = a.Id,
                        Date = a.Date,
                        Start = a.Start,
                        End = a.End,
                        ToUser = new UserModel
                        {
                            Id = a.ToUser.Id,
                            Name = a.ToUser.Name,
                        },
                    }).ToList(),
            })
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (result == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "User could not find");
        }

        return Ok(result);
    }

    /// <summary>
    /// Add user
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpPost]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddUser([FromBody] AddUserModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Payload is invalid");
        }

        var user = await dbContext.Users
            .Where(x => x.Name == model.Name)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (user != null)
        {
            throw new ApiException(StatusCodes.Status406NotAcceptable, "The user name has to be unique");
        }

        user = new Entities.User
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            BeforeEventBuffer = model.BeforeEventBuffer,
            AfterEventBuffer = model.AfterEventBuffer,
        };

        var addedEntry = dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        var addedUserId = addedEntry.Entity.Id;
        var result = new UserModel
        {
            Id = addedUserId,
            Name = model.Name,
            BeforeEventBuffer = model.BeforeEventBuffer,
            AfterEventBuffer = model.AfterEventBuffer,
        };

        return Created($"/users/{addedUserId}", result);
    }

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Payload is invalid");
        }

        var hasSameNameUser = await dbContext.Users
            .Where(x => x.Name == model.Name)
            .Where(x => x.Id != id)
            .AsNoTracking()
            .AnyAsync();

        if (hasSameNameUser)
        {
            throw new ApiException(StatusCodes.Status406NotAcceptable, "The user name has to be unique");
        }

        var user = await dbContext.Users
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new ApiException(StatusCodes.Status406NotAcceptable, "Could not find the user");
        }

        user.Name = model.Name;
        user.BeforeEventBuffer = model.BeforeEventBuffer;
        user.AfterEventBuffer = model.AfterEventBuffer;

        await dbContext.SaveChangesAsync();

        var result = new UserModel
        {
            Id = user.Id,
            Name = user.Name,
            BeforeEventBuffer = model.BeforeEventBuffer,
            AfterEventBuffer = model.AfterEventBuffer,
        };

        return Accepted($"/users/{user.Id}", result);
    }

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        var hasAppointment = await dbContext.Appointments
            .Where(x => x.FromUserId == id || x.ToUserId == id)
            .AsNoTracking()
            .AnyAsync();

        if (hasAppointment)
        {
            throw new ApiException(StatusCodes.Status406NotAcceptable, "Could not delete the user who has appointment");
        }

        var user = await dbContext.Users.Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the user");
        }

        using (var transaction = dbContext.Database.BeginTransaction())
        {
            try
            {
                var availableTimetables = dbContext.UserAvailableTimetables
                    .Where(x => x.UserId == id);

                if (availableTimetables.Any())
                {
                    foreach (var timetable in availableTimetables)
                    {
                        dbContext.UserAvailableTimetables.Remove(timetable);
                    }

                    await dbContext.SaveChangesAsync();
                }

                var overriedTimetables = dbContext.UserTimeTableOverrides
                    .Where(x => x.UserId == id);

                if (overriedTimetables.Any())
                {
                    foreach (var timetable in overriedTimetables)
                    {
                        dbContext.UserTimeTableOverrides.Remove(timetable);
                    }

                    await dbContext.SaveChangesAsync();
                }

                dbContext.Users.Remove(user);

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Accepted();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                logger.LogError(ex, ex.Message);

                throw;
            }
        }
    }

    private readonly AppDbContext dbContext;
    private readonly ILogger logger;
}