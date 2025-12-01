using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mecario_BackEnd.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class CasosController : ControllerBase
    {
        private readonly CasosServicio _service;

        public CasosController(CasosServicio service)
        {
            _service = service;
        }

        //Protocolo HTTP Get para obtener todos los casos donde participo dicho mecanico
        //POST: api/Mecanico/{idMecanico:int}
        [HttpGet("Mecanico/Casos_{idMecanico:int}")]
        public async Task<IActionResult> ListarCasosDeMecanico(int idMecanico)
        {
            try
            {
                var lista = await _service.ListarCasosPorMecanico(idMecanico);
                return Ok(lista);
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

        //Obtener la "factura" (totalCaso) de un caso por su ID
        [HttpGet("FacturaIndividual")]
        [ProducesResponseType(typeof(FacturaCasoDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerFacturaCaso(int idCaso)
        {
            if (idCaso <= 0)
                return BadRequest(new { error = "El id del caso debe ser mayor que cero." });

            try
            {
                var factura = await _service.ObtenerFacturaCasoAsync(idCaso);
                if (factura is null)
                    return NotFound(new { error = $"No se encontró el caso con id {idCaso}." });

                return Ok(factura);
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

        //Obtener la lista de facturas (totalCaso) de todos los casos, opcionalmente filtrando por idMecanico
        [HttpGet("Facturas")]
        [ProducesResponseType(typeof(List<TodasLasFacturasDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarFacturasDeCasos([FromQuery] int? idMecanico, CancellationToken ct = default)
        {
            try
            {
                var lista = await _service.ListarFacturasDeCasosAsync(idMecanico, ct);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }

        }

        //Obtener la lista de casos filtrando por su estado (status)
        [HttpGet("CasosPorStatus")]
        [ProducesResponseType(typeof(List<CasosSegunStatusDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarCasosPorStatus([FromQuery] string? status, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { error = "Debe proporcionar el parámetro 'status'." });

            try
            {
                var casos = await _service.ListarCasosPorStatusAsync(status, ct);

                if (casos.Count == 0)
                    return NotFound(new { error = $"No se encontraron casos con status '{status}'." });

                return Ok(casos);
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
