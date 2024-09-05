using System;

namespace Gerador_de_Pedidos;

public class DownloadSheet
{
    public static void EditarClicked(object sender, EventArgs e, Page page)
    {
        // Use a instância da página para chamar DisplayAlert
        page.DisplayAlert("Botão clicado!", "teste", "OK");
    }
}
