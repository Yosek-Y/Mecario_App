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

        //Protocolo HTTP Post para crear caso
        // POST: Api/Casos/CrearCaso
        [HttpPost("CrearCaso")]
        public async Task<IActionResult> CrearCaso([FromBody] CrearCasoDTO dto)
        {
            try
            {
                var caso = await _service.CrearCasoAsync(dto);
                return Ok(caso);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        //Protocolo HTTP Put para abrir un caso
        // POST: Api/Casos/AbrirCaso
        [HttpPut("AbrirCaso")]
        public async Task<IActionResult> AbrirCaso([FromBody] AbrirCasoDTO dto)
        {
            try
            {
                var caso = await _service.AbrirCasoAsync(dto);
                return Ok(caso);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        //Protocolo HTTP Put para continuar
        // PUT: Api/Casos/ContinuarCaso
        [HttpPut("ContinuarCaso")]
        public async Task<IActionResult> ContinuarCaso([FromBody] ContinuarCasoDTO dto)
        {
            try
            {
                var caso = await _service.ContinuarCasoAsync(dto);
                return Ok(caso);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        //Protocolo HTTP Put para continuar
        // PUT: Api/Casos/CerrarCaso
        [HttpPut("CerrarCaso")]
        public async Task<IActionResult> CerrarCaso([FromBody] CerrarCasoDTO dto)
        {
            try
            {
                var resultado = await _service.CerrarCasoAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
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

        //Protocolo HTTP Post para asignar caso
        // POST: Api/Casos/AsignarCaso
        [HttpPost("AsignarCaso")]
        public async Task<IActionResult> AsignarCasoPorAdmin([FromBody] AsignarCasoAdminDTO dto)
        {
            try
            {
                var resultado = await _service.AsignarCasoPorAdmin(dto);
                return Ok(new { mensaje = resultado });
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

        //Peticion para obetener la factura de un caso en especifico
        //GET: Api/Casos/FacturaIndividual
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

        // Protocolo HTTP Get para obtener todas las facturas de todos los casos
        // GET: Api/Casos/ListarTodasLasFacturas
        [HttpGet("ListarTodasLasFacturas")]
        public async Task<IActionResult> ListarTodasLasFacturas()
        {
            try
            {
                var lista = await _service.ListarTodasLasFacturas();
                return Ok(new
                {
                    mensaje = "Lista de facturas obtenida correctamente",
                    data = lista
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error interno del servidor",
                    detalle = ex.Message
                });
            }
        }

        //Obtener la lista de casos filtrando por su estado (status)
        //GET: Api/Casos/CasosPorStatus
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
