namespace _3dEngine.Network;

public interface INetworkPacket
{
    void Serialize(BinaryWriter writer);
    
    void Deserialize(BinaryReader reader);
}