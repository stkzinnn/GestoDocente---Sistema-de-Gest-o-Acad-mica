using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GestoDocente.Helpers;
using GestoDocente.Models;
using GestoDocente.Services;
using Microsoft.Win32;

namespace GestoDocente.ViewModels
{
    public class EditarPerfilViewModel : INotifyPropertyChanged
    {
        private string _nome;
        private string _caminhoFoto;

        public string Nome
        {
            get => _nome;
            set { _nome = value; OnPropertyChanged(); }
        }

        public string CaminhoFoto
        {
            get => _caminhoFoto;
            set { _caminhoFoto = value; OnPropertyChanged(); }
        }

        public ICommand GuardarCommand { get; }
        public ICommand EscolherFotoCommand { get; }

        public EditarPerfilViewModel()
        {
            var perfil = DataService.LoadJson<Perfil>("perfil.json") ?? new Perfil();
            Nome = perfil.Nome;
            CaminhoFoto = perfil.CaminhoFoto;

            GuardarCommand = new RelayCommand(Guardar);
            EscolherFotoCommand = new RelayCommand(EscolherFoto);
        }

        private void Guardar()
        {
            var perfil = new Perfil { Nome = Nome, CaminhoFoto = CaminhoFoto };
            DataService.SaveJson("perfil.json", perfil);
            System.Windows.MessageBox.Show("Perfil guardado com sucesso!");
        }

        private void EscolherFoto()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Imagens (*.jpg;*.png)|*.jpg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                CaminhoFoto = dialog.FileName;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
