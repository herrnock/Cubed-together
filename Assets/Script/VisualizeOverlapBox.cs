using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeOverlapBox : MonoBehaviour
{
    public Vector3 center;
    public Vector3 halfExtents;
    public Quaternion orientation = Quaternion.identity;
    public Color color = Color.red;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, halfExtents * 2);
    }
}