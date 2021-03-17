using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComputeFeed : MonoBehaviour
{
    public ComputeShader m_rtShader;
    public Texture2D m_skyboxTexture, m_grassTexture;
    private RenderTexture m_renderTarget;
    public Light m_lightSource;

    private Camera m_camera;

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    private void SetShaderData()
    {
        m_rtShader.SetMatrix("worldMat", m_camera.cameraToWorldMatrix);
        m_rtShader.SetMatrix("inverseProjMat", m_camera.projectionMatrix.inverse);
        m_rtShader.SetTexture(0, "skyboxTexture", m_skyboxTexture);
        Vector3 lForwards = m_lightSource.transform.forward;
        m_rtShader.SetVector("directionalLight", new Vector4(lForwards.x, lForwards.y, lForwards.z, m_lightSource.intensity));
        SetComputeBuffer("meshObjects", m_meshObjectBuffer);
        SetComputeBuffer("vertices", m_vertexBuffer);
        SetComputeBuffer("indices", m_indexBuffer);
    }

    private void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {
        RebuildMeshBuffers();
        SetShaderData();
        Render(_destination);
    }

    public static List<RTObject> m_rtObjects = new List<RTObject>();
    struct MeshObject
    {
        public Matrix4x4 localToWorldMatrix;
        public int indicesOffset;
        public int indicesCount;
    }

    static List<MeshObject> m_meshObjects = new List<MeshObject>();
    static List<Vector3> m_vertices = new List<Vector3>();
    static List<int> m_indices = new List<int>();
    static bool meshNeedsRebuilding = false;
    ComputeBuffer m_meshObjectBuffer;
    ComputeBuffer m_vertexBuffer;
    ComputeBuffer m_indexBuffer;

    public static void RegisterObject(RTObject _obj)
    {
        m_rtObjects.Add(_obj);
        meshNeedsRebuilding = true;
    }

    public static void UnRegisterObject(RTObject _obj)
    {
        m_rtObjects.Remove(_obj);
        meshNeedsRebuilding = true;
    }

    private static void CreateComputeBuffer<T>(ref ComputeBuffer _buffer, List<T> _data, int _stride)
    where T : struct
    {
        if(_buffer != null){
            if(_data.Count == 0 || _buffer.count != _data.Count || _buffer.stride != _stride){
                _buffer.Release();
                _buffer = null;
            }
        }
        if(_data.Count != 0){
            if(_buffer == null){
                _buffer = new ComputeBuffer(_data.Count, _stride);
            }
            _buffer.SetData(_data);
        }
    }

    void SetComputeBuffer(string _name, ComputeBuffer _buffer){
        if(_buffer != null){
            m_rtShader.SetBuffer(0, _name, _buffer);
        }
    }

    private void RebuildMeshBuffers()
    {
        if (!meshNeedsRebuilding)
            return;
        meshNeedsRebuilding = false;
        m_meshObjects.Clear();
        m_vertices.Clear();
        m_indices.Clear();

        foreach (RTObject rto in m_rtObjects)
        {
            Mesh m = rto.GetComponent<MeshFilter>().sharedMesh;

            int firstVert = m_vertices.Count;
            m_vertices.AddRange(m.vertices);
            int firstIndex = m_indices.Count;
            int[] indices = m.GetIndices(0);
            m_indices.AddRange(indices.Select(index => index + firstIndex));

            m_meshObjects.Add(new MeshObject(){
                localToWorldMatrix = rto.transform.localToWorldMatrix,
                indicesOffset = firstIndex,
                indicesCount = indices.Length
            });

        }
        CreateComputeBuffer(ref m_meshObjectBuffer, m_meshObjects, 72);
        CreateComputeBuffer(ref m_vertexBuffer, m_vertices, (4 * 3));
        CreateComputeBuffer(ref m_indexBuffer, m_indices, 4);
    }

    private void SetRenderTarget()
    {
        if(m_renderTarget == null || m_renderTarget.width != Screen.width || m_renderTarget.height != Screen.height)
        {
            if (m_renderTarget != null)
                m_renderTarget.Release();
            m_renderTarget = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            m_renderTarget.enableRandomWrite = true;
            m_renderTarget.Create();
        }
    }

    private void Render(RenderTexture _renderDestination)
    {
        SetRenderTarget();
        m_rtShader.SetTexture(0, "Result", m_renderTarget);
        m_rtShader.Dispatch(0, Mathf.CeilToInt(Screen.width / 8), Mathf.CeilToInt(Screen.height / 8), 1);
        Graphics.Blit(m_renderTarget, _renderDestination);
    }

    void OnDisable(){
        m_indexBuffer.Release();
        m_vertexBuffer.Release();
        m_meshObjectBuffer.Release();
    }

}
