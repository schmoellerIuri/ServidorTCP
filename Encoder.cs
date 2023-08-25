using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PraticaSockets
{
    public static class EncoderTarefas
    {
        public static byte[] EncodeListaDeTarefas(List<Tarefa> tarefas)
        {
            string encoded = "";

            foreach (var tarefa in tarefas)
            {
                encoded += EncodeTarefa(tarefa) + "\n";
            }

            return Encoding.UTF8.GetBytes(encoded);
        }

        public static string EncodeTarefa(Tarefa tarefa)
        {
            string encoded = "";

            encoded += "\n{\n";
            encoded += $"\tId={tarefa.Id},\n";
            encoded += $"\tDescricao={tarefa.Descricao},\n";
            encoded += $"\tStatus={tarefa.Status},\n";
            encoded += $"\tDataEntrega={tarefa.DataEntrega:dd/MM/yyyy},\n";
            encoded += $"\tResponsavel={tarefa.Responsavel}\n";
            encoded += "}";

            return encoded;
        }
    }
}