using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Mecario_BackEnd.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class VehiculosController : ControllerBase
    {
        private readonly VehiculosServicio _service;

        public VehiculosController(VehiculosServicio service)
        {
            _service = service;
        }

        //Protocolo HTTP Post para agregar un vehiculo a un cliente 
        // POST: Api/Vehiculos/AgregarVehiculo
        [HttpPost("AgregarVehiculo")]
        public async Task<IActionResult> AgregarVehiculo([FromBody] AgregarVehiculoDTO dto)
        {
            try
            {
                string mensaje = await _service.AgregarVehiculo(dto);

                return Ok(new { mensaje });
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

        //Protocolo HTTP Get para obtener los vehiculos de un cliente 
        // GET: Api/Vehiculos/ListarPorCliente/{idCliente}
        [HttpGet("ListarPorCliente/{idCliente}")]
        public async Task<IActionResult> ListarVehiculosPorCliente(int idCliente)
        {
            var lista = await _service.ListarVehiculosCliente(idCliente);
            return Ok(lista);
        }
    }
}