using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource myAudioSource;
    private int songIndex = 0;
    [SerializeField] private AudioClip[] myAudioClipArray;

    // Start is called before the first frame update
    void Awake()
    {
        
        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
          
            DontDestroyOnLoad(gameObject);
        }

        myAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
      
        if (!myAudioSource.isPlaying)
        {
          
            myAudioSource.clip = myAudioClipArray[songIndex];
            myAudioSource.Play();

            if (songIndex < myAudioClipArray.Length - 1)
            {
                songIndex++;
            }
            else
            {
                songIndex = 0;
            }
        }
    }
}
