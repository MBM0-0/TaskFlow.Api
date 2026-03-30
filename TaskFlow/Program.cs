using Microsoft.EntityFrameworkCore;
using TaskFlow;
using TaskFlow.Data;
using TaskFlow.Extensions;
using TaskFlow.Extensions.Middlewares;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTaskFlowServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthorization();
builder.Services.AddTaskFlowJwt(builder.Configuration);
builder.Services.AddTaskFlowSwagger();

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

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
