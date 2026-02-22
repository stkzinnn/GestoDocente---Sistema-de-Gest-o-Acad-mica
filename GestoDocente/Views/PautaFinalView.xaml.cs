using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GestoDocente.ViewModels;

namespace GestoDocente.Views
{
    public partial class PautaFinalView : Window
    {
        public PautaFinalView()
        {
            InitializeComponent();

            var vm = (PautaFinalViewModel)DataContext;


            foreach (var tituloTarefa in vm.TitulosTarefas)
            {
                dataGrid.Columns.Insert(dataGrid.Columns.Count - 2, new DataGridTextColumn
                {
                    Header = tituloTarefa,
                    Binding = new Binding($"NotasPorTarefa[{tituloTarefa}]") { StringFormat = "F1" },
                    Width = 100
                });
            }



        }
        private void VoltarAoMenu_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

    }
}
