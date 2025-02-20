using OfficeOpenXml;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ExcelReader
{
    public async Task<List<Product>> LerExcelComColuna(string fileUrl, string sheetName, int valorColumnIndex)
    {
        var listaProdutos = new List<Product>();

        int tentativas = 0;
        int maxTentativas = 3;

        while (tentativas < maxTentativas)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(fileUrl);
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets[sheetName];

                            if (worksheet == null || worksheet.Dimension == null || worksheet.Cells == null)
                            {
                                throw new Exception($"Página '{sheetName}' da planilha não encontrada ou as células estão vazias.");
                            }

                            var rowCount = worksheet.Dimension.Rows;
                            bool linhaVazia = true;

                            for (int row = 2; row <= rowCount; row++)
                            {
                                var codigo = worksheet.Cells[row, 1]?.Text;
                                var descricao = worksheet.Cells[row, 2]?.Text;
                                var valor = worksheet.Cells[row, valorColumnIndex]?.Text;

                                if (string.IsNullOrWhiteSpace(codigo) &&
                                    string.IsNullOrWhiteSpace(descricao) &&
                                    string.IsNullOrWhiteSpace(valor))
                                {
                                    continue; // Ignorar linhas vazias
                                }

                                var produto = new Product
                                {
                                    Codigo = !string.IsNullOrWhiteSpace(codigo) ? codigo : "N/A",
                                    Descricao = !string.IsNullOrWhiteSpace(descricao) ? descricao : "N/A",
                                    Valor = !string.IsNullOrWhiteSpace(valor) ? valor : "N/A"
                                };

                                listaProdutos.Add(produto);
                                linhaVazia = false;
                            }

                            if (linhaVazia)
                            {
                                // Adicionar um produto padrão se a planilha estiver vazia
                                listaProdutos.Add(new Product { Codigo = "N/A", Descricao = "Produto Padrão", Valor = "N/A" });
                            }
                        }
                    }
                }
                break; // Saia do loop se a operação foi bem-sucedida
            }
            catch (HttpRequestException ex)
            {
                tentativas++;
                Console.WriteLine($"Erro ao acessar a planilha: {ex.Message}");

                if (tentativas >= maxTentativas)
                {
                    throw new Exception($"Falha ao acessar a planilha após várias tentativas. Erro: {ex.Message}");
                }

                await Task.Delay(5000); // Aguardar 5 segundos antes de tentar novamente
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw new Exception($"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        return listaProdutos;
    }
}
