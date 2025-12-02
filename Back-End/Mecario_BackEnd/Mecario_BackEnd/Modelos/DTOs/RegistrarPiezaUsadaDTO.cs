namespace Mecario_BackEnd.Modelos.DTOs
{
    public class RegistrarPiezaUsadaDTO
    {
        public int idCaso { get; set; }
        public string codigoPieza { get; set; }   // Buscará la pieza por código
        public int cantidad { get; set; }
    }
}
