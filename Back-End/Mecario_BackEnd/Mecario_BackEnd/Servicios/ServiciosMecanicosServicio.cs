using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Modelos.DTOs;
using Microsoft.EntityFrameworkCore;

public class ServiciosMecanicosServicio
{
    private readonly ContextoBD _context;

    public ServiciosMecanicosServicio(ContextoBD context)
    {
        _context = context;
    }

    public async Task<List<ServiciosPorTipoDTO>> ListarServiciosPorTipo(ServiciosMecanicos.tipoDeServicio tipo)
    {
        var lista = await _context.ServiciosMecanicos
            .Where(s => s.tipoServicio == tipo)
            .Select(s => new ServiciosPorTipoDTO
            {
                idServicio = s.idServicio,
                servicio = s.servicio,
                precio = s.precio
            })
            .ToListAsync();

        return lista;
    }
}