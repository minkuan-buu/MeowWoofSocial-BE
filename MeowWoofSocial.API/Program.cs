using MeowWoofSocial.API.Middleware;
using MeowWoofSocial.Business.MapperProfiles;
using MeowWoofSocial.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//========================================== SWAGGER ==============================================

builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MedicineStore.API",
        Description = "Medicine Store"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. " +
                            "\n\nEnter your token in the text input below. " +
                              "\n\nExample: '12345abcde'",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
{
    new OpenApiSecurityScheme{
        Reference = new OpenApiReference{
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    },
    new string[]{}
}
    });
});

builder.Services.AddDbContext<MeowWoofSocialContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//========================================== MAPPER ===============================================

builder.Services.AddAutoMapper(typeof(MapperProfileConfiguration).Assembly);

//========================================== MIDDLEWARE ===========================================

builder.Services.AddSingleton<GlobalExceptionMiddleware>();
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

//========================================== REPOSITORY ===========================================

//=========================================== SERVICE =============================================

//=========================================== CORS ================================================
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
