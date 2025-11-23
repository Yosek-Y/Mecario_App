namespace Mecario_BackEnd.Modelos
{
    public class Usuarios
    {
        public int idUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public string telefonoUsuario { get; set; }
        public string correoUsuario { get; set; }
        public string direccionUsuario { get; set; }
        public string tipoUsuario { get; set; } // "Mecanico" o "Admin"
        public string userName { get; set; }
        public string userPassword { get; set; }

        //RELACIONES
        //Un Usuario(Mecánico) puede atender muchos Casos
        public ICollection<Casos> casos { get; set; }
    }
}
