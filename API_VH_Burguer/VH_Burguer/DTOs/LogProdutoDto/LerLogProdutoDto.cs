namespace VH_Burguer.DTOs.LogProdutoDto
{
    public class LerLogProdutoDto
    {
        public int logID { get; set; }
        public int? produtoID { get; set; }
        public string NomeAnterior { get; set; } = null!;
        public decimal? PrecoAnterior { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
