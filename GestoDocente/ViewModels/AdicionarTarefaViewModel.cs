using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GestoDocente.Models;

namespace GestoDocente.ViewModels
{
    public class AdicionarTarefaViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _titulo = string.Empty;
        private string? _descricao;
        private DateTime _dataInicio = DateTime.Today;
        private DateTime _dataTermino = DateTime.Today.AddDays(7);
        private double _peso = 1.0;

        public string Titulo
        {
            get => _titulo;
            set { _titulo = value; OnPropertyChanged(); }
        }

        public string? Descricao
        {
            get => _descricao;
            set { _descricao = value; OnPropertyChanged(); }
        }

        public DateTime DataInicio
        {
            get => _dataInicio;
            set { _dataInicio = value; OnPropertyChanged(); }
        }

        public DateTime DataTermino
        {
            get => _dataTermino;
            set { _dataTermino = value; OnPropertyChanged(); }
        }

        public double Peso
        {
            get => _peso;
            set { _peso = value; OnPropertyChanged(); }
        }

        public AdicionarTarefaViewModel() { }

        public AdicionarTarefaViewModel(Tarefa tarefa)
        {
            Id = tarefa.Id;
            Titulo = tarefa.Titulo;
            Descricao = tarefa.Descricao;
            DataInicio = tarefa.DataInicio;
            DataTermino = tarefa.DataTermino;
            Peso = tarefa.Peso;
        }

        public Tarefa ToTarefa() => new()
        {
            Id = Id,
            Titulo = Titulo,
            Descricao = Descricao,
            DataInicio = DataInicio,
            DataTermino = DataTermino,
            Peso = Peso 
        };

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
