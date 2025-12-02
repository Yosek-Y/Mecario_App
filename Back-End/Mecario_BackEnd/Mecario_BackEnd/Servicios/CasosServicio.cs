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

        //Metodo para crear un caso
        public async Task<Casos> CrearCasoAsync(CrearCasoDTO dto)
        {
            // Validar que la orden exista
            var orden = await _context.OrdenesServicios.FirstOrDefaultAsync(o => o.idOrden == dto.idOrdenServicio);
            if (orden == null)
                throw new Exception("La orden de servicio no existe.");

            var caso = new Casos
            {
                idOrdenServicio = dto.idOrdenServicio,
                idUsuario = dto.idUsuario ?? 0,       // 0 si no hay usuario asignado
                fechaInicio = DateTime.MinValue,      // Inicial
                fechaFin = null,                       // Sin terminar
                horasTrabajadas = 0,
                estadoCaso = Casos.EstadoCaso.noEmpezado,
                totalCaso = 0
            };

            _context.Casos.Add(caso);
            await _context.SaveChangesAsync();
            return caso;
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

        //Metodo para abrir un caso
        public async Task<Casos> AbrirCasoAsync(AbrirCasoDTO dto)
        {
            var caso = await _context.Casos.FirstOrDefaultAsync(c => c.idCaso == dto.idCaso);
            if (caso == null)
                throw new Exception("El caso especificado no existe.");

            caso.fechaInicio = dto.fechaInicio;
            caso.estadoCaso = Casos.EstadoCaso.enProceso;

            _context.Casos.Update(caso);
            await _context.SaveChangesAsync();

            return caso;
        }

        //Metodo para cuando un caso se continuara despues
        public async Task<Casos> ContinuarCasoAsync(ContinuarCasoDTO dto)
        {
            var caso = await _context.Casos.FirstOrDefaultAsync(c => c.idCaso == dto.idCaso);
            if (caso == null)
                throw new Exception("El caso especificado no existe.");

            // Sumamos las horas recibidas a las existentes
            caso.horasTrabajadas += dto.horasTrabajadas;

            _context.Casos.Update(caso);
            await _context.SaveChangesAsync();

            return caso;
        }

        //Metodo para terminar un caso 
        public async Task<Casos> CerrarCasoAsync(CerrarCasoDTO dto)
        {
            var caso = await _context.Casos.FirstOrDefaultAsync(c => c.idCaso == dto.idCaso);
            if (caso == null)
                throw new Exception("El caso especificado no existe.");

            // Sumar horas trabajadas
            caso.horasTrabajadas += dto.horasTrabajadas;

            // Colocar fecha de finalización
            caso.fechaFin = dto.fechaFin;

            // Cambiar estado a terminado
            caso.estadoCaso = Casos.EstadoCaso.terminado;

            _context.Casos.Update(caso);
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
                    totalCaso = c.totalCaso,
                    fechaInicio = c.fechaInicio,
                    fechaFin = c.fechaFin
                })
                .FirstOrDefaultAsync();

            return factura; // null si no existe
        }

        // Método para listar todas las facturas de todos los casos
        public async Task<List<TodasLasFacturasDTO>> ListarTodasLasFacturas()
        {
            // Trae todos los casos junto con los datos del mecánico
            var lista = await (
                from c in _context.Casos.AsNoTracking()
                join u in _context.Usuarios.AsNoTracking()
                    on c.idUsuario equals u.idUsuario
                orderby c.idCaso
                select new TodasLasFacturasDTO
                {
                    idCaso = c.idCaso,
                    totalCaso = c.totalCaso,
                    fechaInicio = c.fechaInicio,
                    fechaFin = c.fechaFin,
                    horasTrabajadas = c.horasTrabajadas,
                    idUsuario = u.idUsuario,
                    nombreMecanico = u.nombreUsuario ?? string.Empty
                }
            ).ToListAsync();

            return lista; // devuelve lista vacía si no hay casos
        }


        // Listar casos por estado (status)
        public async Task<List<CasosSegunStatusDTO>> ListarCasosPorStatusAsync (string status, CancellationToken ct = default)
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
    }
}