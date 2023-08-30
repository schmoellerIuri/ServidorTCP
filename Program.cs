using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PraticaSockets;

string ip = "127.0.0.1";
int port = 50000;

IPEndPoint ipEndPoint = new(IPAddress.Parse(ip), port);

var servidor = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

servidor.Bind(ipEndPoint);
servidor.Listen(10);

while (true)
{
    var handler = await servidor.AcceptAsync();
    var thread = new Thread(DoWork);
    thread.Start(handler);
}


void DoWork(object handler)
{
    var _handler = (Socket) handler;
        
    while (true)
    {
        //recebe o tamanho em bytes da mensagem
        var sizeInBytes = new byte[4];
        try
        {
            _ = _handler.Receive(sizeInBytes, SocketFlags.None);
        }
        catch
        { break; }

        var size = BitConverter.ToInt32(sizeInBytes, 0);

        //recebe a mensagem
        var buffer = new byte[size];
        var qtdBytesRequest = _handler.Receive(buffer, SocketFlags.None);
        var request = Encoding.UTF8.GetString(buffer, 0, qtdBytesRequest);

        if (String.IsNullOrEmpty(request))
            continue;

        string operacao = request[..5];
        byte[] retornoAoCliente = Array.Empty<byte>();

        if (string.Equals(operacao, "|<E>|")) 
            break;
        

        try
        {
            switch (operacao)
            {
                case "|<C>|":
                    TarefasManager.CriarTarefa(DecoderTarefas.DecodeTarefa(request[5..], false));
                    retornoAoCliente = Encoding.UTF8.GetBytes("Tarefa criada com sucesso!");
                    break;
                case "|<R>|":
                    var idRequisitado = int.Parse(request[5..]);
                    retornoAoCliente = idRequisitado == 0 ? EncoderTarefas.EncodeListaDeTarefas(TarefasManager.LerTarefas()) : Encoding.UTF8.GetBytes(EncoderTarefas.EncodeTarefa(TarefasManager.LerTarefaPorId(idRequisitado)));
                    break;
                case "|<U>|":
                    TarefasManager.AtualizarTarefa(DecoderTarefas.DecodeTarefa(request[5..], true));
                    retornoAoCliente = Encoding.UTF8.GetBytes("Tarefa atualizada com sucesso!");
                    break;
                case "|<D>|":
                    TarefasManager.RemoverTarefa(int.Parse(request[5..]));
                    retornoAoCliente = Encoding.UTF8.GetBytes("Tarefa removida com sucesso!");
                    break;
                default:
                    retornoAoCliente = Encoding.UTF8.GetBytes("Operação inválida");
                    break;
            }
        }
        catch (Exception ex)
        {
            retornoAoCliente = Encoding.UTF8.GetBytes($"Erro ao executar operação: {ex.Message}");

        }
        finally
        {
            var sizeResponse = retornoAoCliente.Length;
            _handler.Send(BitConverter.GetBytes(sizeResponse), SocketFlags.None);
            _handler.Send(retornoAoCliente, SocketFlags.None);
        }
    }

    _handler.Close();
}