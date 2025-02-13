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



    public HistoricoPage()
	{
		InitializeComponent();

        AddPedido();

        pedidosFiltrados = new ObservableCollection<InfoPedido>(InfoPedido);
        AtualizarCoresBotoes("Todos");
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

        // Atualiza as cores dos botões
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
        // Alterando a cor dos botões com base no filtro
        btnTodos.BackgroundColor = (filtro == "Todos") ? Color.FromArgb("#00c4b4") : Color.FromArgb("#fff");
        btnTodos.TextColor = (filtro == "Todos") ? Color.FromArgb("#fff") : Color.FromArgb("#000");
        btnTodos.BorderColor = (filtro == "Todos") ? Color.FromArgb("#fff") : Color.FromArgb("#00c4b4");

        btnOrcamento.BackgroundColor = (filtro == "Orçamento") ? Color.FromArgb("#00c4b4") : Color.FromArgb("#fff");
        btnOrcamento.TextColor = (filtro == "Orçamento") ? Color.FromArgb("#fff") : Color.FromArgb("#000");
        btnOrcamento.BorderColor = (filtro == "Orçamento") ? Color.FromArgb("#fff") : Color.FromArgb("#00c4b4");

        btnVenda.BackgroundColor = (filtro == "Venda") ? Color.FromArgb("#00c4b4") : Color.FromArgb("#fff");
        btnVenda.TextColor = (filtro == "Venda") ? Color.FromArgb("#fff") : Color.FromArgb("#000");
        btnVenda.BorderColor = (filtro == "Venda") ? Color.FromArgb("#fff") : Color.FromArgb("#00c4b4");

        btnGarantia.BackgroundColor = (filtro == "Garantia") ? Color.FromArgb("#00c4b4") : Color.FromArgb("#fff");
        btnGarantia.TextColor = (filtro == "Garantia") ? Color.FromArgb("#fff") : Color.FromArgb("#000");
        btnGarantia.BorderColor = (filtro == "Garantia") ? Color.FromArgb("#fff") : Color.FromArgb("#00c4b4");

    }

    private async void OnDeleteMenuClicked(object sender, EventArgs e) {

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
                    ValorTotal = product.ValorTotal,
                });
            }
        }
        listaprodutos.ItemsSource = new List<InfoPedido>();
        listaprodutos.ItemsSource = InfoPedido;

        AddProdutosFiltrados(); 
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
                // Verifica se o produto já existe na lista pelo Número do Pedido (ou outro identificador único)
                var pedidoExistente = InfoPedido.FirstOrDefault(p => p.NumeroPedido == product.NumeroPedido);

                if (pedidoExistente != null)
                {
                    // Atualiza os dados do pedido existente
                    pedidoExistente.TipoPedido = product.TipoPedido;
                    pedidoExistente.Vendedor = product.Vendedor;
                    pedidoExistente.ValorFrete = product.ValorFrete;
                    pedidoExistente.TipoPagamento = product.TipoPagamento;
                    pedidoExistente.Faturamento = product.Faturamento;
                    pedidoExistente.DefeitoEquipamento = product.DefeitoEquipamento;
                    pedidoExistente.NumSerieEquipamento = product.NumSerieEquipamento;
                    pedidoExistente.TipoNota = product.TipoNota;
                    pedidoExistente.NumNota = product.NumNota;
                    pedidoExistente.ChaveNotaExterna = product.ChaveNotaExterna;
                    pedidoExistente.DataPedido = product.DataPedido;
                    pedidoExistente.ValorTotal = product.ValorTotal;

                }

            }

            // Atualiza a interface gráfica
            listaprodutos.ItemsSource = null;  // Limpa antes de atualizar
            listaprodutos.ItemsSource = InfoPedido;

            AddProdutosFiltrados();
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

    private void btnGarantia_Clicked(object sender, EventArgs e)
    {
        // sender é o botão que foi clicado
        var clickedButton = sender as Button;
        FiltrarPedidos(clickedButton.Text);

    }
}