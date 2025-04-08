using Entity.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Data.FormData;

public class FormDataFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private string _currentProvider;

    public FormDataFactory(
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
        _currentProvider = configuration["DatabaseProvider"] ?? "postgresql";
    }

    public IFormData CreateFormData()
    {
        return CreateFormData(_currentProvider);
    }

    public IFormData CreateFormData(string provider)
    {
        if (string.IsNullOrEmpty(provider))
        {
            provider = _currentProvider;
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        ApplicationDbContext context;

        switch (provider.ToLower())
        {
            case "postgresql":
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
                context = new ApplicationDbContext(optionsBuilder.Options, _configuration);
                return new FormDataPostgreSQL(context, _loggerFactory.CreateLogger<FormDataPostgreSQL>());

            case "mysql":
                optionsBuilder.UseMySql(
                    _configuration.GetConnectionString("MySqlConnection"),
                    ServerVersion.AutoDetect(_configuration.GetConnectionString("MySqlConnection")));
                context = new ApplicationDbContext(optionsBuilder.Options, _configuration);
                return new FormDataMysql(context, _loggerFactory.CreateLogger<FormDataMysql>());

            case "sqlserver":
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnectionServer"));
                context = new ApplicationDbContext(optionsBuilder.Options, _configuration);
                return new FormDataMysqlServer(context, _loggerFactory.CreateLogger<FormDataMysqlServer>());

            default:
                throw new NotSupportedException($"El proveedor de base de datos '{provider}' no está soportado.");
        }
    }

    public void SetCurrentProvider(string provider)
    {
        _currentProvider = provider.ToLower();
    }
}