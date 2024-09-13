using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Gerador_de_Pedidos.Garantia.Models;
using System.Collections.ObjectModel;

namespace Gerador_de_Pedidos
{
    public class EditClicked
    {
        // A função agora é async para suportar o uso de await
        public static async Task EditarClicked(object sender, EventArgs e, ContentPage page, CollectionView listaGarantiaSelect, ObservableCollection<Produtos> ListaSelecionados)
        {
            var selectedItems = listaGarantiaSelect.SelectedItems.Cast<Produtos>().ToList();

            if (selectedItems.Count == 0)
            {
                // Usa o page para exibir o alerta
                await page.DisplayAlert("Aviso", "Nenhum item selecionado.", "OK");
                return;
            }

            // Mostra o menu de opções para o usuário
            string action = await page.DisplayActionSheet("Escolha o campo a editar", "Cancelar", null, "Código", "Descrição");

            if (action == "Cancelar")
                return;

            // Solicita o novo valor com base na escolha do usuário
            string newValue = await page.DisplayPromptAsync("Editar", $"Digite o novo valor para {action}:", "OK", "Cancelar");

            if (string.IsNullOrEmpty(newValue))
                return;

            // Atualiza os campos com o novo valor
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

                }
            }

            // Atualiza a CollectionView com os itens editados
            listaGarantiaSelect.ItemsSource = null;
            listaGarantiaSelect.ItemsSource = ListaSelecionados;
        }
    }

    
}
