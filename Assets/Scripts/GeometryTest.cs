using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GeometryTest : MonoBehaviour
{
    public GameObject go;
    Bounds bounds;
    private void Awake()
    {
    }

    private void OnDrawGizmos()
    {
        bounds = go.GetComponent<MeshRenderer>().bounds;
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(bounds.min, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(bounds.max, Vector3.one);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(go.transform.position, go.transform.localScale);
    }

}
