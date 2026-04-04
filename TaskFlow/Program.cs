using Microsoft.EntityFrameworkCore;
using TaskFlow;
using TaskFlow.Data;
using TaskFlow.Extensions;
using TaskFlow.Extensions.Middlewares;
using Serilog;

Log.Logger = new LoggerConfiguration().MinimumLevel.Warning().WriteTo.File("Logs/TaskFlow-Log.txt", rollingInterval: RollingInterval.Day).CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Services.AddTaskFlowServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthorization();
builder.Services.AddTaskFlowJwt(builder.Configuration);
builder.Services.AddTaskFlowSwagger();
builder.Services.AddTaskFlowRateLimiter();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    var configuration = services.GetRequiredService<IConfiguration>();

    SeedData.Seed(db, configuration);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
