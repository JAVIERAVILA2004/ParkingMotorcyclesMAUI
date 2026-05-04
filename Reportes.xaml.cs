using ParkingMotorcycles.Json;

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

    public async void cargarTabla()
    {
        var reportes = await ReportesFinalizados.FinalizadoLista();
        var vehiculos = await VehiculosJson.ListarVehiculos();


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
                reporte.Estado
            };
        }).ToList();

        colllectionVIewGrupos.ItemsSource = reportesConDatos;
    }

}