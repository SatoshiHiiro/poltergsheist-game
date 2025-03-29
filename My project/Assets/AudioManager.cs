using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);   
        }
        DontDestroyOnLoad(this.gameObject);

    }
}
