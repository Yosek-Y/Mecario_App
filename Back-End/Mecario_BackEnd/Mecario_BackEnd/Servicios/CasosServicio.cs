using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Modelos;
using Mecario_BackEnd.Modelos.DTOs;
using Microsoft.EntityFrameworkCore;
using static Mecario_BackEnd.Modelos.Casos;

namespace Mecario_BackEnd.Servicios
{
    public class CasosServicio
    {
        private readonly ContextoBD _context;

        public CasosServicio(ContextoBD context)
        {
            _context = context;
        }

        //Metodo para que se listen los casos de un mecanico
        public async Task<List<CasosDeMecanicoDTO>> ListarCasosPorMecanico(int idMecanico)
        {
            // Validar que exista el usuario y que sea mecánico
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == idMecanico);

            if (usuario == null)
                throw new ArgumentException("El mecánico no existe.");

            if (usuario.tipoUsuario != Usuarios.Tipousuario.Mecanico)
                throw new ArgumentException("El usuario no es un mecánico.");

            //Obtener los casos
            var casos = await _context.Casos.Where(c => c.idUsuario == idMecanico).ToListAsync();

            if (casos.Count == 0)
                throw new ArgumentException("El mecánico no tiene casos asignados.");

            // Convertir a DTO
            var lista = casos.Select(c => new CasosDeMecanicoDTO
            {
                fechaInicio = c.fechaInicio,
                fechaFin = c.fechaFin,
                horasTrabajadas = c.horasTrabajadas,
                estadoCaso = c.estadoCaso.ToString(),
                totalCaso = c.totalCaso
            }).ToList();

            return lista;
        }

        // Método para que el admin asigne un caso a un mecánico
        public async Task<string> AsignarCasoPorAdmin(AsignarCasoAdminDTO dto)
        {
            if (dto == null)
                throw new ArgumentException("Los datos enviados están vacíos.");

            // Busca el caso
            var caso = await _context.Casos.FirstOrDefaultAsync(c => c.idCaso == dto.idCaso);
            if (caso == null)
                throw new ArgumentException("El caso indicado no existe.");

            // Busca el usuario
            var mecanico = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == dto.idMecanico);
            if (mecanico == null)
                throw new ArgumentException("El mecánico no existe.");

            if (mecanico.tipoUsuario != Usuarios.Tipousuario.Mecanico)
                throw new ArgumentException("El usuario seleccionado no es un mecánico.");

            // Asigna
            caso.idUsuario = dto.idMecanico;

            await _context.SaveChangesAsync();

