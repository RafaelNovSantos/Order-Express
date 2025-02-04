using Gerador_de_Pedidos;
using Gerador_de_Pedidos.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;


namespace Gerador_de_Pedidos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
            {
#if __ANDROID__
    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif __IOS__
    handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
    handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });

            // Customização para Picker
            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(BorderlessPicker), (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });
        }

    }
}
