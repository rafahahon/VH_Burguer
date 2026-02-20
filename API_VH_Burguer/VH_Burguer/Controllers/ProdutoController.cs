using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VH_Burguer.Applications.Services;
using VH_Burguer.DTOs.ProdutoDto;
using VH_Burguer.Exceptions;

namespace VH_Burguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutoController(ProdutoService service)
        {
            _service = service;
        }

        // autenticacao do usuario

        private int ObterUsuarioIdLogado()
        {
            // busca no token/claims o valor armazenado como id do usuario
            // ClaimTypes.NameIdentifier geralmente guarda o ID do usuario no JWT
            string? idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Console.WriteLine(idTexto);
            //Console.WriteLine("banana");

            if (string.IsNullOrWhiteSpace(idTexto))
            {
                throw new DomainException("Usuário não autenticado.");
            }

            // converte o id que veio como texto para inteiro
            // nosso usuarioId no sistema está como int
            // as Claims (informacoes do usuario dentro do token) sempre sao armazenadas como texto
            return int.Parse(idTexto);
        }

        [HttpGet]
        public ActionResult<List<LerProdutoDto>> Listar()
        {
            List<LerProdutoDto> produtos = _service.Listar();

            // return StatusCode(200, produtos);
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public ActionResult<LerProdutoDto> ObterPorId(int id)
        {
            LerProdutoDto produto = _service.ObterPorId(id);

            if(produto == null)
            {
                // return StatusCode(404); 
                return NotFound();
            }

            return Ok(produto);
        }

        // GET -> api/produto/5/imagem
        [HttpGet("{id}/imagem")]
        public ActionResult ObterImagem(int id)
        {
            try
            {
                var imagem = _service.ObterImagem(id);

                // retorna o arquivo para o navegador
                // "image/jpeg" informa o tipo da imagem (MIME type)
                // navegador entende que deve renderizar como imagem
                return File(imagem, "image/jpeg");
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message); // NotFound -> Não encontrado
            }
        }

        [HttpPost] // adicionar o produto
        // indica que recebe dados no formato multipart/form-data
        // necessario quando enviamos arquivos (exemplo: imagem do produto)
        [Consumes("multipart/form-data")]
        [Authorize] // exige o login

        // [FromForm] -> diz que os dados vem do formulario da requisicao (multipart/form-data)
        public ActionResult Adicionar([FromForm] CriarProdutoDto produtoDto)
        {
            try
            {
                int usuarioId = ObterUsuarioIdLogado();

                // cadastro fica associado ao usuario logado
                _service.Adicionar(produtoDto, usuarioId);

                return StatusCode(201); // Created
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize] // exige o login
        public ActionResult Atualizar(int id, [FromForm] AtualizarProdutoDto produtoDto)
        {
            try
            {
                _service.Atualizar(id, produtoDto);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpDelete("{id}")]
        [Authorize] // exige o login
        public ActionResult Remover(int id)
        {
            try
            {
                _service.Remover(id);
                return NoContent();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message); 
            }
        }
    }
}
