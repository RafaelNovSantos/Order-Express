using Gerador_de_Pedidos.Garantia.Services;
using Gerador_de_Pedidos.Models;
using Gerador_de_Pedidos.Services;
using System.Threading.Tasks;

namespace Gerador_de_Pedidos.Pages
{
    public partial class GarantiaPage : BasePage
    {
        public GarantiaPage() : base("https://docs.google.com/spreadsheets/d/1AWp_sTLnWgcM7zVRR4x3zit8wbOucJ9m43s7M4yNuYU/export?usp=sharing")
        {
            InitializeComponent();
        }

        protected override async Task LoadSheet()
        {
            try
            {
                Lista = await ExcelService.LerExcelComColuna(linkplanilha, "Equipamentos", "A", "B");
                equipamentos.ItemsSource = Lista;
            }
            catch
            {
                await DisplayAlert("Erro", "Falha ao carregar a planilha.", "OK");
            }
        }
    }
}
