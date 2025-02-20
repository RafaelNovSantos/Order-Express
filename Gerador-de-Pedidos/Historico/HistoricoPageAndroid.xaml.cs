using System.Collections.ObjectModel;

#if WINDOWS
using Microsoft.UI.Input;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.UI.Input.Preview.Injection;
#endif


namespace Gerador_de_Pedidos.Historico;

public partial class HistoricoPageAndroid : ContentPage
{


    public ObservableCollection<ProdutosPedido> Produtos { get; set; } = new ObservableCollection<ProdutosPedido>();
    public ObservableCollection<InfoPedido> InfoPedido { get; set; } = new ObservableCollection<InfoPedido>();

    public ObservableCollection<InfoPedido> pedidosFiltrados
    {
        get; set;
    }

    public bool pedidoatualizado;

    public HistoricoPageAndroid()
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


    private async void ClickedMenu(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;

        var pedido = button.BindingContext as InfoPedido;
        if (pedido == null) return;

        var option = await Shell.Current.DisplayActionSheet("Menu", "Cancelar", null, "Editar", "Excluir Pedido");

        if (option == "Editar") OnEditMenuClicked(pedido);
        if (option == "Excluir Pedido") OnDeleteMenuClicked(pedido);
    }






    private async void OnEditMenuClicked(InfoPedido pedido)
    {
        if (pedido == null) return;

        var connection = App.Database.GetConnection();

        // Consulta o banco de dados para obter os detalhes do pedido
        var pedidoBanco = await connection.Table<InfoPedido>()
            .Where(p => p.NumeroPedido == pedido.NumeroPedido)
            .FirstOrDefaultAsync();

        if (pedidoBanco == null) return; // Evita erro caso o pedido não seja encontrado

        // Armazena os valores no serviço de compartilhamento de dados
        var dadosService = DependencyService.Get<DadosCompartilhadosService>();

        dadosService.Vendedor = pedidoBanco.Vendedor;
        dadosService.Cliente = pedidoBanco.Cliente;
        dadosService.NumeroPedido = pedidoBanco.NumeroPedido;
        dadosService.TipoPedido = pedidoBanco.TipoPedido;
        dadosService.ValorFrete = pedidoBanco.ValorFrete;
        dadosService.TipoFrete = pedidoBanco.TipoFrete;
        dadosService.TipoPagamento = pedidoBanco.TipoPagamento;
        dadosService.Faturamento = pedidoBanco.Faturamento;
        dadosService.DefeitoEquipamento = pedidoBanco.DefeitoEquipamento;
        dadosService.NumSerieEquipamento = pedidoBanco.NumSerieEquipamento;
        dadosService.TipoNota = pedidoBanco.TipoNota;
        dadosService.NumNota = pedidoBanco.NumNota;
        dadosService.ChaveNotaExterna = pedidoBanco.ChaveNotaExterna;
        dadosService.DataPedido = pedidoBanco.DataPedido;

        // Navega para a página principal com os parâmetros de edição
#if WINDOWS
        await Shell.Current.GoToAsync("//MainPage?edicao=true&titulopedido=edicao");
#elif ANDROID
        await Shell.Current.GoToAsync("//MainPageAndroid?edicao=true&titulopedido=edicao");

#endif
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





    private async void OnDeleteMenuClicked(InfoPedido pedido)
    {
        if (pedido == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir Pedido",
            "Tem certeza que deseja excluir este pedido?",
            "Sim", "Não"
        );

        if (!confirm) return;

        await App.Database.DeletarPedidoPorNumeroPedidoAsync(pedido.NumeroPedido);
        await App.Database.DeletarProdutoPorNumeroPedidoAsync(pedido.NumeroPedido);
        InfoPedido.Clear();

        // Atualiza a lista após a exclusão
        AddPedido();
    }

    private async void AddPedido()
    {
        // Cria a conexão usando o banco de dados assíncrono
        var connection = App.Database.GetConnection();
        // Consulta para pegar o último NumeroPedido
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