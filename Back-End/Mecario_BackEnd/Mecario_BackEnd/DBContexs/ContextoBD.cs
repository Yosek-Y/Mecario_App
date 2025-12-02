using Mecario_BackEnd.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Mecario_BackEnd.DBContexs
{
    public class ContextoBD : DbContext
    {
        public ContextoBD (DbContextOptions <ContextoBD> options) 
            : base (options) 
        {
        
        }
        public DbSet <Usuarios> Usuarios { get; set; }

        public DbSet <Vehiculos> Vehiculos { get; set; }

        public DbSet <Piezas> Piezas { get; set; }

        public DbSet <OrdenesServicio> OrdenesServicios { get; set; }

        public DbSet <ServiciosMecanicos> ServiciosMecanicos {get; set;}

        public DbSet <DetallesPiezas> DetallesPiezas { get; set; }

        public DbSet <DetallesCaso> DetallesCasos { get; set; }

        public DbSet <Clientes> Clientes { get; set; }

        public DbSet <Casos> Casos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================
            // ||  TABLA CLIENTES  ||
            // ======================
            modelBuilder.Entity<Clientes>(entity =>
            {
                //ID AUTOGENERADO
                entity.HasKey(e => e.idCliente);
                entity.Property(e => e.idCliente).ValueGeneratedOnAdd();
                //CAMPOS OBLIGATORIOS (NOT NULL)
                entity.Property(e => e.nombreCliente).IsRequired();      //Nombre completo del cliente
                entity.Property(e => e.telefonoCliente).IsRequired();    //Telefono del cliente
                entity.Property(e => e.correoCliente).IsRequired();      //Correo del cliente
                entity.HasIndex(e => e.correoCliente).IsUnique();        //correo unico
                entity.Property(e => e.direccionCliente).IsRequired();   //Dirección del cliente
                //FK A VEHICULOS
                entity.HasMany(e => e.Vehiculos)
                      .WithOne(v => v.Clientes)
                      .HasForeignKey(v => v.idCliente)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =======================
            // ||  TABLA VEHICULOS  ||
            // =======================
            modelBuilder.Entity<Vehiculos>(entity =>
            {
                //ID AUTOGENERADO
                entity.HasKey(e => e.idVehiculo);
                entity.Property(e => e.idVehiculo).ValueGeneratedOnAdd();
                //CAMPOS OBLIGATORIOS (NOT NULL)
                entity.Property(e => e.placa).IsRequired().HasMaxLength(6);                    //Placa del vehiculo (AABB34)
                entity.Property(e => e.marca).IsRequired();                                    //Marca del vehiculo (Nissan, Toyota, etc..)
                entity.Property(e => e.modelo).IsRequired();                                   //Modelo del vehiculo (Frontier, Hilux, etc..)
                entity.Property(e => e.anio).IsRequired();                                     //Año del vehiculo 
                entity.Property(e => e.color).IsRequired();                                    //Color del vehiculo
                entity.Property(e => e.numeroChasis).IsRequired().HasMaxLength(16);            //Numero del chasis (AAAAAABBBBB121313)
                entity.HasIndex(e => e.numeroChasis).IsUnique();                               //El numero de chasis debe ser unico
                //FK A ORDENES DE SERVICIOS
                entity.HasMany(e => e.ordenesServicios)
                      .WithOne(o => o.vehiculos)
                      .HasForeignKey(o => o.idVehiculo)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =================================
            // ||  TABLA ORDENES DE SERVICIO  ||
            // =================================
            modelBuilder.Entity<OrdenesServicio>(entity =>
            {
                //ID AUTOGENERADO
                entity.HasKey(e => e.idOrden);
                entity.Property(e => e.idOrden).ValueGeneratedOnAdd();
                //CAMPOS OBLIGATORIOS (NOT NULL)
                entity.Property(e => e.tipoServicio).IsRequired().HasConversion<string>();        //Tipo de servicio (Mantenimiento o Reparacion)
                entity.Property(e => e.diagnosticoInicial).IsRequired();                          //String de todos las acciones a hacer
                entity.Property(e => e.costoInicial).IsRequired();                                //Costo calculado en base a las acciones

                //FK A CASOS
                entity.HasMany(o => o.casos)
                      .WithOne(c => c.ordenesServicio)
                      .HasForeignKey(c => c.idOrdenServicio)
                      .OnDelete(DeleteBehavior.Cascade);

                // RELACIÓN MUCHOS A MUCHOS CON SERVICIOS
                entity.HasMany(o => o.Servicios)
                      .WithMany(s => s.OrdenesServicio)
                      .UsingEntity<Dictionary<string, object>>(
                            "OrdenServicio_Servicio",
                            j => j
                                  .HasOne<ServiciosMecanicos>()
                                  .WithMany()
                                  .HasForeignKey("idServicio")
                                  .OnDelete(DeleteBehavior.Cascade),
                            j => j
                                  .HasOne<OrdenesServicio>()
                                  .WithMany()
                                  .HasForeignKey("idOrden")
                                  .OnDelete(DeleteBehavior.Cascade),
                            j =>
                            {
                                j.HasKey("idOrden", "idServicio");
                                j.ToTable("OrdenServicio_Servicio"); // Nombre de la tabla puente
                            }
                      );
            });

            // ================================
            // ||  TABLA SERVICIOS MECANICOS ||
            // ================================
            modelBuilder.Entity<ServiciosMecanicos>(entity =>
            {
                entity.HasKey(s => s.idServicio);
                entity.Property(s => s.idServicio).ValueGeneratedOnAdd();
                entity.Property(s => s.servicio).IsRequired();                                    //Que servicio es
                entity.Property(s => s.tipoServicio).IsRequired().HasConversion<string>();        // Guarda enum como string
                entity.Property(s => s.precio).IsRequired().HasColumnType("decimal(10,2)");
            });

            // ======================
            // ||  TABLA USUARIOS  ||
            // ======================
            modelBuilder.Entity<Usuarios>(entity =>
            {
                //ID AUTOGENERADO
                entity.HasKey(e => e.idUsuario);
                entity.Property(e => e.idUsuario).ValueGeneratedOnAdd();
                //CAMPOS OBLIGATORIOS (NOT NULL)
                entity.Property(e => e.nombreUsuario).IsRequired();                             //Nombre completo del usuario
                entity.Property(e => e.telefonoUsuario).IsRequired();                           //Numero de telefo del usuario
                entity.Property(e => e.correoUsuario).IsRequired();                             //Correo del usuario
                entity.HasIndex(e => e.correoUsuario).IsUnique();                               //Correo unico
                entity.Property(e => e.direccionUsuario).IsRequired();                          //Direccion del usuario
                entity.Property(e => e.tipoUsuario).IsRequired().HasConversion<string>();       //Tipo de usuario o Admin o Mecanico
                entity.Property(e => e.userName).IsRequired();                                  //Nombre del usuario para el app
                entity.HasIndex(e => e.userName).IsUnique();                                    //Que sea unico ese username
                entity.Property(e => e.userPassword).IsRequired();                              //contraseña del user para el app

                //FK A CASOS
                entity.HasMany(u => u.casos)
                      .WithOne(c => c.usuarios)
                      .HasForeignKey(c => c.idUsuario)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ===================
            // ||  TABLA CASOS  ||
            // ===================
            modelBuilder.Entity<Casos>(entity =>
            {
                //ID AUTOGENERADO
                entity.HasKey(e => e.idCaso);
                entity.Property(e => e.idCaso).ValueGeneratedOnAdd();

                //FechaInicio OBLIGATORIO (NOT NULL), FechaFin OPCIONAL (NULL)
                entity.Property(e => e.fechaInicio).IsRequired();                           //Fecha cuando el mecanico INICIA la reparacion o el mantenimiento
                entity.Property(e => e.fechaFin).IsRequired(false);                         //Fecha cuando el mecanico TERMINA la reparacion o el mantenimiento
                //CAMPOS OBLIGATORIOS (NOT NULL)
                entity.Property(e => e.horasTrabajadas).IsRequired();                       //Horas totales en las que el mecanico trabajo en el caso
                entity.Property(e => e.estadoCaso).IsRequired().HasConversion<string>();    //Estado del caso en el momento (No iniciado, En terminado, Terminado)
                entity.Property(e => e.totalCaso).IsRequired();                             //Precio total del caso

                //FK CON DETALLES CASO
                entity.HasMany(e => e.detallesCaso)
                      .WithOne(d => d.casos)
                      .HasForeignKey(d => d.idCaso)
                      .OnDelete(DeleteBehavior.Cascade);

                //FK CON DETALLES PIEZA
                entity.HasMany(e => e.detallesPieza)
                      .WithOne(dp => dp.casos)
                      .HasForeignKey(dp => dp.idCaso)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ==============================
            // ||  TABLA DETALLES DE CASO  ||
            // ==============================
            modelBuilder.Entity<DetallesCaso>(entity =>
            {
                //ID AUTOGENERADO
                entity.HasKey(e => e.idDetalleCaso);
                entity.Property(e => e.idDetalleCaso).ValueGeneratedOnAdd();
                //CAMPOS OBLIGATORIOS (NOT NULL)
                entity.Property(e => e.tareaRealizada).IsRequired();            //Tarea que se realizo en el caso
                entity.Property(e => e.hora).IsRequired();                      //Hora en la que se termino una de las tareas del 
            });

            // ====================
            // ||  TABLA PIEZAS  ||
            // ====================
            modelBuilder.Entity<Piezas>(entity =>
            {
                //ID AUTOGENERADO
                entity.HasKey(e => e.idPieza);
                entity.Property(e => e.idPieza).ValueGeneratedOnAdd();
                //CAMPOS OBLIGATORIOS (NOT NULL)
                entity.Property(e => e.nombrePieza).IsRequired();                                //Nombre de la pieza
                entity.Property(e => e.categoriaPieza).IsRequired().HasConversion<string>();     //Categoria de la pieza
                entity.Property(e => e.descripcionPieza).IsRequired();                           //Descripcion de la pieza
                entity.Property(e => e.precioUnidad).IsRequired();                               //Precio de cada unidad de la pieza
                entity.Property(e => e.stockActual).IsRequired();                                //Cantidad en stock
                entity.Property(e => e.codigoPieza).IsRequired().HasMaxLength(6);                //Codigo del producto de maximo 6 caracteres
                entity.HasIndex(e => e.codigoPieza).IsUnique();                                  //El codigo debe ser unico
                //FK A DETALLES PIEZA
                entity.HasMany(p => p.detallesPieza)
                      .WithOne(dp => dp.piezas)
                      .HasForeignKey(dp => dp.idPieza)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ===========================================
            // ||  TABLA DETALLES PIEZA (PK COMPUESTA)  ||
            // ===========================================
            modelBuilder.Entity<DetallesPiezas>(entity =>
            {
                //ID GENERADO POR ID_CASO Y ID_PIEZA
                entity.HasKey(e => new { e.idCaso, e.idPieza });
                //CAMPOS OBLIGATORIOS (NOT NULL)
                entity.Property(e => e.cantidad).IsRequired();          //Cantidad usada en total  
                entity.Property(e => e.precioUnitario).IsRequired();    //Precio de cada pieza
                entity.Property(e => e.subtotal).IsRequired();          //calculado por cantidad y precio unitarios
            });
        }   
    }
}