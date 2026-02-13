using Microsoft.EntityFrameworkCore;
using VH_Burguer.Contexts;
using VH_Burguer.Domains;
using VH_Burguer.Interfaces;

namespace VH_Burguer.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly VH_BurguerContext _context;

        public ProdutoRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        public List<Produto> Listar()
        {
            List<Produto> produtos = _context.Produto.Include(produto => produto.Categoria) // busca produtos e para cada produto, traz as suas categorias
                .Include(produto => produto.Usuario) // busca produtos e para cada produto, traz os seus usuarios
                .ToList();

            return produtos;
        }

        public Produto ObterPorId(int id)
        {
            Produto? produto = _context.Produto
                .Include(produtoDb => produtoDb.Categoria)
                .Include(produtoDb => produtoDb.Usuario)

                // procura no banco (aux produtoDb) e verifica se o id do produto no banco é igual ao id passado no método ObterPorId
                .FirstOrDefault(produtoDb => produtoDb.ProdutoID == id);

            return produto;
        }

        public bool NomeExiste(string nome, int? produtoIdAtual = null)
        {
            // AsQueryable() -> monta a consulta para executar passo a passo, monta a consulta na tabela produto, nao executa nada no banco ainda
            var produtoConsultado = _context.Produto.AsQueryable();

            // se o produto atual tiver valor, entao atualizamos o produto
            if(produtoIdAtual.HasValue)
            {
                produtoConsultado = produtoConsultado.Where(produto => produto.ProdutoID != produtoIdAtual.Value);
            }

            return produtoConsultado.Any(produto => produto.Nome == nome);
        }

        public byte[] ObterImagem(int id)
        {
            var produto = _context.Produto
                .Where(produto => produto.ProdutoID == id)
                .Select(produto => produto.Imagem)
                .FirstOrDefault();

            return produto;
        }

        public void Adicionar(Produto produto, List<int> categoriaIds)
        {
            List<Categoria> categorias = _context.Categoria
                .Where(categoria => categoriaIds.Contains(categoria.CategoriaID))
                .ToList(); // Contains -> retorna true se houver o registro

            produto.Categoria = categorias; // adiciona as categorias incluidas ao produto

            _context.Produto.Add(produto);
            _context.SaveChanges();
        }

        public void Atualizar(Produto produto, List<int> categoriaIds)
        {
            Produto? produtoBanco = _context.Produto
                    .Include(produto => produto.Categoria)
                    .FirstOrDefault(produtoAux => produtoAux.ProdutoID == produto.ProdutoID);

            if(produtoBanco == null)
            {
                return;
            }

            produtoBanco.Nome = produto.Nome;
            produtoBanco.Preco = produto.Preco;
            produtoBanco.Descricao = produto.Descricao;

            if(produto.Imagem != null && produto.Imagem.Length > 0)
            {
                produtoBanco.Imagem = produto.Imagem;
            }

            if(produto.StatusProduto.HasValue)
            {
                produtoBanco.StatusProduto = produto.StatusProduto;
            }

            var categorias = _context.Categoria
                // busca todas as categorias no banco com o id igual das categorias que vieram da requisicao/front
                .Where(categoria => categoriaIds.Contains(categoria.CategoriaID)) 
                .ToList();

            // Clear -> remove as ligacoes atuais entre o produto e as categorias
            // ele nao apaga a categoria do banco, só remove o vinculo com a tabela ProutoCategoria
            produtoBanco.Categoria.Clear();

            foreach(var categoria in categorias)
            {
                produtoBanco.Categoria.Add(categoria);  
            }

            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            Produto? produto = _context.Produto.FirstOrDefault(produto => produto.ProdutoID == id);

            if (produto == null)
            {
                return;
            }

            _context.Produto.Remove(produto);
            _context.SaveChanges();
        }
    }
}
