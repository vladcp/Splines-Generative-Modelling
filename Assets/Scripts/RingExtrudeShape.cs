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

    // max number of segments where the object is considered to have 'hard edges'
    // in order to calculate normals
    [Range(3, 30)]
    public int hardEdgeThreshold;

    public Mesh surfaceMesh; // the 2D mesh of the object

    private void Awake() {
        surfaceMesh = new Mesh();
        surfaceMesh.name = "2D cross section";
        if (segmentCount > hardEdgeThreshold) {
            GenerateShapeSoftEdges();
        } else {
            GenerateShapeHardEdges();
        }
    }

    private void OnValidate() {
        if (segmentCount > hardEdgeThreshold) {
            GenerateShapeSoftEdges();
        } else {
            GenerateShapeHardEdges();
        }
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
            triangleIndices[id] = i+2;  id++;
            triangleIndices[id] = i+1;  id++;
        }
        surfaceMesh.SetVertices(meshVerts);
        surfaceMesh.SetTriangles(triangleIndices, 0);
    }

    private void GenerateShapeSoftEdges() {
        vertices = new Vertex[segmentCount];
 
        // define vertices
        for (int i = 0; i < segmentCount; i++) {
            float t = i / (float)segmentCount;
            float angRad = - t * Mathfs.TAU;
            Vector2 dir = Mathfs.GetVectorByAngle(angRad);

            vertices[i] = new Vertex { 
                point = dir * radius, 
                normal = dir 
            };
        }
        // define the connection between vertices
        lineIndices = new int[segmentCount*2];
        lineIndices[0] = 0; lineIndices[lineIndices.Length - 1] = 0;
        for (int i = 1; i < lineIndices.Length - 1; i+=2) {
            lineIndices[i] = (i+1)/2;
            lineIndices[i + 1] = (i+1)/2;
        }
    }

    private void GenerateShapeHardEdges() {
        vertices = new Vertex[segmentCount * 2];
        float t1, t2;
        float angRad1 = 0f, angRad2;
        Vector2 dir1 = Mathfs.GetVectorByAngle(angRad1), dir2;
        
        // define vertices
        for(int i = 0; i < segmentCount * 2; i += 2) {
            t1 = (i * .5f) / (float) segmentCount;
            t2 = (i*.5f+1) / (float)segmentCount;
            angRad2 = - t2 * Mathfs.TAU;
            dir2 = Mathfs.GetVectorByAngle(angRad2);
            
            Vector2 halfDir = (dir1 + dir2).normalized;

            vertices[i] = new Vertex {
                point = dir1 * radius,
                normal = halfDir
            };
            vertices[i+1] = new Vertex {
                point = dir2 * radius,
                normal = halfDir
            };

            angRad1 = angRad2;
            dir1 = dir2;
        }
        // define the connection between vertices
        lineIndices = new int[segmentCount*2];
        for(int i = 0; i < lineIndices.Length; i ++) {
            lineIndices[i] = i;
        }
    }
}
