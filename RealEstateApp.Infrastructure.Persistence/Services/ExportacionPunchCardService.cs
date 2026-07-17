using RealEstateApp.Core.Application.DTOs.PunchCard;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class ExportacionPunchCardService : Core.Application.Interfaces.Services.ExportPunchCardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExportacionPunchCardService> _logger;

    public ExportacionPunchCardService(IUnitOfWork unitOfWork, ILogger<ExportacionPunchCardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<byte[]> ExportExcelAsync(int sesionId)
    {
        var registros = await GetRegistrosAsync(sesionId);
        var sesion = await GetSesionAsync(sesionId);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Punch Card Resumen");

        var headers = new[] { "Empleado", "Departamento", "Fecha", "Hora Entrada", "Hora Salida", "Horas", "Estado", "Observacion" };
        for (int col = 0; col < headers.Length; col++)
        {
            var cell = ws.Cell(1, col + 1);
            cell.Value = headers[col];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#8B0000");
            cell.Style.Font.FontColor = XLColor.FromHtml("#FFD700");
        }

        int fila = 2;
        foreach (var r in registros)
        {
            ws.Cell(fila, 1).Value = r.EmployeeName;
            ws.Cell(fila, 2).Value = r.Department ?? "-";
            ws.Cell(fila, 3).Value = r.Date.ToString("dd/MM/yyyy");
            ws.Cell(fila, 4).Value = r.ClockInTime ?? "-";
            ws.Cell(fila, 5).Value = r.ClockOutTime ?? "-";
            ws.Cell(fila, 6).Value = r.HoursWorked?.ToString("F2") ?? "-";
            ws.Cell(fila, 7).Value = r.Status.ToString();
            ws.Cell(fila, 8).Value = r.Notes ?? "-";
            fila++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportPdfAsync(int sesionId)
    {
        var registros = await GetRegistrosAsync(sesionId);
        var sesion = await GetSesionAsync(sesionId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);

                page.Header().Column(col =>
                {
                    col.Item().Text("Resumen de Punch Card").Bold().FontSize(18).FontColor("#8B0000");
                    col.Item().Text($"Archivo: {sesion?.FileName ?? "N/A"}").FontSize(10);
                    col.Item().Text($"Fecha de subida: {sesion?.UploadDate:dd/MM/yyyy HH:mm}").FontSize(10);
                    col.Item().Text($"Total registros: {registros.Count}").FontSize(10);
                    col.Item().PaddingBottom(10).LineHorizontal(1);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Empleado").Bold();
                        header.Cell().Text("Depto.").Bold();
                        header.Cell().Text("Fecha").Bold();
                        header.Cell().Text("Entrada").Bold();
                        header.Cell().Text("Salida").Bold();
                        header.Cell().Text("Horas").Bold();
                        header.Cell().Text("Estado").Bold();
                        header.Cell().Text("Observacion").Bold();
                    });

                    foreach (var r in registros)
                    {
                        table.Cell().Text(r.EmployeeName);
                        table.Cell().Text(r.Department ?? "-");
                        table.Cell().Text(r.Date.ToString("dd/MM/yyyy"));
                        table.Cell().Text(r.ClockInTime ?? "-");
                        table.Cell().Text(r.ClockOutTime ?? "-");
                        table.Cell().Text(r.HoursWorked?.ToString("F2") ?? "-");
                        table.Cell().Text(r.Status.ToString());
                        table.Cell().Text(r.Notes ?? "-");
                    }
                });

                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Pagina ");
                    txt.CurrentPageNumber();
                    txt.Span(" de ");
                    txt.TotalPages();
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportWordAsync(int sesionId)
    {
        var registros = await GetRegistrosAsync(sesionId);
        var sesion = await GetSesionAsync(sesionId);

        var stream = new MemoryStream();
        using (var document = Xceed.Words.NET.DocX.Create(stream))
        {
            document.InsertParagraph("Resumen de Punch Card").FontSize(16).Bold();
            document.InsertParagraph($"Archivo: {sesion?.FileName ?? "N/A"}");
            document.InsertParagraph($"Fecha: {sesion?.UploadDate:dd/MM/yyyy HH:mm} | Total registros: {registros.Count}");
            document.InsertParagraph();

            var table = document.AddTable(registros.Count + 1, 8);

            var headers = new[] { "Empleado", "Depto.", "Fecha", "Entrada", "Salida", "Horas", "Estado", "Observacion" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = table.Rows[0].Cells[i];
                cell.Paragraphs[0].Append(headers[i]).Bold();
            }

            for (int row = 0; row < registros.Count; row++)
            {
                var r = registros[row];
                table.Rows[row + 1].Cells[0].Paragraphs[0].Append(r.EmployeeName);
                table.Rows[row + 1].Cells[1].Paragraphs[0].Append(r.Department ?? "-");
                table.Rows[row + 1].Cells[2].Paragraphs[0].Append(r.Date.ToString("dd/MM/yyyy"));
                table.Rows[row + 1].Cells[3].Paragraphs[0].Append(r.ClockInTime ?? "-");
                table.Rows[row + 1].Cells[4].Paragraphs[0].Append(r.ClockOutTime ?? "-");
                table.Rows[row + 1].Cells[5].Paragraphs[0].Append(r.HoursWorked?.ToString("F2") ?? "-");
                table.Rows[row + 1].Cells[6].Paragraphs[0].Append(r.Status.ToString());
                table.Rows[row + 1].Cells[7].Paragraphs[0].Append(r.Notes ?? "-");
            }

            document.InsertTable(table);
            document.Save();
        }

        return stream.ToArray();
    }

    private async Task<List<RegistrationPunchCardDto>> GetRegistrosAsync(int sesionId)
    {
        var repo = _unitOfWork.Repository<PunchCardRecord>();
        var all = await repo.FindAsync(r => r.PunchCardSessionId == sesionId);
        return all.OrderBy(r => r.EmployeeName).ThenBy(r => r.Date).Select(r => new RegistrationPunchCardDto
        {
            Id = r.Id,
            EmployeeName = r.EmployeeName,
            Department = r.Department,
            Date = r.Date,
            ClockInTime = r.ClockInTime,
            ClockOutTime = r.ClockOutTime,
            HoursWorked = r.HoursWorked,
            Status = r.Status,
            Notes = r.Notes
        }).ToList();
    }

    private async Task<PunchCardSession?> GetSesionAsync(int sesionId)
    {
        var repo = _unitOfWork.Repository<PunchCardSession>();
        return await repo.GetByIdAsync(sesionId);
    }
}
