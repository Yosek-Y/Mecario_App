using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Mecario_BackEnd.Modelos
{
    public class Vehiculos
    {
        public int idVehiculo {  get; set; }
        public string placa {  get; set; }
        public string marca { get; set; }
        public string modelo {  get; set; }
        public int anio { get; set; }
        public string color { get; set; }
        public string numeroChasis { get; set; }

        //LLAVE FORANEA DE CLIENTES
        public int idCliente { get; set; }
        public Clientes Clientes { get; set; }

        //RELACIONES DE LA ENTIDAD VEHICULO
        //Relación 1:N --> Un Vehiculo genera muchas Ordenes de servicio
        public ICollection<OrdenesServicio> ordenesServicios { get; set; }


    }
}
