namespace Gerador_de_Pedidos;

public partial class Garantia : ContentPage
{
	public Garantia()
	{
		InitializeComponent();

        // Exemplo de dados
        var items = new List<Item>
        {
            new Item { ItemName = "Item 1" },
            new Item { ItemName = "Item 2" }
        };

        // Atribuindo os dados ao CollectionView
        itemsCollectionView.ItemsSource = items;
    }


public class Item
{
    public string ItemName { get; set; }
}

private async void OnLabelTapped(object sender, EventArgs e)
    {
        if (sender is Label label)
        {
            // Copia o texto do label para a �rea de transfer�ncia

            await DisplayAlert("Texto Copiado", "O texto foi copiado para a �rea de transfer�ncia.", "OK");
        }
    }
}