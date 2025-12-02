using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Servicios;
using Microsoft.AspNetCore.Mvc;


namespace Mecario_BackEnd.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class OrdenesServicioController : ControllerBase
    {
        private readonly OrdenesServicioServicio _service;

        public OrdenesServicioController(OrdenesServicioServicio service)
        {
            _service = service;
        }

        // Protocolo HTTP Post para crear una nueva orden de servicio
        // POST: Api/OrdenesServicio/NuevaOrden
        [HttpPost("NuevaOrden")]
        public async Task<IActionResult> Crear([FromBody] CrearOrdenServicioDTO dto)
        {
            try
            {
                var orden = await _service.CrearOrden(dto);
                return Ok(orden);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}