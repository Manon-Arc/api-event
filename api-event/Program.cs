using System.Reflection;
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

builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<CredentialsService>();
builder.Services.AddScoped<TicketsService>();
builder.Services.AddScoped<EventsService>();
builder.Services.AddScoped<EventGroupsService>();
builder.Services.AddScoped<LinkEventToGroupService>();
builder.Services.AddScoped<TicketOfficeService>();
builder.Services.AddScoped<PermissionService>();

// Ajouter Swagger pour la documentation de l'API
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
            Url = new Uri("https://yourwebsite.com")
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});


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
        ValidIssuer = builder.Configuration["JwtSetting:Issuer"],
        ValidAudience = builder.Configuration["JwtSetting:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSetting:Key"]))
    };
});

// Ajouter les contrôleurs
builder.Services.AddControllers();

// Créer l'application
var app = builder.Build();

// Ajouter les middlewares d'authentification et d'autorisation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticket API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status404NotFound)
    {
        context.Response.ContentType = "application/json";
        var jsonResponse = new
        {
            Message = "The requested route does not exist"
        };

        await context.Response.WriteAsJsonAsync(jsonResponse);
    }
});

// Ajouter le mapping des contrôleurs
app.MapControllers();

// Lancer l'application
app.Run();