using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OrientedPoint {

    public Vector3 pos;
    public Quaternion rot;

    public OrientedPoint(Vector3 pos, Quaternion rot) {
        this.pos = pos;
        this.rot = rot;
    }

    public OrientedPoint(Vector3 pos, Vector3 forward) {
        this.pos = pos;
        this.rot = Quaternion.LookRotation(forward);
    }

    // given a point in the local Frenet frame
    // return its position in world space
    public Vector3 LocalToWorldPos(Vector3 localSpacePos) {
        return pos + rot * localSpacePos;
    }

    // for normal proper orientation
    public Vector3 LocalToWorldVec(Vector3 dir) {
        return rot * dir;
    }

    public Vector3 WorldToLocalPos(Vector3 worldPos) {
        return Quaternion.Inverse(rot) * (worldPos - pos);
    }
}
