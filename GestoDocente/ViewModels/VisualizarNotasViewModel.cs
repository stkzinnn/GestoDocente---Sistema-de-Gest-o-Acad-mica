using GestoDocente.Models;
using GestoDocente.Services;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GestoDocente.ViewModels
{
    public class AlunoNotaSimples
    {
        public string Nome { get; set; } = string.Empty;
        public double Nota { get; set; }
    }

    public class GrupoSimples
    {
        public string Nome { get; set; } = string.Empty;
        public double Nota { get; set; }
        public List<AlunoNotaSimples> Alunos { get; set; } = new();
    }

    public class VisualizarNotasViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Tarefa> Tarefas { get; set; } = new();
        public ObservableCollection<GrupoSimples> GruposSimplificados { get; set; } = new();

        private Tarefa? _tarefaSelecionada;
        public Tarefa? TarefaSelecionada
        {
            get => _tarefaSelecionada;
            set
            {
                _tarefaSelecionada = value;
                OnPropertyChanged();
                AtualizarGraficos();
                AtualizarTabela();
                CalcularEstatisticas();
            }
        }

        public SeriesCollection GraficoAlunosSeries { get; set; } = new();
        public SeriesCollection GraficoGruposSeries { get; set; } = new();
        public string[] LabelsAlunos { get; set; } = Array.Empty<string>();
        public string[] LabelsGrupos { get; set; } = Array.Empty<string>();
        public Func<double, string> Formatter { get; set; } = value => value.ToString("F1");

        // Estatísticas
        private double _media;
        public double Media
        {
            get => _media;
            set { _media = value; OnPropertyChanged(); }
        }

        private double _notaMaxima;
        public double NotaMaxima
        {
            get => _notaMaxima;
            set { _notaMaxima = value; OnPropertyChanged(); }
        }

        private double _notaMinima;
        public double NotaMinima
        {
            get => _notaMinima;
            set { _notaMinima = value; OnPropertyChanged(); }
        }

        private int _aprovados;
        public int Aprovados
        {
            get => _aprovados;
            set { _aprovados = value; OnPropertyChanged(); }
        }

        private int _reprovados;
        public int Reprovados
        {
            get => _reprovados;
            set { _reprovados = value; OnPropertyChanged(); }
        }

        private List<Grupo> _grupos = new();
        private List<Tarefa> _tarefas = new();

        public VisualizarNotasViewModel()
        {
            _grupos = DataService.LoadJson<List<Grupo>>("grupos.json") ?? new();
            _tarefas = DataService.LoadJson<List<Tarefa>>("tarefas.json") ?? new();
            var alunosAtualizados = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new();

            foreach (var grupo in _grupos)
            {
                for (int i = 0; i < grupo.Alunos.Count; i++)
                {
                    var alunoAtualizado = alunosAtualizados.FirstOrDefault(a => a.Numero == grupo.Alunos[i].Numero);
                    if (alunoAtualizado != null)
                        grupo.Alunos[i] = alunoAtualizado;
                }
            }

            Tarefas = new ObservableCollection<Tarefa>(_tarefas);

            if (Tarefas.Any())
                TarefaSelecionada = Tarefas.First();
        }

        private void AtualizarTabela()
        {
            GruposSimplificados.Clear();
            if (TarefaSelecionada == null) return;

            foreach (var grupo in _grupos)
            {
                grupo.Notas.TryGetValue(TarefaSelecionada.Id, out double notaGrupo);

                var alunos = grupo.Alunos.Select(a => new AlunoNotaSimples
                {
                    Nome = a.Nome,
                    Nota = a.NotasIndividuais.TryGetValue(TarefaSelecionada.Id, out double nota) ? nota : 0
                }).ToList();

                GruposSimplificados.Add(new GrupoSimples
                {
                    Nome = grupo.Nome,
                    Nota = notaGrupo,
                    Alunos = alunos
                });
            }

            OnPropertyChanged(nameof(GruposSimplificados));
        }

        private void AtualizarGraficos()
        {
            if (TarefaSelecionada == null) return;

            int tarefaId = TarefaSelecionada.Id;

            var alunos = _grupos.SelectMany(g => g.Alunos).ToList();
            LabelsAlunos = alunos.Select(a => a.Nome).ToArray();
            var valoresAlunos = alunos.Select(a => a.NotasIndividuais.TryGetValue(tarefaId, out var nota) ? nota : 0);
            GraficoAlunosSeries.Clear();
            GraficoAlunosSeries.Add(new ColumnSeries
            {
                Title = "Nota por Aluno",
                Values = new ChartValues<double>(valoresAlunos)
            });

            LabelsGrupos = _grupos.Select(g => g.Nome).ToArray();
            var valoresGrupos = _grupos.Select(g => g.Notas.TryGetValue(tarefaId, out var nota) ? nota : 0);
            GraficoGruposSeries.Clear();
            GraficoGruposSeries.Add(new ColumnSeries
            {
                Title = "Nota por Grupo",
                Values = new ChartValues<double>(valoresGrupos)
            });

            OnPropertyChanged(nameof(GraficoAlunosSeries));
            OnPropertyChanged(nameof(LabelsAlunos));
            OnPropertyChanged(nameof(GraficoGruposSeries));
            OnPropertyChanged(nameof(LabelsGrupos));
        }

        private void CalcularEstatisticas()
        {
            if (TarefaSelecionada == null) return;

            int tarefaId = TarefaSelecionada.Id;
            var todasNotas = _grupos.SelectMany(g => g.Alunos)
                                    .Select(a => a.NotasIndividuais.TryGetValue(tarefaId, out var nota) ? nota : 0)
                                    .ToList();

            if (todasNotas.Count == 0)
            {
                Media = NotaMaxima = NotaMinima = 0;
                Aprovados = Reprovados = 0;
                return;
            }

            Media = todasNotas.Average();
            NotaMaxima = todasNotas.Max();
            NotaMinima = todasNotas.Min();
            Aprovados = todasNotas.Count(n => n >= 9.5);
            Reprovados = todasNotas.Count(n => n < 9.5);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
