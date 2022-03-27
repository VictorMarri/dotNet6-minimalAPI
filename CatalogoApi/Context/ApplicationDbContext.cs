using CatalogoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Produto>? Produtos { get; set; }
        public DbSet<Categoria>? Categorias { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Propriedades
            modelBuilder.Entity<Categoria>().HasKey(x => x.IdCategoria);
            modelBuilder.Entity<Categoria>().Property(x => x.Nome).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Categoria>().Property(x => x.Descricao).HasMaxLength(150).IsRequired();

            modelBuilder.Entity<Produto>().HasKey(x => x.IdProduto);
            modelBuilder.Entity<Produto>().Property(x => x.Nome).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Produto>().Property(x => x.Descricao).HasMaxLength(150);
            modelBuilder.Entity<Produto>().Property(x => x.ImagemUrl).HasMaxLength(100);
            modelBuilder.Entity<Produto>().Property(x => x.Preco).HasPrecision(14,2);

            //Relacionamentos
            modelBuilder.Entity<Produto>().HasOne(x => x.Categoria).WithMany(x => x.Produtos).HasForeignKey(x => x.IdCategoria);
        }
    }
}
