namespace Mecario_BackEnd.Modelos.DTOs
{
    public class FacturaCasoDTO
    {
        public int idCaso { get; set; }
        public double totalCaso { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
    }
}
