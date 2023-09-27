# ServidorTCP
Servidor TCP local para CRUD da entidade "Tarefa"

**1 - Estrutura**

O sistema é um CRUD para gerenciamento de tarefas (To do list) por meio de uma conexão TCP entre cliente e servidor. A implementação foi feita toda com a linguagem C# e o framework .NET 7 em ambos os lados da aplicação, portanto o seu funcionamento depende da instalação do [.NET 7 SDK.](https://dotnet.microsoft.com/pt-br/download/dotnet/7.0) e é consumido pela aplicação [Cliente TCP](https://github.com/schmoellerIuri/ClienteTCP)

A classe a ser manipulada foi definida como Tarefa e será salva em uma lista de objetos no servidor, ou seja, não é salva após seu desligamento. A estrutura do objeto é a seguinte:
<pre>
public class Tarefa
{
    	public int Id { get; set; }
    	public string? Descricao { get; set; }
    	public StatusTarefa Status { get; set; } = StatusTarefa.A_FAZER;
    	public DateTime DataEntrega { get; set; }
    	public string? Responsavel { get; set; }
}
</pre>

**2 - Funcionalidades**

**2.1 - Confidencialidade por KEA(Key Exchange Algorithm)**
O servidor possui criptografia simétrica para cifrar todos os pacotes que são trocados entre as pontas de comunicação pelo padrão AES. A chave privada utilizada entre as partes é obtida por um KEA, que é um processo que utiliza a criptografia assimétrica com o algoritmo Diffie-Hellman em um canal não seguro para obter um segredo compartilhado que será utilizado como chave privada:

![KEA](https://www.practicalnetworking.net/wp-content/uploads/2015/11/dh-revised.png)
