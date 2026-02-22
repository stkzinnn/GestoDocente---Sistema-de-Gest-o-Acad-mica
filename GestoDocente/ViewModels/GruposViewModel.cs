    using CommunityToolkit.Mvvm.Input;
    using GestoDocente.Models;
    using GestoDocente.Services;
    using GestoDocente.Views;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
using System.Windows;
    using System.Windows.Input;

    public class GruposViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Grupo> Grupos { get; }
        public ObservableCollection<Aluno> AlunosNaoAtribuidos { get; private set; }
        public ObservableCollection<Aluno> AlunosFiltrados { get; set; }

        private Grupo? _grupoSelecionado;
        public Grupo? GrupoSelecionado
        {
            get => _grupoSelecionado;
            set
            {
                _grupoSelecionado = value;
                OnPropertyChanged();
                AtualizarAlunosNaoAtribuidos();
                ((RelayCommand)AtribuirAlunoCommand).NotifyCanExecuteChanged();
                ((RelayCommand)RemoverGrupoCommand).NotifyCanExecuteChanged();
                ((RelayCommand)EditarGrupoCommand).NotifyCanExecuteChanged();
                ((RelayCommand)RemoverAlunoGrupoCommand).NotifyCanExecuteChanged();
            }
        }

        private Aluno? _alunoSelecionado;
        public Aluno? AlunoSelecionado
        {
            get => _alunoSelecionado;
            set
            {
                _alunoSelecionado = value;
                OnPropertyChanged();
                ((RelayCommand)AtribuirAlunoCommand).NotifyCanExecuteChanged();
            }
        }

        private string _filtroAluno = string.Empty;
        public string FiltroAluno
        {
            get => _filtroAluno;
            set
            {
                _filtroAluno = value;
                OnPropertyChanged();
                AtualizarListaFiltrada();
            }
        }

        public ICommand AdicionarGrupoCommand { get; }
        public ICommand RemoverGrupoCommand { get; }
        public ICommand AtribuirAlunoCommand { get; }
        public ICommand RemoverAlunoGrupoCommand { get; }

        public ICommand EditarGrupoCommand { get; }

        public GruposViewModel()
        {
            Grupos = new ObservableCollection<Grupo>(
                DataService.LoadJson<List<Grupo>>("grupos.json") ?? new());

            AlunosNaoAtribuidos = new ObservableCollection<Aluno>(ObterAlunosNaoAtribuidos());
            AlunosFiltrados = new ObservableCollection<Aluno>(AlunosNaoAtribuidos);

            AdicionarGrupoCommand = new RelayCommand(AdicionarGrupo);
            RemoverGrupoCommand = new RelayCommand(RemoverGrupo, () => GrupoSelecionado != null);
            AtribuirAlunoCommand = new RelayCommand(AtribuirAluno, () => GrupoSelecionado != null && AlunoSelecionado != null);
            EditarGrupoCommand = new RelayCommand(EditarGrupo, () => GrupoSelecionado != null);
            RemoverAlunoGrupoCommand = new RelayCommand(RemoverAlunoDoGrupo, () => GrupoSelecionado != null && AlunoGrupoSelecionado != null);



        }
    private void RemoverAlunoDoGrupo()
    {
        if (GrupoSelecionado != null && AlunoGrupoSelecionado != null)
        {
            var resultado = MessageBox.Show(
                "Quer realmente continuar?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                GrupoSelecionado.Alunos.Remove(AlunoGrupoSelecionado);
                AlunosNaoAtribuidos.Add(AlunoGrupoSelecionado);
                AtualizarListaFiltrada();
                AlunoGrupoSelecionado = null;
                Save();
                OnPropertyChanged(nameof(GrupoSelecionado));
            }
        }
    }



    private void AdicionarGrupo()
        {
            var novo = new Grupo { Nome = $"Grupo {Grupos.Count + 1}" };
            Grupos.Add(novo);
            GrupoSelecionado = novo;
            Save();
        }

    private void RemoverGrupo()
    {
        if (GrupoSelecionado != null)
        {
            var resultado = MessageBox.Show(
                "Quer realmente continuar?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                Grupos.Remove(GrupoSelecionado);
                GrupoSelecionado = null;
                AtualizarAlunosNaoAtribuidos();
                Save();
            }
        }
    }

    private void EditarGrupo()
        {
            if (GrupoSelecionado == null) return;

            var janela = new EditarGrupoView(GrupoSelecionado);
            if (janela.ShowDialog() == true)
            {
                GrupoSelecionado.Nome = janela.NomeGrupo;
                Save();
                OnPropertyChanged(nameof(Grupos));
                OnPropertyChanged(nameof(GrupoSelecionado));
            }
        }



        private void AtribuirAluno()
        {
            if (GrupoSelecionado != null && AlunoSelecionado != null)
            {
                GrupoSelecionado.Alunos.Add(AlunoSelecionado);
                AlunosNaoAtribuidos.Remove(AlunoSelecionado);
                AtualizarListaFiltrada();
                AlunoSelecionado = null;
                Save();
            }
        }

        private void AtualizarAlunosNaoAtribuidos()
        {
            var todos = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new List<Aluno>();
            var atribuídos = Grupos.SelectMany(g => g.Alunos.Select(a => a.Numero)).ToHashSet();
            AlunosNaoAtribuidos = new ObservableCollection<Aluno>(todos.Where(a => !atribuídos.Contains(a.Numero)));
            AtualizarListaFiltrada();
            OnPropertyChanged(nameof(AlunosNaoAtribuidos));
        }

        private void AtualizarListaFiltrada()
        {
            if (string.IsNullOrEmpty(FiltroAluno))
            {
                AlunosFiltrados = new ObservableCollection<Aluno>(AlunosNaoAtribuidos);
            }
            else
            {
                var filtrados = AlunosNaoAtribuidos
                    .Where(a => a.Nome.Contains(FiltroAluno, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                AlunosFiltrados = new ObservableCollection<Aluno>(filtrados);
            }
            OnPropertyChanged(nameof(AlunosFiltrados));
        }

        private Aluno? _alunoGrupoSelecionado;
        public Aluno? AlunoGrupoSelecionado
        {
            get => _alunoGrupoSelecionado;
            set
            {
                _alunoGrupoSelecionado = value;
                OnPropertyChanged();
                ((RelayCommand)RemoverAlunoGrupoCommand).NotifyCanExecuteChanged();
            }
        }


        private List<Aluno> ObterAlunosNaoAtribuidos()
        {
            var todos = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new List<Aluno>();
            var atribuídos = Grupos.SelectMany(g => g.Alunos.Select(a => a.Numero)).ToHashSet();
            return todos.Where(a => !atribuídos.Contains(a.Numero)).ToList();
        }

        private void Save()
        {
            DataService.SaveJson("grupos.json", Grupos.ToList());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

