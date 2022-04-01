using CatalogoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ProdutoModel>? Produtos { get; set; }
        public DbSet<CategoriaModel>? Categorias { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Propriedades
            modelBuilder.Entity<CategoriaModel>().HasKey(x => x.IdCategoria);
            modelBuilder.Entity<CategoriaModel>().Property(x => x.Nome).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<CategoriaModel>().Property(x => x.Descricao).HasMaxLength(150).IsRequired();

            modelBuilder.Entity<ProdutoModel>().HasKey(x => x.IdProduto);
            modelBuilder.Entity<ProdutoModel>().Property(x => x.Nome).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<ProdutoModel>().Property(x => x.Descricao).HasMaxLength(150);
            modelBuilder.Entity<ProdutoModel>().Property(x => x.ImagemUrl).HasMaxLength(100);
            modelBuilder.Entity<ProdutoModel>().Property(x => x.Preco).HasPrecision(14,2);

            //Relacionamentos
            modelBuilder.Entity<ProdutoModel>().HasOne(x => x.Categoria).WithMany(x => x.Produtos).HasForeignKey(x => x.IdCategoria);
        }
    }
}
