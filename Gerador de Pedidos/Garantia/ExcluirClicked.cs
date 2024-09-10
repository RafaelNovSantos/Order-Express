using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Gerador_de_Pedidos.Garantia.Models;
using Microsoft.Maui.Controls; // Atualize para o namespace correto

public class ExcluirClicked
{
    public static async Task ExcluirSelectAsync(CollectionView collectionView, ObservableCollection<Produtos> listaSelecionados)
    {
        var selectedItems = collectionView.SelectedItems.Cast<Produtos>().ToList();

        if (selectedItems.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("Aviso", "Nenhum item selecionado para excluir.", "OK");
            return;
        }

        // Confirmação para excluir os itens selecionados
        bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmação", $"Deseja realmente excluir {selectedItems.Count} item(s) selecionado(s)?", "Sim", "Não");
        if (!confirm)
            return;

        foreach (var item in selectedItems)
        {
            listaSelecionados.Remove(item);

        }

        // Atualiza a CollectionView e limpa a seleção
        collectionView.SelectedItems.Clear(); // Limpa a seleção
        collectionView.ItemsSource = null;
        collectionView.ItemsSource = listaSelecionados;

    }
}
