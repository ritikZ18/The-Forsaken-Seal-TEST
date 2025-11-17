using UnityEngine;
using System.Collections;

public class AmbientSound : MonoBehaviour
{
    //public Collider Area;        // The trigger area where sound should play
    public AudioSource source;    // The AudioSource that holds the ambient clip
    public AudioClip audioClip;
    //public GameObject Player;    // The Player object

    //private void Start()
    //{
    //    if (sound == null)
    //        sound = GetComponent<AudioSource>();
    //}

    void upadte()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            source.PlayOneShot(audioClip);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Play or fade in when Player enters the Area
    //    if (other.gameObject == Player || other.CompareTag("Player"))
    //    {
    //        if (!sound.isPlaying)
    //            StartCoroutine(FadeIn(sound, 2f));   // <-- fade in over 2 seconds
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    // Fade out or stop when Player leaves the Area
    //    if (other.gameObject == Player || other.CompareTag("Player"))
    //    {
    //        StartCoroutine(FadeOut(sound, 2f));      // <-- fade out over 2 seconds
    //    }
    //}

    //IEnumerator FadeIn(AudioSource source, float duration)
    //{
    //    float startVolume = 0f;
    //    source.volume = 0f;
    //    source.Play();

    //    while (source.volume < 1f)
    //    {
    //        source.volume += Time.deltaTime / duration;
    //        yield return null;
    //    }
    //}

    //IEnumerator FadeOut(AudioSource source, float duration)
    //{
    //    float startVolume = source.volume;

    //    while (source.volume > 0f)
    //    {
    //        source.volume -= Time.deltaTime / duration;
    //        yield return null;
    //    }

    //    source.Stop();
    //    source.volume = startVolume;
    //}
}
