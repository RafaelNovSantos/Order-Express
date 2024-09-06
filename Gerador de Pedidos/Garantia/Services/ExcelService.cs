using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Gerador_de_Pedidos.Models;

namespace Gerador_de_Pedidos.Garantia.Services
{
    public class ExcelService
    {
        public static async Task<List<Produtos>> LerExcelComColuna(string fileUrl, string sheetName, string codigoColuna, string descricaoColuna)
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
                                    throw new Exception($"Planilha '{sheetName}' não encontrada ou está vazia.");

                                var rowCount = worksheet.Dimension.Rows;
                                int codigoIndex = worksheet.Cells[sheetName + codigoColuna].Start.Column;
                                int descricaoIndex = worksheet.Cells[sheetName + descricaoColuna].Start.Column;

                                for (int row = 2; row <= rowCount; row++)
                                {
                                    var codigo = worksheet.Cells[row, codigoIndex]?.Text;
                                    var descricao = worksheet.Cells[row, descricaoIndex]?.Text;

                                    if (string.IsNullOrWhiteSpace(codigo) && string.IsNullOrWhiteSpace(descricao))
                                        continue;

                                    produtos.Add(new Produtos
                                    {
                                        Codigo = string.IsNullOrWhiteSpace(codigo) ? "N/A" : codigo,
                                        Descricao = string.IsNullOrWhiteSpace(descricao) ? "N/A" : descricao,
                                    });
                                }

                                return produtos.Count > 0 ? produtos : new List<Produtos> { new Produtos { Codigo = "N/A", Descricao = "N/A" } };
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    if (++tentativas >= maxTentativas)
                        throw;
                    await Task.Delay(5000); // Tentar novamente após 5 segundos
                }
            }
            return produtos;
        }
    }
}
