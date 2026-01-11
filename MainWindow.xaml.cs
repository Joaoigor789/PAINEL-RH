using System;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RH_STAFF
{
    public partial class MainWindow : Window
    {
        private bool isLoggedIn = false;
        private string currentUser = "";
        private string currentRole = "";
        private readonly string[] ALLOWED_IPS = new string[]
        {
        "seu ip",
        "",
        ""
        };
        private bool ipChecked = false;

        public MainWindow()
        {
            InitializeComponent();

            // Aguardar o carregamento completo do XAML
            this.Loaded += OnWindowLoaded;
        }

        

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnWindowLoaded;

           
            
            if (!CheckAndValidateIP())
            {
                return;
            }

            InitializeLoginState();
            SetupEventHandlers();
        }

        private bool CheckAndValidateIP()
        {
            string currentIP = GetLocalIPAddress();
            MessageBox.Show($"Seu IP detectado: {currentIP}",
                "Verificação de IP", MessageBoxButton.OK, MessageBoxImage.Information);

            // Verifica se o IP atual está na lista de IPs permitidos
            if (!ALLOWED_IPS.Contains(currentIP))
            {
                ShowIPBlockMessage(currentIP);
                ipChecked = false;
                return false;
            }

            ipChecked = true;
            return true;
        }

        private string GetLocalIPAddress()
        {
            try
            {
                // Método 1: Usando DNS para obter IP local
                string hostName = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);

                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.ToString() != "127.0.0.1") // Ignorar localhost
                        {
                            return address.ToString();
                        }
                    }
                }

                // Método 2: Tentar obter IP da interface de rede ativa
                foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (networkInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                if (!IPAddress.IsLoopback(ip.Address) && !ip.Address.ToString().StartsWith("169.254"))
                                {
                                    return ip.Address.ToString();
                                }
                            }
                        }
                    }
                }

                return "IP não encontrado";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao obter IP: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return "Erro";
            }
        }

        private void ShowIPBlockMessage(string currentIP)
        {
            // Esconder tudo primeiro
            loginPanel.Visibility = Visibility.Collapsed;
            menuPanel.Visibility = Visibility.Collapsed;
            welcomePanel.Visibility = Visibility.Collapsed;
            MainContentFrame.Visibility = Visibility.Collapsed;

            // Criar painel de bloqueio
            Grid blockedGrid = new Grid();

            Border blockedBorder = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.Red,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Width = 500,
                Height = 300,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            StackPanel blockedPanel = new StackPanel
            {
                Margin = new Thickness(20)
            };

            // Ícone de bloqueio
            TextBlock icon = new TextBlock
            {
                Text = "⛔",
                FontSize = 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            TextBlock title = new TextBlock
            {
                Text = "ACESSO BLOQUEADO",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            TextBlock ipInfo = new TextBlock
            {
                Text = $"Seu IP: {currentIP}\nIPs Permitidos:\n{string.Join("\n", ALLOWED_IPS)}", // ← CORRIGIDO
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                Foreground = System.Windows.Media.Brushes.DarkRed,
                Margin = new Thickness(0, 0, 0, 20)
            };

            TextBlock message = new TextBlock
            {
                Text = "Este sistema só pode ser acessado de um endereço IP específico.\n\nContate o administrador do sistema para mais informações.",
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                Foreground = System.Windows.Media.Brushes.Gray,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 30)
            };

            Button exitButton = new Button
            {
                Content = "SAIR DO SISTEMA",
                Background = System.Windows.Media.Brushes.Red,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(30, 10, 30, 10),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Cursor = Cursors.Hand,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            exitButton.Click += (s, e) =>
            {
                MessageBox.Show("Sistema encerrado por restrição de IP.", "Acesso Negado",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                Application.Current.Shutdown();
            };

            blockedPanel.Children.Add(icon);
            blockedPanel.Children.Add(title);
            blockedPanel.Children.Add(ipInfo);
            blockedPanel.Children.Add(message);
            blockedPanel.Children.Add(exitButton);

            blockedBorder.Child = blockedPanel;
            blockedGrid.Children.Add(blockedBorder);

            // Substituir o conteúdo principal
            Content = blockedGrid;

            // Desabilitar botões
            btnLogin.IsEnabled = false;
            btnDashboard.IsEnabled = false;
            btnPlayers.IsEnabled = false;
            btnStaff.IsEnabled = false;
        }

        private void InitializeLoginState()
        {
            if (!ipChecked) return;

            loginPanel.Visibility = Visibility.Visible;
            menuPanel.Visibility = Visibility.Collapsed;
            welcomePanel.Visibility = Visibility.Visible;
            MainContentFrame.Visibility = Visibility.Collapsed;
            txtLoginStatus.Text = "Não logado - IP Verificado";
            txtPageTitle.Text = "Sistema de Staff";

            txtUsername.Focus();
        }

        private void SetupEventHandlers()
        {
            if (!ipChecked) return;

            btnLogin.Click += BtnLogin_Click;
            btnLogout.Click += BtnLogout_Click;
            btnDashboard.Click += BtnDashboard_Click;
            btnPlayers.Click += BtnPlayers_Click;
            btnStaff.Click += BtnStaff_Click;
            btnTelador.Click += BtnTelador_Click;
            btnReports.Click += BtnReports_Click;
            btnSettings.Click += BtnSettings_Click;

            txtPassword.KeyDown += TxtPassword_KeyDown;
            txtUsername.KeyDown += TxtUsername_KeyDown;
        }

        private void TxtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ipChecked)
            {
                txtPassword.Focus();
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ipChecked)
            {
                BtnLogin_Click(null, null);
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!ipChecked)
            {
                MessageBox.Show("Sistema bloqueado por restrição de IP.", "Acesso Negado",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verificar IP novamente antes do login
            string currentIP = GetLocalIPAddress();
            if (!ALLOWED_IPS.Contains(currentIP)) // ← CORRIGIDO
            {
                MessageBox.Show($"Acesso negado! IP não autorizado.\n\nSeu IP: {currentIP}\nIPs Permitidos: {string.Join(", ", ALLOWED_IPS)}",
                    "IP Bloqueado", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowIPBlockMessage(currentIP);
                return;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowLoginError("Preencha usuário e senha");
                return;
            }

            if (AuthenticateUser(username, password))
            {
                LoginSuccessful(username);
            }
            else
            {
                ShowLoginError("Usuário ou senha incorretos");
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            var validUsers = new System.Collections.Generic.Dictionary<string, string>
            {
                { "admin", "admin123" },
                { "doxy", "Telador@123" },
                { "staff", "staff123" },
                { "moderador", "mod123" },
                { "suporte", "sup123" }
            };

            if (validUsers.ContainsKey(username.ToLower()))
            {
                return validUsers[username.ToLower()] == password;
            }

            return false;
        }

        private string GetUserRole(string username)
        {
            return username.ToLower() switch
            {
                "admin" => "Administrador",
                "staff" => "Staff",
                "telador" => "Telador",
                "moderador" => "Moderador",
                "suporte" => "Suporte Técnico",
                _ => "Usuário"
            };
        }

        private void LoginSuccessful(string username)
        {
            if (!ipChecked) return;

            isLoggedIn = true;
            currentUser = username;
            currentRole = GetUserRole(username);

            txtCurrentUser.Text = currentUser;
            txtUserRole.Text = currentRole;
            txtLoginStatus.Text = $"Logado como: {currentUser} | IP: {GetLocalIPAddress()}";
            txtLoginStatus.Foreground = System.Windows.Media.Brushes.Green;

            loginPanel.Visibility = Visibility.Collapsed;
            menuPanel.Visibility = Visibility.Visible;
            welcomePanel.Visibility = Visibility.Collapsed;
            MainContentFrame.Visibility = Visibility.Visible;
            txtPageTitle.Text = "Dashboard";

            txtUsername.Text = "";
            txtPassword.Password = "";
            txtLoginError.Visibility = Visibility.Collapsed;

            LoadDashboard();
        }

        private void ShowLoginError(string message)
        {
            txtLoginError.Text = message;
            txtLoginError.Visibility = Visibility.Visible;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Deseja realmente sair do sistema?",
                "Confirmar Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Logout();
            }
        }

        private void Logout()
        {
            isLoggedIn = false;
            currentUser = "";
            currentRole = "";

            loginPanel.Visibility = Visibility.Visible;
            menuPanel.Visibility = Visibility.Collapsed;
            welcomePanel.Visibility = Visibility.Visible;
            MainContentFrame.Visibility = Visibility.Collapsed;
            txtLoginStatus.Text = $"Não logado | IP: {GetLocalIPAddress()}";
            txtLoginStatus.Foreground = System.Windows.Media.Brushes.Red;
            txtPageTitle.Text = "Sistema de Staff";

            MainContentFrame.Navigate(null);
            txtUsername.Focus();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn || !ipChecked) return;

            if (!VerifyCurrentIP())
                return;

            LoadDashboard();
        }

        private void BtnPlayers_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn || !ipChecked) return;

            if (!VerifyCurrentIP())
                return;

            try
            {
                var playersPage = new PlayersManagementPage();
                MainContentFrame.Navigate(playersPage);
                txtPageTitle.Text = "Gerenciamento de Players";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar gerenciamento de players: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnStaff_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn || !ipChecked) return;

            if (!VerifyCurrentIP())
                return;

            try
            {
                var staffPage = new StaffManagementPage();
                MainContentFrame.Navigate(staffPage);
                txtPageTitle.Text = "Gerenciamento de Staff";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar gerenciamento de staff: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnTelador_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn || !ipChecked) return;
    
            if (!VerifyCurrentIP())
                return;
    
            try
            {
                var teladorPage = new TeladorPage();
                MainContentFrame.Navigate(teladorPage);
                txtPageTitle.Text = "Área do Telador";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar área do telador: {ex.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn || !ipChecked) return;

            if (!VerifyCurrentIP())
                return;

            MessageBox.Show("Funcionalidade de Relatórios em desenvolvimento.", "Em Breve",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedIn || !ipChecked) return;

            if (!VerifyCurrentIP())
                return;

            MessageBox.Show("Funcionalidade de Configurações em desenvolvimento.", "Em Breve",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool VerifyCurrentIP()
        {
            string currentIP = GetLocalIPAddress();
            if (!ALLOWED_IPS.Contains(currentIP)) // ← CORRIGIDO
            {
                MessageBox.Show($"⚠️ ALERTA DE SEGURANÇA ⚠️\n\nIP alterado durante a sessão!\n\nIP Atual: {currentIP}\nIPs Permitidos: {string.Join(", ", ALLOWED_IPS)}\n\nVocê será desconectado por motivos de segurança.",
                    "Violação de Segurança", MessageBoxButton.OK, MessageBoxImage.Error);
                Logout();
                ShowIPBlockMessage(currentIP);
                return false;
            }
            return true;
        }

        private void LoadDashboard()
        {
            if (!isLoggedIn || !ipChecked) return;

            if (!VerifyCurrentIP())
                return;

            try
            {
                var dashboardPage = new DashboardPage();
                MainContentFrame.Navigate(dashboardPage);
                txtPageTitle.Text = "Dashboard";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dashboard: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}