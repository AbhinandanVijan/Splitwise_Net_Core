using Microsoft.AspNetCore.Authentication; // Add this line
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Splitwise.Api.Data;
using Splitwise.Api.Middleware;
using Splitwise.Api.Services;
using Splitwise.Api.Services.Interfaces;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Config
var connStr = builder.Configuration.GetConnectionString("Default") ??
              "Host=localhost;Database=splitwise;Username=splitwise_user;Password=postgres";
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsARandomSecretStringThatIsLongEnoughForHS256AndBeyond";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "splitwise-app";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "splitwise-clients";

// DbContext
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connStr));

// Services
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ISettlementService, SettlementService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();

// Auth (JWT)
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Splitwise API", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, new List<string>() } });
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p => p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Apply migrations on startup (dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("dev");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
