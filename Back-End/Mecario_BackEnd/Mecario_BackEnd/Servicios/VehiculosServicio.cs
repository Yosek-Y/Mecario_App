using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Modelos.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Mecario_BackEnd.Servicios
{
    public class VehiculosServicio
    {
        private readonly ContextoBD _context;

        public VehiculosServicio(ContextoBD context)
        {
            _context = context;
        }

        //Método para agregar vehículo a un cliente
        public async Task<string> AgregarVehiculo(AgregarVehiculoDTO dto)
        {
            //VALIDACIONES 
            if (dto == null)
                throw new ArgumentException("Los datos enviados están vacíos.");

            if (string.IsNullOrWhiteSpace(dto.placa))
                throw new ArgumentException("La placa es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.marca))
                throw new ArgumentException("La marca es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.modelo))
                throw new ArgumentException("El modelo es obligatorio.");

            if (dto.anio < 1900 || dto.anio > DateTime.Now.Year + 1)
                throw new ArgumentException("El año ingresado no es válido.");

            //Validar el ID del cliente
            var cliente = await _context.Clientes.FindAsync(dto.idCliente);
            if (cliente == null)
                throw new ArgumentException("El cliente especificado no existe.");

            //Validar placa única
            bool placaExiste = await _context.Vehiculos.AnyAsync(v => v.placa == dto.placa);
            if (placaExiste)
                throw new ArgumentException("La placa ingresada ya existe. Debe ser única.");

            //Objeto para la base 
            var nuevo = new Vehiculos
            {
                placa = dto.placa,
                marca = dto.marca,
                modelo = dto.modelo,
                anio = dto.anio,
                color = dto.color,
                numeroChasis = dto.numeroChasis,
                idCliente = dto.idCliente
            };

            _context.Vehiculos.Add(nuevo);
            await _context.SaveChangesAsync();

            return "Vehículo agregado correctamente al cliente.";
        }

        //Obtener todos los vehiculos de un cliente en especifico
        public async Task<List<VehiculosClienteDTO>> ListarVehiculosCliente(int idCliente)
        {
            var vehiculos = await _context.Vehiculos
                .Where(v => v.idCliente == idCliente)
                .Select(v => new VehiculosClienteDTO
                {
                    idVehiculo = v.idVehiculo,
                    placa = v.placa,
                    marca = v.marca,
                    modelo = v.modelo,
                    anio = v.anio,
                    color = v.color,
                    numeroChasis = v.numeroChasis
                })
                .ToListAsync();

            return vehiculos;
        }
    }
}