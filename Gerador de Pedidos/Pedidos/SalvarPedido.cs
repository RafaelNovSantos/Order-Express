using System.Collections.ObjectModel;

public class SalvarPedido
{
    private readonly Database _database;

    public SalvarPedido(Database database)
    {
        _database = database;
    }

    public async Task SalvarPedidoAsync(
        string vendedor,
        string tipopedido,
        string txtFrete,
        string tipofrete,
        string tipopagamento,
        string faturamento,
        string defeitoequipamento,
        string numseriequipamento,
        string tiponota,
        string numnota,
        string chavenotaexterna,
        List<Product> produtosSelecionados,
        ObservableCollection<Product> listaSelecionados,
        CollectionView listaProdutosSelect,
        Func<Task<int>> getProximoNumeroPedidoAsync
    )
    {
        decimal? valorfrete = decimal.TryParse(txtFrete, out decimal frete) ? frete : null;
        int novoNumeroPedido = await getProximoNumeroPedidoAsync();


        if (produtosSelecionados.Any())
        {
            foreach (var product in produtosSelecionados)
            {
                var novoProdutoPedido = new ProdutosPedido
                {
                    NumeroPedido = novoNumeroPedido,
                    Codigo = product.Codigo,
                    Descricao = product.Descricao,
                    Valor = product.Valor,
                    Quantidade = product.Quantidade,
                    VersaoPeca = product.Versao_Peca,
                    DataPedido = DateTime.Now
                };
                await _database.SalvarProdutosAsync(novoProdutoPedido);
            }

            var novoInfoPedido = new InfoPedido
            {
                NumeroPedido = novoNumeroPedido,
                TipoPedido = tipopedido,
                Vendedor = vendedor,
                ValorFrete = !tipopedido.StartsWith("Garantia") ? valorfrete : null,
                TipoFrete = !tipopedido.StartsWith("Garantia") ? tipofrete : "",
                TipoPagamento = !tipopedido.StartsWith("Garantia") ? tipopagamento : "",
                Faturamento = !tipopedido.StartsWith("Garantia") ? faturamento : "",
                DefeitoEquipamento = !tipopedido.StartsWith("Garantia") ? "" : defeitoequipamento,
                NumSerieEquipamento = !tipopedido.StartsWith("Garantia") ? "" : numseriequipamento,
                TipoNota = !tipopedido.StartsWith("Garantia") ? "" : tiponota,
                NumNota = !tipopedido.StartsWith("Garantia") ? "" : numnota,
                ChaveNotaExterna = !tipopedido.StartsWith("Garantia") ? "" : chavenotaexterna,
                DataPedido = DateTime.Now
            };

            await _database.SalvarInfoPedidoAsync(novoInfoPedido);

            foreach (var item in produtosSelecionados)
            {
                listaSelecionados.Remove(item);
            }

            listaProdutosSelect.SelectedItems.Clear();
            listaProdutosSelect.ItemsSource = null;
            listaProdutosSelect.ItemsSource = listaSelecionados;
            await getProximoNumeroPedidoAsync();
        }
    }
}
