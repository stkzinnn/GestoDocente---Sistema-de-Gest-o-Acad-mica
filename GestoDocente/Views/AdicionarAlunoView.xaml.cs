using System.Windows;
using GestoDocente.Models;
using GestoDocente.ViewModels;

namespace GestoDocente.Views
{
    public partial class AdicionarAlunoView : Window
    {
        public AdicionarAlunoViewModel ViewModel { get; } = new AdicionarAlunoViewModel();
        public Aluno? NovoAluno { get; private set; }

        public AdicionarAlunoView()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            NovoAluno = ViewModel.ToAluno();
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
