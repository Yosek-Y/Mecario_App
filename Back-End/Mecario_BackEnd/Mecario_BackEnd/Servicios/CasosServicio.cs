using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Modelos.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Mecario_BackEnd.Servicios
{
    public class CasosServicio
    {
        private readonly ContextoBD _context;

        public CasosServicio(ContextoBD context)
        {
            _context = context;
        }

        //Metodo para que se listen los casos de un mecanico
        public async Task<List<CasosDeMecanicoDTO>> ListarCasosPorMecanico(int idMecanico)
        {
            // Validar que exista el usuario y que sea mecánico
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == idMecanico);

            if (usuario == null)
                throw new ArgumentException("El mecánico no existe.");

            if (usuario.tipoUsuario != Usuarios.Tipousuario.Mecanico)
                throw new ArgumentException("El usuario no es un mecánico.");

            //Obtener los casos
            var casos = await _context.Casos.Where(c => c.idUsuario == idMecanico).ToListAsync();

            if (casos.Count == 0)
                throw new ArgumentException("El mecánico no tiene casos asignados.");

            // Convertir a DTO
            var lista = casos.Select(c => new CasosDeMecanicoDTO
            {
                fechaInicio = c.fechaInicio,
                fechaFin = c.fechaFin,
                horasTrabajadas = c.horasTrabajadas,
                estadoCaso = c.estadoCaso.ToString(),
                totalCaso = c.totalCaso
            }).ToList();

            return lista;
        }

        // Método para que el admin asigne un caso a un mecánico
        public async Task<string> AsignarCasoPorAdmin(AsignarCasoAdminDTO dto)
        {
            if (dto == null)
                throw new ArgumentException("Los datos enviados están vacíos.");

            // Busca el caso
            var caso = await _context.Casos.FirstOrDefaultAsync(c => c.idCaso == dto.idCaso);
            if (caso == null)
                throw new ArgumentException("El caso indicado no existe.");

            // Busca el usuario
            var mecanico = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == dto.idMecanico);
            if (mecanico == null)
                throw new ArgumentException("El mecánico no existe.");

            if (mecanico.tipoUsuario != Usuarios.Tipousuario.Mecanico)
                throw new ArgumentException("El usuario seleccionado no es un mecánico.");

            // Asigna
            caso.idUsuario = dto.idMecanico;

            await _context.SaveChangesAsync();

            return "Caso asignado correctamente al mecánico.";
        }

        // Método para asignar un mecánico a un caso
        public async Task<Casos> AsignarMecanico(MecanicoSeAsignaCasoDTO dto)
        {
            // Valida que exista el caso
            var caso = await _context.Casos.FirstOrDefaultAsync(c => c.idCaso == dto.idCaso);
            if (caso == null)
                throw new ArgumentException("El caso no existe.");

            // Valida que exista el mecánico
            var mecanico = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == dto.idMecanico);
            if (mecanico == null || mecanico.tipoUsuario != Usuarios.Tipousuario.Mecanico)
                throw new ArgumentException("El mecánico no existe o no es válido.");

            // Asigna al mecánico el caso
            caso.idUsuario = dto.idMecanico;

            await _context.SaveChangesAsync();
            return caso;
        }
    }
}
