using GestoDocente.Models;
using System.Windows;

namespace GestoDocente.Views
{
    public partial class EditarGrupoView : Window
    {
        public string NomeGrupo => NomeTextBox.Text;

        public EditarGrupoView(Grupo grupo)
        {
            InitializeComponent();
            NomeTextBox.Text = grupo.Nome;
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
