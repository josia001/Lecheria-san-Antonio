using System.Text.Json.Serialization;
using Lecheria.Application.Interfaces;
using Lecheria.Application.Service;
using Lecheria.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Ganadería San Antonio",
        Version = "v1",
        Description = "Registro de una res y consulta de su ficha individual."
    });
    options.DocInclusionPredicate((_, api) => api.RelativePath?.StartsWith("api/", StringComparison.OrdinalIgnoreCase) == true);
});
var conexion = builder.Configuration.GetConnectionString("Lecheria")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:Lecheria para conectar con SQL Server.");
builder.Services.AddDbContext<LecheriaDb>(options => options.UseSqlServer(conexion));
builder.Services.AddScoped<IResRepository, ResRepositorio>();
builder.Services.AddScoped<ResService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LecheriaDb>();
    await db.Database.EnsureCreatedAsync();
}

app.UseExceptionHandler(error => error.Run(context =>
    Results.Problem(statusCode: 500, title: "No se pudo completar la operación.").ExecuteAsync(context)));
app.UseStatusCodePages(status => Results.Problem(statusCode: status.HttpContext.Response.StatusCode)
    .ExecuteAsync(status.HttpContext));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Ganadería San Antonio v1"));
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();
app.Run();
