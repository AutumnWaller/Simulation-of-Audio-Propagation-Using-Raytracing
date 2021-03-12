using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeFeed : MonoBehaviour
{
    public ComputeShader m_rtShader;
    public Texture2D m_skyboxTexture, m_grassTexture;
    private RenderTexture m_renderTarget;
    public Light m_lightSource;
    public GameObject m_wall;

    private Camera m_camera;
    private MeshRenderer m_wallMesh;
    private Bounds m_wallBounds;
    private void Awake()
    {
        m_camera = GetComponent<Camera>();
        m_wallMesh = m_wall.GetComponent<MeshRenderer>();
    }

    private void SetShaderData()
    {
        m_rtShader.SetMatrix("worldMat", m_camera.cameraToWorldMatrix);
        m_rtShader.SetMatrix("inverseProjMat", m_camera.projectionMatrix.inverse);
        m_rtShader.SetTexture(0, "skyboxTexture", m_skyboxTexture);
        Vector3 lForwards = m_lightSource.transform.forward;
        m_rtShader.SetVector("directionalLight", new Vector4(lForwards.x, lForwards.y, lForwards.z, m_lightSource.intensity));
        m_wallBounds = m_wallMesh.bounds;
        Vector3 min = m_wallBounds.min;
        Vector3 max = m_wallBounds.max;
        Vector4 minBounds = new Vector4(min.x, min.y, min.z, 0);
        Vector4 maxBounds = new Vector4(max.x, max.y, max.z, 0);

        m_rtShader.SetVector("minBounds", minBounds);
        m_rtShader.SetVector("maxBounds", maxBounds);
    }

    private void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {
        SetShaderData();
        Render(_destination);
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

}
