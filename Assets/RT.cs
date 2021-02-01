using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RT : MonoBehaviour
{
    public ComputeShader rtShader;
    public Texture2D backgroundTexture;
    private RenderTexture renderTarget;

    List<Ray> rays;
    List<RaycastHit> hits;
    public int maxBounces = 10;
    void Start()
    {

    }

    void FixedUpdate()
    {
        rays = new List<Ray>();
        hits = new List<RaycastHit>();
        RaycastHit hit;
        Ray r = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(r, out hit, 100, ~8))
        {
            rays.Add(r);
            hits.Add(hit);
            RayTrace(r, hit, maxBounces);
        }
    }

    void OnDrawGizmos()
    {
        if (rays != null)
        {
            for(int i = 0; i < rays.Count - 1; i++)
            {
                Debug.DrawRay(rays[i].origin,  rays[i].direction.normalized * hits[i].distance, Color.Lerp(Color.green, Color.red, (float)i / rays.Count));
            }

        }
    }

    void RayTrace(Ray _prevRay, RaycastHit _prevHit, int _bouncesLeft)
    {
        if (_bouncesLeft <= 0)
            return;
        Ray r = new Ray(_prevHit.point,  Vector3.Reflect(_prevRay.direction.normalized, _prevHit.normal));
        rays.Add(r);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, 100, ~8))
        {
            hits.Add(hit);
            RayTrace(r, hit, _bouncesLeft - 1);
        }
    }

    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    CreateRenderTarget();
    //    SetShader();
    //    Graphics.Blit(renderTarget, destination);
    //}

    //private void CreateRenderTarget()
    //{
    //    if (renderTarget != null)
    //    {
    //        renderTarget.Release();
    //    }
    //    renderTarget = new RenderTexture(640, 480, 0);
    //    renderTarget.enableRandomWrite = true;
    //    renderTarget.Create();
    //}

    //private void SetShader()
    //{
    //    rtShader.SetTexture(0, "target", renderTarget);
    //    rtShader.SetTexture(1, "background", backgroundTexture);
    //    rtShader.Dispatch(0, 640, 480, 1);
    //}
}
