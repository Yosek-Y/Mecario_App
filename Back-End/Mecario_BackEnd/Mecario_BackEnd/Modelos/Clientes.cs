using System.Globalization;

namespace Mecario_BackEnd.Modelos
{
    public class Clientes
    {
        public int idCliente {  get; set; }
        public string nombreCliente { get; set; }
        public string telefonoCliente { get; set; }
        public string correoCliente { get; set; }
        public string direccionCliente { get; set; }

        //RELACIONES DE LA ENTIDAD CLIENTE
        //Relación 1:N --> Un Cliente tiene muchos Vehículos
        public ICollection<Vehiculos> Vehiculos { get; set; }
    }
}