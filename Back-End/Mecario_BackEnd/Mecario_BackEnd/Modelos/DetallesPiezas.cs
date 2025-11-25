namespace Mecario_BackEnd.Modelos
{
    public class DetallesPiezas
    {
        //PK COMPUESTA

        public int idCaso { get; set; }
        public Casos casos { get; set; }
        public int idPieza { get; set; }
        public Piezas piezas { get; set; }
        public int cantidad { get; set; }
        public double precioUnitario { get; set; }

        //Subtotal = cantidad × precioUnitario
        public double subtotal { get; set; }

    }
}
