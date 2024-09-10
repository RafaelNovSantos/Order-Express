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

        // Armazenar o item selecionado por tipo
        private Produtos equipamentoSelecionado;
        private Produtos diagnosticoSelecionado;
        private Produtos causaSelecionada;
        private Produtos solucaoSelecionada;

        private CollectionView EquipamentosCollectionView;
        private CollectionView DiagnosticoCollectionView;
        private CollectionView CausaCollectionView;
        private CollectionView SolucaoCollectionView;

        public AddSelection(ObservableCollection<Produtos> listaSelecionados, CollectionView listaGarantiaSelect, CollectionView EquipamentosCollectionView,
        CollectionView DiagnosticoCollectionView,
        CollectionView CausaCollectionView,
        CollectionView SolucaoCollectionView)
        {
            this.ListaSelecionados = listaSelecionados;
            this.listaGarantiaSelect = listaGarantiaSelect;
            this.EquipamentosCollectionView = EquipamentosCollectionView;
            this.DiagnosticoCollectionView = DiagnosticoCollectionView;
            this.CausaCollectionView = CausaCollectionView;
            this.SolucaoCollectionView = SolucaoCollectionView;
        }

        public void EquipamentosCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessSelectionChange(e, ref equipamentoSelecionado);



        }

        public void DiagnosticoCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessSelectionChange(e, ref diagnosticoSelecionado);
            DiagnosticoCollectionView.SelectedItem = null;
        }

        public void CausaCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessSelectionChange(e, ref causaSelecionada);
            CausaCollectionView.SelectedItem = null;
        }

        public void SolucaoCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessSelectionChange(e, ref solucaoSelecionada);
            SolucaoCollectionView.SelectedItem = null;
        }

        private void ProcessSelectionChange(SelectionChangedEventArgs e, ref Produtos itemSelecionado)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                
                itemSelecionado = e.CurrentSelection.Cast<Produtos>().FirstOrDefault();
                AtualizarLista();
            }
        }

        private void AtualizarLista()
        {
            // Limpar a lista atual
            ListaSelecionados.Clear();

            // Adicionar os itens selecionados
            AddItemIfNotNull(equipamentoSelecionado);
            AddItemIfNotNull(diagnosticoSelecionado);
            AddItemIfNotNull(causaSelecionada);
            AddItemIfNotNull(solucaoSelecionada);

            // Atualizar a CollectionView
            listaGarantiaSelect.ItemsSource = null;
            listaGarantiaSelect.ItemsSource = ListaSelecionados;
        }

        private void AddItemIfNotNull(Produtos item)
        {
            if (item != null && !ListaSelecionados.Contains(item))
            {
                ListaSelecionados.Add(item);
            }
        }
    }
}
