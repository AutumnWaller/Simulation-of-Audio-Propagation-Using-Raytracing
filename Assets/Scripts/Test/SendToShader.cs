using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToShader : MonoBehaviour
{
    public struct MeshData{
        public List<Vector3> vertices;
        public List<int> indices;
        public List<Vector3> normals;
        public Matrix4x4 localToWorldMatrix;
    }

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> indices = new List<int>();
    public List<Vector3> normals = new List<Vector3>();
    public List<ObjectInfo> objects = new List<ObjectInfo>();

    public struct ObjectInfo
    {
        public Matrix4x4 localToWorldMatrix;
        public int startingIndex;
        public int indexCount;
        public int startingVertex;
        public int vertexCount;
        public ObjectInfo(Matrix4x4 _localToWorldMatrix, int _startingIndex, int _indexCount, int _startingVertex, int _vertexCount){

            localToWorldMatrix = _localToWorldMatrix;
            startingIndex = _startingIndex;
            indexCount = _indexCount;
            startingVertex = _startingVertex;
            vertexCount = _vertexCount;
        }
    }

    public ComputeShader m_computeShader;
    ComputeBuffer m_bufferVertices;
    ComputeBuffer m_bufferIndices;
    ComputeBuffer m_bufferNormals;
    ComputeBuffer m_bufferObjectInfo;

    RenderTexture m_RT;

    private static List<MeshData> m_meshList = new List<MeshData>();
    private static bool m_isMeshListDirty = false;

    public Camera m_camera;

    void Awake(){
    }

    public static void AddMesh(MeshData _mesh){
        m_isMeshListDirty = true;
        m_meshList.Add(_mesh);
    }

    public static void RemoveMesh(MeshData _mesh){
        m_isMeshListDirty = true;
        m_meshList.Remove(_mesh);
    }

    void SendMeshData(){
        vertices.Clear();
        indices.Clear();
        normals.Clear();
        objects.Clear();

        int vertCount = 0, indexCount = 0;
        for(int i = 0; i < m_meshList.Count; i++){
            MeshData currMesh = m_meshList[i];
            vertices.AddRange(currMesh.vertices);
            indices.AddRange(currMesh.indices);
            normals.AddRange(currMesh.normals);
            objects.Add(new ObjectInfo(currMesh.localToWorldMatrix, indexCount, currMesh.indices.Count, vertCount, currMesh.vertices.Count));
            vertCount += currMesh.vertices.Count;
            indexCount += currMesh.indices.Count;
        }

        if(m_bufferVertices != null)
            m_bufferVertices.Release();
        if(m_bufferIndices != null)
            m_bufferIndices.Release();
        if(m_bufferNormals != null)
            m_bufferNormals.Release();
        if(m_bufferObjectInfo != null)
            m_bufferObjectInfo.Release();

        m_bufferVertices = new ComputeBuffer(vertices.Count, 12);
        m_bufferIndices = new ComputeBuffer(indices.Count, 4);
        m_bufferNormals = new ComputeBuffer(normals.Count, 12);
        m_bufferObjectInfo = new ComputeBuffer(objects.Count, 80);



        m_bufferVertices.SetData(vertices);
        m_bufferIndices.SetData(indices);
        m_bufferNormals.SetData(normals);
        m_bufferObjectInfo.SetData(objects);

        m_computeShader.SetBuffer(0, Shader.PropertyToID("Vertices"), m_bufferVertices);
        m_computeShader.SetBuffer(0, Shader.PropertyToID("Indices"), m_bufferIndices);
        m_computeShader.SetBuffer(0, Shader.PropertyToID("Normals"), m_bufferVertices);
        m_computeShader.SetBuffer(0, Shader.PropertyToID("Objects"), m_bufferObjectInfo);


    }

    private void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {
        if(m_RT == null || m_RT.width != Screen.width || m_RT.height != Screen.height)
        {
            if (m_RT != null)
                m_RT.Release();
            m_RT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            m_RT.enableRandomWrite = true;
            m_RT.Create();
        }
        if(m_isMeshListDirty){
            SendMeshData();
        }
        m_computeShader.SetMatrix("WorldMatrix", m_camera.cameraToWorldMatrix);
        m_computeShader.SetMatrix("InverseProjectionMatrix", m_camera.projectionMatrix.inverse);
        m_computeShader.SetTexture(0, "Source", _source);
        m_computeShader.SetTexture(0, "Result", m_RT);
        m_computeShader.Dispatch(0, Mathf.CeilToInt(Screen.width / 8), Mathf.CeilToInt(Screen.height / 8), 1);
        Graphics.Blit(m_RT, _destination);

    }

}
