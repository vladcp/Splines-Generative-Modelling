using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class QuadRing : MonoBehaviour {
    [Range(0.01f, 1)]
    [SerializeField] float radiusInner;

    [Range(0.01f, 1)]
    [SerializeField] float thickness;

    [Range(3, 32)]
    [SerializeField] int angularSegmentCount = 3;

    Mesh mesh;
    float RadiusOuter => radiusInner + thickness;
    int VertexCount => angularSegmentCount * 2;

    private void OnDrawGizmosSelected() {
        Gizmosfs.DrawWireCircle(transform.position, transform.rotation, radiusInner, angularSegmentCount);
        Gizmosfs.DrawWireCircle(transform.position, transform.rotation, RadiusOuter, angularSegmentCount);
    }
 
    void Awake() {
        mesh = new Mesh();
        mesh.name = "QuadRing";
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    void Update() {
        GenerateMesh();
    }

    void GenerateMesh() {
        mesh.Clear();
        int vCount = VertexCount;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < angularSegmentCount + 1; i++) { // without +1 (without uvs)
            float t = i / (float)angularSegmentCount;
            float angRad = t * Mathfs.TAU;
            Vector2 dir = Mathfs.GetVectorByAngle(angRad);
            
            vertices.Add(dir * RadiusOuter);
            vertices.Add(dir * radiusInner);
            normals.Add(Vector3.forward);
            normals.Add(Vector3.forward);

            uvs.Add(new Vector2(t, 1));
            uvs.Add(new Vector2(t, 0));

            // uvs.Add(dir * 0.5f + Vector2.one * 0.5f)
            // uvs.Add(dir * (radiusInner / RadiusOuter * 0.5f + Vector2.one * 0.5f)
        }

        List<int> triangleIndices = new List<int>();
        for (int i = 0; i < angularSegmentCount; i++) {
            int indexRoot = i * 2;
            int indexInnerRoot = indexRoot + 1;
            int indexOuterNext = (indexRoot + 2); // %vcount without uvs
            int indexInnerNext = (indexRoot + 3); // %vcount without uvs

            triangleIndices.Add(indexRoot);
            triangleIndices.Add(indexOuterNext);
            triangleIndices.Add(indexInnerNext);

            triangleIndices.Add(indexRoot);
            triangleIndices.Add(indexInnerNext);
            triangleIndices.Add(indexInnerRoot);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangleIndices, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
    }
}
