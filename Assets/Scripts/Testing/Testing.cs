using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Testing : MonoBehaviour
{

    private bool m_isRecording = false;
    private bool m_isWriting = false;
    public List<float> m_fps = new List<float>();
    private void Update() {
        if(Input.GetKeyUp(KeyCode.Return)){
            StartFunctions();
        }
        if(Input.GetKeyUp(KeyCode.Escape)){
            m_isRecording = false;
            m_isWriting = false;
        }
    }


    void StartFunctions(){
                    // LogToFile.ToggleRecording("A.txt");
            Debug.Log("Toggled recording");
            if(!m_isRecording)
            {
                m_isRecording = true;
                StartCoroutine("RecordData");
            }else{
                m_isRecording = false;
                m_isWriting = true;
                StartCoroutine("StoreData", "A.csv");
            }
    }

    private IEnumerator RecordData(){
        float[] times = new float[5];
        float average = 0.0f;
        while(m_isRecording){
            for(int i = 0; i < 5; i++){
                yield return new WaitForSeconds(0.2f);
                times[i] = 1 / Time.smoothDeltaTime;
            }
            average = 0.0f;
            for(int j = 0; j < 5; j++){
                average += times[j];
            }
            average /= 5;
            m_fps.Add(average);
            Debug.Log(m_fps.Count);
        }
        yield return null;
    }

    private IEnumerator StoreData(string _filePath){
        if(m_isWriting){
            string path = (string)_filePath;
            if(path == null){
                m_isWriting = false;
            }
            List<string> data = new List<string>();
            data.Add("," + "Frames Per Second");
            float average = 0.0f;
            for(int i = 0; i < m_fps.Count; i++){
                average += m_fps[i];
                data.Add((i+1).ToString() + "," + m_fps[i].ToString());
            }
            data.Add("Average" + "," + average / m_fps.Count);

            File.WriteAllLines(path, data);
            m_isWriting = false;
            m_fps.Clear();
            Debug.Log("Writing completed");
        }
        yield return null;
    }

}
