using Microsoft.EntityFrameworkCore;
using TaskManager.Mappings;
using TaskManager_API.Data;
using TaskManager_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000") 
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); 
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAd");

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireRole("User"));
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TaskManager API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter the Bearer Token: "
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseCosmos(
        builder.Configuration["CosmosDb:Endpoint"],
        builder.Configuration["CosmosDb:Key"],
        builder.Configuration["CosmosDb:Database"]
    ));

builder.Services.AddDbContext<UserContext>(options =>
    options.UseCosmos(
        builder.Configuration["CosmosDb:Endpoint"],
        builder.Configuration["CosmosDb:Key"],
        builder.Configuration["CosmosDb:Database"]
    ));

builder.Services.AddScoped<TaskManager_API.Services.TaskUserService>();
builder.Services.AddAutoMapper(typeof(TaskMapperProfile).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();