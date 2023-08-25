namespace PraticaSockets
{
    public class Tarefa
    {
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public StatusTarefa Status { get; set; } = StatusTarefa.A_FAZER; 
        public DateTime DataEntrega { get; set; }
        public string? Responsavel { get; set; }
    }
}