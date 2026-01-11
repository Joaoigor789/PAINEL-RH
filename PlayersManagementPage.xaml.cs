using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RH_STAFF
{
    public partial class PlayersManagementPage : Page
    {
        private List<Player> players = new List<Player>();
        private Player selectedPlayer;

        public PlayersManagementPage()
        {
            InitializeComponent();
            InitializePlayers();
            UpdateStats();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            btnAddPlayer.Click += BtnAddPlayer_Click;
            btnEditPlayer.Click += BtnEditPlayer_Click;
            btnDeletePlayer.Click += BtnDeletePlayer_Click;
            btnExportData.Click += BtnExportData_Click;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            dgPlayers.SelectionChanged += DgPlayers_SelectionChanged;
        }

        private void InitializePlayers()
        {
            players.Clear();

            players.Add(new Player { Id = 1, Nickname = "PlayerOne", RealName = "João Silva", Age = 25, Country = "Brasil", Status = "Ativo", RegistrationDate = DateTime.Now.AddDays(-30), LastAccess = DateTime.Now.AddHours(-2) });
            players.Add(new Player { Id = 2, Nickname = "DarkKnight", RealName = "Maria Santos", Age = 22, Country = "Portugal", Status = "Inativo", RegistrationDate = DateTime.Now.AddDays(-60), LastAccess = DateTime.Now.AddDays(-10) });
            players.Add(new Player { Id = 3, Nickname = "Shadow", RealName = "Carlos Oliveira", Age = 30, Country = "Brasil", Status = "Banido", RegistrationDate = DateTime.Now.AddDays(-90), LastAccess = DateTime.Now.AddDays(-15) });
            players.Add(new Player { Id = 4, Nickname = "LightWarrior", RealName = "Ana Costa", Age = 28, Country = "EUA", Status = "Ativo", RegistrationDate = DateTime.Now.AddDays(-45), LastAccess = DateTime.Now.AddHours(-1) });

            dgPlayers.ItemsSource = players;
        }

        private void UpdateStats()
        {
            int total = players.Count;
            int selected = dgPlayers.SelectedItems.Count;
            int active = players.Count(p => p.Status == "Ativo");
            txtStats.Text = $"Total de Players: {total} | Selecionados: {selected} | Ativos: {active}";
        }

        private void BtnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new PlayerDialog();
                if (dialog.ShowDialog() == true)
                {
                    var newPlayer = dialog.GetPlayer();
                    newPlayer.Id = players.Count > 0 ? players.Max(p => p.Id) + 1 : 1;
                    players.Add(newPlayer);
                    dgPlayers.Items.Refresh();
                    UpdateStats();
                    MessageBox.Show("Player adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar player: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPlayer != null)
            {
                try
                {
                    var dialog = new PlayerDialog(selectedPlayer);
                    if (dialog.ShowDialog() == true)
                    {
                        var updatedPlayer = dialog.GetPlayer();
                        updatedPlayer.Id = selectedPlayer.Id;
                        updatedPlayer.RegistrationDate = selectedPlayer.RegistrationDate;
                        var index = players.IndexOf(selectedPlayer);
                        if (index != -1)
                        {
                            players[index] = updatedPlayer;
                            dgPlayers.Items.Refresh();
                            UpdateStats();
                            MessageBox.Show("Player atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao editar player: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione um player para editar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDeletePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPlayer != null)
            {
                var result = MessageBox.Show($"Deseja realmente excluir o player '{selectedPlayer.Nickname}'?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        players.Remove(selectedPlayer);
                        dgPlayers.Items.Refresh();
                        UpdateStats();
                        MessageBox.Show("Player excluído com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao excluir player: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um player para excluir.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExportData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Arquivos CSV (*.csv)|*.csv|Todos os arquivos (*.*)|*.*",
                    FileName = $"players_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    Title = "Exportar dados para CSV"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportToCsv(saveFileDialog.FileName);
                    MessageBox.Show($"Dados exportados com sucesso!\nArquivo: {saveFileDialog.FileName}", "Exportação Concluída", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar dados:\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToCsv(string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("ID;Nickname;Nome Real;Idade;País;Status;Data Cadastro;Último Acesso");
                foreach (var player in players)
                {
                    writer.WriteLine($"{player.Id};{player.Nickname};{player.RealName};{player.Age};{player.Country};{player.Status};{player.RegistrationDate:dd/MM/yyyy};{player.LastAccess:dd/MM/yyyy HH:mm}");
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var searchText = txtSearch.Text.ToLower();
                if (searchText == "buscar player...")
                {
                    dgPlayers.ItemsSource = players;
                }
                else if (string.IsNullOrWhiteSpace(searchText))
                {
                    dgPlayers.ItemsSource = players;
                }
                else
                {
                    var filteredPlayers = players.Where(p =>
                        (p.Nickname?.ToLower().Contains(searchText) ?? false) ||
                        (p.RealName?.ToLower().Contains(searchText) ?? false) ||
                        (p.Country?.ToLower().Contains(searchText) ?? false) ||
                        (p.Status?.ToLower().Contains(searchText) ?? false)).ToList();
                    dgPlayers.ItemsSource = filteredPlayers;
                }
                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na busca: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPlayer = dgPlayers.SelectedItem as Player;
            UpdateStats();
        }
    }

    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Ativo" => Brushes.Green,
                    "Inativo" => Brushes.Red,
                    "Banido" => Brushes.Orange,
                    "Suspenso" => Brushes.Yellow,
                    _ => Brushes.Gray
                };
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}