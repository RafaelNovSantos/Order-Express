using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace Gerador_de_Pedidos
{
    public partial class AppShell : Shell
    {
        private const string LicenseUrl = "https://raw.githubusercontent.com/RafaelNovSantos/CuoraConnect/master/CuoraConnect/Licenca/activeLicense.xml";
        public bool IsLicenseValid { get; private set; } = false;
        public bool IsConnectedInternet { get; private set; } = false;

        private readonly HttpClient _httpClient = new HttpClient();

        public AppShell()
        {
            InitializeComponent();

            // Chamar método assíncrono sem bloquear o construtor
            _ = InitializeAppAsync();
        }

        private async Task InitializeAppAsync()
        {
            await CheckLicenseAsync();
            ConfigureUI();
        }

        private void ConfigureUI()
        {
            Debug.WriteLine($"IsConnectedInternet: {IsConnectedInternet}, IsLicenseValid: {IsLicenseValid}");

            if (IsConnectedInternet && IsLicenseValid)
            {
#if ANDROID
                this.FlyoutBehavior = FlyoutBehavior.Disabled; // Desativa o menu no Android
                PageHistorico.Route = "Historico.HistoricoPageAndroid";
                PageHistorico.ContentTemplate = new DataTemplate(typeof(Historico.HistoricoPageAndroid));
                MainPageAndroidContent.IsVisible = true; // Mostra MainPageAndroid
                CurrentItem = MainPageAndroidContent; // Define como página inicial
                PageGarantia.IsVisible = false;
#else
                PageHistorico.Route = "Historico.HistoricoPage";
                PageHistorico.ContentTemplate = new DataTemplate(typeof(Historico.HistoricoPage));
                MainPageContent.IsVisible = true; // Mostra MainPage para outras plataformas
                CurrentItem = MainPageContent; // Define como página inicial
#endif
            }
            else
            {
                this.FlyoutBehavior = FlyoutBehavior.Disabled; // Desativa o menu no Android
                PagePlanilha.IsVisible = false;
                PageHistorico.IsVisible = false;
                MainPageContent.IsVisible = false; // Oculta MainPage para outras plataformas
                MainPageAndroidContent.IsVisible = false; // Oculta MainPageAndroid
                PageGarantia.IsVisible = false;
            }
        }

        public async Task CheckLicenseAsync()
        {
            IsConnectedInternet = await CheckInternetConnectionAsync();

            if (IsConnectedInternet)
            {
                await CheckLicenseValidityAsync();
            }
        }

        private async Task<bool> CheckInternetConnectionAsync()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync("google.com");

                    if (reply.Status == IPStatus.Success)
                    {
                        Debug.WriteLine($"Ping para google.com bem-sucedido! Tempo: {reply.RoundtripTime}ms");
                        return true;
                    }
                    else
                    {
                        Debug.WriteLine($"Falha no ping: {reply.Status}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao verificar a conexão: {ex.Message}");
                return false;
            }
        }

        private async Task CheckLicenseValidityAsync()
        {
            try
            {
                string urlWithTimestamp = $"{LicenseUrl}?_={DateTime.Now.Ticks}";
                string xmlContent = await _httpClient.GetStringAsync(urlWithTimestamp);
                xmlContent = xmlContent.Trim();

                XDocument xdoc = XDocument.Parse(xmlContent);
                var activeValue = xdoc.Root?.Attribute("value")?.Value;

                IsLicenseValid = activeValue?.ToLower() == "true";
                Debug.WriteLine($"Licença válida: {IsLicenseValid}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao acessar o XML: {ex.Message}");
            }
        }
    }
}
