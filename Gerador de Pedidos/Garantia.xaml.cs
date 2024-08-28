namespace Gerador_de_Pedidos
{
    public partial class Garantia : FlyoutPage
    {
        public Garantia()
        {
            InitializeComponent();
        }

        // Método para alterar o conteúdo do Detail
        private void Button_Clicked(object sender, EventArgs e)
        {
            // Verifica se o Detail é uma NavigationPage
            if (Detail is NavigationPage navigationPage)
            {
                // Navega para a página raiz na pilha de navegação
                Navigation.PopToRootAsync();
            }
            else
            {
                // Caso o Detail não seja uma NavigationPage, você pode querer substituí-lo
                Detail = new NavigationPage(new MainPage());
                IsPresented = false; // Fecha o menu lateral (Flyout)
            }
        }
    }
}
