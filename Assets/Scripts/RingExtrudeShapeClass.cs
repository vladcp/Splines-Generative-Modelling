using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingExtrudeShapeClass {

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

    private int edgeCount = 8;
    public int EdgeCount {
        get { return edgeCount; }
        set {
            edgeCount = value;
            GenerateShapeSoftEdges();
        }
    }

    private float radius = 3f;
    public float Radius {
        get { return radius; }
        set {
            radius = value;
            GenerateShapeSoftEdges();
        }
    }

    public int hardEdgeThreshold; // upper limit for segmentCount

    public Mesh surfaceMesh; 

    #region Constructors
    // Constructors
    public RingExtrudeShapeClass(float r) {
        radius = r;
        GenerateShapeSoftEdges();
    }

    public RingExtrudeShapeClass(float r, int eCount) {
        radius = r;
        edgeCount = eCount;
        GenerateShapeSoftEdges();
    }
    #endregion
    
    // TODO add a mesh renderer too
    public Mesh GenerateMesh() {
        surfaceMesh = new Mesh();
        surfaceMesh.Clear();
        Vector3[] meshVerts = new Vector3[VertexCount];
        Vector3[] meshNormals = new Vector3[VertexCount];
        for (int i = 0; i < VertexCount; i++) {
            meshVerts[i] = vertices[i].point;
            meshNormals[i] = vertices[i].normal;
            Debug.Log("Done");
        }
        int triangleCount = edgeCount - 2;
        int id = 0;
        int[] triangleIndices = new int[triangleCount * 3];
        for (int i = 0; i < triangleCount; i+=1) {
            triangleIndices[id] = 0;    id++;
            triangleIndices[id] = i+2;  id++;
            triangleIndices[id] = i+1;  id++;
        }
        surfaceMesh.SetVertices(meshVerts);
        surfaceMesh.SetTriangles(triangleIndices, 0);

        return surfaceMesh;
    }

    private void GenerateShapeSoftEdges() {
        vertices = new Vertex[edgeCount];
 
        // define vertices
        for (int i = 0; i < edgeCount; i++) {
            float t = i / (float)edgeCount;
            float angRad = - t * Mathfs.TAU;
            Vector2 dir = Mathfs.GetVectorByAngle(angRad);

            vertices[i] = new Vertex { 
                point = dir * radius, 
                normal = dir 
            };
        }
        // define the connection between vertices
        lineIndices = new int[edgeCount*2];
        lineIndices[0] = 0; lineIndices[lineIndices.Length - 1] = 0;
        for (int i = 1; i < lineIndices.Length - 1; i+=2) {
            lineIndices[i] = (i+1)/2;
            lineIndices[i + 1] = (i+1)/2;
        }
    }

    private void GenerateShapeHardEdges() {
        vertices = new Vertex[edgeCount * 2];
        float t1, t2;
        float angRad1 = 0f, angRad2;
        Vector2 dir1 = Mathfs.GetVectorByAngle(angRad1), dir2;
        
        // define vertices
        for(int i = 0; i < edgeCount * 2; i += 2) {
            t1 = (i * .5f) / (float) edgeCount;
            t2 = (i*.5f+1) / (float)edgeCount;
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
        lineIndices = new int[edgeCount*2];
        for(int i = 0; i < lineIndices.Length; i ++) {
            lineIndices[i] = i;
        }
    }
}
