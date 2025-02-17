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
#endif
            // Configurar ShellContent com base na plataforma e tamanho
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                
                    MainPageAndroidContent.IsVisible = true; // Mostra MainPageAndroid
                    CurrentItem = MainPageAndroidContent; // Define como página inicial
                    PageGarantia.IsVisible = false;
               
            }
            else
            {
                MainPageAndroidContent.IsVisible = true; // Mostra MainPage para outras plataformas
                CurrentItem = MainPageAndroidContent; // Define como página inicial
            }
        }
    }
}
