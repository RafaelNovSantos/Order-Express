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
        public string linkplanilha;
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

            var fileSearch = new FileSearch();

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

            // Inicializar todos os indicadores de carregamento e desabilitar os frames
            ToggleLoading(loadingIndicatorEquipamentos, true);
            ToggleLoading(loadingIndicatorDiagnostico, true);
            ToggleLoading(loadingIndicatorCausa, true);
            ToggleLoading(loadingIndicatorSolucao, true);

            ToggleFrame(disableFrameEquipamentos, false);
            ToggleFrame(disableFrameDiagnostico, false);
            ToggleFrame(disableFrameCausa, false);
            ToggleFrame(disableFrameSolucao, false);


            // Carregar planilha de Equipamentos
            await CarregarPlanilha("Equipamentos", Equipamentos, loadingIndicatorEquipamentos, disableFrameEquipamentos);

            // Carregar planilha de Diagnostico
            await CarregarPlanilha("Diagnostico", Diagnostico, loadingIndicatorDiagnostico, disableFrameDiagnostico);

            // Carregar planilha de Causa
            await CarregarPlanilha("Causa", Causa, loadingIndicatorCausa, disableFrameCausa);

            // Carregar planilha de Solucao
            await CarregarPlanilha("Solucao", Solucao, loadingIndicatorSolucao, disableFrameSolucao);
        }

        private async Task CarregarPlanilha(string sheetName, ObservableCollection<Produtos> listaProdutos, ActivityIndicator loadingIndicator, Frame disableFrame)
        {
            ToggleLoading(loadingIndicator, true);
            ToggleFrame(disableFrame, false);

            listaProdutos.Clear();
            var produtos = await ExcelHelper.LerExcelComColuna(linkplanilha, sheetName, 1, 2, this, this);
            foreach (var item in produtos) listaProdutos.Add(item);

            ToggleLoading(loadingIndicator, false);
            ToggleFrame(disableFrame, true);
            
        }

        private void ToggleLoading(ActivityIndicator indicator, bool isVisible)
        {
            indicator.IsVisible = isVisible;
            indicator.IsRunning = isVisible;
        }

        private void ToggleFrame(Frame frame, bool isVisible)
        {
            frame.IsVisible = isVisible;
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
            try {
                Lista.Clear();
                
                var fileSearch = new FileSearch();
            await fileSearch.SelectAndSearchPdfAsync(this); // Chamada de método em uma instância
                
            }
            catch(Exception ex) 
            {
                DisplayAlert("Erro", $"{ex}", "Ok");
            }
        }



        private async void ExcluirSelect(object sender, EventArgs e)
        {
            await ExcluirClicked.ExcluirSelectAsync(listaGarantiaSelect, ListaSelecionados);

        }

        private async void ExcluirSelectFile(object sender, EventArgs e)
        {
            await ExcluirClicked.ExcluirSelectFileAsync(listaGarantiaSelectFile, Lista);

        }

        private void listaProdutosSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = e.CurrentSelection.Cast<Produtos>().ToList();
        }

        

        private void listaProdutosSelectFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItemsFile = e.CurrentSelection.Cast<BaseItem>().ToList();
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


        private void Button_Clicked(object sender, EventArgs e)
        {
            LoadLink();
        }

        public List<BaseItem> Lista { get; set; } = new List<BaseItem>();

        public void AddProdutoBase(FileSearch fileSearch)
        {
            try
            {
                Lista.Add(new ProdutosFile
                {
                    Codigo = fileSearch.Codigo,
                    Descricao = fileSearch.Descricao.Replace("-", "").Trim(),
                    Unidade = fileSearch.Unidade
                });

                listaGarantiaSelectFile.ItemsSource = new List<BaseItem>();
                listaGarantiaSelectFile.ItemsSource = Lista;
            }
            catch (Exception ex)
            {
                DisplayAlert("Erro", $"{ex}", "Ok");
            }
        }

        public void AddProdutoDetalhes(FileSearch fileSearch)
        {

            Lista.Add(new VendedorItem
            {
                Vendedor = fileSearch.Vendedor
            });

            Lista.Add(new DataItem
            {
                Data = fileSearch.Data
            });

            if (fileSearch.NomeFantasia == "-") { 

                Lista.Add(new RazaoSocialItem
                {
                    RazaoSocial = fileSearch.RazaoSocial
                });
            }
            else
                {
                Lista.Add(new NomeFantasiaItem
                {
                    NomeFantasia = fileSearch.NomeFantasia
                });

            }
            


            Lista.Add(new CNPJItem
            {
                CNPJ = fileSearch.CNPJ
            });

            Lista.Add(new TelefoneItem
            {
                Telefone = fileSearch.Telefone
            });

            Lista.Add(new SerieItem
            {
                Serie = fileSearch.Serie
            });

            listaGarantiaSelectFile.ItemsSource = new List<BaseItem>();
            listaGarantiaSelectFile.ItemsSource = Lista;
        }

        public async void btncopyFile_Clicked_1(object sender, EventArgs e)
        {
            FileSearch fileSearch = new FileSearch();
            var texto = "";

          

            if (FileSearch.Dicionario.ContainsKey("Data"))
            {
                // Adiciona o valor associado à chave "Serie" ao texto
                texto += $"{FileSearch.Dicionario["Data"]}\t";
                texto += $"{FileSearch.Dicionario["Data"]}\t\t";
                texto += $"SYSTEL\t";
            }

            if (FileSearch.Dicionario.ContainsKey("Vendedor"))
            {
                // Adiciona o valor associado à chave "Serie" ao texto
                texto += $"{FileSearch.Dicionario["Vendedor"]}\t";
            }


            if (FileSearch.Dicionario.ContainsKey("NomeFantasia"))
            {
                // Verifica se o valor associado a "NomeFantasia" é igual a "-"
                if (FileSearch.Dicionario["NomeFantasia"] == "-")
                {
                    // Se "NomeFantasia" for "-", adiciona "RazaoSocial" ao texto
                    texto += $"{FileSearch.Dicionario["RazaoSocial"]}\t";
                }
                else
                {
                    // Caso contrário, adiciona "NomeFantasia" ao texto
                    texto += $"{FileSearch.Dicionario["NomeFantasia"]}\t";
                }
            }



            if (FileSearch.Dicionario.ContainsKey("Telefone"))
            {
                // Adiciona o valor associado à chave "Serie" ao texto
                texto += $"'{FileSearch.Dicionario["Telefone"]}\t";
                texto += $"'+55 11 97453-6688\t";
            }


            // Verifica se listaGarantiaSelect.ItemsSource não é nulo e contém itens
            if (listaGarantiaSelect.ItemsSource != null && listaGarantiaSelect.ItemsSource.Cast<Produtos>().Any())
            {
                var count = 0;

                foreach (var product in listaGarantiaSelect.ItemsSource.Cast<Produtos>())
                {
                    // Adiciona informações do produto ao texto
                    texto += $" {product.Codigo}\t{product.Descricao}\t";

                    // Na primeira iteração, adiciona também os itens do dicionário
                    if (count == 0)
                    {

                        // Verifica se a chave "Serie" está presente no dicionário
                        if (FileSearch.Dicionario.ContainsKey("Serie"))
                        {
                            // Adiciona o valor associado à chave "Serie" ao texto
                            texto += $"{FileSearch.Dicionario["Serie"]}\t";
                        }

                        else
                        {
                            await DisplayAlert("Erro", "O dicionário não contém dados.", "OK");
                        }
                    }

                    count++;
                }

                count = 0;

                // Aqui você pode copiar o texto para a área de transferência ou exibir conforme necessário
                await Clipboard.SetTextAsync(texto);
                btncopyFile.Text = "Copiado!";
                iconCopyFile.Color = Color.FromHex("#000000");
                await Task.Delay(5000);
                btncopyFile.Text = "Copiar";
                iconCopyFile.Color = Color.FromHex("#FF008000");
            }
            else
            {
                await DisplayAlert("Atenção!", "Adicione as informações de Equipamento, Diagnóstico, Causa e Solução", "OK");
                return; // Retorna para que o código de cópia não seja executado
            }


            foreach (var item in FileSearch.ProdutosDicionario)
            {
                texto += $"{item.Value}";
            }

            Clipboard.SetTextAsync(texto);
        }

    }
}
