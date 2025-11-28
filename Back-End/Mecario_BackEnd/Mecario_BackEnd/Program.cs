using Mecario_BackEnd.DBContexs;
using Mecario_BackEnd.Servicios;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
// Configurar Entity Framework Core con SQL Server
builder.Services.AddDbContext<ContextoBD>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddScoped<PiezasServicio>();
builder.Services.AddScoped<UsuariosServicio>();
builder.Services.AddScoped<CasosServicio>();
builder.Services.AddScoped<ClientesServicio>();
builder.Services.AddScoped<VehiculosServicio>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Habilitar CORS para permitir solicitudes desde cualquier origen (HTML local)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Aplicar la política de CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
