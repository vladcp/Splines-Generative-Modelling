using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2D Cross Section
[CreateAssetMenu]
public class ExtrudeShape : ScriptableObject
{

  [System.Serializable]
  public class Vertex
  {
    public Vector2 point;
    public Vector2 normal;
    public float u; // UVs, without v, for texture mapping
                    // bitangent, vertex color, etc
  }

  public Vertex[] vertices;
  public int[] lineIndices; // for defining the connectivity of the points

  public int VertexCount => vertices.Length;
  public int LineCount => lineIndices.Length;

  // public Vertex[] noisyVerts;

  // public void ApplyNoise()
  // {
  //   noisyVerts = new Vertex[VertexCount];
  //   // apply noise to each vertex in the normal direction ??
  //   for (int i = 0; i < VertexCount; i++)
  //   {
  //     // x y and normal
  //     float n = Mathf.PerlinNoise(vertices[i].point.x * .5f, vertices[i].point.y * .5f) * 5f;
  //     Vector2 newPos = vertices[i].point + vertices[i].normal * n;
  //     noisyVerts[i] = new Vertex { point = newPos, normal = vertices[i].normal };
  //   }
  // }

}
