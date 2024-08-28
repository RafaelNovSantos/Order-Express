namespace AplicativoMaui.Data
{
    public class Product
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public double Valor { get; set; }
        public int Quantidade { get; set; }
        public double Total { get; set; }
    }
}
