using System.Text.Json.Serialization;

namespace Mecario_BackEnd.Modelos
{
    public class OrdenesServicio
    {
        public int idOrden { get; set; }
        public enum TipoServicio { Mantenimiento = 1, Reparacion = 2 }
        public TipoServicio tipoServicio { get; set; } 
        public string diagnosticoInicial { get; set; }
        public double costoInicial { get; set; }

        //LLAVE FORANEA DE VEHICULOS
        public int idVehiculo { get; set; }
        public Vehiculos vehiculos { get; set; }

        //RELACIONES DE LA ENTIDAD ORDENES SERVICIOS
        //Relación 1:N --> Una Orden de servicio produce un Caso
        public ICollection<Casos> casos { get; set; }

        //Relación N:N con Servicios
        public ICollection<ServiciosMecanicos> Servicios { get; set; }
    }
}
