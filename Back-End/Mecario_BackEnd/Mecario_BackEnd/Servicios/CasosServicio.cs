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
    }
}
