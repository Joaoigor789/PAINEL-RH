using System;
using System.Windows;
using System.Windows.Controls;

namespace RH_STAFF
{
    public partial class PlayerDialog : Window
    {
        private Player existingPlayer;

        public PlayerDialog()
        {
            InitializeComponent();
        }

        public PlayerDialog(Player existingPlayer) : this()
        {
            this.existingPlayer = existingPlayer;
            LoadPlayerData();
        }

        private void LoadPlayerData()
        {
            if (existingPlayer != null)
            {
                txtNickname.Text = existingPlayer.Nickname ?? "";
                txtRealName.Text = existingPlayer.RealName ?? "";
                txtAge.Text = existingPlayer.Age.ToString();

                foreach (ComboBoxItem item in cmbCountry.Items)
                {
                    if (item.Content.ToString() == (existingPlayer.Country ?? ""))
                    {
                        cmbCountry.SelectedItem = item;
                        break;
                    }
                }

                foreach (ComboBoxItem item in cmbStatus.Items)
                {
                    if (item.Content.ToString() == (existingPlayer.Status ?? "Ativo"))
                    {
                        cmbStatus.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        public Player GetPlayer()
        {
            return new Player
            {
                Id = existingPlayer?.Id ?? 0,
                Nickname = txtNickname.Text.Trim(),
                RealName = txtRealName.Text.Trim(),
                Age = int.TryParse(txtAge.Text, out int age) ? age : 18,
                Country = (cmbCountry.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Brasil",
                Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Ativo",
                RegistrationDate = existingPlayer?.RegistrationDate ?? DateTime.Now,
                LastAccess = DateTime.Now
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFields())
            {
                DialogResult = true;
                Close();
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtNickname.Text))
            {
                MessageBox.Show("Nickname é obrigatório!", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNickname.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRealName.Text))
            {
                MessageBox.Show("Nome real é obrigatório!", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRealName.Focus();
                return false;
            }

            if (!int.TryParse(txtAge.Text, out int age) || age < 1 || age > 120)
            {
                MessageBox.Show("Idade inválida! Deve ser entre 1 e 120 anos.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAge.Focus();
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}