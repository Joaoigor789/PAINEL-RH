using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RH_STAFF
{
    public partial class DashboardPage : Page
    {
        private DispatcherTimer refreshTimer;
        private Random random = new Random();
        private List<Player> players = new List<Player>();

        public DashboardPage()
        {
            InitializeComponent();
            InitializeDashboard();
            SetupEventHandlers();
            StartAutoRefresh();
        }

        private void SetupEventHandlers()
        {
            btnAddPlayer.Click += BtnAddPlayer_Click;
            btnGenerateReport.Click += BtnGenerateReport_Click;
            btnExportData.Click += BtnExportData_Click;
            btnBackup.Click += BtnBackup_Click;
        }

        private void InitializeDashboard()
        {
            LoadSampleData();
            UpdateDashboardStats();
        }

        private void LoadSampleData()
        {
            players.Clear();
            
            players.Add(new Player { Id = 1, Nickname = "PlayerOne", RealName = "João Silva", Age = 25, Country = "Brasil", Status = "Ativo", RegistrationDate = DateTime.Now.AddHours(-2), LastAccess = DateTime.Now });
            players.Add(new Player { Id = 2, Nickname = "DarkKnight", RealName = "Maria Santos", Age = 22, Country = "Portugal", Status = "Inativo", RegistrationDate = DateTime.Now.AddDays(-1), LastAccess = DateTime.Now.AddHours(-5) });
            players.Add(new Player { Id = 3, Nickname = "Shadow", RealName = "Carlos Oliveira", Age = 30, Country = "Brasil", Status = "Banido", RegistrationDate = DateTime.Now.AddDays(-2), LastAccess = DateTime.Now.AddDays(-1) });
            players.Add(new Player { Id = 4, Nickname = "LightWarrior", RealName = "Ana Costa", Age = 28, Country = "EUA", Status = "Ativo", RegistrationDate = DateTime.Now.AddHours(-1), LastAccess = DateTime.Now });
            players.Add(new Player { Id = 5, Nickname = "NightWolf", RealName = "Pedro Almeida", Age = 19, Country = "Brasil", Status = "Ativo", RegistrationDate = DateTime.Now.AddHours(-3), LastAccess = DateTime.Now });
            players.Add(new Player { Id = 6, Nickname = "FireDragon", RealName = "Luís Pereira", Age = 32, Country = "Portugal", Status = "Ativo", RegistrationDate = DateTime.Now.AddHours(-4), LastAccess = DateTime.Now });
            players.Add(new Player { Id = 7, Nickname = "IceQueen", RealName = "Carla Rodrigues", Age = 27, Country = "Brasil", Status = "Inativo", RegistrationDate = DateTime.Now.AddDays(-3), LastAccess = DateTime.Now.AddDays(-1) });
            players.Add(new Player { Id = 8, Nickname = "ThunderGod", RealName = "Rafael Souza", Age = 24, Country = "EUA", Status = "Banido", RegistrationDate = DateTime.Now.AddDays(-4), LastAccess = DateTime.Now.AddDays(-2) });

            LoadRecentPlayers();
        }

        private void UpdateDashboardStats()
        {
            int total = players.Count;
            int active = players.Count(p => p.Status == "Ativo");
            int banned = players.Count(p => p.Status == "Banido");
            int inactive = players.Count(p => p.Status == "Inativo");
            
            int playersToday = players.Count(p => p.RegistrationDate.Date == DateTime.Today);
            int staffOnline = random.Next(3, 8);

            txtTotalPlayers.Text = total.ToString();
            txtActivePlayers.Text = active.ToString();
            txtBannedPlayers.Text = banned.ToString();
            txtStaffOnline.Text = staffOnline.ToString();
            
            txtAtivoCount.Text = $"{active} players";
            txtInativoCount.Text = $"{inactive} players";
            txtBanidoCount.Text = $"{banned} players";
            
            txtPlayersToday.Text = $"{playersToday} novos";
            txtLastBackup.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            
            UpdateSystemStatus();
        }

        private void UpdateSystemStatus()
        {
            string status = "🟢 Operacional";
            string color = "#27AE60";
            
            if (players.Count == 0)
            {
                status = "🟡 Sem Dados";
                color = "#F39C12";
            }
            else if (players.Count(p => p.Status == "Banido") > 5)
            {
                status = "🟡 Alta de Banimentos";
                color = "#F39C12";
            }
            
            txtSystemStatus.Text = status;
            txtSystemStatus.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString(color);
        }

        private void LoadRecentPlayers()
        {
            var recentPlayers = players
                .OrderByDescending(p => p.RegistrationDate)
                .Take(5)
                .Select(p => new PlayerInfo 
                { 
                    Nickname = p.Nickname, 
                    RegistrationDate = p.RegistrationDate, 
                    Status = p.Status 
                })
                .ToList();

            dgRecentPlayers.ItemsSource = recentPlayers;
        }

        private void StartAutoRefresh()
        {
            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(5);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            UpdateDashboardStats();
            LoadRecentPlayers();
            
            if (random.Next(0, 20) == 0)
            {
                AddRandomPlayer();
            }
        }

        private void AddRandomPlayer()
        {
            string[] names = { "NovoJogador", "GamerPro", "PlayerX", "SuperStar", "EliteWarrior" };
            string[] countries = { "Brasil", "Portugal", "EUA", "Espanha", "Argentina" };
            string[] statuses = { "Ativo", "Ativo", "Ativo", "Inativo" };
            
            var newPlayer = new Player
            {
                Id = players.Count + 1,
                Nickname = $"{names[random.Next(names.Length)]}{random.Next(1000)}",
                RealName = "Novo Usuário",
                Age = random.Next(18, 40),
                Country = countries[random.Next(countries.Length)],
                Status = statuses[random.Next(statuses.Length)],
                RegistrationDate = DateTime.Now,
                LastAccess = DateTime.Now
            };
            
            players.Add(newPlayer);
        }

        private void BtnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PlayerDialog();
            if (dialog.ShowDialog() == true)
            {
                var newPlayer = dialog.GetPlayer();
                newPlayer.Id = players.Count > 0 ? players.Max(p => p.Id) + 1 : 1;
                players.Add(newPlayer);
                UpdateDashboardStats();
                LoadRecentPlayers();
                MessageBox.Show("Player adicionado via Dashboard!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string report = $"RELATÓRIO DO SISTEMA - {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
                          $"===============================\n" +
                          $"Total de Players: {players.Count}\n" +
                          $"Ativos: {players.Count(p => p.Status == "Ativo")}\n" +
                          $"Inativos: {players.Count(p => p.Status == "Inativo")}\n" +
                          $"Banidos: {players.Count(p => p.Status == "Banido")}\n" +
                          $"Players hoje: {players.Count(p => p.RegistrationDate.Date == DateTime.Today)}\n" +
                          $"Último backup: {txtLastBackup.Text}";
            
            MessageBox.Show(report, "Relatório Gerado", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnExportData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Arquivos CSV (*.csv)|*.csv",
                    FileName = $"dashboard_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var writer = new System.IO.StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.UTF8))
                    {
                        writer.WriteLine("Nickname;Nome Real;Idade;País;Status;Data Cadastro;Último Acesso");
                        foreach (var player in players)
                        {
                            writer.WriteLine($"{player.Nickname};{player.RealName};{player.Age};{player.Country};{player.Status};{player.RegistrationDate:dd/MM/yyyy HH:mm};{player.LastAccess:dd/MM/yyyy HH:mm}");
                        }
                    }
                    
                    MessageBox.Show($"Dados exportados com sucesso!\n{saveFileDialog.FileName}", "Exportação Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBackup_Click(object sender, RoutedEventArgs e)
        {
            txtLastBackup.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            MessageBox.Show("Backup do sistema realizado com sucesso!", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public class PlayerInfo
        {
            public string Nickname { get; set; }
            public DateTime RegistrationDate { get; set; }
            public string Status { get; set; }
        }
    }
}