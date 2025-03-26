using Gerador_de_Pedidos.Pedidos.Handlers;
using Gerador_de_Pedidos.Pedidos.Models;
using System.Diagnostics;
using System.Collections.ObjectModel;
using GeradordePedidos.Pedidos.Handlers;


namespace Gerador_de_Pedidos
{
    // Vincula o parâmetro "edicao" da URL à propriedade Edicao desta página
    [QueryProperty(nameof(Edicao), "edicao")]
    public partial class MainPage : ContentPage
    { // Propriedade que receberá o valor do parâmetro "edicao"
        public string? Edicao { get; set; }
        public string? Titulo_Pedido { get; set; }

        private string selectedSheetName = string.Empty;

        AdicionarHandler _adicionarHandler;

        ProdutoService _produtoService = new ProdutoService();

        EditarHandler _editarHandler;

        LinkService _linkService = new LinkService();

        LinkService _linkServiceExecute;

        private readonly SalvarPedido _salvarPedido;
        public ObservableCollection<Product> ProdutosFiltradosExcel { get; set; }
        public ObservableCollection<Product> ProdutosFiltradosSelecionados { get; set; }


        public List<Product> Lista = new List<Product>();
        public ObservableCollection<Product> ListaSelecionados { get; set; } = new ObservableCollection<Product>();

        public Budget MeuBudget { get; set; }

        private string _ultimoItemSelecionado = string.Empty;
        public MainPage()
        {
            InitializeComponent();
           LoadLink();
            
             GetProximoNumeroPedidoAsync();
           
            MeuBudget = new Budget { Numero_Pedido = 0 };
            MeuBudget = new Budget { Valor_Total = "0,00" }; // Inicializa com zero
            MeuBudget = new Budget { Titulo_Pedido = "Pedido Número:" };
            _salvarPedido = new SalvarPedido(App.Database);
            ProdutosFiltradosExcel = new ObservableCollection<Product>(Lista);
            ProdutosFiltradosSelecionados = new ObservableCollection<Product>(Lista);
            BindingContext = this;

             _adicionarHandler = new AdicionarHandler(
    ListaSelecionados,
    listaProdutosSelect,
    valores,
    txtCodigo,
    txtDescricao,
    txtValor,
    txtQuantidade,
    txtVersion,
    searchBarProdutoSelecionado,
    OnPickerSelectionChangedPrice,
    AddProdutosProdutosFiltradosSelecionados,
    CallValorTotal,
    CalcularFaturamento
);
            _linkServiceExecute = new LinkService(loadingIndicatorPedido, lblStatusProduto);

            _editarHandler = new EditarHandler(ListaSelecionados, listaProdutosSelect, searchBarProdutoSelecionado, AddProdutosProdutosFiltradosSelecionados, CalcularFaturamento, CallValorTotal);

        }


