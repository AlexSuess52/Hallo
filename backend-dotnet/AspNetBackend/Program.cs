using System.Text;
using AspNetBackend.Data;
using AspNetBackend.Entities;
using AspNetBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// configure PostgreSQL database connection
var connString = builder.Configuration.GetConnectionString("AspNetBackend");
builder.Services.AddDbContext<AspNetPostgresDbContext>(options =>
    options.UseNpgsql(connString));

// add JWT validation parameters
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
            ValidateIssuerSigningKey = true
        };
    });

// register application services with dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<Player>, PasswordHasher<Player>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ILeaderBoardService, LeaderBoardsService>();


// enable Swagger for API documentation and testing
builder.Services.AddSwaggerGen(options =>
{
    // add Bearer token authentication support to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// add standard Web API services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// configure Kestrel to listen on port 80 (HTTP)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});

// @ Marc-André
// important! this allows ALL origins (AllowAnyOrigin) 
// it is only for test purpose because we may want to show this on a mobile device at the presentation
// see code below how it's done correctly by limiting the AllowedOrigins in the appsettings.json file!
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

/*
// Recommended production CORS setup (commented out for testing)
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

if (allowedOrigins == null || allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("AllowedOrigins not configured!");
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
*/

var app = builder.Build();

// apply pending database migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AspNetPostgresDbContext>();
    db.Database.Migrate();
}

// enable Swagger UI for API exploration
app.UseSwagger();
app.UseSwaggerUI();

// apply CORS policy (must be before authentication)
app.UseCors();

// enable authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// activate controllers
app.MapControllers();

// start the application
app.Run();
