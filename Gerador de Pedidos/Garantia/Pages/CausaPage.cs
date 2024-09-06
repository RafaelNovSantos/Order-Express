using Android.Telecom;

namespace Gerador_de_Pedidos.Pages
{
    public partial class CausaPage : BasePage
    {
        public CausaPage() : base("link_padrao_para_causa")
        {
            InitializeComponent();
        }

        protected override async Task LoadSheet()
        {
            try
            {
                Lista = await ExcelService.LerExcelComColuna(linkplanilha, "Causa", "A", "B");
                causa.ItemsSource = Lista;
            }
            catch
            {
                await DisplayAlert("Erro", "Falha ao carregar a planilha.", "OK");
            }
        }
    }
}
