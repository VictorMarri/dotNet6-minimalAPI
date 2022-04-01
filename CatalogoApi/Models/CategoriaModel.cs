using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CatalogoApi.Models
{
    public class CategoriaModel
    {
        public Guid IdCategoria { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }

        [JsonIgnore]
        public ICollection<ProdutoModel>? Produtos { get; set; }
    }
}
