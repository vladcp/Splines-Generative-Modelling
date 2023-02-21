using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMeshGenerator : MonoBehaviour
{
    Mesh mesh;

    // generate a line; boundaries
    public Transform start;
    public Transform end;
    public int stepCount;
    // or simply generate vertices
    // i.e. Vector3[]

    void Awake() {
        mesh = new Mesh();
        mesh.name = "NoisyMesh";
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GenerateMesh();
    }

    void GenerateMesh() {
        mesh.Clear();
        
        // generate one vertex in the mesh
        // Vector3 vertex = new Vector3(0f, 0f, 0f);
        List<Vector3> verts = new List<Vector3>();
        for(int i = 0; i < stepCount; i++) {
            float t = i / (stepCount - 1f);
            verts.Add(Vector3.Lerp(start.position, end.position, t));
            Debug.Log(verts[i]);
        }
        mesh.SetVertices(verts);
    }

    public void OnDrawGizmos() {
        if (mesh != null) {
            // draw all vertices
            for(int i = 0; i < mesh.vertexCount; i++){
                Gizmos.DrawSphere(mesh.vertices[i], 0.01f);
            }
        }
    }
}
