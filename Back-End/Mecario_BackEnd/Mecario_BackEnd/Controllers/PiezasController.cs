using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Mecario_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PiezasController : ControllerBase
    {
        private readonly PiezasServicio _service;

        public PiezasController(PiezasServicio service)
        {
            _service = service;
        }

        // ============================
        // POST: api/Piezas/AgregarPieza
        // ============================
        [HttpPost("AgregarPieza")]
        public async Task<IActionResult> AgregarPieza([FromBody] AgregarPiezaNuevaDTO dto)
        { //Llama al meetodo de Agregar nueva pieza
            try
            {
                var nuevaPieza = await _service.AgregarPiezaNueva(dto);
                return Ok(new
                {
                    mensaje = "Pieza agregada correctamente",
                    data = nuevaPieza
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}
