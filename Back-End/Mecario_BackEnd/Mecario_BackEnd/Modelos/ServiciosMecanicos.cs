using System.Text.Json.Serialization;

namespace Mecario_BackEnd.Modelos
{
    public class ServiciosMecanicos
    {
        public int idServicio { get; set; }
        public string servicio { get; set; }
        public enum tipoDeServicio { Mantenimiento = 1, Reparacion = 2 }
        public tipoDeServicio tipoServicio { get; set; }
        public double precio { get; set; }

        //Relación N:N con OrdenesServicio
        [JsonIgnore]
        public ICollection<OrdenesServicio> OrdenesServicio { get; set; }
    }
}
