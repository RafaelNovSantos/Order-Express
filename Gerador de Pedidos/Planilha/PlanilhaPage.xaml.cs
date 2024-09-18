using CommunityToolkit.Maui.Core.Platform;
using Gerador_de_Pedidos.Garantia;

namespace Gerador_de_Pedidos;

public partial class PlanilhaPage : ContentPage
{
    public PlanilhaPage()
    {
        InitializeComponent();
        
        // Criar uma inst�ncia da GarantiaPage para acessar o linkplanilha


        // Chamar o m�todo para configurar a WebView com o linkplanilha
        AcessPlanilha();
    }

    // M�todo para configurar a WebView com o link da planilha da GarantiaPage
    public async void AcessPlanilha()
    {
        string fileName = "LinkPlanilhaWebView.txt";
        string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        try
        {
            if (System.IO.File.Exists(filePath))
            {
                linkPlanilhaWebView.Source = System.IO.File.ReadAllText(filePath);
               if(System.IO.File.ReadAllText(filePath) == "")
                {
                    linkPlanilhaWebView.Source = @"https://docs.google.com/spreadsheets/d/1kQdH9ON31mX1yXexz0LpEVerVcC7lbYGDkdLtkgDheI/edit?usp=sharing";
                }
            }
            else
            {
                linkPlanilhaWebView.Source = @"https://docs.google.com/spreadsheets/d/1kQdH9ON31mX1yXexz0LpEVerVcC7lbYGDkdLtkgDheI/edit?usp=sharing";

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao ler o link: {ex.Message}", "OK");
            linkPlanilhaWebView.Source = @"https://docs.google.com/spreadsheets/d/1kQdH9ON31mX1yXexz0LpEVerVcC7lbYGDkdLtkgDheI/edit?usp=sharing";
            

        }

    }

    // Evento chamado quando o WebView come�a a navegar
    private void OnNavigating(object sender, WebNavigatingEventArgs e)
    {
        progressBar.IsVisible = true;
        progressBar.Progress = 0;
        _ = SimulateProgress(); // Inicia a simula��o de progresso
    }

    // Evento chamado quando o WebView termina a navega��o
    private void OnNavigated(object sender, WebNavigatedEventArgs e)
    {
        // Quando a navega��o � conclu�da, define o progresso como 100% e oculta a barra
        progressBar.ProgressTo(1, 500, Easing.Linear).ContinueWith(_ =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                progressBar.IsVisible = false;
            });
        });
    }

    // Simula o progresso at� um limite m�ximo (por exemplo, 80%)
    private async Task SimulateProgress()
    {
        double maxProgress = 0.8;
        while (progressBar.Progress < maxProgress)
        {
            await Task.Delay(10); // Intervalo de 200ms entre atualiza��es
            await progressBar.ProgressTo(progressBar.Progress + 0.1, 200, Easing.Linear);
        }
    }

    public void OnBackClicked(object sender, EventArgs e)
    {
        if (linkPlanilhaWebView.CanGoBack)
        {
            linkPlanilhaWebView.GoBack();
        }
    }

    public void OnReloadClicked(object sender, EventArgs e)
    {
        linkPlanilhaWebView.Reload();
    }



    public async void OnLinkChangedClicked(object sender, EventArgs e)
    {
        string senha = await DisplayPromptAsync("Autentica��o", "Digite a senha para alterar o link da planilha Sheet Google:");

        if (senha == "Systelcapacitacao@1234")
        {
            string novoLink = await DisplayPromptAsync("Alterar Link", "Digite o novo link da planilha:");

            if (!string.IsNullOrEmpty(novoLink))
            {
                linkPlanilhaWebView.Source = novoLink;

                string fileName = "LinkPlanilhaWebView.txt";
                string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                try
                {
                    // Extraindo a URL do WebView como string
                    string currentLink = ((UrlWebViewSource)linkPlanilhaWebView.Source).Url;
                    System.IO.File.WriteAllText(filePath, currentLink);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Erro ao salvar o link: {ex.Message}", "OK");
                }

                await DisplayAlert("Link Atualizado", $"O link da planilha foi atualizado com sucesso para: {novoLink}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Erro", "Senha incorreta. A altera��o do link n�o foi autorizada.", "OK");
        }
    }



    public void OnForwardClicked(object sender, EventArgs e)
    {
        if (linkPlanilhaWebView.CanGoForward)
        {
            linkPlanilhaWebView.GoForward();
        }
    }

}
