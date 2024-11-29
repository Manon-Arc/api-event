using System.Reflection;
using System.Security.Cryptography;
using api_event;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var publicKey = builder.Configuration["Jwt:PublicKey"];
var rsa = new RSACryptoServiceProvider(4096);
rsa.ImportFromPem(publicKey.ToCharArray());

builder.Logging.AddConsole();
//Jwt configuration starts here
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ClockSkew = TimeSpan.Zero
        };
    });
//Jwt configuration ends here


// Ajouter la configuration des services directement dans le builder
builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("EventprojDB"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));


var assembly = Assembly.GetExecutingAssembly();
var services = assembly.GetTypes().Where(type => type.IsClass && type.Namespace == "api_event.Services").ToList();
foreach (var service in services) builder.Services.AddScoped(service);

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

    // Ajout du support pour JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Veuillez insérer le JWT dans le champ suivant : Bearer {votre_jwt}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// Ajouter les contrôleurs
builder.Services.AddControllers();

var app = builder.Build();

// Ajouter les middlewares d'authentification et d'autorisation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticket API v1"); });
}

app.UseHttpsRedirection();
app.UseRouting();
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

app.MapControllers();
app.Run();