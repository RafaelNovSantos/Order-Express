using System;
using System.Linq;
using System.Threading.Tasks;


public class ProdutoService
{
    public async Task ProcessarSelecao(string selectedValue, string cod, IList<Product> listaProdutos, Entry txtDescricao, Entry txtValor, Label lblStatusProduto)
    {
        int columnToUse;

        // Definir a coluna correta com base na seleção do Picker
        switch (selectedValue)
        {
            case "Valor ATA":
                columnToUse = 3; // Ajuste com base na coluna correta
                break;
            case "Valor Oficina":
                columnToUse = 4; // Ajuste com base na coluna correta
                break;
            case "Valor Cliente Final":
                columnToUse = 5; // Ajuste com base na coluna correta
                break;
            default:
                await Application.Current.MainPage.DisplayAlert("Erro", "Tipo de valor não selecionado.", "OK");
                lblStatusProduto.Text = "Tipo de valor não selecionado.";
                lblStatusProduto.TextColor = Color.FromHex("#FF0000"); // Vermelho para indicar erro
                return;
        }

        // Recarregar os dados da planilha usando a coluna correta
        if (!string.IsNullOrEmpty(cod))
        {
            cod = cod.ToUpper();
            // Buscar o item correspondente na lista
            var item = listaProdutos.FirstOrDefault(i => i.Codigo == cod);
            if (item != null)
            {
                txtDescricao.Text = item.Descricao;

                // Converter item.Valor para decimal e formatar com duas casas decimais
                if (decimal.TryParse(((item.Valor).Replace("R", "").Replace("$", "")), out decimal valorNumerico))
                {
                    txtValor.Text = valorNumerico.ToString("F2");
                }
                else
                {
                    txtValor.Text = string.Empty;
                    txtValor.Text = "Valor inválido";
                }

                // Atualizar o status do produto
                lblStatusProduto.Text = "Produto Encontrado";
                lblStatusProduto.FontSize = 15;
                lblStatusProduto.TextColor = Color.FromHex("#00FF00"); // Verde para indicar sucesso
            }
            else
            {
                txtDescricao.Text = string.Empty;
                txtValor.Text = string.Empty;
                lblStatusProduto.Text = "Produto Não Encontrado";
                lblStatusProduto.FontSize = 12;
                lblStatusProduto.TextColor = Color.FromHex("#FF0000"); // Vermelho para indicar erro
            }
        }
        else
        {
            txtDescricao.Text = string.Empty;
            txtValor.Text = string.Empty;
            lblStatusProduto.FontSize = 17;
            lblStatusProduto.Text = "Digite o Código...";
            lblStatusProduto.TextColor = Color.FromHex("#FFFAFF00"); // Laranja para indicar aviso
        }
    }
}
