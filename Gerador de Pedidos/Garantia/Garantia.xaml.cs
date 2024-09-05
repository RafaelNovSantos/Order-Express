using System.Collections.ObjectModel;
using System.Net.Http;
using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;

namespace Gerador_de_Pedidos
{
    public partial class Garantia : ContentPage
    {
        // Defina a lista como um campo da classe
        public ObservableCollection<Produto> Lista { get; set; } = new ObservableCollection<Produto>();

        private const string DefaultLinkPlanilha = "https://docs.google.com/spreadsheets/d/1AWp_sTLnWgcM7zVRR4x3zit8wbOucJ9m43s7M4yNuYU/export?usp=sharing";

        public Garantia()
        {
            InitializeComponent();
            _ = LerExcelComColuna(DefaultLinkPlanilha, "Equipamentos");
        }

        private async void EditarClicked(object sender, EventArgs e)
        {
            DownloadSheet.EditarClicked(sender, e, this);
        }

        public async Task LerExcelComColuna(string fileUrl, string sheetName)
        {
            int tentativas = 0;
            int maxTentativas = 3;

            while (tentativas < maxTentativas)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(fileUrl);
                        response.EnsureSuccessStatusCode();

                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                            using (var package = new ExcelPackage(stream))
                            {
                                var worksheet = package.Workbook.Worksheets[sheetName];

                                if (worksheet == null || worksheet.Dimension == null)
                                {
                                    await DisplayAlert("Erro", $"Planilha '{sheetName}' não encontrada ou está vazia.", "OK");
                                    return;
                                }

                                Lista.Clear();

                                var rowCount = worksheet.Dimension.Rows;
                                bool listaVazia = true;

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    var codigo = worksheet.Cells[row, 1]?.Text;
                                    var descricao = worksheet.Cells[row, 2]?.Text;

                                    if (string.IsNullOrWhiteSpace(codigo) && string.IsNullOrWhiteSpace(descricao))
                                    {
                                        continue;
                                    }

                                    var produto = new Produto
                                    {
                                        Codigo = !string.IsNullOrWhiteSpace(codigo) ? codigo : "N/A",
                                        Descricao = !string.IsNullOrWhiteSpace(descricao) ? descricao : "N/A",
                                    };

                                    Lista.Add(produto);
                                    listaVazia = false;
                                }

                                if (listaVazia)
                                {
                                    await DisplayAlert("Aviso", "Nenhum produto encontrado. Adicionando produto padrão.", "OK");
                                    CriarListaComProdutoPadrao();
                                }

                                equipamentos.ItemsSource = Lista;
                            }
                        }
                    }
                    break;
                }
                catch (HttpRequestException ex)
                {
                    tentativas++;
                    Console.WriteLine($"Erro ao acessar a planilha: {ex.Message}");

                    if (tentativas >= maxTentativas)
                    {
                        await DisplayAlert("Erro", "Falha ao carregar a planilha. Verifique sua conexão ou tente novamente mais tarde.", "OK");
                        break;
                    }

                    await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                    await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
                    break;
                }
            }
        }

        void CriarListaComProdutoPadrao()
        {
            Lista.Clear();
            Lista.Add(new Produto
            {
                Codigo = "N/A",
                Descricao = "N/A"
            });

            equipamentos.ItemsSource = Lista;
        }
    }

    public class Produto
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }
}
