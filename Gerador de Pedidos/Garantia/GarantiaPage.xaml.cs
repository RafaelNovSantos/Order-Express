using System.Collections.ObjectModel;
using System.Net.Http;
using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;
using Gerador_de_Pedidos;
using Gerador_de_Pedidos.Garantia;
using Gerador_de_Pedidos.Garantia.Models;
using static OfficeOpenXml.ExcelErrorValue;
using Gerador_de_Pedidos.Garantia.Helpers;
using Microsoft.Maui;

namespace Gerador_de_Pedidos
{
    public partial class GarantiaPage : ContentPage
    {
        private string linkplanilha;
        private LinkManager linkManager = new LinkManager(); // Verifique se LinkManager est� implementado corretamente.

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

            addSelection = new AddSelection(ListaSelecionados, listaGarantiaSelect);
            EquipamentosCollectionView.SelectionChanged += addSelection.EquipamentosCollectionView_SelectionChanged;

            // Carregar o link ao inicializar a p�gina
            LoadLink();

            
        }

               

        private async void EditarClicked(object sender, EventArgs e)
        {
            // Converte a lista de selecionados para ObservableCollection
            var observableListaSelecionados = new ObservableCollection<Produtos>(ListaSelecionados);

            // Chama a fun��o EditarClicked da classe DownloadSheet, passando a ObservableCollection
            await EditClicked.EditarClicked(sender, e, this, listaGarantiaSelect, observableListaSelecionados);
        }

        private async void LoadLink()
        {
            // Carregar o link da planilha do arquivo linkgarantia.txt
            linkplanilha = linkManager.CarregarLink("linkgarantia.txt");

            // Verifique se o link foi carregado corretamente
            if (!string.IsNullOrEmpty(linkplanilha))
            {
                
                await CarregarPlanilhas();
            }
            else
            {
                await DisplayAlert("Erro", "N�o foi poss�vel carregar o link da planilha.", "OK");
            }
        }

        private async Task CarregarPlanilhas()
        {
            try
            {
                // Mostrar indicadores de carregamento
                ToggleActivityIndicators(true);

                // Ocultar frames de carregamento
                ToggleFramesVisibility(false);

                // Carregar dados para cada planilha
                var equipamentos = await ExcelHelper.LerExcelComColuna(linkplanilha, "Equipamentos", 1, 2, this);
                var diagnostico = await ExcelHelper.LerExcelComColuna(linkplanilha, "Diagnostico", 1, 2, this);
                var causa = await ExcelHelper.LerExcelComColuna(linkplanilha, "Causa", 1, 2, this);
                var solucao = await ExcelHelper.LerExcelComColuna(linkplanilha, "Solucao", 1, 2, this);

                // Limpar as cole��es antes de adicionar novos itens
                Equipamentos.Clear();
                Diagnostico.Clear();
                Causa.Clear();
                Solucao.Clear();

                // Adicionar itens �s cole��es
                foreach (var item in equipamentos)
                {
                    Equipamentos.Add(item);
                }

                foreach (var item in diagnostico)
                {
                    Diagnostico.Add(item);
                }

                foreach (var item in causa)
                {
                    Causa.Add(item);
                }

                foreach (var item in solucao)
                {
                    Solucao.Add(item);
                }

                // Atualize a UI
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
            // Certifica-se de que todos os ActivityIndicators est�o vis�veis ou invis�veis de acordo com o par�metro isVisible
            loadingIndicatorEquipamentos.IsVisible = isVisible;
            loadingIndicatorCausa.IsVisible = isVisible;
            loadingIndicatorSolucao.IsVisible = isVisible;
            loadingIndicatorDiagnostico.IsVisible = isVisible;

            // Se voc� quiser controlar a propriedade IsRunning tamb�m, adicione as linhas abaixo:
            loadingIndicatorEquipamentos.IsRunning = isVisible;
            loadingIndicatorCausa.IsRunning = isVisible;
            loadingIndicatorSolucao.IsRunning = isVisible;
            loadingIndicatorDiagnostico.IsRunning = isVisible;
        }


        // M�todo para alternar a visibilidade dos Frames
        private void ToggleFramesVisibility(bool isVisible)
        {
            disableFrameEquipamentos.IsVisible = isVisible;
            disableFrameSolucao.IsVisible = isVisible;
            disableFrameCausa.IsVisible = isVisible;
            disableFrameDiagnostico.IsVisible = isVisible;
        }


        // Fun��o para alterar o link da planilha
        private async void OnAlterarLinkClicked(object sender, EventArgs e)
        {
            string senha = await DisplayPromptAsync("Autentica��o", "Digite a senha para alterar o link da planilha:");

            if (senha == "Systelcapacitacao@1234")
            {
                string novoLink = await DisplayPromptAsync("Alterar Link", "Digite o novo link da planilha:");

                if (!string.IsNullOrEmpty(novoLink))
                {
                    string linkExportacao = ConvertToExportLink(novoLink);

                    if (linkExportacao != linkplanilha)
                    {
                        linkplanilha = linkExportacao;
                        linkManager.SalvarLink("linkgarantia.txt", linkplanilha); // Salvar o novo link no arquivo
                        await DisplayAlert("Link Atualizado", $"O link da planilha foi atualizado com sucesso para: {linkExportacao}", "OK");
                        await CarregarPlanilhas(); // Recarregar as planilhas com o novo link
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "O link informado � o mesmo j� utilizado.", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Erro", "Senha incorreta. A altera��o do link n�o foi autorizada.", "OK");
            }
        }

        // Fun��o para converter o link de edi��o para exporta��o
        private static string ConvertToExportLink(string editLink)
        {
            if (string.IsNullOrWhiteSpace(editLink))
                throw new ArgumentException("O link n�o pode ser nulo ou vazio.", nameof(editLink));

            if (editLink.Contains("/edit"))
            {
                return editLink.Replace("/edit", "/export");
            }

            return editLink;
        }

        // Placeholder para a fun��o LerExcelComColuna
        private async Task LerExcelComColuna(string link, string sheetName, string codigoColuna, string descricaoColuna)
        {
            // Implementa��o de leitura do Excel usando EPPlus ou outra biblioteca
            await Task.Run(() =>
            {
                // Simula��o de leitura de dados
            });
        }

        private void listaProdutosSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = e.CurrentSelection.Cast<Produtos>().ToList();
        }

        private void EquipamentosCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
