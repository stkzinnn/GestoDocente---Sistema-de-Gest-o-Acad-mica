using GestoDocente.Helpers;
using GestoDocente.Models;
using GestoDocente.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace GestoDocente.ViewModels
{
    public class NotasViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Grupo> Grupos { get; set; }
        public ObservableCollection<Tarefa> Tarefas { get; set; }
        public ObservableCollection<Aluno> AlunosDoGrupo { get; set; } = new();

        public string PesquisaGrupo
        {
            get => _pesquisaGrupo;
            set { _pesquisaGrupo = value; OnPropertyChanged(); FiltrarGrupos(); }
        }
        private string _pesquisaGrupo = "";

        public ObservableCollection<Grupo> GruposFiltrados { get; set; } = new();

        public Grupo? GrupoSelecionado
        {
            get => _grupoSelecionado;
            set
            {
                _grupoSelecionado = value;
                OnPropertyChanged();
                AtualizarAlunosDoGrupo();
                AtualizarNotaGrupo();
            }
        }
        private Grupo? _grupoSelecionado;

        public Tarefa? TarefaSelecionada
        {
            get => _tarefaSelecionada;
            set
            {
                _tarefaSelecionada = value;
                OnPropertyChanged();
                AtualizarNotaGrupo();
                AtualizarNotaIndividual();
            }
        }
        private Tarefa? _tarefaSelecionada;

        public Aluno? AlunoSelecionado
        {
            get => _alunoSelecionado;
            set
            {
                _alunoSelecionado = value;
                OnPropertyChanged();
                AtualizarNotaIndividual();
            }
        }
        private Aluno? _alunoSelecionado;

        public double? NotaGrupo { get; set; }
        public double? NotaIndividual { get; set; }

        public ICommand AtualizarNotaGrupoCommand => new RelayCommand(() =>
        {
            if (GrupoSelecionado != null && TarefaSelecionada != null && NotaGrupo != null)
            {
                GrupoSelecionado.Notas[TarefaSelecionada.Id] = NotaGrupo.Value;
                DataService.SaveJson("grupos.json", Grupos.ToList());
                MessageBox.Show("Nota do grupo atualizada!");
            }
        });

        public ICommand RemoverNotaGrupoCommand => new RelayCommand(() =>
        {
            if (GrupoSelecionado != null && TarefaSelecionada != null)
            {
                GrupoSelecionado.Notas.Remove(TarefaSelecionada.Id);
                DataService.SaveJson("grupos.json", Grupos.ToList());
                NotaGrupo = null;
                OnPropertyChanged(nameof(NotaGrupo));
                MessageBox.Show("Nota do grupo removida!");
            }
        });

        public ICommand AtualizarNotaIndividualCommand => new RelayCommand(() =>
        {
            if (AlunoSelecionado != null && TarefaSelecionada != null && NotaIndividual != null)
            {
                // Carregar todos os alunos do ficheiro
                var todosAlunos = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new();
                var aluno = todosAlunos.FirstOrDefault(a => a.Numero == AlunoSelecionado.Numero);

                if (aluno != null)
                {
                    // Atualizar nota
                    aluno.NotasIndividuais[TarefaSelecionada.Id] = NotaIndividual.Value;

                    // Guardar ficheiro atualizado
                    DataService.SaveJson("alunos.json", todosAlunos);

                    // Atualizar a referência local (caso esteja a ser usada por gráficos/pauta)
                    AlunoSelecionado.NotasIndividuais = aluno.NotasIndividuais;
                    OnPropertyChanged(nameof(AlunoSelecionado));
                    AtualizarNotaIndividual();

                    MessageBox.Show("Nota individual atualizada!");
                }
                else
                {
                    MessageBox.Show("O aluno não foi encontrado no ficheiro alunos.json.");
                }
            }
        });
        public ICommand RemoverNotaIndividualCommand => new RelayCommand(() =>
        {
            if (AlunoSelecionado != null && TarefaSelecionada != null)
            {
                var todosAlunos = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new();
                var aluno = todosAlunos.FirstOrDefault(a => a.Numero == AlunoSelecionado.Numero);

                if (aluno != null && aluno.NotasIndividuais.ContainsKey(TarefaSelecionada.Id))
                {
                    aluno.NotasIndividuais.Remove(TarefaSelecionada.Id);
                    DataService.SaveJson("alunos.json", todosAlunos);

                    // Atualizar a view com os dados atualizados
                    NotaIndividual = null;
                    OnPropertyChanged(nameof(NotaIndividual));

                    MessageBox.Show("Nota individual removida com sucesso!");
                }
                else
                {
                    MessageBox.Show("Este aluno não tem uma nota atribuída para esta tarefa.");
                }
            }
        });



        public NotasViewModel()
        {
            Grupos = new ObservableCollection<Grupo>(DataService.LoadJson<List<Grupo>>("grupos.json") ?? new());
            Tarefas = new ObservableCollection<Tarefa>(DataService.LoadJson<List<Tarefa>>("tarefas.json") ?? new());
            FiltrarGrupos();
        }

        private void FiltrarGrupos()
        {
            GruposFiltrados.Clear();
            foreach (var grupo in Grupos.Where(g => string.IsNullOrWhiteSpace(PesquisaGrupo) || g.Nome.Contains(PesquisaGrupo, StringComparison.OrdinalIgnoreCase)))
                GruposFiltrados.Add(grupo);
        }

        private void AtualizarAlunosDoGrupo()
        {
            AlunosDoGrupo.Clear();
            if (GrupoSelecionado != null)
            {
                foreach (var aluno in GrupoSelecionado.Alunos)
                    AlunosDoGrupo.Add(aluno);
            }
        }

        private void AtualizarNotaGrupo()
        {
            if (GrupoSelecionado != null && TarefaSelecionada != null)
            {
                GrupoSelecionado.Notas.TryGetValue(TarefaSelecionada.Id, out double nota);
                NotaGrupo = nota;
                OnPropertyChanged(nameof(NotaGrupo));
            }
        }

        private void AtualizarNotaIndividual()
        {
            if (AlunoSelecionado != null && TarefaSelecionada != null)
            {
                // Recarrega o aluno diretamente do ficheiro
                var todosAlunos = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new();
                var aluno = todosAlunos.FirstOrDefault(a => a.Numero == AlunoSelecionado.Numero);

                if (aluno != null && aluno.NotasIndividuais.TryGetValue(TarefaSelecionada.Id, out double nota))
                {
                    NotaIndividual = nota;
                }
                else
                {
                    NotaIndividual = null;
                }

                OnPropertyChanged(nameof(NotaIndividual));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? nome = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}
