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

        private async Task<decimal> CalcularPago(DateTime horaEntrada, DateTime horaSalida)
        {
            var config = await ConfigTarifaJson.ListarConfiguracionTarifas();

            // Calculamos la diferencia de tiempo
            TimeSpan tiempoEstacionado = horaSalida - horaEntrada;

            // Convertimos a minutos y horas
            int minutosTotales = (int)tiempoEstacionado.TotalMinutes;
            int horasTotales = (int)tiempoEstacionado.TotalHours;

            decimal totalPagar = 0;

            // Si el tiempo es menor a una hora, se cobra por minuto
            if (horasTotales == 0)
            {
                totalPagar = minutosTotales * config.precioporMinuto    ;
            }
            else
            {
                // Se cobra las horas completas + los minutos restantes si los hay
                totalPagar = (horasTotales * config.precioporHora) + ((minutosTotales % 60) * config.precioporMinuto);
            }

            return totalPagar;
        }




    }
}
