using Gerador_de_Pedidos;
using Gerador_de_Pedidos.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using SQLite;
using System.IO;

namespace Gerador_de_Pedidos
{
    public partial class App : Application
    {
        private static Database _database;
        public static Database Database
        {
            get
            {
                if (_database == null)
                {
                    string dbPath;

                    // Verifica o sistema operacional
                    if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        dbPath = Path.Combine(FileSystem.AppDataDirectory, "Pedidos.db3");
                    }
                    else
                    {
                        dbPath = @"C:\Order_Express\Pedidos.db3";
                        string dbDirectory = Path.GetDirectoryName(dbPath);
                        if (!Directory.Exists(dbDirectory))
                        {
                            Directory.CreateDirectory(dbDirectory);
                        }
                    }

                    _database = new Database(dbPath); // Inicializando a instância de Database
                }

                return _database;
            }
        }

        public App()
        {
            InitializeComponent();

            // Registra o serviço de dados compartilhados
            DependencyService.Register<DadosCompartilhadosService>();
            MainPage = new AppShell();

            // Customização para Entry
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
            var database = App.Database; // Acessa a instância do banco

        }
    }
}
