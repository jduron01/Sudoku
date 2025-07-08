using Microsoft.EntityFrameworkCore;
using Sudoku.Models;

var allowOrigins = "_allowOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<BoardContext>(opt =>
    opt.UseInMemoryDatabase("BoardList"));
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7169",
                                             "http://localhost:5173")
                                             .AllowAnyHeader()
                                             .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();
app.UseCors(allowOrigins);
app.UseAuthorization();
app.MapControllers();
app.Run();
