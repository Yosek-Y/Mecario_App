namespace Mecario_BackEnd.Modelos.DTOs
{
    public class CasoDTO
    {
        public int idCaso { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public double horasTrabajadas { get; set; }
        public string estadoCaso { get; set; }
        public double totalCaso { get; set; }

        // Info adicional
        public int idOrdenServicio { get; set; }
        public string? diagnosticoInicial { get; set; }
        public int? idUsuario { get; set; }
        public string? nombreMecanico { get; set; }
    }
}
