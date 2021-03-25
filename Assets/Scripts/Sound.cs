using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public AudioClip audioClipBtnClick;
    public AudioClip audioFanfare;
    private AudioSource audioSource;

    private static Sound instance;
    public static Sound Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Sound>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SoundBtnClick()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClipBtnClick;
        audioSource.Play();
    }

    public void SoundFanfare()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioFanfare;
        audioSource.Play();
    }
}
