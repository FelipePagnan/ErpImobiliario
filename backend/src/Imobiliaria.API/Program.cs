using System.Text;
using Imobiliaria.Application.Interfaces;
using Imobiliaria.Application.Services;
using Imobiliaria.Domain.Interfaces;
using Imobiliaria.Infrastructure.Data;
using Imobiliaria.Infrastructure.Repositories;
using Imobiliaria.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=imobiliaria.db"));

// Repositories
builder.Services.AddScoped<IImovelRepository, ImovelRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProprietarioRepository, ProprietarioRepository>();
builder.Services.AddScoped<ICorretorRepository, CorretorRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IFavoritoRepository, FavoritoRepository>();
builder.Services.AddScoped<IVisitaRepository, VisitaRepository>();
builder.Services.AddScoped<IContratoRepository, ContratoRepository>();
builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();
builder.Services.AddScoped<IComissaoRepository, ComissaoRepository>();
builder.Services.AddScoped<IInteressadoRepository, InteressadoRepository>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

// Services
builder.Services.AddScoped<IImovelService, ImovelService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFavoritoService, FavoritoService>();
builder.Services.AddScoped<IVisitaService, VisitaService>();
builder.Services.AddScoped<IContratoService, ContratoService>();
builder.Services.AddScoped<IFinanceiroService, FinanceiroService>();
builder.Services.AddScoped<ICrmService, CrmService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();

// JWT
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "ChaveSecretaPadrao12345678901234567890";
builder.Services.AddAuthentication(o => { o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; })
.AddJwtBearer(o => { o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "PagnanHubImoveis", ValidAudience = builder.Configuration["Jwt:Audience"] ?? "PagnanHubImoveis", IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)) }; });
builder.Services.AddAuthorization();

builder.Services.AddCors(o => o.AddPolicy("AllowFrontend", p => p.WithOrigins("http://localhost:3000", "http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pagnan Hub Imóveis — API", Version = "v4", Description = "API do sistema Pagnan Hub Imóveis" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Description = "JWT Authorization. Exemplo: \"Bearer {token}\"", Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey, Scheme = "Bearer" });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() } });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope()) { SeedData.Initialize(scope.ServiceProvider.GetRequiredService<AppDbContext>()); }

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
