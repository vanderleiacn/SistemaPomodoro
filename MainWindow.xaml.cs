using SistemaPomodoro.data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SistemaPomodoro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        private List<Tarefa> ListaTarefas { get; set; }
        private bool CicloIniciado { get; set; }
        private bool PausaIniciada { get; set; }
        private DateTime HorarioInicio { get; set; }
        private DateTime HorarioPausa { get; set; }
        private DateTime HorarioRetorno { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.ListaTarefas = new List<Tarefa>();
            this.dgListaDeTarefas.ItemsSource = this.ListaTarefas;
            this.CicloIniciado = false;
            this.PausaIniciada = false;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            this.btnIniciaCiclo.IsEnabled = true;
            this.btnDesligar.IsEnabled = false;
        }

        private void btnDesligar_Click(object sender, RoutedEventArgs e)
        {
            FinalizarTarefa();
        }

        private void FinalizarTarefa()
        {
            var horarioTermino = DateTime.Now;

            var duracaoTarefa = Math.Round((horarioTermino - HorarioInicio).TotalMinutes,2);

            this.txbHorarioDoTermino.Text = horarioTermino.ToString("dd/MM/yyyy HH:mm:ss");
            this.ListaTarefas.Add(
                new Tarefa
                {
                    TipoCiclo = ((ComboBoxItem)cbxTipoDeCiclo.SelectedItem).Content.ToString(),
                    Nome = txbTarefa.Text,
                    Inicio = txbHorarioDeInicio.Text,
                    Fim = txbHorarioDoTermino.Text,
                    Duracao = duracaoTarefa.ToString(),
                }); ;

            dgListaDeTarefas.Items.Refresh();
            this.txbStatus.Text = $"Tarefa finalizada!\n{this.txbHorarioDoTermino.Text}";

            HabilitarDesabilitarControles(false);
        }

        private void btnIniciaCiclo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txbTarefa.Text))
            {
                MessageBox.Show("Insira a tarefa!", "ERRO", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            this.HorarioInicio = DateTime.Now;
            DefinirCiclo();

            this.txbHorarioDeInicio.Text = this.HorarioInicio.ToString("dd/MM/yyyy HH:mm:ss");
            HabilitarDesabilitarControles(true);
        }

        private void HabilitarDesabilitarControles(bool iniciar)
        {
            this.CicloIniciado = iniciar;
            this.btnDesligar.IsEnabled = iniciar;
            this.btnIniciaCiclo.IsEnabled = !iniciar;
            this.cbxTipoDeCiclo.IsEnabled = !iniciar;
            this.txbTarefa.IsEnabled = !iniciar;

            if (iniciar)
                this.txbHorarioDoTermino.Text = String.Empty;
        }

        private void DefinirCiclo()
        {
            var minutosPausa = 0;
            var minutosRetorno = 0;
            switch (cbxTipoDeCiclo.SelectedIndex)
            {
                case 0:
                    minutosPausa = 25;
                    minutosRetorno = 5;
                    break;
                case 1:
                    minutosPausa = 105;
                    minutosRetorno = 15;
                    break;
                case 2:
                    minutosPausa = 1;
                    minutosRetorno = 1;
                    break;
                default:
                    break;
            }

            this.HorarioPausa = DateTime.Now.AddMinutes(minutosPausa);
            this.HorarioRetorno = HorarioPausa.AddMinutes(minutosRetorno);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lblTimer.Content = DateTime.Now.ToString("HH:mm:ss");

            if (this.CicloIniciado)
            {
                if (DateTime.Now >= this.HorarioPausa && this.PausaIniciada == false)
                {
                    this.PausaIniciada = true;
                    this.txbStatus.Text = $"Hora de dar uma pausa!\n{DateTime.Now.ToString("HH:mm:ss")}";
                }
                else if (DateTime.Now >= this.HorarioRetorno)
                {
                    this.PausaIniciada = false;
                    if (MessageBox.Show("Deseja continuar na mesma tarefa?", "Hora de retornar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        FinalizarTarefa();
                    }
                    else
                    {
                        this.txbStatus.Text = $"Tarefa retomada!\n{DateTime.Now.ToString("HH:mm:ss")}";
                        DefinirCiclo();
                    }
                }
            }
        }
    }

}
