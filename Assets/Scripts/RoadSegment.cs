using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RoadSegment : MonoBehaviour
{

  [SerializeField] Transform[] controlPoints = new Transform[4];
  [SerializeField] ExtrudeShape shape2D;
  [SerializeField] RingExtrudeShape ring2D;
  [SerializeField] RingExtrudeShapeClass extrudeShape2D;

  
  [SerializeField] bool addNoise = false;
  [SerializeField] Color baseColor = Color.green;

  Mesh mesh;
  Material[] material;

  Vector3 GetPos(int i) => controlPoints[i].position;
  Vector3[] pts => new Vector3[] { GetPos(0), GetPos(1), GetPos(2), GetPos(3) };

  [Range(.5f, 5f)]
  [SerializeField]
  private float radius = 3f;

  public float Radius {
    get {return radius;}
    set {
      radius = value;
      // if (extrudeShape2D != null) {
      extrudeShape2D.Radius = radius;
      Extrude(mesh, extrudeShape2D, GetOrientedPoints(pts, segmentCount, tTest));
      // }
    }
  }
  [Range(0, 1)]
  [SerializeField] float tTest = 0;
  
  public float T {
    get { return tTest; }
    set {
      tTest = value;
      if (extrudeShape2D != null) {
        Extrude(mesh, extrudeShape2D, GetOrientedPoints(pts, segmentCount, tTest));
      }
    }
  }

  [Range(2, 32)] 
  [SerializeField]
  private int segmentCount = 3;
 
  public int SegmentCount {
    get { return segmentCount; }
    set { 
      segmentCount = value; 
      if (extrudeShape2D != null) {
        Extrude(mesh, extrudeShape2D, GetOrientedPoints(pts, segmentCount, tTest));
      }
    }
  }

  void OnValidate() {
    SegmentCount = segmentCount;
  }

  void Awake() {

    mesh = new Mesh();
    mesh.name = "Segment";

    GetComponent<MeshFilter>().sharedMesh = mesh;
    
    material = GetComponent<MeshRenderer>().materials;
    material[0].color = baseColor;

    extrudeShape2D = new RingExtrudeShapeClass(radius);
    Extrude(this.mesh, extrudeShape2D, GetOrientedPoints(pts, segmentCount, tTest));
  }

  private void Update() {
    // if(addNoise) {
    //   ExtrudeWithNoise(this.mesh, shape2D, GetOrientedPoints(pts, edgeCount));
    // } else {
    //   Extrude(this.mesh, ring2D, GetOrientedPoints(pts, edgeCount));
    // }
    // printPoint();
  }

  // Get all oriented points along curve
  OrientedPoint[] GetOrientedPoints(Vector3[] controlPoints, int segmentCount) {
    OrientedPoint[] path = new OrientedPoint[segmentCount];
    for (int i = 0; i < segmentCount; i++) {
      float t = i / (segmentCount - 1f);
      path[i] = BezierFs.GetOrientedPoint(controlPoints, t, Vector3.up);
    }
    return path;
  }

  // get all oriented points up to some point t
  OrientedPoint[] GetOrientedPoints(Vector3[] controlPoints, int segmentCount, float maxT) {
    // OrientedPoint[] path = new OrientedPoint[segmentCount];
    List<OrientedPoint> path = new List<OrientedPoint>();
    for (int i = 0; i < segmentCount; i++) {
      float t = i / (segmentCount - 1f);
      if (t <= maxT) {
        path.Add(BezierFs.GetOrientedPoint(controlPoints, t, Vector3.up));
      } else break;
    }
    return path.ToArray();
  }

  void Extrude(Mesh mesh, RingExtrudeShapeClass shape, OrientedPoint[] path) {
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

    int multiplier = 1;
    for (int i = 0; i < path.Length; i++) {
      int offset = i * vertsInShape;
      RingExtrudeShapeClass currentShape = new RingExtrudeShapeClass(radius);
      for (int j = 0; j < vertsInShape; j++) {
        int id = offset + j;
        vertices[id] = path[i].LocalToWorldPos(currentShape.vertices[j].point);
        normals[id] = path[i].LocalToWorldVec(currentShape.vertices[j].normal);
        colors[id] = baseColor;
        // uv
      }
      // multiplier += 1;
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

    //GizmosDrawShape2D(extrudeShape2D, testPoint);

    //Gizmos.DrawSphere(testPoint.LocalToWorld(Vector3.right * 0.3f), 0.02f);

    Gizmos.color = Color.white;
  }

  void GizmosDrawShape2D(RingExtrudeShapeClass shape2D, OrientedPoint testPoint){
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
  }

} 