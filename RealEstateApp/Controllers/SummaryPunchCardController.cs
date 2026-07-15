using RealEstateApp.Core.Application.DTOs.PunchCard;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateApp.Controllers;

[Authorize(Roles = "Administrador")]
public class SummaryPunchCardController : Controller
{
    private readonly SummaryPunchCardService _resumenService;
    private readonly ImporterPunchCardService _importadorService;
    private readonly ExportPunchCardService _exportacionService;

    public SummaryPunchCardController(
        SummaryPunchCardService resumenService,
        ImporterPunchCardService importadorService,
        ExportPunchCardService exportacionService)
    {
        _resumenService = resumenService;
        _importadorService = importadorService;
        _exportacionService = exportacionService;
    }

    public async Task<IActionResult> Index()
    {
        var sesiones = await _resumenService.GetSesionesAsync();
        return View(sesiones);
    }

    public async Task<IActionResult> Registros(int? sesionId)
    {
        var lastSesion = sesionId ?? GetCookieInt("PunchCard_SesionId");
        var filtro = new FilterPunchCardDto
        {
            SesionId = lastSesion,
            NombreEmpleado = GetCookieString("PunchCard_Nombre"),
            SoloLicencias = GetCookieBool("PunchCard_SoloLicencias")
        };

        ViewBag.Sesiones = await _resumenService.GetSesionesAsync();
        ViewBag.Filtro = filtro;

        var registros = await _resumenService.GetRegistrosAsync(filtro);
        var total = await _resumenService.GetTotalRegistrosAsync(filtro);
        ViewBag.TotalRegistros = total;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / filtro.TamanoPagina);

        return View(registros);
    }

    [HttpPost]
    public async Task<IActionResult> Registros(FilterPunchCardDto filtro)
    {
        SetCookie("PunchCard_SesionId", filtro.SesionId?.ToString() ?? "");
        SetCookie("PunchCard_Nombre", filtro.NombreEmpleado ?? "");
        SetCookie("PunchCard_SoloLicencias", filtro.SoloLicencias.ToString());

        ViewBag.Sesiones = await _resumenService.GetSesionesAsync();
        ViewBag.Filtro = filtro;

        var registros = await _resumenService.GetRegistrosAsync(filtro);
        var total = await _resumenService.GetTotalRegistrosAsync(filtro);
        ViewBag.TotalRegistros = total;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / filtro.TamanoPagina);

        return View(registros);
    }

    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            ViewBag.Error = "Debe seleccionar un archivo.";
            return View();
        }

        if (archivo.Length > 10 * 1024 * 1024)
        {
            ViewBag.Error = "El archivo no puede exceder 10MB.";
            return View();
        }

        var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (ext is not (".csv" or ".xls" or ".xlsx"))
        {
            ViewBag.Error = "Solo se permiten archivos .csv, .xls o .xlsx.";
            return View();
        }

        try
        {
            using var stream = archivo.OpenReadStream();
            var filas = await _importadorService.LeerArchivoAsync(stream, archivo.FileName);

            var usuarioId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var usuarioNombre = User.Identity?.Name;
            var resultado = await _resumenService.ProcesarImportacionAsync(filas, archivo.FileName, usuarioId, usuarioNombre);

            TempData["Success"] = $"Archivo importado: {resultado.RegistrosInsertados} registros insertados.";
            return RedirectToAction(nameof(Registros), new { sesionId = resultado.SesionId });
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Error al procesar el archivo: {ex.Message}";
            return View();
        }
    }

    public async Task<IActionResult> Editar(int id)
    {
        var registro = await _resumenService.GetByIdAsync(id);
        if (registro == null) return NotFound();

        var dto = new EditPunchCardDto
        {
            Id = registro.Id,
            NombreEmpleado = registro.NombreEmpleado,
            TieneLicencia = registro.Estado == EstadoPunchCard.Licencia,
            Observacion = registro.Observacion
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, EditPunchCardDto dto)
    {
        if (id != dto.Id) return NotFound();
        if (!ModelState.IsValid) return View(dto);

        var editadoPor = User.Identity?.Name ?? "Sistema";
        await _resumenService.EditarRegistroAsync(dto, editadoPor);
        TempData["Success"] = "Registro actualizado correctamente.";
        return RedirectToAction(nameof(Registros), new { sesionId = GetCookieInt("PunchCard_SesionId") });
    }

    public async Task<IActionResult> ExportarExcel(int sesionId)
    {
        var bytes = await _exportacionService.ExportarExcelAsync(sesionId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ResumenPunchCard_{sesionId}.xlsx");
    }

    public async Task<IActionResult> ExportarPdf(int sesionId)
    {
        var bytes = await _exportacionService.ExportarPdfAsync(sesionId);
        return File(bytes, "application/pdf", $"ResumenPunchCard_{sesionId}.pdf");
    }

    public async Task<IActionResult> ExportarWord(int sesionId)
    {
        var bytes = await _exportacionService.ExportarWordAsync(sesionId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"ResumenPunchCard_{sesionId}.docx");
    }

    #region Cookie Helpers
    private void SetCookie(string key, string value, int days = 30)
    {
        Response.Cookies.Append(key, value, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(days),
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        });
    }

    private string GetCookieString(string key)
    {
        Request.Cookies.TryGetValue(key, out var value);
        return value ?? "";
    }

    private int? GetCookieInt(string key)
    {
        var val = GetCookieString(key);
        return int.TryParse(val, out var result) ? result : null;
    }

    private bool GetCookieBool(string key)
    {
        var val = GetCookieString(key);
        return bool.TryParse(val, out var result) && result;
    }
    #endregion
}
