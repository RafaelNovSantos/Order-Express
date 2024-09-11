using Gerador_de_Pedidos.Garantia;

namespace Gerador_de_Pedidos;

public partial class PlanilhaPage : ContentPage
{
    public PlanilhaPage()
    {
        InitializeComponent();
        
        // Criar uma instância da GarantiaPage para acessar o linkplanilha


        // Chamar o método para configurar a WebView com o linkplanilha
        AcessPlanilha();
    }

    // Método para configurar a WebView com o link da planilha da GarantiaPage
    private void AcessPlanilha()
    {

            linkPlanilhaWebView.Source = @"https://docs.google.com/spreadsheets/d/1kQdH9ON31mX1yXexz0LpEVerVcC7lbYGDkdLtkgDheI/edit?usp=sharing";
   
    }

    
}
