using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mathfs {
    public static Vector2 GetVectorByAngle(float angRad) {
        return new Vector2(
            Mathf.Cos(angRad),
            Mathf.Sin(angRad));
    }

    public const float TAU = 6.28318530718f;
}
