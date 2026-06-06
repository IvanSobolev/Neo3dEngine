using _3dEngine;
using _3dEngine.Network;

namespace SampleGame.NetworkPackets;

public class TransformPacket : INetworkPacket
{
    public Vector3 Pos;
    public Vector3 Rot;

    public TransformPacket() { } 

    public TransformPacket(Vector3 p, Vector3 r) { Pos = p; Rot = r; }

    public void Serialize(BinaryWriter w)
    {
        w.Write(Pos.X); w.Write(Pos.Y); w.Write(Pos.Z);
        w.Write(Rot.X); w.Write(Rot.Y); w.Write(Rot.Z);
    }

    public void Deserialize(BinaryReader r)
    {
        Pos = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
        Rot = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
    }
}