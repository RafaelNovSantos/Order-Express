using System;

namespace Gerador_de_Pedidos;

public class DownloadSheet
{
    public static void EditarClicked(object sender, EventArgs e, Page page)
    {
        // Use a inst�ncia da p�gina para chamar DisplayAlert
        page.DisplayAlert("Bot�o clicado!", "teste", "OK");
    }
}
