using System.Windows;
using GestoDocente.Models;
using GestoDocente.ViewModels;

namespace GestoDocente.Views
{
    public partial class AdicionarTarefaView : Window
    {
        public AdicionarTarefaViewModel ViewModel { get; }
        public Tarefa TarefaEditada => ViewModel.ToTarefa();

        public AdicionarTarefaView(Tarefa? tarefa = null)
        {
            InitializeComponent();
            ViewModel = tarefa != null
                ? new AdicionarTarefaViewModel(tarefa)
                : new AdicionarTarefaViewModel();
            DataContext = ViewModel;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
