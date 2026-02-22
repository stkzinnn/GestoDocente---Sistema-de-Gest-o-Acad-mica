using GestoDocente.Helpers;
using GestoDocente.Models;
using GestoDocente.Services;
using GestoDocente.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace GestoDocente.ViewModels
{
    public class TarefasViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Tarefa> Tarefas { get; }

        private Tarefa? _tarefaSelecionada;
        public Tarefa? TarefaSelecionada
        {
            get => _tarefaSelecionada;
            set
            {
                _tarefaSelecionada = value;
                OnPropertyChanged();
                ((RelayCommand)RemoverCommand).RaiseCanExecuteChanged();
                ((RelayCommand)EditarCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AdicionarCommand { get; }
        public ICommand RemoverCommand { get; }
        public ICommand EditarCommand { get; }

        public TarefasViewModel()
        {
            var lista = DataService.LoadJson<List<Tarefa>>("tarefas.json") ?? new();
            Tarefas = new ObservableCollection<Tarefa>(lista.OrderBy(t => t.DataTermino));

            AdicionarCommand = new RelayCommand(AdicionarTarefa);
            RemoverCommand = new RelayCommand(RemoverTarefa, () => TarefaSelecionada != null);
            EditarCommand = new RelayCommand(EditarTarefa, () => TarefaSelecionada != null);
        }

        private void AdicionarTarefa()
        {
            var popup = new AdicionarTarefaView();
            if (popup.ShowDialog() == true)
            {
                var nova = popup.TarefaEditada;
                nova.Id = GerarNovoId();
                Tarefas.Add(nova);
                AtualizarOrdem();
                Save();
            }
        }

        private void EditarTarefa()
        {
            if (TarefaSelecionada == null) return;

            var popup = new AdicionarTarefaView(TarefaSelecionada);
            if (popup.ShowDialog() == true)
            {
                var tarefa = popup.TarefaEditada;
                TarefaSelecionada.Titulo = tarefa.Titulo;
                TarefaSelecionada.Descricao = tarefa.Descricao;
                TarefaSelecionada.DataInicio = tarefa.DataInicio;
                TarefaSelecionada.DataTermino = tarefa.DataTermino;
                TarefaSelecionada.Peso = tarefa.Peso;
                AtualizarOrdem();
                Save();
            }
        }

        private bool PodeRemoverTarefa()
        {
            return TarefaSelecionada != null;
        }

        private void RemoverTarefa()
        {
            if (TarefaSelecionada == null)
                return;

            var grupos = DataService.LoadJson<List<Grupo>>("grupos.json") ?? new();
            var alunos = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new();

            // Verifica se algum grupo tem nota de grupo atribuída
            bool temNotasDeGrupo = grupos.Any(g =>
                g.Notas != null && g.Notas.ContainsKey(TarefaSelecionada.Id));

            // Verifica se algum aluno tem nota individual atribuída à tarefa
            bool temNotasIndividuais = alunos.Any(aluno =>
                aluno.NotasIndividuais != null &&
                aluno.NotasIndividuais.ContainsKey(TarefaSelecionada.Id));

            if (temNotasDeGrupo || temNotasIndividuais)
            {
                MessageBox.Show("Não é possível eliminar esta tarefa porque já tem notas atribuídas (de grupo ou individuais).",
                                "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Tem a certeza que deseja eliminar esta tarefa?",
                                "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Tarefas.Remove(TarefaSelecionada);
                DataService.SaveJson("tarefas.json", Tarefas.ToList());
                TarefaSelecionada = null;
            }
        }






        private int GerarNovoId() =>
            Tarefas.Any() ? Tarefas.Max(t => t.Id) + 1 : 1;

        private void AtualizarOrdem()
        {
            var ordenadas = Tarefas.OrderBy(t => t.DataTermino).ToList();
            Tarefas.Clear();
            foreach (var t in ordenadas)
                Tarefas.Add(t);
        }
        private void LimparNotasDeTarefasRemovidas()
        {
            var tarefasExistentes = DataService.LoadJson<List<Tarefa>>("tarefas.json") ?? new();
            var grupos = DataService.LoadJson<List<Grupo>>("grupos.json") ?? new();

            var idsValidos = tarefasExistentes.Select(t => t.Id).ToHashSet();

            foreach (var grupo in grupos)
            {
                // Limpar notas de grupo
                grupo.Notas = grupo.Notas
                    .Where(kvp => idsValidos.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                // Limpar notas individuais
                foreach (var aluno in grupo.Alunos)
                {
                    aluno.NotasIndividuais = aluno.NotasIndividuais
                        .Where(kvp => idsValidos.Contains(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }
            }

            DataService.SaveJson("grupos.json", grupos);
        }

        public void Save()
        {
            DataService.SaveJson("tarefas.json", Tarefas.ToList());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
