using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio Propagation/Physics/Physical Material")]
public class PhysicalMaterial : ScriptableObject
{

    public float density;
    [Range(0, 100)]
    public int reflectivity;
    [Range(0, 100)]
    public int absorption;
    [Range(0, 100)]
    public int diffusion;

}