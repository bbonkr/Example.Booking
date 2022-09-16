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
                AvailableTimeTables = x.AvailableTimeTables.Select(t => new AvailableTimeTableModel
                {
                    Id = t.Id,
                    DayOfWeek = t.DayOfWeek,
                    Start = t.Start,
                    End = t.End,
                }).ToList(),
                DateTimeOverrides = x.DateTimeOverrides.Select(o => new TimeTableOverrideModel
                {
                    Id = o.Id,
                    Date = o.Date,
                    Start = o.Start,
                    End = o.End,
                }).ToList(),
                ApprovedAppointments = x.ApprovedAppointments.Select(a => new AppointmentModel
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
                RequestedAppointments = x.RequestedAppointments.Select(a => new AppointmentModel
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
                AvailableTimeTables = x.AvailableTimeTables.Select(t => new AvailableTimeTableModel
                {
                    Id = t.Id,
                    DayOfWeek = t.DayOfWeek,
                    Start = t.Start,
                    End = t.End,
                }).ToList(),
                DateTimeOverrides = x.DateTimeOverrides.Select(o => new TimeTableOverrideModel
                {
                    Id = o.Id,
                    Date = o.Date,
                    Start = o.Start,
                    End = o.End,
                }).ToList(),
                ApprovedAppointments = x.ApprovedAppointments.Select(a => new AppointmentModel
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
                RequestedAppointments = x.RequestedAppointments.Select(a => new AppointmentModel
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

    private readonly AppDbContext dbContext;
    private readonly ILogger logger;
}