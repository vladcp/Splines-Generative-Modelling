using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMeshGenerator : MonoBehaviour
{
    Mesh mesh;
    // or simply generate vertices
    // i.e. Vector3[]

    void Awake() {
        mesh = new Mesh();
        mesh.name = "NoisyMesh";
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
