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
    private void AcessPlanilha()
    {

            linkPlanilhaWebView.Source = @"https://docs.google.com/spreadsheets/d/1kQdH9ON31mX1yXexz0LpEVerVcC7lbYGDkdLtkgDheI/edit?usp=sharing";
   
    }

    
}
