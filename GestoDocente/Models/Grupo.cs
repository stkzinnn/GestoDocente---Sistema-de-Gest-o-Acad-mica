using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GestoDocente.Models
{
    public class Grupo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public ObservableCollection<Aluno> Alunos { get; set; } = new();

        public Dictionary<int, double> Notas { get; set; }

        // mostra os nomes dos alunos no tooltip ou na tabela
        public string Membros => string.Join(", ", Alunos.Select(a => a.Nome));

        public Grupo()
        {
            
            Notas = new Dictionary<int, double>();
        }
    }
}
