namespace Mecario_BackEnd.Modelos.DTOs
{
    public class CrearOrdenServicioDTO
    {
        public int TipoServicio { get; set; } // 
        public string diagnosticoInicial { get; set; }
        public double costoInicial { get; set; }
        public int idVehiculo { get; set; }

    }
}
