using MeowWoofSocial.API.Middleware;
using MeowWoofSocial.Business.MapperProfiles;
using MeowWoofSocial.Business.Services.UserServices;
using MeowWoofSocial.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MeowWoofSocial.Data.Repositories.PostRepositories;
using MeowWoofSocial.Data.Repositories.PostAttachmentRepositories;
using MeowWoofSocial.Data.Repositories.HashtagRepositories;
using MeowWoofSocial.Business.Services.PostServices;
using MeowWoofSocial.Data.Repositories.PostReactionRepositories;
using MeowWoofSocial.Data.Repositories.UserFollowingRepositories;
using Google.Cloud.Storage.V1;
using MeowWoofSocial.Business.Services.CloudServices;
using MeowWoofSocial.Business.Services.UserFollowingServices;

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
        Title = "MeowWoofSocial.API",
        Description = "Meow Woof Social"
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

//=========================================== FIREBASE ============================================
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"meowwoofsocial.json");
builder.Services.AddSingleton<ICloudStorage>(s => new CloudStorage(StorageClient.Create()));
//========================================== REPOSITORY ===========================================
builder.Services.AddScoped<IUserRepositories, UserRepositories>();
builder.Services.AddScoped<IPostRepositories, PostRepositories>();
builder.Services.AddScoped<IHashtagRepositories, HastagRepositories>();
builder.Services.AddScoped<IPostAttachmentRepositories, PostAttachmentRepositories>();
builder.Services.AddScoped<IPostReactionRepositories, PostReactionRepositories>();
builder.Services.AddScoped<IUserFollowingRepositories, UserFollowingRepositories>();
//=========================================== SERVICE =============================================
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IPostServices, PostServices>();
builder.Services.AddScoped<IUserFollowingServices, UserFollowingServices>();
//=========================================== CORS ================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll", policy =>
    {
        policy
        //.WithOrigins("http://tradiem.pteducation.edu.vn")
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
        //.AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
