using System;
using System.Windows;

namespace RH_STAFF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configuração de tratamento global de exceções
            this.DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show($"Erro não tratado: {args.Exception.Message}",
                    "Erro do Sistema",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}