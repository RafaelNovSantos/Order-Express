using System.Collections.ObjectModel;
using Gerador_de_Pedidos.Pedidos.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class SalvarPedido
{
    private readonly Database _database;
  
    public SalvarPedido(Database database)
    {
        _database = database;
    }

    public async Task<bool> SalvarPedidoAsync(
        Budget MyBudget,
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
        var Items = (listaProdutosSelect?.ItemsSource as IEnumerable<object>)?
                 .OfType<Product>()
                 .ToList() ?? new List<Product>();
        var dadosService = DependencyService.Get<DadosCompartilhadosService>();
        bool answer;

        var Cliente = dadosService.Cliente;

        if (!Items.Any()) // Maneira mais idiomática de verificar se a lista está vazia
        {
            await Application.Current.MainPage.DisplayAlert("Aviso", "Nenhum item adicionado ao pedido.", "OK");
            return false;
        }

        if (!string.IsNullOrEmpty(Cliente))
        {
            answer = await Application.Current.MainPage.DisplayAlert("Alteração cliente", $"Deseja alterar o nome do cliente: {Cliente} no pedido?", "Sim", "Não");

        }
        else
        {

            answer = await Application.Current.MainPage.DisplayAlert("Cadastro cliente", "Deseja adicionar o nome do cliente no pedido?", "Sim", "Não");

        }

        if (answer == true)
        {
            // Abre o prompt com o valor atual preenchido
            string newValue = await Application.Current.MainPage.DisplayPromptAsync("Editar", $"Digite o nome do cliente:", "OK", "Cancelar");
            if (string.IsNullOrEmpty(newValue))
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Você deixou o campo cliente vazio, o pedido não foi salvo", "OK");
                return false;
            }
            Cliente = newValue;
        }

        decimal? valorfrete = decimal.TryParse(txtFrete, out decimal frete) ? frete : null;
        int novoNumeroPedido = await getProximoNumeroPedidoAsync();


        if (MyBudget.Numero_Pedido != novoNumeroPedido) // Verifica se a data não é nula
        {
            await _database.DeletarProdutoPorNumeroPedidoAsync(MyBudget.Numero_Pedido);
            await _database.DeletarPedidoPorNumeroPedidoAsync(MyBudget.Numero_Pedido);

        }
        else
        {
            MyBudget.Numero_Pedido = novoNumeroPedido;
        }

        if (produtosSelecionados.Any())
        {
            foreach (var product in produtosSelecionados)
            {
                var novoProdutoPedido = new ProdutosPedido
                {
                    NumeroPedido = MyBudget.Numero_Pedido,
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
                NumeroPedido = MyBudget.Numero_Pedido,
                TipoPedido = tipopedido,
                Vendedor = vendedor,
                Cliente = Cliente,
                ValorFrete = !tipopedido.StartsWith("Garantia") ? valorfrete : null,
                TipoFrete = !tipopedido.StartsWith("Garantia") ? tipofrete : "",
                TipoPagamento = !tipopedido.StartsWith("Garantia") ? tipopagamento : "",
                Faturamento = !tipopedido.StartsWith("Garantia") ? faturamento : "",
                DefeitoEquipamento = !tipopedido.StartsWith("Garantia") ? "" : defeitoequipamento,
                NumSerieEquipamento = !tipopedido.StartsWith("Garantia") ? "" : numseriequipamento,
                TipoNota = !tipopedido.StartsWith("Garantia") ? "" : tiponota,
                NumNota = !tipopedido.StartsWith("Garantia") ? "" : numnota,
                ChaveNotaExterna = !tipopedido.StartsWith("Garantia") ? "" : chavenotaexterna,
                ValorTotal = MyBudget.Valor_Total,
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


            return true;
        }
        return false;
    }
}
