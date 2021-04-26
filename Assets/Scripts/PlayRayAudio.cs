using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRayAudio : MonoBehaviour
{
    public void PlayAudio(float _distance){
        AudioManager.Play(_distance);
    }
}
