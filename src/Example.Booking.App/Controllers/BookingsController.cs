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
/// Appointments
/// </summary>
[ApiController]
[ApiVersion(DefaultValues.ApiVersion)]
[Route(DefaultValues.RouteTemplate)]
[Area(DefaultValues.AreaName)]
[Produces("application/json")]
public class BookingsController : ApiControllerBase
{
    public BookingsController(
        AppDbContext dbContext,
        ILogger<BookingsController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Get appointments
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppointmentModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppointments()
    {
        var result = await dbContext.Appointments
            .Include(x => x.FromUser)
            .Include(x => x.ToUser)
            .OrderBy(x => x.Date)
                .ThenBy(x => x.Start)
            .AsNoTracking()
            .Select(x => new AppointmentModel
            {
                Id = x.Id,
                Date = x.Date,
                Start = x.Start,
                End = x.End,
                FromUser = new UserModel
                {
                    Id = x.FromUser.Id,
                    Name = x.FromUser.Name
                },
                ToUser = new UserModel
                {
                    Id = x.ToUser.Id,
                    Name = x.ToUser.Name
                },
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AppointmentModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointment(Guid id)
    {
        var result = await dbContext.Appointments
            .Include(x => x.FromUser)
            .Include(x => x.ToUser)
            .Where(x => x.Id == id)
            .OrderBy(x => x.Date)
                .ThenBy(x => x.Start)
            .AsNoTracking()
            .Select(x => new AppointmentModel
            {
                Id = x.Id,
                Date = x.Date,
                Start = x.Start,
                End = x.End,
                FromUser = new UserModel
                {
                    Id = x.FromUser.Id,
                    Name = x.FromUser.Name
                },
                ToUser = new UserModel
                {
                    Id = x.ToUser.Id,
                    Name = x.ToUser.Name
                },
            })
            .FirstOrDefaultAsync();

        if (result == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the appointment");
        }

        return Ok(result);
    }

    /// <summary>
    /// Make appointment
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpPost]
    [ProducesResponseType(typeof(AppointmentModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MakeAppointment([FromBody] AddAppintmentModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Payload is invalid");
        }

        if (!model.Start.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Start must be between 00:00 and 23:59");
        }

        if (!model.End.IsValidTimeOnlyString())
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "End must be between 00:00 and 23:59");
        }

        var fromUser = await dbContext.Users
            .Where(x => x.Id == model.FromUserId)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (fromUser == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the user who requested.");
        }

        var toUser = await dbContext.Users
            .Where(x => x.Id == model.ToUserId)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (toUser == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the user who makes an appointment with.");
        }

        var dayOfWeek = model.Date.DayOfWeek;

        var overridedSchedules = dbContext.UserTimeTableOverrides
            .Where(x => x.UserId == toUser.Id)
            .Where(x => x.Date.Date == model.Date.Date)
            .AsNoTracking();

        Appointment? appointment = null;

        if (overridedSchedules.Any())
        {
            bool isIncluded = false;
            foreach (var timetable in overridedSchedules)
            {
                isIncluded = model.Start.IsIncluded(timetable.Start, timetable.End);

                if (isIncluded) { break; }
            }

            if (isIncluded)
            {
                appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    Date = model.Date,
                    Start = model.Start,
                    End = model.End,
                    FromUserId = model.FromUserId,
                    ToUserId = model.ToUserId,
                };
            }
        }

        if (appointment == null)
        {
            var timetables = dbContext.UserAvailableTimetables
                .Where(x => x.UserId == toUser.Id)
                .Where(x => x.DayOfWeek == dayOfWeek)
                .AsNoTracking();

            bool isIncludes = false;
            foreach (var timetable in timetables)
            {
                isIncludes = model.Start.IsIncluded(timetable.Start, timetable.End);

                if (isIncludes) { break; }
            }

            if (isIncludes)
            {
                appointment = new Appointment
                {
                    Id = Guid.NewGuid(),
                    Date = model.Date,
                    Start = model.Start,
                    End = model.End,
                    FromUserId = model.FromUserId,
                    ToUserId = model.ToUserId,
                };
            }
        }

        if (appointment == null)
        {
            throw new ApiException(StatusCodes.Status406NotAcceptable, "Requested date and time is unavailable.");
        }

        var appointments = dbContext.Appointments
            .Where(x => x.ToUserId == toUser.Id)
            .Where(x => x.Date.Date == appointment.Date.Date)
            .AsNoTracking();

        if (appointments.Any())
        {
            bool isIncluded = false;
            foreach (var other in appointments)
            {
                var start = other.Start.ToTimeOnly();
                var end = other.End.ToTimeOnly();

                if (start.HasValue && end.HasValue)
                {
                    var startTime = start.Value.AddMinutes(toUser.BeforeEventBuffer ?? 0);
                    var endTime = end.Value.AddMinutes(toUser.AfterEventBuffer ?? 0);

                    isIncluded = appointment.Start.IsIncluded(startTime, endTime) || appointment.End.IsIncluded(startTime, endTime);

                    if (isIncluded) { break; }
                }
                else
                {
                    throw new ApiException(StatusCodes.Status406NotAcceptable, "Appointment data has been corrupted");
                }
            }

            if (isIncluded)
            {
                throw new ApiException(StatusCodes.Status406NotAcceptable, "Requested data and time has been make appointment already");
            }
        }

        var addedEntry = dbContext.Appointments.Add(appointment);
        var newId = addedEntry.Entity.Id;
        await dbContext.SaveChangesAsync();
        var result = new AppointmentModel
        {
            Id = newId,
            Date = appointment.Date,
            Start = appointment.Start,
            End = appointment.End,
            FromUser = new UserModel
            {
                Id = fromUser.Id,
                Name = fromUser.Name,
            },
            ToUser = new UserModel
            {
                Id = toUser.Id,
                Name = toUser.Name,
            },
        };
        return Created($"/bookings/{newId}", result);
    }

    /// <summary>
    /// Delete appointment
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseModel<ErrorModel>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var appointment = await dbContext.Appointments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (appointment == null)
        {
            throw new ApiException(StatusCodes.Status404NotFound, "Could not find the appointment");
        }

        dbContext.Appointments.Remove(appointment);
        await dbContext.SaveChangesAsync();

        return Accepted();
    }

    private readonly AppDbContext dbContext;
    private readonly ILogger logger;
}