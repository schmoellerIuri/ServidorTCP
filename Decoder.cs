namespace PraticaSockets
{
    public static class DecoderTarefas
    {
        public static Tarefa DecodeTarefa(string tarefa, bool update)
        {
            var atributos = tarefa.Split(',');

            int id;
            string idString = "";

            if (update)
            {
                idString = atributos.FirstOrDefault(a => a.Contains("Id")) ?? throw new Exception("Id não encontrado");
                id = int.Parse(idString.Split('=')[1]);
            }
            else
                id = TarefasManager.GenerateNewId();

            var descricaoString = atributos.FirstOrDefault(a => a.Contains("Descricao")) ?? "";
            string descricao = "";
            if (!String.IsNullOrEmpty(descricaoString))
                descricao = descricaoString.Split('=')[1];

            var statusString = atributos.FirstOrDefault(a => a.Contains("Status")) ?? "";
            StatusTarefa status = StatusTarefa.INDEFINIDO;
            if (!String.IsNullOrEmpty(statusString))
                status = (StatusTarefa)Enum.Parse(typeof(StatusTarefa),statusString.Split('=')[1]);

            var dataEntregaString = atributos.FirstOrDefault(a => a.Contains("Data"));
            DateTime dataEntrega = DateTime.MinValue;
            if (!String.IsNullOrEmpty(dataEntregaString))
                dataEntrega = DateTime.Parse(dataEntregaString.Split('=')[1]);

            var responsavelString = atributos.FirstOrDefault(a => a.Contains("Responsavel"));
            string responsavel = "";
            if (!String.IsNullOrEmpty(responsavelString))
                responsavel = responsavelString.Split('=')[1];


            return new Tarefa
            {
                Id = id,
                Descricao = descricao,
                Status = status,
                DataEntrega = dataEntrega,
                Responsavel = responsavel
            };
        }
    }
}