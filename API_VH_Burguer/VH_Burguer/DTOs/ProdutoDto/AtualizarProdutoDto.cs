namespace VH_Burguer.DTOs.ProdutoDto
{
    public class AtualizarProdutoDto
    {
        public string Nome { get; set; } = null!;

        public decimal Preco { get; set; }

        public string Descricao { get; set; } = null!;

        public IFormFile Imagem { get; set; } = null!; // a imagem vem via multipart/form-data, ideal para upload de arquivo

        public List<int> CategoriaIds { get; set; } = new();

        public bool? statusProduto { get; set; }
    }
}
