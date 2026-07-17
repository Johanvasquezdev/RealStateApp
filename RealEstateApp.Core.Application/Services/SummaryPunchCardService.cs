using RealEstateApp.Core.Application.DTOs.PunchCard;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace RealEstateApp.Core.Application.Services;

public class SummaryPunchCardService : ISummaryPunchCardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SummaryPunchCardService> _logger;

    public SummaryPunchCardService(IUnitOfWork unitOfWork, ILogger<SummaryPunchCardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PunchCardImportResultDto> ProcessImportAsync(
        List<ImportedPunchCardRow> rows, string fileName,
        string? userId, string? userName)
    {
        var sesion = new PunchCardSession
        {
            FileName = fileName,
            TotalRecords = rows.Count,
            UserId = userId,
            UserName = userName,
            UploadDate = DateTime.UtcNow
        };

        var repo = _unitOfWork.Repository<PunchCardSession>();
        await repo.AddAsync(sesion);
        await _unitOfWork.SaveChangesAsync();

        var registros = new List<PunchCardRecord>();
        foreach (var fila in rows)
        {
            if (string.IsNullOrWhiteSpace(fila.EmployeeName)) continue;

            var horas = fila.HoursWorked;
            if (!horas.HasValue && !string.IsNullOrWhiteSpace(fila.ClockInTime) && !string.IsNullOrWhiteSpace(fila.ClockOutTime))
            {
                if (TimeSpan.TryParse(fila.ClockInTime, out var entrada) && TimeSpan.TryParse(fila.ClockOutTime, out var salida))
                {
                    horas = (decimal)(salida - entrada).TotalHours;
                }
            }

            registros.Add(new PunchCardRecord
            {
                PunchCardSessionId = sesion.Id,
                EmployeeName = fila.EmployeeName.Trim(),
                Department = fila.Department,
                Date = fila.Date,
                ClockInTime = fila.ClockInTime,
                ClockOutTime = fila.ClockOutTime,
                HoursWorked = horas,
                Status = PunchCardStatus.Normal
            });
        }

        var registroRepo = _unitOfWork.Repository<PunchCardRecord>();
        foreach (var r in registros)
        {
            await registroRepo.AddAsync(r);
        }

        sesion.ProcessedRecords = registros.Count;
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Sesion PunchCard {Id} creada: {Total} registros", sesion.Id, registros.Count);

        return new PunchCardImportResultDto
        {
            SessionId = sesion.Id,
            TotalRows = rows.Count,
            RecordsInserted = registros.Count,
            FileName = fileName
        };
    }

    public async Task<IEnumerable<SessionPunchCardDto>> GetSessionsAsync()
    {
        var repo = _unitOfWork.Repository<PunchCardSession>();
        var sesiones = await repo.GetAllAsync();
        return sesiones.OrderByDescending(s => s.UploadDate).Select(s => new SessionPunchCardDto
        {
            Id = s.Id,
            FileName = s.FileName,
            TotalRecords = s.TotalRecords,
            ProcessedRecords = s.ProcessedRecords,
            ErrorRecords = s.ErrorRecords,
            UserName = s.UserName,
            UploadDate = s.UploadDate
        });
    }

    public async Task<List<RegistrationPunchCardDto>> GetRecordsAsync(FilterPunchCardDto filtro)
    {
        var repo = _unitOfWork.Repository<PunchCardRecord>();

        var all = await repo.GetAllAsync();
        var query = all.AsQueryable();

        if (filtro.SessionId.HasValue)
            query = query.Where(r => r.PunchCardSessionId == filtro.SessionId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.EmployeeName))
            query = query.Where(r => r.EmployeeName.Contains(filtro.EmployeeName, StringComparison.OrdinalIgnoreCase));

        if (filtro.OnLeaveOnly)
            query = query.Where(r => r.Status == PunchCardStatus.OnLeave);

        return query
            .OrderBy(r => r.EmployeeName).ThenBy(r => r.Date)
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .Select(r => new RegistrationPunchCardDto
            {
                Id = r.Id,
                PunchCardSessionId = r.PunchCardSessionId,
                EmployeeName = r.EmployeeName,
                Department = r.Department,
                Date = r.Date,
                ClockInTime = r.ClockInTime,
                ClockOutTime = r.ClockOutTime,
                HoursWorked = r.HoursWorked,
                Status = r.Status,
                Notes = r.Notes,
                NameEdited = r.NameEdited
            }).ToList();
    }

    public async Task<int> GetTotalRecordsAsync(FilterPunchCardDto filtro)
    {
        var repo = _unitOfWork.Repository<PunchCardRecord>();
        var all = await repo.GetAllAsync();
        var query = all.AsQueryable();

        if (filtro.SessionId.HasValue)
            query = query.Where(r => r.PunchCardSessionId == filtro.SessionId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.EmployeeName))
            query = query.Where(r => r.EmployeeName.Contains(filtro.EmployeeName, StringComparison.OrdinalIgnoreCase));

        if (filtro.OnLeaveOnly)
            query = query.Where(r => r.Status == PunchCardStatus.OnLeave);

        return query.Count();
    }

    public async Task<RegistrationPunchCardDto?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.Repository<PunchCardRecord>();
        var r = await repo.GetByIdAsync(id);
        if (r is null) return null;

        return new RegistrationPunchCardDto
        {
            Id = r.Id,
            PunchCardSessionId = r.PunchCardSessionId,
            EmployeeName = r.EmployeeName,
            Department = r.Department,
            Date = r.Date,
            ClockInTime = r.ClockInTime,
            ClockOutTime = r.ClockOutTime,
            HoursWorked = r.HoursWorked,
            Status = r.Status,
            Notes = r.Notes,
            NameEdited = r.NameEdited
        };
    }

    public async Task<RegistrationPunchCardDto?> EditRecordAsync(EditPunchCardDto dto, string? editedBy)
    {
        var repo = _unitOfWork.Repository<PunchCardRecord>();
        var r = await repo.GetByIdAsync(dto.Id);
        if (r is null) return null;

        if (r.EmployeeName != dto.EmployeeName)
        {
            if (!r.NameEdited)
                r.OriginalName = r.EmployeeName;
            r.NameEdited = true;
            r.EmployeeName = dto.EmployeeName;
        }

        r.Status = dto.IsOnLeave ? PunchCardStatus.OnLeave : PunchCardStatus.Normal;
        r.Notes = dto.Notes;

        await repo.UpdateAsync(r);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("PunchCard registro {Id} editado por {EditadoPor}", dto.Id, editedBy);

        return new RegistrationPunchCardDto
        {
            Id = r.Id,
            PunchCardSessionId = r.PunchCardSessionId,
            EmployeeName = r.EmployeeName,
            Department = r.Department,
            Date = r.Date,
            ClockInTime = r.ClockInTime,
            ClockOutTime = r.ClockOutTime,
            HoursWorked = r.HoursWorked,
            Status = r.Status,
            Notes = r.Notes,
            NameEdited = r.NameEdited
        };
    }
}
