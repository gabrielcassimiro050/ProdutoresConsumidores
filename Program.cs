using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;    

BlockingCollection <(int pedido, int prato)> pedidos = new BlockingCollection<(int pedido, int prato)>();

var porcoes = new int[4];
var porcoesString = new string[4];

const int ARROZ = 0, CARNE = 1, MACARRAO = 2, MOLHO = 3;

porcoesString[ARROZ] = "Arroz";
porcoesString[CARNE] = "Carne";
porcoesString[MACARRAO] = "Macarrão";
porcoesString[MOLHO] = "Molho";

const int tempoDePreparo = 2000;

object lockConsole = new object();

void ConsoleLock(string msg, ConsoleColor cor)
{
    lock (lockConsole)
    {
        var aux = Console.ForegroundColor;
        Console.ForegroundColor = cor;
        Console.WriteLine(msg);
        Console.ForegroundColor = aux;
    }
}

void GarcomSpeech(string msg){
    ConsoleLock("                         ", ConsoleColor.Blue);
    ConsoleLock("              ______     ", ConsoleColor.Blue);
    ConsoleLock("             /      \\   ", ConsoleColor.Blue);
    ConsoleLock("            |        |	  ", ConsoleColor.Blue);
    ConsoleLock("            |        |   ", ConsoleColor.Blue);
    ConsoleLock("            |        |   ", ConsoleColor.Blue);
    ConsoleLock("            |        |   "+msg, ConsoleColor.Blue);
    ConsoleLock("            |        |   ", ConsoleColor.Blue);
    ConsoleLock("            | o    o |   ", ConsoleColor.Blue);
    ConsoleLock("           ||___^^___||  ", ConsoleColor.Blue);
    ConsoleLock("            |___||___|   ", ConsoleColor.Blue);
    ConsoleLock("              T    T     ", ConsoleColor.Blue);
}

void ChefSpeech(string msg){
    ConsoleLock("              ______     ", ConsoleColor.Red);
    ConsoleLock("             / |  | \\   ", ConsoleColor.Red);
    ConsoleLock("            | |    | |   ", ConsoleColor.Red);
    ConsoleLock("            | |    | |	  ", ConsoleColor.Red);
    ConsoleLock("             \\_|__|_/   ", ConsoleColor.Red);
    ConsoleLock("              |____|     ", ConsoleColor.Red);
    ConsoleLock("             /      \\   "+msg, ConsoleColor.Red);
    ConsoleLock("            |        |   ", ConsoleColor.Red);
    ConsoleLock("            | o    o |   ", ConsoleColor.Red);
    ConsoleLock("           ||   /\\   || ", ConsoleColor.Red);
    ConsoleLock("            |________|   ", ConsoleColor.Red);
    ConsoleLock("              T    T     ", ConsoleColor.Red);
}

int pedido = 1;

void Garcom(int index)
{
    Random rnd = new Random();
    

    var id = Task.CurrentId;
    Console.WriteLine($"[Garcom {index}] Estou pronto!!!");

    while (true)
    {
        int tempo = rnd.Next(1000, 10_000);
        int prato = rnd.Next(1, 4);
        Thread.Sleep(tempo);

        GarcomSpeech($"[Garcom {index}] Peguei o pedido {pedido} do prato {prato}");

        pedidos.Add((pedido, prato));
        pedido++;
    }
}

void Chef(int index){
    Console.WriteLine($"[Chef {index}] Estou pronto!!!");
    
    foreach (var (pedido, prato) in pedidos.GetConsumingEnumerable())
    {
        ChefSpeech($"[Chef {index}] Iniciando o pedido {pedido} do prato {prato}");
        
        prepararPrato(prato, index);

        ChefSpeech($"[Chef {index}] Finalizei o pedido {pedido} do prato {prato}");
    }
}

void prepararComponente(int componente, int nPorcoes, int chef){
    ChefSpeech($"[Chef {chef}] Preparando {nPorcoes} porções de {porcoesString[componente]}");
    Thread.Sleep(tempoDePreparo);
    porcoes[componente] += nPorcoes;
                     
  
    ChefSpeech($"[Chef {chef}] Finalizei a produção de {nPorcoes} porções de {porcoesString[componente]}. Estoque: {porcoes[componente]}");
}

void prepararPrato(int prato, int chef){
    switch(prato){
        case 1:
            if(porcoes[ARROZ]==0){
                
                prepararComponente(ARROZ, 3, chef);
            }
            if(porcoes[CARNE]==0){
                prepararComponente(CARNE, 2, chef);
            }
            Thread.Sleep(2000);
        break;

        case 2:
            if(porcoes[MACARRAO]==0){
                prepararComponente(MOLHO, 4, chef);
            }
            if(porcoes[MOLHO]==0){
                prepararComponente(MOLHO, 2, chef);
            }
            Thread.Sleep(2000);
        break;

        case 3:
            if(porcoes[ARROZ]==0){
                prepararComponente(ARROZ, 3, chef);
            }
            if(porcoes[CARNE]==0){
                prepararComponente(CARNE, 2, chef);
            }
            if(porcoes[MOLHO]==0){
                prepararComponente(MOLHO, 2, chef);
            }
            Thread.Sleep(3000);
        break;
    }
}


var g = Enumerable.Range(1,5).Select(i => Task.Run(() => Garcom(i))).ToList();
var c = Enumerable.Range(1,3).Select(i => Task.Run(() => Chef(i))).ToList();
//var c = Task.Run(Chef);

Task.WaitAll(c.ToArray());
Task.WaitAll(g.ToArray());

//c.WaitAll(g);