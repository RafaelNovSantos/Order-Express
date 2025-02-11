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



    private async void OnEditMenuClicked(object sender, EventArgs e)
    {
        string Vendedor = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        int NumeroPedido = 2;  // Defina o valor conforme necess�rio
        string TipoPedido = "Venda";  // Defina o valor conforme necess�rio
        string ValorFrete = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        string TipoFrete = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        string TipoPagamento = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        string Faturamento = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        string DefeitoEquipamento = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        string NumSerieEquipamento = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        string TipoNota = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        string NumNota = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        string ChaveNotaExterna = "Valor do Vendedor";  // Defina o valor conforme necess�rio
        string DataPedido = "Valor do Vendedor";  // Defina o valor conforme necess�rio



        // Armazena o valor no servi�o
        var dadosService = DependencyService.Get<DadosCompartilhadosService>();

        dadosService.Vendedor = Vendedor;
        dadosService.NumeroPedido = NumeroPedido;
        dadosService.TipoPedido = TipoPedido;
        dadosService.ValorFrete = ValorFrete;
        dadosService.TipoFrete = TipoFrete;
        dadosService.TipoPagamento = TipoPagamento;
        dadosService.Faturamento = Faturamento;
        dadosService.DefeitoEquipamento = DefeitoEquipamento;
        dadosService.NumSerieEquipamento = NumSerieEquipamento;
        dadosService.TipoNota = TipoNota;
        dadosService.NumNota = NumNota;
        dadosService.ChaveNotaExterna = ChaveNotaExterna;
        dadosService.DataPedido = DataPedido;



        // Navega de volta para a MainPage
        Shell.Current.GoToAsync("//MainPage");

    }


    private async void OnDeleteMenuClicked(object sender, EventArgs e) { }
        private async void AddPedido()
    {
        // Cria a conex�o usando o banco de dados ass�ncrono
        var connection = App.Database.GetConnection();
        // Consulta para pegar o �ltimo NumeroPedido
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
        // Cria a conex�o usando o banco de dados ass�ncrono
        var connection = App.Database.GetConnection();
        // Consulta para pegar o �ltimo NumeroPedido
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