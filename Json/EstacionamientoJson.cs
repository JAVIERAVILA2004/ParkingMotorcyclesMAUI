using ParkingMotorcycles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParkingMotorcycles.Json
{
    class EstacionamientoJson
    {

        public static async Task registrarespacio(List<Estacionamiento> estacionamientos)
        {
            Plataforma oPlataforma = new Plataforma();
            string path = oPlataforma.obtenerpath("estacionamiento.json");
            string json = JsonSerializer.Serialize(estacionamientos, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
        }

        public static async Task<List<Estacionamiento>> Listarestacionamientos()
        {
            Plataforma oPlataforma = new Plataforma();
            string path = oPlataforma.obtenerpath("estacionamiento.json");
            if (!File.Exists(path))
            {
                return new List<Estacionamiento>();
            }

            string json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<Estacionamiento>>(json) ?? new List<Estacionamiento>();
        }


        //public static async Task ActualizarEstacionamiento(List<Estacionamiento> estacionamientoActualizado)
        //{
        //    Plataforma opPlataforma = new Plataforma();
        //    string path = opPlataforma.obtenerpath("estacionamiento.json");

        //    if (!File.Exists(path))
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        // Leer el JSON actual
        //        string json = await File.ReadAllTextAsync(path);
        //        List<Estacionamiento> estacionamientos = JsonSerializer.Deserialize<List<Estacionamiento>>(json) ?? new List<Estacionamiento>();


        //        // Buscar el estacionamiento a actualizar
        //        int index = estacionamientos.FindIndex(e => e.idSession ==  estacionamientoActualizado.);
        //        if (index != -1)
        //        {
        //            estacionamientos[index] = estacionamientoActualizado; // Reemplazar con la nueva información
        //        }

        //        // Guardar el JSON actualizado
        //        string nuevoJson = JsonSerializer.Serialize(estacionamientos, new JsonSerializerOptions { WriteIndented = true });
        //        await File.WriteAllTextAsync(path, nuevoJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error al actualizar estacionamiento: {ex.Message}");
        //    }
        //}


        public static async Task<List<Estacionamiento>> ELiminarEstacionamiento(int idSession)
        {
            {
                Plataforma oplataforma = new Plataforma();
                string path = oplataforma.obtenerpath("estacionamiento.json");
                if (!File.Exists(path))
                {
                    return new List<Estacionamiento>();
                }
                string json = await File.ReadAllTextAsync(path);
                List<Estacionamiento> estacionamiento = JsonSerializer.Deserialize<List<Estacionamiento>>(json) ?? new List<Estacionamiento>();

                var personaEliminar = estacionamiento.Find(p => p.idSession == idSession);
                if (personaEliminar != null)
                {
                    estacionamiento.Remove(personaEliminar);
                    json = JsonSerializer.Serialize(estacionamiento, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(path, json);
                }
                return estacionamiento;
            }
        }


    }
}
