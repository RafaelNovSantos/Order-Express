using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Gerador_de_Pedidos;
using Gerador_de_Pedidos.Garantia.Models;

namespace Gerador_de_Pedidos.Garantia.Helpers
{
    public class ExcelHelper
    {
        public static async Task<List<Produtos>> LerExcelComColuna(string fileUrl, string sheetName, int codigoColunaIndex, int descricaoColunaIndex, GarantiaPage GarantiaPage, ContentPage page)
        {
            var produtos = new List<Produtos>();
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
                                if (worksheet == null || worksheet.Dimension == null)
                                {
                                    await page.DisplayAlert("Erro", $"Planilha '{sheetName}' não encontrada ou está vazia.", "OK");
                                    return produtos;
                                }

                                var rowCount = worksheet.Dimension.Rows;

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    var codigo = worksheet.Cells[row, codigoColunaIndex]?.Text;
                                    var descricao = worksheet.Cells[row, descricaoColunaIndex]?.Text;

                                    if (string.IsNullOrWhiteSpace(codigo) && string.IsNullOrWhiteSpace(descricao))
                                        continue;

                                    produtos.Add(new Produtos
                                    {
                                        Codigo = string.IsNullOrWhiteSpace(codigo) ? "N/A" : codigo,
                                        Descricao = string.IsNullOrWhiteSpace(descricao) ? "N/A" : descricao,
                                    });

                                    Console.WriteLine($"Linha {row}: Código = {codigo}, Descrição = {descricao}"); // Log para depuração
                                }

                                return produtos.Count > 0 ? produtos : new List<Produtos> { new Produtos { Codigo = "N/A", Descricao = "N/A" } };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}"); // Log para depuração
                    if (++tentativas >= maxTentativas)
                        throw;
                    await Task.Delay(5000); // Tentar novamente após 5 segundos
                }
            }

            return produtos;
        }


    }
}