        // Esse método é chamado automaticamente quando a página recebe parâmetros de consulta.
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("edicao"))
            {
                Edicao = query["edicao"]?.ToString();
            }
            if (query.ContainsKey("titulopedido"))
            {
                Titulo_Pedido = query["titulopedido"]?.ToString();
            }
        }


        // Use OnAppearing para reagir à navegação e, se necessário, executar a lógica de edição.
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!string.IsNullOrEmpty(Edicao) && Edicao.Equals("true", StringComparison.OrdinalIgnoreCase))
            {

                var dadosService = DependencyService.Get<DadosCompartilhadosService>();
                if (dadosService != null && dadosService.NumeroPedido != 0)
                {
                    // Deixa a página pedidos em modo de edição conforme pedido selecionado na página historico
                    _produtoService.EditOrderToHistoric(CleanInputs, btncancelaredicao, txtVendedor, MeuBudget, dadosService, pedido, txtFrete, TipoFrete, pag, txtFaturamento, txtDefeitos, txtNS, notaPicker, txtnota, txtChaveNotaExterna, ListaSelecionados, listaProdutosSelect, AddProdutosProdutosFiltradosSelecionados, CallValorTotal);
                }

                Edicao = "false";
            }
            //Verifica se o pedido atual é o próximo pedido do banco, se não for ele apaga todos os dados
            if (!btncancelaredicao.IsVisible)
            {
                 await GetProximoNumeroPedidoAsync();

            }
        }

        private void CleanInputs()
        {
            _produtoService.CleanInputsService(txtVendedor,pedido,txtFrete,TipoFrete,pag,txtFaturamento,txtDefeitos,txtNS,notaPicker,txtnota,txtChaveNotaExterna,ListaSelecionados,listaProdutosSelect,equipamentos,valores,txtCodigo,txtDescricao,txtValor,txtQuantidade,txtVersion);
        }

        private void OnSearchBarProdutosExcelTextChanged(object sender, TextChangedEventArgs e)
        {
            string termoBusca = e.NewTextValue?.ToLower() ?? "";
            Lista.Clear();
            foreach (var produto in ProdutosFiltradosExcel)
            {
                if (produto.Codigo.ToLower().Contains(termoBusca) ||
                    produto.Descricao.ToLower().Contains(termoBusca))
                {
                    Lista.Add(produto);
                }
            }
            // Atualizar a visualização da lista
            listaProdutosExcel.ItemsSource = null; // Limpar a origem de itens para forçar a atualização
            listaProdutosExcel.ItemsSource = Lista;
        }

        private void OnSearchBarProdutoSelecionadoTextChanged(object sender, TextChangedEventArgs e)
        {
            string termoBusca = e.NewTextValue?.ToLower() ?? "";
            ListaSelecionados.Clear();
            foreach (var produto in ProdutosFiltradosSelecionados)
            {
                if (produto.Codigo.ToLower().Contains(termoBusca) ||
                    produto.Descricao.ToLower().Contains(termoBusca))
                {
                    ListaSelecionados.Add(produto);
                }
            }
            // Atualizar a visualização da lista
            listaProdutosSelect.ItemsSource = null; // Limpar a origem de itens para forçar a atualização
            listaProdutosSelect.ItemsSource = ListaSelecionados;
        }
        public async Task<int> GetProximoNumeroPedidoAsync()
        {
            // Cria a conexão usando o banco de dados assíncrono
            var connection = App.Database.GetConnection();

            // Consulta para pegar o último NumeroPedido
            var ultimoPedido = await connection.Table<ProdutosPedido>().OrderByDescending(p => p.NumeroPedido).FirstOrDefaultAsync();

            MeuBudget.Numero_Pedido = ultimoPedido?.NumeroPedido + 1 ?? 1;

            CallValorTotal();
            // Se houver pedidos, incrementa o último NumeroPedido + 1, senão começa com 1
            return ultimoPedido?.NumeroPedido + 1 ?? 1;
        }
        private async void LoadLink()
        {

            await _linkService.LoadLink(pedido, equipamentos, pag, notaPicker, TipoFrete, valores, OnPickerSelectionChangedPrice);

        }
        private async void OnAlterarLinkClicked(object sender, EventArgs e)
        {
           
            
            await _linkService.AlterarLink( loadingIndicatorPedido);
            LoadLink();

        }

        //Bloqueia caracteres, funciona somente números inteiros
        private void OnIntegerTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                if (!e.NewTextValue.All(char.IsDigit))
                {
                    DisplayAlert("Entrada Inválida", "Por favor, insira apenas números.", "OK");
                    entry.Text = e.OldTextValue;
                }
            }
        }
        //Bloqueia caracteres, funciona somente números inteiros e double
        private void OnDoubleTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                if (e.NewTextValue == "Valor inválido")
                {
                    // Exibe a mensagem para "N/A"
                    DisplayAlert("Valor Inválido", "Produto na planilha sem valor", "OK");
                    entry.Text = e.OldTextValue;
                }
                else if (!double.TryParse(e.NewTextValue, out _))
                {
                    // Exibe a mensagem para valores não numéricos
                    DisplayAlert("Entrada Inválida", "Por favor, insira apenas números.", "OK");
                    entry.Text = e.OldTextValue;
                }
            }
            CallValorTotal();
            CalcularFaturamento();
        }
        private void SelectionChangedCopyCod(object sender, SelectionChangedEventArgs e)
        {

            // Obtém os itens selecionados na CollectionView
            var selectedItems = e.CurrentSelection.Cast<Product>().ToList();

            if (selectedItems.Count > 0)
            {
                // Assume que seleciona apenas um item
                var selectedItem = selectedItems.FirstOrDefault();

                if (selectedItem != null && (lblStatusProduto.Text == "Produto Encontrado" || lblStatusProduto.Text == "Digite o Código..." || lblStatusProduto.Text == "Produto Não Encontrado"))
                {
                    txtCodigo.Text = null;
                    // Atualiza o txtCodigo com o código do produto selecionado
                    txtCodigo.Text = selectedItem.Codigo;
                    txtVendedor.Focus();
                    txtQuantidade.Focus();
                    // Força a chamada ao método OnTxtCodigoTextChangedUnified
                    OnTxtCodigoTextChangedUnified(txtCodigo, new TextChangedEventArgs(string.Empty, txtCodigo.Text));
                }
            }
        }
        private async void OnPickerSelectionChanged(object sender, EventArgs e)
        {
            var saveCodigo = txtCodigo.Text;
            
            var selectedValue = valores.SelectedItem?.ToString();
            var picker = sender as Picker;
            if (picker != null && picker.SelectedItem != null)
            {
                selectedSheetName = picker.SelectedItem.ToString(); // Atualiza a variável de instância
                Debug.WriteLine($"Nome da planilha selecionada: {selectedSheetName}"); // Adicionando um log para depuração
                OnPickerSelectionChangedPrice(valores, EventArgs.Empty);
                await ExecuteTask(selectedValue);
                // Chame a função LerExcel passando o nome da planilha correspondente
                // Remova esta chamada se você não quiser que a função seja chamada automaticamente ao selecionar um item

            }
            txtCodigo.Text = "";
            txtCodigo.Text = saveCodigo;
        }

        //Bloqueia caracteres, funciona somente número inteiros e chama a função para atualizar a tabela com a lista da planilha
        private void OnTxtCodigoTextChangedUnified(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            // Chama o método para atualizar o Picker, ou qualquer outra lógica adicional
            OnPickerSelectionChangedPrice(valores, EventArgs.Empty);
        }
        // Defina o evento OnPickerSelectionChanged

        public async Task ExecuteTask(string selectedValue)
        {
            int columnToUse;
            // Definir a coluna correta com base na seleção do Picker
            switch (selectedValue)
            {
                case "Valor ATA":
                    columnToUse = 3; // Ajuste com base na coluna correta
                    break;
                case "Valor Oficina":

                    columnToUse = 4; // Ajuste com base na coluna correta
                    break;
                case "Valor Cliente Final":

                    columnToUse = 5; // Ajuste com base na coluna correta
                    break;
                default:

                    loadingIndicatorPedido.IsVisible = false; // Ocultar o botão se houve erro
                    loadingIndicatorPedido.IsRunning = false;
                    lblStatusProduto.IsVisible = true; // Mostrar o texto de status se houve erro
                    lblStatusProduto.Text = "Tipo de valor \nnão selecionado.";
                    lblStatusProduto.LineBreakMode = LineBreakMode.WordWrap; // Para quebra de linha automática

                    lblStatusProduto.TextColor = Color.FromArgb("#FF0000"); // Vermelho para indicar erro
                    return;
            }
            await LerExcelComColuna(_linkService.LinkPlanilha, selectedSheetName, columnToUse);
            // Evite chamar `LerExcelComColuna` novamente, já que isso é feito com base na seleção
            AddProdutosFiltradosExcel();
            OnSearchBarProdutosExcelTextChanged(searchBarprodutosexcel, new TextChangedEventArgs(searchBarprodutosexcel.Text, searchBarprodutosexcel.Text));
        }

        private void AddProdutosFiltradosExcel()
        {
            ProdutosFiltradosExcel.Clear();
            foreach (var produto in Lista)
            {
                ProdutosFiltradosExcel.Add(produto);
            }
        }
        private async Task ProcessarSelecao(string selectedValue)
        {
            // Exemplo de como usar o ProdutoService para processar a seleção

            await _produtoService.ProcessarSelecao(selectedValue, txtCodigo.Text, Lista, txtDescricao, txtValor, lblStatusProduto);
        }

        private async void OnPickerSelectionChangedPrice(object? sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker == null || picker.SelectedItem == null)
                return;

            var selectedValue = picker.SelectedItem.ToString();

            // Verifica se o item selecionado mudou
            if (selectedValue != _ultimoItemSelecionado)
            {
                lblStatusProduto.TextColor = Color.FromArgb("#00000000");
                await ExecuteTask(selectedValue);

                // Atualiza o item selecionado após a tarefa ser executada
                _ultimoItemSelecionado = selectedValue;
            }

            // Mostrar o botão de busca e ocultar o texto de status enquanto a busca está em andamento
            loadingIndicatorPedido.IsVisible = true;
            loadingIndicatorPedido.IsRunning = true;
            lblStatusProduto.IsVisible = true;
            lblStatusProduto.Text = "center";
            lblStatusProduto.TextColor = Color.FromArgb("#00000000");

            // Inicia a rotação do ícone e do botão de busca

            await ProcessarSelecao(selectedValue);

            // Parar a rotação e ocultar os ícones

            loadingIndicatorPedido.IsRunning = false;
            loadingIndicatorPedido.IsVisible = false;
            lblStatusProduto.IsVisible = true;
        }

        private async void OnAtualizarClicked(object sender, EventArgs e)
        {
            // Supondo que o Picker `valores` já tenha um item selecionado
            var selectedValue = valores.SelectedItem?.ToString();
            txtCodigo.Text += 0.ToString();

            if (txtCodigo.Text.Length > 0)
            {
                txtCodigo.Text = txtCodigo.Text.Substring(0, txtCodigo.Text.Length - 1);
            }

            if (selectedValue != null)
            {
                await ExecuteTask(selectedValue);
            }
            else
            {
                await DisplayAlert("Erro", "Nenhum valor selecionado.", "OK");
            }
        }
        async Task LerExcelComColuna(string fileUrl, string sheetName, int valorColumnIndex)
        {
            await _linkService.LerExcelComColuna(fileUrl, sheetName, valorColumnIndex, Lista, listaProdutosExcel, disableFrame, loadingIndicator);
        }

       
        private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = e.CurrentSelection.Cast<Product>().ToList();
        }
        private async void OnEditarClicked(object sender, EventArgs e)
        {
            await _editarHandler.HandlerEditarClicked();
        }
        private async void OnCancelarClicked(object sender, EventArgs e)
        {
           await GetProximoNumeroPedidoAsync();
            CleanInputs();
            btncancelaredicao.IsVisible = false;
        }
        private async void OnExcluirClicked(object sender, EventArgs e)
        {
            var selectedItems = listaProdutosSelect.SelectedItems.Cast<Product>().ToList();

            if (selectedItems.Count == 0)
            {
                await DisplayAlert("Aviso", "Nenhum item selecionado para excluir.", "OK");
                return;
            }

            // Confirmação para excluir os itens selecionados
            bool confirm = await DisplayAlert("Confirmação", $"Deseja realmente excluir {selectedItems.Count} item(s) selecionado(s)?", "Sim", "Não");
            if (!confirm)
                return;
            var saveTextSearchBarProdutoSelecioando = searchBarProdutoSelecionado.Text;

            if (!string.IsNullOrEmpty(saveTextSearchBarProdutoSelecioando))
            {
                searchBarProdutoSelecionado.Text = null;
            }

            foreach (var item in selectedItems)
            {
                ListaSelecionados.Remove(item);
            }
            // Atualiza a CollectionView e limpa a seleção
            listaProdutosSelect.SelectedItems.Clear(); // Limpa a seleção
            listaProdutosSelect.ItemsSource = null;
            listaProdutosSelect.ItemsSource = ListaSelecionados;
            AddProdutosProdutosFiltradosSelecionados();
            CallValorTotal();
            CalcularFaturamento();
            searchBarProdutoSelecionado.Text = saveTextSearchBarProdutoSelecioando;
        }
        private async void OnAdicionarClicked(object sender, EventArgs e)
        {
           await _adicionarHandler.HandleAdicionarClicked();

        }
        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            bool pedidosalvo = await _salvarPedido.SalvarPedidoAsync(
               MeuBudget,
               txtVendedor.Text,
               pedido.SelectedItem?.ToString() ?? "",
               txtFrete.Text,
               TipoFrete.SelectedItem?.ToString() ?? "",
               pag.SelectedItem?.ToString() ?? "",
               txtFaturamento.Text,
               txtDefeitos.Text,
               txtNS.Text,
               notaPicker.SelectedItem?.ToString() ?? "",
               txtnota.Text,
               txtChaveNotaExterna.Text,
               listaProdutosSelect.ItemsSource?.Cast<Product>().ToList() ?? new List<Product>(),
               ListaSelecionados,
               listaProdutosSelect,
               GetProximoNumeroPedidoAsync
           );
            if (pedidosalvo == true)
            {
                OnCancelarClicked(btncancelaredicao, EventArgs.Empty);
            }
        }

        private void AddProdutosProdutosFiltradosSelecionados()
        {
            ProdutosFiltradosSelecionados.Clear();
            foreach (var produto in ListaSelecionados)
            {
                ProdutosFiltradosSelecionados.Add(produto);
            }
        }
        private async void OnCopiarClicked(object sender, EventArgs e)
        {

            var service = new CopiarPedidoService();
            await service.CopiarTextoAsync(
                txtVendedor.Text,
                pedido.SelectedItem?.ToString(),
                TipoFrete.SelectedItem?.ToString(),
                pag.SelectedItem?.ToString(),
                txtFrete.Text,
                txtFaturamento.Text,
                txtDefeitos.Text,
                txtNS.Text,
                txtnota.Text,
                txtChaveNotaExterna.Text,
                notaPicker.SelectedItem,
                pedido.SelectedItem,
                listaProdutosSelect,
                btncopy,
                iconCopy
            );
        }
        private decimal CallValorTotal()
        {
            decimal frete = 0m;
            bool isFreteParsed = !string.IsNullOrEmpty(txtFrete.Text) &&
                                 decimal.TryParse(txtFrete.Text.Replace("R$", "").Trim(), out frete);
            decimal totalGeral = 0m;
            var pickerPedido = pedido.SelectedItem as string;
            bool isGarantia = pickerPedido == "Garantia com retorno" || pickerPedido == "Garantia sem retorno";


            if (listaProdutosSelect.ItemsSource != null)
            {
                foreach (var product in listaProdutosSelect.ItemsSource.Cast<Product>())
                {
                    var valorUnidade = decimal.TryParse(product.Valor, out var val) ? val : 0m;
                    var quantidade = decimal.TryParse(product.Quantidade, out var qnt) ? qnt : 0m;
                    totalGeral += valorUnidade * quantidade;
                }
            }
            // Adiciona o frete apenas uma vez, fora do loop
            if (isFreteParsed && !isGarantia)
            {
                totalGeral += frete;
            }
            if (MeuBudget != null)
            {
                MeuBudget.Valor_Total = $"{totalGeral:F2}";
            }
            else
            {
                Debug.WriteLine("MeuBudget é nulo");
            }

            return totalGeral;
        }
        private void OnVerificarSelecoesClicked(object sender, EventArgs e)
        {
            _produtoService.OnVerificarSelecoesClickedService(txtVendedor, pedido, secaofrete, TipoFrete, pag, txtpag, txtFaturamento , txtDefeitos, txtNS , notaPicker, typeNota, txtnota, txtChaveNotaExterna, CallValorTotal);
        }

        private void CalcularFaturamento()
        {
            _produtoService.CalcularFaturamentoService(CallValorTotal, pag, txtFaturamento);
        }
    }
}
