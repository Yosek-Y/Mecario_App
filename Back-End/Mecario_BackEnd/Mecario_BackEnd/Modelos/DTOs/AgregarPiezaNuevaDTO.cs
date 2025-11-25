namespace Mecario_BackEnd.Modelos.DTOs
{
    public class AgregarPiezaNuevaDTO
    {
        public string nombrePieza { get; set; }
        public int categoriaPieza { get; set; } //se manda como número del enum
        public string descripcionPieza { get; set; }
        public string codigoPieza { get; set; }
        public double precioUnidad { get; set; }
        public int stockActual { get; set; }
    }
}
