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

    public async Task<byte[]> ExportarExcelAsync(int sesionId)
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
            ws.Cell(fila, 1).Value = r.NombreEmpleado;
            ws.Cell(fila, 2).Value = r.Departamento ?? "-";
            ws.Cell(fila, 3).Value = r.Fecha.ToString("dd/MM/yyyy");
            ws.Cell(fila, 4).Value = r.HoraEntrada ?? "-";
            ws.Cell(fila, 5).Value = r.HoraSalida ?? "-";
            ws.Cell(fila, 6).Value = r.HorasTrabajadas?.ToString("F2") ?? "-";
            ws.Cell(fila, 7).Value = r.Estado.ToString();
            ws.Cell(fila, 8).Value = r.Observacion ?? "-";
            fila++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream.ToArray();
    }

    public async Task<byte[]> ExportarPdfAsync(int sesionId)
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
                    col.Item().Text($"Archivo: {sesion?.NombreArchivo ?? "N/A"}").FontSize(10);
                    col.Item().Text($"Fecha de subida: {sesion?.FechaSubida:dd/MM/yyyy HH:mm}").FontSize(10);
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
                        table.Cell().Text(r.NombreEmpleado);
                        table.Cell().Text(r.Departamento ?? "-");
                        table.Cell().Text(r.Fecha.ToString("dd/MM/yyyy"));
                        table.Cell().Text(r.HoraEntrada ?? "-");
                        table.Cell().Text(r.HoraSalida ?? "-");
                        table.Cell().Text(r.HorasTrabajadas?.ToString("F2") ?? "-");
                        table.Cell().Text(r.Estado.ToString());
                        table.Cell().Text(r.Observacion ?? "-");
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

    public async Task<byte[]> ExportarWordAsync(int sesionId)
    {
        var registros = await GetRegistrosAsync(sesionId);
        var sesion = await GetSesionAsync(sesionId);

        var stream = new MemoryStream();
        using (var document = Xceed.Words.NET.DocX.Create(stream))
        {
            document.InsertParagraph("Resumen de Punch Card").FontSize(16).Bold();
            document.InsertParagraph($"Archivo: {sesion?.NombreArchivo ?? "N/A"}");
            document.InsertParagraph($"Fecha: {sesion?.FechaSubida:dd/MM/yyyy HH:mm} | Total registros: {registros.Count}");
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
                table.Rows[row + 1].Cells[0].Paragraphs[0].Append(r.NombreEmpleado);
                table.Rows[row + 1].Cells[1].Paragraphs[0].Append(r.Departamento ?? "-");
                table.Rows[row + 1].Cells[2].Paragraphs[0].Append(r.Fecha.ToString("dd/MM/yyyy"));
                table.Rows[row + 1].Cells[3].Paragraphs[0].Append(r.HoraEntrada ?? "-");
                table.Rows[row + 1].Cells[4].Paragraphs[0].Append(r.HoraSalida ?? "-");
                table.Rows[row + 1].Cells[5].Paragraphs[0].Append(r.HorasTrabajadas?.ToString("F2") ?? "-");
                table.Rows[row + 1].Cells[6].Paragraphs[0].Append(r.Estado.ToString());
                table.Rows[row + 1].Cells[7].Paragraphs[0].Append(r.Observacion ?? "-");
            }

            document.InsertTable(table);
            document.Save();
        }

        return stream.ToArray();
    }

    private async Task<List<RegistrationPunchCardDto>> GetRegistrosAsync(int sesionId)
    {
        var repo = _unitOfWork.Repository<RegistroPunchCard>();
        var all = await repo.GetAllAsync();
        var filtered = all.Where(r => r.SesionPunchCardId == sesionId);
        return filtered.OrderBy(r => r.NombreEmpleado).ThenBy(r => r.Fecha).Select(r => new RegistrationPunchCardDto
        {
            Id = r.Id,
            NombreEmpleado = r.NombreEmpleado,
            Departamento = r.Departamento,
            Fecha = r.Fecha,
            HoraEntrada = r.HoraEntrada,
            HoraSalida = r.HoraSalida,
            HorasTrabajadas = r.HorasTrabajadas,
            Estado = r.Estado,
            Observacion = r.Observacion
        }).ToList();
    }

    private async Task<SesionPunchCard?> GetSesionAsync(int sesionId)
    {
        var repo = _unitOfWork.Repository<SesionPunchCard>();
        return await repo.GetByIdAsync(sesionId);
    }
}
