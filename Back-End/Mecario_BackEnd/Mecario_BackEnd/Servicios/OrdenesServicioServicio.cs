using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Modelos.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Mecario_BackEnd.Servicios
{
    public class OrdenesServicioServicio
    {
        private readonly ContextoBD _context;

        public OrdenesServicioServicio(ContextoBD context)
        {
            _context = context;
        }
        public async Task<OrdenesServicio> CrearOrden(CrearOrdenServicioDTO dto)
        {
            // 1. Extraer los IDs que vienen como string
            var idsString = dto.diagnosticoInicial.Split(',', StringSplitOptions.RemoveEmptyEntries);
            List<int> ids = idsString.Select(id => int.Parse(id.Trim())).ToList();

            // 2. Buscar los servicios en la BD
            var servicios = await _context.ServiciosMecanicos
                .Where(s => ids.Contains(s.idServicio))
                .ToListAsync();

            if (servicios.Count == 0)
                throw new Exception("Ninguno de los servicios existe.");

            // 3. Sumar precios
            double subtotal = servicios.Sum(s => s.precio);

            // 4. Convertir los NOMBRES de los servicios a un string
            string diagnosticoLegible = string.Join(", ", servicios.Select(s => s.servicio));

            // 5. Crear la orden usando el string legible
            var orden = new OrdenesServicio
            {
                tipoServicio = (OrdenesServicio.TipoServicio)dto.TipoServicio,
                diagnosticoInicial = diagnosticoLegible, 
                costoInicial = subtotal,
                idVehiculo = dto.idVehiculo,
                Servicios = servicios
            };

            // 6. Guardar en BD
            _context.OrdenesServicios.Add(orden);
            await _context.SaveChangesAsync();

            return orden;
        }

    }
}