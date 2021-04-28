using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    [RequireComponent(typeof(MeshRenderer))]
public class SendMeshToShader : MonoBehaviour
{
    ShaderDispatcher.MeshData data;
    void OnEnable(){
        data = new ShaderDispatcher.MeshData();
        data.vertices = new List<Vector3>();
        data.indices = new List<int>();
        data.normals = new List<Vector3>();
        Mesh m = GetComponent<MeshRenderer>().GetComponent<MeshFilter>().mesh;
        data.vertices.AddRange(m.vertices);
        data.indices.AddRange(m.GetIndices(0));
        data.normals.AddRange(m.normals);
        data.localToWorldMatrix = transform.localToWorldMatrix;
        ShaderDispatcher.AddMesh(data);
    }

    void OnDisable(){
        ShaderDispatcher.RemoveMesh(data);
    }
}
