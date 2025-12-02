using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Modelos.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Mecario_BackEnd.Servicios
{
    public class DetallesPiezaServicio
    {
        private readonly ContextoBD _context;

        public DetallesPiezaServicio(ContextoBD context)
        {
            _context = context;
        }

        //Metodo para registrar la pieza en el caso
        public async Task<DetallesPiezas> RegistrarPiezaEnCaso (RegistrarPiezaUsadaDTO dto)
        {
            // 1. Buscar si existe el caso
            var caso = await _context.Casos.FindAsync(dto.idCaso);

            if (caso == null)
                throw new Exception("El caso no existe.");

            // 2. Buscar la pieza por el código
            var pieza = await _context.Piezas
                .FirstOrDefaultAsync(p => p.codigoPieza == dto.codigoPieza);

            if (pieza == null)
                throw new Exception("La pieza con ese código no existe.");

            // 3. Validar stock
            if (pieza.stockActual < dto.cantidad)
                throw new Exception("Stock insuficiente para registrar esta pieza.");

            // 4. Restar el stock
            pieza.stockActual -= dto.cantidad;

            // 5. Crear el registro DetallesPiezas
            var detalle = new DetallesPiezas
            {
                idCaso = dto.idCaso,
                idPieza = pieza.idPieza,
                cantidad = dto.cantidad,
                precioUnitario = pieza.precioUnidad,
                subtotal = dto.cantidad * pieza.precioUnidad
            };

            _context.DetallesPiezas.Add(detalle);

            // 6. Guardar cambios
            await _context.SaveChangesAsync();

            return detalle;
        }
    }
}