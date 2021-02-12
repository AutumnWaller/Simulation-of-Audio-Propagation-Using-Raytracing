using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAudioSources : MonoBehaviour
{
    public GameObject original;
    void Start()
    {
        for(int i = 1; i < 360; i++)
        {
            RT rt = original.AddComponent<RT>();
        }
    }

}
