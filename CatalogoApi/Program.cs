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

#region Categoria
//Inserir Categoria
app.MapPost("/categorias", async (CategoriaModel categoria, ApplicationDbContext context) =>
{
    context.Add(categoria);
    await context.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.IdCategoria}", categoria);
}).WithTags("Categorias");

//Retornar Categorias
app.MapGet("/categorias", async (ApplicationDbContext context) =>
{
    return await context.Categorias?.ToListAsync();
}).WithTags("Categorias");

//Retornar categoria pelo ID
app.MapGet("/categorias/{id:guid}", async (Guid id, ApplicationDbContext context) =>
{
    return await context.Categorias.FindAsync(id) is CategoriaModel categoria ? Results.Ok(categoria) : Results.NotFound("Guid inexistente");
}).WithTags("Categorias");

//Obter categoria dos produtos
app.MapGet("categoriaprodutos", async(ApplicationDbContext context) => 
await context.Categorias.Include(x => x.Produtos).ToListAsync()

).Produces<List<CategoriaModel>>(StatusCodes.Status200OK)
.WithTags("Categorias");

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

}).WithTags("Categorias");

//Deletar uma categoria pelo ID
app.MapDelete("/categorias/{id:guid}", async (Guid id, ApplicationDbContext context) =>
{
    var categoria = await context.Categorias.FindAsync(id);

    if (categoria is null) return Results.NotFound("Categoria nao encontrada");

    context.Categorias.Remove(categoria);

    return Results.NoContent();
}).WithTags("Categorias");
#endregion

#region Produtos
//Get Produtos
app.MapGet("/produtos", async (ApplicationDbContext context) => await context.Produtos.ToListAsync())
    .Produces<List<ProdutoModel>>(StatusCodes.Status200OK)
    .WithTags("Produtos");

//Get Produtos pelo Id
app.MapGet("/produtos/{id:guid}", async (Guid id, ApplicationDbContext context) =>
{
    return await context.Produtos.FindAsync(id)
                 is ProdutoModel produto
                 ? Results.Ok(produto)
                 : Results.NotFound("Produto procurado não encontrado no banco");

}).Produces<ProdutoModel>(StatusCodes.Status200OK) //Documentando statusCode que pode aparecer
  .Produces(StatusCodes.Status404NotFound) //Documentando o status code que pode aparecer
  .WithTags("Produtos");

//Localizar um produto atraves de um criterio
app.MapGet("/produtos/nome/{criterio}", (string criterio, ApplicationDbContext context) =>
{
    var produtosSelecionadosComBaseNoCriterioPassado = context.Produtos
                                                       .Where(x => x.Nome.ToLower().Contains(criterio.ToLower()))
                                                       .ToList();

    return produtosSelecionadosComBaseNoCriterioPassado.Count > 0
    ? Results.Ok(produtosSelecionadosComBaseNoCriterioPassado)
    : Results.NotFound(Array.Empty<ProdutoModel>());
}).Produces<List<ProdutoModel>>(StatusCodes.Status200OK)
  .WithName("ProdutosPorNomeCriterio")
  .WithTags("Produtos");

//Obter Produtos por paginação
app.MapGet("/produtosporpagina", async (int numeroPagina, int tamanhoPagina, ApplicationDbContext context) =>
{
    await context.Produtos
    .Skip((numeroPagina - 1) * tamanhoPagina) //Ignora registros definidos na exprssao
    .Take(tamanhoPagina) //Ignora paginas que eu tenho que saltar
    .ToListAsync();

}).Produces<List<ProdutoModel>>(StatusCodes.Status200OK)
  .WithName("ProdutosPorPagina")
  .WithTags("Produtos");

//Post produtos
app.MapPost("/produtos", async (ProdutoModel produto, ApplicationDbContext context) =>
{
    context.Add(produto);

    await context.SaveChangesAsync();

    return Results.Created($"/produtos/{produto.IdProduto}", produto);
}).Produces<ProdutoModel>(StatusCodes.Status201Created)
  .WithName("CriarNovoProduto") //Identificando o endpoint de forma unica
  .WithTags("Produtos"); //Separar a categoria de endpoints em tags

//Put Atualiza nome do Produto
app.MapPut("/produtos", async (Guid IdProduto, string nomeProduto, ApplicationDbContext context) =>
{
    var produtoDb = context.Produtos.SingleOrDefault(x => x.IdProduto == IdProduto);

    if (produtoDb == null) return Results.NotFound();

    produtoDb.Nome = nomeProduto;

    await context.SaveChangesAsync();

    return Results.Ok(produtoDb);
}).Produces<ProdutoModel>(StatusCodes.Status200OK)
  .Produces(StatusCodes.Status404NotFound)
  .WithName("AtualizarNomeProduto")
  .WithTags("Produtos");

//Atualiza todos os dados possiveis do produto
//app.MapPut("/produtos", async (Guid IdProduto, ProdutoModel produto, ApplicationDbContext context) =>
//{
//    if (produto.IdProduto != IdProduto) return Results.BadRequest();

//    var produtoDb = await context.Produtos.FindAsync(IdProduto);

//    if (produtoDb is null) return Results.NotFound();

//    //TODO: AutoMapper
//    produtoDb.Nome = produto.Nome;
//    produtoDb.Descricao = produto.Descricao;
//    produtoDb.Preco = produto.Preco;
//    produtoDb.DataCompra = produto.DataCompra;
//    produtoDb.Estoque = produto.Estoque;
//    produtoDb.ImagemUrl = produto.ImagemUrl;
//    produtoDb.IdCategoria = produto.IdCategoria;

//    await context.SaveChangesAsync();
//    return Results.Ok(produtoDb);


//}).Produces(StatusCodes.Status200OK)
//  .Produces(StatusCodes.Status404NotFound)
//  .WithName("AtualizandoProduto")
//  .WithTags("Produtos");

//DELETE Produtos
app.MapDelete("/produtos/{id:guid}", async (Guid IdProduto, ApplicationDbContext context) =>
{
    var produtoDb = await context.Produtos.FindAsync(IdProduto);

    if (produtoDb is null) return Results.NotFound();

    context.Produtos.Remove(produtoDb);
    context.SaveChangesAsync();
    return Results.Ok("Produto excluido com sucesso");
}).Produces<ProdutoModel>(StatusCodes.Status200OK)
  .Produces(StatusCodes.Status404NotFound)
  .WithName("DeletaProduto")
  .WithTags("Produtos");
#endregion

// Configure the HTTP request pipeline.
// Configure:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();

