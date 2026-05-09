using ParkingMotorcycles.Json;
using ParkingMotorcycles.Models;

namespace ParkingMotorcycles;

public partial class Config : ContentPage
{
    public Config()
    {
        InitializeComponent();
        CargarConfiguracion();
    }

    private async void CargarConfiguracion()
    {
        var config = await ConfigTarifaJson.ListarConfiguracionTarifas();
        PrecioMinuto.Text = config.precioporMinuto.ToString();
        PrecioHora.Text = config.precioporHora.ToString();
        CobroMinimo.Text = config.cobroMinimo.ToString();
    }

    private async void GuardarConfiguracion(object sender, EventArgs e)
    {
        if (decimal.TryParse(PrecioMinuto.Text, out decimal precioMinuto) &&
            decimal.TryParse(PrecioHora.Text, out decimal precioHora) &&
            decimal.TryParse(CobroMinimo.Text, out decimal cobroMinimo))
        {
            ConfigTarifa config = new ConfigTarifa
            {
                precioporMinuto = precioMinuto,
                precioporHora = precioHora,
                cobroMinimo = cobroMinimo
            };

            await ConfigTarifaJson.GuardarConfiguracionTarifas(config);
            await DisplayAlert("Éxito", "Configuración guardada correctamente.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Ingrese valores numéricos válidos.", "OK");
        }
    }

    private async void Volver(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
