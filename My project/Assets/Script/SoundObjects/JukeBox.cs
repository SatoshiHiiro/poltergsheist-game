using UnityEngine;

public class JukeBox : SoundDetection, IPossessable
{
    private AudioSource audioSource;
    private PossessionManager possessionManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        possessionManager = GetComponent<PossessionManager>();
    }

    private void PlaySoundOnRepeat()
    {
        audioSource.Play();
        audioSource.loop = true;
    }
    public void OnDepossessed()
    {
        //
    }

    public void OnPossessed()
    {
        // TODO EMPËCHER MULTIPLE POSSESSION UNE FOIS QUE C'EST POSSÉDÉ
        //if (!possessionManager.IsPossessed)
        //{
        //    PlaySoundOnRepeat();
        //}
    }
}
