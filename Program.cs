using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;    

BlockingCollection <(int pedido, int prato)> pedidos = new BlockingCollection<(int pedido, int prato)>();
ConcurrentDictionary <string, int> porcoes = new ConcurrentDictionary<string, int>();

porcoes.TryAdd("Arroz", 0);
porcoes.TryAdd("Carne", 0);
porcoes.TryAdd("Macarrao", 0);
porcoes.TryAdd("Molho", 0);

const int tempoDePreparo = 2000;

object lockConsole = new object();

void ConsoleLock(string msg)
{
    //Console.WriteLine(msg);
    lock (lockConsole)
    {
        Console.WriteLine(msg);
    }
}

void GarcomSpeech(string msg){
    var aux = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("                         ");
    Console.WriteLine("              ______     ");
    Console.WriteLine("             /      \\   ");
    Console.WriteLine("            |        |	");
    Console.WriteLine("            |        |   ");
    Console.WriteLine("            |        |   ");
          ConsoleLock("            |        |   "+msg);
    Console.WriteLine("            |        |   ");
    Console.WriteLine("            | o    o |   ");
    Console.WriteLine("           ||___^^___||  ");
    Console.WriteLine("            |___||___|   ");
    Console.WriteLine("              T    T     ");
    Console.ForegroundColor = aux;
}

void ChefSpeech(string msg, ConsoleColor color){
    var aux = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine("              ______     ");
    Console.WriteLine("             / |  | \\   "+$"Arroz: {porcoes["Arroz"]}");
    Console.WriteLine("            | |    | |   "+$"Carne: {porcoes["Carne"]}");
    Console.WriteLine("            | |    | |	"+$"Macarrão: {porcoes["Macarrao"]}");
    Console.WriteLine("             \\_|__|_/   "+$"Molho: {porcoes["Molho"]}");
    Console.WriteLine("              |____|     ");
          ConsoleLock("             /      \\   "+msg);
    Console.WriteLine("            |        |   ");
    Console.WriteLine("            | o    o |   ");
    Console.WriteLine("           ||   /\\   || ");
    Console.WriteLine("            |________|   ");
    Console.WriteLine("              T    T     ");
    Console.ForegroundColor = aux;
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
        ChefSpeech($"[Chef {index}] Iniciando o pedido {pedido} do prato {prato}", ConsoleColor.Red);
        
        prepararPrato(prato, index);

        ChefSpeech($"[Chef {index}] Finalizei o pedido {pedido} do prato {prato}", ConsoleColor.Red);
    }
}

void prepararComponente(string componente, int nPorcoes, int chef){
    if(porcoes[componente] > 0) return;
    porcoes[componente] += nPorcoes;
    ChefSpeech($"[Chef {chef}] Preparando {nPorcoes} porções de "+componente, ConsoleColor.Green);

    Thread.Sleep(tempoDePreparo);
    

    ChefSpeech($"[Chef {chef}] Finalizei a produção de {nPorcoes} porções de {componente}", ConsoleColor.Green);
}

void prepararPrato(int prato, int chef){
    switch(prato){
        case 1:
            Thread.Sleep(2000);
            if(porcoes["Arroz"]<=0 || porcoes["Arroz"]-1<=0){
                prepararComponente("Arroz", 3, chef);
            }
            if(porcoes["Carne"]<=0 || porcoes["Carne"]-1<=0){
                prepararComponente("Carne", 2, chef);
            }

            if(porcoes["Arroz"]>0 && porcoes["Carne"]>0){
                porcoes["Arroz"]--;
                porcoes["Carne"]--;
            }else{
                prepararPrato(prato, chef);
            }
            
        break;

        case 2:
            Thread.Sleep(2000);
            if(porcoes["Macarrao"]<=0 || porcoes["Macarrao"]-1<=0){
                prepararComponente("Macarrao", 4, chef);
            }
            if(porcoes["Molho"]<=0 || porcoes["Molho"]-1<=0){
                prepararComponente("Molho", 2, chef);
            }
            if(porcoes["Macarrao"]>0 && porcoes["Molho"]>0){
                porcoes["Macarrao"]--;
                porcoes["Molho"]--;
            }else{
                prepararPrato(prato, chef);
            }

        break;

        case 3:
           Thread.Sleep(3000);
            
            if(porcoes["Arroz"]<=0 || porcoes["Arroz"]-1<=0){
                prepararComponente("Arroz", 3, chef);
            }
            if(porcoes["Carne"]<=0|| porcoes["Carne"]-1<=0){
                prepararComponente("Carne", 2, chef);
            }
            if(porcoes["Molho"]<=0|| porcoes["Molho"]-1<=0){
                prepararComponente("Molho", 2, chef);
            }

            if(porcoes["Arroz"]>0 && porcoes["Carne"]>0 && porcoes["Molho"]>0){
                porcoes["Arroz"]--;
                porcoes["Carne"]--;
                porcoes["Molho"]--;
            }else{
                prepararPrato(prato, chef);
            }

            
        break;
    }
}


var g = Enumerable.Range(1,2).Select(i => Task.Run(() => Garcom(i))).ToList();
var c = Enumerable.Range(1,2).Select(i => Task.Run(() => Chef(i))).ToList();
//var c = Task.Run(Chef);

Task.WaitAll(c.ToArray());
Task.WaitAll(g.ToArray());

//c.WaitAll(g);