namespace Mecario_BackEnd.Modelos.DTOs
{
    public class AgregarVehiculoDTO
    {
        public string placa { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public int anio { get; set; }
        public string color { get; set; }
        public string numeroChasis { get; set; }
        public int idCliente { get; set; }
    }
}
