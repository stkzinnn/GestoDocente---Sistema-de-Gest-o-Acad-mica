using GestoDocente.Models;
using GestoDocente.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GestoDocente.ViewModels
{
    public class AlunoPautaViewModel
    {
        public string Nome { get; set; } = string.Empty;
        public Dictionary<string, double> NotasPorTarefa { get; set; } = new();
        public double NotaFinal { get; set; }
        public string Estado => NotaFinal >= 9.5 ? "✅ Aprovado" : "❌ Reprovado";
    }

    public class PautaFinalViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<AlunoPautaViewModel> AlunosNotas { get; set; } = new();
        public List<string> TitulosTarefas { get; private set; } = new();

        public PautaFinalViewModel()
        {
            CarregarPauta();
        }

        private void CarregarPauta()
        {
            var alunosAtualizados = DataService.LoadJson<List<Aluno>>("alunos.json") ?? new();
            var grupos = DataService.LoadJson<List<Grupo>>("grupos.json") ?? new();
            var tarefas = DataService.LoadJson<List<Tarefa>>("tarefas.json") ?? new();

            TitulosTarefas = tarefas.Select(t => $"{t.Titulo} ({t.Peso}%)").ToList();

            foreach (var grupo in grupos)
            {
                foreach (var aluno in grupo.Alunos)
                {
                    // Substituir pelo aluno real vindo do ficheiro alunos.json
                    var alunoCompleto = alunosAtualizados.FirstOrDefault(a => a.Numero == aluno.Numero);
                    if (alunoCompleto == null)
                        continue;

                    var alunoNotas = new AlunoPautaViewModel
                    {
                        Nome = alunoCompleto.Nome
                    };

                    foreach (var tarefa in tarefas)
                    {
                        string tituloComPeso = $"{tarefa.Titulo} ({tarefa.Peso}%)";
                        alunoNotas.NotasPorTarefa[tituloComPeso] =
                            alunoCompleto.NotasIndividuais.TryGetValue(tarefa.Id, out var nota) ? nota : 0;
                    }

                    alunoNotas.NotaFinal = tarefas.Sum(t =>
                    {
                        string tituloComPeso = $"{t.Titulo} ({t.Peso}%)";
                        return alunoNotas.NotasPorTarefa.TryGetValue(tituloComPeso, out var nota)
                            ? nota * (t.Peso / 100.0)
                            : 0;
                    });

                    AlunosNotas.Add(alunoNotas);
                }
            }

            OnPropertyChanged(nameof(AlunosNotas));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
