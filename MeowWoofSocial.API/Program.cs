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
using MeowWoofSocial.Business.Services.CartServices;
using MeowWoofSocial.Business.Services.CategoryServices;
using MeowWoofSocial.Business.Services.CloudServices;
using MeowWoofSocial.Business.Services.PetStoreProductServices;
using MeowWoofSocial.Business.Services.UserFollowingServices;
using Microsoft.AspNetCore.Authentication;
using MeowWoofSocial.Data.Repositories.NotificationRepositories;
using MeowWoofSocial.Data.Repositories.PostStoredRepositories;
using MeowWoofSocial.Data.Repositories.ReportRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreRepositories;
using MeowWoofSocial.Business.Services.PetStoreServices;
using MeowWoofSocial.Data.Repositories.PetStoreProductAttachmentRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreProductItemRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreProductRepositories;
using MeowWoofSocial.Business.Services.TransactionServices;
using MeowWoofSocial.Business.Services.PostReactionServices;
using MeowWoofSocial.Data.Repositories.OrderRepositories;
using MeowWoofSocial.Data.Repositories.TransactionRepositories;
using MeowWoofSocial.Data.Repositories.UserAddressRepositories;
using MeowWoofSocial.Business.Services.UserAddressServices;
using MeowWoofSocial.Business.Services.UserPetServices;
using MeowWoofSocial.Data.Repositories.UserPetRepositories;
using MeowWoofSocial.Data.Repositories.IOTPRepositories;
using MeowWoofSocial.Business.Services.OTPServices;
using MeowWoofSocial.Business.Services.RatingServices;
using MeowWoofSocial.Business.Ultilities.Email;
using MeowWoofSocial.Data.Repositories.CartRepositories;
using MeowWoofSocial.Data.Repositories.CategoryRepositories;
using MeowWoofSocial.Data.Repositories.PetStoreProductRatingRepositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

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

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<MeowWoofSocialContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//======================================= AUTHENTICATION ==========================================
builder.Services.AddAuthentication("MeowWoofAuthentication")
    .AddScheme<AuthenticationSchemeOptions, AuthorizeMiddleware>("MeowWoofAuthentication", null);

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
builder.Services.AddScoped<INotificationRepositories, NotificationRepositories>();
builder.Services.AddScoped<IPostStoredRepositories, PostStoredRepositories>();
builder.Services.AddScoped<IReportRepositories, ReportRepositories>();
builder.Services.AddScoped<IPetStoreRepositories, PetStoreRepositories>();
builder.Services.AddScoped<IPetStoreProductRepositories, PetStoreProductRepositories>();
builder.Services.AddScoped<IPetStoreProductAttachmentRepositories, PetStoreProductAttachmentRepositories>();
builder.Services.AddScoped<IPetStoreProductItemRepositories, PetStoreProductItemRepositories>();
builder.Services.AddScoped<IOrderRepositories, OrderRepositories>();
builder.Services.AddScoped<IOrderDetailRepositories, OrderDetailRepositories>();
builder.Services.AddScoped<IUserAddressRepositories, UserAddressRepositories>();
builder.Services.AddScoped<ITransactionRepositories, TransactionRepositories>();
builder.Services.AddScoped<IUserPetRepositories, UserPetRepositories>();
builder.Services.AddScoped<IOTPRepositories, OTPRepositories>();
builder.Services.AddScoped<ICartRepositories, CartRepositories>();
builder.Services.AddScoped<IPetStoreProductRatingRepositories, PetStoreProductRatingRepositories>();
builder.Services.AddScoped<ICategoryRepositories, CategoryRepositories>();

//=========================================== SERVICE =============================================
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IPostServices, PostServices>();
builder.Services.AddScoped<IUserFollowingServices, UserFollowingServices>();
builder.Services.AddScoped<IPostReactionServices, PostReactionServices>();
builder.Services.AddScoped<IPetStoreServices, PetStoreServices>();
builder.Services.AddScoped<IPetStoreProductServices, PetStoreProductServices>();
builder.Services.AddScoped<ITransactionServices, TransactionServices>();
builder.Services.AddScoped<IUserAddressServices, UserAddressServices>();
builder.Services.AddScoped<IUserPetServices, UserPetServices>();
builder.Services.AddScoped<IOTPServices, OTPServices>();
builder.Services.AddScoped<ICartServices, CartServices>();
builder.Services.AddScoped<IEmail, Email>();
builder.Services.AddScoped<IRatingServices, RatingServices>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();

//=========================================== CORS ================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOrigin", policy =>
    {
        policy
            .WithOrigins("https://meowwoofsocial.com", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Cho phép cookies, authorization headers, hoặc TLS client certificates
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Đảm bảo URL hub đúng
app.MapHub<TransactionHub>("/hub/transactionhub");

app.Run();
