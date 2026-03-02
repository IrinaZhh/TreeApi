using Microsoft.EntityFrameworkCore;
using TreeApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Подключение к локальной базе MS SQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Минимальный пример API
app.MapGet("/trees", async (AppDbContext db) =>
{
    return await db.Trees
        .Include(t => t.Nodes)
        .ThenInclude(n => n.Children)
        .ToListAsync();
});

app.Run();