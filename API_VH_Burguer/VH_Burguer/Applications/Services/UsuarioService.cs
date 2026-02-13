using System.Security.Cryptography;
using System.Text;
using VH_Burguer.Domains;
using VH_Burguer.DTOs.UsuarioDto;
using VH_Burguer.Exceptions;
using VH_Burguer.Interfaces;

namespace VH_Burguer.Applications.Services
{
    // service concentra o "como fazer"
    public class UsuarioService
    {
        // repository é o canal para acessar os dados
        private readonly IUsuarioRepository _repository;

        // injecao de dependencias
        // implementamos o repositorio e o service so depende da interface
        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository; 
        }

        // pq private? pq o metodo nao e regra de negocio e nao faz sentido existir fora do UsuarioService

        private static LerUsuarioDto LerDto(Usuario usuario) // pega a entidade usuario e gera um DTO
        {
            LerUsuarioDto LerUsuario = new LerUsuarioDto() // instanciar o objeto é duplicar o molde
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                StatusUsuario = usuario.StatusUsuario ?? true // se nao tiver nenhum status no banco, deixa como true
            };

            return LerUsuario;
        }

        public List<LerUsuarioDto> Listar()
        {
            List<Usuario> usuarios = _repository.Listar();

            List<LerUsuarioDto> usuariosDto = usuarios.Select(usuarioBanco => LerDto(usuarioBanco)).ToList(); // SELECT que percorre cada usuario e LerDto(usuario)

            return usuariosDto;
        }

        // regras do sistema
        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                throw new DomainException("Email inválido.");
            }
        }

        private static byte[] HashSenha(string senha)
        {
            if(string.IsNullOrWhiteSpace(senha)) // garante que a senha nao esta vazia
            {
                throw new DomainException("Senha é obrigatória.");
            }

            using var sha256 = SHA256.Create(); // gera um hash SHA256 e devolve em byte[]
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        }

        public LerUsuarioDto ObterPorId(int id)
        {
            Usuario? usuario = _repository.ObterPorId(id);

            if(usuario == null)
            {
                throw new DomainException("Usuário não existe.");
            }

            return LerDto(usuario); // se existe usuario, converte para DTO e devolve para o usuario 
        }

        public LerUsuarioDto ObterPorEmail(string email)
        {
            Usuario? usuario = _repository.ObterPorEmail(email);

            if (usuario == null)
            {
                throw new DomainException("Usuário não existe.");
            }

            return LerDto(usuario); // se existe usuario, converte para DTO e devolve para o usuario 
        }

        public LerUsuarioDto Adicionar(CriarUsuarioDto usuarioDto)
        {
            ValidarEmail(usuarioDto.Email);

            if(_repository.EmailExiste(usuarioDto.Email))
            {
                throw new DomainException("Já existe um usuário com este e-mail.");
            }

            Usuario usuario = new Usuario // criando entidade usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                Senha = HashSenha(usuarioDto.Senha),
                StatusUsuario = true
            };

            _repository.Adicionar(usuario); // cria e salva o usuario no banco

            return LerDto(usuario); // retorna LerDto para nao retornar o objeto com a senha
        }


        // atualiza os dados do usuario caso ele nao tenha o email ou id repetido
        public LerUsuarioDto Atualizar(int id, CriarUsuarioDto usuarioDto)
        {
            ValidarEmail(usuarioDto.Email);

            Usuario usuarioBanco = _repository.ObterPorId(id);

            if(usuarioBanco == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            ValidarEmail(usuarioDto.Email);

            Usuario usuarioComMesmoEmail = _repository.ObterPorEmail(usuarioDto.Email);

            if(usuarioComMesmoEmail != null && usuarioComMesmoEmail.UsuarioID != id)
            {
                throw new DomainException("Já existe um usuário com este email.");
            }

            // substitui as informacoes do banco (usuarioBanco) inserindo as alteracoes que estao vindo de usuarioDto
            usuarioBanco.Nome = usuarioDto.Nome;
            usuarioBanco.Email = usuarioDto.Email;
            usuarioBanco.Senha = HashSenha(usuarioDto.Senha);

            _repository.Atualizar(usuarioBanco);

            return LerDto(usuarioBanco);
        }

        // remove o usuario do banco
        public void Remover(int id)
        {
            Usuario usuario = _repository.ObterPorId(id);

            if(usuario == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            _repository.Remover(id);
        }

    }
}
