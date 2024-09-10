using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

public class FileSearch
{
    public static async Task SelectAndSearchPdfAsync()
    {
        try
        {
            // Usar o FilePicker para selecionar um arquivo PDF
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf,
                PickerTitle = "Selecione um arquivo PDF"
            });

            if (result != null)
            {
                // Caminho completo do arquivo selecionado
                string pdfPath = result.FullPath;

                // Abrir o arquivo PDF
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfPath)))
                {
                    // String para armazenar todo o conteúdo do PDF
                    string content = string.Empty;

                    // Extrair o conteúdo de cada página
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        var page = pdfDoc.GetPage(i);
                        var strategy = new SimpleTextExtractionStrategy();
                        var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                        content += text;
                    }

                    // Padrão para Data (exemplo: 01/01/2024)
                    string dataPattern = @"\bData:\s*(\d{2}/\d{2}/\d{4})\b";
                    Match dataMatch = Regex.Match(content, dataPattern);
                    string data = dataMatch.Success ? dataMatch.Groups[1].Value : "Data não encontrada";

                    // Padrão para Nome Fantasia (ajuste conforme o formato no PDF)
                    string nomeFantasiaPattern = @"\bNome\s*Fantasia:\s*(.+)";
                    Match nomeFantasiaMatch = Regex.Match(content, nomeFantasiaPattern);
                    string nomeFantasia = nomeFantasiaMatch.Success ? nomeFantasiaMatch.Groups[1].Value.Trim() : "Nome Fantasia não encontrado";

                    // Padrão para CNPJ (exemplo: 00.000.000/0000-00)
                    string cnpjPattern = @"\bCNPJ:\s*(\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2})\b";
                    Match cnpjMatch = Regex.Match(content, cnpjPattern);
                    string cnpj = cnpjMatch.Success ? cnpjMatch.Groups[1].Value : "CNPJ não encontrado";

                    // Padrão para Telefone no formato +55 37 99941-0778
                    string telefonePattern = @"\+55\s*\d{2}\s*\d{5}-\d{4}";
                    Match telefoneMatch = Regex.Match(content, telefonePattern);
                    string telefone = telefoneMatch.Success ? telefoneMatch.Value : "Telefone não encontrado";
                    // Padrão para Número de Série
                    string seriePattern = @"N/S\s+EQUIPAMENTO:\s*(?:\d+\s*)?(.+)";

                    Match serieMatch = Regex.Match(content, seriePattern);
                    string serie = serieMatch.Success ? $"{serieMatch.Groups[1].Value}{serieMatch.Groups[2].Value}" : "Número de Série não encontrado";
                    serie = serie.Replace("BR", "").Trim();

                    // Exibir as informações extraídas
                    await Application.Current.MainPage.DisplayAlert("Data", $"{data}", "Ok");
                    await Application.Current.MainPage.DisplayAlert("Nome Fantasia", $"{nomeFantasia}", "Ok");
                    await Application.Current.MainPage.DisplayAlert("CNPJ", $"{cnpj}", "Ok");
                    await Application.Current.MainPage.DisplayAlert("Telefone", $"{telefone}", "Ok");
                    await Application.Current.MainPage.DisplayAlert("N/S Equipamento", $"{serie}", "Ok");

                    // Log do conteúdo extraído
                    Debug.WriteLine("Conteúdo extraído do PDF:");
                    Debug.WriteLine(content);

                    // Padrão simplificado para capturar Código e Descrição
                    string simplifiedPattern = @"^\d+\s+(\d+)\s+(.+?)(?:\s+\d*,\d*){0,2}\s+(\d+)\s+UN\s+([\d.,]+)\s+([\d.,]+)?$";

                    MatchCollection matches = Regex.Matches(content, simplifiedPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    foreach (Match match in matches)
                    {
                        if (match.Success)
                        {
                            string codigo = match.Groups[1].Value;
                            string descricao = match.Groups[2].Value;

                            descricao = descricao.Replace("-", "").Trim();

                            // Exibir os resultados ou processar conforme necessário
                            await Application.Current.MainPage.DisplayAlert("Produto", $"{codigo} {descricao}", "Ok");
                        }
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
