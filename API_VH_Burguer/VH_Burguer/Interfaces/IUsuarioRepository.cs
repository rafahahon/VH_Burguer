using VH_Burguer.Domains;

namespace VH_Burguer.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> Listar();

        // pode ser que nao venha nenhum usuario na busca, entao colocamos "?"
        Usuario? ObterPorId(int id);

        Usuario? ObterPorEmail(string email);

        bool EmailExiste(string email);

        void Adicionar(Usuario usuario);

        void Atualizar(Usuario usuario);

        void Remover(int id);
    }
}
