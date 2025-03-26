using Gerador_de_Pedidos;
using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using OfficeOpenXml;
using System.Diagnostics;
using System.Collections.ObjectModel;


public class LinkService
{
    private readonly ActivityIndicator _loadingIndicatorPedido;
    private readonly Label _lblStatusProduto;

    public LinkService() { 
    }

    public LinkService(ActivityIndicator loadingIndicatorPedido, Label lblStatusProduto)
    {
        _loadingIndicatorPedido = loadingIndicatorPedido;
        _lblStatusProduto = lblStatusProduto;

    }

    public string LinkPlanilha { get; set; } = "https://docs.google.com/spreadsheets/d/1tF_sKR6Mne3H1HPSuz9G2rlHahqnTmX_KaxUuHV6qBw/export?usp=sharing";
    public async Task<string> LoadLink( Picker pedidoPicker, Picker equipamentosPicker, Picker pagPicker, Picker notaPicker, Picker TipoFretePicker, Picker valoresPicker, EventHandler OnPickerSelectionChangedPrice)
    {
        // Cria a conexão usando o banco de dados assíncrono
        var connection = App.Database.GetConnection();
        await App.Database.ObterPlanilhaAsync();
        // Consulta para pegar o último LinkPlanilha
        var planilha = await connection.Table<Planilha>()
 .OrderByDescending(p => p.DataMudanca) // Ordena pela data mais recente
 .FirstOrDefaultAsync();
        // Verifica se não há nenhum link no banco, caso não tenha, define o link padrão
        try
        {
            if (planilha == null || string.IsNullOrEmpty(planilha.LinkPlanilha))
            {
                LinkPlanilha = "https://docs.google.com/spreadsheets/d/1tF_sKR6Mne3H1HPSuz9G2rlHahqnTmX_KaxUuHV6qBw/export?usp=sharing";
            }
            else
            {
                LinkPlanilha = planilha.LinkPlanilha;
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao ler o link: {ex.Message}", "OK");
            LinkPlanilha = "https://docs.google.com/spreadsheets/d/1tF_sKR6Mne3H1HPSuz9G2rlHahqnTmX_KaxUuHV6qBw/export?usp=sharing";

        }

        pedidoPicker.SelectedIndex = 0;
        equipamentosPicker.SelectedIndex = 0;
        pagPicker.SelectedIndex = 0;
        notaPicker.SelectedIndex = 0;
        TipoFretePicker.SelectedIndex = 0;
        valoresPicker.SelectedIndex = 0;
        OnPickerSelectionChangedPrice(valoresPicker, EventArgs.Empty);
        return LinkPlanilha;
    }
    public async Task AlterarLink( ActivityIndicator loadingIndicatorPedido)
    {
        string senha = await Application.Current.MainPage.DisplayPromptAsync("Autenticação", "Digite a senha para alterar o link da planilha Sheet Google:");

        

        if (senha == "Systelcapacitacao@1234")
        {

            string novoLink = await Application.Current.MainPage.DisplayPromptAsync("Alterar Link", "Digite o novo link da planilha:");


            if (!string.IsNullOrEmpty(novoLink))
            {
                string linkExportacao = ConvertToExportLink(novoLink);
                var adicionarnovoLink = new Planilha
                {
                    Modelo = "Pedido",
                    LinkPlanilha = linkExportacao,
                    DataMudanca = DateTime.Now
                };
                await App.Database.SalvarPlanilhaAsync(adicionarnovoLink);
               
                try
                {
                    
                    await Application.Current.MainPage.DisplayAlert("Link Atualizado", $"O link da planilha foi atualizado com sucesso para: {linkExportacao}", "OK");
                    
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao salvar o link: {ex.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "O novo link não pode ser vazio.", "OK");
            }
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Erro", "Senha incorreta. A alteração do link não foi autorizada.", "OK");
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

    public async Task LerExcelComColuna(string fileUrl, string sheetName, int valorColumnIndex, List<Product> Lista, CollectionView listaProdutosExcel, Frame disableFrame, ActivityIndicator loadingIndicator)
    {
        // Mostrar o indicador de carregamento e desativar o ScrollView
        disableFrame.IsVisible = false;
        loadingIndicator.IsRunning = true;
        loadingIndicator.IsVisible = true;
        listaProdutosExcel.IsVisible = false;
        int tentativas = 0;
        int maxTentativas = 3;
        HttpResponseMessage response = null;  // Declare a variável 'response' fora do bloco de try-catch

        while (tentativas < maxTentativas)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                   
                    response = await client.GetAsync(fileUrl);  // Use a variável 'response' já declarada
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[sheetName];

                            if (worksheet == null || worksheet.Dimension == null || worksheet.Cells == null)
                            {
                                await Application.Current.MainPage.DisplayAlert("Erro", $"Página '{sheetName}' da planilha não encontrada ou as células da planilha estão vazias", "OK");
                                CriarListaComProdutoPadrao(Lista, listaProdutosExcel);
                                return;
                            }
                            Lista.Clear();

                            var rowCount = worksheet.Dimension.Rows;
                            bool linhaVazia = true;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var codigo = worksheet.Cells[row, 1]?.Text;
                                var descricao = worksheet.Cells[row, 2]?.Text;
                                var valor = worksheet.Cells[row, valorColumnIndex]?.Text;
                                if (string.IsNullOrWhiteSpace(codigo) &&
                                    string.IsNullOrWhiteSpace(descricao) &&
                                    string.IsNullOrWhiteSpace(valor))
                                {
                                    continue; // Ignorar linhas vazias
                                }

                                var produto = new Product
                                {
                                    Codigo = !string.IsNullOrWhiteSpace(codigo) ? codigo : "N/A",
                                    Descricao = !string.IsNullOrWhiteSpace(descricao) ? descricao : "N/A",
                                    Valor = !string.IsNullOrWhiteSpace(valor) ? valor : "N/A"
                                };

                                Lista.Add(produto);
                                linhaVazia = false;
                            }

                            if (linhaVazia)
                            {
                                CriarListaComProdutoPadrao(Lista, listaProdutosExcel);
                            }

                            // Atualizar a visualização da lista
                            listaProdutosExcel.ItemsSource = null; // Limpar a origem de itens para forçar a atualização
                            listaProdutosExcel.ItemsSource = Lista;
                        }
                    }
                }

                break; // Saia do loop se a operação foi bem-sucedida
            }
            catch (HttpRequestException ex)
            {
                tentativas++;
                Debug.WriteLine($"Erro ao acessar a planilha: {ex.Message}");

                if (tentativas >= maxTentativas)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Falha ao acessar a planilha após várias tentativas. Erro: " + ex.Message, "OK");
                    CriarListaComProdutoPadrao(Lista, listaProdutosExcel);
                    break;
                }

