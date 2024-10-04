using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api_event;
using api_event.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter la configuration des services directement dans le builder
builder.Services.Configure<EventprojDBSettings>(
    builder.Configuration.GetSection("EventprojDB"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

// Ajouter les services utilisateurs et credentials
builder.Services.AddSingleton<UsersService>();
builder.Services.AddSingleton<CredentialsService>();
// Enregistrer UsersService dans le conteneur de services
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<TicketsService>();
builder.Services.AddScoped<EventsService>();

// Configurer l'authentification JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Ajouter les contrôleurs
builder.Services.AddControllers();

// Créer l'application
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Ticket API",
        Description = "An API for managing event tickets",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your-email@example.com",
            Url = new Uri("https://yourwebsite.com"),
        }
    });
});

var app = builder.Build();

// Ajouter les middlewares d'authentification et d'autorisation
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticket API v1");
        c.RoutePrefix = string.Empty; // To make Swagger UI accessible at the root
    });
}

app.UseHttpsRedirection();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Ajouter le mapping des contrôleurs
app.MapControllers();

// Lancer l'application
app.Run();
