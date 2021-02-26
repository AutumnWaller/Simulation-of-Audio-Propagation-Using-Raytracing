using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RT : MonoBehaviour
{
    public struct AudioRay
    {
        public Ray currRay;
        public AudioClip clip;
        public Vector3 origin;
        public float distance;
        public float energy;
    }

    //Cache the distance from the source, the amount of bounces, the properties of those bounces into a texture.
    public ComputeShader rtShader;
    public Texture2D backgroundTexture;
    public Text display;
    private RenderTexture renderTarget;

    public AudioClip clip;
    public GameObject player;
    public Listener listener;
    List<AudioRay> rays;
    List<RaycastHit> hits;
    public int max = 500;
    private int maxBounces = 500;
    public Vector3 direction = Vector3.zero;

    private void Start()
    {
        listener = player.GetComponent<Listener>();
        Random.InitState((int)Time.time * 27389);
    }

    void FixedUpdate()
    {
        direction = transform.forward;
        display.text = transform.forward.ToString();
        maxBounces = max;
        rays = new List<AudioRay>();
        hits = new List<RaycastHit>();
        RaycastHit hit;
        Ray r = new Ray(transform.position, direction);
        AudioRay ar = new AudioRay();
        ar.currRay = r;
        ar.clip = clip;
        ar.origin = transform.position;
        ar.distance = 0;
        ar.energy = 100;
        float dist = Vector3.Distance(ar.origin, player.transform.position);
        
        if (Physics.Raycast(r, out hit, 100, ~8))
        {
            rays.Add(ar);
            hits.Add(hit);
            RayTrace(ar, hit, maxBounces);
            //display.text = hits.Count.ToString();
        }
    }

    void OnDrawGizmos()
    {
        if (rays != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(transform.position, Vector3.one);
            for (int i = 0; i < rays.Count - 1; i++)
            {
                Debug.DrawRay(rays[i].currRay.origin,  rays[i].currRay.direction.normalized * hits[i].distance, Color.Lerp(Color.green, Color.red, rays[i].energy));
            }
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(rays[rays.Count - 1].currRay.origin, Vector3.one);

        }
    }



    void RayTrace(AudioRay _ray, RaycastHit _prevHit, int _bouncesLeft)
    {
        if (_bouncesLeft <= 0)
            return;
        Ray r = new Ray(_prevHit.point,  Vector3.Reflect(_ray.currRay.direction.normalized, _prevHit.normal));
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, 100, ~8))
        {
            Wall wall;
            if ((wall = hit.transform.GetComponent<Wall>()) != null)
            {
                float energyLost = _ray.energy * (wall.physics.material.absorption * 0.01f);
                print($"Energy lost: {energyLost}");
                _ray.energy -= energyLost;
            }

            if (hit.transform.tag == "Player")
            {
                _ray.distance += Vector3.Distance(hit.point, transform.position);
                Listener l = hit.transform.GetComponent<Listener>();
                l.AudioRayHit(_ray);
                return;
            }
            AudioRay newRay = new AudioRay();
            newRay.currRay = r;
            newRay.clip = _ray.clip;
            newRay.origin = hit.point;
            newRay.energy = _ray.energy;
            float dist = Vector3.Distance(hit.point, transform.position);
            newRay.distance = _ray.distance + dist;
            _ray.clip = null;
            rays.Add(newRay);
            hits.Add(hit);
            RayTrace(newRay, hit, _bouncesLeft - 1);
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
