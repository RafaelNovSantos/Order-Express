using System.Collections.ObjectModel;

namespace Gerador_de_Pedidos
{
    public partial class Garantia : ContentPage
    {
        public ObservableCollection<Item> Items { get; set; }

        public Garantia()
        {
            InitializeComponent();

            // Inicializa a coleção de itens
            Items = new ObservableCollection<Item>
            {
                new Item { Texto = "Texto 1", OutroCampo = "Valor 1" },
                new Item { Texto = "Texto 2", OutroCampo = "Valor 2" }
            };

            // Define o BindingContext para a página
            BindingContext = this;
        }

        // Evento acionado quando um item é selecionado na CollectionView
        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtém o item selecionado
            var selectedItem = e.CurrentSelection.FirstOrDefault() as Item;
            if (selectedItem != null)
            {
                // Obtém o texto do item selecionado
                string textoSelecionado = selectedItem.Texto;

                // Aqui você pode adicionar a lógica para o que deseja fazer com o texto selecionado
                DisplayAlert("Texto Selecionado", textoSelecionado, "OK");
            }
        }
    }

    // Classe que representa cada item na CollectionView
    public class Item
    {
        public string Texto { get; set; }
        public string OutroCampo { get; set; }
    }
}
