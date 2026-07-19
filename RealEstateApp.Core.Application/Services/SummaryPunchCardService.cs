using RealEstateApp.Core.Application.DTOs.PunchCard;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace RealEstateApp.Core.Application.Services;

public class SummaryPunchCardService : Interfaces.Services.SummaryPunchCardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SummaryPunchCardService> _logger;

    public SummaryPunchCardService(IUnitOfWork unitOfWork, ILogger<SummaryPunchCardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultimportationPunchCardDto> ProcesarImportacionAsync(
        List<RowPunchCardImportada> filas, string nombreArchivo,
        string? usuarioId, string? usuarioNombre)
    {
        var sesion = new SesionPunchCard
        {
            NombreArchivo = nombreArchivo,
            TotalRegistros = filas.Count,
            UsuarioId = usuarioId,
            UsuarioNombre = usuarioNombre,
            FechaSubida = DateTime.UtcNow
        };

        var repo = _unitOfWork.Repository<SesionPunchCard>();
        await repo.AddAsync(sesion);
        await _unitOfWork.SaveChangesAsync();

        var registros = new List<RegistroPunchCard>();
        foreach (var fila in filas)
        {
            if (string.IsNullOrWhiteSpace(fila.NombreEmpleado)) continue;

            var horas = fila.HorasTrabajadas;
            if (!horas.HasValue && !string.IsNullOrWhiteSpace(fila.HoraEntrada) && !string.IsNullOrWhiteSpace(fila.HoraSalida))
            {
                if (TimeSpan.TryParse(fila.HoraEntrada, out var entrada) && TimeSpan.TryParse(fila.HoraSalida, out var salida))
                {
                    horas = (decimal)(salida - entrada).TotalHours;
                }
            }

            registros.Add(new RegistroPunchCard
            {
                SesionPunchCardId = sesion.Id,
                NombreEmpleado = fila.NombreEmpleado.Trim(),
                Departamento = fila.Departamento,
                Fecha = fila.Fecha,
                HoraEntrada = fila.HoraEntrada,
                HoraSalida = fila.HoraSalida,
                HorasTrabajadas = horas,
                Estado = EstadoPunchCard.Normal
            });
        }

        var registroRepo = _unitOfWork.Repository<RegistroPunchCard>();
        foreach (var r in registros)
        {
            await registroRepo.AddAsync(r);
        }

        sesion.RegistrosProcesados = registros.Count;
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Sesion PunchCard {Id} creada: {Total} registros", sesion.Id, registros.Count);

        return new ResultimportationPunchCardDto
        {
            SesionId = sesion.Id,
            TotalFilas = filas.Count,
            RegistrosInsertados = registros.Count,
            NombreArchivo = nombreArchivo
        };
    }

    public async Task<IEnumerable<SessionPunchCardDto>> GetSesionesAsync()
    {
        var repo = _unitOfWork.Repository<SesionPunchCard>();
        var sesiones = await repo.GetAllAsync();
        return sesiones.OrderByDescending(s => s.FechaSubida).Select(s => new SessionPunchCardDto
        {
            Id = s.Id,
            NombreArchivo = s.NombreArchivo,
            TotalRegistros = s.TotalRegistros,
            RegistrosProcesados = s.RegistrosProcesados,
            RegistrosConError = s.RegistrosConError,
            UsuarioNombre = s.UsuarioNombre,
            FechaSubida = s.FechaSubida
        });
    }

    public async Task<List<RegistrationPunchCardDto>> GetRegistrosAsync(FilterPunchCardDto filtro)
    {
        var repo = _unitOfWork.Repository<RegistroPunchCard>();

        var all = await repo.GetAllAsync();
        var query = all.AsQueryable();

        if (filtro.SesionId.HasValue)
            query = query.Where(r => r.SesionPunchCardId == filtro.SesionId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.NombreEmpleado))
            query = query.Where(r => r.NombreEmpleado.Contains(filtro.NombreEmpleado, StringComparison.OrdinalIgnoreCase));

        if (filtro.SoloLicencias)
            query = query.Where(r => r.Estado == EstadoPunchCard.Licencia);

        return query
            .OrderBy(r => r.NombreEmpleado).ThenBy(r => r.Fecha)
            .Skip((filtro.Pagina - 1) * filtro.TamanoPagina)
            .Take(filtro.TamanoPagina)
            .Select(r => new RegistrationPunchCardDto
            {
                Id = r.Id,
                SesionPunchCardId = r.SesionPunchCardId,
                NombreEmpleado = r.NombreEmpleado,
                Departamento = r.Departamento,
                Fecha = r.Fecha,
                HoraEntrada = r.HoraEntrada,
                HoraSalida = r.HoraSalida,
                HorasTrabajadas = r.HorasTrabajadas,
                Estado = r.Estado,
                Observacion = r.Observacion,
                NombreEditado = r.NombreEditado
            }).ToList();
    }

    public async Task<int> GetTotalRegistrosAsync(FilterPunchCardDto filtro)
    {
        var repo = _unitOfWork.Repository<RegistroPunchCard>();
        var all = await repo.GetAllAsync();
        var query = all.AsQueryable();

        if (filtro.SesionId.HasValue)
            query = query.Where(r => r.SesionPunchCardId == filtro.SesionId.Value);

        if (!string.IsNullOrWhiteSpace(filtro.NombreEmpleado))
            query = query.Where(r => r.NombreEmpleado.Contains(filtro.NombreEmpleado, StringComparison.OrdinalIgnoreCase));

        if (filtro.SoloLicencias)
            query = query.Where(r => r.Estado == EstadoPunchCard.Licencia);

        return query.Count();
    }

    public async Task<RegistrationPunchCardDto?> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.Repository<RegistroPunchCard>();
        var r = await repo.GetByIdAsync(id);
        if (r is null) return null;

        return new RegistrationPunchCardDto
        {
            Id = r.Id,
            SesionPunchCardId = r.SesionPunchCardId,
            NombreEmpleado = r.NombreEmpleado,
            Departamento = r.Departamento,
            Fecha = r.Fecha,
            HoraEntrada = r.HoraEntrada,
            HoraSalida = r.HoraSalida,
            HorasTrabajadas = r.HorasTrabajadas,
            Estado = r.Estado,
            Observacion = r.Observacion,
            NombreEditado = r.NombreEditado
        };
    }

    public async Task<RegistrationPunchCardDto?> EditarRegistroAsync(EditPunchCardDto dto, string? editadoPor)
    {
        var repo = _unitOfWork.Repository<RegistroPunchCard>();
        var r = await repo.GetByIdAsync(dto.Id);
        if (r is null) return null;

        if (r.NombreEmpleado != dto.NombreEmpleado)
        {
            if (!r.NombreEditado)
                r.NombreOriginal = r.NombreEmpleado;
            r.NombreEditado = true;
            r.NombreEmpleado = dto.NombreEmpleado;
        }

        r.Estado = dto.TieneLicencia ? EstadoPunchCard.Licencia : EstadoPunchCard.Normal;
        r.Observacion = dto.Observacion;

        await repo.UpdateAsync(r);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("PunchCard registro {Id} editado por {EditadoPor}", dto.Id, editadoPor);

        return new RegistrationPunchCardDto
        {
            Id = r.Id,
            SesionPunchCardId = r.SesionPunchCardId,
            NombreEmpleado = r.NombreEmpleado,
            Departamento = r.Departamento,
            Fecha = r.Fecha,
            HoraEntrada = r.HoraEntrada,
            HoraSalida = r.HoraSalida,
            HorasTrabajadas = r.HorasTrabajadas,
            Estado = r.Estado,
            Observacion = r.Observacion,
            NombreEditado = r.NombreEditado
        };
    }
}
