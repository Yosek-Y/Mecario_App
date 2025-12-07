using Mecario_BackEnd.Modelos;
using Microsoft.AspNetCore.Mvc;

[Route("Api/[controller]")]
[ApiController]
public class ServiciosMecanicosController : ControllerBase
{
    private readonly ServiciosMecanicosServicio _service;

    public ServiciosMecanicosController(ServiciosMecanicosServicio service)
    {
        _service = service;
    }

    [HttpGet("ListarPorTipo/{tipo}")]
    public async Task<IActionResult> ListarServiciosPorTipo(int tipo)
    {
        if (tipo != 1 && tipo != 2)
            return BadRequest("El tipo debe ser 1 (Mantenimiento) o 2 (Reparacion)");

        var tipoEnum = (ServiciosMecanicos.tipoDeServicio)tipo;

        var lista = await _service.ListarServiciosPorTipo(tipoEnum);

        if (lista == null || lista.Count == 0)
            return NotFound("No hay servicios registrados para este tipo.");

        return Ok(lista);
    }

}
