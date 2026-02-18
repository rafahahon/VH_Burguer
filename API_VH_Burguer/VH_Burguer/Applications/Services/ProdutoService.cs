using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using VH_Burguer.Applications.Conversoes;
using VH_Burguer.Applications.Regras;
using VH_Burguer.Domains;
using VH_Burguer.DTOs.ProdutoDto;
using VH_Burguer.Exceptions;
using VH_Burguer.Interfaces;

namespace VH_Burguer.Applications.Services
{
    public class ProdutoService
    {
        private readonly IProdutoRepository _repository;

        public ProdutoService(IProdutoRepository repository)
        {
            _repository = repository;
        }

        // Para cada produto que veio do banco, cria um DTO so com o que a requisicao/front precisa
        public List<LerProdutoDto> Listar()
        {
            List<Produto> produtos = _repository.Listar();

            // SELECT percorre cada Produto e transforma em DTO -> LerProdutoDto
            List<LerProdutoDto> produtoDto = produtos.Select(ProdutoParaDto.ConverterParaDto).ToList();

            return produtoDto;
        }

        public LerProdutoDto ObterPorId(int id)
        {
            Produto produto = _repository.ObterPorId(id);

            if (produto == null)
            {
                throw new DomainException("Produto não encontrado.");
            }

            // converte o produto encontrado para DTO e devolve
            return ProdutoParaDto.ConverterParaDto(produto);
        }

        private static void ValidarCadastro(CriarProdutoDto produtoDto)
        {
            if(string.IsNullOrWhiteSpace(produtoDto.Nome))
            {
                throw new DomainException("Nome é obrigatório.");
            }

            if(produtoDto.Preco < 0)
            {
                throw new DomainException("Preço deve ser maior que zero.");
            }

            if(string.IsNullOrWhiteSpace(produtoDto.Descricao))
            {
                throw new DomainException("Descrição é obrigatória.");
            }

            if(produtoDto.Imagem == null || produtoDto.Imagem.Length == 0)
            {
                throw new DomainException("Imagem é obrigatória.");
            }

            if(produtoDto.CategoriaIds == null || produtoDto.CategoriaIds.Count() == 0)
            {
                throw new DomainException("Produto deve ter amo menos uma categoria.");
            }
        }

        public byte[] ObterImagem(int id)
        {
            var imagem = _repository.ObterImagem(id);

            if(imagem == null || imagem.Length == 0)
            {
                throw new DomainException("Imagem não encontrada.");
            }

            return imagem;
        }

        public LerProdutoDto Adicionar(CriarProdutoDto produtoDto, int usuarioId)
        {
            ValidarCadastro(produtoDto);

            if(_repository.NomeExiste(produtoDto.Nome)) // confere se o nome do produto ja existe
            {
                throw new DomainException("Produto já existente.");
            }

            Produto produto = new Produto()
            {
                Nome = produtoDto.Nome,
                Preco = produtoDto.Preco,
                Descricao = produtoDto.Descricao,
                Imagem = ImagemParaBytes.ConverterImagem(produtoDto.Imagem),
                StatusProduto = true,
                UsuarioID = usuarioId // usuario que adicionou o produto
            };

            _repository.Adicionar(produto, produtoDto.CategoriaIds);

            return ProdutoParaDto.ConverterParaDto(produto);
        }

        public LerProdutoDto Atualizar(int id, AtualizarProdutoDto produtoDto)
        {
            HorarioAlteracaoProduto.ValidarHorario(); // se o horario for o de funcionamento, o codigo nao continua

            Produto produtoBanco = _repository.ObterPorId(id);

            if(produtoBanco == null)
            {
                throw new DomainException("Produto não encontrado.");
            }

            // produtoIdAtual: -> dois pontos serve para passar o valor do parametro
            if(_repository.NomeExiste(produtoDto.Nome, idProdutoIdAtual: id))
            {
                throw new DomainException("Já existe um produto com esse nome.");
            }

            if (produtoDto.CategoriaIds == null || produtoDto.CategoriaIds.Count() == 0)
            {
                throw new DomainException("Produto deve ter amo menos uma categoria.");
            }

            if(produtoDto.Preco < 0)
            {
                throw new DomainException("Preço deve ser maior que zero.");
            }

            produtoBanco.Nome = produtoDto.Nome;
            produtoBanco.Preco = produtoDto.Preco;
            produtoBanco.Descricao = produtoDto.Descricao;

            if(produtoDto.Imagem != null && produtoDto.Imagem.Length > 0)
            { 
                produtoBanco.Imagem = ImagemParaBytes.ConverterImagem(produtoDto.Imagem);
            }

            if(produtoDto.statusProduto.HasValue)
            {
                produtoBanco.StatusProduto = produtoDto.statusProduto.Value;
            }

            _repository.Atualizar(produtoBanco, produtoDto.CategoriaIds);

            return ProdutoParaDto.ConverterParaDto(produtoBanco);
        }

        public void Remover(int id)
        {
            HorarioAlteracaoProduto.ValidarHorario();

            Produto produto = _repository.ObterPorId(id);

            if (produto == null)
            {
                throw new DomainException("Produto não encontrado.");
            }

            _repository.Remover(id);
        }
    }
}
