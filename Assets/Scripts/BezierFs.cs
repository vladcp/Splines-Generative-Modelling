using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BezierFs {

    // get the position of the point on the curve at 'time' t
    public static Vector3 GetPoint(Vector3[] pts, float t) {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return (pts[0] * (omt2 * omt) +
               pts[1] * (3f * omt2 * t) +
               pts[2] * (3f * omt * t2) +
               pts[3] * (t2 * t));
    }

    // get the tangent at time t
    public static Vector3 GetTangent(Vector3[] pts, float t) {
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

    // get the normal at time 't'
    // the up vector is the point of reference
    public static Vector3 GetNormal3D(Vector3[] pts, float t, Vector3 up) {
        Vector3 tng = GetTangent(pts, t);
        Vector3 binormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    }

    // get orientation at a point
    // always align the normal with the up vector, hence no twists or turns
    public static Quaternion GetOrientation3D(Vector3[] pts, float t, Vector3 up) {
        Vector3 tng = GetTangent(pts, t);
        Vector3 nrm = GetNormal3D(pts, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }
    /// <summary>
    /// A method that returns a frenet frame, given a bezier curve, a time value, and a reference up vector
    /// </summary>
    /// <param name="pts">World positions of the Bezier curve control points</param>
    /// <param name="t">'Time' parameter along the curve</param>
    /// <param name="up">A reference vector</param>
    /// <returns></returns>
    public static OrientedPoint GetOrientedPoint(Vector3[] pts, float t, Vector3 up) {
        Vector3 pos = GetPoint(pts, t);
        //Vector3 tangent = GetTangent(pts, t);
        Quaternion rot = GetOrientation3D(pts, t, up);

        return new OrientedPoint(pos, rot);
    }
}
