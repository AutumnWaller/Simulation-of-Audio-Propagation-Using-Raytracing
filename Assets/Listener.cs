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
        source.volume = 20 * Mathf.Log10(_ray.distanceFromSource / 1.0f);
        //print(_ray.currVolume);
        source.clip = _ray.clip;
        source.Play();
    }
}
