using VH_Burguer.Contexts;
using VH_Burguer.Domains;
using VH_Burguer.Interfaces;

// a repositories serve para garantir a veracidade dos dados do banco
namespace VH_Burguer.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VH_BurguerContext _context;

        public UsuarioRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        // vai listar todos os usuarios do banco
        public List<Usuario> Listar()
        {
            return _context.Usuario.ToList();
        }

        public Usuario? ObterPorId(int id)
        {
            // .Find funciona bem com chaves primarias
            return _context.Usuario.Find(id);
        }

        public Usuario? ObterPorEmail(string email)
        {
            // FirstOrDefault -> retorna nosso usuario do banco
            return _context.Usuario.FirstOrDefault(usuario => usuario.Email == email);
        }

        public bool EmailExiste(string email)
        {
            // Any -> retorna um true or false para validar se existe ALGUM usuario com esse email
            return _context.Usuario.Any(usuario => usuario.Email == email);
        }

        public void Adicionar(Usuario usuario)
        {
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        // recebe um usuario como parametro para atualizar o usuario, mudar o nome e mudar o email, por exemplo
        // entao vou pegar o usuario do banco, validar se existe, se ele existir, vai atualizar o nome do usuario no banco pelo novo que passamos
        public void Atualizar(Usuario usuario)
        {
            Usuario? usuarioBanco = _context.Usuario.FirstOrDefault(usuarioAux => usuarioAux.UsuarioID == usuario.UsuarioID);

            if (usuarioBanco == null)
            {
                return;
            }

            // altera o nome salvo no banco para o novo nome que passmos
            usuarioBanco.Nome = usuario.Nome;
            usuarioBanco.Email = usuario.Email;
            usuarioBanco.Senha = usuario.Senha;

            // salva no banco
            _context.SaveChanges();
        }

        public void Remover(int id)
        {                                                       // auxiliar que percorre o banco
            Usuario? usuario = _context.Usuario.FirstOrDefault(usuarioAux => usuarioAux.UsuarioID == id);

            if (usuario == null)
            {
                return;
            }

            // remove o usuario e salva
            _context.Usuario.Remove(usuario);
            _context.SaveChanges();
        }

    }
}
