using System.Collections.ObjectModel;
using System.Net.Http;
using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;
using static OfficeOpenXml.ExcelErrorValue;

namespace Gerador_de_Pedidos
{
    public partial class Garantia : ContentPage
    {
        // Defina a lista como um campo da classe
        public List<Equipamentos> Lista { get; set; } = new List<Equipamentos>();

        private string linkplanilha;

        public Garantia()
        {
            InitializeComponent();

            LoadLink();
        }

        private void EditarClicked(object sender, EventArgs e)
        {
            DownloadSheet.EditarClicked(sender, e, this);
        }


        private async void LoadLink()
        {
            string fileName = "link.txt";
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    linkplanilha = System.IO.File.ReadAllText(filePath);
                    
                }
                else
                {
                    linkplanilha = "https://docs.google.com/spreadsheets/d/197z0M4GrqaY3Kl6BvtEgGt-tYMX4IdpcW2dm8Ze5bZQ/export?usp=sharing";
                    
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao ler o link: {ex.Message}", "OK");
                linkplanilha = "https://docs.google.com/spreadsheets/d/197z0M4GrqaY3Kl6BvtEgGt-tYMX4IdpcW2dm8Ze5bZQ/export?usp=sharing";
               

            }
        }

        async Task LerExcelComColuna(string fileUrl, string sheetName, int valorColumnIndex)
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

                                if (worksheet == null || worksheet.Dimension == null || worksheet.Cells == null)
                                {
                                    return;
                                }

                                Lista.Clear();  // Limpe a lista antes de adicionar novos itens

                                var rowCount = worksheet.Dimension.Rows;
                                bool linhaVazia = true;

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    var codigo = worksheet.Cells[row, 1]?.Text;
                                    var descricao = worksheet.Cells[row, 2]?.Text;


                                    if (string.IsNullOrWhiteSpace(codigo) &&
                                        string.IsNullOrWhiteSpace(descricao))
                                    {
                                        continue; // Ignorar linhas vazias
                                    }

                                    var produto = new Equipamentos
                                    {
                                        Codigo = !string.IsNullOrWhiteSpace(codigo) ? codigo : "N/A",
                                        Descricao = !string.IsNullOrWhiteSpace(descricao) ? descricao : "N/A",
                                    };

                                    Lista.Add(produto);
                                    linhaVazia = false;
                                }

                                if (linhaVazia)
                                {
                                    CriarListaComProdutoPadrao();
                                }

                                // Atualizar a visualização da lista
                                equipamentos.ItemsSource = null; // Limpar a origem de itens para forçar a atualização
                                equipamentos.ItemsSource = Lista;
                            }
                        }
                    }

                    break; // Saia do loop se a operação foi bem-sucedida
                }
                catch (HttpRequestException ex)
                {
                    tentativas++;
                    Console.WriteLine($"Erro ao acessar a planilha: {ex.Message}");

                    if (tentativas >= maxTentativas)
                    {
                        break;
                    }

                    await Task.Delay(5000); // Aguardar 5 segundos antes de tentar novamente
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                    break;
                }
            }
        }
        void CriarListaComProdutoPadrao()
        {
            Lista.Clear();
            Lista.Add(new Equipamentos
            {
                Codigo = "N/A",
                Descricao = "N/A"
            });

            equipamentos.ItemsSource = new List<Equipamentos>();
            equipamentos.ItemsSource = Lista;
        }
    }





    public class Equipamentos
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }

    public class Diagnostico
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }

    public class Causa
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
    }

    public class Solucao
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
    }
}
