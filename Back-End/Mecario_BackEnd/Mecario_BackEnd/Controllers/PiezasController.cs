using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Mecario_BackEnd.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class PiezasController : ControllerBase
    {
        private readonly PiezasServicio _service;

        public PiezasController(PiezasServicio service)
        {
            _service = service;
        }

        //Protocolo HTTP Post para agregar nueva pieza al inventario
        //POST: api/Piezas/AgregarPieza
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

        //Protocolo HTTP Put para añadir stock a una pieza existente
        //PUT: api/Piezas/AgregarStock
        [HttpPut("AgregarStock")]
        public async Task<IActionResult> AgregarStock([FromBody] AgregarReducirStockDTO dto)
        {
            try
            {
                var piezaActualizada = await _service.AgregarStock(dto);
                return Ok(new
                {
                    mensaje = "Stock actualizado correctamente",
                    data = piezaActualizada
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

        //Protocolo HTTP Put para reducir stock a una pieza existente
        //PUT: api/Piezas/AgregarStock
        [HttpPut("ReducirStock")]
        public async Task<IActionResult> ReducirStock([FromBody] AgregarReducirStockDTO dto)
        {
            try
            {
                var piezaActualizada = await _service.ReducirStock(dto);
                return Ok(new
                {
                    mensaje = "Stock actualizado correctamente",
                    data = piezaActualizada
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

        //Protocolo HTTP Get para obtener todas las piezas
        //GET: Api/Piezas/TodasLasPiezas
        [HttpGet("TodasLasPiezas")]
        public async Task<IActionResult> ObtenerTodasLasPiezas()
        {
            var piezas = await _service.ObtenerTodasLasPiezasAsync();
            return Ok(piezas);
        }

        //Protocolo HTTP Get para obtener todas las piezas por categoria
        //GET: Api/Piezas/Categoria/{categoriaId}
        [HttpGet("Categoria/{categoriaId}")]
        public async Task<IActionResult> ObtenerPiezasPorCategoria(int categoriaId)
        {
            try
            {
                var piezas = await _service.ObtenerPiezasPorCategoriaAsync(categoriaId);
                return Ok(piezas);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
