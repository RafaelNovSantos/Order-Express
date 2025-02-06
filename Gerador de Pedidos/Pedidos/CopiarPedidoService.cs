using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Gerador_de_Pedidos.Services
{
    public class CopiarPedidoService
    {
        public async Task CopiarTextoAsync(
        string vendedor,
        string saida,
        
        string tipofrete,
        string pagamento,
        string txtFrete,
        string txtFaturamento,
        string txtDefeitos,
        string txtNS,
        string txtnota,
        string txtChaveNotaExterna,
        object notaSelecionada,
        object pedidoSelecionado,
        CollectionView listaProdutosSelect,
        Button btncopy,
        FontImageSource iconCopy) // <-- Alterado de Image para FontImageSource
        {
            string freteTotal = "";
            decimal totalGeral = 0m;
            var texto = "";
            if (pedidoSelecionado?.ToString() != "Orçamento") { 
            texto = $"VENDEDOR: {vendedor}\nSAÍDA: {saida}\n\n";
            }

            if (listaProdutosSelect.ItemsSource != null && listaProdutosSelect.ItemsSource.Cast<Product>().Any())
            {
                foreach (var product in listaProdutosSelect.ItemsSource.Cast<Product>())
                {
                    var valorUnidade = decimal.TryParse(product.Valor, out var val) ? val : 0m;
                    var totalProduto = valorUnidade * (decimal.TryParse(product.Quantidade, out var qnt) ? qnt : 0m);
                    totalGeral += totalProduto;

                    texto += $"Cod.: {product.Codigo}\nDesc: {product.Descricao}\n";
                    if (!string.IsNullOrEmpty(product.Versao_Peca))
                        texto += $"Versão da Peça: {product.Versao_Peca}\n";

                    texto += $"Valor/Un: R$ {valorUnidade:F2}\nQntd: {product.Quantidade} (R$ {totalProduto:F2})\n\n";
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Atenção!", "Adicione algum produto no pedido", "OK");
                return;
            }

            decimal frete = 0m;
            if (pedidoSelecionado?.ToString() != "Garantia com retorno" && pedidoSelecionado?.ToString() != "Garantia sem retorno")
            {
                bool isFreteParsed = !string.IsNullOrEmpty(txtFrete) &&
                       decimal.TryParse(txtFrete.Replace("R$", "")
                                                   .Trim()
                                                   .Replace(".", ",", StringComparison.InvariantCulture),
                                       out frete);


                if (isFreteParsed && frete > 0)
                {
                    totalGeral += frete;
                    texto += $"FRETE({tipofrete}): R$ {frete:F2}\n\n";
                    freteTotal = " + FRETE";
                }
                else
                {
                    freteTotal = "";
                    texto += $"Frete a cotar\n\n";
                }
                texto += $"Pagamento: {pagamento}\n";

                if (pedidoSelecionado?.ToString() == "Venda" && pagamento == "BOLETO")
                {
                    texto += $"Faturamento: {txtFaturamento}\n";
                }
            }
            else
            {
                texto += $"DEFEITO: {txtDefeitos}\n\nBalança em posse do cliente:\nN/S EQUIPAMENTO: {txtNS}\n\n";
                texto += $"{notaSelecionada}:\nNº Nota: {txtnota}\n";

                if (notaSelecionada?.ToString() == "Nota Externa")
                {
                    texto += $"CHAVE NOTA EXTERNA: {txtChaveNotaExterna}\n";
                }
            }

            texto += $"\nTOTAL VALOR{freteTotal} = R$ {totalGeral:F2}";

            try
            {
                await Clipboard.SetTextAsync(texto);
                btncopy.Text = "Copiado!";
                iconCopy.Color = Colors.Black; // Agora alteramos diretamente o FontImageSource
                await Task.Delay(5000);
                btncopy.Text = "Copiar";
                iconCopy.Color = Colors.Green;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível copiar o texto: {ex.Message}", "OK");
            }
        }
    }
    }