            return "Caso asignado correctamente al mecánico.";
        }

        // Método para asignar un mecánico a un caso
        public async Task<Casos> AsignarMecanico(MecanicoSeAsignaCasoDTO dto)
        {
            // Valida que exista el caso
            var caso = await _context.Casos.FirstOrDefaultAsync(c => c.idCaso == dto.idCaso);
            if (caso == null)
                throw new ArgumentException("El caso no existe.");

            // Valida que exista el mecánico
            var mecanico = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == dto.idMecanico);
            if (mecanico == null || mecanico.tipoUsuario != Usuarios.Tipousuario.Mecanico)
                throw new ArgumentException("El mecánico no existe o no es válido.");

            // Asigna al mecánico el caso
            caso.idUsuario = dto.idMecanico;

            await _context.SaveChangesAsync();
            return caso;
        }
        // NUEVO: Obtener factura (totalCaso) de un caso por su ID
        public async Task<FacturaCasoDTO?> ObtenerFacturaCasoAsync(int idCaso)
        {
            if (idCaso <= 0)
                throw new ArgumentException("El id del caso debe ser mayor que cero.");

            // Proyección directa al DTO para eficiencia
            var factura = await _context.Casos
                .AsNoTracking()
                .Where(c => c.idCaso == idCaso) // Ajusta si se llama distinto (ej: c.id)
                .Select(c => new FacturaCasoDTO
                {
                    idCaso = c.idCaso,
                    totalCaso = c.totalCaso,
                    fechaInicio = c.fechaInicio,
                    fechaFin = c.fechaFin
                })
                .FirstOrDefaultAsync();

            return factura; // null si no existe
        }

        // NUEVO: Listar todas las facturas de todos los casos
        public async Task<List<TodasLasFacturasDTO>> ListarFacturasDeCasosAsync(int? idMecanico = null, CancellationToken ct = default)
        {
            // Validar el mecánico si se envía idMecanico
            if (idMecanico.HasValue)
            {
                if (idMecanico.Value <= 0)
                    throw new ArgumentException("El id del mecánico debe ser mayor que cero.");

                var usuario = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.idUsuario == idMecanico.Value, ct);

                if (usuario is null)
                    throw new KeyNotFoundException($"No existe el mecánico con id {idMecanico.Value}.");

                // se verifica que debe ser Mecanico
                if (usuario.tipoUsuario != Usuarios.Tipousuario.Mecanico)
                    throw new ArgumentException($"El usuario con id {idMecanico.Value} no es un mecánico.");
            }

            var query =
                from c in _context.Casos.AsNoTracking()
                join u in _context.Usuarios.AsNoTracking()
                    on c.idUsuario equals u.idUsuario
                select new
                {
                    c.idCaso,
                    c.fechaInicio,
                    c.fechaFin,
                    c.horasTrabajadas,
                    c.totalCaso,
                    idMecanico = u.idUsuario,
                    mecanicoNombre = u.nombreUsuario 
                };

            if (idMecanico.HasValue)
            {
                query = query.Where(x => x.idMecanico == idMecanico.Value);
            }

            var lista = await query
                .OrderBy(x => x.idCaso)
                .Select(x => new TodasLasFacturasDTO
                {
                    idCaso = x.idCaso,
                    fechaInicio = x.fechaInicio,
                    fechaFin = x.fechaFin,
                    horasTrabajadas = x.horasTrabajadas,
                    totalCaso = x.totalCaso,
                    idUsuario = x.idMecanico,
                    nombreMecanico = x.mecanicoNombre ?? string.Empty
                })
                .ToListAsync(ct);

            // Nota: si el mecánico existe pero no tiene casos, se devuelve una lista vacía.
            return lista;
        }


        // Listar casos por estado (status)
        public async Task<List<CasosSegunStatusDTO>> ListarCasosPorStatusAsync(string status, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("El parámetro 'status' es obligatorio.");

            // Normalización mínima
            var input = status.Trim();

            EstadoCaso estadoEnum;

            // Intentar parse numérico (1, 2, 3)
            if (int.TryParse(input, out int num))
            {
                if (num == 1) estadoEnum = EstadoCaso.noEmpezado;
                else if (num == 2) estadoEnum = EstadoCaso.enProceso;
                else if (num == 3) estadoEnum = EstadoCaso.terminado;
                else
                    throw new ArgumentException("Valor numérico inválido. Use 1, 2 o 3.");
            }
            else
            {
                // Quitar espacios / guiones bajos / guiones
                var norm = input
                    .Replace(" ", "")
                    .Replace("_", "")
                    .Replace("-", "")
                    .ToLower();

                if (norm == "noempezado") estadoEnum = EstadoCaso.noEmpezado;
                else if (norm == "enproceso") estadoEnum = EstadoCaso.enProceso;
                else if (norm == "terminado") estadoEnum = EstadoCaso.terminado;
                else
                    throw new ArgumentException("Status inválido. Valores aceptados: noEmpezado, enProceso, terminado (o 1,2,3).");
            }

            // Consulta filtrada
            var lista = await (
                from c in _context.Casos.AsNoTracking()
                join u in _context.Usuarios.AsNoTracking()
                    on c.idUsuario equals u.idUsuario
                where c.estadoCaso == estadoEnum
                orderby c.idCaso
                select new CasosSegunStatusDTO
                {
                    idCaso = c.idCaso,
                    fechaInicio = c.fechaInicio,
                    fechaFin = c.fechaFin,
                    horasTrabajadas = c.horasTrabajadas,
                    estadoCaso = c.estadoCaso.ToString(),
                    totalCaso = c.totalCaso,            
                    idUsuario = u.idUsuario,
                    nombreMecanico = u.nombreUsuario    
                }
            ).ToListAsync(ct);

            return lista;
        }

    }
}
