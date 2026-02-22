using GestoDocente.ViewModels;

public class GrupoNotasExpandido
{
    public string Grupo { get; set; } = string.Empty;
    public Dictionary<string, double> NotasPorTarefa { get; set; } = new();
    public List<AlunoNotaViewModel> Alunos { get; set; } = new();
}
