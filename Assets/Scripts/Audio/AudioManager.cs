using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AudioManager : MonoBehaviour
{

    AudioSource audioSource;

    [SerializeField] AudioSO audioSO;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayTrack(AUDIOCLIPS clipName)
    {
        audioSource.clip = audioSO.GetAudioClip(clipName);
        audioSource.Play();
    }


}
