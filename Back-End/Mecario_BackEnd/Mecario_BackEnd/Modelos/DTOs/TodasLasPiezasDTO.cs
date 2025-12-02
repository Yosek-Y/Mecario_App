namespace Mecario_BackEnd.Modelos.DTOs
{
    public class TodasLasPiezasDTO
    {
        public string NombrePieza { get; set; }
        public string CategoriaPieza { get; set; }
        public string DescripcionPieza { get; set; }
        public string CodigoPieza { get; set; }
        public double PrecioUnidad { get; set; }
        public int StockActual { get; set; }
    }
}
