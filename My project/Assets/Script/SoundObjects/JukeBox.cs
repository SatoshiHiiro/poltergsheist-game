using UnityEngine;
using UnityEngine.Rendering;

public class JukeBox : SoundDetection, IPossessable
{
    private AudioSource audioSource;
    private PossessionManager possessionManager;
    private bool isPlaying;
    
    private void Start()
    {
        objectType = SoundEmittingObject.SoundObject;
        isPlaying = false;
        audioSource = GetComponent<AudioSource>();
        possessionManager = GetComponent<PossessionManager>();
    }

    private void PlaySoundOnRepeat()
    {
        audioSource.Play();
        audioSource.loop = true;
        isPlaying = true;
    }
    public void OnDepossessed()
    {
        // No behaviour expected
    }

    public void OnPossessed()
    {
        if (!isPlaying)
        {
            PlaySoundOnRepeat();
            NotifyNearbyEnemies(this);
        }
    }

    // Stop the sound
    public override void ResetObject()
    {
        audioSource.Stop();
        isPlaying = false;
    }

    //public void StopSound()
    //{
    //    audioSource.Stop();
    //    isPlaying = false;
    //}
}
