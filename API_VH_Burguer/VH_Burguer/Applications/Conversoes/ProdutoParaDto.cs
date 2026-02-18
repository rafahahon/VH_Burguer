using VH_Burguer.Domains;
using VH_Burguer.DTOs.ProdutoDto;

namespace VH_Burguer.Applications.Conversoes
{
    public class ProdutoParaDto
    {
        public static LerProdutoDto ConverterParaDto(Produto produto)
        {
            return new LerProdutoDto
            {
                ProdutoID = produto.ProdutoID,
                Nome = produto.Nome,
                Preco = produto.Preco,
                Descricao = produto.Descricao,
                StatusProduto = produto.StatusProduto,

                CategoriasIds = produto.Categoria.Select(categoria => categoria.CategoriaID).ToList(), // lista as categorias (pelo id) do produto que chamarmos

                Categorias = produto.Categoria.Select(categoria => categoria.Nome).ToList(),

                UsuarioID = produto.UsuarioID,
                UsuarioNome = produto.Usuario.Nome,
                UsuarioEmail = produto.Usuario.Email
            };
        }
    }
}