                await Task.Delay(5000); // Aguardar 5 segundos antes de tentar novamente
            }
            catch (Exception ex)
            {
                // Tentativa de reconectar ou manipular erro
                for (int tentativa = 0; tentativa < 3; tentativa++)
                {
                    try
                    {
                        if (response != null) // Verifique se a resposta foi obtida
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {
                                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                                using (var package = new ExcelPackage(stream))
                                {
                                    var worksheet = package.Workbook.Worksheets[sheetName];

                                    if (worksheet == null || worksheet.Dimension == null)
                                    {
                                        throw new Exception("A planilha não foi carregada corretamente.");
                                    }

                                    break; // Sai do loop se der certo
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        await Task.Delay(3000); // Espera 3 segundos antes de tentar de novo
                    }
                }

                Debug.WriteLine($"Erro inesperado: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erro", "Ocorreu um erro inesperado: " + ex.Message, "OK");
                CriarListaComProdutoPadrao(Lista, listaProdutosExcel);
                break;
            }
        }

        disableFrame.IsVisible = true;
        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
        listaProdutosExcel.IsVisible = true;
    }

    public int ExecuteTaskService(string selectedValue)
    {
        int columnToUse;
        // Definir a coluna correta com base na seleção do Picker
        switch (selectedValue)
        {
            case "Valor ATA":
                columnToUse = 3; // Ajuste com base na coluna correta
                break;
            case "Valor Oficina":

                columnToUse = 4; // Ajuste com base na coluna correta
                break;
            case "Valor Cliente Final":

                columnToUse = 5; // Ajuste com base na coluna correta
                break;
            default:

                _loadingIndicatorPedido.IsVisible = false; // Ocultar o botão se houve erro
                _loadingIndicatorPedido.IsRunning = false;
                _lblStatusProduto.IsVisible = true; // Mostrar o texto de status se houve erro
                _lblStatusProduto.Text = "Tipo de valor \nnão selecionado.";
                _lblStatusProduto.LineBreakMode = LineBreakMode.WordWrap; // Para quebra de linha automática

                _lblStatusProduto.TextColor = Color.FromArgb("#FF0000"); // Vermelho para indicar erro
                return 3;
        }

        return columnToUse;


    }

    void CriarListaComProdutoPadrao(List<Product> Lista, CollectionView listaProdutosExcel)
    {
        Lista.Clear();
        Lista.Add(new Product
        {
            Codigo = "N/A",
            Descricao = "N/A",
            Valor = "N/A",
            Quantidade = "N/A"
        });

        listaProdutosExcel.ItemsSource = new List<Product>();
        listaProdutosExcel.ItemsSource = Lista;
    }
}
