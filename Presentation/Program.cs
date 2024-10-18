using Microsoft.EntityFrameworkCore;
using Presentation.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var contentRootPath = builder.Environment.ContentRootPath;
var dbPath = System.IO.Path.Combine(contentRootPath, "db", "TodoItems.db");
builder.Services.AddDbContext<MyContext>(x => x.UseSqlite($"Data Source= {dbPath}"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();