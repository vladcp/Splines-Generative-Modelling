using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RingExtrudeShape : ExtrudeShape {
    // create vertices and line indices array for a circle with a resolution
    [Range(3, 32)]
    public int segmentCount = 3;

    [Range(1f, 5f)]
    public float radius = 3f;

    public Mesh surfaceMesh; // the 2D mesh of the object
    private void Awake() {
        surfaceMesh = new Mesh();
        surfaceMesh.name = "2D cross section";
        GenerateRing();
    }
    private void OnValidate() {
        GenerateRing();
    }

    // TODO add a mesh renderer too
    public void GenerateMesh() {

        surfaceMesh.Clear();
        Vector3[] meshVerts = new Vector3[VertexCount];
        Vector3[] meshNormals = new Vector3[VertexCount];
        for (int i = 0; i < VertexCount; i++) {
            meshVerts[i] = vertices[i].point;
            meshNormals[i] = vertices[i].normal;
            Debug.Log("Done");
        }
        int triangleCount = segmentCount - 2;
        int id = 0;
        int[] triangleIndices = new int[triangleCount * 3];
        for (int i = 0; i < triangleCount; i+=1) {
            triangleIndices[id] = 0;    id++;
            triangleIndices[id] = i+2; id++;
            triangleIndices[id] = i+1;   id++;
        }
        surfaceMesh.SetVertices(meshVerts);
        surfaceMesh.SetTriangles(triangleIndices, 0);
    }

    private void GenerateRing() {
        vertices = new Vertex[segmentCount];
        for (int i = 0; i < segmentCount; i++) {
            float t = i / (float)segmentCount;
            float angRad = - t * Mathfs.TAU;
            Vector2 dir = Mathfs.GetVectorByAngle(angRad);

            vertices[i] = new Vertex { 
                point = dir * radius, 
                normal = dir 
            };
        }
        lineIndices = new int[segmentCount*2];
        lineIndices[0] = 0; lineIndices[lineIndices.Length - 1] = 0;
        for (int i = 1; i < lineIndices.Length - 1; i+=2) {
            lineIndices[i] = (i+1)/2;
            lineIndices[i + 1] = (i+1)/2;
        }

        //mesh.SetVertices(vertices);
        //mesh.SetTriangles(triangleIndices, 0);
        //mesh.SetNormals(normals);
        //mesh.SetUVs(0, uvs);
    }
}
