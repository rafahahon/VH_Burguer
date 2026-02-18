namespace VH_Burguer.Applications.Conversoes
{
    public class ImagemParaBytes
    {
        public static byte[] ConverterImagem(IFormFile imagem)
        {
            using var ms = new MemoryStream();
            imagem.CopyTo(ms); // memoria temporaria salva e transforma o link da imagem em array para salvar no banco
            return ms.ToArray();
        }
    }
}
