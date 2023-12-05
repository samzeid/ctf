using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    private static SFX main;

    public List<AudioClip> clips;
    private AudioSource audioSource;

    void Awake()
    {
        main = this;
        audioSource = GetComponent<AudioSource>();
    }

    public static void Play(int clipID)
    {
        if (main != null)
            main.PlayClip(clipID);
    }

    public void PlayClip(int clipID)
    {
        if (audioSource != null && clips.Count > clipID)
        {
            audioSource.PlayOneShot(clips[clipID]);
        }
    }
}