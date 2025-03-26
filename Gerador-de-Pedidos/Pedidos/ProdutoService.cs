using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Gerador_de_Pedidos;
using Gerador_de_Pedidos.Pedidos.Models;


public class ProdutoService
{


    public async Task ProcessarSelecao(string selectedValue, string cod, IList<Product> listaProdutos, Entry txtDescricao, Entry txtValor, Label lblStatusProduto)
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
                await Application.Current.MainPage.DisplayAlert("Erro", "Tipo de valor não selecionado.", "OK");
                lblStatusProduto.Text = "Tipo de valor não selecionado.";
                lblStatusProduto.TextColor = Color.FromHex("#FF0000"); // Vermelho para indicar erro
                return;
        }

        // Recarregar os dados da planilha usando a coluna correta
        if (!string.IsNullOrEmpty(cod))
        {
            cod = cod.ToUpper();
            // Buscar o item correspondente na lista
            var item = listaProdutos.FirstOrDefault(i => i.Codigo == cod);
            if (item != null)
            {
                txtDescricao.Text = item.Descricao;

                // Converter item.Valor para decimal e formatar com duas casas decimais
                if (decimal.TryParse(((item.Valor).Replace("R", "").Replace("$", "")), out decimal valorNumerico))
                {
                    txtValor.Text = valorNumerico.ToString("F2");
                }
                else
                {
                    txtValor.Text = string.Empty;
                    txtValor.Text = "Valor inválido";
                }

                // Atualizar o status do produto
                lblStatusProduto.Text = "Produto Encontrado";
                lblStatusProduto.FontSize = 15;
                lblStatusProduto.TextColor = Color.FromHex("#00FF00"); // Verde para indicar sucesso
            }
            else
            {
                txtDescricao.Text = string.Empty;
                txtValor.Text = string.Empty;
                lblStatusProduto.Text = "Produto Não Encontrado";
                lblStatusProduto.FontSize = 12;
                lblStatusProduto.TextColor = Color.FromHex("#FF0000"); // Vermelho para indicar erro
            }
        }
        else
        {
            txtDescricao.Text = string.Empty;
            txtValor.Text = string.Empty;
            lblStatusProduto.FontSize = 17;
            lblStatusProduto.Text = "Digite o Código...";
            lblStatusProduto.TextColor = Color.FromHex("#FFFAFF00"); // Laranja para indicar aviso
        }
    }

    public void CalcularFaturamentoService(Func<decimal> _callValorTotal, Picker pagPicker, Entry txtFaturamento)
    {
        decimal valorTotal = _callValorTotal();
        if (valorTotal < 110)
        {
            pagPicker.SelectedIndex = 0;
            txtFaturamento.Text = "";
        }
        else
        {
            pagPicker.SelectedIndex = 1;

            if (valorTotal <= 300)
                txtFaturamento.Text = "15 dias";
            else if (valorTotal <= 700)
                txtFaturamento.Text = "15/30";
            else if (valorTotal <= 2000)
                txtFaturamento.Text = "30/45/60";
            else if (valorTotal <= 3500)
                txtFaturamento.Text = "30/60/90";
            else if (valorTotal <= 5000)
                txtFaturamento.Text = "30/45/60/75/90/105";
            else
                txtFaturamento.Text = "30/60/90/120";
        }
    }

    public async void EditOrderToHistoric(Action _cleanInputs, Button btncancelaredicao, Entry txtVendedor, Budget MeuBudget, DadosCompartilhadosService dadosService, Picker pedido, Entry txtFrete, Picker TipoFrete, Picker pag, Entry txtFaturamento, Entry txtDefeitos, Entry txtNS, Picker notaPicker, Entry txtnota,Entry txtChaveNotaExterna, ObservableCollection<Product> ListaSelecionados, CollectionView listaProdutosSelect, Action AddProdutosProdutosFiltradosSelecionados, Func<decimal> CallValorTotal)
    {
        
        // Deixa a página pedidos em modo de edição conforme pedido selecionado na página historico
        _cleanInputs();
        btncancelaredicao.IsVisible = true;
        txtVendedor.Text = dadosService.Vendedor;
        MeuBudget.Numero_Pedido = dadosService.NumeroPedido;
        pedido.SelectedItem = dadosService.TipoPedido;
        txtFrete.Text = dadosService.ValorFrete.ToString();
        TipoFrete.SelectedItem = dadosService.TipoFrete;
        pag.SelectedItem = dadosService.TipoPagamento;
        txtFaturamento.Text = dadosService.Faturamento;
        txtDefeitos.Text = dadosService.DefeitoEquipamento;
        txtNS.Text = dadosService.NumSerieEquipamento;
        notaPicker.SelectedItem = dadosService.TipoNota;
        txtnota.Text = dadosService.NumNota;
        txtChaveNotaExterna.Text = dadosService.ChaveNotaExterna;

        // Confirmação de exclusão
        var connection = App.Database.GetConnection();

        // Consulta para pegar o último NumeroPedido
        var produtosBanco = await connection.Table<ProdutosPedido>()
    .Where(p => p.NumeroPedido == dadosService.NumeroPedido)
    .ToListAsync();

        foreach (var item in produtosBanco)
        {
            var produto = new Product
            {
                Codigo = item.Codigo,
                Descricao = item.Descricao,
                Valor = item.Valor,
                Quantidade = item.Quantidade,
                Versao_Peca = item.VersaoPeca,
                Data = item.DataPedido
            };
            ListaSelecionados.Add(produto);
        }



        // Atualizar a visualização da lista
        listaProdutosSelect.ItemsSource = null; // Limpar a origem de itens para forçar a atualização
        listaProdutosSelect.ItemsSource = ListaSelecionados;
        AddProdutosProdutosFiltradosSelecionados();
        CallValorTotal();
    }

    public void CleanInputsService( Entry txtVendedor, Picker pedido, Entry txtFrete, Picker TipoFrete, Picker pag, Entry txtFaturamento, Entry txtDefeitos, Entry txtNS, Picker notaPicker, Entry txtnota, Entry txtChaveNotaExterna, ObservableCollection<Product> ListaSelecionados, CollectionView listaProdutosSelect, Picker equipamentos, Picker valores, Entry txtCodigo, Entry txtDescricao, Entry txtValor, Entry txtQuantidade, Entry txtVersion)
    {
        ListaSelecionados.Clear();
        listaProdutosSelect.ItemsSource = null; // Limpar a origem de itens para forçar a atualização
        listaProdutosSelect.ItemsSource = ListaSelecionados;
        pedido.SelectedIndex = 0;
        equipamentos.SelectedIndex = 0;
        pag.SelectedIndex = 0;
        notaPicker.SelectedIndex = 0;
        TipoFrete.SelectedIndex = 0;
        valores.SelectedIndex = 0;
        // Caso haja algum valor numérico, defina como 0 ou o valor default
        txtCodigo.Text = string.Empty;
        txtDescricao.Text = string.Empty;
        txtValor.Text = string.Empty;
        txtQuantidade.Text = string.Empty;
        txtVendedor.Text = string.Empty;
        txtFrete.Text = string.Empty;
        txtVersion.Text = string.Empty;
        txtFaturamento.Text = string.Empty;
        txtDefeitos.Text = string.Empty;
        txtNS.Text = string.Empty;
        txtnota.Text = string.Empty;
        txtChaveNotaExterna.Text = string.Empty;
    }


        // Atualiza a visibilidade com base nos estados calculados
    public void OnVerificarSelecoesClickedService(Entry txtVendedor, Picker pedido, HorizontalStackLayout secaofrete, Picker TipoFrete, Picker pag, Label txtPag, Entry txtFaturamento, Entry txtDefeitos, Entry txtNS, Picker notaPicker, Label typeNota, Entry txtnota, Entry txtChaveNotaExterna, Func<decimal> CallValorTotal)
    {
        // Obtém as seleções de cada Picker
        var pickerPedido = pedido.SelectedItem as string;
        var pickerNota = notaPicker.SelectedItem as string;
        var pickerPagamento = pag.SelectedItem as string;

        // Determina os estados com base nas seleções
        bool isVenda = pickerPedido == "Venda";
        bool isOrcamento = pickerPedido == "Orçamento";
        bool isPix = pickerPagamento == "PIX";
        bool isGarantia = pickerPedido == "Garantia com retorno" || pickerPedido == "Garantia sem retorno";
        bool isNotaExterna = pickerNota == "Nota Externa" && isGarantia;
        bool isNotaInterna = pickerNota == "Nota Interna" && isGarantia;

        // Atualiza a visibilidade com base nos estados calculados
        txtFaturamento.IsVisible = !isGarantia && !isPix;
        txtVendedor.IsVisible = !isOrcamento;
        txtChaveNotaExterna.IsVisible = isNotaExterna;
        txtDefeitos.IsVisible = !isVenda && !isOrcamento;
        txtNS.IsVisible = !isVenda && !isOrcamento;
        notaPicker.IsVisible = !isVenda && !isOrcamento;
        txtnota.IsVisible = !isVenda && !isOrcamento;
        typeNota.IsVisible = !isVenda && !isOrcamento;
        pag.IsVisible = !isGarantia;
        txtPag.IsVisible = !isGarantia;
        secaofrete.IsVisible = !isGarantia;
        TipoFrete.IsVisible = !isGarantia;
        CallValorTotal();
    }

}
