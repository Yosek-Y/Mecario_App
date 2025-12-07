using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Modelos.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Mecario_BackEnd.Servicios
{
    public class DetallesCasoServicio
    {
        private readonly ContextoBD _context;

        public DetallesCasoServicio(ContextoBD context)
        {
            _context = context;
        }

        public async Task<DetallesCaso> RegistrarTareaAsync(RegistrarDetalleCasoDTO dto)
        {
            // Validar que el caso exista
            var caso = await _context.Casos.FirstOrDefaultAsync(c => c.idCaso == dto.idCaso);

            if (caso == null)
                throw new Exception("El caso especificado no existe.");

            var detalle = new DetallesCaso
            {
                idCaso = dto.idCaso,
                hora = dto.hora,
                tareaRealizada = dto.tareaRealizada
            };

            _context.DetallesCasos.Add(detalle);
            await _context.SaveChangesAsync();

            return detalle;
        }

        // Obtener detalles del caso
        public async Task<List<DetallesCasoDTO>> ObtenerDetallesCasoAsync(int idCaso)
        {
            var detalles = await _context.DetallesCasos
                .Where(d => d.idCaso == idCaso)
                .OrderBy(d => d.hora)
                .ToListAsync();

            return detalles.Select(d => new DetallesCasoDTO
            {
                idDetalleCaso = d.idDetalleCaso,
                hora = d.hora,
                tareaRealizada = d.tareaRealizada,
                idCaso = d.idCaso
            }).ToList();
        }
    }
}