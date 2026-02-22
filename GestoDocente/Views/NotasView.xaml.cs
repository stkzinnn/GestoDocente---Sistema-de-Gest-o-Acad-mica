using System.Windows;
using GestoDocente.ViewModels;

namespace GestoDocente.Views
{
    public partial class NotasView : Window
    {
        public NotasView()
        {
            InitializeComponent();
            DataContext = new NotasViewModel();
        }

        private void VoltarMenu_Click(object sender, RoutedEventArgs e)
        {
            
            var mainWindow = new MainWindow();
            mainWindow.Show();

            
            Close();
        }
    }
}
