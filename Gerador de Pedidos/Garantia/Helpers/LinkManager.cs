using System;
using System.IO;

namespace Gerador_de_Pedidos.Garantia.Helpers
{
    public class LinkManager
    {
        private string linkPadrao = "https://docs.google.com/spreadsheets/d/1AWp_sTLnWgcM7zVRR4x3zit8wbOucJ9m43s7M4yNuYU/export?usp=sharing";

        public string CarregarLink(string fileName)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                else
                {
                    return linkPadrao;
                }
            }
            catch (Exception)
            {
                return linkPadrao;
            }
        }

        public void SalvarLink(string fileName, string novoLink)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            try
            {
                File.WriteAllText(filePath, novoLink);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao salvar o link", ex);
            }
        }

        public string ConvertToExportLink(string editLink)
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
}
