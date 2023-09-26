# ServidorTCP
Servidor TCP local para CRUD da entidade "Tarefa"

**1 - Estrutura**

O sistema é um CRUD para gerenciamento de tarefas (To do list) por meio de uma conexão TCP entre cliente e servidor. A implementação foi feita toda com a linguagem C# e o framework .NET 7 em ambos os lados da aplicação, portanto o seu funcionamento depende da instalação do [.NET 7 SDK.](https://dotnet.microsoft.com/pt-br/download/dotnet/7.0)

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

**2 - Fluxo de execução e estrutura dos pacotes**

O servidor inicia a sua execução esperando alguma conexão TCP na porta definida para assim iniciar uma thread que corresponde a cada cliente que se conecta.

Para cada cliente é executado o KEA(Key Exchange Algorithm), onde cada ponta de comunicação envia a sua chave pública para a outra e descobrem um segredo compartilhado por meio do algoritmo Diffie-Helmann. A imagem a seguir representa de forma simples o KEA:

![Key Exchange Algorithm (Wikipedia)](https://upload.wikimedia.org/wikipedia/commons/thumb/4/4c/Public_key_shared_secret.svg/1200px-Public_key_shared_secret.svg.png)
