using ParkingMotorcycles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParkingMotorcycles.Json
{
    class VehiculosJson
    {

        public static async Task RegsitarVehiculos(List<Vehiculo> vehiculos)
        {
            Plataforma oPLataforma = new Plataforma();
            string path = oPLataforma.obtenerpath("Vehiculos.json");
            string json = JsonSerializer.Serialize(vehiculos, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
        }

        public static async Task<List<Vehiculo>> ListarVehiculos()
        {
            Plataforma oPLataforma = new Plataforma();
            string path = oPLataforma.obtenerpath("Vehiculos.json");
            if (!File.Exists(path))
            {
                return new List<Vehiculo>();
            }


            string json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<Vehiculo>>(json) ?? new List<Vehiculo>();

        }



       

    }
}
