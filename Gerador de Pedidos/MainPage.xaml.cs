﻿using System.Net.Http;
using OfficeOpenXml;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Gerador_de_Pedidos.Services;
using Gerador_de_Pedidos;
using Gerador_de_Pedidos.Historico;
using static System.Net.WebRequestMethods;
using SQLite;

namespace Gerador_de_Pedidos
{
    public partial class MainPage : ContentPage
    {
        private string linkplanilha;
        public MainPage()
        {
            InitializeComponent();
            pedido.SelectedIndex = 0;
            equipamentos.SelectedIndex = 0;
            pag.SelectedIndex = 0;
            nota.SelectedIndex = 0;
            TipoFrete.SelectedIndex = 0;
            valores.SelectedIndex = 0;
            GetProximoNumeroPedidoAsync();
            LoadLink();
            MeuBudget = new Budget { Numero_Pedido = 0 };
            MeuBudget = new Budget { Valor_Total = "0,00" }; // Inicializa com zero
            BindingContext = this;
        }
        public List<Product> Lista = new List<Product>();
        public List<Product> ListaSelecionados = new List<Product>();
        public Budget MeuBudget { get; set; }
        private string _ultimoItemSelecionado;

        public async Task<int> GetProximoNumeroPedidoAsync()
        {
            // Cria a conexão usando o banco de dados assíncrono
            var connection = App.Database.GetConnection();

            // Consulta para pegar o último NumeroPedido
            var ultimoPedido = await connection.Table<Pedido>().OrderByDescending(p => p.NumeroPedido).FirstOrDefaultAsync();

            MeuBudget.Numero_Pedido = ultimoPedido?.NumeroPedido + 1 ?? 1;

            CalValorTotal();
            // Se houver pedidos, incrementa o último NumeroPedido + 1, senão começa com 1
            return ultimoPedido?.NumeroPedido + 1 ?? 1;
        }

        private async void LoadLink()
        {
            string fileName = "link.txt";
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    linkplanilha = System.IO.File.ReadAllText(filePath);
                    OnPickerSelectionChangedPrice(valores, EventArgs.Empty);
                }
                else
                {
                    linkplanilha = "https://docs.google.com/spreadsheets/d/1tF_sKR6Mne3H1HPSuz9G2rlHahqnTmX_KaxUuHV6qBw/export?usp=sharing";
                    OnPickerSelectionChangedPrice(valores, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao ler o link: {ex.Message}", "OK");
                linkplanilha = "https://docs.google.com/spreadsheets/d/1tF_sKR6Mne3H1HPSuz9G2rlHahqnTmX_KaxUuHV6qBw/export?usp=sharing";
                OnPickerSelectionChangedPrice(valores, EventArgs.Empty);

            }
        }
        private async void OnAlterarLinkClicked(object sender, EventArgs e)
        {

            var selectedValue = valores.SelectedItem?.ToString();

            string senha = await DisplayPromptAsync("Autenticação", "Digite a senha para alterar o link da planilha Sheet Google:");

            if (senha == "Systelcapacitacao@1234")
            {
                string novoLink = await DisplayPromptAsync("Alterar Link", "Digite o novo link da planilha:");

                if (!string.IsNullOrEmpty(novoLink))
                {
                    string linkExportacao = ConvertToExportLink(novoLink);
                    linkplanilha = linkExportacao;

                    string fileName = "link.txt";
                    string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                    try
                    {
                        System.IO.File.WriteAllText(filePath, linkplanilha);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Erro", $"Erro ao salvar o link: {ex.Message}", "OK");
                    }

                    await DisplayAlert("Link Atualizado", $"O link da planilha foi atualizado com sucesso para: {linkExportacao}", "OK");
                    await ExecuteTask(selectedValue);

                }
            }
            else
            {
                await DisplayAlert("Erro", "Senha incorreta. A alteração do link não foi autorizada.", "OK");
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

        private string selectedSheetName;

        //Bloqueia caracteres, funciona somente números inteiros
        private void OnIntegerTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                if (!e.NewTextValue.All(char.IsDigit))
                {
                    DisplayAlert("Entrada Inválida", "Por favor, insira apenas números.", "OK");
                    entry.Text = e.OldTextValue;
                }
            }
        }
        //Bloqueia caracteres, funciona somente números inteiros e double
        private void OnDoubleTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;

            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                if (e.NewTextValue == "Valor inválido")
                {
                    // Exibe a mensagem para "N/A"
                    DisplayAlert("Valor Inválido", "Produto na planilha sem valor", "OK");
                    entry.Text = e.OldTextValue;
                }
                else if (!double.TryParse(e.NewTextValue, out _))
                {
                    // Exibe a mensagem para valores não numéricos
                    DisplayAlert("Entrada Inválida", "Por favor, insira apenas números.", "OK");
                    entry.Text = e.OldTextValue;
                }
            }

