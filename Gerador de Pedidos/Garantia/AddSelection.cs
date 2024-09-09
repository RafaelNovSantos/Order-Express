using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Gerador_de_Pedidos.Garantia.Models;
using System.Linq;

namespace Gerador_de_Pedidos.Garantia
{
    public class AddSelection
    {
        private ObservableCollection<Produtos> ListaSelecionados;
        private CollectionView listaGarantiaSelect;

        public AddSelection(ObservableCollection<Produtos> listaSelecionados, CollectionView listaGarantiaSelect)
        {
            this.ListaSelecionados = listaSelecionados;
            this.listaGarantiaSelect = listaGarantiaSelect;
        }

        public void EquipamentosCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                var selectedEquipamentos = e.CurrentSelection.Cast<Produtos>().ToList();

                foreach (var item in selectedEquipamentos)
                {
                    if (!ListaSelecionados.Contains(item))
                    {
                        ListaSelecionados.Add(item);
                    }
                }

                listaGarantiaSelect.ItemsSource = null;
                listaGarantiaSelect.ItemsSource = ListaSelecionados;
            }
        }
    }
}
