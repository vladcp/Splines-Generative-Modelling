using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class QuadGenerator : MonoBehaviour
{
    // defining 4 vertices (in a quad)
    Vector3[] vertices = new Vector3[]
    {
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
        new Vector3(-1, 0, -1)
    };

    // define normals for each vertex (pointing up)
    Vector3[] normals = new Vector3[]
    {
        new Vector3(0, 1, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 1, 0)
    };

    // 2 triangles defined
    int[] triangles = new int[]
    {
        2, 0, 1,
        2, 1, 3
    };

    // uv coordinates for texture mapping
    Vector2[] uvs = new Vector2[]
    {
        new Vector2(0, 1),
        new Vector2(0, 0),
        new Vector2(1, 1),
        new Vector2(1, 0)
    };

    private void Awake()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
            mf.sharedMesh = new Mesh();
        Mesh mesh = mf.sharedMesh;
        mesh.name = "Procedural Quad";

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }
    // getting a point on a spline 
    // cubic bezier curve equations simplified
    Vector3 GetPoint(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return pts[0] * (omt2 * omt) +
               pts[1] * (3f * omt2 * t) +
               pts[2] * (3f * omt * t2) +
               pts[3] * (t2 * t);
    }

    // return normalized tangent at a point t
    Vector3 GetTangent(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        Vector3 tangent =
            pts[0] * (-omt2) +
            pts[1] * (3 * omt2 - 2 * omt) +
            pts[2] * (-3 * t2 + 2 * t) +
            pts[3] * (t2);
        return tangent.normalized;
    }

    Vector3 GetNormal3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 binormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    }

    Quaternion GetOrientation3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 nrm = GetNormal3D(pts, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }
}
