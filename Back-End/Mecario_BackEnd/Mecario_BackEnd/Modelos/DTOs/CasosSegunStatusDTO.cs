namespace Mecario_BackEnd.Modelos.DTOs
{
    public class CasosSegunStatusDTO
    {
        public DateTime fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public double horasTrabajadas { get; set; }
        public string estadoCaso { get; set; } = string.Empty;
        public double totalCaso { get; set; }
        public int idUsuario { get; set; }
        public string nombreMecanico { get; set; } = string.Empty;
    }   
}
