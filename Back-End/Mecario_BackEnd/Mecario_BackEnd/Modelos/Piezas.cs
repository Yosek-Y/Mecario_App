namespace Mecario_BackEnd.Modelos
{
    public class Piezas
    {
        public int idPieza { get; set; }    
        public string nombrePieza { get; set; }
        //ENUM PARA DIFERENCIAR LA CATEGORIA Y SEA SOLO ESA
        public enum CategoriaPieza { Motor = 1, Transmision = 2, Frenos = 3, Suspension = 4,
            Electrico = 5, Escape = 6, Enfriamiento = 7, Carroceria = 8, Ruedas = 9, Direccion = 10,
            Lubricantes = 11, Accesorios = 12 }
        public CategoriaPieza categoriaPieza { get; set; }
        public string descripcionPieza { get; set; }
        public string codigoPieza { get; set; }
        public double precioUnidad { get; set; }
        public int stockActual { get; set; }

        //RELACIONES
        //N:N --> Una pieza puede estar en muchos DetallesPieza
        public ICollection<DetallesPiezas> detallesPieza { get; set; }

    }
}
