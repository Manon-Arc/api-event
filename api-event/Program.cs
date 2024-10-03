using api_event;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<EventprojDBSettings>(
    builder.Configuration.GetSection("EventprojDB"));