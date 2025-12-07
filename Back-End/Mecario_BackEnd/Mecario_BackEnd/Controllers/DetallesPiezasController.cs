using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Mecario_BackEnd.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class DetallesPiezasController : ControllerBase
    {
        private readonly DetallesPiezaServicio _servicio;

        public DetallesPiezasController(DetallesPiezaServicio servicio)
        {
            _servicio = servicio;
        }

        //Metodo Post para agregar el uso de una pieza a el caso
        // POST: Api/DetallesPiezas/registrar
        [HttpPost("RegistrarPieza")]
        public async Task<IActionResult> RegistrarPieza([FromBody] RegistrarPiezaUsadaDTO dto)
        {
            try
            {
                var detalle = await _servicio.RegistrarPiezaEnCaso(dto);
                return Ok(detalle);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET Api/DetallesPiezas/{idCaso}
        [HttpGet("{idCaso}")]
        public async Task<IActionResult> ObtenerPiezas(int idCaso)
        {
            var piezas = await _servicio.ObtenerPiezasDeCasoAsync(idCaso);
            return Ok(piezas);
        }
    }
}