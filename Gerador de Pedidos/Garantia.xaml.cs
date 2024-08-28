namespace Gerador_de_Pedidos
{
    public partial class Garantia : FlyoutPage
    {
        public Garantia()
        {
            InitializeComponent();
        }

        // M�todo para alterar o conte�do do Detail
        private void Button_Clicked(object sender, EventArgs e)
        {
            // Verifica se o Detail � uma NavigationPage
            if (Detail is NavigationPage navigationPage)
            {
                // Navega para a p�gina raiz na pilha de navega��o
                Navigation.PopToRootAsync();
            }
            else
            {
                // Caso o Detail n�o seja uma NavigationPage, voc� pode querer substitu�-lo
                Detail = new NavigationPage(new MainPage());
                IsPresented = false; // Fecha o menu lateral (Flyout)
            }
        }
    }
}
