using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RTObject : MonoBehaviour
{
    private void OnEnable()
    {
        ComputeFeed.RegisterObject(this);
    }

    private void OnDisable()
    {
        ComputeFeed.UnRegisterObject(this);
    }

}
