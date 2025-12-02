using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos.DTOs;
using Mecario_BackEnd.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Mecario_BackEnd.Servicios
{
    public class PiezasServicio
    {
        private readonly ContextoBD _context;

        public PiezasServicio(ContextoBD context)
        {
            _context = context;
        }

        //Metodo para que el admin agregue una nueva pieza al "inventario"
        public async Task<Piezas> AgregarPiezaNueva(AgregarPiezaNuevaDTO dto)
        {
            //VALIDACIONES

            if (dto == null)
                throw new ArgumentException("Los datos enviados están vacíos.");

            if (string.IsNullOrWhiteSpace(dto.nombrePieza))
                throw new ArgumentException("El nombre de la pieza es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.descripcionPieza))
                throw new ArgumentException("La descripción es obligatoria.");

            if (dto.precioUnidad <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0.");

            if (dto.stockActual < 0)
                throw new ArgumentException("El stock no puede ser negativo.");

            // Validación enum CategoriaPieza
            if (!Enum.IsDefined(typeof(Piezas.CategoriaPieza), dto.categoriaPieza))
                throw new ArgumentException("La categoría enviada no existe en la lista de categorías.");

            //Verificar que el codigo sea unico y no exista
            bool codigoExiste = await _context.Piezas.AnyAsync(p => p.codigoPieza == dto.codigoPieza);

            if (codigoExiste)
                throw new ArgumentException("El código ingresado ya existe. Debe ser único.");

            //  CREACIÓN DEL OBJETO MODELO
            var nuevaPieza = new Piezas
            {
                nombrePieza = dto.nombrePieza,
                categoriaPieza = (Piezas.CategoriaPieza)dto.categoriaPieza, //Agarra y transforma el Enum a la categoria correspontiende
                descripcionPieza = dto.descripcionPieza,
                codigoPieza = dto.codigoPieza,
                precioUnidad = dto.precioUnidad,
                stockActual = dto.stockActual
            };

            //GUARDAR EN BASE DE DATOS
            _context.Piezas.Add(nuevaPieza);
            await _context.SaveChangesAsync();

            return nuevaPieza;
        }

        //Metodo para agregar stock a una pieza existente
        public async Task<Piezas> AgregarStock(AgregarReducirStockDTO dto)
        {
            //VALIDACIONES
            if (dto == null)
                throw new ArgumentException("Los datos enviados están vacíos.");

            if (string.IsNullOrWhiteSpace(dto.codigoPieza))
                throw new ArgumentException("El código de la pieza es obligatorio.");

            if (dto.cantidad <= 0)
                throw new ArgumentException("La cantidad a sumar debe ser mayor a 0.");

            //Busca la pieza por código
            var pieza = await _context.Piezas.FirstOrDefaultAsync(p => p.codigoPieza == dto.codigoPieza);

            if (pieza == null)
                throw new ArgumentException("No existe una pieza con ese código.");

            //Sumar al stock
            pieza.stockActual += dto.cantidad;

            //Guardar cambios
            await _context.SaveChangesAsync();

            return pieza;
        }

        //Metodo para reducir el stock a una pieza existente (Si basicamente lo reutilizamos)
        public async Task<Piezas> ReducirStock(AgregarReducirStockDTO dto)
        {
            //VALIDACIONES
            if (dto == null)
                throw new ArgumentException("Los datos enviados están vacíos.");

            if (string.IsNullOrWhiteSpace(dto.codigoPieza))
                throw new ArgumentException("El código de la pieza es obligatorio.");

            if (dto.cantidad <= 0)
                throw new ArgumentException("La cantidad a sumar debe ser mayor a 0.");

            //Busca la pieza por código
            var pieza = await _context.Piezas.FirstOrDefaultAsync(p => p.codigoPieza == dto.codigoPieza);

            if (pieza == null)
                throw new ArgumentException("No existe una pieza con ese código.");

            //Restar al stock
            pieza.stockActual -= dto.cantidad;

            //Guardar cambios
            await _context.SaveChangesAsync();

            return pieza;
        }

        //Metodo para ver todas las piezas 
        public async Task<List<TodasLasPiezasDTO>> ObtenerTodasLasPiezasAsync()
        {
            var piezas = await _context.Piezas.ToListAsync();

            return piezas.Select(p => new TodasLasPiezasDTO
            {
                NombrePieza = p.nombrePieza,
                CategoriaPieza = p.categoriaPieza.ToString(), // Convertir enum a string
                DescripcionPieza = p.descripcionPieza,
                CodigoPieza = p.codigoPieza,
                PrecioUnidad = p.precioUnidad,
                StockActual = p.stockActual
            }).ToList();
        }

        //Metodo para ver las piezas por categoria
        public async Task<List<TodasLasPiezasDTO>> ObtenerPiezasPorCategoriaAsync(int categoriaId)
        {
            // Validar que el ID de categoría sea válido
            if (categoriaId < 1 || categoriaId > 12)
            {
                throw new ArgumentException("El ID de categoría debe estar entre 1 y 12.");
            }

            var piezas = await _context.Piezas
                .Where(p => (int)p.categoriaPieza == categoriaId)
                .ToListAsync();

            return piezas.Select(p => new TodasLasPiezasDTO
            {
                NombrePieza = p.nombrePieza,
                CategoriaPieza = p.categoriaPieza.ToString(),
                DescripcionPieza = p.descripcionPieza,
                CodigoPieza = p.codigoPieza,
                PrecioUnidad = p.precioUnidad,
                StockActual = p.stockActual
            }).ToList();
        }

    }
}