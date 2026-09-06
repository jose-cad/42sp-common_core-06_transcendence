using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Carrega a Connection String do Ambiente (.env injetado via Docker ou Local)
var connectionString = builder.Configuration.GetConnectionString("Default");

// 2. Configura o Banco de Dados PostgreSQL (Requisito Multi-tenant / Concorrência)
// builder.Services.AddDbContext(options => options.UseNpgsql(connectionString));

// 3. Configura a Base do Sistema de Autenticação JWT
var jwtSecret = builder.Configuration["Jwt:Secret"]
             ?? builder.Configuration["JWT_SECRET"]
             ?? throw new InvalidOperationException("JWT_SECRET não configurado.");
builder.Services.AddAuthentication(options =>
{
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuerSigningKey = true,
IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
ValidateIssuer = false,
ValidateAudience = false
};
});

// 4. Adiciona suporte a Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Configura o Swagger para o Critério de Aceite (Mesmo em produção/Docker se necessário)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fleet Management API v1");
c.RoutePrefix = "swagger"; // Acessível em https://localhost:8443/swagger
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 6. Rota de Health-Check Simples exigida na issue
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.MapControllers();

app.Run();
