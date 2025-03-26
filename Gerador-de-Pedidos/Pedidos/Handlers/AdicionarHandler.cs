using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerador_de_Pedidos.Pedidos.Handlers
{
    public class AdicionarHandler
    {
        private readonly ObservableCollection<Product> _listaSelecionados;
        private readonly CollectionView _listaProdutosSelect;
        private readonly Picker _valores;
        private readonly Entry _txtCodigo;
        private readonly Entry _txtDescricao;
        private readonly Entry _txtValor;
        private readonly Entry _txtQuantidade;
        private readonly Entry _txtVersion;
        private readonly SearchBar _searchBarProdutoSelecionado;
        private EventHandler _onPickerSelectionChangedPrice;
        private Action _addProdutosProdutosFiltradosSelecionados;
        private Func<decimal> _callValorTotal;
        private Action _calcularFaturamento;

        public AdicionarHandler(
            ObservableCollection<Product> listaSelecionados,
            CollectionView listaProdutosSelect,
            Picker valores,
            Entry txtCodigo,
            Entry txtDescricao,
            Entry txtValor,
            Entry txtQuantidade,
            Entry txtVersion,
            SearchBar searchBarProdutoSelecionado,
            EventHandler onPickerSelectionChangedPrice,
            Action addProdutosProdutosFiltradosSelecionados,
            Func<decimal> callValorTotal,
            Action calcularFaturamento)
        {
            _listaSelecionados = listaSelecionados;
            _listaProdutosSelect = listaProdutosSelect;
            _valores = valores;
            _txtCodigo = txtCodigo;
            _txtDescricao = txtDescricao;
            _txtValor = txtValor;
            _txtQuantidade = txtQuantidade;
            _txtVersion = txtVersion;
            _searchBarProdutoSelecionado = searchBarProdutoSelecionado;
            _onPickerSelectionChangedPrice = onPickerSelectionChangedPrice;
            _addProdutosProdutosFiltradosSelecionados = addProdutosProdutosFiltradosSelecionados;
            _callValorTotal = callValorTotal;
            _calcularFaturamento = calcularFaturamento;
        }

        public async Task HandleAdicionarClicked()
        {
            // Obtém os valores dos campos de entrada
            string cod = _txtCodigo.Text;
            string descricao = _txtDescricao.Text;
            string valor = _txtValor.Text;
            string quantidade = _txtQuantidade.Text;
            string versao_peca = _txtVersion.Text;

            // Verifica se todos os campos estão preenchidos
            bool isCodEmpty = string.IsNullOrEmpty(cod);
            bool isDescricaoEmpty = string.IsNullOrEmpty(descricao);
            bool isValorEmpty = string.IsNullOrEmpty(valor);
            bool isQuantidadeEmpty = string.IsNullOrEmpty(quantidade);

            if (isCodEmpty || isDescricaoEmpty || isValorEmpty || isQuantidadeEmpty)
            {
                // Cria uma mensagem com os campos que estão faltando
                string missingFieldsMessage = "Por favor, preencha os seguintes campos:\n";
                if (isCodEmpty) missingFieldsMessage += "- Código\n";
                if (isDescricaoEmpty) missingFieldsMessage += "- Descrição\n";
                if (isValorEmpty) missingFieldsMessage += "- Valor\n";
                if (isQuantidadeEmpty) missingFieldsMessage += "- Quantidade\n";

                // Exibe o alerta para o usuário
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Campos Faltando", missingFieldsMessage, "OK");
                }
            }
            else
            {
                _searchBarProdutoSelecionado.Text = "";
                // Cria um novo produto e adiciona à lista se todos os campos estiverem preenchidos
                var produto = new Product
                {
                    Codigo = cod,
                    Descricao = descricao,
                    Valor = valor,
                    Quantidade = quantidade,
                    Versao_Peca = versao_peca
                };

                // Adiciona o produto à lista de produtos selecionados
                _listaSelecionados.Add(produto);

                // Atualiza a lista de produtos selecionados na interface
                _listaProdutosSelect.ItemsSource = null; // Reset the ItemsSource
                _listaProdutosSelect.ItemsSource = _listaSelecionados; // Set the updated list

                // Atualiza o Picker se necessário
                if (_valores.SelectedItem != null)
                {
                    // Força a atualização dos dados do Picker
                    _onPickerSelectionChangedPrice(_valores, EventArgs.Empty);
                }
            }

            _addProdutosProdutosFiltradosSelecionados();
            _callValorTotal();
            _calcularFaturamento();

        }

    }
}
