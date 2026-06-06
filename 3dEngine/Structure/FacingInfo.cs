namespace _3dEngine;

public struct FacingInfo(int[] vertexIndex, int normalIndex)
{
    public readonly int Vertex1 = vertexIndex[0];
    public readonly int Vertex2 = vertexIndex[1];
    public readonly int Vertex3 = vertexIndex[2];
    public readonly int NormalIndex = normalIndex;
}