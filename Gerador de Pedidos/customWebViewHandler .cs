#if WINDOWS
using Microsoft.UI.Xaml.Controls;
using Microsoft.Maui.Handlers;
using WebView = Microsoft.Maui.Controls.WebView;

public partial class CustomWebViewHandler : WebViewHandler
{
    protected override void ConnectHandler(Microsoft.UI.Xaml.Controls.WebView2 platformView)
    {
        base.ConnectHandler(platformView);

        // Aqui você pode configurar o WebView2 se necessário
    }

    public void ExecuteJavaScript(string script)
    {
        // Executa JavaScript no WebView2
        if (PlatformView != null)
        {
            PlatformView.CoreWebView2.ExecuteScriptAsync(script);
        }
    }
}
#endif
