using GestoDocente.Helpers;
using GestoDocente.Models;
using GestoDocente.Services;
using GestoDocente.Views;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace GestoDocente.ViewModels
{
    public class AlunosViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Aluno> Alunos { get; }
        public ObservableCollection<Aluno> AlunosFiltrados { get; set; }

        private Aluno? _alunoSelecionado;
        public Aluno? AlunoSelecionado
        {
            get => _alunoSelecionado;
            set
            {
                _alunoSelecionado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(GrupoDoAluno));
                OnPropertyChanged(nameof(NotasDoAluno));
                ((RelayCommand)RemoverCommand).RaiseCanExecuteChanged();
            }
        }

        public string GrupoDoAluno => AlunoSelecionado?.Grupo?.Nome ?? "Sem grupo atribuído";

        public List<string> NotasDoAluno =>
            AlunoSelecionado?.NotasIndividuais?
                .Select(n => $"Tarefa {n.Key}: {n.Value} valores")
                .ToList() ?? new();

        private string _filtroTexto = string.Empty;
        public string FiltroTexto
        {
            get => _filtroTexto;
            set
            {
                _filtroTexto = value;
                OnPropertyChanged();
                FiltrarAlunos();
            }
        }

        public ICommand AdicionarCommand { get; }
        public ICommand RemoverCommand { get; }
        public ICommand ImportCsvCommand { get; }

        public AlunosViewModel()
        {
            var alunosBase = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new();
            var grupos = DataService.LoadJson<List<Grupo>>("grupos.json") ?? new();

          
            foreach (var grupo in grupos)
            {
                foreach (var alunoGrupo in grupo.Alunos)
                {
                    var alunoOriginal = alunosBase.FirstOrDefault(a => a.Numero == alunoGrupo.Numero);
                    if (alunoOriginal != null)
                    {
                        alunoOriginal.Grupo = grupo;
                        alunoOriginal.NotasIndividuais = alunoGrupo.NotasIndividuais;
                    }
                }
            }

            Alunos = new ObservableCollection<Aluno>(alunosBase);
            AlunosFiltrados = new ObservableCollection<Aluno>(alunosBase);

            AdicionarCommand = new RelayCommand(AdicionarAluno);
            RemoverCommand = new RelayCommand(RemoverAluno, () => AlunoSelecionado != null);
            ImportCsvCommand = new RelayCommand(ImportarCsv);
        }

        private void FiltrarAlunos()
        {
            var filtrados = Alunos
                .Where(a => a.Nome.Contains(FiltroTexto, System.StringComparison.OrdinalIgnoreCase)
                         || a.Numero.ToString().Contains(FiltroTexto))
                .ToList();

            AlunosFiltrados = new ObservableCollection<Aluno>(filtrados);
            OnPropertyChanged(nameof(AlunosFiltrados));
        }

        private void AdicionarAluno()
        {
            var popup = new AdicionarAlunoView();
            if (popup.ShowDialog() == true && popup.NovoAluno != null)
            {
                Alunos.Add(popup.NovoAluno);
                AlunosFiltrados.Add(popup.NovoAluno);
                Save();
                MessageBox.Show("Aluno adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RemoverAluno()
        {
            if (AlunoSelecionado == null) return;

            var grupos = DataService.LoadJson<List<Grupo>>("grupos.json") ?? new();
            bool alunoEmGrupo = grupos.Any(g => g.Alunos.Any(a => a.Numero == AlunoSelecionado.Numero));

            if (alunoEmGrupo)
            {
                MessageBox.Show("❌ Não é possível remover este aluno porque pertence a um grupo.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var resultado = MessageBox.Show(
                "Quer realmente continuar?",
                "Confirmação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                Alunos.Remove(AlunoSelecionado);
                AlunosFiltrados.Remove(AlunoSelecionado);
                Save();
                MessageBox.Show("Aluno removido com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportarCsv()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Importar Alunos",
                Filter = "CSV Files (*.csv)|*.csv",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var alunosNovos = new List<Aluno>();
                    var erros = new List<string>();
                    var linhas = File.ReadLines(openFileDialog.FileName, Encoding.GetEncoding("iso-8859-1"));
                    var alunosExistentes = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new();

                    foreach (var line in linhas)
                    {
                        var campos = line.Split(',');

                        if (campos.Length >= 3 && int.TryParse(campos[1], out int numero))
                        {
                            string nome = campos[0].Trim();
                            string email = campos[2].Trim();
                            string emailEsperado = $"al{numero}@alunos.utad.pt";

                            if (numero < 10000 || numero > 99999)
                            {
                                erros.Add($"Linha com número inválido: {numero} (deve ter 5 dígitos)");
                                continue;
                            }

                            if (!email.Equals(emailEsperado, System.StringComparison.OrdinalIgnoreCase))
                            {
                                erros.Add($"Email inválido para o número {numero}. Esperado: {emailEsperado}");
                                continue;
                            }

                            if (alunosExistentes.Any(a => a.Numero == numero) || alunosNovos.Any(a => a.Numero == numero))
                            {
                                erros.Add($"Número duplicado: {numero}");
                                continue;
                            }

                            alunosNovos.Add(new Aluno
                            {
                                Nome = nome,
                                Numero = numero,
                                Email = email
                            });
                        }
                        else
                        {
                            erros.Add("Linha inválida (esperados 3 campos: Nome, Número, Email)");
                        }
                    }

                    foreach (var aluno in alunosNovos)
                    {
                        Alunos.Add(aluno);
                        AlunosFiltrados.Add(aluno);
                        alunosExistentes.Add(aluno);
                    }

                    Save();

                    string mensagem = $"✅ {alunosNovos.Count} aluno(s) importado(s) com sucesso.";
                    if (erros.Any())
                    {
                        mensagem += $"\n⚠️ {erros.Count} erro(s) encontrados:\n\n" + string.Join("\n", erros);
                    }

                    MessageBox.Show(mensagem, "Importação", MessageBoxButton.OK,
                        erros.Any() ? MessageBoxImage.Warning : MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Erro ao importar alunos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void Save()
        {
            DataService.SaveJson("alunos.json", Alunos.ToList());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
