using System.Text;
using ECommerce.API.Filters;
using ECommerce.API.Middleware;
using ECommerce.Business.Abstract;
using ECommerce.Business.Concrete;
using ECommerce.Business.Configs;
using ECommerce.Business.Mapping;
using ECommerce.Business.Validators;
using ECommerce.Data;
using ECommerce.Data.Abstract;
using ECommerce.Data.Concrete;
using ECommerce.Entity.Concrete;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'PostgreSqlConnection' bulunamadı!");
}

builder.Services.AddDbContext<ECommerceDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.Configure<AppUrlConfig>(builder.Configuration.GetSection("AppUrlConfig"));
builder.Services.Configure<CorsConfig>(builder.Configuration.GetSection("CorsSettings"));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateDtoValidator>();

var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Şifre politikalarımız
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;

    // Kullanıcı politikalarımız
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ECommerceDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtConfig!.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtConfig.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
    };
});

var corsConfig = builder.Configuration.GetSection("CorsSettings").Get<CorsConfig>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedSpesificOrigins", policy =>
    {
        if (corsConfig!.AllowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(corsConfig.AllowedOrigins)
                .WithMethods(corsConfig.AllowedMethods)
                .WithHeaders(corsConfig.AllowedHeaders);
        }
        else
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
        if (corsConfig.AllowCredentials)
        {
            policy.AllowCredentials();
        }
    });

});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    var migrationLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    
    try
    {
        var connString = configuration.GetConnectionString("PostgreSqlConnection");
        var maskedConnString = connString?.Replace("Password=", "Password=***");
        migrationLogger.LogInformation("=== Migration İşlemi Başlatılıyor ===");
        migrationLogger.LogInformation("Connection String: {ConnectionString}", maskedConnString);
        migrationLogger.LogInformation("Veritabanı bağlantısı kontrol ediliyor...");
        
        var canConnect = await dbContext.Database.CanConnectAsync();
        migrationLogger.LogInformation("Veritabanına bağlanılabilir: {CanConnect}", canConnect);
        
        if (!canConnect)
        {
            migrationLogger.LogError("❌ Veritabanına bağlanılamıyor! Connection string'i kontrol edin.");
            throw new InvalidOperationException("Veritabanına bağlanılamıyor!");
        }
        
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        migrationLogger.LogInformation("Bekleyen migration sayısı: {Count}", pendingMigrations.Count());
        
        if (pendingMigrations.Any())
        {
            migrationLogger.LogInformation("Uygulanacak migration'lar: {Migrations}", string.Join(", ", pendingMigrations));
            await dbContext.Database.MigrateAsync();
            migrationLogger.LogInformation("✅ Veri tabanı migration işlemleri başarıyla uygulandı!");
        }
        else
        {
            migrationLogger.LogInformation("✅ Tüm migration'lar zaten uygulanmış. Bekleyen migration yok.");
        }
    }
    catch (Exception ex)
    {
        migrationLogger.LogError(ex, "❌ Migration sırasında hata oluştu: {Message}", ex.Message);
        if (ex.InnerException != null)
        {
            migrationLogger.LogError("Inner Exception: {InnerException}", ex.InnerException.Message);
        }
        migrationLogger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
        throw;
    }
}
;

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowedSpesificOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
