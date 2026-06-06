using _3dEngine;
using _3dEngine.Implementation;
using _3dEngine.Network;
using SampleGame.Scenes;

namespace SampleGame;

class Program
{
    static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== 3D ENGINE ONLINE SETUP ===");
        Console.ResetColor();
        
        Console.Write("Select Mode: [S]erver or [C]lient: ");
        var key = Console.ReadKey(true).Key;
        Console.WriteLine(key);
        
        bool isServer = (key == ConsoleKey.S);
        string ip = "127.0.0.1";
        int port = 7777;
        
        if (isServer)
        {
            string localIP = NetworkUtils.GetLocalIPAddress();
            Console.WriteLine($"\nYour Local IP: {localIP}");
            Console.WriteLine("Give this IP to your friend!\n");
        
            Console.Write("Enter Port to listen (default 7777): ");
            string portInput = Console.ReadLine();
            if (!int.TryParse(portInput, out port)) port = 7777;
        }
        else
        {
            Console.Write("\nEnter Server IP (default 127.0.0.1): ");
            string ipInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(ipInput)) ip = ipInput;
        
            Console.Write("Enter Server Port (default 7777): ");
            string portInput = Console.ReadLine();
            if (!int.TryParse(portInput, out port)) port = 7777;
        }
        
        Console.WriteLine($"\nStarting {(isServer ? "Server" : "Client")} on {ip}:{port}...");
        Thread.Sleep(500);
        Console.Clear();
        
        new Frame(new PriviewNetworkScene(new DisplayManagerAsync(), isServer, ip, port), new ConsoleScreenAsync()).MainLoop();
        //new Frame(new PreviewScene(new DisplayManagerAsync()), new ConsoleScreenAsync()).MainLoop();
    }
}