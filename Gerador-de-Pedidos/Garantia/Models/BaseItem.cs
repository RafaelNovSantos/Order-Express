public abstract class BaseItem
{
    // Classe base abstrata
}

public class ProdutosFile : BaseItem
{
    public string Codigo { get; set; }
    public string Descricao { get; set; }
    public string Unidade { get; set; }
}

public class VendedorItem : BaseItem
{
    public string Vendedor { get; set; }
}

public class DataItem : BaseItem
{
    public string Data { get; set; }
}

public class NomeFantasiaItem : BaseItem
{
    public string NomeFantasia { get; set; }
}

public class RazaoSocialItem : BaseItem
{
    public string RazaoSocial { get; set; }
}

public class CNPJItem : BaseItem
{
    public string CNPJ { get; set; }
}

public class TelefoneItem : BaseItem
{
    public string Telefone { get; set; }
}

public class SerieItem : BaseItem
{
    public string Serie { get; set; }
}
