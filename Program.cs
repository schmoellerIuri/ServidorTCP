using System.Net;
using System.Net.Sockets;
using PraticaSockets;

Console.Clear();

string ip = "127.0.0.1";
int port = 50000;

IPEndPoint ipEndPoint = new(IPAddress.Parse(ip), port);

var servidor = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

servidor.Bind(ipEndPoint);
servidor.Listen(10);
int i = 0;

while (true)
{
    var handler = await servidor.AcceptAsync();
    var thread = new Thread(DoWork)
    {
        Name = "Thread " + i++
    };
    thread.Start(handler);
}


void DoWork(object handler)
{
    var _handler = (Socket)handler;

    #region Troca de chaves para segredo compartilhado
    var clientPublicKey = Array.Empty<byte>();
    try
    {
        var sizeOfPublicKeyBytes = BitConverter.GetBytes(CryptoManager.PublicKey.Length);
        _handler.Send(sizeOfPublicKeyBytes, SocketFlags.None);
        _handler.Send(CryptoManager.PublicKey, SocketFlags.None);

        var sizeOfClientPublicKeyBytes = new byte[4];
        _handler.Receive(sizeOfClientPublicKeyBytes, SocketFlags.None);

        int sizeOfClientPublicKey = BitConverter.ToInt32(sizeOfClientPublicKeyBytes, 0);

        clientPublicKey = new byte[sizeOfClientPublicKey];
        _handler.Receive(clientPublicKey, SocketFlags.None);
    }
    catch
    {
        Console.WriteLine($"Erro ao receber chave pública do cliente: {Thread.CurrentThread.Name}");
        return;
    }
    #endregion

    #region Autenticação do cliente
    string authMessage = "|<A>|Auth Message!";
    string rsaPublicParams = "";
    try
    {
        SendEncryptedMessage(_handler, authMessage, clientPublicKey);

        rsaPublicParams = ReceiveDecryptedMessage(_handler, clientPublicKey);

        var authSignature = ReceiveDecryptedMessage(_handler, clientPublicKey, true);

        var hashedAuthMessage = ReceiveDecryptedMessage(_handler, clientPublicKey, true);

        if (!CryptoManager.Authenticate(rsaPublicParams, authMessage, authSignature, hashedAuthMessage))
        {
            var message = "|<E>|Falha na autenticação!";
            SendEncryptedMessage(_handler, message, clientPublicKey);

            Console.WriteLine($"Falha na autenticação: {Thread.CurrentThread.Name}");
            return;
        }
    }
    catch (Exception)
    {
        var message = "|<E>|Falha na autenticação!";
        SendEncryptedMessage(_handler, message, clientPublicKey);

        Console.WriteLine($"Erro ao receber mensagem de autenticação: {Thread.CurrentThread.Name}");
        return;
    }

    var messageSucces = "|<S>|Autenticação realizada com sucesso!";
    SendEncryptedMessage(_handler, messageSucces, clientPublicKey);
    #endregion

    while (true)
    {
        string request = "";
        try
        {
            request = ReceiveDecryptedMessage(_handler, clientPublicKey);
        }
        catch
        { break; }


        if (string.IsNullOrEmpty(request))
            continue;

        string operacao = request[..5];
        string retornoAoCliente = "";

        if (string.Equals(operacao, "|<E>|"))
            break;


        try
        {
            switch (operacao)
            {
                case "|<C>|":
                    var id = TarefasManager.CriarTarefa(DecoderTarefas.DecodeTarefa(request[5..], false));
                    retornoAoCliente = "Tarefa criada com sucesso! Id: " + id.ToString();
                    break;
                case "|<R>|":
                    var idRequisitado = int.Parse(request[5..]);
                    retornoAoCliente = idRequisitado == 0 ? EncoderTarefas.EncodeListaDeTarefas(TarefasManager.LerTarefas()) : EncoderTarefas.EncodeTarefa(TarefasManager.LerTarefaPorId(idRequisitado));
                    break;
                case "|<U>|":
                    TarefasManager.AtualizarTarefa(DecoderTarefas.DecodeTarefa(request[5..], true));
                    retornoAoCliente = "Tarefa atualizada com sucesso!";
                    break;
                case "|<D>|":
                    TarefasManager.RemoverTarefa(int.Parse(request[5..]));
                    retornoAoCliente = "Tarefa removida com sucesso!";
                    break;
                default:
                    retornoAoCliente = "Operação inválida";
                    break;
            }
        }
        catch (Exception ex)
        {
            retornoAoCliente = $"Erro ao executar operação: {ex.Message}";

        }
        finally
        {
            SendEncryptedMessage(_handler, retornoAoCliente, clientPublicKey);
        }
    }

    _handler.Close();
}

void SendEncryptedMessage(Socket handler, string message, byte[] clientPublicKey)
{
    var encryptedRequest = CryptoManager.Encrypt(message, clientPublicKey);

    var sizeOfIVBytes = BitConverter.GetBytes(encryptedRequest.IV.Length);
    handler.Send(sizeOfIVBytes, SocketFlags.None);

    handler.Send(encryptedRequest.IV, SocketFlags.None);

    var sizeOfRequestBytes = BitConverter.GetBytes(encryptedRequest.encryptedMessage.Length);
    handler.Send(sizeOfRequestBytes, SocketFlags.None);

    handler.Send(encryptedRequest.encryptedMessage, SocketFlags.None);
}

dynamic ReceiveDecryptedMessage(Socket handler, byte[] clientPublicKey, bool receiveBytes = false)
{
    var sizeOfIVResponseBytes = new byte[4];
    handler.Receive(sizeOfIVResponseBytes, SocketFlags.None);
    int sizeOfIVResponse = BitConverter.ToInt32(sizeOfIVResponseBytes);

    var ivResponse = new byte[sizeOfIVResponse];
    handler.Receive(ivResponse, SocketFlags.None);

    var sizeOfResponseBytes = new byte[4];
    handler.Receive(sizeOfResponseBytes, SocketFlags.None);
    int sizeOfResponse = BitConverter.ToInt32(sizeOfResponseBytes);

    var responseBytes = new byte[sizeOfResponse];
    handler.Receive(responseBytes, SocketFlags.None);
    var response = CryptoManager.Decrypt(responseBytes, ivResponse, clientPublicKey, receiveBytes);

    return response;
}
