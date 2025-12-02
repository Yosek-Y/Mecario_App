using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Mecario_BackEnd.Controladores
{
    [ApiController]
    [Route("Api/[controller]")]
    public class DetallesCasoController : ControllerBase
    {
        private readonly DetallesCasoServicio _servicio;

        public DetallesCasoController(DetallesCasoServicio servicio)
        {
            _servicio = servicio;
        }

        // POST: api/DetallesCaso
        [HttpPost("RegistrarTarea")]
        public async Task<IActionResult> RegistrarTarea([FromBody] RegistrarDetalleCasoDTO dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            try
            {
                var detalleCreado = await _servicio.RegistrarTareaAsync(dto);
                return Ok(detalleCreado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}