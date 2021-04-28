using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public WallPhysics physics;

    private void Start()
    {
        physics.width = transform.localScale.x;
        physics.height = transform.localScale.y;
        physics.length = transform.localScale.z;
        float volume = physics.length * physics.width * physics.height;
        physics.mass = physics.material.density * volume;
    }

}
