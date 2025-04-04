using Business;
using Data;
using Entity.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
 

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<RolData>();  
builder.Services.AddScoped<RolBusiness>();

builder.Services.AddScoped<UserData>();
builder.Services.AddScoped<UserBusiness>();

builder.Services.AddScoped<FormData>();
builder.Services.AddScoped<FormBusiness>();

builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<ModuleBusinness>();

builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<PermissionBusinness>();

builder.Services.AddScoped<PersonData>();
builder.Services.AddScoped<PersonBusiness>();

builder.Services.AddScoped<RolUserData>();
builder.Services.AddScoped<UserRolBusiness>();

builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<ModuleBusinness>();

builder.Services.AddScoped<ModuleFormData>();
builder.Services.AddScoped<ModuleFormBusinness>();


builder.Services.AddScoped<RolFormPermissionData>();
builder.Services.AddScoped<RolFormPermissionBusiness>();


var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrígenesPermitidos");
Console.WriteLine($"OrígenesPermitidos: {OrigenesPermitidos}");


builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(politica =>
    {
        politica.WithOrigins(OrigenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });
});

// dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
opciones.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();