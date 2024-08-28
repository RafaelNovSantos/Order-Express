using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;

namespace Gerador_de_Pedidos
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
               .UseMauiApp<App>()
               .ConfigureFonts(fonts =>
               {
                   fonts.AddFont("FontAwesome5Brands-Regular.ttf", "FontAwesome5BrandsRegular");
                   fonts.AddFont("FontAwesome5Free-Regular.ttf", "FontAwesome5FreeRegular");
                   fonts.AddFont("FontAwesome5Free-Solid.ttf", "FontAwesome5FreeSolid");
                   fonts.AddFont("fontello.ttf","IconsFont");
               });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
