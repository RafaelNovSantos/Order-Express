public class ItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate ProdutoTemplate { get; set; }
    public DataTemplate DataTemplate { get; set; }
    public DataTemplate NomeFantasiaTemplate { get; set; }
    public DataTemplate CNPJTemplate { get; set; }
    public DataTemplate TelefoneTemplate { get; set; }
    public DataTemplate SerieTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is Produtos) return ProdutoTemplate;
        if (item is DataItem) return DataTemplate;
        if (item is NomeFantasiaItem) return NomeFantasiaTemplate;
        if (item is CNPJItem) return CNPJTemplate;
        if (item is TelefoneItem) return TelefoneTemplate;
        if (item is SerieItem) return SerieTemplate;

        return null;
    }
}
