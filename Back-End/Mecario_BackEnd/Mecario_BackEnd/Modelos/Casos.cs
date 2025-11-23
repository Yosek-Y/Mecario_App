namespace Mecario_BackEnd.Modelos
{
    public class Casos
    {
        public int idCaso { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public double horasTrabajadas { get; set; }

        //Este total será calculado con el Subtotal de OrdenServicio + Suma de DetallesPieza + costoHorasMecanico
        public double totalCaso { get; set; }

        //FK HACIA ORDEN DE SERVICIO
        public int idOrdenServicio { get; set; }
        public OrdenesServicio ordenesServicio { get; set; }

        //FK HACIA USUARIO (solo mecánicos)
        public int idUsuario { get; set; }
        public Usuarios usuarios { get; set; }

        //RELACIONES
        //1:N --> Un caso tiene muchos detalles
        public ICollection<DeatallesCaso> detallesCaso { get; set; }

        //N:N --> Un caso usa muchas piezas
        public ICollection<DetallesPiezas> detallesPieza { get; set; }
    }
}
