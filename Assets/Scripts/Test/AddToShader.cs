using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    [RequireComponent(typeof(MeshRenderer))]
public class AddToShader : MonoBehaviour
{
    SendToShader.MeshData data;
    void OnEnable(){
        data = new SendToShader.MeshData();
        data.vertices = new List<Vector3>();
        data.indices = new List<int>();
        data.normals = new List<Vector3>();
        Mesh m = GetComponent<MeshRenderer>().GetComponent<MeshFilter>().mesh;
        data.vertices.AddRange(m.vertices);
        data.indices.AddRange(m.GetIndices(0));
        data.normals.AddRange(m.normals);
        data.localToWorldMatrix = transform.localToWorldMatrix;
        SendToShader.AddMesh(data);
    }

    void OnDisable(){
        SendToShader.RemoveMesh(data);
    }
}
