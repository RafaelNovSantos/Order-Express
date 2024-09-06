namespace Gerador_de_Pedidos.Pages
{
    public partial class DiagnosticoPage : BasePage
    {
        public DiagnosticoPage() : base("link_padrao_para_diagnostico")
        {
            InitializeComponent();
        }

        protected override async Task LoadSheet()
        {
            try
            {
                Lista = await ExcelService.LerExcelComColuna(linkplanilha, "Diagnostico", "A", "B");
                diagnostico.ItemsSource = Lista;
            }
            catch
            {
                await DisplayAlert("Erro", "Falha ao carregar a planilha.", "OK");
            }
        }
    }
}
