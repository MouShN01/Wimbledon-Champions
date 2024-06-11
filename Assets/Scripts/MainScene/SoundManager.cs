using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SoundManager : NetworkBehaviour
{
    public AudioSource fanR;
    public AudioSource fanL;
    public AudioSource whistle;
    public AudioClip calm;
    public AudioClip huray;
    public bool hurayIsPlaying = false;
    
    // Start is called before the first frame update
    void Start()
    {
        fanR = GameObject.Find("RightFans").GetComponent<AudioSource>();
        fanL = GameObject.Find("LeftFans").GetComponent<AudioSource>();
        whistle = GameObject.Find("Ring").GetComponent<AudioSource>();
        fanL.clip = calm;
        fanR.clip = calm;
        fanL.loop = true;
        fanR.loop = true;
    }

    public void PlayGoal()
    {
        if (!hurayIsPlaying)
        {
            hurayIsPlaying = true;
            fanL.loop = false;
            fanR.loop = false;
            whistle.Play();
            fanL.clip = huray;
            fanR.clip = huray;
            fanL.Play();
            fanR.Play();
            StartCoroutine(ReturnToLoopClipAfter(huray.length));
        }
    }
    
    IEnumerator ReturnToLoopClipAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        fanL.clip = calm;
        fanR.loop = true;  // Включаем режим циклического воспроизведения
        fanL.loop = true;  // Включаем режим циклического воспроизведения
        fanR.clip = calm;
        fanL.Play();
        fanR.Play();
        hurayIsPlaying = false;
    }
}
