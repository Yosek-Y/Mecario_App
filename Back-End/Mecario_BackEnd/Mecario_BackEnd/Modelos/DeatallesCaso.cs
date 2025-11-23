namespace Mecario_BackEnd.Modelos
{
    public class DeatallesCaso
    {
        public int idDetalleCaso { get; set; }
        public DateTime hora { get; set; }
        public string tareaRealizada { get; set; }

        //FK HACIA CASO
        public int idCaso { get; set; }
        public Casos casos { get; set; }
    }
}
