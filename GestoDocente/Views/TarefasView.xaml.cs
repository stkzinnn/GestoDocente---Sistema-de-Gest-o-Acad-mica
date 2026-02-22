using System.Windows;

namespace GestoDocente.Views
{
    public partial class TarefasView : Window
    {
        public TarefasView()
        {
            InitializeComponent();
        }

        private void VoltarAoMenu_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}
