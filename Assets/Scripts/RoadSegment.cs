using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RoadSegment : MonoBehaviour
{

  [SerializeField] ExtrudeShape shape2D;
  [SerializeField] RingExtrudeShape ring2D;

  [Range(2, 32)]
  [SerializeField] int edgeCount = 8;

  Mesh mesh;

  [SerializeField] Transform[] controlPoints = new Transform[4];
  [Range(0, 1)]
  [SerializeField] float tTest = 0;

  Vector3 GetPos(int i) => controlPoints[i].position;
  Vector3[] pts => new Vector3[] { GetPos(0), GetPos(1), GetPos(2), GetPos(3) };

  [SerializeField] bool addNoise = false;

  void Awake() {
    mesh = new Mesh();
    mesh.name = "Segment";

    GetComponent<MeshFilter>().sharedMesh = mesh;
  }

  private void Update() {
    if(addNoise) {
      ExtrudeWithNoise(this.mesh, shape2D, GetOrientedPoints(pts, edgeCount));
    } else {
      Extrude(this.mesh, shape2D, GetOrientedPoints(pts, edgeCount));
    }
    // printPoint();
  }
  void Extrude() {
    mesh.Clear();
    // todo: pass an ExtrudeShape as parameter - can be a square, circle (maybe any shape defined by user)
    // also pass in a path of oriented points (unless the user can change the no of segments)
    // ALSO: curve subdivision according to the curvature of the curve 

    // Vertices 
    List<Vector3> verts = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    for (int ring = 0; ring < edgeCount; ring++)
    {
      float t = ring / (edgeCount - 1f);
      Debug.Log("T = " + t);
      OrientedPoint op = BezierFs.GetOrientedPoint(pts, t, Vector3.up);

      for (int i = 0; i < shape2D.VertexCount; i++)
      {
        verts.Add(op.LocalToWorldPos(shape2D.vertices[i].point));
        normals.Add(op.LocalToWorldVec(shape2D.vertices[i].normal));
      }
    }

    Debug.Log("First vertex: " + verts[0]);
    // Triangles
    List<int> triIndices = new List<int>();
    for (int ring = 0; ring < edgeCount - 1; ring++) {
      int rootIndex = ring * shape2D.VertexCount;
      int rootIndexNext = (ring + 1) * shape2D.VertexCount;

      for (int line = 0; line < shape2D.LineCount; line += 2) {
        int lineIndexA = shape2D.lineIndices[line];
        int lineIndexB = shape2D.lineIndices[line + 1];

        int currentA = rootIndex + lineIndexA;
        int currentB = rootIndex + lineIndexB;
        int nextA = rootIndexNext + lineIndexA;
        int nextB = rootIndexNext + lineIndexB;

        triIndices.Add(currentA);
        triIndices.Add(nextA);
        triIndices.Add(nextB);

        triIndices.Add(currentA);
        triIndices.Add(nextB);
        triIndices.Add(currentB);
      }
    }

    mesh.SetVertices(verts);
    mesh.SetTriangles(triIndices, 0);
    //mesh.RecalculateNormals();
    mesh.SetNormals(normals);

  }

  OrientedPoint[] GetOrientedPoints(Vector3[] controlPoints, int edgeCount) {
    OrientedPoint[] path = new OrientedPoint[edgeCount];
    for (int i = 0; i < edgeCount; i++) {
      float t = i / (edgeCount - 1f);
      path[i] = BezierFs.GetOrientedPoint(controlPoints, t, Vector3.up);
    }
    return path;
  }
  void Extrude(Mesh mesh, ExtrudeShape shape, OrientedPoint[] path) {
    int vertsInShape = shape.VertexCount;
    int segments = path.Length - 1;
    int edgeLoops = path.Length;
    int vertCount = vertsInShape * edgeLoops;
    int triCount = shape.LineCount * segments;
    int triIndexCount = triCount * 3;

    int[] triangleIndices = new int[triIndexCount];
    Vector3[] vertices = new Vector3[vertCount];
    Vector3[] normals = new Vector3[vertCount];
    Vector2[] uvs = new Vector2[vertCount];
    Color[] colors = new Color[vertCount];
    Color baseColor = Color.grey;

    for (int i = 0; i < path.Length; i++) {
      int offset = i * vertsInShape;
      for (int j = 0; j < vertsInShape; j++) {
        int id = offset + j;
        vertices[id] = path[i].LocalToWorldPos(shape.vertices[j].point);
        normals[id] = path[i].LocalToWorldVec(shape.vertices[j].normal);
        colors[id] = baseColor;
        // uv
      }
    }
    int ti = 0;
    for (int i = 0; i < segments; i++) {
      int offset = i * vertsInShape;
      for (int l = 0; l < shape.LineCount; l += 2) {
        int a = offset + shape.lineIndices[l] + vertsInShape;
        int b = offset + shape.lineIndices[l];
        int c = offset + shape.lineIndices[l + 1];
        int d = offset + shape.lineIndices[l + 1] + vertsInShape;
        triangleIndices[ti] = b; ti++;
        triangleIndices[ti] = a; ti++;
        triangleIndices[ti] = d; ti++;
        triangleIndices[ti] = b; ti++;
        triangleIndices[ti] = d; ti++;
        triangleIndices[ti] = c; ti++;
      }
    }

    mesh.Clear();
    mesh.vertices = vertices;
    mesh.triangles = triangleIndices;
    mesh.normals = normals;
    mesh.colors = colors;
    //mesh.uv = uvs;
  }

  void ExtrudeWithNoise(Mesh mesh, ExtrudeShape shape, OrientedPoint[] path) {
    int vertsInShape = shape.VertexCount;
    int segments = path.Length - 1;
    int edgeLoops = path.Length;
    int vertCount = vertsInShape * edgeLoops;
    int triCount = shape.LineCount * segments;
    int triIndexCount = triCount * 3;

    int[] triangleIndices = new int[triIndexCount];
    Vector3[] vertices = new Vector3[vertCount];
    Vector3[] normals = new Vector3[vertCount];
    Vector2[] uvs = new Vector2[vertCount];

    Color[] colors = new Color[vertCount];
    Color baseColor = Color.grey;

    for (int i = 0; i < path.Length; i++) {
      int offset = i * vertsInShape;
      for (int j = 0; j < vertsInShape; j++) {
        int id = offset + j;
        vertices[id] = path[i].LocalToWorldPos(shape.vertices[j].point);
        normals[id] = path[i].LocalToWorldVec(shape.vertices[j].normal);
        //noise
        float n = Mathf.PerlinNoise(vertices[id].x, vertices[id].y) * 5f;
        vertices[id] = vertices[id] + normals[id] * n;
        colors[id] = baseColor * n;
        // uv
      }
    }
    int ti = 0;
    for (int i = 0; i < segments; i++) {

      int offset = i * vertsInShape;
      for (int l = 0; l < shape.LineCount; l += 2) {
        int a = offset + shape.lineIndices[l] + vertsInShape;
        int b = offset + shape.lineIndices[l];
        int c = offset + shape.lineIndices[l + 1];
        int d = offset + shape.lineIndices[l + 1] + vertsInShape;
        triangleIndices[ti] = b; ti++;
        triangleIndices[ti] = a; ti++;
        triangleIndices[ti] = d; ti++;
        triangleIndices[ti] = b; ti++;
        triangleIndices[ti] = d; ti++;
        triangleIndices[ti] = c; ti++;
      }
    }

    mesh.Clear();
    mesh.vertices = vertices;
    mesh.triangles = triangleIndices;
    mesh.normals = normals;
    mesh.colors = colors;
    //mesh.uv = uvs;
  }


  public void OnDrawGizmos()
  {
    // draw bezier control points
    for (int i = 0; i < 4; i++) {
      Gizmos.DrawSphere(GetPos(i), 0.03f);
    }
    Handles.DrawBezier(GetPos(0), GetPos(3), GetPos(1), GetPos(2), Color.white, EditorGUIUtility.whiteTexture, 1f);
    Gizmos.color = Color.red;

    OrientedPoint testPoint = BezierFs.GetOrientedPoint(pts, tTest, Vector3.up);

    Gizmos.DrawSphere(testPoint.pos, 0.07f);
    Handles.PositionHandle(testPoint.pos, testPoint.rot);

    void DrawPoint(Vector2 localPos) => Gizmos.DrawSphere(testPoint.LocalToWorldPos(localPos), 0.1f);

    Vector3[] localVerts = shape2D.vertices.Select(v => testPoint.LocalToWorldPos(v.point)).ToArray();
    Vector3[] localNormals = shape2D.vertices.Select(v => testPoint.LocalToWorldVec(v.normal)).ToArray();

    // draw cross section outline
    for (int i = 0; i < shape2D.VertexCount; i++) {
      DrawPoint(shape2D.vertices[i].point);
    }

    // iterate through cross section vertices
    for (int i = 0; i < shape2D.LineCount; i += 2) {
      Vector3 a = localVerts[shape2D.lineIndices[i]];
      Vector3 b = localVerts[shape2D.lineIndices[i + 1]];

      Gizmos.DrawLine(a, b);
      Gizmos.DrawRay(a, localNormals[shape2D.lineIndices[i]]);
      Gizmos.DrawRay(b, localNormals[shape2D.lineIndices[i+1]]);
    }

    //Gizmos.DrawSphere(testPoint.LocalToWorld(Vector3.right * 0.3f), 0.02f);

    Gizmos.color = Color.white;
  }

} 