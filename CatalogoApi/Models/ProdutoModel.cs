using System.Text.Json.Serialization;

namespace CatalogoApi.Models
{
    public class ProdutoModel
    {
        [JsonIgnore]
        public Guid IdProduto { get; set; }
        public Guid IdCategoria { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public string? ImagemUrl { get; set; }
        public DateTime DataCompra { get; set; }
        public int Estoque { get; set; }

        [JsonIgnore]
        public CategoriaModel? Categoria { get; set; }

    }
}
