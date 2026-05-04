using ParkingMotorcycles.Json;
using ParkingMotorcycles.Models;
using System.Data;
using System.Threading.Tasks;

namespace ParkingMotorcycles;

public partial class Parking : ContentPage
{
    private Button espacioSeleccionado; // Variable global para guardar el espacio seleccionado

    private Dictionary<string, bool> estadoEspacios = new Dictionary<string, bool>();

    public Parking()
    {
        InitializeComponent();
        CargarEstadosEspacios();
    }

    private async void CargarEstadosEspacios()
    {
        var estacionamientos = await EstacionamientoJson.Listarestacionamientos();
        foreach (var i in Enumerable.Range(1, 14))
        {
            string espacio = i.ToString();
            var btn = this.FindByName<Button>($"btnP{espacio}");

            // Guardamos el número original en AutomationId
            btn.AutomationId = espacio;

            if (estacionamientos.Any(e => e.idEspacio == i && e.Estado == "Ocupado"))
            {
                btn.Text = "Ocupado";
                btn.BackgroundColor = Colors.Red;
                btn.TextColor = Colors.White;
                estadoEspacios[espacio] = true;
            }
            else
            {
                btn.Text = espacio;
                btn.BackgroundColor = Colors.Green;
                btn.TextColor = Colors.White;
                estadoEspacios[espacio] = false;
            }
        }

    }

    private async void AbrirModal(object sender, EventArgs e)
    {
        espacioSeleccionado = (Button)sender;
        string numeroEspacio = espacioSeleccionado.CommandParameter.ToString();

        lblTitulo.Text = $"Registrar vehículo en espacio {numeroEspacio}";
        lblparking.Text = numeroEspacio;

        // Buscamos el estado del espacio
        var estacionamientos = await EstacionamientoJson.Listarestacionamientos();
        var estacionamiento = estacionamientos.FirstOrDefault(e => e.idEspacio == Convert.ToInt32(numeroEspacio));

        // Limpiamos los campos por defecto
        txtplaca.Text = string.Empty;
        txtMarca.Text = string.Empty;
        txtNombre.Text = string.Empty;

        if (estacionamiento != null && estacionamiento.Estado == "Ocupado")
        {
            var vehiculos = await VehiculosJson.ListarVehiculos();
            var vehiculo = vehiculos.FirstOrDefault(v => v.idVehiculo == estacionamiento.idVehiculo);
            btnsalida.IsVisible = true;
            btnRegistar.IsVisible = false;

            if (vehiculo != null)
            {
                txtplaca.Text = vehiculo.Placa;
                txtMarca.Text = vehiculo.Marca;
                txtNombre.Text = vehiculo.Propietario;
            }

            await DisplayAlert("Espacio ocupado", "Este espacio ya tiene un vehículo registrado.", "OK");
        }
        else
        {
            btnsalida.IsVisible = false;
            btnRegistar.IsVisible = true;
        }

        
        ModalView.IsVisible = true;
    }

    private void CerrarModal(object sender, EventArgs e)
    {
        ModalView.IsVisible = false;
    }

    private async void RegistrarVehiculo(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtplaca.Text) || string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtMarca.Text))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
            return;
        }

        var vehiculosExistentes = await VehiculosJson.ListarVehiculos();
        Vehiculo oVehiculo = new Vehiculo
        {
            idVehiculo = new Random().Next(1, 9999),
            Placa = txtplaca.Text,
            Marca = txtMarca.Text,
            Propietario = txtNombre.Text
        };

        vehiculosExistentes.Add(oVehiculo);
        await VehiculosJson.RegsitarVehiculos(vehiculosExistentes);


        var estacionamientosExistentes = await EstacionamientoJson.Listarestacionamientos();
        Estacionamiento estacionamiento = new Estacionamiento
        {
            idSession = new Random().Next(1, 9999),
            idEspacio = Convert.ToInt16(lblparking.Text),
            hora_Entrada = DateTime.Now,
            idVehiculo = oVehiculo.idVehiculo,
            Estado = "Ocupado"
        };

        estacionamientosExistentes.Add(estacionamiento);
        await EstacionamientoJson.registrarespacio(estacionamientosExistentes);

        espacioSeleccionado.Text = "Ocupado";
        espacioSeleccionado.BackgroundColor = Colors.Red;
        estadoEspacios[espacioSeleccionado.AutomationId] = true;

        await DisplayAlert("Ingreso Exitoso", $"Vehículo {oVehiculo.Placa} registrado correctamente.", "OK");
        ModalView.IsVisible = false;
    }

    private async void btnsalida_Clicked(object sender, EventArgs e)
    {
        string numeroEspacio = espacioSeleccionado.CommandParameter?.ToString() ?? lblparking.Text;

        if (int.TryParse(numeroEspacio, out int espacioNumero))
        {
            var estacionamientos = await EstacionamientoJson.Listarestacionamientos();
            var estacionamiento = estacionamientos.FirstOrDefault(e => e.idEspacio == espacioNumero);

            if (estacionamiento != null)
            {
                estacionamiento.hora_Salida = DateTime.Now;
                estacionamiento.Estado = "Finalizado";

                TimeSpan tiempoEstacionado = estacionamiento.hora_Salida - estacionamiento.hora_Entrada;

                var config = await ConfigTarifaJson.ListarConfiguracionTarifas();
                decimal precioMinuto = config?.precioporMinuto ?? 0;
                decimal precioHora = config?.precioporHora ?? 0;

                decimal totalPagar = CalcularPago(tiempoEstacionado, precioMinuto, precioHora);

                await DisplayAlert("Salida Exitosa", $"El vehículo ha salido del estacionamiento. Total a pagar: ${totalPagar:F2}", "OK");

                // Registrar en reportes finalizados
                var reportesExistentes = await ReportesFinalizados.FinalizadoLista();
                reportesExistentes.Add(estacionamiento);
                await ReportesFinalizados.registrarFinalizado(reportesExistentes);

                // Eliminar del estacionamiento activo
                await EstacionamientoJson.ELiminarEstacionamiento(estacionamiento.idSession);

                // Actualizar interfaz
                espacioSeleccionado.Text = numeroEspacio;
                espacioSeleccionado.BackgroundColor = Colors.Green;
                estadoEspacios[numeroEspacio] = false;
            }
            else
            {
                await DisplayAlert("Error", "No se encontró el espacio seleccionado.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Número de espacio inválido.", "OK");
        }
    }

    // Función para calcular el costo del estacionamiento
    private decimal CalcularPago(TimeSpan tiempo, decimal precioMinuto, decimal precioHora)
    {
        int minutos = (int)tiempo.TotalMinutes;
        int horas = minutos / 60;
        int minutosRestantes = minutos % 60;

        decimal costo = (horas * precioHora) + (minutosRestantes * precioMinuto);
        return costo;
    }




}