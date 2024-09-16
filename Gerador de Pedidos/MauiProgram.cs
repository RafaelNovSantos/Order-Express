using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using CommunityToolkit.Maui; // Não se esqueça de importar o pacote CommunityToolkit

namespace Gerador_de_Pedidos
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
               .UseMauiApp<App>() // Inicializa o aplicativo MAUI
               .UseMauiCommunityToolkit() // Encadeia o uso do Maui Community Toolkit
               .ConfigureFonts(fonts =>
               {
                   fonts.AddFont("FontAwesome5Brands-Regular.ttf", "FontAwesome5BrandsRegular");
                   fonts.AddFont("FontAwesome5Free-Regular.ttf", "FontAwesome5FreeRegular");
                   fonts.AddFont("FontAwesome5Free-Solid.ttf", "FontAwesome5FreeSolid");
                   fonts.AddFont("Poppins-ExtraBold.ttf", "Poppins-ExtraBold");
                   fonts.AddFont("Poppins-Medium.ttf", "Poppins-Medium");
                   fonts.AddFont("Poppins-Bold.ttf", "Poppins-Bold");
                   fonts.AddFont("fontello.ttf", "IconsFont");
               });

            return builder.Build(); // Certifique-se de retornar a instância do MauiApp
        }
    }
}
