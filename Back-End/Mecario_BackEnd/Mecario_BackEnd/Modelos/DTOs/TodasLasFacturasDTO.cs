namespace Mecario_BackEnd.Modelos.DTOs
{
    public class TodasLasFacturasDTO
    {

        public int idCaso { get; set; }
        public double totalCaso { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public double horasTrabajadas { get; set; }

        //datos del mecanico
        public int idUsuario { get; set; }
        public string nombreMecanico { get; set; } = string.Empty;
    }
}
