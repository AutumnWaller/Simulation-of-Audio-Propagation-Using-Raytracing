using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSoundSource : MonoBehaviour
{

    SendToShader.SoundSource m_source;

    void OnEnable(){
        m_source = new SendToShader.SoundSource();
        m_source.direction = transform.forward;
        m_source.localToWorldMatrix = transform.localToWorldMatrix;
        SendToShader.AddSource(m_source);
    }

    void OnDisable(){
        SendToShader.RemoveSource(m_source);
    }
}
