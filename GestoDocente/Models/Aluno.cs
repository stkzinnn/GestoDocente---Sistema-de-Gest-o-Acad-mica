namespace GestoDocente.Models
{
    public class Aluno
    {
        public int Numero { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Dictionary<int, double> NotasIndividuais { get; set; } = new();

        public Grupo? Grupo { get; set; }
    }
}
