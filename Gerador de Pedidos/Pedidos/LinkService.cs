using Gerador_de_Pedidos;
using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;

public class LinkService
{
    
    public async Task AlterarLink(string senha, string novoLink, string selectedValue, ActivityIndicator loadingIndicatorPedido, Label lblStatusProduto)
    {
        if (senha == "Systelcapacitacao@1234")
        {
            
            if (!string.IsNullOrEmpty(novoLink))
            {
                string linkExportacao = ConvertToExportLink(novoLink);
                var adicionarnovoLink = new Planilha
                {
                    Modelo = "Pedido",
                    LinkPlanilha = linkExportacao,
                    DataMudanca = DateTime.Now
                };
                await App.Database.SalvarPlanilhaAsync(adicionarnovoLink);
               
                try
                {
                    
                    await Application.Current.MainPage.DisplayAlert("Link Atualizado", $"O link da planilha foi atualizado com sucesso para: {linkExportacao}", "OK");
                    
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao salvar o link: {ex.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "O novo link não pode ser vazio.", "OK");
            }
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Erro", "Senha incorreta. A alteração do link não foi autorizada.", "OK");
        }
    }

    private static string ConvertToExportLink(string editLink)
    {
        if (string.IsNullOrWhiteSpace(editLink))
            throw new ArgumentException("O link não pode ser nulo ou vazio.", nameof(editLink));
        if (editLink.Contains("/edit"))
        {
            return editLink.Replace("/edit", "/export");
        }
        return editLink;
    }
}
