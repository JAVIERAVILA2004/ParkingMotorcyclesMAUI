using ParkingMotorcycles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParkingMotorcycles.Json
{
    class ConfigTarifaJson
    {

        public static async Task GuardarConfiguracionTarifas(ConfigTarifa config)
        {
            Plataforma oPlataforma = new Plataforma();
            string path = oPlataforma.obtenerpath("ConfiguracionTarifas.json");
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
        }

        public static async Task<ConfigTarifa> ListarConfiguracionTarifas()
        {
            Plataforma oPlataforma = new Plataforma();
            string path = oPlataforma.obtenerpath("ConfiguracionTarifas.json");

            if (File.Exists(path))
            {
                string json = await File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<ConfigTarifa>(json) ?? new ConfigTarifa();
            }

            return new ConfigTarifa(); 
        }

        public static async Task<decimal> CalcularPago(DateTime horaEntrada, DateTime horaSalida)
        {
            var config = await ListarConfiguracionTarifas();

            TimeSpan tiempoEstacionado = horaSalida - horaEntrada;

            int minutosTotales = (int)tiempoEstacionado.TotalMinutes;

            if (minutosTotales < 60)
            {
                return config.cobroMinimo;
            }

            decimal total = config.precioporHora;


            int minutosExtra = minutosTotales - 60;

            total += minutosExtra * config.precioporMinuto;

            return total;
        }




    }
}
