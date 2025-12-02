using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Modelos.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Mecario_BackEnd.Servicios
{
    public class UsuariosServicio
    {
        private readonly ContextoBD _context;

        public UsuariosServicio(ContextoBD context)
        {
            _context = context;
        }

        //Metodo para que el admin agregue un mecanico nuevo al sistema
        public async Task<Usuarios> AgregarMecanico(AgregarMecanicoDTO dto)
        {
            //VALIDACIONES
            if (dto == null)
                throw new ArgumentException("Los datos enviados están vacíos.");

            if (string.IsNullOrWhiteSpace(dto.nombreUsuario))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.userName))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.userPassword))
                throw new ArgumentException("La contraseña es obligatoria.");

            //Valida el correo que sea valido
            try
            {
                var correoValido = new MailAddress(dto.correoUsuario);
            }
            catch
            {
                throw new ArgumentException("El correo ingresado no es válido.");
            }

            //Validar username único
            bool existeUsername = await _context.Usuarios.AnyAsync(u => u.userName == dto.userName);
            if (existeUsername)
                throw new ArgumentException("El nombre de usuario ya existe. Debe ser único.");

            var nuevo = new Usuarios
            {
                nombreUsuario = dto.nombreUsuario,
                telefonoUsuario = dto.telefonoUsuario,
                correoUsuario = dto.correoUsuario,
                direccionUsuario = dto.direccionUsuario,
                //Le agrega automaticamente el rol o "tipo" de usuario que es, osea Mecanico
                tipoUsuario = Usuarios.Tipousuario.Mecanico,
                userName = dto.userName,
                userPassword = dto.userPassword
            };

            _context.Usuarios.Add(nuevo);
            await _context.SaveChangesAsync();

            return nuevo;
        }

        //Metodo para que el admin vea a todos los mecanicos del sistema
        public async Task<List<TodosLosMecanicosDTO>> ObtenerMecanicos()
        {
            var mecanicos = await _context.Usuarios.Where(u => u.tipoUsuario == Usuarios.Tipousuario.Mecanico)
                .Select(u => new TodosLosMecanicosDTO
                {
                    nombreUsuario = u.nombreUsuario,
                    telefonoUsuario = u.telefonoUsuario,
                    correoUsuario = u.correoUsuario,
                    direccionUsuario = u.direccionUsuario,
                    userName = u.userName
                })
                .ToListAsync();

            return mecanicos;
        }

        //Metodo de inicio de sesion
        public async Task<object> IniciarSesion(InicioSesionDTO dto)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.userName == dto.usuario);

            if (user == null)
                return "El usuario o contraseña es incorrecta";

            if (user.userPassword != dto.contrasena)
                return "El usuario o contraseña es incorrecta";

            return new { idUsuario = user.idUsuario };
        }
    }
}