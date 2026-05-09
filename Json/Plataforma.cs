using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingMotorcycles.Json
{
    class Plataforma
    {

        public string obtenerpath(string nombreArchivo)
        {
            string ruta = "";
            string directorio = "archivoJson";
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), directorio);
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), directorio);
            }

            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }

            ruta = Path.Combine(ruta, nombreArchivo);
            return ruta;

        }



    }
}
