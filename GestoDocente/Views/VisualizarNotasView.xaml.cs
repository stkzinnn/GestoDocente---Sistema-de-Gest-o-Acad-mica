using System.Windows;
using GestoDocente.ViewModels;

namespace GestoDocente.Views
{
    public partial class VisualizarNotasView : Window
    {
        public VisualizarNotasView()
        {
            InitializeComponent();
            DataContext = new VisualizarNotasViewModel();
        }

        private void VoltarAoMenu_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}
