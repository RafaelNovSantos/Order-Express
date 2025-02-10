using System.Collections.ObjectModel;
using SQLite;
#if WINDOWS
using Microsoft.UI.Input;
using Windows.Devices.Input;
using Windows.UI.Input.Preview.Injection;
#endif
namespace Gerador_de_Pedidos.Historico;

public partial class HistoricoPage : ContentPage
{
    public ObservableCollection<ProdutosPedido> Produtos { get; set; } = new ObservableCollection<ProdutosPedido>();
    public ObservableCollection<InfoPedido> InfoPedido { get; set; } = new ObservableCollection<InfoPedido>();

    public HistoricoPage()
	{
		InitializeComponent();
        AddPedido(); 
        // Exemplo de lógica para ajustar as colunas e linhas dependendo do dispositivo ou tamanho da tela
    }

#if WINDOWS


private void ClickedMenu(object sender, EventArgs e)
{
    var injector = InputInjector.TryCreate();
    if (injector != null)
    {
        var info = new InjectedInputMouseInfo
        {
            MouseOptions = InjectedInputMouseOptions.RightDown
        };
        injector.InjectMouseInput(new[] { info });

        info = new InjectedInputMouseInfo
        {
            MouseOptions = InjectedInputMouseOptions.RightUp
        };
        injector.InjectMouseInput(new[] { info });
    }
}
#endif

    private void OnEditMenuClicked(object sender, EventArgs e)
    {
        if (Shell.Current.CurrentPage is MainPage mainPage)
        {
            mainPage.VendedorTexto = "Novo Vendedor";
        }


    }

    private void OnDeleteMenuClicked(object sender, EventArgs e)
    {

    }
        private async void AddPedido()
    {
        // Cria a conexão usando o banco de dados assíncrono
        var connection = App.Database.GetConnection();
        // Consulta para pegar o último NumeroPedido
        var ultimoPedido = await connection.Table<InfoPedido>().ToListAsync();
        if (ultimoPedido.Count == 0)
        {
            InfoPedido.Add(new InfoPedido
            {
                NumeroPedido = 1,
                TipoPedido = "product.Codigo",
                Vendedor = "product.Descricao",
                ValorFrete = 10,
                TipoPagamento = "product.Valor",
                Faturamento = "product.VersaoPeca",
                DefeitoEquipamento = "product.VersaoPeca",
                NumSerieEquipamento = "product.VersaoPeca",
                TipoNota = "product.VersaoPeca",
                NumNota = "product.VersaoPeca",
                ChaveNotaExterna = "product.VersaoPeca",
                DataPedido = DateTime.Now,
            });
        }
        else
        {
            foreach (var product in ultimoPedido)
            {
                InfoPedido.Add(new InfoPedido
                {
                    NumeroPedido = product.NumeroPedido,
                    TipoPedido = product.TipoPedido,
                    Vendedor = product.Vendedor,
                    ValorFrete = product.ValorFrete,
                    TipoPagamento = product.TipoPagamento,
                    Faturamento = product.Faturamento,
                    DefeitoEquipamento = product.DefeitoEquipamento,
                    NumSerieEquipamento = product.NumSerieEquipamento,
                    TipoNota = product.TipoNota,
                    NumNota = product.NumNota,
                    ChaveNotaExterna = product.ChaveNotaExterna,
                    DataPedido = product.DataPedido,
                });
            }
        }
        listaprodutos.ItemsSource = new List<InfoPedido>();
        listaprodutos.ItemsSource = InfoPedido;
    }
    private async void AddProduct()
	{
        // Cria a conexão usando o banco de dados assíncrono
        var connection = App.Database.GetConnection();
        // Consulta para pegar o último NumeroPedido
        var ultimoPedido = await connection.Table<ProdutosPedido>().ToListAsync();
        if (ultimoPedido.Count == 0)
        {
            Produtos.Add(new ProdutosPedido
            {
                NumeroPedido = 1,
                Codigo = "product.Codigo",
                Descricao = "product.Descricao",
                Quantidade = "product.Quantidade",
                Valor = "product.Valor",
                VersaoPeca = "product.VersaoPeca",
                DataPedido = DateTime.Now,
            });
        }
        else
        {
        foreach (var product in ultimoPedido){
            Produtos.Add(new ProdutosPedido
            {
                NumeroPedido = product.NumeroPedido,
                Codigo = product.Codigo,
                Descricao = product.Descricao,
                Quantidade = product.Quantidade,
                Valor = product.Valor,
                VersaoPeca = product.VersaoPeca,
                DataPedido = product.DataPedido
            });
            }
        }
    }
}