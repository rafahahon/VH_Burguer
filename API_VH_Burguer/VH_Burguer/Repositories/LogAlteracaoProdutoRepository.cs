using VH_Burguer.Contexts;
using VH_Burguer.Domains;
using VH_Burguer.Interfaces;

namespace VH_Burguer.Repositories
{
    public class LogAlteracaoProdutoRepository : ILogAlteracaoProdutoRepository
    {
        private readonly VH_BurguerContext _context; // a context é a conversa direta com o banco, e a Repository é a primeira camada que conversa com banco,
                                                     // que acessa a camada do banco para poder manipular os dados atraves da context

        public LogAlteracaoProdutoRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        public List<Log_AlteracaoProduto> Listar()
        {
            List<Log_AlteracaoProduto> log = _context.Log_AlteracaoProduto.OrderByDescending(l => l.DataAlteracao).ToList(); // OrderByDescending -> Ordena por data em ordem descrescente
            
            return log;
        }

        public List<Log_AlteracaoProduto> ListarPorProduto(int produtoId)
        {
            List<Log_AlteracaoProduto> AlteracoesProduto = _context.Log_AlteracaoProduto
                                                            .Where(log => log.ProdutoID == produtoId)
                                                            .OrderByDescending(log => log.DataAlteracao)
                                                            .ToList();

            return AlteracoesProduto;
        }
    }
}
