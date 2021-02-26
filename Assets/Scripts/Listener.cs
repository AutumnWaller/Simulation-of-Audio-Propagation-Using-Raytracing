using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Listener : MonoBehaviour
{
    AudioSource source;
    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }



    public void AudioRayHit(RT.AudioRay _ray)
    {
        if (source.isPlaying)
            return;
        print(_ray.distance);
        source.volume = 1 / (_ray.distance * _ray.distance);
        //print(_ray.currVolume);
        source.clip = _ray.clip;
        source.Play();

    }
}
