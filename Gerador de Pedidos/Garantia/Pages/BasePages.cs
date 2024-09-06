using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gerador_de_Pedidos.Models;
using Gerador_de_Pedidos.Services;

namespace Gerador_de_Pedidos.Pages
{
    public abstract class BasePage : ContentPage
    {
        public List<Produtos> Lista { get; set; } = new List<Produtos>();
        protected string linkplanilha;
        private string linkPadrao;

        public BasePage(string padrao)
        {
            linkPadrao = padrao;
            LoadLink();
        }

        private async void LoadLink()
        {
            string fileName = "link.txt";
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            try
            {
                linkplanilha = File.Exists(filePath) ? File.ReadAllText(filePath) : linkPadrao;
            }
            catch
            {
                linkplanilha = linkPadrao;
            }

            await LoadSheet();
        }

        protected abstract Task LoadSheet();
    }
}
