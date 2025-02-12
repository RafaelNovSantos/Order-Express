public class DadosCompartilhadosService
{
    public bool BaseChanged { get; set; }
    public string Vendedor { get; set; }
    public int NumeroPedido { get; set; }
    public string TipoPedido { get; set; }
    public decimal? ValorFrete { get; set; }
    public string TipoFrete { get; set; }
    public string TipoPagamento { get; set; }
    public string Faturamento { get; set; }
    public string DefeitoEquipamento { get; set; }
    public string NumSerieEquipamento { get; set; }
    public string TipoNota { get; set; }
    public string NumNota { get; set; }
    public string ChaveNotaExterna { get; set; }
    public DateTime DataPedido { get; set; }
}
