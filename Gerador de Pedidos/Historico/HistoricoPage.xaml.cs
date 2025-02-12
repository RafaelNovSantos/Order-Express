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


    protected override void OnAppearing()
    {
        base.OnAppearing();
        var dadosService = DependencyService.Get<DadosCompartilhadosService>();

        if (dadosService.BaseChanged == true)
        {
            UpdatePedido();
            dadosService.BaseChanged = false;
        }
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
    

        var menuItem = sender as MenuFlyoutItem;
        if (menuItem?.BindingContext is InfoPedido pedido)
        {
            // Confirmação de exclusão
            var connection = App.Database.GetConnection();

            // Consulta para pegar o último NumeroPedido
            var pedidoBanco = await connection.Table<InfoPedido>()
        .Where(p => p.NumeroPedido == pedido.NumeroPedido)
        .FirstOrDefaultAsync();

            string Vendedor = pedidoBanco.Vendedor;
            int NumeroPedido = pedidoBanco.NumeroPedido;
            string TipoPedido = pedidoBanco.TipoPedido;
            decimal? ValorFrete = pedidoBanco.ValorFrete;
            string TipoFrete = pedidoBanco.TipoFrete;
            string TipoPagamento = pedidoBanco.TipoPagamento;
            string Faturamento = pedidoBanco.Faturamento;
            string DefeitoEquipamento = pedidoBanco.DefeitoEquipamento;
            string NumSerieEquipamento = pedidoBanco.NumSerieEquipamento;
            string TipoNota = pedidoBanco.TipoNota;
            string NumNota = pedidoBanco.NumNota;
            string ChaveNotaExterna = pedidoBanco.ChaveNotaExterna;
            DateTime DataPedido = pedidoBanco.DataPedido;




            // Armazena o valor no serviço
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

            // Exemplo: passando edicao com o valor "true"
            _ = Shell.Current.GoToAsync("//MainPage?edicao=true");



        }
        // Navega de volta para a MainPage

    }


    private async void OnDeleteMenuClicked(object sender, EventArgs e) {
    
    
    }
        private async void AddPedido()
    {
        // Cria a conexão usando o banco de dados assíncrono
        var connection = App.Database.GetConnection();
        // Consulta para pegar o último NumeroPedido
        var pedido = await connection.Table<InfoPedido>().ToListAsync();
        if (pedido.Count == 0)
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
            foreach (var product in pedido)
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


    private async void UpdatePedido()
    {
        // Cria a conexão usando o banco de dados assíncrono
        var connection = App.Database.GetConnection();

        // Consulta para pegar o último NumeroPedido
        var pedidos = await connection.Table<InfoPedido>().ToListAsync();

        if (pedidos.Count != 0)
        {
            foreach (var product in pedidos)
            {
                var pedidoAtualizado = new InfoPedido
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
                };

                // Atualiza o pedido no banco de dados
                await App.Database.AtualizarInfoPedidooAsync(pedidoAtualizado);

                // Atualiza a lista
                InfoPedido.Add(pedidoAtualizado);
            }


            // Atualiza a interface gráfica
            listaprodutos.ItemsSource = new List<InfoPedido>();
            listaprodutos.ItemsSource = InfoPedido;
        }
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