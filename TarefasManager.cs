namespace PraticaSockets
{
    public static class TarefasManager
    {
        static List<Tarefa> tarefas = new();

        public static int GenerateNewId()
        {
            Random random = new();

            int id = random.Next(100, 9999);

            do
            {
                id = random.Next(1000, 9999);

            } while (tarefas.FirstOrDefault(t => t.Id == id) != null);

            return id;
        }

        public static void CriarTarefa(Tarefa tarefa)
        {
            tarefas.Add(tarefa);
        }

        public static List<Tarefa> LerTarefas()
        {
            if(tarefas.Count == 0) throw new Exception("Não existem tarefas cadastradas");
            return tarefas;
        }

        public static Tarefa LerTarefaPorId(int id)
        {
            if(tarefas.Count == 0) throw new Exception("Não existem tarefas cadastradas");
            return tarefas.FirstOrDefault(t => t.Id == id) ?? throw new Exception("Tarefa não encontrada");
        }

        public static void AtualizarTarefa(Tarefa tarefa)
        {
            var tarefaAtualizada = tarefas.FirstOrDefault(t => t.Id == tarefa.Id);

            if (tarefaAtualizada != null)
            {
                if (!String.IsNullOrEmpty(tarefa.Descricao))
                    tarefaAtualizada.Descricao = tarefa.Descricao;
                if (tarefa.Status != StatusTarefa.INDEFINIDO)
                    tarefaAtualizada.Status = tarefa.Status;    
                if (tarefa.DataEntrega != DateTime.MinValue)
                    tarefaAtualizada.DataEntrega = tarefa.DataEntrega;
                if (!String.IsNullOrEmpty(tarefa.Responsavel))
                    tarefaAtualizada.Responsavel = tarefa.Responsavel;
            }
            else
                throw new Exception("Tarefa não encontrada");
        }

        public static void RemoverTarefa(int id)
        {
            var tarefaRemovida = tarefas.FirstOrDefault(t => t.Id == id);

            if (tarefaRemovida != null)
            {
                tarefas.Remove(tarefaRemovida);
            }
            else
                throw new Exception("Tarefa não encontrada");
        }
    }
}