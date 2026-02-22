using System.Windows;
using GestoDocente.ViewModels;

namespace GestoDocente.Views
{
    public partial class GruposView : Window
    {
        public GruposView()
        {
            InitializeComponent();

            this.DataContext = new GruposViewModel();
        }



        private void VoltarAoMenu_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}
