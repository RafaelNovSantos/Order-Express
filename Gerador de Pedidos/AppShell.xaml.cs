using Microsoft.Maui.Devices;

namespace Gerador_de_Pedidos
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();


            // Obter informações da tela
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            double screenWidth = displayInfo.Width / displayInfo.Density;
#if ANDROID
            this.FlyoutBehavior = FlyoutBehavior.Disabled; // Desativa o menu no Android
            
                PageHistorico.Route = "Historico.HistoricoPageAndroid";
                PageHistorico.ContentTemplate = new DataTemplate(typeof(Historico.HistoricoPageAndroid));
                MainPageAndroidContent.IsVisible = true; // Mostra MainPageAndroid
                    CurrentItem = MainPageAndroidContent; // Define como página inicial
                    PageGarantia.IsVisible = false;
#else
            PageHistorico.Route = "Historico.HistoricoPage";
            PageHistorico.ContentTemplate = new DataTemplate(typeof(Historico.HistoricoPage));
            MainPageContent.IsVisible = true; // Mostra MainPage para outras plataformas
            CurrentItem = MainPageContent; // Define como página inicial
#endif





        }
    }
}
