using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class AudioManagerMenu : MonoBehaviour
{
    public static AudioManagerMenu Instance;

    [SerializeField] private AK.Wwise.Event menuMusic;
    private List<string> menuScenes = new List<string> { "UI_Accueil", "Histoire", "LevelSelect", "Controls", "Lilou" };
    private bool isMusicPlaying = false;
    

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        if(IsMenuScene(scene.name) && !isMusicPlaying)
        {
            print("MUSIC PLAYING!");
            menuMusic.Post(gameObject);
            isMusicPlaying = true;
        }
    }

    private bool IsMenuScene(string sceneName)
    {
        if (menuScenes.Contains(sceneName))
        {
            return true;
        }
        return false;
    }

    public void StopMenuMusic()
    {
        menuMusic.Stop(gameObject);
        isMusicPlaying = false;
    }
}
