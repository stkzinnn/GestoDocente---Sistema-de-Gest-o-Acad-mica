using System.Windows;
using GestoDocente.ViewModels;

namespace GestoDocente
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel(); // <- linha essencial
        }

        
    }
}
