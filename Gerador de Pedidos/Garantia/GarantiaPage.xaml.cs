using System.Collections.ObjectModel;
using System.Net.Http;
using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui.Storage;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Gerador_de_Pedidos;
using Gerador_de_Pedidos.Garantia;
using Gerador_de_Pedidos.Garantia.Models;
using static OfficeOpenXml.ExcelErrorValue;
using Gerador_de_Pedidos.Garantia.Helpers;
using Microsoft.Maui;
using System.Diagnostics;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Gerador_de_Pedidos
{
    public partial class GarantiaPage : ContentPage
    {
        private string linkplanilha;
        private LinkManager linkManager = new LinkManager(); // Verifique se LinkManager está implementado corretamente.

        private ObservableCollection<Produtos> ListaSelecionados = new ObservableCollection<Produtos>();
        private AddSelection addSelection;

        public ObservableCollection<Produtos> Equipamentos { get; set; } = new ObservableCollection<Produtos>();
        public ObservableCollection<Produtos> Diagnostico { get; set; } = new ObservableCollection<Produtos>();
        public ObservableCollection<Produtos> Causa { get; set; } = new ObservableCollection<Produtos>();
        public ObservableCollection<Produtos> Solucao { get; set; } = new ObservableCollection<Produtos>();

        public GarantiaPage()
        {
            InitializeComponent();

            EquipamentosCollectionView.ItemsSource = Equipamentos;
            DiagnosticoCollectionView.ItemsSource = Diagnostico;
            CausaCollectionView.ItemsSource = Causa;
            SolucaoCollectionView.ItemsSource = Solucao;




            addSelection = new AddSelection(ListaSelecionados, listaGarantiaSelect, EquipamentosCollectionView,
            DiagnosticoCollectionView,
            CausaCollectionView,
            SolucaoCollectionView );

            EquipamentosCollectionView.SelectionChanged += addSelection.EquipamentosCollectionView_SelectionChanged;
            DiagnosticoCollectionView.SelectionChanged += addSelection.DiagnosticoCollectionView_SelectionChanged;
            CausaCollectionView.SelectionChanged += addSelection.CausaCollectionView_SelectionChanged;
            SolucaoCollectionView.SelectionChanged += addSelection.SolucaoCollectionView_SelectionChanged;

            // Carregar o link ao inicializar a página
            LoadLink();
        }

        private async void EditarClicked(object sender, EventArgs e)
        {
            var observableListaSelecionados = new ObservableCollection<Produtos>(ListaSelecionados);
            await EditClicked.EditarClicked(sender, e, this, listaGarantiaSelect, observableListaSelecionados);
        }

        public async void LoadLink()
        {
            linkplanilha = linkManager.CarregarLink("linkgarantia.txt");

            if (!string.IsNullOrEmpty(linkplanilha))
            {
                await CarregarPlanilhas();
            }
            else
            {
                await DisplayAlert("Erro", "Não foi possível carregar o link da planilha.", "OK");
            }
        }

        public async Task CarregarPlanilhas()
        {
            try
            {
                ToggleActivityIndicators(true);
                ToggleFramesVisibility(false);

                var equipamentos = await ExcelHelper.LerExcelComColuna(linkplanilha, "Equipamentos", 1, 2, this);
                var diagnostico = await ExcelHelper.LerExcelComColuna(linkplanilha, "Diagnostico", 1, 2, this);
                var causa = await ExcelHelper.LerExcelComColuna(linkplanilha, "Causa", 1, 2, this);
                var solucao = await ExcelHelper.LerExcelComColuna(linkplanilha, "Solucao", 1, 2, this);

                Equipamentos.Clear();
                Diagnostico.Clear();
                Causa.Clear();
                Solucao.Clear();

                foreach (var item in equipamentos) Equipamentos.Add(item);
                foreach (var item in diagnostico) Diagnostico.Add(item);
                foreach (var item in causa) Causa.Add(item);
                foreach (var item in solucao) Solucao.Add(item);

                ToggleActivityIndicators(false);
                ToggleFramesVisibility(true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar as planilhas: {ex.Message}", "OK");
            }
        }

        private void ToggleActivityIndicators(bool isVisible)
        {
            loadingIndicatorEquipamentos.IsVisible = isVisible;
            loadingIndicatorCausa.IsVisible = isVisible;
            loadingIndicatorSolucao.IsVisible = isVisible;
            loadingIndicatorDiagnostico.IsVisible = isVisible;
            loadingIndicatorEquipamentos.IsRunning = isVisible;
            loadingIndicatorCausa.IsRunning = isVisible;
            loadingIndicatorSolucao.IsRunning = isVisible;
            loadingIndicatorDiagnostico.IsRunning = isVisible;
        }

        private void ToggleFramesVisibility(bool isVisible)
        {
            disableFrameEquipamentos.IsVisible = isVisible;
            disableFrameSolucao.IsVisible = isVisible;
            disableFrameCausa.IsVisible = isVisible;
            disableFrameDiagnostico.IsVisible = isVisible;
        }

        private async void OnAlterarLinkClicked(object sender, EventArgs e)
        {
            string senha = await DisplayPromptAsync("Autenticação", "Digite a senha para alterar o link da planilha:");

            if (senha == "Systelcapacitacao@1234")
            {
                string novoLink = await DisplayPromptAsync("Alterar Link", "Digite o novo link da planilha:");

                if (!string.IsNullOrEmpty(novoLink))
                {
                    string linkExportacao = ConvertToExportLink(novoLink);

                    if (linkExportacao != linkplanilha)
                    {
                        linkplanilha = linkExportacao;
                        linkManager.SalvarLink("linkgarantia.txt", linkplanilha);
                        await DisplayAlert("Link Atualizado", $"O link da planilha foi atualizado com sucesso para: {linkExportacao}", "OK");
                        await CarregarPlanilhas();
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "O link informado é o mesmo já utilizado.", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Erro", "Senha incorreta. A alteração do link não foi autorizada.", "OK");
            }
        }

        private static string ConvertToExportLink(string editLink)
        {
            if (string.IsNullOrWhiteSpace(editLink))
                throw new ArgumentException("O link não pode ser nulo ou vazio.", nameof(editLink));

            if (editLink.Contains("/edit"))
            {
                return editLink.Replace("/edit", "/export");
            }

            return editLink;
        }

        private async void FileSearchButton(object sender, EventArgs e)
        {
            await FileSearch.SelectAndSearchPdfAsync();
        }

        private async void ExcluirSelect(object sender, EventArgs e)
        {
            await ExcluirClicked.ExcluirSelectAsync(listaGarantiaSelect, ListaSelecionados);

        }

        private void listaProdutosSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = e.CurrentSelection.Cast<Produtos>().ToList();
        }

        private void EquipamentosCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void DiagnosticoCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CausaCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void SolucaoCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
