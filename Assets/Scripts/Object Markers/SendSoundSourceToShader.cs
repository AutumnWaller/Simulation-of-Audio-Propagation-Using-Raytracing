using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendSoundSourceToShader : MonoBehaviour
{

    ShaderDispatcher.MeshData m_data;

    void OnEnable(){
        m_data = new ShaderDispatcher.MeshData();
        m_data.vertices = new List<Vector3>();
        m_data.indices = new List<int>();
        m_data.normals = new List<Vector3>();
        Mesh m = GetComponent<MeshRenderer>().GetComponent<MeshFilter>().mesh;
        m_data.vertices.AddRange(m.vertices);
        m_data.indices.AddRange(m.GetIndices(0));
        m_data.normals.AddRange(m.normals);
        m_data.localToWorldMatrix = transform.localToWorldMatrix;
        ShaderDispatcher.AddSourceMesh(m_data);
    }

    void OnDisable(){
        ShaderDispatcher.RemoveSourceMesh(m_data);
    }
}
