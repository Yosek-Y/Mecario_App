using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Mecario_BackEnd.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosServicio _service;

        public UsuariosController(UsuariosServicio service)
        {
            _service = service;
        }

        //Protocolo HTTP Post para agregar un nuevo mecanico al sistema
        //POST: api/Piezas/AgregarMecanico
        [HttpPost("AgregarMecanico")]
        public async Task<IActionResult> AgregarMecanico([FromBody] AgregarMecanicoDTO dto)
        {
            try
            {
                var mecanico = await _service.AgregarMecanico(dto);

                return Ok(new
                {
                    mensaje = "Mecánico agregado correctamente",
                    data = mecanico
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

        //Protocolo HTTP Get para listar todos los mecanicos de la base
        //POST: api/Usuarios/Mecanicos
        [HttpGet("Mecanicos")]
        public async Task<IActionResult> ObtenerMecanicos()
        {
            try
            {
                var lista = await _service.ObtenerMecanicos();
                return Ok(new
                {
                    cantidad = lista.Count,
                    mecanicos = lista
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }
        }

        //Protocolo HTTP Post para el inicio de sesion
        //POST: Api/Usuarios/Iniciar-Sesion
        [HttpPost("Iniciar-Sesion")]
        public async Task<IActionResult> IniciarSesion([FromBody] InicioSesionDTO dto)
        {
            var resultado = await _service.IniciarSesion(dto);

            if (resultado is string)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}