using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Gerador_de_Pedidos.Garantia.Models
{
    public class FileSearch
    {
        public string Vendedor { get; private set; }
        public string Codigo { get; private set; }
        public string Descricao { get; private set; }
        public string Data { get; private set; }
        public string NomeFantasia { get; private set; }
        public string CNPJ { get; private set; }
        public string Telefone { get; private set; }
        public string Serie { get; private set; }
        public string Unidade { get; private set; }
        public string RazaoSocial { get; private set; }

        public static Dictionary<string, string> Dicionario { get; private set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> ProdutosDicionario { get; private set; } = new Dictionary<string, string>();
        public async Task SelectAndSearchPdfAsync(GarantiaPage garantiaPage)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Pdf,
                    PickerTitle = "Selecione um arquivo PDF"
                });

                if (result != null)
                {
                    string pdfPath = result.FullPath;

                    using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfPath)))
                    {
                        string content = string.Empty;

                        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                        {
                            var page = pdfDoc.GetPage(i);
                            var strategy = new SimpleTextExtractionStrategy();
                            var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                            content += text;
                        }

                        // Extração dos dados usando expressões regulares
                        string dataPattern = @"\bData:\s*(\d{2}/\d{2}/\d{4})\b";
                        Match dataMatch = Regex.Match(content, dataPattern);
                        Data = dataMatch.Success ? dataMatch.Groups[1].Value : "Data não encontrada";

                        string razaoSocialPattern = @"\bRazão\s*Social:\s*([\s\S]*?)\s+(?=CNPJ|$)";
                        Match razaoSocialMatch = Regex.Match(content, razaoSocialPattern);
                        RazaoSocial = razaoSocialMatch.Success
     ? Regex.Replace(razaoSocialMatch.Groups[1].Value.Trim(), @"\r\n|\r|\n", " ")
     : "Razão Social não encontrado";



                        string nomeFantasiaPattern = @"\bNome\s*Fantasia:\s*(.+)";
                        Match nomeFantasiaMatch = Regex.Match(content, nomeFantasiaPattern);
                        NomeFantasia = nomeFantasiaMatch.Success ? nomeFantasiaMatch.Groups[1].Value.Trim() : "Nome Fantasia não encontrado";

                        string vendedorPattern = @"(?i)\bVendedor:\s*(.+)";
                        Match vendedorMatch = Regex.Match(content, vendedorPattern);
                        Vendedor = vendedorMatch.Success ? vendedorMatch.Groups[1].Value.Replace(";", "").Trim() : "Vendedor não encontrado";
                   

                        string cnpjPattern = @"\bCNPJ:\s*(\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})\b";
                        Match cnpjMatch = Regex.Match(content, cnpjPattern);
                        CNPJ = cnpjMatch.Success ? cnpjMatch.Groups[1].Value : "CNPJ não encontrado";

                        string telefonePattern = @"\bTelefone:\s*(.+)";
                        Match telefoneMatch = Regex.Match(content, telefonePattern);
                        Telefone = telefoneMatch.Success ? telefoneMatch.Groups[1].Value : "Telefone não encontrado";

                        string seriePattern = @"N/S\s+EQUIPAMENTO:\s*(?:\d+\s*)?(.+)";
                        Match serieMatch = Regex.Match(content, seriePattern);
                        Serie = serieMatch.Success ? serieMatch.Groups[1].Value.Replace("BR", "").Trim() : "Número de Série não encontrado";

                        string unidadePattern = @"(\d+)\s+UN";
                        Match unidadeMatch = Regex.Match(content, unidadePattern);
                        Unidade = unidadeMatch.Success ? unidadeMatch.Groups[1].Value : "Únidade do produto não encontrada";

                        string simplifiedPattern = @"^\d+\s+(\d+)\s+(.+?)(?:\s+\d*,\d*){0,2}\s+(\d+)\s+UN\s+([\d.,]+)\s+([\d.,]+)?$";

                        MatchCollection matches = Regex.Matches(content, simplifiedPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                        garantiaPage.AddProdutoDetalhes(this);

                        Debug.WriteLine($"{content}");

                        // Cria o dicionário com dados principais
                        Dicionario = new Dictionary<string, string>
{  { "Vendedor", Vendedor },
{ "RazaoSocial", RazaoSocial },
    { "Data", Data },
    { "NomeFantasia", NomeFantasia },
    { "CNPJ", CNPJ },
    { "Telefone", Telefone },
    { "Unidade", Unidade },
    { "Serie", Serie }
};

                        ProdutosDicionario = new Dictionary<string, string>
{

};

                        // Lista para armazenar os dados dos produtos
                        List<string> produtos = new List<string>();

                       foreach (Match match in matches)
                        {
                            if (match.Success)
                            {
                                Codigo = match.Groups[1].Value;
                                Descricao = match.Groups[2].Value;
                                Unidade = match.Groups[3].Value;
                                string codigo = match.Groups[1].Value;
                                string descricao = match.Groups[2].Value;

                                descricao = descricao.Replace("-", "").Trim();

                                // Adiciona as informações formatadas à lista

                                produtos.Add($"{codigo} {descricao} Unidade: {Unidade}");

                                // Se necessário, adicione o produto a uma página
                                garantiaPage.AddProdutoBase(this);
                            }
                        }

                        string textoFormatado = string.Empty;
                        if (produtos.Count >= 1)
                        {
                            textoFormatado = string.Join(" + ", produtos);
                        }

                        // Adiciona a lista de produtos ao dicionário
                        ProdutosDicionario["Produtos"] = textoFormatado;

                        // Mostre todos os dados no console
                        foreach (var item in ProdutosDicionario)
                        {
                            Debug.WriteLine($"{item.Value}");
                        }


                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao selecionar ou ler o arquivo PDF: {ex.Message}");
            }
        }


    }
}
