using SQLite;

public class ProdutosPedido
{
    public int NumeroPedido { get; set; }
    public string Codigo { get; set; }
    public string Descricao { get; set; }
    public string Valor { get; set; }
    public string Quantidade { get; set; }
    public string VersaoPeca { get; set; }
    public DateTime DataPedido { get; set; }
    // Propriedade que contém as posições no grid
  
}
