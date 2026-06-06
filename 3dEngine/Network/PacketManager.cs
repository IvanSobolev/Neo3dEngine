namespace _3dEngine.Network;

public class PacketManager
{
    private static readonly Dictionary<int, Type> _idToType = new();
    
    private static readonly Dictionary<Type, int> _typeToId = new();

    private static readonly Dictionary<Type, Action<INetworkPacket, int>> _handlers = new();

    public static void RegisterPacket<T>() where T : INetworkPacket, new()
    {
        Type type = typeof(T);
        
        int id = GetStableHash(type.Name);

        if (_idToType.ContainsKey(id)) return;

        _idToType[id] = type;
        _typeToId[type] = id;
        
        Console.WriteLine($"[Network] Registered packet '{type.Name}' with ID {id}");
    }
    
    private static int GetStableHash(string str)
    {
        unchecked
        {
            int hash = 23;
            foreach (char c in str)
                hash = hash * 31 + c;
            return hash;
        }
    }

    public static int GetId<T>() => _typeToId[typeof(T)];
    public static int GetId(Type t) => _typeToId[t];

    public static INetworkPacket CreateInstance(int id)
    {
        if (!_idToType.TryGetValue(id, out Type type))
            throw new Exception($"Unknown packet ID: {id}");
            
        return (INetworkPacket)Activator.CreateInstance(type)!;
    }

    public static void Subscribe<T>(Action<T, int> handler) where T : INetworkPacket
    {
        Type type = typeof(T);
        if (!_handlers.ContainsKey(type)) _handlers[type] = delegate { };

        _handlers[type] += (packet, senderId) => handler((T)packet, senderId);
    }

    public static void InvokeHandler(INetworkPacket packet, int senderId)
    {
        Type type = packet.GetType();
        if (_handlers.TryGetValue(type, out var handler))
        {
            handler(packet, senderId);
        }
    }
}