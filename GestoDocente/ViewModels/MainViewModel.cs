using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GestoDocente.Views;
using GestoDocente.Helpers;
using GestoDocente.Models;
using GestoDocente.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace GestoDocente.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public string NomeUtilizador { get; private set; }
        public string MensagemBoasVindas => $"Bem-vindo, {NomeUtilizador}!";
        public BitmapImage FotoUtilizador { get; }
        public Perfil Perfil { get; set; }
        
        public ObservableCollection<Tarefa> TarefasPreview { get; private set; }

        public ICommand AbrirAlunosCommand { get; }
        public ICommand AbrirGruposCommand { get; }
        public ICommand AbrirTarefasCommand { get; }
        public ICommand AbrirNotasCommand { get; }
        public ICommand AbrirVisualizarNotasCommand { get; }
        public ICommand AbrirPautaCommand { get; }

        public ICommand AbrirGerirPerfilCommand { get; }

        public MainViewModel()
        {
            // Obter o nome do utilizador do sistema
            NomeUtilizador = Environment.UserName;

            // Inicializar comandos
            Perfil = DataService.LoadJson<Perfil>("perfil.json") ?? new Perfil();
            AbrirAlunosCommand = new RelayCommand(AbrirAlunos);
            AbrirGruposCommand = new RelayCommand(AbrirGrupos);
            AbrirTarefasCommand = new RelayCommand(AbrirTarefas);
            AbrirNotasCommand = new RelayCommand(AbrirNotas);
            AbrirVisualizarNotasCommand = new RelayCommand(AbrirVisualizarNotas);
            AbrirPautaCommand = new RelayCommand(AbrirPauta);
            AbrirGerirPerfilCommand = new RelayCommand(AbrirGerirPerfil);
            // Carregar a lista de tarefas (máx. 5)
            var lista = DataService.LoadJson<List<Tarefa>>("tarefas.json") ?? new();
            TarefasPreview = new ObservableCollection<Tarefa>(lista.OrderBy(t => t.DataTermino).Take(5));
        }
        private void AbrirGerirPerfil()
        {
            var editarperfilview = new EditarPerfilView();
            editarperfilview.Show();
            FecharJanelaPrincipal();
        }
        private void AbrirAlunos()
        {
            var alunosView = new AlunosView();
            alunosView.Show();
            FecharJanelaPrincipal();
        }

        private void AbrirGrupos()
        {
            var gruposView = new GruposView();
            gruposView.Show();
            FecharJanelaPrincipal();
        }

        private void AbrirTarefas()
        {
            var tarefasView = new TarefasView();
            tarefasView.Show();
            FecharJanelaPrincipal();
        }

        private void AbrirNotas()
        {
            var notasView = new NotasView();
            notasView.Show();
            FecharJanelaPrincipal();
        }

        private void AbrirVisualizarNotas()
        {
            var visualizarnotasView = new VisualizarNotasView();
            visualizarnotasView.Show();
            FecharJanelaPrincipal();
        }
        private void AbrirPauta()
        {
            var pautafinalview = new PautaFinalView ();
            pautafinalview.Show();
            FecharJanelaPrincipal();
        }

        private void FecharJanelaPrincipal()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
