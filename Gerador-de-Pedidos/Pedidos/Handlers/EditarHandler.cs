using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerador_de_Pedidos.Pedidos.Handlers
{
    public class EditarHandler
    {
        private readonly ObservableCollection<Product> _listaSelecionados;
        private readonly CollectionView _listaProdutosSelect;
        private readonly SearchBar _searchBarProdutoSelecionado;
        private Action _addProdutosProdutosFiltradosSelecionados;
        private Action _calcularFaturamento;
        private Func<decimal> _callValorTotal;

        public EditarHandler(ObservableCollection<Product> listaSelecionados, CollectionView listaProdutosSelect, SearchBar searchBarProdutoSelecionado, Action addProdutosProdutosFiltradosSelecionados, Action calcularFaturamento, Func<decimal> callValorTotal)
        {
            _listaSelecionados = listaSelecionados;
            _listaProdutosSelect = listaProdutosSelect;
            _searchBarProdutoSelecionado = searchBarProdutoSelecionado;
            _addProdutosProdutosFiltradosSelecionados = addProdutosProdutosFiltradosSelecionados;
            _calcularFaturamento = calcularFaturamento;
            _callValorTotal = callValorTotal;
        }


        public async Task HandlerEditarClicked()
        {
            var selectedItems = _listaProdutosSelect.SelectedItems?.Cast<Product>().ToList();

            if (selectedItems == null || selectedItems.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Nenhum item selecionado.", "OK");
                return;
            }

            string action = await Application.Current.MainPage.DisplayActionSheet("Escolha o campo a editar", "Cancelar", null, "Código", "Descrição", "Valor", "Quantidade", "Versão Peça");

            if (action == "Cancelar")
                return;

            // Pega o valor do primeiro item selecionado como referência para pré-preencher o campo
            string valorAtual = action switch
            {
                "Código" => selectedItems[0].Codigo,
                "Descrição" => selectedItems[0].Descricao,
                "Valor" => selectedItems[0].Valor,
                "Quantidade" => selectedItems[0].Quantidade,
                "Versão Peça" => selectedItems[0].Versao_Peca,
                _ => ""
            };

            // Abre o prompt com o valor atual preenchido
            string newValue = await Application.Current.MainPage.DisplayPromptAsync("Editar", $"Digite o novo valor para {action}:", "OK", "Cancelar", initialValue: valorAtual);

            if (string.IsNullOrEmpty(newValue))
                return;
            var saveTextSearchBarProdutoSelecioando = _searchBarProdutoSelecionado.Text;
            _searchBarProdutoSelecionado.Text = "";
            // Atualiza todos os itens selecionados
            foreach (var item in selectedItems)
            {
                switch (action)
                {
                    case "Código":
                        item.Codigo = newValue;
                        break;
                    case "Descrição":
                        item.Descricao = newValue;
                        break;
                    case "Valor":
                        item.Valor = newValue;
                        break;
                    case "Quantidade":
                        item.Quantidade = newValue;
                        break;
                    case "Versão Peça":
                        item.Versao_Peca = newValue;
                        break;
                }
            }

            // Atualiza a interface automaticamente
            _listaProdutosSelect.ItemsSource = null;
            _listaProdutosSelect.ItemsSource = _listaSelecionados;
            _addProdutosProdutosFiltradosSelecionados();
            _callValorTotal();
            _calcularFaturamento();
            _searchBarProdutoSelecionado.Text = saveTextSearchBarProdutoSelecioando;
        }
    }
}
