namespace Mecario_BackEnd.Modelos.DTOs
{
    public class ServiciosDeCasoDTO
    {
        public int idOrden { get; set; }
        public string diagnosticoInicial { get; set; }
        public List<ServiciosPorTipoDTO> servicios { get; set; }
    }
}
