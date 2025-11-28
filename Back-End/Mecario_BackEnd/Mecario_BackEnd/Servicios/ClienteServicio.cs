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
    }
}