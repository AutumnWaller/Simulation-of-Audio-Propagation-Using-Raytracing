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


    public List<Vector3> m_vertices = new List<Vector3>();
    public List<int> m_indices = new List<int>();
    public List<Vector3> m_normals = new List<Vector3>();
    public List<ObjectInfo> m_objects = new List<ObjectInfo>();
    public List<ObjectInfo> m_soundObjectInfo = new List<ObjectInfo>();

    public static List<MeshData> m_soundSourceObjects = new List<MeshData>();

    List<ShaderOutput> m_output = new List<ShaderOutput>();


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

    struct ShaderOutput{
        public Vector3 position;
    }

    public ComputeShader m_computeShader;
    ComputeBuffer m_bufferVertices;
    ComputeBuffer m_bufferIndices;
    ComputeBuffer m_bufferNormals;
    ComputeBuffer m_bufferObjectInfo;
    ComputeBuffer m_bufferSoundSources;
    ComputeBuffer m_bufferOutput;


    RenderTexture m_RT;

    private static List<MeshData> m_meshList = new List<MeshData>();
    private static bool m_isMeshListDirty = false;

    public Camera m_camera;

    void Awake(){
        m_camera = GetComponent<Camera>();

    }

    public static void AddMesh(MeshData _mesh){
        m_meshList.Add(_mesh);
        m_isMeshListDirty = true;
    }

    public static void RemoveMesh(MeshData _mesh){
        m_meshList.Remove(_mesh);
        m_isMeshListDirty = true;
    }

    public static void AddSourceMesh(MeshData _soundSource){
        m_soundSourceObjects.Add(_soundSource);
        m_isMeshListDirty = true;
    }

    public static void RemoveSourceMesh(MeshData _soundSource){
        m_soundSourceObjects.Remove(_soundSource);
        m_isMeshListDirty = true;
    }

    void SendMeshData(){
        m_vertices.Clear();
        m_indices.Clear();
        m_normals.Clear();
        m_objects.Clear();
        m_soundObjectInfo.Clear();
        m_output.Clear();

        int vertCount = 0, indexCount = 0;
        for(int i = 0; i < m_meshList.Count; i++){
            MeshData currMesh = m_meshList[i];
            m_vertices.AddRange(currMesh.vertices);
            m_indices.AddRange(currMesh.indices);
            m_normals.AddRange(currMesh.normals);
            m_objects.Add(new ObjectInfo(currMesh.localToWorldMatrix, indexCount, currMesh.indices.Count, vertCount, currMesh.vertices.Count));
            vertCount += currMesh.vertices.Count;
            indexCount += currMesh.indices.Count;
        }

        for(int i = 0; i < m_soundSourceObjects.Count; i++){
            MeshData currMesh = m_soundSourceObjects[i];
            m_vertices.AddRange(currMesh.vertices);
            m_indices.AddRange(currMesh.indices);
            m_normals.AddRange(currMesh.normals);
            m_soundObjectInfo.Add(new ObjectInfo(currMesh.localToWorldMatrix, indexCount, currMesh.indices.Count, vertCount, currMesh.vertices.Count));
            vertCount += currMesh.vertices.Count;
            indexCount += currMesh.indices.Count;
        }


        ShaderOutput output = new ShaderOutput();
        output.position = new Vector3();
        m_output.Add(output);
        if(m_bufferVertices != null)
            m_bufferVertices.Release();
        if(m_bufferIndices != null)
            m_bufferIndices.Release();
        if(m_bufferNormals != null)
            m_bufferNormals.Release();
        if(m_bufferObjectInfo != null)
            m_bufferObjectInfo.Release();
        if(m_bufferSoundSources != null)
            m_bufferSoundSources.Release();

        m_bufferVertices = new ComputeBuffer(m_vertices.Count, 12);
        m_bufferIndices = new ComputeBuffer(m_indices.Count, 4);
        m_bufferNormals = new ComputeBuffer(m_normals.Count, 12);
        m_bufferObjectInfo = new ComputeBuffer(m_objects.Count, 80);
        m_bufferSoundSources = new ComputeBuffer(m_soundObjectInfo.Count, 80);
        m_bufferOutput = new ComputeBuffer(m_output.Count, 12);

        m_bufferVertices.SetData(m_vertices);
        m_bufferIndices.SetData(m_indices);
        m_bufferNormals.SetData(m_normals);
        m_bufferObjectInfo.SetData(m_objects);
        m_bufferSoundSources.SetData(m_soundObjectInfo);
        m_bufferOutput.SetData(m_output);

        m_computeShader.SetBuffer(0, Shader.PropertyToID("Vertices"), m_bufferVertices);
        m_computeShader.SetBuffer(0, Shader.PropertyToID("Indices"), m_bufferIndices);
        m_computeShader.SetBuffer(0, Shader.PropertyToID("Normals"), m_bufferVertices);
        m_computeShader.SetBuffer(0, Shader.PropertyToID("Objects"), m_bufferObjectInfo);
        m_computeShader.SetBuffer(0, Shader.PropertyToID("SoundSourceObjects"), m_bufferSoundSources);
        m_computeShader.SetBuffer(0, Shader.PropertyToID("Outputs"), m_bufferOutput);

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
        Vector3[] positions = new Vector3[m_output.Count];
        m_bufferOutput.GetData(positions);
        for(int i = 0; i < m_output.Count; i++){
            ShaderOutput o = new ShaderOutput();
            o.position = positions[i];
            m_output[i] = o;
        }
    }

    void OnPostRender(){


    }

}
