using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Modelos.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Mecario_BackEnd.Servicios
{
    public class ClientesServicio
    {
        private readonly ContextoBD _context;

        public ClientesServicio(ContextoBD context)
        {
            _context = context;
        }

        //Validar correo válido usando MailAddress
        private bool CorreoEsValido(string correo)
        {
            try
            {
                var mail = new MailAddress(correo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Método para agregar nuevo cliente
        public async Task<Clientes> AgregarCliente(AgregarClienteNuevoDTO dto)
        {
            //VALIDACIONES
            if (dto == null)
                throw new ArgumentException("Los datos enviados están vacíos.");

            if (string.IsNullOrWhiteSpace(dto.nombreCliente))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.telefonoCliente))
                throw new ArgumentException("El teléfono es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.correoCliente))
                throw new ArgumentException("El correo es obligatorio.");

            if (!CorreoEsValido(dto.correoCliente))
                throw new ArgumentException("El correo ingresado no es válido.");

            if (string.IsNullOrWhiteSpace(dto.direccionCliente))
                throw new ArgumentException("La dirección es obligatoria.");

            // Validar correo único
            bool correoExiste = await _context.Clientes
                .AnyAsync(c => c.correoCliente == dto.correoCliente);

            if (correoExiste)
                throw new ArgumentException("El correo ya existe. Debe ser único.");

            // CREACIÓN DEL OBJETO
            var nuevo = new Clientes
            {
                nombreCliente = dto.nombreCliente,
                telefonoCliente = dto.telefonoCliente,
                correoCliente = dto.correoCliente,
                direccionCliente = dto.direccionCliente
            };

            _context.Clientes.Add(nuevo);
            await _context.SaveChangesAsync();

            return nuevo;
        }

        //Listar todos los clientes de la base
        public async Task<List<TodosLosClientesDTO>> ListarClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();

            var lista = clientes.Select(c => new TodosLosClientesDTO
            {
                nombreCliente = c.nombreCliente,
                telefonoCliente = c.telefonoCliente,
                correoCliente = c.correoCliente,
                direccionCliente = c.direccionCliente
            }).ToList();

            return lista;
        }

        //Buscar un cliente en especifico por nombre o correo
        public async Task<List<BuscarClienteDTO>> BuscarCliente(string? nombre, string? correo)
        {
            if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("Debe proporcionar un nombre o un correo.");

            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(c => c.nombreCliente.Contains(nombre));
            }

            if (!string.IsNullOrWhiteSpace(correo))
            {
                query = query.Where(c => c.correoCliente.ToLower() == correo.ToLower());
            }

            var clientes = await query.ToListAsync();

            if (clientes.Count == 0)
                throw new Exception("No se encontraron clientes con los datos proporcionados.");

            // Mapear a DTO
            return clientes.Select(c => new BuscarClienteDTO
            {
                idCliente = c.idCliente,
                nombreCliente = c.nombreCliente,
                telefonoCliente = c.telefonoCliente,
                correoCliente = c.correoCliente,
                direccionCliente = c.direccionCliente
            }).ToList();
        }
    }
}