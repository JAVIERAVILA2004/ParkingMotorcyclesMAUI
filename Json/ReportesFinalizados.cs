using ParkingMotorcycles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParkingMotorcycles.Json
{
    class ReportesFinalizados
    {
        public static async Task registrarFinalizado(List<Estacionamiento> estacionamientos)
        {
            Plataforma oPlataforma = new Plataforma();
            string path = oPlataforma.obtenerpath("ReportesFinalizados.json");
            string json = JsonSerializer.Serialize(estacionamientos, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
        }

        public static async Task<List<Estacionamiento>> FinalizadoLista()
        {
            Plataforma oPlataforma = new Plataforma();
            string path = oPlataforma.obtenerpath("ReportesFinalizados.json");
            if (!File.Exists(path))
            {
                return new List<Estacionamiento>();
            }

            string json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<Estacionamiento>>(json) ?? new List<Estacionamiento>();
        }






    }
}
