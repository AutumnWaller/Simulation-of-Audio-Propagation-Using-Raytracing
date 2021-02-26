using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio Propagation/Physics/Wall Physics")]
public class WallPhysics : ScriptableObject
{
    public PhysicalMaterial material;
    public float width = 1;
    public float length = 1;
    public float height = 1;
    public float mass = 1;

}
