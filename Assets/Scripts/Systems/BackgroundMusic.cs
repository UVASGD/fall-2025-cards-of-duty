using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioClip[] musicClips = Resources.LoadAll<AudioClip>("bgmusic");
        if (musicClips.Length == 0)
        {
            Debug.LogError("No music clips found");
            return;
        }
        
        AudioClip selectedClip = musicClips[Random.Range(0, musicClips.Length)];
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = selectedClip;
        audioSource.loop = true;
        audioSource.Play();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
