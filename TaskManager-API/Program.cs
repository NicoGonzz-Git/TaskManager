using Microsoft.EntityFrameworkCore;
using TaskManager_API.Data;
using TaskManager_API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();