namespace VH_Burguer.Applications.ContentSafety
{
    public interface IContentSafetyRepository
    {
        // Task<> representa uma função assíncrona
        // aprovado -> texto foi aprovado ou não
        // msg -> aviso da recusa do texto
        Task<(bool aprovado, string msg)> ValidarConteudo(string texto);
    }
}
