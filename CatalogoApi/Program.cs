using CatalogoApi.Context;
using CatalogoApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ConfigureServices
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

//Inserir Categoria
app.MapPost("/categorias", async (CategoriaModel categoria, ApplicationDbContext context) =>
{
    context.Add(categoria);
    await context.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.IdCategoria}", categoria);
});

//Retornar Categorias
app.MapGet("/categorias", async (ApplicationDbContext context) =>
{
    return await context.Categorias?.ToListAsync();
});

//Retornar categoria pelo ID
app.MapGet("/categorias/{id:guid}", async (Guid id, ApplicationDbContext context) =>
{
    return await context.Categorias.FindAsync(id) is CategoriaModel categoria ? Results.Ok(categoria) : Results.NotFound("Guid inexistente");
});

//Atualizar uma categoria pelo ID
app.MapPut("/categorias/{id:guid}", async (Guid id, CategoriaModel categoria, ApplicationDbContext context) =>
{
    if (categoria.IdCategoria != id) return Results.BadRequest();

    var categoriaDb = await context.Categorias.FindAsync(id);

    if (categoriaDb is null) return Results.NotFound();

    //TODO: Implementar um Automapper
    categoriaDb.Nome = categoria.Nome;
    categoriaDb.Descricao = categoria.Descricao;

    await context.SaveChangesAsync();
    return Results.Ok(categoriaDb);

});

//Deletar uma categoria pelo ID
app.MapDelete("/categorias/{id:guid}", async (Guid id, ApplicationDbContext context) =>
{
    var categoria = await context.Categorias.FindAsync(id);

    if (categoria is null) return Results.NotFound("Categoria nao encontrada");

    context.Categorias.Remove(categoria);

    return Results.NoContent();
});

// Configure the HTTP request pipeline.
// Configure:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();

