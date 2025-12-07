namespace Mecario_BackEnd.Modelos.DTOs
{
    public class DetallesPiezasDTO
    {
        public int idCaso { get; set; }
        public int idPieza { get; set; }
        public string nombrePieza { get; set; }
        public int cantidad { get; set; }
        public double precioUnitario { get; set; }
        public double subtotal { get; set; }
    }
}
