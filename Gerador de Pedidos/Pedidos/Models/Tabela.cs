public class PlanilhaService
{
    private static PlanilhaService _instance;
    public static PlanilhaService Instance => _instance ??= new PlanilhaService();

    public string SelectedSheetName { get; set; }
}
