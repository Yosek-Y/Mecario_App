namespace Mecario_BackEnd.Modelos
{
    public class Piezas
    {
        public int idPieza { get; set; }
        public string nombrePieza { get; set; }
        public string categoriaPieza { get; set; }
        public string descripcionPieza { get; set; }
        public double precioUnidad { get; set; }
        public int stockActual { get; set; }

        //RELACIONES
        //N:N --> Una pieza puede estar en muchos DetallesPieza
        public ICollection<DetallesPiezas> detallesPieza { get; set; }

    }
}
