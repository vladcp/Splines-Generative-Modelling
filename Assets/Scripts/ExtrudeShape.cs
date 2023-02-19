using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2D Cross Section
[CreateAssetMenu]
public class ExtrudeShape : ScriptableObject {

    [System.Serializable]
    public class Vertex {
        public Vector2 point;
        public Vector2 normal;
        public float u; // UVs, without v, for texture mapping
        // bitangent, vertex color, etc
    }

    public Vertex[] vertices;
    public int[] lineIndices; // for defining the connectivity of the points

    
    public int VertexCount => vertices.Length;
    public int LineCount => lineIndices.Length;

}
