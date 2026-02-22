using System.Windows;

namespace GestoDocente.Views
{
    public partial class EditarPerfilView : Window
    {
        private void VoltarAoMenu_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
        public EditarPerfilView()
        {
            InitializeComponent();
        }
    }

}
