using GestoDocente.Models;
using GestoDocente.Services;
using GestoDocente.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace GestoDocente.ViewModels
{
    public class AdicionarAlunoViewModel : INotifyPropertyChanged
    {
        private int _numero;
        private string _nome = string.Empty;
        private string _email = string.Empty;

        public int Numero
        {
            get => _numero;
            set { _numero = value; OnPropertyChanged(); }
        }

        public string Nome
        {
            get => _nome;
            set { _nome = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public Aluno ToAluno() => new Aluno { Numero = Numero, Nome = Nome, Email = Email };

        public ICommand GuardarCommand => new RelayCommand(GuardarAluno);

        private void GuardarAluno()
        {
            var alunos = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new();

            // Validar que o número é de 5 dígitos
            if (Numero < 10000 || Numero > 99999)
            {
                MessageBox.Show("⚠️ O número deve ter exatamente 5 dígitos.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar o  email obrigatório e formato correto
            var emailEsperado = $"al{Numero}@alunos.utad.pt";
            if (string.IsNullOrWhiteSpace(Email) || Email != emailEsperado)
            {
                MessageBox.Show($"⚠️ O email tem de ser '{emailEsperado}'", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (alunos.Any(a => a.Numero == Numero))
            {
                MessageBox.Show($"⚠️ Já existe um aluno com o número {Numero}.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var novoAluno = ToAluno();
            alunos.Add(novoAluno);
            DataService.SaveJson("alunos.json", alunos);

            MessageBox.Show("✅ Aluno adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.IsActive)
                ?.Close();
        }
        public ICommand CancelarCommand { get; }

        public AdicionarAlunoViewModel()
        {
            CancelarCommand = new RelayCommand(ExecutarCancelar);
        }

        private void ExecutarCancelar()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?.Close();
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
