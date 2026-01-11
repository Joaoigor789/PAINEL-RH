using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace RH_STAFF
{
    public partial class TeladorPage : Page
    {
        private List<Telagem> telagens = new List<Telagem>();
        private List<Anexo> anexos = new List<Anexo>();

        public TeladorPage()
        {
            InitializeComponent();
            InitializeData();
            SetupEventHandlers();
            LoadSampleData();
            UpdateStats();
        }

        private void SetupEventHandlers()
        {
            btnSalvarTelagem.Click += BtnSalvarTelagem_Click;
            btnLimparForm.Click += BtnLimparForm_Click;
            btnSelecionarArquivo.Click += BtnSelecionarArquivo_Click;
            btnUploadArquivo.Click += BtnUploadArquivo_Click;

            dpDataEntrega.SelectedDate = DateTime.Today;
            cmbStatus.SelectedIndex = 0;
            cmbTecido.SelectedIndex = 0;
        }

        private void InitializeData()
        {
            telagens = new List<Telagem>();
            anexos = new List<Anexo>();
        }

        private void LoadSampleData()
        {
            telagens.Add(new Telagem
            {
                Id = 1,
                Cliente = "Confecções Silva",
                TipoTecido = "Algodão",
                Cor = "Azul Marinho",
                Metragem = 150,
                DataCadastro = DateTime.Now.AddDays(-5),
                DataEntrega = DateTime.Now.AddDays(2),
                Status = "Em Produção",
                Observacoes = "Tecido de alta qualidade"
            });

            telagens.Add(new Telagem
            {
                Id = 2,
                Cliente = "Moda Jeans Ltda",
                TipoTecido = "Jeans",
                Cor = "Azul Claro",
                Metragem = 200,
                DataCadastro = DateTime.Now.AddDays(-3),
                DataEntrega = DateTime.Now.AddDays(5),
                Status = "Pendente",
                Observacoes = "Com elastano"
            });

            telagens.Add(new Telagem
            {
                Id = 3,
                Cliente = "Tecidos Finos SA",
                TipoTecido = "Seda",
                Cor = "Vermelho",
                Metragem = 80,
                DataCadastro = DateTime.Now.AddDays(-1),
                DataEntrega = DateTime.Now.AddDays(7),
                Status = "Concluído",
                Observacoes = "Para vestidos de festa"
            });

            RefreshTelagensList();
        }

        private void BtnSalvarTelagem_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarFormulario())
            {
                var novaTelagem = new Telagem
                {
                    Id = telagens.Count > 0 ? telagens.Max(t => t.Id) + 1 : 1,
                    Cliente = txtCliente.Text.Trim(),
                    TipoTecido = (cmbTecido.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Cor = txtCor.Text.Trim(),
                    Metragem = decimal.TryParse(txtMetragem.Text, out decimal metragem) ? metragem : 0,
                    DataCadastro = DateTime.Now,
                    DataEntrega = dpDataEntrega.SelectedDate ?? DateTime.Now.AddDays(7),
                    Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    Observacoes = txtObservacoes.Text.Trim()
                };

                telagens.Add(novaTelagem);
                RefreshTelagensList();
                UpdateStats();
                LimparFormulario();

                MessageBox.Show("Telagem registrada com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                MessageBox.Show("Informe o cliente.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCliente.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCor.Text))
            {
                MessageBox.Show("Informe a cor do tecido.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCor.Focus();
                return false;
            }

            if (!decimal.TryParse(txtMetragem.Text, out decimal metragem) || metragem <= 0)
            {
                MessageBox.Show("Informe uma metragem válida.", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMetragem.Focus();
                return false;
            }

            return true;
        }

        private void BtnLimparForm_Click(object sender, RoutedEventArgs e)
        {
            LimparFormulario();
        }

        private void LimparFormulario()
        {
            txtCliente.Text = "";
            txtCor.Text = "";
            txtMetragem.Text = "";
            txtObservacoes.Text = "";
            cmbStatus.SelectedIndex = 0;
            cmbTecido.SelectedIndex = 0;
            dpDataEntrega.SelectedDate = DateTime.Today;
            txtCliente.Focus();
        }

        private void BtnSelecionarArquivo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Todos os arquivos (*.*)|*.*|Imagens (*.jpg;*.png;*.gif)|*.jpg;*.png;*.gif|PDF (*.pdf)|*.pdf|Documentos (*.doc;*.docx;*.xls;*.xlsx)|*.doc;*.docx;*.xls;*.xlsx",
                Title = "Selecionar arquivo para anexar"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtArquivoSelecionado.Text = openFileDialog.FileName;
                txtDescricaoArquivo.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void BtnUploadArquivo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArquivoSelecionado.Text) ||
                txtArquivoSelecionado.Text == "Nenhum arquivo selecionado")
            {
                MessageBox.Show("Selecione um arquivo primeiro.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var novoAnexo = new Anexo
                {
                    Id = anexos.Count + 1,
                    NomeArquivo = System.IO.Path.GetFileName(txtArquivoSelecionado.Text),
                    CaminhoCompleto = txtArquivoSelecionado.Text,
                    Descricao = txtDescricaoArquivo.Text.Trim(),
                    DataUpload = DateTime.Now,
                    TamanhoBytes = new System.IO.FileInfo(txtArquivoSelecionado.Text).Length
                };

                anexos.Add(novoAnexo);
                UpdateStats();

                txtArquivoSelecionado.Text = "Nenhum arquivo selecionado";
                txtDescricaoArquivo.Text = "Descrição do arquivo...";

                MessageBox.Show($"Arquivo '{novoAnexo.NomeArquivo}' anexado com sucesso!", "Upload Concluído",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao anexar arquivo: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshTelagensList()
        {
            var recentes = telagens
                .OrderByDescending(t => t.DataCadastro)
                .Take(10)
                .ToList();

            dgTelagens.ItemsSource = recentes;
        }

        private void UpdateStats()
        {
            txtTotalTelagens.Text = telagens.Count.ToString();
            txtConcluidos.Text = telagens.Count(t => t.Status == "Concluído" || t.Status == "Entregue").ToString();
            txtPendentes.Text = telagens.Count(t => t.Status == "Pendente" || t.Status == "Em Produção").ToString();
            txtTotalAnexos.Text = anexos.Count.ToString();
        }

        // Classes de dados
        public class Telagem
        {
            public int Id { get; set; }
            public string Cliente { get; set; }
            public string TipoTecido { get; set; }
            public string Cor { get; set; }
            public decimal Metragem { get; set; }
            public DateTime DataCadastro { get; set; }
            public DateTime DataEntrega { get; set; }
            public string Status { get; set; }
            public string Observacoes { get; set; }

            // Propriedades para display
            public string Tecido => TipoTecido;
        }

        public class Anexo
        {
            public int Id { get; set; }
            public string NomeArquivo { get; set; }
            public string CaminhoCompleto { get; set; }
            public string Descricao { get; set; }
            public DateTime DataUpload { get; set; }
            public long TamanhoBytes { get; set; }

            public string TamanhoFormatado
            {
                get
                {
                    if (TamanhoBytes < 1024) return $"{TamanhoBytes} B";
                    if (TamanhoBytes < 1048576) return $"{TamanhoBytes / 1024:F1} KB";
                    return $"{TamanhoBytes / 1048576:F1} MB";
                }
            }
        }
    }
}