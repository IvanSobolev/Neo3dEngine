using _3dEngine.Shape;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3dEngine.StaticClass;
public class ObjLoader
{
    public static Object3d Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Model file not found: {filePath}");

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<FacingInfo> faces = new List<FacingInfo>();

        CultureInfo ci = CultureInfo.InvariantCulture;

        foreach (string line in File.ReadAllLines(filePath))
        {
            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string type = parts[0];

            if (type == "v")
            {
                float x = float.Parse(parts[1], ci);
                float y = float.Parse(parts[2], ci);
                float z = float.Parse(parts[3], ci);
                vertices.Add(new Vector3(x, y, z));
            }
            else if (type == "vn")
            {
                float x = float.Parse(parts[1], ci);
                float y = float.Parse(parts[2], ci);
                float z = float.Parse(parts[3], ci);
                normals.Add(new Vector3(x, y, z));
            }
            else if (type == "f")
            {

                List<int> vertexIndices = new List<int>();
                int normalIndex = 1;

                for (int i = 1; i < parts.Length; i++)
                {
                    string[] facePart = parts[i].Split('/');

                    if (int.TryParse(facePart[0], out int vIndex))
                    {
                        vertexIndices.Add(vIndex);
                    }

                    if (facePart.Length > 2 && int.TryParse(facePart[2], out int nIndex))
                    {
                        normalIndex = nIndex;
                    }
                }

                if (vertexIndices.Count >= 3)
                {
                    faces.Add(new FacingInfo(
                        new int[] { vertexIndices[0], vertexIndices[1], vertexIndices[2] },
                        normalIndex
                    ));

                    if (vertexIndices.Count == 4)
                    {
                        faces.Add(new FacingInfo(
                            new int[] { vertexIndices[0], vertexIndices[2], vertexIndices[3] },
                            normalIndex
                        ));
                    }
                }
            }
        }

       if (normals.Count == 0)
        {
            normals.Add(new Vector3(0, 1, 0));
        }

        return new Object3d(vertices.ToArray(), normals.ToArray(), faces.ToArray());
    }
}