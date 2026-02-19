using VH_Burguer.Applications.Autenticacao;
using VH_Burguer.Domains;
using VH_Burguer.DTOs.AutenticacaoDto;
using VH_Burguer.Exceptions;
using VH_Burguer.Interfaces;

namespace VH_Burguer.Applications.Services
{
    public class AutenticacaoService
    {
        private readonly IUsuarioRepository _repository;
        private readonly GeradorTokenJwt _TokenJwt;

        public AutenticacaoService(IUsuarioRepository repository, GeradorTokenJwt tokenJwt)
        {
            _repository = repository;
            _TokenJwt = tokenJwt;
        }

        // compara a hash SHA256
        private static bool VerificarSenha(string senhaDigitada, byte[] senhaHashBanco)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hashDigitado = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senhaDigitada));

            return hashDigitado.SequenceEqual(senhaHashBanco);
        }

        public TokenDto Login(LoginDto loginDto)
        {
            Usuario usuario = _repository.ObterPorEmail(loginDto.Email);

            // comparar o email digitado com o email armazenado
            if (usuario == null)
            {
                throw new DomainException("E-mail ou senha inválidos.");
            }

            // comparar a senha digitada com a senha armazenada
            if(!VerificarSenha(loginDto.Senha, usuario.Senha))
            {
                throw new DomainException("E-mail ou senha inválidos");
            }

            // gerando o token
            var token = _TokenJwt.GerarToken(usuario);

            TokenDto novoToken = new TokenDto {  Token = token };

            return novoToken;
        }
    }
}