            CalValorTotal();
        }
        private void SelectionChangedCopyCod(object sender, SelectionChangedEventArgs e)
        {

            // Obtém os itens selecionados na CollectionView
            var selectedItems = e.CurrentSelection.Cast<Product>().ToList();

            if (selectedItems.Count > 0)
            {
                // Assume que seleciona apenas um item
                var selectedItem = selectedItems.FirstOrDefault();

                if (selectedItem != null && (lblStatusProduto.Text == "Produto Encontrado" || lblStatusProduto.Text == "Digite o Código..." || lblStatusProduto.Text == "Produto Não Encontrado"))
                {
                    txtCodigo.Text = null;
                    // Atualiza o txtCodigo com o código do produto selecionado
                    txtCodigo.Text = selectedItem.Codigo;
                    txtVendedor.Focus();
                    txtQuantidade.Focus();
                    // Força a chamada ao método OnTxtCodigoTextChangedUnified
                    OnTxtCodigoTextChangedUnified(txtCodigo, new TextChangedEventArgs(string.Empty, txtCodigo.Text));
                }
            }
        }
        private async void OnPickerSelectionChanged(object sender, EventArgs e)
        {
            var selectedValue = valores.SelectedItem?.ToString();
            var picker = sender as Picker;
            if (picker != null && picker.SelectedItem != null)
            {
                selectedSheetName = picker.SelectedItem.ToString(); // Atualiza a variável de instância
                Console.WriteLine($"Nome da planilha selecionada: {selectedSheetName}"); // Adicionando um log para depuração
                OnPickerSelectionChangedPrice(valores, EventArgs.Empty);
                await ExecuteTask(selectedValue);
                // Chame a função LerExcel passando o nome da planilha correspondente
                // Remova esta chamada se você não quiser que a função seja chamada automaticamente ao selecionar um item

            }
        }

        //Bloqueia caracteres, funciona somente número inteiros e chama a função para atualizar a tabela com a lista da planilha
        private void OnTxtCodigoTextChangedUnified(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;


            // Chama o método para atualizar o Picker, ou qualquer outra lógica adicional
            OnPickerSelectionChangedPrice(valores, EventArgs.Empty);
        }
        // Defina o evento OnPickerSelectionChanged

        private async Task ExecuteTask(string selectedValue)
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
                    await DisplayAlert("Erro", "Tipo de valor não selecionado.", "OK");
                    loadingIndicatorPedido.IsVisible = false; // Ocultar o botão se houve erro
                    loadingIndicatorPedido.IsRunning = false;
                    lblStatusProduto.IsVisible = true; // Mostrar o texto de status se houve erro
                    lblStatusProduto.Text = "Tipo de valor não selecionado.";
                    lblStatusProduto.TextColor = Color.FromHex("#FF0000"); // Vermelho para indicar erro
                    return;
            }
            await LerExcelComColuna(linkplanilha, selectedSheetName, columnToUse);
            // Evite chamar `LerExcelComColuna` novamente, já que isso é feito com base na seleção
        }
        private async Task ProcessarSelecao(string selectedValue)
        {
            // Exemplo de como usar o ProdutoService para processar a seleção
            var produtoService = new ProdutoService();
            await produtoService.ProcessarSelecao(selectedValue, txtCodigo.Text, Lista, txtDescricao, txtValor, lblStatusProduto);
        }

        private async void OnPickerSelectionChangedPrice(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker == null || picker.SelectedItem == null)
                return;

            var selectedValue = picker.SelectedItem.ToString();

            // Verifica se o item selecionado mudou
            if (selectedValue != _ultimoItemSelecionado)
            {
                await ExecuteTask(selectedValue);

                // Atualiza o item selecionado após a tarefa ser executada
                _ultimoItemSelecionado = selectedValue;
            }

            // Mostrar o botão de busca e ocultar o texto de status enquanto a busca está em andamento
            loadingIndicatorPedido.IsVisible = true;
            loadingIndicatorPedido.IsRunning = true;
            lblStatusProduto.IsVisible = true;
            lblStatusProduto.Text = "center";
            lblStatusProduto.TextColor = Color.FromHex("#00000000");

            // Inicia a rotação do ícone e do botão de busca


            await ProcessarSelecao(selectedValue);

            // Parar a rotação e ocultar os ícones

            loadingIndicatorPedido.IsRunning = false;
            loadingIndicatorPedido.IsVisible = false;
            lblStatusProduto.IsVisible = true;
        }

        private async void OnAtualizarClicked(object sender, EventArgs e)
        {
            // Supondo que o Picker `valores` já tenha um item selecionado
            var selectedValue = valores.SelectedItem?.ToString();
            txtCodigo.Text += 0.ToString();

            if (txtCodigo.Text.Length > 0)
            {
                txtCodigo.Text = txtCodigo.Text.Substring(0, txtCodigo.Text.Length - 1);
            }

            if (selectedValue != null)
            {
                await ExecuteTask(selectedValue);
            }
            else
            {
                await DisplayAlert("Erro", "Nenhum valor selecionado.", "OK");
            }
        }

        async Task LerExcelComColuna(string fileUrl, string sheetName, int valorColumnIndex)
        {
            // Mostrar o indicador de carregamento e desativar o ScrollView

            disableFrame.IsVisible = false;
            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;
            listaProdutosExcel.IsVisible = false;

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
                                    await DisplayAlert("Erro", $"Página '{sheetName}' da planilha não encontrada ou as células da planilha estão vazias", "OK");
                                    CriarListaComProdutoPadrao();
                                    return;
                                }

                                Lista.Clear();

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

                                    Lista.Add(produto);
                                    linhaVazia = false;
                                }

                                if (linhaVazia)
                                {
                                    CriarListaComProdutoPadrao();
                                }

                                // Atualizar a visualização da lista
                                listaProdutosExcel.ItemsSource = null; // Limpar a origem de itens para forçar a atualização
                                listaProdutosExcel.ItemsSource = Lista;
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
                        await DisplayAlert("Erro", "Falha ao acessar a planilha após várias tentativas. Erro: " + ex.Message, "OK");
                        CriarListaComProdutoPadrao();
                        break;
                    }

                    await Task.Delay(5000); // Aguardar 5 segundos antes de tentar novamente
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                    await DisplayAlert("Erro", "Ocorreu um erro inesperado: " + ex.Message, "OK");
                    CriarListaComProdutoPadrao();
                    break;
                }
            }
            disableFrame.IsVisible = true;
            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
            listaProdutosExcel.IsVisible = true;
        }
        void CriarListaComProdutoPadrao()
        {
            Lista.Clear();
            Lista.Add(new Product
            {
                Codigo = "N/A",
                Descricao = "N/A",
                Valor = "N/A",
                Quantidade = "N/A"
            });

            listaProdutosExcel.ItemsSource = new List<Product>();
            listaProdutosExcel.ItemsSource = Lista;
        }
        private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = e.CurrentSelection.Cast<Product>().ToList();
        }
        private async void OnEditarClicked(object sender, EventArgs e)
        {
            var selectedItems = listaProdutosSelect.SelectedItems?.Cast<Product>().ToList();

            if (selectedItems == null || selectedItems.Count == 0)
            {
                await DisplayAlert("Aviso", "Nenhum item selecionado.", "OK");
                return;
            }

            string action = await DisplayActionSheet("Escolha o campo a editar", "Cancelar", null, "Código", "Descrição", "Valor", "Quantidade", "Versão Peça");

            if (action == "Cancelar")
                return;

            // Pega o valor do primeiro item selecionado como referência para pré-preencher o campo
            string valorAtual = action switch
            {
                "Código" => selectedItems[0].Codigo,
                "Descrição" => selectedItems[0].Descricao,
                "Valor" => selectedItems[0].Valor,
                "Quantidade" => selectedItems[0].Quantidade,
                "Versão Peça" => selectedItems[0].Versao_Peca,
                _ => ""
            };

            // Abre o prompt com o valor atual preenchido
            string newValue = await DisplayPromptAsync("Editar", $"Digite o novo valor para {action}:", "OK", "Cancelar", initialValue: valorAtual);

            if (string.IsNullOrEmpty(newValue))
                return;

            // Atualiza todos os itens selecionados
            foreach (var item in selectedItems)
            {
                switch (action)
                {
                    case "Código":
                        item.Codigo = newValue;
                        break;
                    case "Descrição":
                        item.Descricao = newValue;
                        break;
                    case "Valor":
                        item.Valor = newValue;
                        break;
                    case "Quantidade":
                        item.Quantidade = newValue;
                        break;
                    case "Versão Peça":
                        item.Versao_Peca = newValue;
                        break;
                }
            }

            // Atualiza a interface automaticamente
            listaProdutosSelect.ItemsSource = null;
            listaProdutosSelect.ItemsSource = ListaSelecionados;
        CalValorTotal();
        }
        private async void OnExcluirClicked(object sender, EventArgs e)
        {
            var selectedItems = listaProdutosSelect.SelectedItems.Cast<Product>().ToList();

            if (selectedItems.Count == 0)
            {
                await DisplayAlert("Aviso", "Nenhum item selecionado para excluir.", "OK");
                return;
            }

            // Confirmação para excluir os itens selecionados
            bool confirm = await DisplayAlert("Confirmação", $"Deseja realmente excluir {selectedItems.Count} item(s) selecionado(s)?", "Sim", "Não");
            if (!confirm)
                return;

            foreach (var item in selectedItems)
            {
                ListaSelecionados.Remove(item);
            }
            // Atualiza a CollectionView e limpa a seleção
            listaProdutosSelect.SelectedItems.Clear(); // Limpa a seleção
            listaProdutosSelect.ItemsSource = null;
            listaProdutosSelect.ItemsSource = ListaSelecionados;
            CalValorTotal();
        }
        private async void OnAdicionarClicked(object sender, EventArgs e)
        {
            // Obtém os valores dos campos de entrada
            string cod = txtCodigo.Text;
            string descricao = txtDescricao.Text;
            string valor = txtValor.Text;
            string quantidade = txtQuantidade.Text;
            string versao_peca = txtVersion.Text;

            // Verifica se todos os campos estão preenchidos
            bool isCodEmpty = string.IsNullOrEmpty(cod);
            bool isDescricaoEmpty = string.IsNullOrEmpty(descricao);
            bool isValorEmpty = string.IsNullOrEmpty(valor);
            bool isQuantidadeEmpty = string.IsNullOrEmpty(quantidade);

            if (isCodEmpty || isDescricaoEmpty || isValorEmpty || isQuantidadeEmpty)
            {
                // Cria uma mensagem com os campos que estão faltando
                string missingFieldsMessage = "Por favor, preencha os seguintes campos:\n";
                if (isCodEmpty) missingFieldsMessage += "- Código\n";
                if (isDescricaoEmpty) missingFieldsMessage += "- Descrição\n";
                if (isValorEmpty) missingFieldsMessage += "- Valor\n";
                if (isQuantidadeEmpty) missingFieldsMessage += "- Quantidade\n";

                // Exibe o alerta para o usuário
                await DisplayAlert("Campos Faltando", missingFieldsMessage, "OK");
            }
            else
            {
                // Cria um novo produto e adiciona à lista se todos os campos estiverem preenchidos
                var produto = new Product
                {
                    Codigo = cod,
                    Descricao = descricao,
                    Valor = valor,
                    Quantidade = quantidade,
                    Versao_Peca = versao_peca
                };

                // Adiciona o produto à lista de produtos selecionados
                ListaSelecionados.Add(produto);

                // Atualiza a lista de produtos selecionados na interface
                listaProdutosSelect.ItemsSource = null; // Reset the ItemsSource
                listaProdutosSelect.ItemsSource = ListaSelecionados; // Set the updated list

                // Atualiza o Picker se necessário
                if (valores.SelectedItem != null)
                {
                    // Força a atualização dos dados do Picker
                    OnPickerSelectionChangedPrice(valores, EventArgs.Empty);
                }
            }
            CalValorTotal();
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            var AllItems = listaProdutosSelect.ItemsSource.Cast<Product>().ToList();
            int novoNumeroPedido = await GetProximoNumeroPedidoAsync();
            if (listaProdutosSelect.ItemsSource != null && listaProdutosSelect.ItemsSource.Cast<Product>().Any())
            {
                foreach (var product in listaProdutosSelect.ItemsSource.Cast<Product>())
                {
                    var novoPedido = new Pedido
                    {
                        NumeroPedido = novoNumeroPedido,
                        Codigo = product.Codigo,
                        Descricao = product.Descricao,
                        Valor = product.Valor,
                        Quantidade = product.Quantidade,
                        VersaoPeca = product.Versao_Peca,
                        DataPedido = DateTime.Now
                    };

                    await App.Database.SalvarPedidoAsync(novoPedido);
                }
                foreach (var item in AllItems)
                {
                    ListaSelecionados.Remove(item);
                }
                // Atualiza a CollectionView e limpa a seleção
                listaProdutosSelect.SelectedItems.Clear(); // Limpa a seleção
                listaProdutosSelect.ItemsSource = null;
                listaProdutosSelect.ItemsSource = ListaSelecionados;
            }
            await GetProximoNumeroPedidoAsync();
        }
     private async void OnCopiarClicked(object sender, EventArgs e)
{
    var service = new CopiarPedidoService();
    await service.CopiarTextoAsync(
        txtVendedor.Text,
        pedido.SelectedItem?.ToString(),
        TipoFrete.SelectedItem?.ToString(),
        pag.SelectedItem?.ToString(),
        txtFrete.Text,
        txtFaturamento.Text,
        txtDefeitos.Text,
        txtNS.Text,
        txtnota.Text,
        txtChaveNotaExterna.Text,
        nota.SelectedItem,
        pedido.SelectedItem,
        listaProdutosSelect,
        btncopy,
        iconCopy
    );
}
        private decimal CalValorTotal()
        {
            decimal frete = 0m;
            bool isFreteParsed = !string.IsNullOrEmpty(txtFrete.Text) &&
                                 decimal.TryParse(txtFrete.Text.Replace("R$", "").Trim().Replace(".", ","), out frete);
            decimal totalGeral = 0m;

            if (listaProdutosSelect.ItemsSource != null)
            {
                foreach (var product in listaProdutosSelect.ItemsSource.Cast<Product>())
                {
                    var valorUnidade = decimal.TryParse(product.Valor, out var val) ? val : 0m;
                    var quantidade = decimal.TryParse(product.Quantidade, out var qnt) ? qnt : 0m;
                    totalGeral += valorUnidade * quantidade;
                }
            }
            // Adiciona o frete apenas uma vez, fora do loop
            if (isFreteParsed)
            {
                totalGeral += frete;
            }
            MeuBudget.Valor_Total = $"{totalGeral:F2}";
            return totalGeral;
        }
        private void OnVerificarSelecoesClicked(object sender, EventArgs e)
        {
            // Obtém as seleções de cada Picker
            var pickerPedido = pedido.SelectedItem as string;
            var pickerNota = nota.SelectedItem as string;
            var pickerPagamento = pag.SelectedItem as string;

            // Determina os estados com base nas seleções
            bool isVenda = pickerPedido == "Venda";
            bool isPix = pickerPagamento == "PIX";
            bool isGarantia = pickerPedido == "Garantia com retorno" || pickerPedido == "Garantia sem retorno";
            bool isNotaExterna = pickerNota == "Nota Externa" && isGarantia;
            bool isNotaInterna = pickerNota == "Nota Interna" && isGarantia;
            // Atualiza a visibilidade com base nos estados calculados
            SetVisibility(txtFaturamento, isVenda && !isPix);
            SetVisibility(txtChaveNotaExterna, isNotaExterna);
            SetVisibility(txtDefeitos, !isVenda);
            SetVisibility(txtNS, !isVenda);
            SetVisibility(typeNota, !isVenda);
            SetVisibility(nota, !isVenda);
            SetVisibility(txtnota, !isVenda);
            SetVisibility(pag, !isGarantia);
            SetVisibility(txtpag, !isGarantia);
            SetVisibility(txtFrete, !isGarantia);
            SetVisibility(TipoFrete, !isGarantia);
            SetVisibility(secaofrete, !isGarantia);
        }
        /// <summary>
        /// Define a visibilidade de um elemento de forma centralizada.
        /// </summary>
        /// <param name="control">O controle cuja visibilidade será alterada.</param>
        /// <param name="isVisible">True para visível, False para invisível.</param>
        private void SetVisibility(View control, bool isVisible)
        {
            if (control != null)
            {
                control.IsVisible = isVisible;
            }
        }

    }
}
