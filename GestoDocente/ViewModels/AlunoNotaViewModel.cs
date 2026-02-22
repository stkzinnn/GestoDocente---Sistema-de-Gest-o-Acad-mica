using System.Collections.Generic;

namespace GestoDocente.ViewModels
{
    public class AlunoNotaViewModel
    {
        public string Nome { get; set; } = string.Empty;

       
        public Dictionary<int, double> NotasPorTarefa { get; set; } = new();
    }
}
