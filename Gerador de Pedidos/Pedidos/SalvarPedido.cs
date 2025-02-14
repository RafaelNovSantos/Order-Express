using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class SalvarPedido
{
    private readonly Database _database;
  
    public SalvarPedido(Database database)
    {
        _database = database;
    }

    public async Task SalvarPedidoAsync(
        string valortotal,
        int numeropedido,
        string vendedor,
        string cliente,
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
        var dadosService = DependencyService.Get<DadosCompartilhadosService>();

        if (numeropedido != novoNumeroPedido) // Verifica se a data não é nula
        {
            await _database.DeletarProdutoPorNumeroPedidoAsync(numeropedido);
            await _database.DeletarPedidoPorNumeroPedidoAsync(numeropedido);

        }
        else
        {
            numeropedido = novoNumeroPedido;
        }

        if (produtosSelecionados.Any())
        {
            foreach (var product in produtosSelecionados)
            {
                var novoProdutoPedido = new ProdutosPedido
                {
                    NumeroPedido = numeropedido,
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
                NumeroPedido = numeropedido,
                TipoPedido = tipopedido,
                Vendedor = vendedor,
                Cliente = cliente,
                ValorFrete = !tipopedido.StartsWith("Garantia") ? valorfrete : null,
                TipoFrete = !tipopedido.StartsWith("Garantia") ? tipofrete : "",
                TipoPagamento = !tipopedido.StartsWith("Garantia") ? tipopagamento : "",
                Faturamento = !tipopedido.StartsWith("Garantia") ? faturamento : "",
                DefeitoEquipamento = !tipopedido.StartsWith("Garantia") ? "" : defeitoequipamento,
                NumSerieEquipamento = !tipopedido.StartsWith("Garantia") ? "" : numseriequipamento,
                TipoNota = !tipopedido.StartsWith("Garantia") ? "" : tiponota,
                NumNota = !tipopedido.StartsWith("Garantia") ? "" : numnota,
                ChaveNotaExterna = !tipopedido.StartsWith("Garantia") ? "" : chavenotaexterna,
                ValorTotal = valortotal,
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
            dadosService.BaseChanged = true;

            

        }
    }
}
