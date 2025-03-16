using UnityEngine;
using UnityEngine.Rendering;

public class JukeBox : SoundDetection, IPossessable, IResetObject
{
    private PossessionManager possessionManager;
    private bool isPlaying;
    
    protected override void Start()
    {
        base.Start();
        objectType = SoundEmittingObject.SoundObject;
        isPlaying = false;
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
    public void ResetObject()
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
