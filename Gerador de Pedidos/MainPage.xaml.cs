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
using static System.Net.WebRequestMethods;



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

            LoadLink();

        }
        public List<Produto> Lista = new List<Produto>();
        public List<Produto> ListaSelecionados = new List<Produto>();

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
                    linkplanilha = "https://docs.google.com/spreadsheets/d/197z0M4GrqaY3Kl6BvtEgGt-tYMX4IdpcW2dm8Ze5bZQ/export?usp=sharing";
                    OnPickerSelectionChangedPrice(valores, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao ler o link: {ex.Message}", "OK");
                linkplanilha = "https://docs.google.com/spreadsheets/d/197z0M4GrqaY3Kl6BvtEgGt-tYMX4IdpcW2dm8Ze5bZQ/export?usp=sharing";
                OnPickerSelectionChangedPrice(valores, EventArgs.Empty);

            }
        }

        private async void OnAlterarLinkClicked(object sender, EventArgs e)
        {
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
                    OnPickerSelectionChangedPrice(valores, EventArgs.Empty);

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
        }


        private void SelectionChangedCopyCod(object sender, SelectionChangedEventArgs e)
        {



            // Obtém os itens selecionados na CollectionView
            var selectedItems = e.CurrentSelection.Cast<Produto>().ToList();

            if (selectedItems.Count > 0)
            {
                // Assume que seleciona apenas um item
                var selectedItem = selectedItems.FirstOrDefault();

                if (selectedItem != null && (lblStatusProduto.Text == "Produto Encontrado" || lblStatusProduto.Text == "Digite o Código..." || lblStatusProduto.Text == "Produto Não Encontrado"))
                {
                    txtCodigo.Text = null;
                    // Atualiza o txtCodigo com o código do produto selecionado
                    txtCodigo.Text = selectedItem.Codigo;
                    listaProdutosExcel.SelectedItems.Clear(); // Limpa a seleção
                    listaProdutosExcel.SelectedItem = null;
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

            if (!string.IsNullOrEmpty(e.NewTextValue))
            {

                if (e.NewTextValue.Length > 9)
                {
                    DisplayAlert("Limite de Dígitos Excedido", "O código não pode ter mais que 9 dígitos.", "OK");
                    entry.Text = e.NewTextValue.Substring(0, 9); // Trunca o texto para 10 dígitos
                    return;
                }
                // Validação numérica
                if (!int.TryParse(((e.NewTextValue).Replace("-", "").Replace("M", "").Replace("m", "")), out _))
                {
                    DisplayAlert("Entrada Inválida", "Por favor, insira apenas os números do código.", "OK");
                    entry.Text = e.OldTextValue;
                    return; // Retorna para evitar executar a próxima lógica caso o texto não seja numérico.
                }

                // Verificação do limite de 10 dígitos

            }


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
                    btnBuscarProduto.IsVisible = false; // Ocultar o botão se houve erro
                    lblStatusProduto.IsVisible = true; // Mostrar o texto de status se houve erro
                    lblStatusProduto.Text = "Tipo de valor não selecionado.";
                    lblStatusProduto.TextColor = Color.FromHex("#FF0000"); // Vermelho para indicar erro
                    return;
            }

            await LerExcelComColuna(linkplanilha, selectedSheetName, columnToUse);


            if (!string.IsNullOrEmpty(selectedSheetName))
            {
                await LerExcelComColuna(linkplanilha, selectedSheetName, columnToUse);
            }
        }



        private async Task ProcessarSelecao(string selectedValue)
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
                    btnBuscarProduto.IsVisible = false; // Ocultar o botão se houve erro
                    lblStatusProduto.IsVisible = true; // Mostrar o texto de status se houve erro
                    lblStatusProduto.Text = "Tipo de valor não selecionado.";
                    lblStatusProduto.TextColor = Color.FromHex("#FF0000"); // Vermelho para indicar erro
                    return;
            }

            // Recarregar os dados da planilha usando a coluna correta
           

            string cod = txtCodigo.Text;

            if (!string.IsNullOrEmpty(cod))
            {
                cod = cod.ToUpper();
                // Buscar o item correspondente na lista
                var item = Lista.FirstOrDefault(i => i.Codigo == cod);
                if (item != null)
                {
                    txtDescricao.Text = item.Descricao;

                    // Converter item.Valor para decimal e formatar com duas casas decimais
                    if ((decimal.TryParse(((item.Valor).Replace("R", "").Replace("$", "")), out decimal valorNumerico)))
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
                    lblStatusProduto.FontSize = 17;
                    lblStatusProduto.TextColor = Color.FromHex("#00FF00"); // Verde para indicar sucesso
                }
                else
                {
                    txtDescricao.Text = string.Empty;
                    txtValor.Text = string.Empty;
                    lblStatusProduto.Text = "Produto Não Encontrado";
                    lblStatusProduto.FontSize = 13;
                    lblStatusProduto.TextColor = Color.FromHex("#FF0000"); // Vermelho para indicar erro
                }
            }
            else
            {
                txtDescricao.Text = string.Empty;
                txtValor.Text = string.Empty;
                lblStatusProduto.FontSize = 18;
                lblStatusProduto.Text = "Digite o Código...";
                lblStatusProduto.TextColor = Color.FromHex("#FFFAFF00"); // Laranja para indicar aviso
            }
        }


        private string _ultimoItemSelecionado;

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
            btnBuscarProduto.IsVisible = true;
            lblStatusProduto.IsVisible = true;
            lblStatusProduto.Text = "center";
            lblStatusProduto.TextColor = Color.FromHex("#00000000");

            // Inicia a rotação do ícone e do botão de busca
            btnBuscarProduto.Rotation = 0;
            IconeAnimado.Rotation = 0;

            var rotateIcon = IconeAnimado.RotateTo(360, 1000, Easing.Linear);
            var rotateButton = btnBuscarProduto.RotateTo(360, 1000, Easing.Linear);

            await ProcessarSelecao(selectedValue);

            // Parar a rotação e ocultar os ícones
            IconeAnimado.Rotation = 0;
            btnBuscarProduto.Rotation = 0;
            btnBuscarProduto.IsVisible = false;
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
            refreshList.IsVisible = true;
            refreshList.IsRunning = true;
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
                                    var quantidade = worksheet.Cells[row, 4]?.Text;

                                    if (string.IsNullOrWhiteSpace(codigo) &&
                                        string.IsNullOrWhiteSpace(descricao) &&
                                        string.IsNullOrWhiteSpace(valor) &&
                                        string.IsNullOrWhiteSpace(quantidade))
                                    {
                                        continue;
                                    }

                                    var produto = new Produto
                                    {
                                        Codigo = !string.IsNullOrWhiteSpace(codigo) ? codigo : "N/A",
                                        Descricao = !string.IsNullOrWhiteSpace(descricao) ? descricao : "N/A",
                                        Valor = !string.IsNullOrWhiteSpace(valor) ? valor : "N/A",
                                        Quantidade = !string.IsNullOrWhiteSpace(quantidade) ? quantidade : "N/A"
                                    };

                                    Lista.Add(produto);
                                    linhaVazia = false;
                                }

                                // Se todas as linhas estiverem vazias, adicionar uma linha padrão
                                if (linhaVazia)
                                {
                                    CriarListaComProdutoPadrao();
                                }

                                // Atualizar a CollectionView
                                listaProdutosExcel.ItemsSource = null;
                                listaProdutosExcel.ItemsSource = Lista;
                            }
                        }
                    }

                    break; // Se a operação foi bem-sucedida, sair do loop
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

                    await Task.Delay(5000); // Aguardar 5 segundos antes de reintentar
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                    await DisplayAlert("Erro", "Ocorreu um erro inesperado: " + ex.Message, "OK");
                    CriarListaComProdutoPadrao();
                    break;
                }
            }

            refreshList.IsVisible = false;
            refreshList.IsRunning = false;
            listaProdutosExcel.IsVisible = true;
        }

        void CriarListaComProdutoPadrao()
        {
            Lista.Clear();
            Lista.Add(new Produto
            {
                Codigo = "N/A",
                Descricao = "N/A",
                Valor = "N/A",
                Quantidade = "N/A"
            });

            listaProdutosExcel.ItemsSource = null;
            listaProdutosExcel.ItemsSource = Lista;
        }







       


        private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = e.CurrentSelection.Cast<Produto>().ToList();



        }




        private async void OnEditarClicked(object sender, EventArgs e)
        {
            var selectedItems = listaProdutosSelect.SelectedItems.Cast<Produto>().ToList();

            if (selectedItems.Count == 0)
            {
                await DisplayAlert("Aviso", "Nenhum item selecionado.", "OK");
                return;
            }

            // Mostra o menu de opções para o usuário
            string action = await DisplayActionSheet("Escolha o campo a editar", "Cancelar", null, "Código", "Descrição", "Valor", "Quantidade");

            if (action == "Cancelar")
                return;

            // Solicita o novo valor com base na escolha do usuário
            string newValue = await DisplayPromptAsync("Editar", $"Digite o novo valor para {action}:", "OK", "Cancelar");

            if (string.IsNullOrEmpty(newValue))
                return;

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
                }
            }

            // Atualiza a CollectionView com os itens editados
            listaProdutosSelect.ItemsSource = null;
            listaProdutosSelect.ItemsSource = ListaSelecionados;
        }


        private async void OnExcluirClicked(object sender, EventArgs e)
        {
            var selectedItems = listaProdutosSelect.SelectedItems.Cast<Produto>().ToList();

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
        }




        private async void OnAdicionarClicked(object sender, EventArgs e)
        {
            // Obtém os valores dos campos de entrada
            string cod = txtCodigo.Text;
            string descricao = txtDescricao.Text;
            string valor = txtValor.Text;
            string quantidade = txtQuantidade.Text;

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
                var produto = new Produto
                {
                    Codigo = cod,
                    Descricao = descricao,
                    Valor = valor,
                    Quantidade = quantidade
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
        }



        private async void OnCopiarClicked(object sender, EventArgs e)
        {
            var vendedor = txtVendedor.Text;
            var saida = pedido.SelectedItem?.ToString();
            var tipofrete = TipoFrete.SelectedItem?.ToString();
            var pagamento = pag.SelectedItem?.ToString();

            decimal frete = 0m;
            bool isFreteParsed = !string.IsNullOrEmpty(txtFrete.Text) &&
                                 decimal.TryParse(txtFrete.Text.Replace("R$", "").Trim().Replace(".", ",", StringComparison.InvariantCulture), out frete);

            decimal totalGeral = 0m;
            var texto = $"VENDEDOR: {vendedor}\n";
            texto += $"SAÍDA: {saida}\n\n";

            // Verifica se listaProdutosSelect.ItemsSource não é nulo e contém itens
            if (listaProdutosSelect.ItemsSource != null && listaProdutosSelect.ItemsSource.Cast<Produto>().Any())
            {
                foreach (var product in listaProdutosSelect.ItemsSource.Cast<Produto>())
                {
                    var valorUnidade = decimal.TryParse(product.Valor, out var val) ? val : 0m;
                    var totalProduto = valorUnidade * (decimal.TryParse(product.Quantidade, out var qnt) ? qnt : 0m);
                    totalGeral += totalProduto;

                    texto += $"Cod.: {product.Codigo}\n";
                    texto += $"Desc: {product.Descricao}\n";
                    texto += $"Valor/Un: R$ {valorUnidade:F2}\n";

                    if (decimal.TryParse(product.Quantidade, out var quantidade) && quantidade != 1)
                    {
                        texto += $"Qntd: {product.Quantidade} (R$ {totalProduto:F2})\n\n";
                    }
                    else
                    {
                        texto += $"Qntd: {product.Quantidade}\n\n";
                    }
                }
            }
            else
            {
                await DisplayAlert("Erro", "Adicione algum produto no pedido", "OK");
                return; // Retorna para que o código de cópia não seja executado
            }

            if (pedido.SelectedItem?.ToString() != "Garantia com retorno" && pedido.SelectedItem?.ToString() != "Garantia sem retorno")
            {
                if (isFreteParsed && frete > 0)
                {
                    totalGeral += frete;
                    texto += $"FRETE({tipofrete}): R$ {frete:F2}\n\n";
                }
                else
                {
                    texto += $"Frete a cotar\n\n";
                }

                texto += $"Pagamento: {pagamento}\n";
            }


            if (pedido.SelectedItem?.ToString() == "Venda" && pagamento == "BOLETO")
            {

                texto += $"Faturamento: {txtFaturamento.Text}\n";
            }



            if (pedido.SelectedItem?.ToString() == "Garantia com retorno" || pedido.SelectedItem?.ToString() == "Garantia sem retorno")
            {
                texto += $"DEFEITO: {txtDefeitos.Text}\n\n";
                texto += $"Balança em posse do cliente:\n";
                texto += $"N/S EQUIPAMENTO: {txtNS.Text}\n\n";
                texto += $"{nota.SelectedItem?.ToString()}:\nNº Nota: {txtnota.Text}\n";

                if (nota.SelectedItem?.ToString() == "Nota Externa")
                {
                    texto += $"CHAVE NOTA EXTERNA: {txtChaveNotaExterna.Text}\n";
                }
            }

            texto += $"\nTOTAL VALOR + FRETE = R$ {totalGeral:F2}";

            try
            {
                await Clipboard.SetTextAsync(texto); // Use SetTextAsync para compatibilidade
                btncopy.Text = "Copiado!";
                iconCopy.Color = Color.FromHex("#000000");
                await Task.Delay(10000);
                btncopy.Text = "Copiar";
                iconCopy.Color = Color.FromHex("#FF008000");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Não foi possível copiar o texto para a área de transferência: {ex.Message}", "OK");
            }
        }




        private void OnPedidoSelected(object sender, EventArgs e)
        {
            // Identifica o Picker que disparou o evento
            Picker picker = sender as Picker;

            if (picker != null)
            {
                var selectedValue = picker.SelectedItem.ToString();

                // Verifica qual Picker foi acionado e define a visibilidade do Label correspondente
                if (picker == pedido)
                {
                    bool isVenda = selectedValue == "Venda";
                    bool isGarantia = selectedValue == "Garantia com retorno" || selectedValue == "Garantia sem retorno";

                    // Atualiza a visibilidade dos campos com base no valor selecionado
                    txtDefeitos.Text = "";
                    txtNS.Text = "";
                    txtnota.Text = "";
                    txtFaturamento.Text = "";
                    txtDefeitos.IsVisible = !isVenda;
                    txtNS.IsVisible = !isVenda;
                    typeNota.IsVisible = !isVenda;
                    nota.IsVisible = !isVenda;
                    txtnota.IsVisible = !isVenda;
                    pag.IsVisible = !isGarantia;
                    txtpag.IsVisible = !isGarantia;
                    txtFaturamento.IsVisible = !isGarantia;
                    txtFrete.IsVisible = !isGarantia;
                    TipoFrete.IsVisible = !isGarantia;
                    secaofrete.IsVisible = !isGarantia;
                }
            }
        }


        private void OnNotaSelected(object sender, EventArgs e)
        {
            // Identifica o Picker que disparou o evento
            Picker picker = sender as Picker;

            if (picker != null)
            {
                var selectedValue = picker.SelectedItem.ToString();

                // Verifica qual Picker foi acionado e define a visibilidade do Label correspondente
                if (picker == nota)
                {
                    txtChaveNotaExterna.IsVisible = selectedValue != "Nota Interna";

                }

                // Adicione mais condições se tiver mais pickers
            }
        }


        private void OnPagamentoSelected(object sender, EventArgs e)
        {
            // Identifica o Picker que disparou o evento
            Picker picker = sender as Picker;

            if (picker != null)
            {
                var selectedValue = picker.SelectedItem.ToString();

                if (picker == pag)
                {
                    txtFaturamento.IsVisible = selectedValue != "PIX";


                }

            }

        }




        public class Produto
        {

            
            public string Codigo { get; set; } 
            public string Descricao { get; set; } 
            public string Valor { get; set; } 
            public string Quantidade { get; set; }



        }

    }
}
