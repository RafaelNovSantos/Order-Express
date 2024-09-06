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
        public List<Produtos> Lista { get; set; } = new List<Produtos>();

        private string linkplanilha;
        private string linkPadrao = "https://docs.google.com/spreadsheets/d/1AWp_sTLnWgcM7zVRR4x3zit8wbOucJ9m43s7M4yNuYU/export?usp=sharing";

        public Garantia()
        {
            InitializeComponent();

            // Carregar o link ao inicializar a página
            LoadLink();
        }

        private void EditarClicked(object sender, EventArgs e)
        {
            DownloadSheet.EditarClicked(sender, e, this);
        }

        // Função para carregar o link da planilha
        private async void LoadLink()
        {
            string fileName = "linkgarantia.txt";
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    linkplanilha = System.IO.File.ReadAllText(filePath);
                }
                else
                {
                    linkplanilha = linkPadrao;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao ler o link: {ex.Message}", "OK");
                linkplanilha = linkPadrao;
            }

            // Chama a função para carregar a planilha
            await LerExcelComColuna(linkplanilha, "Equipamentos", 1);
        }

        private async void OnAlterarLinkClicked(object sender, EventArgs e)
        {
            string senha = await DisplayPromptAsync("Autenticação", "Digite a senha para alterar o link da planilha Sheet Google:");

            if (senha == "Systelcapacitacao@1234")
            {
                string novoLink = await DisplayPromptAsync("Alterar Link", "Digite o novo link da planilha:");

                if (!string.IsNullOrEmpty(novoLink))
                {
                    string linkExportacao = ConvertToExportLink(novoLink);

                    if (linkExportacao != linkplanilha) // Verifica se o link foi alterado
                    {
                        linkplanilha = linkExportacao;

                        string fileName = "link.txt";
                        string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                        try
                        {
                            System.IO.File.WriteAllText(filePath, linkplanilha);
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Erro", $"Erro ao salvar o link: {ex.Message}", "OK");
                        }

                        await DisplayAlert("Link Atualizado", $"O link da planilha foi atualizado com sucesso para: {linkExportacao}", "OK");

                        // Recarregar a planilha com o novo link
                        await LerExcelComColuna(linkplanilha, "Equipamentos", 1);
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

        public async Task LerExcelComColuna(string fileUrl, string sheetName, int valorColumnIndex)
        {
            int tentativas = 0;
            int maxTentativas = 3;
            sheetName = "Equipamentos"; // Nome correto da planilha

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
                                    return; // Planilha vazia ou não encontrada
                                }

                                Lista.Clear(); // Limpar lista antes de adicionar novos itens

                                var rowCount = worksheet.Dimension.Rows;
                                bool listaVazia = true;

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    var codigo = worksheet.Cells[row, 1]?.Text;
                                    var descricao = worksheet.Cells[row, 2]?.Text;

                                    if (string.IsNullOrWhiteSpace(codigo) && string.IsNullOrWhiteSpace(descricao))
                                    {
                                        continue; // Ignorar linhas vazias
                                    }

                                    var produto = new Produtos
                                    {
                                        Codigo = !string.IsNullOrWhiteSpace(codigo) ? codigo : "N/A",
                                        Descricao = !string.IsNullOrWhiteSpace(descricao) ? descricao : "N/A",
                                    };

                                    Lista.Add(produto);
                                    listaVazia = false;

                                    // Debug: Mostrar no console cada produto lido
                                    Console.WriteLine($"Produto: Código = {produto.Codigo}, Descrição = {produto.Descricao}");
                                }

                                if (listaVazia)
                                {
                                    await DisplayAlert("Aviso", "Nenhum produto encontrado. Adicionando produto padrão.", "OK");
                                    CriarListaComProdutoPadrao(); // Adicionar produto padrão se a lista estiver vazia
                                }

                                // Atualizar a CollectionView com a lista de produtos
                                equipamentos.ItemsSource = null; // Limpar a origem dos itens para forçar atualização
                                equipamentos.ItemsSource = Lista;

                                // Debug: Verificar se os itens foram adicionados à CollectionView
                                Console.WriteLine($"Total de produtos carregados: {Lista.Count}");
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
                        await DisplayAlert("Erro", "Falha ao carregar a planilha. Verifique sua conexão ou tente novamente mais tarde.", "OK");
                        break;
                    }

                    await Task.Delay(5000); // Aguardar 5 segundos antes de tentar novamente
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
            Lista.Add(new Produtos
            {
                Codigo = "N/A",
                Descricao = "N/A"
            });

            equipamentos.ItemsSource = new List<Produtos>();
            equipamentos.ItemsSource = Lista;
        }
    }






    public class Produtos
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }


}


