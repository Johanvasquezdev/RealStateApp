using RealEstateApp.Core.Application.DTOs.PunchCard;
using RealEstateApp.Core.Application.Interfaces.Services;
using ClosedXML.Excel;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class ImportadorPunchCardCsv : ImporterPunchCardService
{
    public async Task<List<ImportedPunchCardRow>> ReadFileAsync(Stream stream, string nombreArchivo)
    {
        var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
        return extension == ".csv"
            ? await ReadCsvAsync(stream)
            : await ReadExcelAsync(stream);
    }

    private async Task<List<ImportedPunchCardRow>> ReadCsvAsync(Stream stream)
    {
        var filas = new List<ImportedPunchCardRow>();
        using var reader = new StreamReader(stream);
        var lines = await reader.ReadToEndAsync();
        var rows = lines.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (rows.Length < 2) return filas;

        var delimiter = rows[0].Contains(';') ? ';' : ',';
        var headers = rows[0].Split(delimiter).Select(h => h.Trim().Trim('"').ToLower()).ToArray();

        var nameIdx = Array.FindIndex(headers, h => h is "nombre" or "nombreempleado" or "name" or "empleado");
        var deptIdx = Array.FindIndex(headers, h => h is "departamento" or "department" or "depto");
        var dateIdx = Array.FindIndex(headers, h => h is "fecha" or "date");
        var entryIdx = Array.FindIndex(headers, h => h is "horaentrada" or "entrada" or "on-duty" or "first on-duty" or "check in");
        var exitIdx = Array.FindIndex(headers, h => h is "horasalida" or "salida" or "off-duty" or "first off-duty" or "check out");
        var hoursIdx = Array.FindIndex(headers, h => h is "horas" or "horastrabajadas" or "hours" or "total");

        for (int i = 1; i < rows.Length; i++)
        {
            var cols = rows[i].Split(delimiter).Select(c => c.Trim().Trim('"')).ToArray();
            if (cols.Length <= nameIdx || string.IsNullOrWhiteSpace(cols[nameIdx])) continue;

            var fila = new ImportedPunchCardRow
            {
                EmployeeName = cols[nameIdx],
                Department = deptIdx >= 0 && deptIdx < cols.Length ? cols[deptIdx] : null,
                Date = dateIdx >= 0 && dateIdx < cols.Length && DateTime.TryParse(cols[dateIdx], out var f) ? f : DateTime.Today,
                ClockInTime = entryIdx >= 0 && entryIdx < cols.Length ? ParseTime(cols[entryIdx]) : null,
                ClockOutTime = exitIdx >= 0 && exitIdx < cols.Length ? ParseTime(cols[exitIdx]) : null,
                HoursWorked = hoursIdx >= 0 && hoursIdx < cols.Length && decimal.TryParse(cols[hoursIdx], out var h) ? h : null
            };
            filas.Add(fila);
        }
        return filas;
    }

    private async Task<List<ImportedPunchCardRow>> ReadExcelAsync(Stream stream)
    {
        var filas = new List<ImportedPunchCardRow>();
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet(1);
        if (ws == null) return filas;

        var headerRow = 1;
        for (int r = 1; r <= Math.Min(5, ws.LastRowUsed()?.RowNumber() ?? 1); r++)
        {
            var cell = ws.Cell(r, 1).GetString();
            var cellLower = cell?.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(cell) && (cellLower == "id" || cellLower == "codigo" || cellLower == "no."))
            {
                headerRow = r;
                break;
            }
        }

        var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 1;
        var colMap = new Dictionary<string, int>();
        for (int c = 1; c <= lastCol; c++)
        {
            var h = ws.Cell(headerRow, c).GetString()?.Trim().ToLower();
            if (!string.IsNullOrEmpty(h) && !colMap.ContainsKey(h))
                colMap[h] = c;
        }

        int GetCol(params string[] names)
        {
            foreach (var n in names)
                if (colMap.TryGetValue(n, out var idx)) return idx;
            return -1;
        }

        var nameC = GetCol("name", "nombre", "empleado", "employee");
        var deptC = GetCol("department", "departamento", "depto");
        var dateC = GetCol("date", "fecha");
        var entryC = GetCol("first on-duty", "on-duty", "entrada", "check in", "horaentrada");
        var exitC = GetCol("first off-duty", "off-duty", "salida", "check out", "horasalida");
        var hoursC = GetCol("total(min)", "hours", "horas", "total");

        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
        for (int r = headerRow + 1; r <= lastRow; r++)
        {
            var nombre = nameC > 0 ? ws.Cell(r, nameC).GetString()?.Trim() : null;
            if (string.IsNullOrWhiteSpace(nombre)) continue;

            filas.Add(new ImportedPunchCardRow
            {
                EmployeeName = nombre,
                Department = deptC > 0 ? ws.Cell(r, deptC).GetString()?.Trim() : null,
                Date = dateC > 0 && DateTime.TryParse(ws.Cell(r, dateC).GetString(), out var f) ? f : DateTime.Today,
                ClockInTime = entryC > 0 ? ParseTime(ws.Cell(r, entryC).GetString()) : null,
                ClockOutTime = exitC > 0 ? ParseTime(ws.Cell(r, exitC).GetString()) : null,
                HoursWorked = hoursC > 0 && decimal.TryParse(ws.Cell(r, hoursC).GetString(), out var h) ? h : null
            });
        }

        return await Task.FromResult(filas);
    }

    private static string? ParseTime(string? timeStr)
    {
        if (string.IsNullOrWhiteSpace(timeStr)) return null;
        var cleaned = timeStr.Trim();
        if (cleaned is "--" or "-" or "AM" or "PM" or "00:00") return null;
        return TimeSpan.TryParse(cleaned, out _) ? cleaned : null;
    }
}
