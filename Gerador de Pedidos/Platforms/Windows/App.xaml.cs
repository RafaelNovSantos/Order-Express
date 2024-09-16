using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
using Microsoft.UI;

namespace Gerador_de_Pedidos.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        private Microsoft.UI.Xaml.Window nativeWindow;

        public App()
        {
            this.InitializeComponent();

            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
                nativeWindow = handler.PlatformView;
                nativeWindow.Activated += OnWindowActivated;
            });
        }

        private void OnWindowActivated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            if (nativeWindow == null) return;

            // Get the window handle
            IntPtr windowHandle = WindowNative.GetWindowHandle(nativeWindow);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            // Make sure appWindow is valid
            if (appWindow != null)
            {
                var displayArea = GetDisplayArea();
                appWindow.MoveAndResize(displayArea);

                // Set the window to be maximized
                var presenter = appWindow.Presenter as OverlappedPresenter;
                if (presenter != null)
                {
                    presenter.IsMaximizable = true;
                    presenter.IsResizable = true;
                    presenter.Maximize(); // Maximiza a janela
                }
            }

            // Optionally, remove the event handler after use if not needed
            nativeWindow.Activated -= OnWindowActivated;
        }

        private RectInt32 GetDisplayArea()
        {
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            return new RectInt32(0, 0, (int)displayInfo.Width, (int)displayInfo.Height);
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
