using System.Collections.Concurrent;  
using System.Net;  
using System.Net.Sockets;  
  
namespace _3dEngine.Network;  
  
public class NetworkManager  
{  
    private TcpClient _client;  
    private TcpListener _server;  
    private NetworkStream _stream;  
    private readonly ConcurrentQueue<(INetworkPacket packet, int senderId)> _packetQueue = new();  
  
    public void StartServer(int port)  
    {  
        _server = new TcpListener(IPAddress.Any, port);  
        _server.Start();  
        Task.Run(AcceptClientsAsync);  
    }  
  
    public void Connect(string ip, int port)  
    {  
        _client = new TcpClient();  
        _client.Connect(ip, port);  
        _stream = _client.GetStream();  
        Task.Run(ReceiveLoopAsync);  
    }  
  
    public void SendPacket<T>(T packet, int senderId) where T : INetworkPacket  
    {  
        if (_stream == null) return;  
  
        int packetTypeId = PacketManager.GetId(packet.GetType());  
  
        using var packetMs = new MemoryStream();  
        using var packetWriter = new BinaryWriter(packetMs);  
        packet.Serialize(packetWriter);  
        byte[] packetData = packetMs.ToArray();  
  
        using var finalMs = new MemoryStream();  
        using var writer = new BinaryWriter(finalMs);  
  
        writer.Write(packetTypeId);  
        writer.Write(senderId);  
        writer.Write(packetData.Length);  
        writer.Write(packetData);  
  
        byte[] finalBytes = finalMs.ToArray();  
          
        try { _stream.Write(finalBytes); } catch {}  
    }  
  
    private async Task ReceiveLoopAsync()  
    {  
        // Заголовок: TypeId(4) + SenderId(4) + Length(4) = 12 байт  
        byte[] header = new byte[12];   
  
        while (_client != null && _client.Connected)  
        {  
            try  
            {  
                int read = await _stream.ReadAsync(header, 0, 12);  
                if (read == 0) break;  
  
                using var ms = new MemoryStream(header);  
                using var reader = new BinaryReader(ms);  
  
                int typeId = reader.ReadInt32();  
                int senderId = reader.ReadInt32();  
                int length = reader.ReadInt32();  
  
                byte[] data = new byte[length];  
                int totalRead = 0;  
                while (totalRead < length)  
                {  
                    totalRead += await _stream.ReadAsync(data, totalRead, length - totalRead);  
                }  
  
                INetworkPacket packet = PacketManager.CreateInstance(typeId);  
                  
                using var dataMs = new MemoryStream(data);  
                using var dataReader = new BinaryReader(dataMs);  
                packet.Deserialize(dataReader);  
  
                _packetQueue.Enqueue((packet, senderId));  
            }  
            catch { break; }  
        }  
    }  
      
    private async Task AcceptClientsAsync()  
    {  
        while (true)  
        {  
            var client = await _server.AcceptTcpClientAsync();  
            _client = client;   
            _stream = _client.GetStream();  
            Task.Run(ReceiveLoopAsync);  
        }  
    }  
  
    public void ProcessEvents()  
    {  
        while (_packetQueue.TryDequeue(out var item))  
        {  
            PacketManager.InvokeHandler(item.packet, item.senderId);  
        }  
    }  
}