using System.Collections.ObjectModel;

#if WINDOWS
using Microsoft.UI.Input;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.UI.Input.Preview.Injection;
#endif


namespace Gerador_de_Pedidos.Historico;

public partial class HistoricoPage : ContentPage
{


    public ObservableCollection<ProdutosPedido> Produtos { get; set; } = new ObservableCollection<ProdutosPedido>();
    public ObservableCollection<InfoPedido> InfoPedido { get; set; } = new ObservableCollection<InfoPedido>();

    public ObservableCollection<InfoPedido> pedidosFiltrados
    {
        get; set;
    }

    public bool pedidoatualizado;

    public HistoricoPage()
	{
		InitializeComponent();


        pedidoatualizado = true;
        AddPedido();

        pedidosFiltrados = new ObservableCollection<InfoPedido>(InfoPedido);
        AtualizarCoresBotoes("Todos");
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        var dadosService = DependencyService.Get<DadosCompartilhadosService>();

        if (dadosService.BaseChanged == true && pedidoatualizado != true)
        {
            InfoPedido.Clear();
            AddPedido();
            dadosService.BaseChanged = false;
            pedidoatualizado = false;
        }
        pedidoatualizado = false;
    }





    private void ClickedMenu(object sender, EventArgs e)
{
#if WINDOWS
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
#endif
}




    private async void OnEditMenuClicked(object sender, EventArgs e)
    {
    

        var menuItem = sender as MenuFlyoutItem;
        if (menuItem?.BindingContext is InfoPedido pedido)
        {
            // Confirma��o de exclus�o
            var connection = App.Database.GetConnection();

            // Consulta para pegar o �ltimo NumeroPedido
            var pedidoBanco = await connection.Table<InfoPedido>()
        .Where(p => p.NumeroPedido == pedido.NumeroPedido)
        .FirstOrDefaultAsync();

            string Vendedor = pedidoBanco.Vendedor;
            string Cliente = pedidoBanco.Cliente;
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




            // Armazena o valor no servi�o
            var dadosService = DependencyService.Get<DadosCompartilhadosService>();

            dadosService.Vendedor = Vendedor;
            dadosService.Cliente = Cliente;
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
            await Shell.Current.GoToAsync("//MainPage?edicao=true&titulopedido=edicao");




        }
        // Navega de volta para a MainPage

    }
    private void FiltrarPedidos(string filtro)
    {
      
        InfoPedido.Clear();


        // Filtra os pedidos com base no filtro
        var pedidosSelecionados = (string.IsNullOrEmpty(filtro) || filtro == "Todos")
            ? pedidosFiltrados
            : pedidosFiltrados.Where(p => p.TipoPedido.StartsWith(filtro, StringComparison.OrdinalIgnoreCase));


        // Adiciona os pedidos filtrados
        foreach (var pedido in pedidosSelecionados)
        {
            InfoPedido.Add(pedido);
        }

        // Atualiza o ItemsSource com os pedidos filtrados
        listaprodutos.ItemsSource = new List<InfoPedido>();
        listaprodutos.ItemsSource = InfoPedido;

        // Atualiza as cores dos bot�es
        AtualizarCoresBotoes(filtro);
    }

    private void AddProdutosFiltrados()
    {
        pedidosFiltrados.Clear();
        foreach (var produto in InfoPedido)
        {
            pedidosFiltrados.Add(produto);
        }
    }
    private void AtualizarCoresBotoes(string filtro)
    {
        // Alterando a cor dos bot�es com base no filtro
        btnTodos.BackgroundColor = (filtro == "Todos") ? Color.FromArgb("#00c4b4") : Color.FromArgb("#fff");
        btnTodos.TextColor = (filtro == "Todos") ? Color.FromArgb("#fff") : Color.FromArgb("#000");
        btnTodos.BorderColor = (filtro == "Todos") ? Color.FromArgb("#fff") : Color.FromArgb("#00c4b4");

        btnOrcamento.BackgroundColor = (filtro == "Or�amento") ? Color.FromArgb("#00c4b4") : Color.FromArgb("#fff");
        btnOrcamento.TextColor = (filtro == "Or�amento") ? Color.FromArgb("#fff") : Color.FromArgb("#000");
        btnOrcamento.BorderColor = (filtro == "Or�amento") ? Color.FromArgb("#fff") : Color.FromArgb("#00c4b4");

        btnVenda.BackgroundColor = (filtro == "Venda") ? Color.FromArgb("#00c4b4") : Color.FromArgb("#fff");
        btnVenda.TextColor = (filtro == "Venda") ? Color.FromArgb("#fff") : Color.FromArgb("#000");
        btnVenda.BorderColor = (filtro == "Venda") ? Color.FromArgb("#fff") : Color.FromArgb("#00c4b4");

        btnGarantia.BackgroundColor = (filtro == "Garantia") ? Color.FromArgb("#00c4b4") : Color.FromArgb("#fff");
        btnGarantia.TextColor = (filtro == "Garantia") ? Color.FromArgb("#fff") : Color.FromArgb("#000");
        btnGarantia.BorderColor = (filtro == "Garantia") ? Color.FromArgb("#fff") : Color.FromArgb("#00c4b4");

    }

    private async void OnDeleteMenuClicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuFlyoutItem;
        if (menuItem?.BindingContext is InfoPedido pedido)
        {
            await App.Database.DeletarPedidoPorNumeroPedidoAsync(pedido.NumeroPedido);
            await App.Database.DeletarProdutoPorNumeroPedidoAsync(pedido.NumeroPedido);
            InfoPedido.Clear();
            AddPedido();
        }
    }
        private async void AddPedido()
    {
        // Cria a conex�o usando o banco de dados ass�ncrono
        var connection = App.Database.GetConnection();
        // Consulta para pegar o �ltimo NumeroPedido
        var pedido = await connection.Table<InfoPedido>().ToListAsync();
        if (pedido.Count == 0)
        {
            
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
                    Cliente = product.Cliente,
                    ValorFrete = product.ValorFrete,
                    TipoPagamento = product.TipoPagamento,
                    Faturamento = product.Faturamento,
                    DefeitoEquipamento = product.DefeitoEquipamento,
                    NumSerieEquipamento = product.NumSerieEquipamento,
                    TipoNota = product.TipoNota,
                    NumNota = product.NumNota,
                    ChaveNotaExterna = product.ChaveNotaExterna,
                    DataPedido = product.DataPedido,
                    ValorTotal = product.ValorTotal,
                });
            }
        }
        listaprodutos.ItemsSource = new List<InfoPedido>();
        listaprodutos.ItemsSource = InfoPedido;

        AddProdutosFiltrados();

        

        
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

    private void btnGarantia_Clicked(object sender, EventArgs e)
    {
        // sender � o bot�o que foi clicado
        var clickedButton = sender as Button;
        FiltrarPedidos(clickedButton.Text);

    }
}