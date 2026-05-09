using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingMotorcycles.Models
{
 public   class Estacionamiento
    {

        public int idSession { get; set; }
        public DateTime hora_Entrada { get; set; }
        public DateTime? hora_Salida { get; set; }
        public int idEspacio { get; set; }
        public string? Estado { get; set; }
        public int idVehiculo { get; set; }
    }
}
