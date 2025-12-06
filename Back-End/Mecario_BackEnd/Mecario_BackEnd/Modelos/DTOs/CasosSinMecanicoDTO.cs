namespace Mecario_BackEnd.Modelos.DTOs
{
    public class CasosSinMecanicoDTO
    {
        public int idCaso { get; set; }
        public DateTime fechaInicio { get; set; }
        public double horasTrabajadas { get; set; }
        public string estadoCaso { get; set; }
        public double totalCaso { get; set; }
        public int idOrdenServicio { get; set; }
    }
}
