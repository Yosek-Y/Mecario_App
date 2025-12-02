using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Mecario_BackEnd.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClientesServicio _service;

        public ClientesController(ClientesServicio service)
        {
            _service = service;
        }

        //Protocolo HTTP Post para añadir un nuevo cliente a la base
        //POST: Api/Clientes/NuevoCliente
        [HttpPost("NuevoCliente")]
        public async Task<IActionResult> AgregarCliente([FromBody] AgregarClienteNuevoDTO dto)
        {
            try
            {
                var nuevoCliente = await _service.AgregarCliente(dto);

                return Ok(new
                {
                    mensaje = "Cliente agregado correctamente",
                    data = new
                    {
                        nuevoCliente.nombreCliente,
                        nuevoCliente.telefonoCliente,
                        nuevoCliente.correoCliente,
                        nuevoCliente.direccionCliente
                    }
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

        //Protocolo HTTP Get para ver todos los clientes
        //POST: Api/Clientes/ListarClientes
        [HttpGet("ListarClientes")]
        public async Task<IActionResult> ListarClientes()
        {
            try
            {
                var lista = await _service.ListarClientes();
                return Ok(new
                {
                    mensaje = "Lista de clientes obtenida correctamente",
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

        //Protocolo HTTP Get para buscar un cliente
        //POST: Api/Clientes/Buscar
        [HttpGet("Buscar")]
        public async Task<IActionResult> BuscarCliente([FromQuery] string? nombre, [FromQuery] string? correo)
        {
            try
            {
                var resultado = await _service.BuscarCliente(nombre, correo);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}