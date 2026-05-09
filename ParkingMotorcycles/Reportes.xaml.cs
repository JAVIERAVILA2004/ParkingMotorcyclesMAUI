using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ParkingMotorcycles.Json;
using ClosedXML.Excel;
#if ANDROID
using Android.OS;
#endif

namespace ParkingMotorcycles;

public partial class Reportes : ContentPage
{
	public Reportes()
	{
		InitializeComponent();
	}


    protected override void OnAppearing()
    {
        base.OnAppearing();
        cargarTabla();
    }

    private List<dynamic> listaOriginal;
    private List<dynamic> listaFiltrada = new();

    private async void fechaFiltro_DateSelected(object sender, DateChangedEventArgs e)
    {
        var reportes = await ReportesFinalizados.FinalizadoLista();
        var vehiculos = await VehiculosJson.ListarVehiculos();

        if (reportes == null || vehiculos == null)
        {
            await DisplayAlert("Error", "No se pudieron cargar los datos para el filtro.", "OK");
            return;
        }

        var filtrados = reportes
            .Where(r => r.hora_Entrada != null && r.hora_Entrada.Date == e.NewDate.Date)
            .Select(reporte =>
            {
                var vehiculo = vehiculos
                    .FirstOrDefault(v => v.idVehiculo == reporte.idVehiculo);

                return new
                {
                    Propietario = vehiculo?.Propietario ?? "Desconocido",
                    Placa = vehiculo?.Placa ?? "Desconocida",
                    Marca = vehiculo?.Marca ?? "Desconocida",
                    reporte.hora_Entrada,
                    reporte.hora_Salida,
                    reporte.idEspacio,
                    reporte.TotalPagado,
                    reporte.Estado
                };
            }).ToList();

        if (!filtrados.Any())
        {
            await DisplayAlert("Sin resultados",
                "No existen reportes para esa fecha.",
                "OK");

            return;
        }

        colllectionVIewGrupos.ItemsSource = filtrados;
        listaFiltrada = filtrados.Cast<dynamic>().ToList();
    }



    public async void cargarTabla()
    {
        var reportes = await ReportesFinalizados.FinalizadoLista();
        var vehiculos = await VehiculosJson.ListarVehiculos();

        if (reportes == null || vehiculos == null)
        {
            colllectionVIewGrupos.ItemsSource = new List<dynamic>();
            listaFiltrada = new List<dynamic>();
            return;
        }

        var reportesConDatos = reportes.Select(reporte =>
        {
            var vehiculo = vehiculos.FirstOrDefault(v => v.idVehiculo == reporte.idVehiculo);
            return new
            {
                Propietario = vehiculo?.Propietario ?? "Desconocido",
                Placa = vehiculo?.Placa ?? "Desconocida",
                Marca = vehiculo?.Marca ?? "Desconocida",
                reporte.hora_Entrada,
                reporte.hora_Salida,
                reporte.idEspacio,
                reporte.TotalPagado,
                reporte.Estado
            };
        }).ToList();

        colllectionVIewGrupos.ItemsSource = reportesConDatos;
        listaFiltrada = reportesConDatos.Cast<dynamic>().ToList();
    }

    private void LimpiarFiltro_Clicked(object sender, EventArgs e){
    fechaFiltro.Date = DateTime.Now;
    cargarTabla();
}
    private async void ExportarExcel_Clicked(object sender, EventArgs e)
    {
        if (listaFiltrada == null || !listaFiltrada.Any())
        {
            await DisplayAlert("Error", "No hay datos para exportar.", "OK");
            return;
        }

        string ruta;

#if ANDROID
        var downloadsFile = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
        var downloadsPath = downloadsFile?.AbsolutePath;
        if (string.IsNullOrEmpty(downloadsPath))
        {
            await DisplayAlert("Error", "No se pudo obtener la ruta de Descargas en Android.", "OK");
            return;
        }

        string nombreArchivo = $"Reporte_{fechaFiltro.Date:yyyyMMdd}_{DateTime.Now:HHmmss}.xlsx";
        ruta = Path.Combine(downloadsPath, nombreArchivo);
#else
        // En plataformas no-Android guardamos en Documents por defecto
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (string.IsNullOrEmpty(folder))
        {
            await DisplayAlert("Error", "No se pudo obtener la ruta para guardar el archivo.", "OK");
            return;
        }
        ruta = Path.Combine(folder, "ReporteParqueadero.xlsx");
#endif

        using (var workbook = new XLWorkbook())
        {
            var hoja = workbook.Worksheets.Add("Reportes");

            // Encabezados
            hoja.Cell(1, 1).Value = "Propietario";
            hoja.Cell(1, 2).Value = "Placa";
            hoja.Cell(1, 3).Value = "Marca";
            hoja.Cell(1, 4).Value = "Espacio";
            hoja.Cell(1, 5).Value = "Hora Entrada";
            hoja.Cell(1, 6).Value = "Hora Salida";
            hoja.Cell(1, 7).Value = "Total";
            hoja.Cell(1, 8).Value = "Estado";

            int fila = 2;

            foreach (var item in listaFiltrada)
            {
                hoja.Cell(fila, 1).Value = item?.Propietario ?? "Desconocido";
                hoja.Cell(fila, 2).Value = item?.Placa ?? "Desconocida";
                hoja.Cell(fila, 3).Value = item?.Marca ?? "Desconocida";
                hoja.Cell(fila, 4).Value = item?.idEspacio;
                hoja.Cell(fila, 5).Value = item?.hora_Entrada?.ToString() ?? "";
                hoja.Cell(fila, 6).Value = item?.hora_Salida?.ToString() ?? "";
                hoja.Cell(fila, 7).Value = item?.TotalPagado;
                hoja.Cell(fila, 8).Value = item?.Estado ?? "";

                fila++;
            }

            hoja.Columns().AdjustToContents();

            workbook.SaveAs(ruta);
        }

        await DisplayAlert(
            "Excel Exportado",
            $"Archivo guardado en:\n{ruta}",
            "OK");
    }
}