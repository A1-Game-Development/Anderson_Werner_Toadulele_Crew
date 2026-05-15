using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleTriggerAudio : MonoBehaviour
{
    public string playerTag = "Player";

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public List<AudioClip> audioClips = new List<AudioClip>();

    private List<AudioClip> remainingClips = new List<AudioClip>();

    void Start()
    {
        ResetCycle();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayNextSound();
        }
    }

    void PlayNextSound()
    {
        if (audioSource == null || audioClips.Count == 0)
            return;

        // If we've used all clips, refill the list
        if (remainingClips.Count == 0)
        {
            ResetCycle();
        }

        int index = Random.Range(0, remainingClips.Count);
        AudioClip clip = remainingClips[index];

        remainingClips.RemoveAt(index);

        audioSource.PlayOneShot(clip);
    }

    void ResetCycle()
    {
        remainingClips = new List<AudioClip>(audioClips);
    }
}